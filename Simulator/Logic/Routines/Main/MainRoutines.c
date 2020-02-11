//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	MainRoutines.c        		//
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
#include "Simulator/Logic/JumpSelection/JumpSelection.h"
#include "Simulator/Logic/Routines/Environment/EnvRoutines.h"
#include "Simulator/Logic/Routines/Helper/HelperRoutines.h"
#include "Simulator/Logic/Routines/Statistics/McStatistics.h"
#include "Simulator/Logic/Initializers/ContextInit/ContextInit.h"
#include "Simulator/Logic/Routines/Tracking/TransitionTracking.h"
#include "Framework/Basic/Macros/BinarySearch.h"
#include "InternalLibraries/Interfaces/ProgressPrint.h"
#include "Framework/Math/Random/Approx.h"

// Calculates the result of the exponential function depending on the settings
static inline double GetExpResult(SCONTEXT_PARAM, const double exponent)
{
    return SCONTEXT->UseExpApproximation ? Exp_Fast32_Rms(exponent) : exp(exponent);
}


// Advances the block counters of the main loop to the next step goal
static inline void AdvanceMainCycleCounterToNextStepGoal(SCONTEXT_PARAM)
{
    var counters = getMainCycleCounters(SCONTEXT);
    counters->NextExecutionPhaseGoalMcsCount += counters->McsCountPerExecutionPhase;
}

// Sets the simulation run info to the current program status
static inline void SetRuntimeInfoToCurrent(SCONTEXT_PARAM)
{
    var runInfo = getRuntimeInformation(SCONTEXT);
    runInfo->MainRoutineStartClock = clock();
    runInfo->PreviousBlockFinishClock = runInfo->MainRoutineStartClock;
}

// Sets the required active counter collection on the passed context
static inline void SetActiveCounterCollection(SCONTEXT_PARAM)
{
    var cycleState = getCycleState(SCONTEXT);
    cycleState->ActiveCounterCollection = getMainStateCounterAt(SCONTEXT, JUMPPATH[0]->ParticleId);
}

void PrepareForMainRoutine(SCONTEXT_PARAM)
{
    let cycleCounters = getMainCycleCounters(SCONTEXT);

    PrepareContextForSimulation(SCONTEXT);
    while (cycleCounters->NextExecutionPhaseGoalMcsCount <= cycleCounters->McsCount)
        AdvanceMainCycleCounterToNextStepGoal(SCONTEXT);

    SetRuntimeInfoToCurrent(SCONTEXT);
}

error_t StartMainSimulationRoutine(SCONTEXT_PARAM)
{
    runtime_assertion(!StateFlagsAreSet(SCONTEXT, STATE_FLG_SIMERROR), SIMERROR, "Cannot start main simulation routine, state error flag is set.");

    ProgressPrint_OnSimulationStart(SCONTEXT, stdout);
    if (JobInfoFlagsAreSet(SCONTEXT, INFO_FLG_KMC))
    {
        if (StateFlagsAreSet(SCONTEXT, STATE_FLG_PRERUN))
        {
            SIMERROR = KMC_StartPreRunRoutine(SCONTEXT);
            error_assert(SIMERROR, "Pre-run execution of main KMC routine aborted with an error");
        }
        return SIMERROR = KMC_StartMainRoutine(SCONTEXT);
    }
    if (JobInfoFlagsAreSet(SCONTEXT, INFO_FLG_MMC))
    {
        if (StateFlagsAreSet(SCONTEXT, STATE_FLG_PRERUN))
        {
            SIMERROR = MMC_StartPreRunRoutine(SCONTEXT);
            error_assert(SIMERROR, "Pre-run execution of main KMC routine aborted with an error");
        }
        return SIMERROR = MMC_StartMainRoutine(SCONTEXT);
    }

    error_assert(ERR_UNKNOWN, "Main routine starter skipped selection process. Neither MMC nor KMC flags is set.");
    return ERR_UNKNOWN; // GCC [-Wall] expects return value, even with exit(..) statement
}

error_t KMC_StartPreRunRoutine(SCONTEXT_PARAM)
{
    var abortFlag = KMC_UpdateAndCheckAbortConditions(SCONTEXT);
    while(abortFlag == STATE_FLG_CONTINUE)
    {
        SIMERROR = KMC_EnterSOPExecutionPhase(SCONTEXT);
        error_assert(SIMERROR, "Simulation abort due to error in KMC cycle block execution.");

        SIMERROR = KMC_FinishExecutionPhase(SCONTEXT);
        error_assert(SIMERROR, "Simulation abort due to error in KMC cycle block finisher execution.");

        abortFlag = KMC_UpdateAndCheckAbortConditions(SCONTEXT);
        ProgressPrint_OnBlockFinish(SCONTEXT, stdout, true);
    }
    return KMC_FinishPreRun(SCONTEXT);
}

error_t KMC_FinishPreRun(SCONTEXT_PARAM)
{
    SIMERROR = KMC_ResetContextAfterPreRun(SCONTEXT);
    return_if(SIMERROR, SIMERROR);

    setMainStateFlags(SCONTEXT, STATE_FLG_PRERUN_RESET);
    unSetMainStateFlags(SCONTEXT, STATE_FLG_PRERUN);

    ProgressPrint_OnContextReset(SCONTEXT, stdout);
    return ERR_OK;
}

