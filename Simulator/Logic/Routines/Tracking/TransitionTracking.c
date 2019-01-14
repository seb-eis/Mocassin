//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	TransitionTracking.c        //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   MC transition tracking      //
//////////////////////////////////////////

#include "Simulator/Logic/Routines/Tracking/TransitionTracking.h"

void UpdateDynamicMovementTracker(__SCONTEXT_PAR, EnvironmentState_t*restrict envState)
{
}

void UpdateJumpTrackingSystem(__SCONTEXT_PAR)
{
    for (int32_t pathId = 0; pathId < getActiveJumpDirection(SCONTEXT)->JumpLength; pathId++)
    {

    }
}