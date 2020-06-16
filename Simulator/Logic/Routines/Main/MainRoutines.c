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
static inline double GetExpResult(SCONTEXT_PARAMETER, const double exponent)
{
    return simContext->IsExpApproximationActive ? FastExp32RmsError(exponent) : exp(exponent);
}


// Advances the block counters of the main loop to the next step goal
static inline void AdvanceMainCycleCounterToNextStepGoal(SCONTEXT_PARAMETER)
{
    var counters = getMainCycleCounters(simContext);
    counters->NextExecutionPhaseGoalMcsCount += counters->McsCountPerExecutionPhase;
}

// Sets the simulation run info to the current program status
static inline void SetRuntimeInfoToCurrent(SCONTEXT_PARAMETER)
{
    var runInfo = getRuntimeInformation(simContext);
    runInfo->MainRoutineStartClock = clock();
    runInfo->PreviousBlockFinishClock = runInfo->MainRoutineStartClock;
}

// Sets the required active counter collection on the passed context
static inline void SetActiveCounterCollection(SCONTEXT_PARAMETER)
{
    var cycleState = getCycleState(simContext);
    cycleState->ActiveCounterCollection = getMainStateCounterAt(simContext, JUMPPATH[0]->ParticleId);
}

void PrepareSimulationContextForMainRoutine(SCONTEXT_PARAMETER)
{
    let cycleCounters = getMainCycleCounters(simContext);

    InitializeContextForSimulation(simContext);
    while (cycleCounters->NextExecutionPhaseGoalMcsCount <= cycleCounters->McsCount)
        AdvanceMainCycleCounterToNextStepGoal(simContext);

    SetRuntimeInfoToCurrent(simContext);
}

static void ExitIfAlreadyCompleted(SCONTEXT_PARAMETER)
{
    let counters = getMainCycleCounters(simContext);
    if (counters->McsCount >= counters->TotalSimulationGoalMcsCount)
    {
        printf("Simulation already completed!\n");
        exit(0);
    }
}

error_t StartMainSimulationRoutine(SCONTEXT_PARAMETER)
{
    assert_true(!StateFlagsAreSet(simContext, STATE_FLG_SIMERROR), SIMERROR, "Cannot start main simulation routine, state error flag is set.");

    ExitIfAlreadyCompleted(simContext);

    PrintMocassinSimulationStartInfo(simContext, stdout);
    if (JobInfoFlagsAreSet(simContext, INFO_FLG_KMC))
    {
        if (StateFlagsAreSet(simContext, STATE_FLG_PRERUN))
        {
            SIMERROR = StartKmcPreRunRoutine(simContext);
            assert_success(SIMERROR, "Pre-run execution of main KMC routine aborted with an error");
        }
        return SIMERROR = StartKmcMainRoutine(simContext);
    }
    if (JobInfoFlagsAreSet(simContext, INFO_FLG_MMC))
    {
        if (StateFlagsAreSet(simContext, STATE_FLG_PRERUN))
        {
            SIMERROR = StartMmcPreRunRoutine(simContext);
            assert_success(SIMERROR, "Pre-run execution of main MMC routine aborted with an error");
        }
        return SIMERROR = StartMmcMainRoutine(simContext);
    }

    assert_success(ERR_UNKNOWN, "Main routine starter skipped selection process. Neither MMC nor KMC flags is set.");
    return ERR_UNKNOWN; // GCC [-Wall] expects return value, even with exit(..) statement
}

error_t StartKmcPreRunRoutine(SCONTEXT_PARAMETER)
{
    var abortFlag = UpdateAndEvaluateKmcAbortConditions(simContext);
    while(abortFlag == STATE_FLG_CONTINUE)
    {
        SIMERROR = RunOneKmcAutoOptimizationExecutionBlock(simContext);
        assert_success(SIMERROR, "Simulation abort due to error in KMC cycle block execution.");

        SIMERROR = FinishKmcExecutionBlock(simContext);
        assert_success(SIMERROR, "Simulation abort due to error in KMC cycle block finisher execution.");

        abortFlag = UpdateAndEvaluateKmcAbortConditions(simContext);
        PrintMocassinSimulationBlockInfo(simContext, stdout, true);
    }
    return FinishKmcPreRunRoutine(simContext);
}

error_t FinishKmcPreRunRoutine(SCONTEXT_PARAMETER)
{
    SIMERROR = ResetContextAfterKmcPreRun(simContext);
    return_if(SIMERROR, SIMERROR);

    setMainStateFlags(simContext, STATE_FLG_PRERUN_RESET);
    unSetMainStateFlags(simContext, STATE_FLG_PRERUN);

    PrintMocassinSimulationContextResetInfo(simContext, stdout);
    return ERR_OK;
}