// Starts the main kinetic simulation routine
error_t KMC_StartMainRoutine(SCONTEXT_PARAM)
{
    var abortFlag = KMC_UpdateAndCheckAbortConditions(SCONTEXT);
    while(abortFlag == STATE_FLG_CONTINUE)
    {
        SIMERROR = KMC_EnterExecutionPhase(SCONTEXT);
        error_assert(SIMERROR, "Simulation abort due to error in KMC cycle block execution.");

        SIMERROR = KMC_FinishExecutionPhase(SCONTEXT);
        error_assert(SIMERROR, "Simulation abort due to error in KMC cycle block finisher execution.");

        abortFlag = KMC_UpdateAndCheckAbortConditions(SCONTEXT);
        ProgressPrint_OnBlockFinish(SCONTEXT, stdout, true);
    }
    return KMC_FinishMainRoutine(SCONTEXT);
}

error_t MMC_StartPreRunRoutine(SCONTEXT_PARAM)
{
    return ERR_NOTIMPLEMENTED;
}

error_t MMC_FinishPreRun(SCONTEXT_PARAM)
{
    return ERR_NOTIMPLEMENTED;
}

// Starts the main metropolis simulation routine
error_t MMC_StartMainRoutine(SCONTEXT_PARAM)
{
    var abortFlag = MMC_UpdateAndCheckAbortConditions(SCONTEXT);
    while(abortFlag == STATE_FLG_CONTINUE)
    {
        SIMERROR = MMC_EnterExecutionPhase(SCONTEXT);
        error_assert(SIMERROR, "Simulation abort due to error in MMC cycle block execution.");

        SIMERROR = MMC_FinishExecutionPhase(SCONTEXT);
        error_assert(SIMERROR, "Simulation abort due to error in MMC cycle block finisher execution.");

        abortFlag = MMC_UpdateAndCheckAbortConditions(SCONTEXT);
        ProgressPrint_OnBlockFinish(SCONTEXT, stdout, true);
    }

    return MMC_FinishMainRoutine(SCONTEXT);
}

// Calls the output plugin callback if any is set
static inline error_t CallOutputPlugin(SCONTEXT_PARAM)
{
    return_if(SCONTEXT->Plugins.OnDataOutput == NULL, ERR_OK);
    let plugins = getPluginCollection(SCONTEXT);

    plugins->OnDataOutput(SCONTEXT);
    return SIMERROR;
}

// Writes the current jump delta energy of an MMC transition to the abort buffer (If the abort tolerance is equal or below 0, the buffer write is skipped)
static inline void MMC_WriteJumpEnergyToAbortBuffer(SCONTEXT_PARAM)
{
    let header = getDbModelJobHeaderAsMMC(SCONTEXT);
    return_if(header->AbortTolerance <= 0.0);

    var buffer = getLatticeEnergyBuffer(SCONTEXT);
    let energyInfo = getJumpEnergyInfo(SCONTEXT);
    let factors = getPhysicalFactors(SCONTEXT);

    if (buffer->End == buffer->CapacityEnd)
    {
        buffer->LastSum = buffer->CurrentSum;
        buffer->CurrentSum = 0;

        cpp_foreach(value, *buffer) buffer->CurrentSum += *value;
        buffer->CurrentSum *= factors->EnergyFactorKtToEv;
        buffer->End = buffer->Begin;
        return;
    }

    list_PushBack(*buffer, energyInfo->S0toS2DeltaEnergy);
}

// Action for cases where the MMC jump selection leads to an unstable end state
static inline void KMC_OnJumpIsToUnstableState(SCONTEXT_PARAM)
{
    var counters = getActiveCounters(SCONTEXT);

    ++counters->UnstableEndCount;
    KMC_AddCurrentJumpDataToHistograms(SCONTEXT);
    AdvanceSimulatedTimeByCurrentStep(SCONTEXT);
}

// Action for cases where the jump selection enables to leave a currently unstable state
static inline void KMC_OnJumpIsFromUnstableState(SCONTEXT_PARAM)
{
    var counters = getActiveCounters(SCONTEXT);

    ++counters->UnstableStartCount;
    KMC_AdvanceTransitionTrackingSystem(SCONTEXT);
    KMC_AdvanceSystemToFinalState(SCONTEXT);

    let jumpCountHasChanged = KMC_UpdateJumpPool(SCONTEXT);
    if (jumpCountHasChanged) UpdateTimeStepPerJumpToCurrent(SCONTEXT);
}

// Updates the maximum jump probability to a new value if required (Skips values above the jump-limit value)
static inline void UpdateMaxJumpProbabilityBackjumpUnsafe(SCONTEXT_PARAM)
{
    let energyInfo = getJumpEnergyInfo(SCONTEXT);
    var metaData = getMainStateMetaData(SCONTEXT);

    return_if(energyInfo->RawS0toS2Probability > MC_CONST_JUMPLIMIT_MAX);
    metaData->RawMaxJumpProbability = getMaxOfTwo(metaData->RawMaxJumpProbability, energyInfo->RawS0toS2Probability);
}

