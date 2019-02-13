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
#include "Simulator/Logic/Routines/Debug/DebugRoutines.h"

// Advances the block counters of the main loop to the next step goal
static inline void AdvanceMainCycleCounterStepGoal(SCONTEXT_PARAM)
{
    var counters = getMainCycleCounters(SCONTEXT);
    counters->StepGoalMcs += counters->McsPerBlock;
}

// Sets the simulation run info to the current program status
static inline void SetRuntimeInfoToCurrent(SCONTEXT_PARAM)
{
    var runInfo = getRuntimeInformation(SCONTEXT);
    runInfo->MainRoutineStartClock = clock();
    runInfo->PreviousBlockFinishClock = runInfo->MainRoutineStartClock;
}

void PrepareForMainRoutine(SCONTEXT_PARAM)
{
    PrepareContextForSimulation(SCONTEXT);

    if (getMainCycleCounters(SCONTEXT)->StepGoalMcs == 0)
        AdvanceMainCycleCounterStepGoal(SCONTEXT);

    SetRuntimeInfoToCurrent(SCONTEXT);
}

error_t ResetContextAfterPreRun(SCONTEXT_PARAM)
{
    return_if(!JobInfoFlagsAreSet(SCONTEXT, INFO_FLG_USEPRERUN), ERR_OK);
    setMainStateFlags(SCONTEXT, STATE_FLG_PRERUN_RESET);

    return ERR_NOTIMPLEMENTED;
}

error_t StartMainSimulationRoutine(SCONTEXT_PARAM)
{
    runtime_assertion(!StateFlagsAreSet(SCONTEXT, STATE_FLG_SIMERROR), SIMERROR, "Cannot start main simulation routine, state error flag is set.")

    if (JobInfoFlagsAreSet(SCONTEXT, INFO_FLG_KMC))
    {
        if (StateFlagsAreSet(SCONTEXT, STATE_FLG_PRERUN))
        {
            SIMERROR = KMC_StartMainRoutine(SCONTEXT);
            error_assert(SIMERROR, "Pre-run execution of main KMC routine aborted with an error");

            SIMERROR = FinishRoutinePreRun(SCONTEXT);
            error_assert(SIMERROR, "Pre-run finish of KMC main routine failed.")
        }
        
        return KMC_StartMainRoutine(SCONTEXT);
    }

    if (JobInfoFlagsAreSet(SCONTEXT, INFO_FLG_MMC))
    {
        if (StateFlagsAreSet(SCONTEXT, STATE_FLG_PRERUN))
        {
            SIMERROR = MMC_StartMainRoutine(SCONTEXT);
            error_assert(SIMERROR, "Pre-run execution of main KMC routine aborted with an error");

            SIMERROR = FinishRoutinePreRun(SCONTEXT);
            error_assert(SIMERROR, "Pre-run finish of KMC main routine failed.")
        }
        
        return MMC_StartMainRoutine(SCONTEXT);
    }

    error_assert(ERR_UNKNOWN, "Main routine starter skipped selection process. Neither MMC nor KMC flags is set.");
    return ERR_UNKNOWN; // GCC [-Wall] expects return value, even with exit(..) statement
}

// Finishes the routine pre-run and resets the context for the main simulation routine
error_t FinishRoutinePreRun(SCONTEXT_PARAM)
{
    SIMERROR = SaveSimulationState(SCONTEXT);
    error_assert(SIMERROR, "State save after pre-run completion failed.");

    SIMERROR = ResetContextAfterPreRun(SCONTEXT);
    error_assert(SIMERROR, "Context reset after pre-run completion failed.");

    UnsetMainStateFlags(SCONTEXT, STATE_FLG_PRERUN);
    return SIMERROR;
}

