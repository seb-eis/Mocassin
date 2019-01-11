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

void PrepareForMainRoutine(__SCONTEXT_PAR)
{
    PrepareContextForSimulation(SCONTEXT);
}

error_t ResetContextAfterPrerun(__SCONTEXT_PAR)
{
    return ERR_NOTIMPLEMENTED;
}

error_t StartMainRoutine(__SCONTEXT_PAR)
{
    runtime_assertion(MainStateHasFlags(SCONTEXT, FLG_STATEERROR), SIMERROR, "Cannot start main simulation routine, state error flag is set.")

    if (MainStateHasFlags(SCONTEXT, FLG_KMC))
    {
        if (MainStateHasFlags(SCONTEXT, FLG_PRERUN))
        {
            SIMERROR = StartMainKmcRoutine(SCONTEXT);
            error_assert(SIMERROR, "Pre-run execution of main KMC routine aborted with an error");

            SIMERROR = FinishRoutinePrerun(SCONTEXT);
            error_assert(SIMERROR, "Pre-run finish of KMC main routine failed.")
        }
        
        return StartMainKmcRoutine(SCONTEXT);
    }

    if (MainStateHasFlags(SCONTEXT, FLG_MMC))
    {
        if (MainStateHasFlags(SCONTEXT, FLG_PRERUN))
        {
            SIMERROR = StartMainMmcRoutine(SCONTEXT);
            error_assert(SIMERROR, "Prerun execution of main KMC routine aborted with an error");

            SIMERROR = FinishRoutinePrerun(SCONTEXT);
            error_assert(SIMERROR, "Prerun finish of KMC main routine failed.")
        }
        
        return StartMainKmcRoutine(SCONTEXT);
    }

    error_assert(ERR_UNKNOWN, "Main routine starter skipped selection process. Neither MMC nor KMC flags is set.");
    return ERR_UNKNOWN; // GCC [-Wall] expects return value, even with exit(..) statement
}

// Finishes the routine pre-run and resets the context for the main simulation routine
error_t FinishRoutinePrerun(__SCONTEXT_PAR)
{
    SIMERROR = SaveSimulationState(SCONTEXT);
    error_assert(SIMERROR, "State save after pre-run completion failed.");

    SIMERROR = ResetContextAfterPrerun(SCONTEXT);
    error_assert(SIMERROR, "Context reset after pre-run completion failed.");

    UnsetMainStateFlags(SCONTEXT, FLG_PRERUN);
    return SIMERROR;
}

// Starts the main kinetic simulation routine
error_t StartMainKmcRoutine(__SCONTEXT_PAR)
{
    while (GetKmcAbortCondEval(SCONTEXT) == FLG_CONTINUE)
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
    while (GetMmcAbortCondEval(SCONTEXT) == FLG_CONTINUE)
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
    UnsetMainStateFlags(SCONTEXT, FLG_FIRSTCYCLE);

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

    bool_t isTimeout = (getMainStateMetaData(SCONTEXT)->ProgramRunTime >= getJobInformation(SCONTEXT)->TimeLimit) || (blockEta > getJobInformation(SCONTEXT)->TimeLimit);
    return (isTimeout) ? FLG_TIMEOUT : FLG_CONTINUE;
}

static inline bool_t GetRateAbortEval(__SCONTEXT_PAR)
{
    getMainStateMetaData(SCONTEXT)->SuccessRate = getMainCycleCounters(SCONTEXT)->Mcs / getMainStateMetaData(SCONTEXT)->ProgramRunTime;
    getMainStateMetaData(SCONTEXT)->CycleRate = getMainCycleCounters(SCONTEXT)->Cycles / getMainStateMetaData(SCONTEXT)->ProgramRunTime;

    if (getMainStateMetaData(SCONTEXT)->CycleRate < getJobInformation(SCONTEXT)->MinimalSuccessRate)
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
    if (MainStateHasFlags(SCONTEXT, FLG_FIRSTCYCLE))
    {
        return FLG_CONTINUE;
    }
    if (GetTimeoutAbortEval(SCONTEXT))
    {
        setMainStateFlags(SCONTEXT, FLG_TIMEOUT);
        return FLG_TIMEOUT;
    }
    if (GetRateAbortEval(SCONTEXT))
    {
        setMainStateFlags(SCONTEXT, FLG_RATELIMIT);
        return FLG_RATELIMIT;
    }
    if (GetMcsTargetReachedEval(SCONTEXT))
    {
        setMainStateFlags(SCONTEXT, FLG_COMPLETED);
        return FLG_COMPLETED;
    }

    return FLG_CONTINUE;
}

error_t GetKmcAbortCondEval(__SCONTEXT_PAR)
{
    if (GetGeneralAbortCondEval(SCONTEXT) != FLG_CONTINUE)
    {
        return FLG_ABORTCONDITION;
    }
    return FLG_CONTINUE;
}

error_t GetMmcAbortCondEval(__SCONTEXT_PAR)
{
    if (GetGeneralAbortCondEval(SCONTEXT) != FLG_CONTINUE)
    {
        return FLG_ABORTCONDITION;
    }
    return FLG_CONTINUE;
}

error_t SyncSimulationState(__SCONTEXT_PAR)
{
    return ERR_OK;
}

error_t SaveSimulationState(__SCONTEXT_PAR)
{
    if (MainStateHasFlags(SCONTEXT, FLG_PRERUN))
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
    setMainStateFlags(SCONTEXT, FLG_COMPLETED);
    SIMERROR = SaveSimulationState(SCONTEXT);
    return SIMERROR;
}

