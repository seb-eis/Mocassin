//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	EnvRoutines.c        		//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Main simulation routines    //
//////////////////////////////////////////

#include "Simulator/Logic/Routines/Main/MainRoutines.h"
#include "Simulator/Logic/Constants/Constants.h"
#include "Simulator/Data/SimContext/ContextAccess.h"
#include "Framework/Basic/FileIO/FileIO.h"
#include "Simulator/Logic/Objects/JumpSelection.h"
#include "Simulator/Logic/Routines/Environment/EnvRoutines.h"
#include "Simulator/Logic/Routines/Helper/HelperRoutines.h"
#include "Simulator/Logic/Routines/Statistics/McStatistics.h"
#include "Simulator/Logic/Initializers/ContextInit/ContextInit.h"
#include "Simulator/Logic/Routines/Tracking/TransitionTracking.h"
#include "Framework/Basic/Macros/BinarySearch.h"

void PrepareForMainRoutine(__SCONTEXT_PAR)
{
    PrepareContextForSimulation(SCONTEXT);
}

error_t ResetContextAfterPrerun(__SCONTEXT_PAR)
{
    return_if(!JobInfoFlagsAreSet(SCONTEXT, INFO_FLG_USEPRERUN), ERR_OK);

    if(!StateFlagsAreSet(SCONTEXT, STATE_FLG_PRERUN_RESET))
    {
        setMainStateFlags(SCONTEXT, STATE_FLG_PRERUN_RESET);
    }

    return ERR_NOTIMPLEMENTED;
}

error_t StartMainRoutine(__SCONTEXT_PAR)
{
    runtime_assertion(StateFlagsAreSet(SCONTEXT, STATE_FLG_SIMERROR), SIMERROR, "Cannot start main simulation routine, state error flag is set.")

    if (JobInfoFlagsAreSet(SCONTEXT, INFO_FLG_KMC))
    {
        if (StateFlagsAreSet(SCONTEXT, STATE_FLG_PRERUN))
        {
            SIMERROR = StartMainKmcRoutine(SCONTEXT);
            error_assert(SIMERROR, "Pre-run execution of main KMC routine aborted with an error");

            SIMERROR = FinishRoutinePreRun(SCONTEXT);
            error_assert(SIMERROR, "Pre-run finish of KMC main routine failed.")
        }
        
        return StartMainKmcRoutine(SCONTEXT);
    }

    if (JobInfoFlagsAreSet(SCONTEXT, INFO_FLG_MMC))
    {
        if (StateFlagsAreSet(SCONTEXT, STATE_FLG_PRERUN))
        {
            SIMERROR = StartMainMmcRoutine(SCONTEXT);
            error_assert(SIMERROR, "Prerun execution of main KMC routine aborted with an error");

            SIMERROR = FinishRoutinePreRun(SCONTEXT);
            error_assert(SIMERROR, "Prerun finish of KMC main routine failed.")
        }
        
        return StartMainKmcRoutine(SCONTEXT);
    }

    error_assert(ERR_UNKNOWN, "Main routine starter skipped selection process. Neither MMC nor KMC flags is set.");
    return ERR_UNKNOWN; // GCC [-Wall] expects return value, even with exit(..) statement
}

// Finishes the routine pre-run and resets the context for the main simulation routine
error_t FinishRoutinePreRun(__SCONTEXT_PAR)
{
    SIMERROR = SaveSimulationState(SCONTEXT);
    error_assert(SIMERROR, "State save after pre-run completion failed.");

    SIMERROR = ResetContextAfterPrerun(SCONTEXT);
    error_assert(SIMERROR, "Context reset after pre-run completion failed.");

    UnsetMainStateFlags(SCONTEXT, STATE_FLG_PRERUN);
    return SIMERROR;
}

