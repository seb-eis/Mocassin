//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	TransitionTracking.c        //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   MC transition tracking      //
//////////////////////////////////////////

#include <math.h>
#include "Simulator/Logic/Routines/Tracking/TransitionTracking.h"
#include "Simulator/Logic/Routines/Helper/HelperRoutines.h"
#include "Simulator/Data/State/StateModel.h"

// Updates the mobile tracker index on all used path environment states using the previously set backup data
static void UpdateMobileTrackerMapping(SCONTEXT_PARAM)
{
    for (int32_t pathId = 0; pathId < getActiveJumpDirection(SCONTEXT)->JumpLength; ++pathId)
    {
        let pullId = getActiveJumpRule(SCONTEXT)->TrackerOrderCode[pathId];

        // On debug assert that no immobile particle has a tracker reorder instruction
        debug_assert(!((pullId != pathId) && !JUMPPATH[pathId]->IsMobile));
        JUMPPATH[pathId]->MobileTrackerId = getEnvironmentBackup(SCONTEXT)->PathMobileMappings[pullId];
    }
}

// Updates the path environment movement tracking at the specified index
static inline void UpdatePathEnvironmentMovementTracking(SCONTEXT_PARAM, const byte_t pathId)
{
    let moveVector = &span_Get(getActiveJumpDirection(SCONTEXT)->MovementSequence, pathId);

    // Update the movement data on the mobile tracker
    var mobileTracker = getMobileTrackerAt(SCONTEXT, JUMPPATH[pathId]->MobileTrackerId);
    vector3VectorOp(*mobileTracker, *moveVector, +=);

    // Update the movement data of the static tracker
    var staticTracker = getStaticMovementTrackerAt(SCONTEXT, &JUMPPATH[pathId]->PositionVector, JUMPPATH[pathId]->ParticleId);
    vector3VectorOp(*staticTracker, *moveVector, +=);

    // Update the movement data on the global tracker
    var globalTracker = getGlobalMovementTrackerAt(SCONTEXT, getActiveJumpCollection(SCONTEXT)->ObjectId, JUMPPATH[pathId]->ParticleId);
    vector3VectorOp(*globalTracker, *moveVector, +=);
}


void AddEnergyValueToJumpHistogram(JumpHistogram_t*restrict jumpHistogram, const double value)
{
    // Handle the correct counter id generation and overflow cases
    if (value < jumpHistogram->MinValue)
    {
        ++jumpHistogram->UnderflowCount;
        return;
    }

    let counterId = (int32_t) round((value - jumpHistogram->MinValue) * jumpHistogram->SteppingInverse);
    if (counterId >= STATE_JUMPSTAT_SIZE)
    {
        ++jumpHistogram->OverflowCount;
        return;
    }

    // Handle the correct insert
    ++jumpHistogram->CountBuffer[counterId];
}

void AddEnergyValueToDynamicJumpHistogram(DynamicJumpHistogram_t*restrict jumpHistogram, const double value)
{
    // Handle the correct counter id generation and overflow cases
    if (value < jumpHistogram->Header->MinValue)
    {
        ++jumpHistogram->Header->UnderflowCount;
        return;
    }

    let counterId = (int32_t) round((value - jumpHistogram->Header->MinValue) * jumpHistogram->Header->SteppingInverse);
    if (counterId >= jumpHistogram->Header->EntryCount)
    {
        ++jumpHistogram->Header->OverflowCount;
        return;
    }

    // Handle the correct insert
    ++span_Get(jumpHistogram->Counters, counterId);
}

// Updates the jump statistics affiliated with the passed environment state in the current simulation cycle context
static inline void UpdatePathEnvironmentJumpStatistics(SCONTEXT_PARAM, const byte_t pathId)
{
    let toEvFactor = getPhysicalFactors(SCONTEXT)->EnergyFactorKtToEv;
    let energyInfo = getJumpEnergyInfo(SCONTEXT);
    var jumpStatistic = getJumpStatisticAt(SCONTEXT, getActiveJumpCollection(SCONTEXT)->ObjectId, JUMPPATH[pathId]->ParticleId);

    // Handle edge energy
    let energyS1 = energyInfo->S1Energy * toEvFactor;
    AddEnergyValueToJumpHistogram(&jumpStatistic->EdgeEnergyHistogram, energyS1);

    // Handle conformation influence depending on prefix
    let energyConf = energyInfo->ConformationDeltaEnergy * toEvFactor;
    if (energyConf < 0.0)
        AddEnergyValueToJumpHistogram(&jumpStatistic->NegConfEnergyHistogram, fabs(energyConf));
    else
        AddEnergyValueToJumpHistogram(&jumpStatistic->PosConfEnergyHistogram, energyConf);

    // Handle the total energy value
    let totEnergy = energyInfo->S0toS2DeltaEnergy * toEvFactor;
    AddEnergyValueToJumpHistogram(&jumpStatistic->TotalEnergyHistogram, totEnergy);
}

// Updates all tracking data (movement and jump statistics) affiliated with the passed path id in the current cycle context
static inline void UpdatePathEnvironmentTrackingData(SCONTEXT_PARAM, const byte_t pathId)
{
    getEnvironmentBackup(SCONTEXT)->PathMobileMappings[pathId] = JUMPPATH[pathId]->MobileTrackerId;
    return_if(!JUMPPATH[pathId]->IsMobile);

    // ToDo: Optimize this to lookup the required globalTrackerId only once (Needed for movement and jump statistics)
    UpdatePathEnvironmentMovementTracking(SCONTEXT, pathId);
    UpdatePathEnvironmentJumpStatistics(SCONTEXT, pathId);
}

