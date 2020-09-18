//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	JumpHistogramPrint.h   		//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Jump histogram printer      //
//////////////////////////////////////////

#pragma once

#include "Libraries/Simulator/Data/State/SimulationStateUtility.h"

// Prints the jump histogram collection of the passed state to the passed file stream
error_t PrintJumpHistogramsToStream(SimulationState_t *simulationState, file_t *fstream);

// Loads the passed simulation state file and prints the contained jump histogram information
void PrintJumpHistogramsFromStateFile(char const *stateFileName, char const *outFileName);

// The utility command to print the jump histogram to stdout using the passed argument information
void UtilityCmd_PrintJumpHistogram(int32_t argc, const char*const* argv);