// Starts the main kinetic simulation routine
error_t StartKmcMainRoutine(SCONTEXT_PARAMETER)
{
    var abortFlag = UpdateAndEvaluateKmcAbortConditions(simContext);
    while(abortFlag == STATE_FLG_CONTINUE)
    {
        SIMERROR = RunOneKmcExecutionBlock(simContext);
        assert_success(SIMERROR, "Simulation abort due to error in KMC cycle block execution.");

        SIMERROR = FinishKmcExecutionBlock(simContext);
        assert_success(SIMERROR, "Simulation abort due to error in KMC cycle block finisher execution.");

        abortFlag = UpdateAndEvaluateKmcAbortConditions(simContext);
        PrintMocassinSimulationBlockInfo(simContext, stdout, true);
    }
    return FinishMainKmcRoutine(simContext);
}

error_t StartMmcPreRunRoutine(SCONTEXT_PARAMETER)
{
    return ERR_NOTIMPLEMENTED;
}

error_t FinishMmcPreRunRoutine(SCONTEXT_PARAMETER)
{
    return ERR_NOTIMPLEMENTED;
}

// Starts the main metropolis simulation routine
error_t StartMmcMainRoutine(SCONTEXT_PARAMETER)
{
    var abortFlag = UpdateAndEvaluateMmcAbortConditions(simContext);
    while(abortFlag == STATE_FLG_CONTINUE)
    {
        SIMERROR = RunOneMmcExecutionBlock(simContext);
        assert_success(SIMERROR, "Simulation abort due to error in MMC cycle block execution.");

        SIMERROR = FinishMmcExecutionBlock(simContext);
        assert_success(SIMERROR, "Simulation abort due to error in MMC cycle block finisher execution.");

        abortFlag = UpdateAndEvaluateMmcAbortConditions(simContext);
        PrintMocassinSimulationBlockInfo(simContext, stdout, true);
    }

    return FinishMmcMainRoutine(simContext);
}

// Calls the output plugin callback if any is set
static inline error_t TryCallOutputPlugin(SCONTEXT_PARAMETER)
{
    return_if(simContext->Plugins.OnDataOutput == NULL, ERR_OK);
    let plugins = getPluginCollection(simContext);

    plugins->OnDataOutput(simContext);
    return SIMERROR;
}

