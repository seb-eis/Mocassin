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
#include "Libraries/Simulator/Data/SimContext/SimulationContextAccess.h"

// Prepares the simulation context for the simulation
void InitializeContextForSimulation(SCONTEXT_PARAMETER);

// Resets the required simulation context components after pre run completion in KMC routines
error_t ResetContextAfterKmcPreRun(SCONTEXT_PARAMETER);