// Starts the main kinetic simulation routine
error_t KMC_StartMainRoutine(SCONTEXT_PARAM)
{
    error_t abortFlag = KMC_CheckAbortConditions(SCONTEXT);

    while(abortFlag == STATE_FLG_CONTINUE)
    {
        SIMERROR = KMC_DoNextCycleBlock(SCONTEXT);
        error_assert(SIMERROR, "Simulation abort due to error in KMC cycle block execution.");

        SIMERROR = KMC_FinishCycleBlock(SCONTEXT);
        error_assert(SIMERROR, "Simulation abort due to error in KMC cycle block finisher execution.");

        abortFlag = KMC_CheckAbortConditions(SCONTEXT);
        PrintRunStatistics(SCONTEXT, stdout);
    }

    return KMC_FinishMainRoutine(SCONTEXT);
}

// Starts the main metropolis simulation routine
error_t MMC_StartMainRoutine(SCONTEXT_PARAM)
{
    error_t abortFlag = MMC_CheckAbortConditions(SCONTEXT);

    while(abortFlag == STATE_FLG_CONTINUE)
    {
        SIMERROR = MMC_DoNextCycleBlock(SCONTEXT);
        error_assert(SIMERROR, "Simulation abort due to error in MMC cycle block execution.");

        SIMERROR = MMC_FinishCycleBlock(SCONTEXT);
        error_assert(SIMERROR, "Simulation abort due to error in MMC cycle block finisher execution.");

        abortFlag = MMC_CheckAbortConditions(SCONTEXT);
        PrintRunStatistics(SCONTEXT, stdout);
    }

    return MMC_FinishMainRoutine(SCONTEXT);
}

// Calls the output plugin callback if any is set
static inline error_t CallOutputPlugin(SCONTEXT_PARAM)
{
    return_if(SCONTEXT->Plugins.OnDataOutput == NULL, ERR_OK);
    SCONTEXT->Plugins.OnDataOutput(SCONTEXT);
    return SIMERROR;
}

// Action for cases where the MMC jump selection leads to an unstable end state
static inline void KMC_OnJumpIsToUnstableState(SCONTEXT_PARAM)
{
    getActiveCounters(SCONTEXT)->UnstableEndCount++;
    AdvanceSimulatedTime(SCONTEXT);
}

// Action for cases where the jump selection enables to leave a currently unstable state
static inline void KMC_OnJumpIsFromUnstableState(SCONTEXT_PARAM)
{
    getActiveCounters(SCONTEXT)->UnstableStartCount++;

    AdvanceTransitionTrackingSystem(SCONTEXT);
    KMC_AdvanceSystemToFinalState(SCONTEXT);

    if (KMC_UpdateJumpPool(SCONTEXT)) UpdateTimeStepping(SCONTEXT);
}

// Action for cases where the jump selection has been statistically accepted
static inline void KMC_OnJumpIsStatisticallyAccepted(SCONTEXT_PARAM)
{
    ++getActiveCounters(SCONTEXT)->McsCount;
    ++getMainCycleCounters(SCONTEXT)->Mcs;

    AdvanceSimulatedTime(SCONTEXT);
    AdvanceTransitionTrackingSystem(SCONTEXT);

    KMC_AdvanceSystemToFinalState(SCONTEXT);

    if (KMC_UpdateJumpPool(SCONTEXT)) UpdateTimeStepping(SCONTEXT);
}

// Action for cases where the jump selection has been statistically rejected
static inline void KMC_OnJumpIsStatisticallyRejected(SCONTEXT_PARAM)
{
    getActiveCounters(SCONTEXT)->RejectionCount++;
    AdvanceSimulatedTime(SCONTEXT);
}

// Action for cases where the jump selection has no valid rule and is site-blocking
static inline void KMC_OnJumpIsSiteBlocked(SCONTEXT_PARAM)
{
    getActiveCounters(SCONTEXT)->SiteBlockingCount++;
    AdvanceSimulatedTime(SCONTEXT);
}