// Starts the main kinetic simulation routine
error_t StartMainKmcRoutine(__SCONTEXT_PAR)
{
    while (GetKmcAbortCondEval(SCONTEXT) == STATE_FLG_CONTINUE)
    {
        SIMERROR = DoNextKmcCycleBlock(SCONTEXT);
        error_assert(SIMERROR, "Simulation abort due to error in KMC cycle block execution.");

        SIMERROR = FinishKmcCycleBlock(SCONTEXT);
        error_assert(SIMERROR, "Simulation abort due to error in KMC cycle block finisher execution.");
    }

    return FinishMainRoutineKmc(SCONTEXT);
}

// Starts the main metropolis simulation routine
error_t StartMainMmcRoutine(__SCONTEXT_PAR)
{
    while (GetMmcAbortCondEval(SCONTEXT) == STATE_FLG_CONTINUE)
    {
        SIMERROR = DoNextMmcCycleBlock(SCONTEXT);
        error_assert(SIMERROR, "Simulation abort due to error in MMC cycle block execution.");

        SIMERROR = FinishMmcCycleBlock(SCONTEXT);
        error_assert(SIMERROR, "Simulation abort due to error in MMC cycle block finisher execution.");
    }

    return FinishMainRoutineMmc(SCONTEXT);
}

// Calls the output plugin callback if any is set
static inline error_t CallOutputPlugin(__SCONTEXT_PAR)
{
    return_if(SCONTEXT->Plugins.OnDataOutput == NULL, ERR_OK);
    SCONTEXT->Plugins.OnDataOutput(SCONTEXT);
    return SIMERROR;
}

static inline void AdvanceBlockCounters(__SCONTEXT_PAR)
{
    getMainCycleCounters(SCONTEXT)->StepGoalMcs += getMainCycleCounters(SCONTEXT)->McsPerBlock;
}

// Action for cases where the MMC jump selection leads to an unstable end state
static inline void OnKmcJumpToUnstableState(__SCONTEXT_PAR)
{
    getActiveCounters(SCONTEXT)->UnstableEndCount++;
    AdvanceSimulatedTime(SCONTEXT);
}

// Action for cases where the jump selection enables to leave a currently unstable state
static inline void OnKmcJumpFromUnstableState(__SCONTEXT_PAR)
{
    getActiveCounters(SCONTEXT)->UnstableStartCount++;

    AdvanceTransitionTrackingSystem(SCONTEXT);
    AdvanceKmcSystemToState2(SCONTEXT);

    if (MakeJumpPoolUpdateKmc(SCONTEXT))
    {
        UpdateTimeStepping(SCONTEXT);
    }
}

// Action for cases where the jump selection has been statistically accepted
static inline void OnKmcJumpAccepted(__SCONTEXT_PAR)
{
    getActiveCounters(SCONTEXT)->McsCount++;

    AdvanceSimulatedTime(SCONTEXT);
    AdvanceTransitionTrackingSystem(SCONTEXT);

    AdvanceKmcSystemToState2(SCONTEXT);

    if (MakeJumpPoolUpdateKmc(SCONTEXT))
    {
        UpdateTimeStepping(SCONTEXT);
    }

}

// Action for cases where the jump selection has been statistically rejected
static inline void OnKmcJumpRejected(__SCONTEXT_PAR)
{
    getActiveCounters(SCONTEXT)->RejectionCount++;
    AdvanceTransitionTrackingSystem(SCONTEXT);
    AdvanceSimulatedTime(SCONTEXT);
}

// Action for cases where the jump selection has no valid rule and is site-blocking
static inline void OnKmcJumpSiteBlocked(__SCONTEXT_PAR)
{
    getActiveCounters(SCONTEXT)->SiteBlockingCount++;
}

