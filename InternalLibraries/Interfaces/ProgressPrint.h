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

// Print action that is called on blocks finished
void ProgressPrint_OnBlockFinish(SCONTEXT_PARAM, file_t *fstream, bool_t onlyMobiles);

// Print action that is called on simulation start
void ProgressPrint_OnSimulationStart(SCONTEXT_PARAM, file_t *fstream);

// Print action that is called on context reset (pre-run -> run switch)
void ProgressPrint_OnContextReset(SCONTEXT_PARAM, file_t *fstream);

// Print action that is called on simulation finish
void ProgressPrint_OnSimulationFinish(SCONTEXT_PARAM, file_t *fstream);
