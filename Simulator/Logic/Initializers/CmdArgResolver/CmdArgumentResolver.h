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

#include "Framework/Errors/McErrors.h"
#include "Simulator/Data/SimContext/ContextAccess.h"

// Resolves all passed command line arguments and sets the affiliated context information
void ResolveCommandLineArguments(SCONTEXT_PARAMETER, int32_t argCount, char const * const * argValues);