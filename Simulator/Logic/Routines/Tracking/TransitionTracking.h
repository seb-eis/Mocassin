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

// Updates the tracking system on the simulation state after a successful KMC transition
void UpdateJumpTrackingSystem(__SCONTEXT_PAR);