error_t KMC_DoNextCycleBlock(SCONTEXT_PARAM)
{
    var counters = getMainCycleCounters(SCONTEXT);
    for (;counters->Mcs < counters->StepGoalMcs; counters->Cycles += counters->CyclesPerBlock)
    {
        for (size_t i = 0; i < counters->CyclesPerBlock; ++i)
        {
            KMC_SetNextJumpSelection(SCONTEXT);
            KMC_SetJumpPathProperties(SCONTEXT);

            if (KMC_TrySetJumpRule(SCONTEXT))
            {
                KMC_SetJumpProperties(SCONTEXT);
                KMC_OnEnergeticJumpEvaluation(SCONTEXT);
            }
            else
            {
                KMC_OnJumpIsSiteBlocked(SCONTEXT);
            }
        }
    }
    return SIMERROR;
}

static void SharedCycleBlockFinish(SCONTEXT_PARAM)
{
    UnsetMainStateFlags(SCONTEXT, STATE_FLG_FIRSTCYCLE);

    SIMERROR = SyncSimulationStateToRunStatus(SCONTEXT);
    error_assert(SIMERROR, "Simulation aborted due to failed synchronization between dynamic model and state object.");

    SIMERROR = SaveSimulationState(SCONTEXT);
    error_assert(SIMERROR, "Simulation aborted due to error during serialization of the state object.");

    SIMERROR = CallOutputPlugin(SCONTEXT);
    error_assert(SIMERROR, "Simulation aborted due to error in the external output plugin.");
}

error_t KMC_FinishCycleBlock(SCONTEXT_PARAM)
{
    AdvanceMainCycleCounterStepGoal(SCONTEXT);
    SharedCycleBlockFinish(SCONTEXT);

    return SIMERROR;
}

// Action for cases where the MMC jump selection leads to an unstable end state
static inline void MMC_OnJumpIsToUnstableState(SCONTEXT_PARAM)
{
    getActiveCounters(SCONTEXT)->UnstableEndCount++;
}

// Action for cases where the jump selection enables to leave a currently unstable state
static inline void MMC_OnJumpIsFromUnstableState(SCONTEXT_PARAM)
{
    getActiveCounters(SCONTEXT)->UnstableStartCount++;
}

// Action for cases where the jump selection has been statistically accepted
static inline void MMC_OnJumpIsStatisticallyAccepted(SCONTEXT_PARAM)
{
    ++getActiveCounters(SCONTEXT)->McsCount;
    ++getMainCycleCounters(SCONTEXT)->Mcs;

    MMC_AdvanceSystemToFinalState(SCONTEXT);
    MMC_UpdateJumpPool(SCONTEXT);
}

// Action for cases where the jump selection has been statistically rejected
static inline void MMC_OnJumpIsStatisticallyRejected(SCONTEXT_PARAM)
{
    getActiveCounters(SCONTEXT)->RejectionCount++;
}

// Action for cases where the jump selection has no valid rule and is site-blocking
static inline void MMC_OnJumpIsSiteBlocked(SCONTEXT_PARAM)
{
    getActiveCounters(SCONTEXT)->SiteBlockingCount++;
}

error_t MMC_DoNextCycleBlock(SCONTEXT_PARAM)
{
    var counters = getMainCycleCounters(SCONTEXT);
    for (;counters->Mcs < counters->StepGoalMcs; counters->Cycles += counters->CyclesPerBlock)
    {
        for (size_t i = 0; i < counters->CyclesPerBlock; i++)
        {
            MMC_SetNextJumpSelection(SCONTEXT);
            MMC_SetJumpPathProperties(SCONTEXT);

            if (MMC_TrySetJumpRule(SCONTEXT))
            {
                MMC_SetJumpProperties(SCONTEXT);
                MMC_OnEnergeticJumpEvaluation(SCONTEXT);
            }
            else
            {
                MMC_OnJumpIsSiteBlocked(SCONTEXT);
            }
        }
    }
    return SIMERROR;
}

error_t MMC_FinishCycleBlock(SCONTEXT_PARAM)
{
    AdvanceMainCycleCounterStepGoal(SCONTEXT);
    SharedCycleBlockFinish(SCONTEXT);

    return SIMERROR;
}

