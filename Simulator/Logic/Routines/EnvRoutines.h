//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	EnvRoutines.h        		//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Env routines for simulation //
//////////////////////////////////////////

#pragma once
#include "Framework/Errors/McErrors.h"
#include "Simulator/Data/Model/SimContext/SimContext.h"

// Initializes the linking between all env states and creates the updating information
error_t init_env_state_linking(sim_context_t* restrict sim_context);

// Initializes all env states energies from the current status of the lattice
error_t init_env_state_energies(sim_context_t* restrict sim_context);

// Calculates the error correction for the energy of an mmc S2 state. This function does not update the env states
double get_tmp_s2_energy_cor_mmc(const sim_context_t* restrict sim_context);

// Calculates the error correction for the energy of an mmc S2 state. This function does not update the env states
double get_tmp_s2_energy_cor_kmc(const sim_context_t* restrict sim_context);

// Updates the env states of the simulation to the s2 state after an accepted mmc transition
void update_env_states_to_s2_mmc(sim_context_t* restrict sim_context);

// Updates the env states of the simulation to the s2 state after an accepted kmc transition
void update_env_states_to_s2_kmc(sim_context_t* restrict sim_context);