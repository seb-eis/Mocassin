//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	TransitionTracking.h        //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   MC transition tracking      //
//////////////////////////////////////////

#pragma once

#include <stdint.h>
#include "Simulator/Data/SimContext/ContextAccess.h"

// Updates the tracking system on the simulation state after a successful KMC transition with the current data
void AdvanceTransitionTrackingSystem(__SCONTEXT_PAR);

// Initializes the jump statistics system on the passed simulation context
error_t InitJumpStatisticsTrackingSystem(__SCONTEXT_PAR);

// Synchronizes the mobile tracker mapping of the main simulation state to the current values in the simulation lattice
error_t SyncMainStateTrackerMappingToSimulation(__SCONTEXT_PAR);