void KMC_AddCurrentJumpDataToHistograms(SCONTEXT_PARAM)
{
    for (byte_t pathId = 0; pathId < getActiveJumpDirection(SCONTEXT)->JumpLength; ++pathId)
    {
        continue_if(!JUMPPATH[pathId]->IsMobile);
        UpdatePathEnvironmentJumpStatistics(SCONTEXT, pathId);
    }
}

void KMC_AdvanceTransitionTrackingSystem(SCONTEXT_PARAM)
{
    for (byte_t pathId = 0; pathId < getActiveJumpDirection(SCONTEXT)->JumpLength; ++pathId)
        UpdatePathEnvironmentTrackingData(SCONTEXT, pathId);

    UpdateMobileTrackerMapping(SCONTEXT);
}

error_t SyncMainStateTrackerMappingToSimulation(SCONTEXT_PARAM)
{
    error_t error = ERR_OK;
    var trackerMapping = getMobileTrackerMapping(SCONTEXT);

    // Update mobile tracker mapping in the state to the current simulation lattice state
    cpp_foreach(envState, *getEnvironmentLattice(SCONTEXT))
    {
        continue_if(envState->MobileTrackerId <= INVALID_INDEX);
        span_Get(*trackerMapping, envState->MobileTrackerId) = envState->EnvironmentId;
    }

    return error;
}

// Init the passed jump histogram to the default state
static inline void InitJumpHistogramToDefault(JumpHistogram_t*restrict jumpHistogram)
{
    jumpHistogram->MinValue = STATE_JUMPSTAT_EMIN;
    jumpHistogram->MaxValue = STATE_JUMPSTAT_EMAX;
    jumpHistogram->UnderflowCount = 0;
    jumpHistogram->OverflowCount = 0;
    jumpHistogram->SteppingInverse = STATE_JUMPSTAT_SIZE / (STATE_JUMPSTAT_EMAX - STATE_JUMPSTAT_EMIN);

    memset(&jumpHistogram->CountBuffer, 0, sizeof(jumpHistogram->CountBuffer));
}

// Initializes the full jump statistic system to the default state
static inline void InitJumpStatisticSystemToDefault(SCONTEXT_PARAM)
{
    cpp_foreach(jumpStatistic, *getJumpStatistics(SCONTEXT))
    {
        InitJumpHistogramToDefault(&jumpStatistic->TotalEnergyHistogram);
        InitJumpHistogramToDefault(&jumpStatistic->PosConfEnergyHistogram);
        InitJumpHistogramToDefault(&jumpStatistic->NegConfEnergyHistogram);
        InitJumpHistogramToDefault(&jumpStatistic->EdgeEnergyHistogram);
    }
}

error_t InitJumpStatisticsTrackingSystem(SCONTEXT_PARAM)
{
    if (!StateFlagsAreSet(SCONTEXT, STATE_FLG_INITIALIZED))
    {
        InitJumpStatisticSystemToDefault(SCONTEXT);
    }

    return ERR_OK;
}

error_t ChangeDynamicJumpHistogramSamplingAreaByMinMax(DynamicJumpHistogram_t*restrict jumpHistogram, double minValue, double maxValue)
{
    return_if(jumpHistogram->Header == NULL || jumpHistogram->Header->EntryCount <= 0, ERR_ARGUMENT);
    return_if(minValue >= maxValue, ERR_ARGUMENT);

    ResetDynamicJumpHistogramToEmptyState(jumpHistogram);
    let stepping = (maxValue-minValue) / (double) jumpHistogram->Header->EntryCount;
    let steppingInv = 1.0 / stepping;

    return_if(!isfinite(stepping) || !isfinite(steppingInv), ERR_ARGUMENT);

    jumpHistogram->Header->MinValue = minValue;
    jumpHistogram->Header->MaxValue = maxValue;
    jumpHistogram->Header->Stepping = stepping;
    jumpHistogram->Header->SteppingInverse = steppingInv;

    return ERR_OK;
}

error_t ChangeDynamicJumpHistogramSamplingAreaByRange(DynamicJumpHistogram_t*restrict jumpHistogram, double centerValue, double valueRange)
{
    return_if(jumpHistogram->Header == NULL || jumpHistogram->Header->EntryCount <= 0, ERR_ARGUMENT);
    return_if(!isfinite(centerValue) || !isfinite(valueRange), ERR_ARGUMENT);
    ChangeDynamicJumpHistogramSamplingAreaByMinMax(jumpHistogram, centerValue - valueRange, centerValue + valueRange);
}

double CalculateDynamicJumpHistogramMeanEnergy(const DynamicJumpHistogram_t*restrict jumpHistogram)
{
    double samplingSum = 0.0;
    int64_t totalCount = 0;

    for (int64_t i = 0; i < jumpHistogram->Header->EntryCount; i++)
    {
        let count = span_Get(jumpHistogram->Counters, i);
        let sampleEnergy = jumpHistogram->Header->MinValue + (double) i * jumpHistogram->Header->Stepping;
        samplingSum += count * sampleEnergy;
        totalCount += count;
    }

    return samplingSum / (double) totalCount;
}

double FindDynamicJumpHistogramMaxValue(const DynamicJumpHistogram_t*restrict jumpHistogram)
{
    int64_t maxEntry = 0, id = 0;
    for (int64_t i = 0; i < jumpHistogram->Header->EntryCount; i++)
    {
        let counter = span_Get(jumpHistogram->Counters, i);
        if (counter < maxEntry) continue;
        maxEntry = counter;
        id = i;
    }

    return jumpHistogram->Header->MinValue + (double) id * jumpHistogram->Header->Stepping;
}