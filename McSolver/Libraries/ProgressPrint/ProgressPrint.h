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
#include "Libraries/Framework/Errors/McErrors.h"
#include "Libraries/Simulator/Data/SimContext/SimulationContextAccess.h"

// Print action that is called on blocks finished
void PrintMocassinSimulationBlockInfo(SCONTEXT_PARAMETER, file_t *fstream, bool_t onlyMobiles);

// Print action that is called on simulation start
void PrintMocassinSimulationStartInfo(SCONTEXT_PARAMETER, file_t *fstream);

// Print action that is called on context reset (pre-run -> run switch)
void PrintMocassinSimulationContextResetInfo(SCONTEXT_PARAMETER, file_t *fstream);

// Print action that is called on simulation finish
void PrintMocassinSimulationFinishInfo(SCONTEXT_PARAMETER, file_t *fstream);
