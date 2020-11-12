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
#include "TransitionTrackingRoutines.h"
#include "Libraries/Simulator/Logic/Routines/HelperRoutines.h"

// Updates the mobile tracker index on path environment state by path id using the previously set backup data
static inline void UpdateMobileTrackerMappingByPathId(SCONTEXT_PARAMETER, const int32_t pathId, const JumpRule_t*restrict jumpRule, const EnvironmentBackup_t*restrict envBackup)
{
    let newId = jumpRule->TrackerOrderCode[pathId];

    // On debug assert that no immobile particle has a tracker reorder instruction
    debug_assert(!((newId != pathId) && !JUMPPATH[pathId]->IsMobile));

    JUMPPATH[newId]->MobileTrackerId = envBackup->PathMobileMappings[pathId];
}

// Updates the mobile tracker index on all used path environment states using the previously set backup data
static void UpdateMobileTrackerMapping(SCONTEXT_PARAMETER)
{
    let length = getActiveJumpDirection(simContext)->JumpLength;
    let jumpRule = getActiveJumpRule(simContext);
    let envBackup = getEnvironmentBackup(simContext);

    // Fallthrough switch of possible jump lengths, path id 0 and 2 are always mobile and 1 never is
    switch(length)
    {
        case 8:
            UpdateMobileTrackerMappingByPathId(simContext, 7, jumpRule, envBackup);
        case 7:
            UpdateMobileTrackerMappingByPathId(simContext, 6, jumpRule, envBackup);
        case 6:
            UpdateMobileTrackerMappingByPathId(simContext, 5, jumpRule, envBackup);
        case 5:
            UpdateMobileTrackerMappingByPathId(simContext, 4, jumpRule, envBackup);
        case 4:
            UpdateMobileTrackerMappingByPathId(simContext, 3, jumpRule, envBackup);
        case 3:
            UpdateMobileTrackerMappingByPathId(simContext, 2, jumpRule, envBackup);
            UpdateMobileTrackerMappingByPathId(simContext, 1, jumpRule, envBackup);
            UpdateMobileTrackerMappingByPathId(simContext, 0, jumpRule, envBackup);
        default:
            break;
    }
}

// Updates the path environment movement tracking at the specified index
static inline void UpdatePathEnvironmentMovementTracking(SCONTEXT_PARAMETER, const int32_t pathId)
{
    let moveVector = &span_Get(getActiveJumpDirection(simContext)->MovementSequence, pathId);
    let envState = JUMPPATH[pathId];

    var mobileTracker = getMobileTrackerAt(simContext, envState->MobileTrackerId);
    vector3VectorOp(*mobileTracker, *moveVector, +=);

    var staticTracker = getStaticMovementTrackerAt(simContext, &envState->LatticeVector, envState->ParticleId);
    vector3VectorOp(*staticTracker, *moveVector, +=);

    var globalTracker = getGlobalMovementTrackerAt(simContext, getActiveJumpCollection(simContext)->ObjectId, envState->ParticleId);
    vector3VectorOp(*globalTracker, *moveVector, +=);
}


void AddEnergyValueToJumpHistogram(JumpHistogram_t*restrict jumpHistogram, const double value)
{
    if (value < jumpHistogram->MinValue)
    {
        ++jumpHistogram->UnderflowCount;
        return;
    }

    let counterId = (int32_t) ((value - jumpHistogram->MinValue) * jumpHistogram->SteppingInverse);
    if (counterId >= STATE_JUMPSTAT_SIZE)
    {
        ++jumpHistogram->OverflowCount;
        return;
    }

    ++jumpHistogram->CountBuffer[counterId];
}

void AddEnergyValueToDynamicJumpHistogram(DynamicJumpHistogram_t*restrict jumpHistogram, const double value)
{
    if (value < jumpHistogram->Header->MinValue)
    {
        ++jumpHistogram->Header->UnderflowCount;
        return;
    }

    let counterId = (int32_t) ((value - jumpHistogram->Header->MinValue) * jumpHistogram->Header->SteppingInverse);
    if (counterId >= jumpHistogram->Header->EntryCount)
    {
        ++jumpHistogram->Header->OverflowCount;
        return;
    }

    ++span_Get(jumpHistogram->Counters, counterId);
}

// Updates the jump statistics affiliated with the passed environment state in the current simulation cycle context
static inline void UpdatePathEnvironmentJumpStatistics(SCONTEXT_PARAMETER, const int32_t pathId)
{
    let toEvFactor = getPhysicalFactors(simContext)->EnergyFactorKtToEv;
    let energyInfo = getJumpEnergyInfo(simContext);
    var jumpStatistic = getJumpStatisticAt(simContext, getActiveJumpCollection(simContext)->ObjectId, JUMPPATH[pathId]->ParticleId);

    let energyS1 = energyInfo->S1Energy * toEvFactor;
    AddEnergyValueToJumpHistogram(&jumpStatistic->EdgeEnergyHistogram, energyS1);

    let energyConf = energyInfo->ConformationDeltaEnergy * toEvFactor;
    if (energyConf < 0.0)
        AddEnergyValueToJumpHistogram(&jumpStatistic->NegConfEnergyHistogram, fabs(energyConf));
    else
        AddEnergyValueToJumpHistogram(&jumpStatistic->PosConfEnergyHistogram, energyConf);

    let totEnergy = energyInfo->S0toS2DeltaEnergy * toEvFactor;
    AddEnergyValueToJumpHistogram(&jumpStatistic->TotalEnergyHistogram, totEnergy);
}