// Evaluates if the ETA of the simulation is likely to timeout during the next block
static inline bool_t UpdateAndEvaluateTimeoutAbortCondition(SCONTEXT_PARAM)
{
    let jobInfo = getDbModelJobInfo(SCONTEXT);
    var runInfo = getRuntimeInformation(SCONTEXT);
    var metaData = getMainStateMetaData(SCONTEXT);
    var newClock = clock();

    metaData->TimePerBlock = (newClock - runInfo->PreviousBlockFinishClock) / CLOCKS_PER_SEC;
    metaData->ProgramRunTime += (newClock - runInfo->PreviousBlockFinishClock) / CLOCKS_PER_SEC;
    var blockEta = metaData->TimePerBlock + metaData->ProgramRunTime;

    runInfo->PreviousBlockFinishClock = newClock;

    bool_t isTimeout = (metaData->ProgramRunTime >= jobInfo->TimeLimit) || (blockEta >jobInfo->TimeLimit);
    return isTimeout;
}

// Evaluates if the current success and cycles rates are below the limit
static inline bool_t UpdateAndEvaluateRateAbortConditions(SCONTEXT_PARAM)
{
    let jobInfo = getDbModelJobInfo(SCONTEXT);
    let counters = getMainCycleCounters(SCONTEXT);
    var metaData = getMainStateMetaData(SCONTEXT);

    metaData->SuccessRate = counters->Mcs / metaData->ProgramRunTime;
    metaData->CycleRate = counters->Cycles / metaData->ProgramRunTime;

    return (metaData->CycleRate < jobInfo->MinimalSuccessRate);
}

// Evaluates if the mcs target is reached
static inline bool_t UpdateAndEvaluateMcsAbortCondition(SCONTEXT_PARAM)
{
    let counters = getMainCycleCounters(SCONTEXT);
    return (counters->Mcs >= counters->TotalGoalMcs);
}

// Evaluates the general abort conditions and returns the corresponding state flag
static error_t EvaluateGeneralAbortConditions(SCONTEXT_PARAM)
{
    if (StateFlagsAreSet(SCONTEXT, STATE_FLG_FIRSTCYCLE))
        return STATE_FLG_CONTINUE;

    if (UpdateAndEvaluateTimeoutAbortCondition(SCONTEXT))
    {
        setMainStateFlags(SCONTEXT, STATE_FLG_TIMEOUT);
        return STATE_FLG_TIMEOUT;
    }
    if (UpdateAndEvaluateRateAbortConditions(SCONTEXT))
    {
        setMainStateFlags(SCONTEXT, STATE_FLG_RATEABORT);
        return STATE_FLG_RATEABORT;
    }
    if (UpdateAndEvaluateMcsAbortCondition(SCONTEXT))
    {
        setMainStateFlags(SCONTEXT, STATE_FLG_COMPLETED);
        return STATE_FLG_COMPLETED;
    }

    return STATE_FLG_CONTINUE;
}

error_t KMC_CheckAbortConditions(SCONTEXT_PARAM)
{
    return (EvaluateGeneralAbortConditions(SCONTEXT) != STATE_FLG_CONTINUE)
        ? STATE_FLG_CONDABORT
        : STATE_FLG_CONTINUE;
}

error_t MMC_CheckAbortConditions(SCONTEXT_PARAM)
{
    return (EvaluateGeneralAbortConditions(SCONTEXT) != STATE_FLG_CONTINUE)
        ? STATE_FLG_CONDABORT
        : STATE_FLG_CONTINUE;
}

error_t SyncMainStateLatticeToRunStatus(SCONTEXT_PARAM)
{
    let environmentLattice = getEnvironmentLattice(SCONTEXT);
    var latticeState = getMainStateLattice(SCONTEXT);

    return_if(span_GetSize(*latticeState) != array_GetSize(*environmentLattice), ERR_DATACONSISTENCY);

    cpp_foreach(envState, *getEnvironmentLattice(SCONTEXT))
        span_Get(*latticeState, envState->EnvironmentId) = envState->ParticleId;

    return ERR_OK;
}