error_t DoNextKmcCycleBlock(__SCONTEXT_PAR)
{
    while (getMainCycleCounters(SCONTEXT)->Mcs < getMainCycleCounters(SCONTEXT)->StepGoalMcs)
    {
        for (size_t i = 0; i < getMainCycleCounters(SCONTEXT)->McsPerBlock; i++)
        {
            SetNextKmcJumpSelection(SCONTEXT);
            SetKmcJumpPathProperties(SCONTEXT);

            if (GetKmcJumpRuleEvaluation(SCONTEXT))
            {
                SetKmcJumpProperties(SCONTEXT);
                SetKmcJumpEvaluationResults(SCONTEXT);
            }
            else
            {
                OnKmcJumpSiteBlocked(SCONTEXT);
            }
        }
    }
    return SIMERROR;
}

static void SharedCycleBlockFinish(__SCONTEXT_PAR)
{
    UnsetMainStateFlags(SCONTEXT, STATE_FLG_FIRSTCYCLE);

    SIMERROR = SyncSimulationState(SCONTEXT);
    error_assert(SIMERROR, "Simulation aborted due to failed sycnhronization between dynamic model and state object.");

    SIMERROR = SaveSimulationState(SCONTEXT);
    error_assert(SIMERROR, "Simulation aborted due to error during serialization of the state object.");

    SIMERROR = CallOutputPlugin(SCONTEXT);
    error_assert(SIMERROR, "Simulation aborted due to error in the external output plugin.");
}

error_t FinishKmcCycleBlock(__SCONTEXT_PAR)
{
    AdvanceBlockCounters(SCONTEXT);
    SharedCycleBlockFinish(SCONTEXT);

    return SIMERROR;
}

// Action for cases where the MMC jump selection leads to an unstable end state
static inline void OnMmcJumpToUnstableState(__SCONTEXT_PAR)
{
    getActiveCounters(SCONTEXT)->UnstableEndCount++;
}

// Action for cases where the jump selection enables to leave a currently unstable state
static inline void OnMmcJumpFromUnstableState(__SCONTEXT_PAR)
{
    getActiveCounters(SCONTEXT)->UnstableStartCount++;
}

// Action for cases where the jump selection has been statistically accepted
static inline void OnMmcJumpAccepted(__SCONTEXT_PAR)
{
    getActiveCounters(SCONTEXT)->McsCount++;
    AdvanceMmcSystemToState2(SCONTEXT);
    MakeJumpPoolUpdateMmc(SCONTEXT);
}

// Action for cases where the jump selection has been statistically rejected
static inline void OnMmcJumpRejected(__SCONTEXT_PAR)
{
    getActiveCounters(SCONTEXT)->RejectionCount++;
}

// Action for cases where the jump selection has no valid rule and is site-blocking
static inline void OnMmcJumpSiteBlocked(__SCONTEXT_PAR)
{
    getActiveCounters(SCONTEXT)->SiteBlockingCount++;
}

error_t DoNextMmcCycleBlock(__SCONTEXT_PAR)
{
    while (getMainCycleCounters(SCONTEXT)->Mcs < getMainCycleCounters(SCONTEXT)->StepGoalMcs)
    {
        for (size_t i = 0; i < getMainCycleCounters(SCONTEXT)->McsPerBlock; i++)
        {
            SetNextMmcJumpSelection(SCONTEXT);
            SetMmcJumpPathProperties(SCONTEXT);

            if (GetMmcJumpRuleEvaluation(SCONTEXT))
            {
                SetMmcJumpProperties(SCONTEXT);
                SetMmcJumpEvaluationResults(SCONTEXT);
            }
            else
            {
                OnMmcJumpSiteBlocked(SCONTEXT);
            }
        }
    }
    return SIMERROR;
}

error_t FinishMmcCycleBlock(__SCONTEXT_PAR)
{
    AdvanceBlockCounters(SCONTEXT);
    SharedCycleBlockFinish(SCONTEXT);

    return SIMERROR;
}

