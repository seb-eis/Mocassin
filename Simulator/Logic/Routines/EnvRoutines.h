//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	EnvRoutines.h        		//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Env routines for simulation //
//////////////////////////////////////////

#include "Simulator/Data/Model/SimContext/SimContext.h"

// Updates the state of the passed environment and distributes the change to all environment links of the state
void distribute_env_state_change(env_state_t* restrict env_state, const int32_t new_particle_id);

// Uses the passed env link and particle change information to lookup the affiliated env state in the simulation context and update the state
void update_env_state(sim_context_t* restrict sim_context, const env_link_t* restrict env_link, const int32_t old_par_id, const int32_t new_part_id);