// Updates the maximum jump probability to a new value if required (Skips values above the jump-limit value & does a backjump check)
static inline void UpdateMaxJumpProbabilityBackjumpSafe(SCONTEXT_PARAM)
{
    let energyInfo = getJumpEnergyInfo(SCONTEXT);
    var metaData = getMainStateMetaData(SCONTEXT);

    return_if(energyInfo->RawS0toS2Probability > MC_CONST_JUMPLIMIT_MAX);
    metaData->RawMaxJumpProbability = getMaxOfTwo(metaData->RawMaxJumpProbability, energyInfo->RawS0toS2Probability);

    // Note: This is a safety check for the backjump to prevent the normalization system from accidentally over-normalizing
    return_if(energyInfo->S0toS2DeltaEnergy >= energyInfo->S2toS0DeltaEnergy);
    metaData->RawMaxJumpProbability = getMaxOfTwo(metaData->RawMaxJumpProbability, GetExpResult(SCONTEXT, -energyInfo->S2toS0DeltaEnergy));
}

// Action for cases where the jump selection has been statistically accepted
static inline void KMC_OnJumpIsStatisticallyAccepted(SCONTEXT_PARAM)
{
    var activeCounters = getActiveCounters(SCONTEXT);
    var cycleCounters = getMainCycleCounters(SCONTEXT);

    ++activeCounters->McsCount;
    ++cycleCounters->McsCount;

    UpdateMaxJumpProbabilityBackjumpUnsafe(SCONTEXT);

    AdvanceSimulatedTimeByCurrentStep(SCONTEXT);
    KMC_AdvanceTransitionTrackingSystem(SCONTEXT);
    KMC_AdvanceSystemToFinalState(SCONTEXT);

    let jumpCountHasChanged = KMC_UpdateJumpPool(SCONTEXT);
    if (jumpCountHasChanged) UpdateTimeStepPerJumpToCurrent(SCONTEXT);
    SCONTEXT->CycleResult = MC_ACCEPTED_CYCLE;
}

// Action for cases where the jump selection has been statistically rejected
static inline void KMC_OnJumpIsStatisticallyRejected(SCONTEXT_PARAM)
{
    var counters = getActiveCounters(SCONTEXT);
    ++counters->RejectionCount;
    AdvanceSimulatedTimeByCurrentStep(SCONTEXT);
    KMC_AddCurrentJumpDataToHistograms(SCONTEXT);
    SCONTEXT->CycleResult = MC_REJECTED_CYCLE;
}

// Action for cases where the jump selection has no valid rule and is site-blocking
static inline void KMC_OnJumpIsSiteBlocked(SCONTEXT_PARAM)
{
    var counters = getActiveCounters(SCONTEXT);
    ++counters->SiteBlockingCount;
    AdvanceSimulatedTimeByCurrentStep(SCONTEXT);
    SCONTEXT->CycleResult = MC_BLOCKED_CYCLE;
}

// Action that is called if a KMC jump is pre-skipped due to failed attempt frequency test
static inline void KMC_OnJumpIsFrequencySkipped(SCONTEXT_PARAM)
{
    SetActiveCounterCollection(SCONTEXT);
    let counters = getActiveCounters(SCONTEXT);
    ++counters->SkipCount;
    AdvanceSimulatedTimeByCurrentStep(SCONTEXT);
    SCONTEXT->CycleResult = MC_SKIPPED_CYCLE;
}

// Pre-check of the attempt frequency factor using another double roll [0;1]
static inline bool_t KMC_PrecheckAttemptFrequencyFactor(SCONTEXT_PARAM)
{
    #if defined(OPT_PRECHECK_FREQUENCY)
    let jumpRule = getActiveJumpRule(SCONTEXT);
    return (jumpRule->FrequencyFactor < OPT_FRQPRECHECK_LIMIT) && (jumpRule->FrequencyFactor < GetNextRandomDouble(SCONTEXT));
    #else
    return true;
    #endif
}

void KMC_ExecuteSimulationCycle(SCONTEXT_PARAM)
{
    KMC_SetNextJumpSelection(SCONTEXT);
    KMC_SetJumpPathProperties(SCONTEXT);

    if (KMC_TrySetActiveJumpRule(SCONTEXT))
    {
        #if defined(OPT_PRECHECK_FREQUENCY)
        if (KMC_PrecheckAttemptFrequencyFactor(SCONTEXT))
        {
            KMC_OnJumpIsFrequencySkipped(SCONTEXT);
            return;
        }
        #endif
        KMC_SetJumpProperties(SCONTEXT);
        KMC_OnEnergeticJumpEvaluation(SCONTEXT);
        return;
    }

    KMC_OnJumpIsSiteBlocked(SCONTEXT);
}

void KMC_ExecuteSOPSimulationCycle(SCONTEXT_PARAM)
{
    KMC_SetNextJumpSelection(SCONTEXT);
    KMC_SetJumpPathProperties(SCONTEXT);

    if (KMC_TrySetActiveJumpRule(SCONTEXT))
    {
        #if defined(OPT_PRECHECK_FREQUENCY)
        if (KMC_PrecheckAttemptFrequencyFactor(SCONTEXT))
        {
            KMC_OnJumpIsFrequencySkipped(SCONTEXT);
            return;
        }
        #endif
        KMC_SetJumpProperties(SCONTEXT);
        KMC_OnEnergeticJumpEvaluation(SCONTEXT);
        UpdateMaxJumpProbabilityBackjumpSafe(SCONTEXT);
        return;
    }

    KMC_OnJumpIsSiteBlocked(SCONTEXT);
}

