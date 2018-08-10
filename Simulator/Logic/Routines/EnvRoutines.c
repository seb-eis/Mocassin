//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	EnvRoutines.h        		//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Env routines for simulation //
//////////////////////////////////////////

#include "Simulator/Logic/Routines/EnvRoutines.h"

error_t init_env_state_linking(sim_context_t* restrict sim_context)
{
    return MC_SIM_ERROR;
}

error_t init_env_state_energies(sim_context_t* restrict sim_context)
{
    return MC_SIM_ERROR;
}

double get_tmp_s2_energy_cor_mmc(const sim_context_t* restrict sim_context)
{
    return 0.0;
}

double get_tmp_s2_energy_cor_kmc(const sim_context_t* restrict sim_context)
{
    return 0.0;
}

void update_env_states_to_s2_mmc(sim_context_t* restrict sim_context)
{

}

void update_env_states_to_s2_kmc(sim_context_t* restrict sim_context)
{

}