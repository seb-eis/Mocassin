//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	ContextInitializer.h   		//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Context initializer logic   //
//////////////////////////////////////////

#pragma once
#include "Simulator/Data/SimContext/ContextAccess.h"

// Prepares the simulation context for the simulation
void PrepareContextForSimulation(SCONTEXT_PARAM);

// Resets the required simulation context components after pre run completion in KMC routines
error_t KMC_ResetContextAfterPreRun(SCONTEXT_PARAM);