error_t KMC_EnterExecutionPhase(SCONTEXT_PARAM)
{
    var counters = getMainCycleCounters(SCONTEXT);
    for (;counters->McsCount < counters->NextExecutionPhaseGoalMcsCount;)
    {
        let countPerLoop = counters->CycleCountPerExecutionLoop;
        for (int64_t i = 0; i < countPerLoop; ++i)
        {
            KMC_ExecuteSimulationCycle(SCONTEXT);
        }
        counters->CycleCount += countPerLoop;
        return_if(KMC_UpdateAndCheckAbortConditions(SCONTEXT) != STATE_FLG_CONTINUE, ERR_OK);
    }
    return SIMERROR;
}

error_t KMC_EnterSOPExecutionPhase(SCONTEXT_PARAM)
{
    var counters = getMainCycleCounters(SCONTEXT);
    for (;counters->McsCount < counters->NextExecutionPhaseGoalMcsCount;)
    {
        let countPerLoop = counters->CycleCountPerExecutionLoop;
        for (int64_t i = 0; i < countPerLoop; ++i)
        {
            KMC_ExecuteSOPSimulationCycle(SCONTEXT);
        }
        counters->CycleCount += countPerLoop;
        KMC_UpdateTotalJumpNormalization(SCONTEXT);
    }
    return ERR_OK;
}

void MC_DoCommonPhaseFinish(SCONTEXT_PARAM)
{
    unSetMainStateFlags(SCONTEXT, STATE_FLG_FIRSTCYCLE);

    SIMERROR = SyncSimulationStateToRunStatus(SCONTEXT);
    error_assert(SIMERROR, "Simulation aborted due to failed synchronization between dynamic model and state object.");

    SIMERROR = SaveSimulationState(SCONTEXT);
    error_assert(SIMERROR, "Simulation aborted due to error during serialization of the state object.");

    SIMERROR = CallOutputPlugin(SCONTEXT);
    error_assert(SIMERROR, "Simulation aborted due to error in the external output plugin.");
}

error_t KMC_FinishExecutionPhase(SCONTEXT_PARAM)
{
    AdvanceMainCycleCounterToNextStepGoal(SCONTEXT);
    MC_DoCommonPhaseFinish(SCONTEXT);
    return SIMERROR;
}

// Action for cases where the jump selection has been statistically accepted
static inline void MMC_OnJumpIsStatisticallyAccepted(SCONTEXT_PARAM)
{
    MMC_WriteJumpEnergyToAbortBuffer(SCONTEXT);
    var activeCounters = getActiveCounters(SCONTEXT);
    var cycleCounters = getMainCycleCounters(SCONTEXT);

    ++activeCounters->McsCount;
    ++cycleCounters->McsCount;

    UpdateMaxJumpProbabilityBackjumpUnsafe(SCONTEXT);
    MMC_AdvanceSystemToFinalState(SCONTEXT);
    MMC_UpdateJumpPool(SCONTEXT);
    SCONTEXT->CycleResult = MC_ACCEPTED_CYCLE;
}

// Action for cases where the jump selection has been statistically rejected
static inline void MMC_OnJumpIsStatisticallyRejected(SCONTEXT_PARAM)
{
    var counters = getActiveCounters(SCONTEXT);
    ++counters->RejectionCount;
    SCONTEXT->CycleResult = MC_REJECTED_CYCLE;
}

// Action for cases where the jump selection has no valid rule and is site-blocking
static inline void MMC_OnJumpIsSiteBlocked(SCONTEXT_PARAM)
{
    var counters = getActiveCounters(SCONTEXT);
    ++counters->SiteBlockingCount;
    SCONTEXT->CycleResult = MC_BLOCKED_CYCLE;
}

void MMC_ExecuteSimulationCycle(SCONTEXT_PARAM)
{
    MMC_SetNextJumpSelection(SCONTEXT);
    MMC_SetJumpPathProperties(SCONTEXT);

    if (MMC_TrySetActiveJumpRule(SCONTEXT))
    {
        MMC_SetJumpProperties(SCONTEXT);
        MMC_OnEnergeticJumpEvaluation(SCONTEXT);
        return;
    }

    MMC_OnJumpIsSiteBlocked(SCONTEXT);
}

error_t MMC_EnterExecutionPhase(SCONTEXT_PARAM)
{
    var counters = getMainCycleCounters(SCONTEXT);
    for (;counters->McsCount < counters->NextExecutionPhaseGoalMcsCount;)
    {
        for (int64_t i = 0; i < counters->CycleCountPerExecutionLoop; i++)
        {
            MMC_ExecuteSimulationCycle(SCONTEXT);
        }
        counters->CycleCount += counters->CycleCountPerExecutionLoop;
        return_if(MMC_UpdateAndCheckAbortConditions(SCONTEXT) != STATE_FLG_CONTINUE, ERR_OK);
    }
    return SIMERROR;
}

error_t MMC_FinishExecutionPhase(SCONTEXT_PARAM)
{
    AdvanceMainCycleCounterToNextStepGoal(SCONTEXT);
    MC_DoCommonPhaseFinish(SCONTEXT);
    return SIMERROR;
}

