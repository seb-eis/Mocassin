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

#include "StateModel.h"
#include "Libraries/Framework/Basic/FileIO/FileIO.h"

// Function type for all state restore callbacks
typedef error_t (*FStateRestoreCallback_t)(SimulationState_t* simulationState);

// The collection type for multiple state restore callbacks
typedef Span_t(FStateRestoreCallback_t, StateRestoreCallbacks) StateRestoreCallbacks_t;

// Get the callback collection to rebuild the state access struct from its set buffer access
StateRestoreCallbacks_t GetStateRestoreCallbackCollection();

//  Restores the simulation state access struct to the passed buffer if possible
error_t RestoreSimulationStateAccessToBuffer(Buffer_t *buffer, SimulationState_t *simulationState);

// Loads a simulation state from the passed filename without creating the simulation context
error_t LoadContextFreeSimulationStateFromFile(char const *fileName, SimulationState_t *simulationState);