// Writes the current jump delta energy of an MMC transition to the abort buffer (If the abort tolerance is equal or below 0, the buffer write is skipped)
static inline void WriteMmcJumpEnergyToAbortBuffer(SCONTEXT_PARAMETER)
{
    let header = getDbModelJobHeaderAsMMC(simContext);
    return_if(header->AbortTolerance <= 0.0);

    var buffer = getLatticeEnergyBuffer(simContext);
    let energyInfo = getJumpEnergyInfo(simContext);
    let factors = getPhysicalFactors(simContext);

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
static inline void OnKmcEventEndStateIsUnstable(SCONTEXT_PARAMETER)
{
    var counters = getActiveCounters(simContext);

    ++counters->UnstableEndCount;
    AddCurrentKmcTransitionDataToHistograms(simContext);
    AdvanceSimulatedTimeByCurrentStep(simContext);
}

// Action for cases where the jump selection enables to leave a currently unstable state
static inline void OnKmcEventStartStateIsUnstable(SCONTEXT_PARAMETER)
{
    var counters = getActiveCounters(simContext);

    ++counters->UnstableStartCount;
    AdvanceKmcTransitionTrackingSystem(simContext);
    AdvanceKmcSystemToFinalState(simContext);

    let jumpCountHasChanged = UpdateTransitionPoolAfterKmcSystemAdvance(simContext);
    if (jumpCountHasChanged) UpdateTimeStepPerJumpToCurrent(simContext);
}

// Updates the maximum jump probability to a new value if required (Skips values above the jump-limit value)
static inline void UpdateMaxJumpProbabilityBackjumpUnsafe(SCONTEXT_PARAMETER)
{
    let energyInfo = getJumpEnergyInfo(simContext);
    var metaData = getMainStateMetaData(simContext);

    return_if(energyInfo->RawS0toS2Probability > MC_CONST_JUMPLIMIT_MAX);
    metaData->RawMaxJumpProbability = getMaxOfTwo(metaData->RawMaxJumpProbability, energyInfo->RawS0toS2Probability);
}

// Updates the maximum jump probability to a new value if required (Skips values above the jump-limit value & does a backjump check)
static inline void UpdateMaxJumpProbabilityBackjumpSafe(SCONTEXT_PARAMETER)
{
    let energyInfo = getJumpEnergyInfo(simContext);
    var metaData = getMainStateMetaData(simContext);

    return_if(energyInfo->RawS0toS2Probability > MC_CONST_JUMPLIMIT_MAX);
    metaData->RawMaxJumpProbability = getMaxOfTwo(metaData->RawMaxJumpProbability, energyInfo->RawS0toS2Probability);

    // Note: This is a safety check for the backjump to prevent the normalization system from accidentally over-normalizing
    return_if(energyInfo->S0toS2DeltaEnergy >= energyInfo->S2toS0DeltaEnergy);
    metaData->RawMaxJumpProbability = getMaxOfTwo(metaData->RawMaxJumpProbability, GetExpResult(simContext, -energyInfo->S2toS0DeltaEnergy));
}

// Action for cases where the jump selection has been statistically accepted
static inline void OnKmcEventIsAccepted(SCONTEXT_PARAMETER)
{
    var activeCounters = getActiveCounters(simContext);
    var cycleCounters = getMainCycleCounters(simContext);

    ++activeCounters->McsCount;
    ++cycleCounters->McsCount;

    AdvanceSimulatedTimeByCurrentStep(simContext);
    AdvanceKmcTransitionTrackingSystem(simContext);
    AdvanceKmcSystemToFinalState(simContext);

    let jumpCountHasChanged = UpdateTransitionPoolAfterKmcSystemAdvance(simContext);
    if (jumpCountHasChanged) UpdateTimeStepPerJumpToCurrent(simContext);
    simContext->CycleResult = MC_ACCEPTED_CYCLE;
}

// Action for cases where the jump selection has been statistically rejected
static inline void OnKmcEventIsRejected(SCONTEXT_PARAMETER)
{
    var counters = getActiveCounters(simContext);
    ++counters->RejectionCount;
    AdvanceSimulatedTimeByCurrentStep(simContext);
    AddCurrentKmcTransitionDataToHistograms(simContext);
    simContext->CycleResult = MC_REJECTED_CYCLE;
}

// Action for cases where the jump selection has no valid rule and is site-blocking
static inline void OnKmcEventIsSiteBlocked(SCONTEXT_PARAMETER)
{
    var counters = getActiveCounters(simContext);
    ++counters->SiteBlockingCount;
    AdvanceSimulatedTimeByCurrentStep(simContext);
    simContext->CycleResult = MC_BLOCKED_CYCLE;
}

// Action that is called if a KMC jump is pre-skipped due to failed attempt frequency test
static inline void OnKmcEventIsFrequencySkipped(SCONTEXT_PARAMETER)
{
    SetActiveCounterCollection(simContext);
    let counters = getActiveCounters(simContext);
    ++counters->SkipCount;
    AdvanceSimulatedTimeByCurrentStep(simContext);
    simContext->CycleResult = MC_SKIPPED_CYCLE;
}

// Pre-check of the attempt frequency factor using another double roll [0;1]
static inline bool_t CheckKmcEventFrequencySkip(SCONTEXT_PARAMETER)
{
    #if defined(OPT_PRECHECK_FREQUENCY)
    let jumpRule = getActiveJumpRule(simContext);
    return (jumpRule->FrequencyFactor < OPT_FRQPRECHECK_LIMIT) && (jumpRule->FrequencyFactor <
            GetNextRandomDoubleFromContextRng(simContext));
    #else
    return true;
    #endif
}

void ExecuteKmcSimulationCycle(SCONTEXT_PARAMETER)
{
    SetNextKmcJumpSelectionOnContext(simContext);
    SetKmcJumpPathPropertiesOnContext(simContext);

    if (TrySetActiveKmcJumpRuleOnContext(simContext))
    {
        #if defined(OPT_PRECHECK_FREQUENCY)
        if (CheckKmcEventFrequencySkip(simContext))
        {
            OnKmcEventIsFrequencySkipped(simContext);
            return;
        }
        #endif
        SetKmcJumpPropertiesOnContext(simContext);
        SetEnergeticKmcEventEvaluationOnContext(simContext);
        return;
    }

    OnKmcEventIsSiteBlocked(simContext);
}

void ExecuteKmcAutoOptimizingSimulationCycle(SCONTEXT_PARAMETER)
{
    SetNextKmcJumpSelectionOnContext(simContext);
    SetKmcJumpPathPropertiesOnContext(simContext);

    if (TrySetActiveKmcJumpRuleOnContext(simContext))
    {
        #if defined(OPT_PRECHECK_FREQUENCY)
        if (CheckKmcEventFrequencySkip(simContext))
        {
            OnKmcEventIsFrequencySkipped(simContext);
            return;
        }
        #endif
        SetKmcJumpPropertiesOnContext(simContext);
        SetEnergeticKmcEventEvaluationOnContext(simContext);
        UpdateMaxJumpProbabilityBackjumpSafe(simContext);
        return;
    }

    OnKmcEventIsSiteBlocked(simContext);
}

error_t RunOneKmcExecutionBlock(SCONTEXT_PARAMETER)
{
    var counters = getMainCycleCounters(simContext);
    for (;counters->McsCount < counters->NextExecutionPhaseGoalMcsCount;)
    {
        let countPerLoop = counters->CycleCountPerExecutionLoop;
        for (int64_t i = 0; i < countPerLoop; ++i)
        {
            ExecuteKmcSimulationCycle(simContext);
        }
        counters->CycleCount += countPerLoop;
        return_if(UpdateAndEvaluateKmcAbortConditions(simContext) != STATE_FLG_CONTINUE, ERR_OK);
    }
    return SIMERROR;
}

error_t RunOneKmcAutoOptimizationExecutionBlock(SCONTEXT_PARAMETER)
{
    var counters = getMainCycleCounters(simContext);
    for (;counters->McsCount < counters->NextExecutionPhaseGoalMcsCount;)
    {
        let countPerLoop = counters->CycleCountPerExecutionLoop;
        for (int64_t i = 0; i < countPerLoop; ++i)
        {
            ExecuteKmcAutoOptimizingSimulationCycle(simContext);
        }
        counters->CycleCount += countPerLoop;
        UpdateTotalKmcJumpNormalization(simContext);
    }
    return ERR_OK;
}

void ExecuteSharedMcBlockFinisher(SCONTEXT_PARAMETER)
{
    unSetMainStateFlags(simContext, STATE_FLG_FIRSTCYCLE);

    SIMERROR = SyncSimulationStateToRunStatus(simContext);
    assert_success(SIMERROR, "Simulation aborted due to failed synchronization between dynamic model and state object.");

    SIMERROR = SaveCurrentSimulationStateToOutDirectory(simContext);
    assert_success(SIMERROR, "Simulation aborted due to error during serialization of the state object.");

    SIMERROR = TryCallOutputPlugin(simContext);
    assert_success(SIMERROR, "Simulation aborted due to error in the external output plugin.");
}

error_t FinishKmcExecutionBlock(SCONTEXT_PARAMETER)
{
    AdvanceMainCycleCounterToNextStepGoal(simContext);
    ExecuteSharedMcBlockFinisher(simContext);
    return SIMERROR;
}

// Action for cases where the jump selection has been statistically accepted
static inline void OnMmcEventIsAccepted(SCONTEXT_PARAMETER)
{
    WriteMmcJumpEnergyToAbortBuffer(simContext);
    var activeCounters = getActiveCounters(simContext);
    var cycleCounters = getMainCycleCounters(simContext);

    ++activeCounters->McsCount;
    ++cycleCounters->McsCount;

    AdvanceMmcSystemToFinalState(simContext);
    UpdateTransitionPoolAfterMmcSystemAdvance(simContext);
    simContext->CycleResult = MC_ACCEPTED_CYCLE;
}

// Action for cases where the jump selection has been statistically rejected
static inline void OnMmcEventIsRejected(SCONTEXT_PARAMETER)
{
    var counters = getActiveCounters(simContext);
    ++counters->RejectionCount;
    simContext->CycleResult = MC_REJECTED_CYCLE;
}

// Action for cases where the jump selection has no valid rule and is site-blocking
static inline void OnMmcEventIsSiteBlocked(SCONTEXT_PARAMETER)
{
    var counters = getActiveCounters(simContext);
    ++counters->SiteBlockingCount;
    simContext->CycleResult = MC_BLOCKED_CYCLE;
}

void ExecuteMmcSimulationCycle(SCONTEXT_PARAMETER)
{
    SetNextMmcJumpSelectionOnContext(simContext);
    SetMmcJumpPathPropertiesOnContext(simContext);

    if (TrySetActiveMmcJumpRuleOnContext(simContext))
    {
        SetMmcJumpPropertiesOnContext(simContext);
        SetEnergeticMmcEventEvaluationOnContext(simContext);
        return;
    }

    OnMmcEventIsSiteBlocked(simContext);
}

error_t RunOneMmcExecutionBlock(SCONTEXT_PARAMETER)
{
    var counters = getMainCycleCounters(simContext);
    for (;counters->McsCount < counters->NextExecutionPhaseGoalMcsCount;)
    {
        let countPerLoop = counters->CycleCountPerExecutionLoop;
        for (int64_t i = 0; i < countPerLoop; i++)
        {
            ExecuteMmcSimulationCycle(simContext);
        }
        counters->CycleCount += countPerLoop;
        return_if(UpdateAndEvaluateMmcAbortConditions(simContext) != STATE_FLG_CONTINUE, ERR_OK);
    }
    return SIMERROR;
}

error_t FinishMmcExecutionBlock(SCONTEXT_PARAMETER)
{
    AdvanceMainCycleCounterToNextStepGoal(simContext);
    ExecuteSharedMcBlockFinisher(simContext);
    return SIMERROR;
}

// Evaluates if the ETA of the simulation is likely to timeout during the next block
static inline bool_t UpdateAndEvaluateTimeoutAbortCondition(SCONTEXT_PARAMETER)
{
    let jobInfo = getDbModelJobInfo(simContext);
    var runInfo = getRuntimeInformation(simContext);
    var metaData = getMainStateMetaData(simContext);
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
static inline bool_t UpdateAndEvaluateRateAbortConditions(SCONTEXT_PARAMETER)
{
    let jobInfo = getDbModelJobInfo(simContext);
    let counters = getMainCycleCounters(simContext);
    var metaData = getMainStateMetaData(simContext);

    return_if(metaData->ProgramRunTime == 0, false);

    metaData->SuccessRate = counters->McsCount / metaData->ProgramRunTime;
    metaData->CycleRate = counters->CycleCount / metaData->ProgramRunTime;
    return (metaData->CycleRate < jobInfo->MinimalSuccessRate);
}

// Evaluates if the mcs target is reached
static inline bool_t UpdateAndEvaluateMcsAbortCondition(SCONTEXT_PARAMETER)
{
    let counters = getMainCycleCounters(simContext);
    return (counters->McsCount >= counters->TotalSimulationGoalMcsCount);
}

// Evaluates the general abort conditions and returns the corresponding state flag
static error_t EvaluateGeneralAbortConditions(SCONTEXT_PARAMETER)
{
    if (UpdateAndEvaluateTimeoutAbortCondition(simContext))
    {
        // Note: Timeout evaluation does not yield usable statistics before a second of run time
        return_if(getMainStateMetaData(simContext)->ProgramRunTime < 1.0, STATE_FLG_CONTINUE);

        setMainStateFlags(simContext, STATE_FLG_TIMEOUT | STATE_FLG_CONDABORT);
        return STATE_FLG_TIMEOUT;
    }
    if (UpdateAndEvaluateRateAbortConditions(simContext))
    {
        setMainStateFlags(simContext, STATE_FLG_RATEABORT | STATE_FLG_CONDABORT);
        return STATE_FLG_RATEABORT;
    }
    if (UpdateAndEvaluateMcsAbortCondition(simContext))
    {
        setMainStateFlags(simContext, STATE_FLG_COMPLETED | STATE_FLG_CONDABORT);
        return STATE_FLG_COMPLETED;
    }
    return STATE_FLG_CONTINUE;
}

// Evaluates the pre run abort conditions
static error_t EvaluatePreRunAbortConditions(SCONTEXT_PARAMETER)
{
    return_if(!StateFlagsAreSet(simContext, STATE_FLG_PRERUN), STATE_FLG_CONTINUE);
    let counters = getMainCycleCounters(simContext);
    return (counters->McsCount >= counters->PrerunGoalMcs) ? STATE_FLG_PRERUN_RESET : STATE_FLG_CONTINUE;
}

error_t UpdateAndEvaluateKmcAbortConditions(SCONTEXT_PARAMETER)
{
    error_t result = 0;
    result |= EvaluatePreRunAbortConditions(simContext);
    result |= EvaluateGeneralAbortConditions(simContext);
    return result;
}

// Checks if the fluctuation in the energy abort buffer indicates a relaxed system
static inline bool_t CheckMmcEnergyRelaxationAbortCondition(SCONTEXT_PARAMETER)
{
    let metaData = getMainStateMetaData(simContext);
    let jobHeader = getDbModelJobHeaderAsMMC(simContext);
    let abortBuffer = getLatticeEnergyBuffer(simContext);

    if (jobHeader->AbortTolerance <= 0.0) return false;

    let limitFluctuation = fabs(metaData->LatticeEnergy * jobHeader->AbortTolerance);
    let currentFluctuation = abortBuffer->CurrentSum;

    return (isfinite(currentFluctuation)) ? (limitFluctuation >= fabs(currentFluctuation)) : false;
}

error_t UpdateAndEvaluateMmcAbortConditions(SCONTEXT_PARAMETER)
{
    return_if(EvaluateGeneralAbortConditions(simContext) != STATE_FLG_CONTINUE, STATE_FLG_CONDABORT);

    if (CheckMmcEnergyRelaxationAbortCondition(simContext))
    {
        setMainStateFlags(simContext, STATE_FLG_ENERGYABORT | STATE_FLG_CONDABORT);
        return STATE_FLG_CONDABORT;
    }
    return STATE_FLG_CONTINUE;
}

error_t SyncMainStateLatticeToRunStatus(SCONTEXT_PARAMETER)
{
    let environmentLattice = getEnvironmentLattice(simContext);
    var latticeState = getMainStateLattice(simContext);

    return_if(span_Length(*latticeState) != array_Length(*environmentLattice), ERR_DATACONSISTENCY);

    cpp_foreach(envState, *getEnvironmentLattice(simContext))
        span_Get(*latticeState, getEnvironmentStateIdByPointer(simContext, envState)) = envState->ParticleId;

    return ERR_OK;
}

error_t SyncMainStateCountersToRunStatus(SCONTEXT_PARAMETER)
{
    let counterState = getMainCycleCounters(simContext);
    var headerData = getMainStateHeader(simContext)->Data;

    headerData->Cycles = counterState->CycleCount;
    headerData->Mcs = counterState->McsCount;
    return ERR_OK;
}

error_t SyncMainStateMetaDataToRunStatus(SCONTEXT_PARAMETER)
{
    let rng = getMainRng(simContext);
    var data = getMainStateMetaData(simContext);

    data->RngState = rng->State;
    data->RngIncrease = rng->Inc;
    return ERR_OK;
}

error_t SyncSimulationStateToRunStatus(SCONTEXT_PARAMETER)
{
    ResynchronizeEnvironmentEnergyStatus(simContext);

    var error = SyncMainStateTrackerMappingToSimulation(simContext);
    return_if(error, error);

    error = SyncMainStateLatticeToRunStatus(simContext);
    return_if(error, error);

    error = SyncMainStateCountersToRunStatus(simContext);
    return_if(error, error);

    error = SyncMainStateMetaDataToRunStatus(simContext);
    return error;
}

error_t SaveCurrentSimulationStateToOutDirectory(SCONTEXT_PARAMETER)
{
    return_if(JobInfoFlagsAreSet(simContext, INFO_FLG_SKIPSAVE), ERR_OK);
    let stateBuffer = getMainStateBuffer(simContext);
    let targetFile = StateFlagsAreSet(simContext, STATE_FLG_PRERUN)
            ? getPreRunStateFile(simContext)
            : getMainRunStateFile(simContext);

    return SIMERROR = SaveWriteBufferToFile(targetFile, FMODE_BINARY_W, stateBuffer);
}

static error_t SharedMcSimulationFinish(SCONTEXT_PARAMETER)
{
    setMainStateFlags(simContext, STATE_FLG_COMPLETED);
    SIMERROR = SaveCurrentSimulationStateToOutDirectory(simContext);
    return SIMERROR;
}

error_t FinishMainKmcRoutine(SCONTEXT_PARAMETER)
{
    SIMERROR = SharedMcSimulationFinish(simContext);
    assert_success(SIMERROR, "Simulation aborted due to error in general simulation finisher routine execution.");
    return SIMERROR;
}

error_t FinishMmcMainRoutine(SCONTEXT_PARAMETER)
{
    SIMERROR = SharedMcSimulationFinish(simContext);
    assert_success(SIMERROR, "Simulation aborted due to error in general simulation finisher routine execution.");
    return SIMERROR;
}

// Get the currently active jump id drom the simulation context
static inline int32_t GetActiveJumpId(SCONTEXT_PARAMETER)
{
    let jumpMapping = getJumpDirectionMapping(simContext);
    let jumpSelection = getJumpSelectionInfo(simContext);
    return array_Get(*jumpMapping, JUMPPATH[0]->LatticeVector.D, JUMPPATH[0]->ParticleId, jumpSelection->RelativeJumpId);
}

// Sets the active jump direction and collection on the context (Requires start path entry to be set!)
static inline void SetActiveJumpDirectionAndCollection(SCONTEXT_PARAMETER)
{
    var jumpSelection = getJumpSelectionInfo(simContext);
    var cycleState = getCycleState(simContext);

    jumpSelection->GlobalJumpId = GetActiveJumpId(simContext);
    cycleState->ActiveJumpDirection = getJumpDirectionAt(simContext, jumpSelection->GlobalJumpId);
    cycleState->ActiveJumpCollection = getJumpCollectionAt(simContext, cycleState->ActiveJumpDirection->JumpCollectionId);
}

// Set the path start environment using the current internal state of the context
static inline void SetActivePathStartEnvironment(SCONTEXT_PARAMETER)
{
    let selectionInfo = getJumpSelectionInfo(simContext);
    var cycleState = getCycleState(simContext);
    let envState = getEnvironmentStateAt(simContext, selectionInfo->EnvironmentId);
    SetOccupationCodeByteAt(&cycleState->ActiveStateCode, 0, envState->ParticleId);
    envState->PathId = 0;
    JUMPPATH[0] = envState;
}

// Sets the active jump status for the currently selected KMC on the context
static inline void KMC_SetActiveJumpStatus(SCONTEXT_PARAMETER)
{
    let statusArray = getJumpStatusArray(simContext);
    let direction = getActiveJumpDirection(simContext);
    var cycleState = getCycleState(simContext);

    debug_assert(!array_IsIndexOutOfRange(*statusArray, vecCoorSet3(JUMPPATH[0]->LatticeVector), direction->ObjectId));
    cycleState->ActiveJumpStatus = &array_Get(*statusArray, vecCoorSet3(JUMPPATH[0]->LatticeVector), direction->ObjectId);
}

void SetNextKmcJumpSelectionOnContext(SCONTEXT_PARAMETER)
{
    var cycleState = getCycleState(simContext);
    cycleState->ActiveStateCode.Value = 0ULL;

    UniformSelectNextKmcJumpSelection(simContext);
    SetActivePathStartEnvironment(simContext);
    SetActiveJumpDirectionAndCollection(simContext);
    SetActiveCounterCollection(simContext);
}

// Sets the jump path property of one step by the step index
static inline void SetKmcJumpPathPropertyByIndex(SCONTEXT_PARAMETER, int32_t stepIndex, const Vector4_t*restrict baseVector, const JumpSequence_t* restrict jumpSequence, const Vector4_t*restrict latticeSizes, OccupationCode64_t*restrict stateCode)
{
    let actVector = AddAndTrimVector4(baseVector, &span_Get(*jumpSequence, stepIndex), latticeSizes);
    var envState = getEnvironmentStateByVector4(simContext, &actVector);
    ++stepIndex;
    envState->PathId = stepIndex;
    SetOccupationCodeByteAt(stateCode, stepIndex, envState->ParticleId);
    JUMPPATH[stepIndex] = envState;
}

void SetKmcJumpPathPropertiesOnContext(SCONTEXT_PARAMETER)
{
    // Note: Jump paths could be cached, but profiling showed that dynamically calculating them is faster, especially when
    // cache misses are to be expected in larger simulation systems

    let latticeSizes = getLatticeSizeVector(simContext);
    let jumpSequence = &getActiveJumpDirection(simContext)->JumpSequence;
    let length = span_Length(*jumpSequence);
    let baseVector = &JUMPPATH[0]->LatticeVector;
    var stateCode = &getCycleState(simContext)->ActiveStateCode;

    // Fallthrough switch of the sequence length
    switch (length)
    {
        case 7:
            SetKmcJumpPathPropertyByIndex(simContext, 6, baseVector, jumpSequence, latticeSizes, stateCode);
        case 6:
            SetKmcJumpPathPropertyByIndex(simContext, 5, baseVector, jumpSequence, latticeSizes, stateCode);
        case 5:
            SetKmcJumpPathPropertyByIndex(simContext, 4, baseVector, jumpSequence, latticeSizes, stateCode);
        case 4:
            SetKmcJumpPathPropertyByIndex(simContext, 3, baseVector, jumpSequence, latticeSizes, stateCode);
        case 3:
            SetKmcJumpPathPropertyByIndex(simContext, 2, baseVector, jumpSequence, latticeSizes, stateCode);
        case 2:
            SetKmcJumpPathPropertyByIndex(simContext, 0, baseVector, jumpSequence, latticeSizes, stateCode);
            SetKmcJumpPathPropertyByIndex(simContext, 1, baseVector, jumpSequence, latticeSizes, stateCode);
            break;
        default:
            break;
    }
}

static inline void LinearSearchAndSetActiveJumpRule(SCONTEXT_PARAMETER)
{
    let stateCode = getPathStateCode(simContext);
    var cycleState = getCycleState(simContext);

    cpp_foreach(jumpRule, cycleState->ActiveJumpCollection->JumpRules)
    {
        if (jumpRule->StateCode0.Value == stateCode.Value)
        {
            cycleState->ActiveJumpRule = jumpRule;
            return;
        }
    }
    cycleState->ActiveJumpRule = NULL;
}

static inline void FindAndSetActiveJumpRule(SCONTEXT_PARAMETER)
{
    // ToDo: Potentially change this to use either linear or binary search, depending on the number of rules
    LinearSearchAndSetActiveJumpRule(simContext);
}

JumpRule_t* TrySetActiveKmcJumpRuleOnContext(SCONTEXT_PARAMETER)
{
    FindAndSetActiveJumpRule(simContext);
    return getActiveJumpRule(simContext);
}

void SetKmcJumpPropertiesOnContext(SCONTEXT_PARAMETER)
{
    KMC_SetActiveJumpStatus(simContext);
    SetKmcStateEnergiesOnContext(simContext);
}

void SetKmcJumpProbabilitiesOnContext(SCONTEXT_PARAMETER)
{
    var energyInfo = getJumpEnergyInfo(simContext);
    let preFactor = GetCurrentProbabilityPreFactor(simContext);

    energyInfo->S0toS2DeltaEnergy = energyInfo->S1Energy - energyInfo->S0Energy + energyInfo->ElectricFieldEnergy;
    energyInfo->S2toS0DeltaEnergy = energyInfo->S1Energy - energyInfo->S2Energy - energyInfo->ElectricFieldEnergy;

    energyInfo->RawS0toS2Probability = GetExpResult(simContext, -energyInfo->S0toS2DeltaEnergy);
    energyInfo->NormalizedS0toS2Probability = energyInfo->RawS0toS2Probability * preFactor;
}

// Default internal S1 calculation function that uses the 0.5 * (S_2 - S_0) interpolation method
static inline void SetDefaultKmcTransitionStateEnergy(double* restrict states)
{
    let deltaConf = states[2] - states[0];
    states[4] = deltaConf;
    states[1] += states[0] + 0.5 * deltaConf;
}

// Alternate internal S1 calculation function that uses the factor interpolation method with a shift alpha
static void SetKmcTransitionStateEnergyAlphaMethod(double* restrict states, const double alpha)
{
    let deltaConf = states[2] - states[0];
    let deltaAbs = fabs(deltaConf);
    states[4] = deltaConf;
    states[1] += states[0] + 0.5 * (deltaConf - deltaAbs) + alpha * deltaAbs;
}

void SetEnergeticKmcEventEvaluationOnContext(SCONTEXT_PARAMETER)
{
    let plugins = getPluginCollection(simContext);
    let energyInfo = getJumpEnergyInfo(simContext);

    // Use the internal energy function of no plugin is set
    if (plugins->OnSetTransitionStateEnergy == NULL)
    {
        SetDefaultKmcTransitionStateEnergy(&energyInfo->S0Energy);
    }
    else
    {
        plugins->OnSetTransitionStateEnergy(&energyInfo->S0Energy);
    }

    // Calculates the probabilities from the set state energies
    SetKmcJumpProbabilitiesOnContext(simContext);

    // Unstable end: Do not advance system, update counter and simulated time
    if (energyInfo->S2toS0DeltaEnergy < MC_CONST_JUMPLIMIT_MIN)
    {
        OnKmcEventEndStateIsUnstable(simContext);
        return;
    }
    // Unstable start: Advance system, update counter but not simulated time, do pool update
    if (energyInfo->NormalizedS0toS2Probability > MC_CONST_JUMPLIMIT_MAX)
    {
        OnKmcEventStartStateIsUnstable(simContext);
        return;
    }
    // Successful jump: Advance system, update counters and simulated time, do pool update
    let random = GetNextRandomDoubleFromContextRng(simContext);
    if (energyInfo->NormalizedS0toS2Probability >= random)
    {
        OnKmcEventIsAccepted(simContext);
        return;
    }
    // Rejected jump: Do not advance system, update counter and simulated time, no pool update
    OnKmcEventIsRejected(simContext);
}

void SetNextMmcJumpSelectionOnContext(SCONTEXT_PARAMETER)
{
    UniformSelectNextMmcJumpSelection(simContext);
    SetActivePathStartEnvironment(simContext);
    SetActiveCounterCollection(simContext);
    SetActiveJumpDirectionAndCollection(simContext);
}

void SetMmcJumpPathPropertiesOnContext(SCONTEXT_PARAMETER)
{
    let selectionInfo = getJumpSelectionInfo(simContext);
    let jumpVector = getActiveJumpDirection(simContext)->JumpSequence.Begin;
    let lattice = getEnvironmentLattice(simContext);
    var cycleState = getCycleState(simContext);

    // Translate the offset index into the target environment
    let offsetVector = &getEnvironmentStateAt(simContext, selectionInfo->MmcOffsetSourceId)->LatticeVector;
    var pathState1 = &array_Get(*lattice, offsetVector->A, offsetVector->B, offsetVector->C, jumpVector->D);
    pathState1->PathId = 1;

    // Correct the active state code byte and set the path id of the second environment state
    SetOccupationCodeByteAt(&cycleState->ActiveStateCode, 1, pathState1->ParticleId);
    JUMPPATH[1] = pathState1;
}

JumpRule_t* TrySetActiveMmcJumpRuleOnContext(SCONTEXT_PARAMETER)
{
    FindAndSetActiveJumpRule(simContext);
    return getActiveJumpRule(simContext);
}

void SetMmcJumpPropertiesOnContext(SCONTEXT_PARAMETER)
{
    SetMmcStateEnergiesOnContext(simContext);
}

void SetMmcJumpProbabilitiesOnContext(SCONTEXT_PARAMETER)
{
    var energyInfo = getJumpEnergyInfo(simContext);

    energyInfo->S0toS2DeltaEnergy = energyInfo->S2Energy - energyInfo->S0Energy;
    energyInfo->RawS0toS2Probability = GetExpResult(simContext, -energyInfo->S0toS2DeltaEnergy);
    energyInfo->NormalizedS0toS2Probability = energyInfo->RawS0toS2Probability;
}

void SetEnergeticMmcEventEvaluationOnContext(SCONTEXT_PARAMETER)
{
    let energyInfo = getJumpEnergyInfo(simContext);

    SetMmcJumpProbabilitiesOnContext(simContext);

    // Handle case where the jump is statistically accepted
    let random = GetNextRandomDoubleFromContextRng(simContext);
    if (energyInfo->NormalizedS0toS2Probability >= random)
    {
        OnMmcEventIsAccepted(simContext);
        return;
    }
    // Handle case where the jump is statistically rejected
    OnMmcEventIsRejected(simContext);
}

/* Special MMC routines with alpha factor */

void ExecuteMmcSimulationCycleWithAlpha(SCONTEXT_PARAMETER, double alpha)
{
    SetNextMmcJumpSelectionOnContext(simContext);
    SetMmcJumpPathPropertiesOnContext(simContext);

    if (TrySetActiveMmcJumpRuleOnContext(simContext))
    {
        SetMmcJumpPropertiesOnContext(simContext);
        OnEnergeticMmcJumpEvaluationWithAlpha(simContext, alpha);
        return;
    }

    OnMmcEventIsSiteBlocked(simContext);
}

void SetMmcJumpProbabilitiesOnContextWithAlpha(SCONTEXT_PARAMETER, double alpha)
{
    var energyInfo = getJumpEnergyInfo(simContext);

    energyInfo->S0toS2DeltaEnergy = energyInfo->S2Energy - energyInfo->S0Energy;
    energyInfo->RawS0toS2Probability = GetExpResult(simContext, -energyInfo->S0toS2DeltaEnergy * alpha);
}

void OnEnergeticMmcJumpEvaluationWithAlpha(SCONTEXT_PARAMETER, double alpha)
{
    let energyInfo = getJumpEnergyInfo(simContext);

    SetMmcJumpProbabilitiesOnContextWithAlpha(simContext, alpha);

    // Handle case where the jump is statistically accepted
    let random = GetNextRandomDoubleFromContextRng(simContext);
    if (energyInfo->RawS0toS2Probability >= random)
    {
        OnMmcEventIsAccepted(simContext);
        return;
    }
    // Handle case where the jump is statistically rejected
    OnMmcEventIsRejected(simContext);
}