// Evaluates if the ETA of the simulation is likely to timeout during the next block
static inline bool_t UpdateAndEvaluateTimeoutAbortCondition(SCONTEXT_PARAM)
{
    let jobInfo = getDbModelJobInfo(SCONTEXT);
    var runInfo = getRuntimeInformation(SCONTEXT);
    var metaData = getMainStateMetaData(SCONTEXT);
    var newClock = clock();
    let deltaClock = newClock - runInfo->PreviousBlockFinishClock;

    metaData->TimePerBlock = (double) deltaClock / CLOCKS_PER_SEC;
    metaData->TimePerBlock = (metaData->TimePerBlock <= 0.0) ? (1.0 / CLOCKS_PER_SEC) : metaData->TimePerBlock;
    metaData->ProgramRunTime += metaData->TimePerBlock;

    var nextBlockEtaClock = newClock + deltaClock;
    runInfo->PreviousBlockFinishClock = newClock;

    bool_t isTimeout = nextBlockEtaClock > (jobInfo->TimeLimit * CLOCKS_PER_SEC);
    return isTimeout;
}

// Evaluates if the current success and cycles rates are below the limit
static inline bool_t UpdateAndEvaluateRateAbortConditions(SCONTEXT_PARAM)
{
    let jobInfo = getDbModelJobInfo(SCONTEXT);
    let counters = getMainCycleCounters(SCONTEXT);
    var metaData = getMainStateMetaData(SCONTEXT);

    return_if(metaData->ProgramRunTime == 0, false);

    metaData->SuccessRate = counters->McsCount / metaData->ProgramRunTime;
    metaData->CycleRate = counters->CycleCount / metaData->ProgramRunTime;
    return (metaData->CycleRate < jobInfo->MinimalSuccessRate);
}

// Evaluates if the mcs target is reached
static inline bool_t UpdateAndEvaluateMcsAbortCondition(SCONTEXT_PARAM)
{
    let counters = getMainCycleCounters(SCONTEXT);
    return (counters->McsCount >= counters->TotalSimulationGoalMcsCount);
}

// Evaluates the general abort conditions and returns the corresponding state flag
static error_t EvaluateGeneralAbortConditions(SCONTEXT_PARAM)
{
    if (UpdateAndEvaluateTimeoutAbortCondition(SCONTEXT))
    {
        // Note: Timeout evaluation does not yield usable statistics before a second of run time
        return_if(getMainStateMetaData(SCONTEXT)->ProgramRunTime < 1.0, STATE_FLG_CONTINUE);

        setMainStateFlags(SCONTEXT, STATE_FLG_TIMEOUT | STATE_FLG_CONDABORT);
        return STATE_FLG_TIMEOUT;
    }
    if (UpdateAndEvaluateRateAbortConditions(SCONTEXT))
    {
        setMainStateFlags(SCONTEXT, STATE_FLG_RATEABORT | STATE_FLG_CONDABORT);
        return STATE_FLG_RATEABORT;
    }
    if (UpdateAndEvaluateMcsAbortCondition(SCONTEXT))
    {
        setMainStateFlags(SCONTEXT, STATE_FLG_COMPLETED | STATE_FLG_CONDABORT);
        return STATE_FLG_COMPLETED;
    }
    return STATE_FLG_CONTINUE;
}

// Evaluates the pre run abort conditions
static error_t EvaluatePreRunAbortConditions(SCONTEXT_PARAM)
{
    return_if(!StateFlagsAreSet(SCONTEXT, STATE_FLG_PRERUN), STATE_FLG_CONTINUE);

    let counters = getMainCycleCounters(SCONTEXT);
    return (counters->McsCount >= counters->PrerunGoalMcs) ? STATE_FLG_PRERUN_RESET : STATE_FLG_CONTINUE;
}

error_t KMC_UpdateAndCheckAbortConditions(SCONTEXT_PARAM)
{
    error_t result = 0;
    result |= EvaluatePreRunAbortConditions(SCONTEXT);
    result |= EvaluateGeneralAbortConditions(SCONTEXT);
    return result;
}

// Checks if the fluctuation in the energy abort buffer indicates a relaxed system
static inline bool_t MMC_CheckEnergyRelaxationAbortCondition(SCONTEXT_PARAM)
{
    let metaData = getMainStateMetaData(SCONTEXT);
    let jobHeader = getDbModelJobHeaderAsMMC(SCONTEXT);
    let abortBuffer = getLatticeEnergyBuffer(SCONTEXT);

    if (jobHeader->AbortTolerance <= 0.0) return false;

    let limitFluctuation = fabs(metaData->LatticeEnergy * jobHeader->AbortTolerance);
    let currentFluctuation = abortBuffer->CurrentSum;

    return (isfinite(currentFluctuation)) ? (limitFluctuation >= fabs(currentFluctuation)) : false;
}

error_t MMC_UpdateAndCheckAbortConditions(SCONTEXT_PARAM)
{
    return_if(EvaluateGeneralAbortConditions(SCONTEXT) != STATE_FLG_CONTINUE, STATE_FLG_CONDABORT);

    if (MMC_CheckEnergyRelaxationAbortCondition(SCONTEXT))
    {
        setMainStateFlags(SCONTEXT, STATE_FLG_ENERGYABORT | STATE_FLG_CONDABORT);
        return STATE_FLG_CONDABORT;
    }
    return STATE_FLG_CONTINUE;
}