static inline Bitmask_t GetTimeoutAbortEval(__SCONTEXT_PAR)
{
    int64_t newClock = clock();
    int64_t lastClock = getRuntimeInformation(SCONTEXT)->LastClock;

    getMainStateMetaData(SCONTEXT)->TimePerBlock = (newClock - lastClock) / CLOCKS_PER_SEC;
    getMainStateMetaData(SCONTEXT)->ProgramRunTime += (newClock - lastClock) / CLOCKS_PER_SEC;
    int64_t blockEta = getMainStateMetaData(SCONTEXT)->TimePerBlock + getMainStateMetaData(SCONTEXT)->ProgramRunTime;

    bool_t isTimeout = (getMainStateMetaData(SCONTEXT)->ProgramRunTime >= getDbModelJobInfo(SCONTEXT)->TimeLimit) || (blockEta >
            getDbModelJobInfo(SCONTEXT)->TimeLimit);
    return (isTimeout) ? STATE_FLG_TIMEOUT : STATE_FLG_CONTINUE;
}

static inline bool_t GetRateAbortEval(__SCONTEXT_PAR)
{
    getMainStateMetaData(SCONTEXT)->SuccessRate = getMainCycleCounters(SCONTEXT)->Mcs / getMainStateMetaData(SCONTEXT)->ProgramRunTime;
    getMainStateMetaData(SCONTEXT)->CycleRate = getMainCycleCounters(SCONTEXT)->Cycles / getMainStateMetaData(SCONTEXT)->ProgramRunTime;

    if (getMainStateMetaData(SCONTEXT)->CycleRate < getDbModelJobInfo(SCONTEXT)->MinimalSuccessRate)
    {
        return true;
    }
    return false;
}

static inline bool_t GetMcsTargetReachedEval(__SCONTEXT_PAR)
{
    if (getMainCycleCounters(SCONTEXT)->Mcs >= getMainCycleCounters(SCONTEXT)->TotalGoalMcs)
    {
        return true;
    }
    return false;
}

static error_t GetGeneralAbortCondEval(__SCONTEXT_PAR)
{
    if (StateFlagsAreSet(SCONTEXT, STATE_FLG_FIRSTCYCLE))
    {
        return STATE_FLG_CONTINUE;
    }
    if (GetTimeoutAbortEval(SCONTEXT))
    {
        setMainStateFlags(SCONTEXT, STATE_FLG_TIMEOUT);
        return STATE_FLG_TIMEOUT;
    }
    if (GetRateAbortEval(SCONTEXT))
    {
        setMainStateFlags(SCONTEXT, STATE_FLG_RATEABORT);
        return STATE_FLG_RATEABORT;
    }
    if (GetMcsTargetReachedEval(SCONTEXT))
    {
        setMainStateFlags(SCONTEXT, STATE_FLG_COMPLETED);
        return STATE_FLG_COMPLETED;
    }

    return STATE_FLG_CONTINUE;
}

error_t GetKmcAbortCondEval(__SCONTEXT_PAR)
{
    if (GetGeneralAbortCondEval(SCONTEXT) != STATE_FLG_CONTINUE)
    {
        return STATE_FLG_CONDABORT;
    }
    return STATE_FLG_CONTINUE;
}

error_t GetMmcAbortCondEval(__SCONTEXT_PAR)
{
    if (GetGeneralAbortCondEval(SCONTEXT) != STATE_FLG_CONTINUE)
    {
        return STATE_FLG_CONDABORT;
    }
    return STATE_FLG_CONTINUE;
}

error_t SyncSimulationState(__SCONTEXT_PAR)
{
    return ERR_OK;
}

error_t SaveSimulationState(__SCONTEXT_PAR)
{
    if (StateFlagsAreSet(SCONTEXT, STATE_FLG_PRERUN))
    {
        SIMERROR = SaveWriteBufferToFile(FILE_PRERSTATE, FMODE_BINARY_W, getMainStateBuffer(SCONTEXT));
    }
    else
    {
        SIMERROR = SaveWriteBufferToFile(FILE_MAINSTATE, FMODE_BINARY_W, getMainStateBuffer(SCONTEXT));
    }
    return SIMERROR;
}

