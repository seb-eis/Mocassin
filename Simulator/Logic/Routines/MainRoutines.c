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

static inline void prepare_transition_path_kmc(sim_context_t* restrict sim_context)
{
    int_vector_t vector = sim_context->transition_state.path_env_states[0]->pos_vector;
    for (int32_t i = 0; i < sim_context->transition_state.cur_jump_dir->jump_length - 1;)
    {
        vector = add_int_vectors(&vector, ARRAY_GET_PTR(sim_context->transition_state.cur_jump_dir->jump_sequence, i));
        sim_context->transition_state.path_env_states[++i] = MDA_GET_4(sim_context->env_states, vector.a, vector.b, vector.c, vector.d);
        REINTERPRET_CAST(byte_t, &sim_context->transition_state.state_code)[i] = sim_context->transition_state.path_env_states[i-1]->particle_id;
    }
}

static inline void prepare_transition_path_mmc(sim_context_t* restrict sim_context)
{
    int_vector_t* off_vec = &ARRAY_GET(sim_context->env_states, sim_context->transition_state.roll_info.id_2).pos_vector;
    int_vector_t* base_vec = ARRAY_GET_PTR(sim_context->transition_state.cur_jump_dir->jump_sequence, 0);
    sim_context->transition_state.path_env_states[1] = MDA_GET_4(sim_context->env_states, off_vec->a, off_vec->b, off_vec->c, base_vec->d);
    REINTERPRET_CAST(byte_t, &sim_context->transition_state.state_code)[1] = sim_context->transition_state.path_env_states[1]->particle_id;
}

static inline void reset_transition_state_code(sim_context_t* restrict sim_context)
{
    sim_context->transition_state.state_code = 0;
    REINTERPRET_CAST(byte_t, &sim_context->transition_state.state_code)[0] = sim_context->transition_state.path_env_states[0]->particle_id; 
}

static inline void prepare_state_for_path_creation(sim_context_t* restrict sim_context)
{
    sim_context->transition_state.path_env_states[0] = ARRAY_GET_PTR(sim_context->env_states, sim_context->transition_state.roll_info.id_0);
    sim_context->transition_state.cur_jump_dir = ARRAY_GET_PTR(
        sim_context->db_model.transition_model.jump_dir_list,
        *MDA_GET_3(
            sim_context->db_model.transition_model.jump_dir_assign_table,
            sim_context->transition_state.path_env_states[0]->pos_vector.d,
            sim_context->transition_state.path_env_states[0]->particle_id,
            sim_context->transition_state.roll_info.id_1)
    );
    sim_context->transition_state.cur_jump_col = ARRAY_GET_PTR(sim_context->db_model.transition_model.jump_col_list, sim_context->transition_state.cur_jump_dir->col_id);   
}

error_t roll_kmc_jump_state(sim_context_t* restrict sim_context)
{
    roll_kmc_transition_info(&sim_context->jump_pool, &sim_context->transition_state.roll_info);
    prepare_state_for_path_creation(sim_context);
    reset_transition_state_code(sim_context);
    prepare_transition_path_kmc(sim_context);

    return MC_NO_ERROR;
}

error_t roll_mmc_jump_state(sim_context_t* restrict sim_context)
{
    roll_mmc_transition_info(&sim_context->jump_pool, sim_context->meta_info.num_of_mobiles, &sim_context->transition_state.roll_info);
    prepare_state_for_path_creation(sim_context);
    reset_transition_state_code(sim_context);
    prepare_transition_path_mmc(sim_context);

    return MC_NO_ERROR;
}

static inline void set_active_counter_col(sim_context_t* restrict sim_context)
{
    sim_context->transition_state.cur_counter_col = ARRAY_GET_PTR(sim_context->mc_state.counter_state, sim_context->transition_state.path_env_states[0]->particle_id);
}

static inline jump_rule_t* find_jump_rule_linear(const sim_context_t* restrict sim_context)
{
    jump_rule_t* it = sim_context->transition_state.cur_jump_col->jump_rule_list.start_it;
    while ((it->state_0 < sim_context->transition_state.state_code) && (it < sim_context->transition_state.cur_jump_col->jump_rule_list.end_it))
    {
        if (it->state_0 == sim_context->transition_state.state_code)
        {
            return it;
        }
        ++it;
    }
    return NULL;
}

static inline jump_rule_t* find_jump_rule_binary(const jump_rule_array_t* restrict jum_rules)
{
    return NULL;
}

error_t evaluate_jump_rule(sim_context_t* restrict sim_context)
{
    sim_context->transition_state.cur_jump_rule = find_jump_rule_linear(sim_context);
    set_active_counter_col(sim_context);

    if (sim_context->transition_state.cur_jump_rule == NULL)
    {
        ++sim_context->transition_state.cur_counter_col->mc_blocks;
        return -1;
    }
    return MC_NO_ERROR;
}

static inline double get_env_energy(const env_state_t* restrict env_state, const byte_t particle_id)
{
    return ARRAY_GET(env_state->state_energies, particle_id);
}

static inline void null_state_energies_mmc(state_energies_t* restrict state_energies)
{
    state_energies->state_0 = 0;
    state_energies->state_2 = 0;
}

