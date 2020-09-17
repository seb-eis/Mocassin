//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	CmdArgumentResolver.h		//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Resolver for cmd arguments  //
//////////////////////////////////////////

#pragma once

#include "Libraries/Framework/Errors/McErrors.h"
#include "Libraries/Simulator/Data/SimContext/SimulationContextAccess.h"

// Resolves all passed command line arguments and sets the affiliated context information
void ResolveMocassinCommandLineArguments(SCONTEXT_PARAMETER, int32_t argCount, char const * const * argValues);