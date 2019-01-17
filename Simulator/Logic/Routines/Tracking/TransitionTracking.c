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
static void UpdateMobileTrackerMapping(__SCONTEXT_PAR)
{
    for (int32_t pathId = 0; pathId < getActiveJumpDirection(SCONTEXT)->JumpLength; ++pathId)
    {
        byte_t pullId = getActiveJumpRule(SCONTEXT)->TrackerOrderCode[pathId];

        // On debug assert that no immobile particle has a tracker reorder instruction
        debug_assert(!((pullId != pathId) && !JUMPPATH[pathId]->IsMobile));
        JUMPPATH[pathId]->MobileTrackerId = getEnvironmentBackup(SCONTEXT)->PathMobileMappings[pullId];
    }
}

// Updates the path environment movement tracking at the specified index
static inline void UpdatePathEnvironmentMovementTracking(__SCONTEXT_PAR, const byte_t pathId)
{
    Vector3_t* moveVector = &span_Get(getActiveJumpDirection(SCONTEXT)->MovementSequence, pathId);

    // Update the movement data on the mobile tracker
    Tracker_t* mobileTracker = getMobileTrackerAt(SCONTEXT, JUMPPATH[pathId]->MobileTrackerId);
    vector3VectorOp(*mobileTracker, *moveVector, +=);

    // Update the movement data of the static tracker
    Tracker_t* staticTracker = getStaticMovementTrackerAt(SCONTEXT, JUMPPATH[pathId]->PositionVector.D, JUMPPATH[pathId]->ParticleId);
    vector3VectorOp(*staticTracker, *moveVector, +=);

    // Update the movement data on the global tracker
    Tracker_t* globalTracker = getGlobalMovementTrackerAt(SCONTEXT, getActiveJumpCollection(SCONTEXT)->ObjectId, JUMPPATH[pathId]->ParticleId);
    vector3VectorOp(*globalTracker, *moveVector, +=);

}

// Adds an occurred energy value to the passed histogram by increasing the correct counter value
static inline void AddEnergyValueToJumpHistogram(JumpHistogram_t*restrict jumpHistogram, const double energy)
{
    // ToDo: Buffer the stepping value to perform this calculation only once
    double stepping = (jumpHistogram->MaxValue - jumpHistogram->MinValue) / STATE_JUMPSTAT_SIZE;

    // Handle the over and underflow cases
    if (energy > jumpHistogram->MaxValue)
    {
        ++jumpHistogram->OverflowCount;
    }
    if (energy < jumpHistogram->MinValue)
    {
        ++jumpHistogram->UnderflowCount;
    }

    // Handle the calculation of the correct counter id
    int32_t counterId = (int32_t) round(energy / stepping);
    ++jumpHistogram->CountBuffer[counterId];
}

// Updates the jump statistics affiliated with the passed environment state in the current simulation cycle context
static inline void UpdatePathEnvironmentJumpStatistics(__SCONTEXT_PAR, const byte_t pathId)
{
    JumpEnergyInfo_t* energyInfo = getJumpEnergyInfo(SCONTEXT);
    JumpStatistic_t* jumpStatistic = getJumpStatisticAt(SCONTEXT, JUMPPATH[pathId]->PositionVector.D, JUMPPATH[pathId]->ParticleId);

    // Handle edge energy
    AddEnergyValueToJumpHistogram(&jumpStatistic->EdgeEnergyHistogram, energyInfo->Energy0);

    // Handle conformation influence depending on prefix
    if (energyInfo->ConformationDelta < 0.0)
    {
        AddEnergyValueToJumpHistogram(&jumpStatistic->NegConfEnergyHistogram, fabs(energyInfo->ConformationDelta));
    }
    else
    {
        AddEnergyValueToJumpHistogram(&jumpStatistic->PosConfEnergyHistogram, energyInfo->ConformationDelta);
    }

    // Handle the total energy value
    AddEnergyValueToJumpHistogram(&jumpStatistic->TotalEnergyHistogram, energyInfo->Energy0To2);
}

// Updates all tracking data (movement and jump statistics) affiliated with the passed path id in the current cycle context
static inline void UpdatePathEnvironmentTrackingData(__SCONTEXT_PAR, const byte_t pathId)
{
    getEnvironmentBackup(SCONTEXT)->PathMobileMappings[pathId] = JUMPPATH[pathId]->MobileTrackerId;
    voidreturn_if(!JUMPPATH[pathId]->IsMobile);

    // ToDo: Optimize this to lookup the required globalTrackerId only once (Needed for movement and jump statistics)
    UpdatePathEnvironmentMovementTracking(SCONTEXT, pathId);
    UpdatePathEnvironmentJumpStatistics(SCONTEXT, pathId);
}

void AdvanceTransitionTrackingSystem(__SCONTEXT_PAR)
{
    for (byte_t pathId = 0; pathId < getActiveJumpDirection(SCONTEXT)->JumpLength; ++pathId)
    {
        UpdatePathEnvironmentTrackingData(SCONTEXT, pathId);
    }

    UpdateMobileTrackerMapping(SCONTEXT);
}

error_t SyncMainStateTrackerMappingToSimulation(__SCONTEXT_PAR)
{
    error_t error = ERR_OK;
    MobileTrackerMapping_t* trackerMapping = getMobileTrackerMapping(SCONTEXT);

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

    c_foreach(item, jumpHistogram->CountBuffer)
    {
        *item = 0;
    }
}

// Initializes the full jump statistic system to the default state
static inline void InitJumpStatisticSystemToDefault(__SCONTEXT_PAR)
{
    cpp_foreach(jumpStatistic, *getJumpStatistics(SCONTEXT))
    {
        InitJumpHistogramToDefault(&jumpStatistic->TotalEnergyHistogram);
        InitJumpHistogramToDefault(&jumpStatistic->PosConfEnergyHistogram);
        InitJumpHistogramToDefault(&jumpStatistic->NegConfEnergyHistogram);
        InitJumpHistogramToDefault(&jumpStatistic->EdgeEnergyHistogram);
    }
}

error_t InitJumpStatisticsTrackingSystem(__SCONTEXT_PAR)
{
    if (!StateFlagsAreSet(SCONTEXT, STATE_FLG_INITIALIZED))
    {
        // ToDo: Implement customization of EMIN and EMAX for the histograms
        InitJumpStatisticSystemToDefault(SCONTEXT);
    }

    return ERR_OK;
}