static inline void null_state_energies_kmc(state_energies_t* restrict state_energies)
{
    null_state_energies_mmc(state_energies);
    state_energies->state_1 = 0;
}

error_t set_state_energies_kmc(sim_context_t* restrict sim_context)
{
    null_state_energies_kmc(&sim_context->transition_state.state_energies);
    env_state_t** env_state = &sim_context->transition_state.path_env_states[0];
    jump_rule_t* jump_rule = sim_context->transition_state.cur_jump_rule;
    
    for (int32_t i = 0; i < sim_context->transition_state.cur_jump_dir->jump_length; i++)
    {
        if (env_state[0]->is_stable)
        {
            sim_context->transition_state.state_energies.state_0 += get_env_energy(env_state[i], REINTERPRET_CAST(byte_t, &jump_rule->state_0)[i]);
            sim_context->transition_state.state_energies.state_2 += get_env_energy(env_state[i], REINTERPRET_CAST(byte_t, &jump_rule->state_2)[i]);
        }
        else
        {
            sim_context->transition_state.state_energies.state_1 += get_env_energy(env_state[i], REINTERPRET_CAST(byte_t, &jump_rule->state_1)[i]);
        }
        ++env_state;
    }
    sim_context->transition_state.state_energies.state_2 += get_tmp_s2_energy_cor_kmc(sim_context);
    return MC_NO_ERROR;
}

error_t set_state_energies_mmc(sim_context_t* restrict sim_context)
{
    null_state_energies_mmc(&sim_context->transition_state.state_energies);
    env_state_t** env_state = &sim_context->transition_state.path_env_states[0];
    jump_rule_t* jump_rule = sim_context->transition_state.cur_jump_rule;

    sim_context->transition_state.state_energies.state_0 += get_env_energy(env_state[0], REINTERPRET_CAST(byte_t, &jump_rule->state_0)[0]);
    sim_context->transition_state.state_energies.state_2 += get_env_energy(env_state[0], REINTERPRET_CAST(byte_t, &jump_rule->state_2)[0]);
    sim_context->transition_state.state_energies.state_0 += get_env_energy(env_state[1], REINTERPRET_CAST(byte_t, &jump_rule->state_0)[1]);
    sim_context->transition_state.state_energies.state_2 += get_env_energy(env_state[1], REINTERPRET_CAST(byte_t, &jump_rule->state_2)[1]);

    return MC_NO_ERROR;
}

error_t set_kmc_transition_probs(sim_context_t* restrict sim_context)
{
    state_energies_t* state_energies = &sim_context->transition_state.state_energies;
    jump_probs_t* jump_probs = &sim_context->transition_state.jump_probs;

    state_energies->delta_0_to_2 = 0.5 * (state_energies->state_2 - state_energies->state_0);
    state_energies->delta_2_to_0 = -state_energies->delta_0_to_2 + state_energies->state_1;
    state_energies->delta_0_to_2 += state_energies->state_1;
    jump_probs->state_0_to_2 = exp(state_energies->delta_0_to_2);
    return MC_NO_ERROR;
}

error_t set_mmc_transition_probs(sim_context_t* restrict sim_context)
{
    state_energies_t* state_energies = &sim_context->transition_state.state_energies;
    jump_probs_t* jump_probs = &sim_context->transition_state.jump_probs;

    state_energies->delta_0_to_2 = state_energies->state_2 - state_energies->state_0;
    jump_probs->state_0_to_2 = exp(state_energies->delta_0_to_2);
    return MC_NO_ERROR;
}

error_t evaluate_kmc_transition(sim_context_t* restrict sim_context)
{
    if (sim_context->transition_state.state_energies.delta_2_to_0 < MC_NEG_ENERGY_LIMIT)
    {
        sim_context->transition_state.cur_counter_col->mc_to_unstable++;
        return MC_NO_ERROR;
    }
    if (sim_context->transition_state.state_energies.delta_0_to_2 < MC_NEG_ENERGY_LIMIT)
    {
        sim_context->transition_state.cur_counter_col->mc_on_unstable++;
        update_env_states_to_s2_kmc(sim_context);
        return MC_NO_ERROR;
    }
    if (pcg32_global_next_d() < sim_context->transition_state.jump_probs.state_0_to_2)
    {
        sim_context->transition_state.cur_counter_col->mc_steps++;
        sim_context->cur_mcs++;
        update_env_states_to_s2_kmc(sim_context);
        return MC_NO_ERROR;
    }
    else
    {
        sim_context->transition_state.cur_counter_col->mc_rejects++;
        return MC_NO_ERROR;
    }
    return MC_NO_ERROR;
}

error_t evaluate_mmc_transition(sim_context_t* restrict sim_context)
{
    if (pcg32_global_next_d() <= sim_context->transition_state.jump_probs.state_0_to_2)
    {
        sim_context->transition_state.cur_counter_col->mc_steps++;
        sim_context->cur_mcs++;

        update_env_states_to_s2_mmc(sim_context);
    }
    else
    {
        sim_context->transition_state.cur_counter_col->mc_rejects++;
    }
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