error_t SyncMainStateCountersToRunStatus(SCONTEXT_PARAM)
{
    let counterState = getMainCycleCounters(SCONTEXT);
    var headerData = getMainStateHeader(SCONTEXT)->Data;

    headerData->Cycles = counterState->Cycles;
    headerData->Mcs = counterState->Mcs;

    return ERR_OK;
}

error_t SyncMainStateMetaDataToRunStatus(SCONTEXT_PARAM)
{
    let rng = getMainRng(SCONTEXT);
    var data = getMainStateMetaData(SCONTEXT);

    data->RngState = rng->State;
    data->RngIncrease = rng->Inc;

    return ERR_OK;
}

error_t SyncSimulationStateToRunStatus(SCONTEXT_PARAM)
{
    ResynchronizeEnvironmentEnergyStatus(SCONTEXT);

    var error = SyncMainStateTrackerMappingToSimulation(SCONTEXT);
    return_if(error, error);

    error = SyncMainStateLatticeToRunStatus(SCONTEXT);
    return_if(error, error);

    error = SyncMainStateCountersToRunStatus(SCONTEXT);
    return_if(error, error);

    error = SyncMainStateMetaDataToRunStatus(SCONTEXT);

    return error;
}

error_t SaveSimulationState(SCONTEXT_PARAM)
{
    if (StateFlagsAreSet(SCONTEXT, STATE_FLG_PRERUN))
        SIMERROR = SaveWriteBufferToFile(FILE_PRERSTATE, FMODE_BINARY_W, getMainStateBuffer(SCONTEXT));
    else
        SIMERROR = SaveWriteBufferToFile(FILE_MAINSTATE, FMODE_BINARY_W, getMainStateBuffer(SCONTEXT));

    return SIMERROR;
}

static error_t GeneralSimulationFinish(SCONTEXT_PARAM)
{
    setMainStateFlags(SCONTEXT, STATE_FLG_COMPLETED);
    SIMERROR = SaveSimulationState(SCONTEXT);
    return SIMERROR;
}

error_t KMC_FinishMainRoutine(SCONTEXT_PARAM)
{
    SIMERROR = GeneralSimulationFinish(SCONTEXT);
    error_assert(SIMERROR, "Simulation aborted due to error in general simulation finisher routine execution.");
    return SIMERROR;
}

error_t MMC_FinishMainRoutine(SCONTEXT_PARAM)
{
    SIMERROR = GeneralSimulationFinish(SCONTEXT);
    error_assert(SIMERROR, "Simulation aborted due to error in general simulation finisher routine execution.");
    return SIMERROR;
}

static inline int32_t GetActiveJumpId(SCONTEXT_PARAM)
{
    let jumpMapping = getJumpDirectionMapping(SCONTEXT);
    let jumpSelection = getJumpSelectionInfo(SCONTEXT);
    return array_Get(*jumpMapping, JUMPPATH[0]->PositionVector.D, JUMPPATH[0]->ParticleId, jumpSelection->RelativeId);
}

// Sets the active jump direction and collection on the context (Requires start path entry to be set!)
static inline void SetActiveJumpDirectionAndCollection(SCONTEXT_PARAM)
{
    var jumpSelection = getJumpSelectionInfo(SCONTEXT);
    var cycleState = getCycleState(SCONTEXT);

    jumpSelection->JumpId = GetActiveJumpId(SCONTEXT);
    cycleState->ActiveJumpDirection = getJumpDirectionAt(SCONTEXT, jumpSelection->JumpId);
    cycleState->ActiveJumpCollection = getJumpCollectionAt(SCONTEXT, cycleState->ActiveJumpDirection->JumpCollectionId);
}

static inline void SetActivePathStartEnvironment(SCONTEXT_PARAM)
{
    JUMPPATH[0] = getEnvironmentStateAt(SCONTEXT, getJumpSelectionInfo(SCONTEXT)->EnvironmentId);
    SetCodeByteAt(&getCycleState(SCONTEXT)->ActiveStateCode, 0, JUMPPATH[0]->ParticleId);
    JUMPPATH[0]->PathId = 0;
}

