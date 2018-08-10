//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	EnvRoutines.h        		//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Main simulation routines    //
//////////////////////////////////////////

#pragma once
#include <math.h>
#include "Simulator/Data/Model/SimContext/SimContext.h"
#include "Simulator/Logic/Routines/EnvRoutines.h"

#define SIMULATION_FLAG_KMC 1
#define SIMULATION_FLAG_MMC 2
#define SIMULATION_FLAG_ERR -1

#define MC_NEG_ENERGY_LIMIT -0.0003

// Determines and returns the type id of the routine encoded in the passed simulation context
error_t get_sim_type_id(sim_context_t* restrict sim_context); 

// Runs the simulation based on the passed simulation context
error_t run_simulation(sim_context_t* restrict sim_context);

// Runs the simulation as a kmc simulation based on the passed simulation context
error_t run_kmc_routine(sim_context_t* restrict sim_context);

// Runs the simulation as a mmc simulation based on the passed simulation context
error_t run_mmc_routine(sim_context_t* restrict sim_context);

// Rolls a new transition state for the next transition attempt in a kmc simulation
error_t roll_kmc_jump_state(sim_context_t* restrict sim_context);

// Rolls a new transition state for the next transition attempt in a mmc simulation
error_t roll_mmc_jump_state(sim_context_t* restrict sim_context);

// Checks if the jump rule of the selected transition state is physically possible (returns 0) and increases the affiliated counters if not.
error_t evaluate_jump_rule(sim_context_t* restrict sim_context);

// Calculates and sets the state energies S_0, S_1 and S_2
error_t set_state_energies_kmc(sim_context_t* restrict sim_context);

// Calculates and sets the state energies S_0, S_2
error_t set_state_energies_mmc(sim_context_t* restrict sim_context);

// Calculates and sets the forward and backward jump probabilities for a kmc transition in the simulation context
error_t set_kmc_transition_probs(sim_context_t* restrict sim_context);

// Calculates and sets the forward transition probability for a mmc transition in the simulation context
error_t set_mmc_transition_probs(sim_context_t* restrict sim_context);

// Evalutes the jump probabilities for the kmc case and calls the required update functions
error_t evaluate_kmc_transition(sim_context_t* restrict sim_context);

// Evalutes the jump probabilities for the mmc case and calls the required update functions
error_t evaluate_mmc_transition(sim_context_t* restrict sim_context);

// Performs all actions required if the next target mcs goal is reached for a mmc simulation
error_t on_next_target_reached_kmc(const sim_context_t* restrict sim_context);

// Performs all actions required if the next target mcs goal is reached for a kmc simulation
error_t on_next_target_reached_mmc(const sim_context_t* restrict sim_context);

// Performs all actions required on finishing the kmc routine
error_t on_kmc_routine_finish(const sim_context_t* restrict sim_context);

// Performs all actions required on finishing the mmc routine
error_t on_mmc_routine_finish(const sim_context_t* restrict sim_context);