error_t SyncMainStateLatticeToRunStatus(SCONTEXT_PARAM)
{
    let environmentLattice = getEnvironmentLattice(SCONTEXT);
    var latticeState = getMainStateLattice(SCONTEXT);

    return_if(span_Length(*latticeState) != array_Length(*environmentLattice), ERR_DATACONSISTENCY);

    cpp_foreach(envState, *getEnvironmentLattice(SCONTEXT))
        span_Get(*latticeState, getEnvironmentStateIdByPointer(SCONTEXT, envState)) = envState->ParticleId;

    return ERR_OK;
}

error_t SyncMainStateCountersToRunStatus(SCONTEXT_PARAM)
{
    let counterState = getMainCycleCounters(SCONTEXT);
    var headerData = getMainStateHeader(SCONTEXT)->Data;

    headerData->Cycles = counterState->CycleCount;
    headerData->Mcs = counterState->McsCount;
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
    return_if(JobInfoFlagsAreSet(SCONTEXT, INFO_FLG_SKIPSAVE), ERR_OK);
    let stateBuffer = getMainStateBuffer(SCONTEXT);
    let targetFile = StateFlagsAreSet(SCONTEXT, STATE_FLG_PRERUN)
            ? getPreRunStateFile(SCONTEXT)
            : getMainRunStateFile(SCONTEXT);

    return SIMERROR = SaveWriteBufferToFile(targetFile, FMODE_BINARY_W, stateBuffer);
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

// Get the currently active jump id drom the simulation context
static inline int32_t GetActiveJumpId(SCONTEXT_PARAM)
{
    let jumpMapping = getJumpDirectionMapping(SCONTEXT);
    let jumpSelection = getJumpSelectionInfo(SCONTEXT);
    return array_Get(*jumpMapping, JUMPPATH[0]->PositionVector.D, JUMPPATH[0]->ParticleId, jumpSelection->RelativeJumpId);
}

// Sets the active jump direction and collection on the context (Requires start path entry to be set!)
static inline void SetActiveJumpDirectionAndCollection(SCONTEXT_PARAM)
{
    var jumpSelection = getJumpSelectionInfo(SCONTEXT);
    var cycleState = getCycleState(SCONTEXT);

    jumpSelection->GlobalJumpId = GetActiveJumpId(SCONTEXT);
    cycleState->ActiveJumpDirection = getJumpDirectionAt(SCONTEXT, jumpSelection->GlobalJumpId);
    cycleState->ActiveJumpCollection = getJumpCollectionAt(SCONTEXT, cycleState->ActiveJumpDirection->JumpCollectionId);
}

// Set the path start environment using the current internal state of the context
static inline void SetActivePathStartEnvironment(SCONTEXT_PARAM)
{
    let selectionInfo = getJumpSelectionInfo(SCONTEXT);
    var cycleState = getCycleState(SCONTEXT);

    JUMPPATH[0] = getEnvironmentStateAt(SCONTEXT, selectionInfo->EnvironmentId);
    SetCodeByteAt(&cycleState->ActiveStateCode, 0, JUMPPATH[0]->ParticleId);
    JUMPPATH[0]->PathId = 0;
}

// Sets the active jump status for the currently selected KMC on the context
static inline void KMC_SetActiveJumpStatus(SCONTEXT_PARAM)
{
    let statusArray = getJumpStatusArray(SCONTEXT);
    let direction = getActiveJumpDirection(SCONTEXT);
    var cycleState = getCycleState(SCONTEXT);

    debug_assert(!array_IndicesAreOutOfRange(*statusArray, vecCoorSet3(JUMPPATH[0]->PositionVector), direction->ObjectId));
    cycleState->ActiveJumpStatus = &array_Get(*statusArray, vecCoorSet3(JUMPPATH[0]->PositionVector), direction->ObjectId);
}

void KMC_SetNextJumpSelection(SCONTEXT_PARAM)
{
    var cycleState = getCycleState(SCONTEXT);
    cycleState->ActiveStateCode = 0ULL;

    KMC_RollNextJumpSelection(SCONTEXT);
    SetActivePathStartEnvironment(SCONTEXT);
    SetActiveJumpDirectionAndCollection(SCONTEXT);
    SetActiveCounterCollection(SCONTEXT);
}

// Sets the jump path property of one step by the step index
static inline void KMC_SetJumpPathPropertyByIndex(SCONTEXT_PARAM, int32_t stepIndex, const Vector4_t*restrict baseVector, const JumpSequence_t* restrict jumpSequence, const Vector4_t*restrict latticeSizes, OccupationCode64_t*restrict stateCode)
{
    let actVector = AddAndTrimVector4(baseVector, &span_Get(*jumpSequence, stepIndex), latticeSizes);
    var envState = getEnvironmentStateByVector4(SCONTEXT, &actVector);
    ++stepIndex;
    envState->PathId = stepIndex;
    SetCodeByteAt(stateCode, stepIndex, envState->ParticleId);
    JUMPPATH[stepIndex] = envState;
}

void KMC_SetJumpPathProperties(SCONTEXT_PARAM)
{
    let latticeSizes = getLatticeSizeVector(SCONTEXT);
    let jumpSequence = &getActiveJumpDirection(SCONTEXT)->JumpSequence;
    let length = span_Length(*jumpSequence);
    let baseVector = &JUMPPATH[0]->PositionVector;
    var stateCode = &getCycleState(SCONTEXT)->ActiveStateCode;
    switch (length)
    {
        case 7:
            KMC_SetJumpPathPropertyByIndex(SCONTEXT, 6, baseVector, jumpSequence, latticeSizes, stateCode);
        case 6:
            KMC_SetJumpPathPropertyByIndex(SCONTEXT, 5, baseVector, jumpSequence, latticeSizes, stateCode);
        case 5:
            KMC_SetJumpPathPropertyByIndex(SCONTEXT, 4, baseVector, jumpSequence, latticeSizes, stateCode);
        case 4:
            KMC_SetJumpPathPropertyByIndex(SCONTEXT, 3, baseVector, jumpSequence, latticeSizes, stateCode);
        case 3:
            KMC_SetJumpPathPropertyByIndex(SCONTEXT, 2, baseVector, jumpSequence, latticeSizes, stateCode);
        case 2:
            KMC_SetJumpPathPropertyByIndex(SCONTEXT, 0, baseVector, jumpSequence, latticeSizes, stateCode);
            KMC_SetJumpPathPropertyByIndex(SCONTEXT, 1, baseVector, jumpSequence, latticeSizes, stateCode);
            break;
        default:
            break;
    }
}

// Deprecated loop version of KMC_SetJumpPathProperties
//void KMC_SetJumpPathProperties(SCONTEXT_PARAM)
//{
//    let latticeSizes = getLatticeSizeVector(SCONTEXT);
//    var stateCode = &getCycleState(SCONTEXT)->ActiveStateCode;
//    let baseVector = &JUMPPATH[0]->PositionVector;
//    byte_t index = 1;
//    cpp_foreach(relVector, getActiveJumpDirection(SCONTEXT)->JumpSequence)
//    {
//        let actVector = AddAndTrimVector4(baseVector, relVector, latticeSizes);
//        let envState = getEnvironmentStateByVector4(SCONTEXT, &actVector);
//        SetCodeByteAt(stateCode, index, envState->ParticleId);
//        envState->PathId = index;
//        JUMPPATH[index] = envState;
//        ++index;
//    }
//}

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

static inline void FindAndSetActiveJumpRule(SCONTEXT_PARAM)
{
    LinearSearchAndSetActiveJumpRule(SCONTEXT);
}

JumpRule_t* KMC_TrySetActiveJumpRule(SCONTEXT_PARAM)
{
    FindAndSetActiveJumpRule(SCONTEXT);
    return getActiveJumpRule(SCONTEXT);
}

void KMC_SetJumpProperties(SCONTEXT_PARAM)
{
    KMC_SetActiveJumpStatus(SCONTEXT);
    KMC_SetStateEnergies(SCONTEXT);
}

void KMC_SetJumpProbabilities(SCONTEXT_PARAM)
{
    var energyInfo = getJumpEnergyInfo(SCONTEXT);

    energyInfo->ConformationDeltaEnergy = energyInfo->S2Energy - energyInfo->S0Energy;

    energyInfo->S0toS2DeltaEnergy = energyInfo->S1Energy - energyInfo->S0Energy + energyInfo->ElectricFieldEnergy;
    energyInfo->S2toS0DeltaEnergy = energyInfo->S1Energy - energyInfo->S2Energy - energyInfo->ElectricFieldEnergy;

    energyInfo->RawS0toS2Probability = GetExpResult(SCONTEXT, -energyInfo->S0toS2DeltaEnergy);
    energyInfo->RawS2toS0Probability = (energyInfo->S2toS0DeltaEnergy < MC_CONST_JUMPLIMIT_MIN)
            ? MC_CONST_BACKJUMP_INF
            : MC_CONST_BACKJUMP_NULL;

    energyInfo->NormalizedS0toS2Probability = energyInfo->RawS0toS2Probability * GetCurrentProbabilityPreFactor(SCONTEXT);
}

// Default internal S1 calculation function that uses the 0.5 * (S_2 - S_0) interpolation method
static void KMC_SetTransitionStateEnergyDefault(double* restrict states)
{
    states[1] += 0.5 * (states[0] + states[2]);
}

// Alternate internal S1 calculation function that uses the factor interpolation method with a shift alpha
static void KMC_SetTransitionStateEnergyAlphaMethod(double* restrict states, const double alpha)
{
    let deltaConf = states[2] - states[0];
    let deltaAbs = fabs(deltaConf);
    states[1] += states[0] + 0.5 * (deltaConf - deltaAbs) + alpha * deltaAbs;
}

void KMC_OnEnergeticJumpEvaluation(SCONTEXT_PARAM)
{
    let plugins = getPluginCollection(SCONTEXT);
    let energyInfo = getJumpEnergyInfo(SCONTEXT);

    // Use the internal energy function of no plugin is set
    if (plugins->OnSetTransitionStateEnergy == NULL)
    {
        KMC_SetTransitionStateEnergyDefault(&energyInfo->S0Energy);
    }
    else
    {
        plugins->OnSetTransitionStateEnergy(&energyInfo->S0Energy);
    }

    // Calculates the probabilities from the set state energies
    KMC_SetJumpProbabilities(SCONTEXT);

    // Unstable end: Do not advance system, update counter and simulated time
    if (energyInfo->RawS2toS0Probability == MC_CONST_BACKJUMP_INF)
    {
        KMC_OnJumpIsToUnstableState(SCONTEXT);
        return;
    }
    // Unstable start: Advance system, update counter but not simulated time, do pool update
    if (energyInfo->NormalizedS0toS2Probability > MC_CONST_JUMPLIMIT_MAX)
    {
        KMC_OnJumpIsFromUnstableState(SCONTEXT);
        return;
    }
    // Successful jump: Advance system, update counters and simulated time, do pool update
    let random = GetNextRandomDouble(SCONTEXT);
    if (energyInfo->NormalizedS0toS2Probability >= random)
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
    SetActivePathStartEnvironment(SCONTEXT);
    SetActiveCounterCollection(SCONTEXT);
    SetActiveJumpDirectionAndCollection(SCONTEXT);
}

void MMC_SetJumpPathProperties(SCONTEXT_PARAM)
{
    let selectionInfo = getJumpSelectionInfo(SCONTEXT);
    let jumpVector = getActiveJumpDirection(SCONTEXT)->JumpSequence.Begin;
    let lattice = getEnvironmentLattice(SCONTEXT);
    var cycleState = getCycleState(SCONTEXT);

    // Translate the offset index into the target environment
    let offsetVector = &getEnvironmentStateAt(SCONTEXT, selectionInfo->MmcOffsetSourceId)->PositionVector;
    var pathState1 = &array_Get(*lattice, offsetVector->A, offsetVector->B, offsetVector->C, jumpVector->D);
    pathState1->PathId = 1;

    // Correct the active state code byte and set the path id of the second environment state
    SetCodeByteAt(&cycleState->ActiveStateCode, 1, pathState1->ParticleId);
    JUMPPATH[1] = pathState1;
}

JumpRule_t* MMC_TrySetActiveJumpRule(SCONTEXT_PARAM)
{
    FindAndSetActiveJumpRule(SCONTEXT);
    return getActiveJumpRule(SCONTEXT);
}

void MMC_SetJumpProperties(SCONTEXT_PARAM)
{
    MMC_SetStateEnergies(SCONTEXT);
}

void MMC_SetJumpProbabilities(SCONTEXT_PARAM)
{
    var energyInfo = getJumpEnergyInfo(SCONTEXT);

    energyInfo->S0toS2DeltaEnergy = energyInfo->S2Energy - energyInfo->S0Energy;
    energyInfo->RawS0toS2Probability = GetExpResult(SCONTEXT, -energyInfo->S0toS2DeltaEnergy);
    energyInfo->NormalizedS0toS2Probability = energyInfo->RawS0toS2Probability;
}

void MMC_OnEnergeticJumpEvaluation(SCONTEXT_PARAM)
{
    let energyInfo = getJumpEnergyInfo(SCONTEXT);

    MMC_SetJumpProbabilities(SCONTEXT);

    // Handle case where the jump is statistically accepted
    let random = GetNextRandomDouble(SCONTEXT);
    if (energyInfo->NormalizedS0toS2Probability >= random)
    {
        MMC_OnJumpIsStatisticallyAccepted(SCONTEXT);
        return;
    }
    // Handle case where the jump is statistically rejected
    MMC_OnJumpIsStatisticallyRejected(SCONTEXT);
}

/* Special MMC routines with alpha factor */

void MMC_ExecuteSimulationCycle_WithAlpha(SCONTEXT_PARAM, const double alpha)
{
    MMC_SetNextJumpSelection(SCONTEXT);
    MMC_SetJumpPathProperties(SCONTEXT);

    if (MMC_TrySetActiveJumpRule(SCONTEXT))
    {
        MMC_SetJumpProperties(SCONTEXT);
        MMC_OnEnergeticJumpEvaluation_WithAlpha(SCONTEXT, alpha);
        return;
    }

    MMC_OnJumpIsSiteBlocked(SCONTEXT);
}

void MMC_SetJumpProbabilities_WithAlpha(SCONTEXT_PARAM, const double alpha)
{
    var energyInfo = getJumpEnergyInfo(SCONTEXT);

    energyInfo->S0toS2DeltaEnergy = energyInfo->S2Energy - energyInfo->S0Energy;
    energyInfo->RawS0toS2Probability = GetExpResult(SCONTEXT, -energyInfo->S0toS2DeltaEnergy * alpha);
    energyInfo->NormalizedS0toS2Probability = energyInfo->RawS0toS2Probability;
}

void MMC_OnEnergeticJumpEvaluation_WithAlpha(SCONTEXT_PARAM, const double alpha)
{
    let energyInfo = getJumpEnergyInfo(SCONTEXT);

    MMC_SetJumpProbabilities_WithAlpha(SCONTEXT, alpha);

    // Handle case where the jump is statistically accepted
    let random = GetNextRandomDouble(SCONTEXT);
    if (energyInfo->NormalizedS0toS2Probability >= random)
    {
        MMC_OnJumpIsStatisticallyAccepted(SCONTEXT);
        return;
    }
    // Handle case where the jump is statistically rejected
    MMC_OnJumpIsStatisticallyRejected(SCONTEXT);
}