static inline void SetActiveCounterCollection(SCONTEXT_PARAM)
{
    getCycleState(SCONTEXT)->ActiveCounterCollection = getMainStateCounterAt(SCONTEXT, JUMPPATH[0]->ParticleId);
}

// Sets the active jump status for the currently selected KMC on the context
static inline void KMC_SetActiveJumpStatus(SCONTEXT_PARAM)
{
    let statusArray = getJumpStatusArray(SCONTEXT);
    let jumpDirection = getActiveJumpDirection(SCONTEXT);

    debug_assert(!array_IndicesAreOutOfRange(*statusArray, vecCoorSet3(JUMPPATH[0]->PositionVector), jumpDirection->ObjectId));
    getCycleState(SCONTEXT)->ActiveJumpStatus = &array_Get(*statusArray, vecCoorSet3(JUMPPATH[0]->PositionVector), jumpDirection->ObjectId);
}

void KMC_SetNextJumpSelection(SCONTEXT_PARAM)
{
    KMC_RollNextJumpSelection(SCONTEXT);
    getCycleState(SCONTEXT)->ActiveStateCode = 0ULL;

    SetActivePathStartEnvironment(SCONTEXT);

    SetActiveJumpDirectionAndCollection(SCONTEXT);
    SetActiveCounterCollection(SCONTEXT);
}

void KMC_SetJumpPathProperties(SCONTEXT_PARAM)
{
    let latticeSizes = getLatticeSizeVector(SCONTEXT);
    var stateCode = &getCycleState(SCONTEXT)->ActiveStateCode;
    Vector4_t actVector;
    byte_t index = 1;

    cpp_foreach(relVector, getActiveJumpDirection(SCONTEXT)->JumpSequence)
    {
        actVector = AddAndTrimVector4(&JUMPPATH[0]->PositionVector, relVector, latticeSizes);
        JUMPPATH[index] = getEnvironmentStateByVector4(SCONTEXT, &actVector);

        SetCodeByteAt(stateCode, index, JUMPPATH[index]->ParticleId);
        JUMPPATH[index]->PathId = index;
        ++index;
    }
}

static inline void LinearSearchAndSetActiveJumpRule(SCONTEXT_PARAM)
{
    let stateCode = getPathStateCode(SCONTEXT);
    var cycleState = getCycleState(SCONTEXT);

    cpp_foreach(jumpRule, cycleState->ActiveJumpCollection->JumpRules)
    {
        if (jumpRule->StateCode0 == stateCode)
        {
            cycleState->ActiveJumpRule = jumpRule;
            return;
        }
    }
    cycleState->ActiveJumpRule = NULL;
}

static inline void LookupAndSetActJumpRule(SCONTEXT_PARAM)
{
    LinearSearchAndSetActiveJumpRule(SCONTEXT);
}

bool_t KMC_TrySetJumpRule(SCONTEXT_PARAM)
{
    LookupAndSetActJumpRule(SCONTEXT);
    return getActiveJumpRule(SCONTEXT) != NULL;
}

void KMC_SetJumpProperties(SCONTEXT_PARAM)
{
    KMC_SetActiveJumpStatus(SCONTEXT);
    KMC_SetStateEnergies(SCONTEXT);
}

void KMC_SetJumpProbabilities(SCONTEXT_PARAM)
{
    let factors = getPhysicalFactors(SCONTEXT);
    let jumpRule = getActiveJumpRule(SCONTEXT);
    var energyInfo = getJumpEnergyInfo(SCONTEXT);

    energyInfo->FieldInfluence = CalcElectricFieldInfluence(SCONTEXT);
    energyInfo->ConformationDelta = 0.5 * (energyInfo->Energy2 - energyInfo->Energy0);

    energyInfo->Energy0To2 = energyInfo->Energy1 + energyInfo->ConformationDelta + energyInfo->FieldInfluence;
    energyInfo->Energy2To0 = energyInfo->Energy1 - energyInfo->ConformationDelta - energyInfo->FieldInfluence;

    energyInfo->Probability0to2 = exp(-energyInfo->Energy0To2) * jumpRule->FrequencyFactor * factors->TotalNormalizationFactor;

    energyInfo->Probability2to0 = (energyInfo->Energy2To0 < 0.0) ? INFINITY : 0.0;
}