static error_t GeneralSimulationFinish(__SCONTEXT_PAR)
{
    setMainStateFlags(SCONTEXT, STATE_FLG_COMPLETED);
    SIMERROR = SaveSimulationState(SCONTEXT);
    return SIMERROR;
}

error_t FinishMainRoutineKmc(__SCONTEXT_PAR)
{
    SIMERROR = GeneralSimulationFinish(SCONTEXT);
    error_assert(SIMERROR, "Simulation aborted due to error in general simulation finisher routine execution.");
    return SIMERROR;
}

error_t FinishMainRoutineMmc(__SCONTEXT_PAR)
{
    SIMERROR = GeneralSimulationFinish(SCONTEXT);
    error_assert(SIMERROR, "Simulation aborted due to error in general simulation finisher routine execution.");
    return SIMERROR;
}

static inline int32_t LookupActJumpId(__SCONTEXT_PAR)
{
    return array_Get(*getJumpDirectionMapping(SCONTEXT), JUMPPATH[0]->PositionVector.D, JUMPPATH[0]->ParticleId, getJumpSelectionInfo(SCONTEXT)->RelativeId);
}

static inline void SetActJumpDirAndCol(__SCONTEXT_PAR)
{
    getJumpSelectionInfo(SCONTEXT)->JumpId = LookupActJumpId(SCONTEXT);
    getCycleState(SCONTEXT)->ActiveJumpDirection = getJumpDirectionAt(SCONTEXT, getJumpSelectionInfo(SCONTEXT)->JumpId);
    getCycleState(SCONTEXT)->ActiveJumpCollection = getJumpCollectionAt(SCONTEXT, getActiveJumpDirection(
            SCONTEXT)->JumpCollectionId);
}

static inline void SetActPathStartEnv(__SCONTEXT_PAR)
{
    JUMPPATH[0] = getEnvironmentStateAt(SCONTEXT, getJumpSelectionInfo(SCONTEXT)->EnvironmentId);
    SetCodeByteAt(&getCycleState(SCONTEXT)->ActiveStateCode, 0, JUMPPATH[0]->ParticleId);
    JUMPPATH[0]->PathId = 0;
}

static inline void SetActCounterCol(__SCONTEXT_PAR)
{
    getCycleState(SCONTEXT)->ActiveCounterCollection = getMainStateCounterAt(SCONTEXT, JUMPPATH[0]->ParticleId);
}

void SetNextKmcJumpSelection(__SCONTEXT_PAR)
{
    RollNextKmcSelect(SCONTEXT);
    getCycleState(SCONTEXT)->ActiveStateCode = 0ULL;

    SetActCounterCol(SCONTEXT);
    SetActPathStartEnv(SCONTEXT);
    SetActJumpDirAndCol(SCONTEXT);
}

void SetKmcJumpPathProperties(__SCONTEXT_PAR)
{
    Vector4_t actVector;
    byte_t index = 0;

    cpp_foreach(relVector, getActiveJumpDirection(SCONTEXT)->JumpSequence)
    {
        actVector = AddAndTrimVector4(&JUMPPATH[0]->PositionVector, relVector, getLatticeSizeVector(SCONTEXT));
        JUMPPATH[index] = getEnvironmentStateByVector4(SCONTEXT, &actVector);

        SetCodeByteAt(&getCycleState(SCONTEXT)->ActiveStateCode, index, JUMPPATH[index]->ParticleId);
        JUMPPATH[index]->PathId = index;
    }
}

static inline OccCode_t GetLastPossibleJumpCode(__SCONTEXT_PAR)
{
    return getActiveJumpCollection(SCONTEXT)->JumpRules.End[-1].StateCode0;
}

