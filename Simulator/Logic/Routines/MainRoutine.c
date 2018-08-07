//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	EnvRoutines.c        		//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Main simulation routines    //
//////////////////////////////////////////

#include "Simulator/Logic/Routines/MainRoutines.h"
#include "Simulator/Logic/Objects/JumpSelection.h"

error_t get_sim_type_id(sim_context_t* restrict sim_context)
{
    return SIMULATION_FLAG_KMC;
}

error_t run_simulation(sim_context_t* restrict sim_context)
{   
    error_t sim_type_flag = get_sim_type_id(sim_context);
    
    if (sim_type_flag == SIMULATION_FLAG_MMC)
    {
        return run_mmc_routine(sim_context);
    }

    if (sim_type_flag == SIMULATION_FLAG_KMC)
    {
        return run_kmc_routine(sim_context);
    }
    
    if (sim_type_flag == SIMULATION_FLAG_ERR)
    {
        MC_DUMP_ERROR_AND_EXIT(SIMULATION_FLAG_ERR, "Simulation type flag determination returned an error");
    }
    return MC_SIM_ERROR;
}

error_t run_kmc_routine(sim_context_t* restrict sim_context)
{
    while (sim_context->cur_mcs < sim_context->total_target_mcs)
    {
        for (int64_t cycle_count = 0; cycle_count < sim_context->cycle_size; cycle_count++)
        {
            roll_kmc_jump_state(sim_context);  
            if (evaluate_jump_rule(sim_context) != MC_NO_ERROR)
            {
                continue;
            }
            set_state_energies_kmc(sim_context);
            set_kmc_transition_probs(sim_context);
            evaluate_kmc_transition(sim_context);
        }

        if (sim_context->cur_mcs >= sim_context->cur_target_mcs)
        {
            on_next_target_reached_kmc(sim_context);
        }
    }
    return on_kmc_routine_finish(sim_context);
}

error_t run_mmc_routine(sim_context_t* restrict sim_context)
{
    while (sim_context->cur_mcs < sim_context->total_target_mcs)
    {
        for (int64_t cycle_count = 0; cycle_count < sim_context->cycle_size; cycle_count++)
        {
            roll_mmc_jump_state(sim_context);
            if (evaluate_jump_rule(sim_context) != MC_NO_ERROR)
            {
                continue;
            }
            set_state_energies_mmc(sim_context);
            set_mmc_transition_probs(sim_context);
            evaluate_mmc_transition(sim_context);
        }

        if (sim_context->cur_mcs >= sim_context->cur_target_mcs)
        {
            on_next_target_reached_mmc(sim_context);
        }
    }
    return on_mmc_routine_finish(sim_context);  
}

static void set_jump_path_kmc(sim_context_t* restrict sim_context)
{
    // Translates the 4D relative jump seqeunce into absolute vectors and retrieve the env state pointers for the path
    int_vector_t vector = sim_context->transition_state.path_env_states[0]->pos_vector;
    for (int32_t i = 0; i < sim_context->transition_state.cur_jump_dir->jump_length - 1;)
    {
        vector = add_int_vectors(&vector, ARRAY_GET_PTR(sim_context->transition_state.cur_jump_dir->jump_sequence, i));
        sim_context->transition_state.path_env_states[++i] = MDA_GET_4(sim_context->env_states, vector.a, vector.b, vector.c, vector.d);
    }
}

static inline void set_jump_path_mmc(sim_context_t* restrict sim_context)
{
    // Build the target vector by using the 'd' coordinate of the jump direction sequence 0 entry and the 'a,b,c' coordinates of the rolled offset position id
    int_vector_t* off_vec = &ARRAY_GET(sim_context->env_states, sim_context->transition_state.roll_info.id_2).pos_vector;
    int_vector_t* base_vec = ARRAY_GET_PTR(sim_context->transition_state.cur_jump_dir->jump_sequence, 0);
    sim_context->transition_state.path_env_states[1] = MDA_GET_4(sim_context->env_states, off_vec->a, off_vec->b, off_vec->c, base_vec->d);
}

static inline jump_dir_t* get_jump_dir_ptr(const transition_model_t* restrict trans_model, const env_state_t* restrict env_state, const roll_info_t* restrict roll_info)
{
    return ARRAY_GET_PTR(ARRAY_GET(trans_model->jump_col_list, env_state->pos_vector.d).jump_dir_list, roll_info->id_1);
}

error_t roll_kmc_jump_state(sim_context_t* restrict sim_context)
{
    transition_state_t* trans_state = &sim_context->transition_state;

    // Get the kmc roll info and translate the set roll info into an actual transiton state
    roll_kmc_transition_info(&sim_context->jump_pool, &trans_state->roll_info);

    // Use first id to get the env state ptr and the second to get the jump direction ptr
    trans_state->path_env_states[0] = ARRAY_GET_PTR(sim_context->env_states, trans_state->roll_info.id_0);
    trans_state->cur_jump_dir = get_jump_dir_ptr(&sim_context->db_model.transition_model, trans_state->path_env_states[0], &trans_state->roll_info);

    // Set the actual path state
    set_jump_path_kmc(sim_context);

    return MC_NO_ERROR;
}

error_t roll_mmc_jump_state(sim_context_t* restrict sim_context)
{
    transition_state_t* trans_state = &sim_context->transition_state;

    // Get the mmc roll info and translate the rolled ids into the actual transition state
    roll_mmc_transition_info(&sim_context->jump_pool, sim_context->meta_info.num_of_mobiles, &trans_state->roll_info);

    // Use first id to get start env state and 2 id to generate the offset vector
    trans_state->path_env_states[0] = ARRAY_GET_PTR(sim_context->env_states, trans_state->roll_info.id_0);
    trans_state->cur_jump_dir = get_jump_dir_ptr(&sim_context->db_model.transition_model, trans_state->path_env_states[0], &trans_state->roll_info);

    // Use last id to generate the actual target and set the jump path
    set_jump_path_mmc(sim_context);

    return MC_NO_ERROR;
}

error_t evaluate_jump_rule(sim_context_t* restrict sim_context)
{
    return MC_SIM_ERROR;
}

error_t set_state_energies_kmc(sim_context_t* restrict sim_context)
{
    return MC_SIM_ERROR;
}

error_t set_state_energies_mmc(sim_context_t* restrict sim_context)
{
    return MC_SIM_ERROR;
}

error_t set_kmc_transition_probs(sim_context_t* restrict sim_context)
{
    return MC_SIM_ERROR;
}

error_t set_mmc_transition_probs(sim_context_t* restrict sim_context)
{
    return MC_SIM_ERROR;
}

error_t evaluate_kmc_transition(sim_context_t* restrict sim_context)
{
    return MC_SIM_ERROR;
}

error_t evaluate_mmc_transition(sim_context_t* restrict sim_context)
{
    return MC_SIM_ERROR;
}

error_t on_next_target_reached_mmc(const sim_context_t* restrict sim_context)
{
    return MC_SIM_ERROR;
}

error_t on_next_target_reached_kmc(const sim_context_t* restrict sim_context)
{
    return MC_SIM_ERROR;
}

error_t on_kmc_routine_finish(const sim_context_t* restrict sim_context)
{
    return MC_SIM_ERROR;
}

error_t on_mmc_routine_finish(const sim_context_t* restrict sim_context)
{
    return MC_SIM_ERROR;
}