error_t FinishMainRoutineKmc(__SCONTEXT_PAR)
{
    SIMERROR = GeneralSimulationFinish(SCONTEXT);
    error_assert(SIMERROR, "Simulation aborted due to error in general simulation finisher routine exceution.");
    return SIMERROR;
}

error_t FinishMainRoutineMmc(__SCONTEXT_PAR)
{
    SIMERROR = GeneralSimulationFinish(SCONTEXT);
    error_assert(SIMERROR, "Simulation aborted due to error in general simulation finisher routine exceution.");
    return SIMERROR;
}

static inline int32_t LookupActJumpId(__SCONTEXT_PAR)
{
    return array_Get(*getJumpIdToPositionsAssignmentTable(SCONTEXT), JUMPPATH[0]->PositionVector.d, JUMPPATH[0]->ParticleId, getJumpSelectionInfo(SCONTEXT)->RelativeId);
}

static inline void SetActJumpDirAndCol(__SCONTEXT_PAR)
{
    getJumpSelectionInfo(SCONTEXT)->JumpId = LookupActJumpId(SCONTEXT);
    getCycleState(SCONTEXT)->ActiveJumpDirection = getJumpDirectionById(SCONTEXT, getJumpSelectionInfo(SCONTEXT)->JumpId);
    getCycleState(SCONTEXT)->ActiveJumpCollection = getJumpCollectionById(SCONTEXT, getActiveJumpDirection(SCONTEXT)->JumpCollectionId);
}

static inline void SetActPathStartEnv(__SCONTEXT_PAR)
{
    JUMPPATH[0] = getEnvironmentStateById(SCONTEXT, getJumpSelectionInfo(SCONTEXT)->EnvironmentId);
    SetCodeByteAt(&getCycleState(SCONTEXT)->ActiveStateCode, 0, JUMPPATH[0]->ParticleId);
    JUMPPATH[0]->PathId = 0;
}

static inline void SetActCounterCol(__SCONTEXT_PAR)
{
    getCycleState(SCONTEXT)->ActiveCounterCollection = getMainStateCounterById(SCONTEXT, JUMPPATH[0]->ParticleId);
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

static inline void LinearJumpRuleLookup(__SCONTEXT_PAR)
{
    if (GetLastPossibleJumpCode(SCONTEXT) < getActiveStateCode(SCONTEXT))
    {
        getCycleState(SCONTEXT)->ActiveJumpRule = NULL;
    }
    else
    {
        getCycleState(SCONTEXT)->ActiveJumpRule = getActiveJumpCollection(SCONTEXT)->JumpRules.Begin;
        while (getActiveJumpRule(SCONTEXT)->StateCode0 < getActiveStateCode(SCONTEXT))
        {
            getCycleState(SCONTEXT)->ActiveJumpRule++;
        }
        if (getActiveJumpRule(SCONTEXT)->StateCode0 != getActiveStateCode(SCONTEXT))
        {
            getCycleState(SCONTEXT)->ActiveJumpRule = NULL;
        }
    }
}

static inline void BinaryJumpRuleLookup(__SCONTEXT_PAR)
{
    // Possible implementation on optimization
}

static inline void LookupAndSetActJumpRule(__SCONTEXT_PAR)
{
    LinearJumpRuleLookup(SCONTEXT);
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
    energyInfo->Probability0to2 = exp(energyInfo->Energy1 + energyInfo->ConformationDelta) * getActiveJumpRule(SCONTEXT)->FrequencyFactor;
    energyInfo->Probability2to0 = (energyInfo->ConformationDelta > energyInfo->Energy1) ? INFINITY : 0.0;
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
    JUMPPATH[2] = getEnvironmentStateById(SCONTEXT, getJumpSelectionInfo(SCONTEXT)->OffsetId);
    JUMPPATH[1] = getEnvironmentStateById(SCONTEXT, 0);

    // Advance the pointer by the affiliated block jumps
    JUMPPATH[1] += getEnvironmentLattice(SCONTEXT)->Header->Blocks[0] * JUMPPATH[2]->PositionVector.a;
    JUMPPATH[1] += getEnvironmentLattice(SCONTEXT)->Header->Blocks[1] * JUMPPATH[2]->PositionVector.b;
    JUMPPATH[1] += getEnvironmentLattice(SCONTEXT)->Header->Blocks[2] * JUMPPATH[2]->PositionVector.c;
    JUMPPATH[1] += JUMPPATH[0]->PositionVector.d;

    // Correct the active state code byte and set the path id of the second environment state
    SetCodeByteAt(&getCycleState(SCONTEXT)->ActiveStateCode, 1, JUMPPATH[1]->ParticleId);
    JUMPPATH[1]->PathId = 1;
}

bool_t GetMmcJumpRuleEvaluation(__SCONTEXT_PAR)
{
    LookupAndSetActJumpRule(SCONTEXT);
    return getCycleState(SCONTEXT)->ActiveJumpRule == 0;
}

void SetMmcJumpProperties(__SCONTEXT_PAR)
{
    SetAllStateEnergiesMmc(SCONTEXT);
}

void SetMmcJumpProbabilities(__SCONTEXT_PAR)
{
    getJumpEnergyInfo(SCONTEXT)->Probability0to2 = exp(getJumpEnergyInfo(SCONTEXT)->Energy2 - getJumpEnergyInfo(SCONTEXT)->Energy0);
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