static inline void LinearSearchAndSetActiveJumpRule(__SCONTEXT_PAR)
{
    if (GetLastPossibleJumpCode(SCONTEXT) < getPathStateCode(SCONTEXT))
    {
        getCycleState(SCONTEXT)->ActiveJumpRule = NULL;
    }
    else
    {
        getCycleState(SCONTEXT)->ActiveJumpRule = getActiveJumpCollection(SCONTEXT)->JumpRules.Begin;
        while (getActiveJumpRule(SCONTEXT)->StateCode0 < getPathStateCode(SCONTEXT))
        {
            getCycleState(SCONTEXT)->ActiveJumpRule++;
        }
        if (getActiveJumpRule(SCONTEXT)->StateCode0 != getPathStateCode(SCONTEXT))
        {
            getCycleState(SCONTEXT)->ActiveJumpRule = NULL;
        }
    }
}

static inline void BinarySearchAndSetActiveJumpRule(__SCONTEXT_PAR)
{
    decllocal(FUNCDECL_COMPARER, BinarySearchAndSetActiveJumpRule_Compare, JumpRule_t);
    decllocal(FUNCDECL_BINARYSEARCH, BinarySearchAndSetActiveJumpRule_Search, JumpRules_t, JumpRule_t);

    JumpRule_t searchObj = {.StateCode0 = getPathStateCode(SCONTEXT)};
    int32_t id = local_BinarySearchAndSetActiveJumpRule_Search(&getActiveJumpCollection(SCONTEXT)->JumpRules, &searchObj);
    *getActiveJumpRule(SCONTEXT) = span_Get(getActiveJumpCollection(SCONTEXT)->JumpRules, id);
}

impllocal(FUNCIMPL_COMPARER, local_BinarySearchAndSetActiveJumpRule_Compare, JumpRule_t, makeCompGetter, StateCode0);
impllocal(FUNCIMPL_BINARYSEARCH, local_BinarySearchAndSetActiveJumpRule_Search, JumpRules_t, JumpRule_t, local_BinarySearchAndSetActiveJumpRule_Compare);

static inline void LookupAndSetActJumpRule(__SCONTEXT_PAR)
{
    LinearSearchAndSetActiveJumpRule(SCONTEXT);
}

bool_t GetKmcJumpRuleEvaluation(__SCONTEXT_PAR)
{
    LookupAndSetActJumpRule(SCONTEXT);
    return getActiveJumpRule(SCONTEXT) == NULL;
}

void SetKmcJumpProperties(__SCONTEXT_PAR)
{
    SetAllStateEnergiesKmc(SCONTEXT);
}

void SetKmcJumpProbabilities(__SCONTEXT_PAR)
{
    JumpEnergyInfo_t* energyInfo = getJumpEnergyInfo(SCONTEXT);

    energyInfo->FieldInfluence = CalcElectricFieldInfluence(SCONTEXT);
    energyInfo->ConformationDelta = 0.5 * (energyInfo->Energy2 - energyInfo->Energy0);

    energyInfo->Energy0To2 = energyInfo->Energy1 + energyInfo->ConformationDelta + energyInfo->FieldInfluence;
    energyInfo->Energy0To2 = energyInfo->Energy1 - energyInfo->ConformationDelta + energyInfo->FieldInfluence;

    energyInfo->Probability0to2 = exp(energyInfo->Energy0To2) * getActiveJumpRule(SCONTEXT)->FrequencyFactor;
    energyInfo->Probability2to0 = (energyInfo->Energy2To0 < 0.0) ? INFINITY : 0.0;
}

void SetKmcJumpEvaluationResults(__SCONTEXT_PAR)
{
    SCONTEXT->Plugins.OnSetJumpProbabilities(SCONTEXT);

    // Unstable end: Do not advance system, update counter and simulated time
    if (getJumpEnergyInfo(SCONTEXT)->Probability2to0 > MC_CONST_JUMPLIMIT_MAX)
    {
        OnKmcJumpToUnstableState(SCONTEXT);
        return;
    }

    // Unstable start: Advance system, update counter but not simulated time, do pool update
    if (getJumpEnergyInfo(SCONTEXT)->Probability0to2 > MC_CONST_JUMPLIMIT_MAX)
    {
        OnKmcJumpFromUnstableState(SCONTEXT);
        return;
    }
    
    // Successful jump: Advance system, update counters and simulated time, do pool update
    if (GetNextRandomDouble(SCONTEXT) < getJumpEnergyInfo(SCONTEXT)->Probability0to2)
    {
        OnKmcJumpAccepted(SCONTEXT);
        return;
    }

    // Rejected jump: Do not advance system, update counter and simulated time, no pool update
    OnKmcJumpRejected(SCONTEXT);
}

