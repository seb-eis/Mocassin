//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	StateModel.h        		//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Simulation state logic      //
//////////////////////////////////////////

#pragma once
#include "Simulator/Data/Model/State/StateModel.h"
#include "Simulator/Data/Model/SimContext/SimContext.h"

#define MC_RUN_STATE_FILE "./run.state"
#define MC_PRE_STATE_FILE "./pre.state"

error_t ConstructSimulationState(sim_context_t* restrict simContext);

error_t PrepareSimulationState(sim_context_t* restrict simContext);

error_t WriteSimulationStateToFile(const sim_context_t* restrict simContext);

error_t LoadSimulationStateFromFile(sim_context_t* restrict simContext);

error_t GetSimulationStateEval(const sim_context_t* restrict simContext);