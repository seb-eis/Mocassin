//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	DebugRoutines.h        		//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Debug routines              //
//////////////////////////////////////////

#pragma once
#include "Framework/Errors/McErrors.h"
#include "Simulator/Data/SimContext/ContextAccess.h"

// Prints the run statistics to a stream with an optional flag to print only the mobile species data
void PrintFullSimulationStatistics(SCONTEXT_PARAM, file_t *fstream, bool_t onlyMobiles);

// Prints the start information of the simulation
void PrintJobStartInfo(SCONTEXT_PARAM, file_t *fstream);

// Prints the pre run context rest notification
void PrintContextResetNotice(SCONTEXT_PARAM, file_t *fstream);

// Prints the simulation finsihe notice
void PrintFinishNotice(SCONTEXT_PARAM, file_t* fstream);