void SetNextMmcJumpSelection(__SCONTEXT_PAR)
{
    RollNextMmcSelect(SCONTEXT);

    SetActCounterCol(SCONTEXT);
    SetActPathStartEnv(SCONTEXT);
    SetActJumpDirAndCol(SCONTEXT);
}

void SetMmcJumpPathProperties(__SCONTEXT_PAR)
{
    // Get the first environment state pointer (0,0,0,0) and write the offset source state to the unused 3rd path index
    JUMPPATH[2] = getEnvironmentStateAt(SCONTEXT, getJumpSelectionInfo(SCONTEXT)->OffsetId);
    JUMPPATH[1] = getEnvironmentStateAt(SCONTEXT, 0);

    // Advance the pointer by the affiliated block jumps
    JUMPPATH[1] += getEnvironmentLattice(SCONTEXT)->Header->Blocks[0] * JUMPPATH[2]->PositionVector.A;
    JUMPPATH[1] += getEnvironmentLattice(SCONTEXT)->Header->Blocks[1] * JUMPPATH[2]->PositionVector.B;
    JUMPPATH[1] += getEnvironmentLattice(SCONTEXT)->Header->Blocks[2] * JUMPPATH[2]->PositionVector.C;
    JUMPPATH[1] += JUMPPATH[0]->PositionVector.D;

    // Correct the active state code byte and set the path id of the second environment state
    SetCodeByteAt(&getCycleState(SCONTEXT)->ActiveStateCode, 1, JUMPPATH[1]->ParticleId);
    JUMPPATH[1]->PathId = 1;
}

bool_t GetMmcJumpRuleEvaluation(__SCONTEXT_PAR)
{
    LookupAndSetActJumpRule(SCONTEXT);
    return getCycleState(SCONTEXT)->ActiveJumpRule == NULL;
}

void SetMmcJumpProperties(__SCONTEXT_PAR)
{
    SetAllStateEnergiesMmc(SCONTEXT);
}

void SetMmcJumpProbabilities(__SCONTEXT_PAR)
{
    JumpEnergyInfo_t* energyInfo = getJumpEnergyInfo(SCONTEXT);
    energyInfo->Energy0To2 = energyInfo->Energy2 - energyInfo->Energy0;
    energyInfo->Probability0to2 = exp(energyInfo->Energy0To2);
}

void SetMmcJumpEvaluationResults(__SCONTEXT_PAR)
{
    SCONTEXT->Plugins.OnSetJumpProbabilities(SCONTEXT);

    // Handle case where the end state is unstable
    if (getJumpEnergyInfo(SCONTEXT)->Probability2to0 > MC_CONST_JUMPLIMIT_MAX)
    {
        OnMmcJumpToUnstableState(SCONTEXT);
        return;
    }

    // Handle case where the start state is unstable
    if (getJumpEnergyInfo(SCONTEXT)->Probability0to2 > MC_CONST_JUMPLIMIT_MAX)
    {
        OnMmcJumpFromUnstableState(SCONTEXT);
        return;
    }

    // Handle case where the jump is statistically accepted
    if (GetNextRandomDouble(SCONTEXT) < getJumpEnergyInfo(SCONTEXT)->Probability0to2)
    {
        OnMmcJumpAccepted(SCONTEXT);
        return;
    }

    // Handle case where the jump is statistically rejected
    OnMmcJumpRejected(SCONTEXT);
}