// Updates all tracking data (movement and jump statistics) affiliated with the passed path id in the current cycle context
static inline void UpdatePathEnvironmentTrackingData(SCONTEXT_PARAMETER, const int32_t pathId)
{
    let envState = JUMPPATH[pathId];
    getEnvironmentBackup(simContext)->PathMobileMappings[pathId] = envState->MobileTrackerId;
    return_if(!envState->IsMobile);

    UpdatePathEnvironmentMovementTracking(simContext, pathId);
    return_if(simContext->IsJumpLoggingDisabled);
    UpdatePathEnvironmentJumpStatistics(simContext, pathId);
}

void AddCurrentKmcTransitionDataToHistograms(SCONTEXT_PARAMETER)
{
    return_if(simContext->IsJumpLoggingDisabled);
    let length = getActiveJumpDirection(simContext)->JumpLength;

    // Fallthrough switch of possible jump lengths
    switch(length)
    {
        case 8:
            if (JUMPPATH[7]->IsMobile && JUMPPATH[7]->IsStable) UpdatePathEnvironmentJumpStatistics(simContext, 7);
        case 7:
            if (JUMPPATH[6]->IsMobile && JUMPPATH[6]->IsStable) UpdatePathEnvironmentJumpStatistics(simContext, 6);
        case 6:
            if (JUMPPATH[5]->IsMobile && JUMPPATH[5]->IsStable) UpdatePathEnvironmentJumpStatistics(simContext, 5);
        case 5:
            if (JUMPPATH[4]->IsMobile && JUMPPATH[4]->IsStable) UpdatePathEnvironmentJumpStatistics(simContext, 4);
        case 4:
            if (JUMPPATH[3]->IsMobile && JUMPPATH[3]->IsStable) UpdatePathEnvironmentJumpStatistics(simContext, 3);
        case 3:
            UpdatePathEnvironmentJumpStatistics(simContext, 2);
            if (JUMPPATH[1]->IsMobile && JUMPPATH[1]->IsStable) UpdatePathEnvironmentJumpStatistics(simContext, 1);
            UpdatePathEnvironmentJumpStatistics(simContext, 0);
        default:
            break;
    }
}

void AdvanceKmcTransitionTrackingSystem(SCONTEXT_PARAMETER)
{
    let length = getActiveJumpDirection(simContext)->JumpLength;

    // Fallthrough switch of possible jump lengths
    switch(length)
    {
        case 8:
            UpdatePathEnvironmentTrackingData(simContext, 7);
        case 7:
            UpdatePathEnvironmentTrackingData(simContext, 6);
        case 6:
            UpdatePathEnvironmentTrackingData(simContext, 5);
        case 5:
            UpdatePathEnvironmentTrackingData(simContext, 4);
        case 4:
            UpdatePathEnvironmentTrackingData(simContext, 3);
        case 3:
            UpdatePathEnvironmentTrackingData(simContext, 2);
            UpdatePathEnvironmentTrackingData(simContext, 1);
            UpdatePathEnvironmentTrackingData(simContext, 0);
        default:
            break;
    }

    UpdateMobileTrackerMapping(simContext);
}

error_t SyncMainStateTrackerMappingToSimulation(SCONTEXT_PARAMETER)
{
    error_t error = ERR_OK;
    var trackerMapping = getMobileTrackerMapping(simContext);

    cpp_foreach(envState, *getEnvironmentLattice(simContext))
    {
        continue_if(envState->MobileTrackerId <= INVALID_INDEX);
        let envId = getEnvironmentStateIdByPointer(simContext, envState);
        span_Get(*trackerMapping, envState->MobileTrackerId) = envId;
    }

    return error;
}

// Init the passed jump histogram to the default state
static inline void InitJumpHistogramToDefault(SCONTEXT_PARAMETER, JumpHistogram_t*restrict jumpHistogram)
{
    jumpHistogram->MinValue = STATE_JUMPSTAT_EMIN;
    jumpHistogram->MaxValue = getUpperJumpHistogramLimit(simContext);
    jumpHistogram->UnderflowCount = 0;
    jumpHistogram->OverflowCount = 0;
    jumpHistogram->SteppingInverse = STATE_JUMPSTAT_SIZE / (jumpHistogram->MaxValue - STATE_JUMPSTAT_EMIN);

    memset(&jumpHistogram->CountBuffer, 0, sizeof(jumpHistogram->CountBuffer));
}

// Initializes the full jump statistic system to the default state
static inline void InitJumpStatisticSystemToDefault(SCONTEXT_PARAMETER)
{
    cpp_foreach(jumpStatistic, *getJumpStatistics(simContext))
    {
        InitJumpHistogramToDefault(simContext, &jumpStatistic->TotalEnergyHistogram);
        InitJumpHistogramToDefault(simContext, &jumpStatistic->PosConfEnergyHistogram);
        InitJumpHistogramToDefault(simContext, &jumpStatistic->NegConfEnergyHistogram);
        InitJumpHistogramToDefault(simContext, &jumpStatistic->EdgeEnergyHistogram);
    }
}

error_t InitJumpStatisticsTrackingSystem(SCONTEXT_PARAMETER)
{
    if (!StateFlagsAreSet(simContext, STATE_FLG_INITIALIZED))
    {
        InitJumpStatisticSystemToDefault(simContext);
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
    return ChangeDynamicJumpHistogramSamplingAreaByMinMax(jumpHistogram, centerValue - valueRange, centerValue + valueRange);
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