void KMC_OnEnergeticJumpEvaluation(SCONTEXT_PARAM)
{
    SCONTEXT->Plugins.OnSetJumpProbabilities(SCONTEXT);

    // Unstable end: Do not advance system, update counter and simulated time
    if (getJumpEnergyInfo(SCONTEXT)->Probability2to0 > MC_CONST_JUMPLIMIT_MAX)
    {
        KMC_OnJumpIsToUnstableState(SCONTEXT);
        return;
    }

    // Unstable start: Advance system, update counter but not simulated time, do pool update
    if (getJumpEnergyInfo(SCONTEXT)->Probability0to2 > MC_CONST_JUMPLIMIT_MAX)
    {
        KMC_OnJumpIsFromUnstableState(SCONTEXT);
        return;
    }
    
    // Successful jump: Advance system, update counters and simulated time, do pool update
    if (GetNextRandomDouble(SCONTEXT) < getJumpEnergyInfo(SCONTEXT)->Probability0to2)
    {
        KMC_OnJumpIsStatisticallyAccepted(SCONTEXT);
        return;
    }

    // Rejected jump: Do not advance system, update counter and simulated time, no pool update
    KMC_OnJumpIsStatisticallyRejected(SCONTEXT);
}

void MMC_SetNextJumpSelection(SCONTEXT_PARAM)
{
    MMC_RollNextJumpSelection(SCONTEXT);

    SetActiveCounterCollection(SCONTEXT);
    SetActivePathStartEnvironment(SCONTEXT);
    SetActiveJumpDirectionAndCollection(SCONTEXT);
}

void MMC_SetJumpPathProperties(SCONTEXT_PARAM)
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

bool_t MMC_TrySetJumpRule(SCONTEXT_PARAM)
{
    LookupAndSetActJumpRule(SCONTEXT);
    return getActiveJumpRule(SCONTEXT) != NULL;
}

void MMC_SetJumpProperties(SCONTEXT_PARAM)
{
    MMC_SetStateEnergies(SCONTEXT);
}

void MMC_SetJumpProbabilities(SCONTEXT_PARAM)
{
    var energyInfo = getJumpEnergyInfo(SCONTEXT);
    energyInfo->Energy0To2 = energyInfo->Energy2 - energyInfo->Energy0;
    energyInfo->Probability0to2 = exp(energyInfo->Energy0To2);
}

void MMC_OnEnergeticJumpEvaluation(SCONTEXT_PARAM)
{
    SCONTEXT->Plugins.OnSetJumpProbabilities(SCONTEXT);

    // Handle case where the end state is unstable
    if (getJumpEnergyInfo(SCONTEXT)->Probability2to0 > MC_CONST_JUMPLIMIT_MAX)
    {
        MMC_OnJumpIsToUnstableState(SCONTEXT);
        return;
    }

    // Handle case where the start state is unstable
    if (getJumpEnergyInfo(SCONTEXT)->Probability0to2 > MC_CONST_JUMPLIMIT_MAX)
    {
        MMC_OnJumpIsFromUnstableState(SCONTEXT);
        return;
    }

    // Handle case where the jump is statistically accepted
    if (GetNextRandomDouble(SCONTEXT) < getJumpEnergyInfo(SCONTEXT)->Probability0to2)
    {
        MMC_OnJumpIsStatisticallyAccepted(SCONTEXT);
        return;
    }

    // Handle case where the jump is statistically rejected
    MMC_OnJumpIsStatisticallyRejected(SCONTEXT);
}
