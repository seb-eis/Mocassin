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

// Adds an occurred energy value to the passed histogram by increasing the correct counter value
static inline void AddEnergyValueToJumpHistogram(JumpHistogram_t*restrict jumpHistogram, const double energy)
{
    // Handle the over and underflow cases
    if (energy > jumpHistogram->MaxValue)
        ++jumpHistogram->OverflowCount;

    if (energy < jumpHistogram->MinValue)
        ++jumpHistogram->UnderflowCount;

    // Handle the calculation of the correct counter id
    let counterId = (int32_t) round(energy / jumpHistogram->Stepping);
    ++jumpHistogram->CountBuffer[counterId];
}

// Updates the jump statistics affiliated with the passed environment state in the current simulation cycle context
static inline void UpdatePathEnvironmentJumpStatistics(SCONTEXT_PARAM, const byte_t pathId)
{
    let toEvFactor = getPhysicalFactors(SCONTEXT)->EnergyFactorKtToEv;
    let energyInfo = getJumpEnergyInfo(SCONTEXT);
    var jumpStatistic = getJumpStatisticAt(SCONTEXT, getActiveJumpCollection(SCONTEXT)->ObjectId, JUMPPATH[pathId]->ParticleId);

    // Handle edge energy
    AddEnergyValueToJumpHistogram(&jumpStatistic->EdgeEnergyHistogram, energyInfo->Energy0 * toEvFactor);

    // Handle conformation influence depending on prefix
    if (energyInfo->ConformationDelta < 0.0)
        AddEnergyValueToJumpHistogram(&jumpStatistic->NegConfEnergyHistogram, fabs(energyInfo->ConformationDelta) * toEvFactor);
    else
        AddEnergyValueToJumpHistogram(&jumpStatistic->PosConfEnergyHistogram, energyInfo->ConformationDelta * toEvFactor);

    // Handle the total energy value
    AddEnergyValueToJumpHistogram(&jumpStatistic->TotalEnergyHistogram, energyInfo->Energy0To2 * toEvFactor);
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

void AdvanceTransitionTrackingSystem(SCONTEXT_PARAM)
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
    jumpHistogram->Stepping = (STATE_JUMPSTAT_EMAX - STATE_JUMPSTAT_EMIN) / STATE_JUMPSTAT_SIZE;

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
        // ToDo: Implement customization of EMIN and EMAX for the histograms
        InitJumpStatisticSystemToDefault(SCONTEXT);
    }

    return ERR_OK;
}