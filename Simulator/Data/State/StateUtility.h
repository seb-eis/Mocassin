//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	StateUtility.h        		//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Simulation state utilities  //
//////////////////////////////////////////

#pragma once

#include "Simulator/Data/State/StateModel.h"
#include "Framework/Basic/FileIO/FileIO.h"

typedef error_t (*FStateRestoreCallback_t)(SimulationState_t* simulationState);
typedef Span_t(FStateRestoreCallback_t, StateRestoreCallbacks) StateRestoreCallbacks_t;

// Get the callback collection to rebuild  state access struct from its set buffer access
StateRestoreCallbacks_t GetStateRestoreCallbackCollection();

//  Creates the simulation state access struct to the passed buffer
error_t CreateSimulationStateAccessToBuffer(Buffer_t* buffer, SimulationState_t* simulationState);

// Loads a simulation state from the passed filename without creating the simulation context
error_t LoadContextFreeSimulationState(char const *fileName, SimulationState_t *simulationState);

// Prints the jump histogram collection of the passed state to the passed file stream
error_t PrintFormattedJumpHistograms(SimulationState_t* simulationState, file_t* fstream);

// Loads the passed simulation state file and prints the contained jump histogram information
void ExtractAndPrintJumpHistograms(char const *stateFileName, char const* outFileName);