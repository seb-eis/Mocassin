//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	JumpSelection.c        		//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Jump selection logic        //
//////////////////////////////////////////

#include "Simulator/Logic/Objects/JumpSelection.h"

static int32_t get_max_jump_count(const int32_array2_t* restrict jump_count_table)
{
    // Search the jump table for the largest entry
    int32_t count = 0;
    for (int32_t* it = jump_count_table->start_it; it < jump_count_table->end_it; it++)
    {
        count = (*it > count) ? *it : count;
    }
    return count;
}

static int32_t set_id_redirection_array(const int32_t max_jump_count, const int32_array2_t* restrict jump_count_table, jump_pool_t* restrict jump_pool)
{
    // Build the id redirection array to correct size
    buffer_t buffer = allocate_buffer(max_jump_count + 1, sizeof(int32_t));
    jump_pool->dir_count_to_pool_id = BUFFER_TO_ARRAY(buffer, int32_array_t);

    // Set all redirects to the 'no entry' tag
    for (int32_t* it = jump_pool->dir_count_to_pool_id.start_it; it < jump_pool->dir_count_to_pool_id.end_it; it++)
    {
        *it = NO_JUMP_POOL_ENTRY;
    }

    // Redirect existing pool sizes in order of appereance (if not yet redirected)
    int32_t index = 0;
    for (int32_t* it = jump_count_table->start_it; it < jump_count_table->end_it; it++)
    {
        if ((*it > 0) && (ARRAY_GET(jump_pool->dir_count_to_pool_id, *it) == NO_JUMP_POOL_ENTRY))
        {
            ARRAY_GET(jump_pool->dir_count_to_pool_id, *it) = index++;
        }
    }

    // Return the number of created pools
    return index;
}

static void set_jump_dir_counts(const int32_t max_jump_count, jump_pool_t* restrict jump_pool)
{
    // Sets the jump direction counts on all existing jump direction pools
    for (int32_t i = 0; i <= max_jump_count; i++)
    {
        int32_t pool_id = ARRAY_GET(jump_pool->dir_count_to_pool_id, i);
        if (pool_id != NO_JUMP_POOL_ENTRY)
        {
            ARRAY_GET(jump_pool->dir_pools, pool_id).dir_count = i;
        }
    }
}

static error_t allocate_direction_pools(const int32_t pool_count, const int32_t pool_size, jump_pool_t* restrict jump_pool)
{
    buffer_t tmp_buffer;

    // Allocate the required pool array buffers. Retruns mem allocation error if buffer allocation fails
    if (allocate_buffer_checked(pool_count, sizeof(dir_pool_t), &tmp_buffer) != MC_NO_ERROR)
    {
        return MC_MEM_ALLOCATION_ERROR;
    }
    jump_pool->dir_pools = BUFFER_TO_ARRAY(tmp_buffer, dir_pool_array_t);

    // Allocate the required dir pool buffers and set the dir pools to default conditions
    for (dir_pool_t* it = jump_pool->dir_pools.start_it; it < jump_pool->dir_pools.end_it; it++)
    {
        if (allocate_buffer_checked(pool_size, sizeof(int32_t), &tmp_buffer))
        {
            return MC_MEM_ALLOCATION_ERROR;
        }
        it->env_state_ids = BUFFER_TO_LIST(tmp_buffer, int32_list_t);
        it->jump_count = 0;
        it->pos_count = 0;
    }
    return MC_NO_ERROR;
}

static error_t allocate_jump_dir_ptr_table(jump_dir_ptr_table_t* restrict jump_dir_ptr_table, const int32_array2_t* restrict jump_count_table)
{
    // Tmp items for memory allocation
    buffer_t tmp_buffer;
    blob_t tmp_blob;

    // Allocate the correct list sizes for each pos_id + particle_id combination
    int32_t id_max[2] = {0,0};
    get_md_array_dimensions((int32_t*)jump_count_table->header, id_max);

    // Allocate a 2D array of list that matches the required combinations of pos_ids and part_ids
    if (allocate_md_array_checked(2, sizeof(jump_dir_ptr_list_t), id_max, &tmp_blob) != MC_NO_ERROR)
    {
        return MC_MEM_ALLOCATION_ERROR;
    }
    *jump_dir_ptr_table = BLOB_CAST(jump_dir_ptr_table_t, tmp_blob);

    // Allocate a ptr list of correct size for each combination
    for (int32_t pos_id = 0; pos_id < id_max[0]; pos_id++)
    {
        for (int32_t part_id = 0; part_id < id_max[1]; part_id++)
        {
            int32_t jump_count = *MDA_GET_2(*jump_count_table, pos_id, part_id);
            if (jump_count > 0)
            {
                if (allocate_buffer_checked(jump_count, sizeof(jump_dir_t*), &tmp_buffer) != MC_NO_ERROR)
                {
                    return MC_MEM_ALLOCATION_ERROR;
                }
                *MDA_GET_2(*jump_dir_ptr_table, pos_id, part_id) = BUFFER_TO_LIST(tmp_buffer, jump_dir_ptr_list_t);
            }
        }
    }
    return MC_NO_ERROR;
}

static void add_jump_dir_ptr_for_all_particle_ids(jump_dir_ptr_table_t* restrict jump_ptr_table, occode_t particle_mask, jump_dir_t* restrict jump_dir)
{
    int32_t part_id = 0;
    while (particle_mask != 0)
    {
        if ((particle_mask & 1ULL) == 1ULL)
        {
            jump_dir_ptr_list_t* dir_list = MDA_GET_2(*jump_ptr_table, jump_dir->pos_id, part_id);
            LIST_ADD(*dir_list, jump_dir);
        }
        particle_mask >>= 1;
        ++part_id;
    }
}

static error_t fill_jump_dir_ptr_table(jump_dir_ptr_table_t* restrict jump_ptr_table, const jump_col_array_t* restrict jump_cols)
{
    // Iterate over all jump collections and add the pointers to all
    int32_t id_max[2] = {0,0};
    get_md_array_dimensions((int32_t*)jump_ptr_table->header, id_max);

    for (jump_col_t* jump_col = jump_cols->start_it; jump_col < jump_cols->end_it; jump_col++)
    {
        for (jump_dir_t* jump_dir = jump_col->jump_dir_list.start_it; jump_dir < jump_col->jump_dir_list.end_it; jump_dir++)
        {
            add_jump_dir_ptr_for_all_particle_ids(jump_ptr_table, jump_col->particle_mask, jump_dir);
        }
    }
    return MC_NO_ERROR;
}

static error_t build_new_pool(sim_context_t* restrict sim_context)
{
    // Collect the build information and create the id redirection array
    int32_t max_jump_count = get_max_jump_count(&sim_context->db_model.transition_model.jump_count_table);
    int32_t pool_count = set_id_redirection_array(max_jump_count, &sim_context->db_model.transition_model.jump_count_table, &sim_context->jump_pool);

    if ((max_jump_count <= 0) || (pool_count <= 0))
    {
        return MC_SIM_ERROR;
    }

    // Allocate the direction pools and set the jump direction counts on the created pools
    if (allocate_direction_pools(pool_count, sim_context->meta_info.num_of_mobiles, &sim_context->jump_pool) != MC_NO_ERROR)
    {
        return MC_MEM_ALLOCATION_ERROR;
    }
    set_jump_dir_counts(max_jump_count, &sim_context->jump_pool);

    // Allocate and fill the direction pointer table for the pos_id + part_id combinations
    if (allocate_jump_dir_ptr_table(&sim_context->jump_pool.jump_dir_ptr_table, &sim_context->db_model.transition_model.jump_count_table) != MC_NO_ERROR)
    {
        return MC_MEM_ALLOCATION_ERROR;
    }
    fill_jump_dir_ptr_table(&sim_context->jump_pool.jump_dir_ptr_table, &sim_context->db_model.transition_model.jump_col_list);
    return MC_NO_ERROR;
}

static error_t sync_jump_pool_counters(jump_pool_t* restrict jump_pool)
{
    // Syncs all jump pool counters
    for (dir_pool_t* it = jump_pool->dir_pools.start_it; it < jump_pool->dir_pools.end_it; it++)
    {
        it->jump_count = it->pos_count * it->jump_count;
        jump_pool->jump_count += it->jump_count;
    }

    // Return ok if the total jump count is not zero
    if (jump_pool->jump_count <= 0)
    {
        return MC_SIM_ERROR;
    }
    return MC_NO_ERROR;
}

static error_t sync_env_states_and_jump_pool(sim_context_t* restrict sim_context)
{
    // Assign each env state its pool id and relative pool id information (if mobile) or the no jump pool tag (not mobile)
    for (env_state_t* it = sim_context->env_states.start_it; it < sim_context->env_states.end_it; it++)
    {
        if (it->is_mobile == true)
        {
            int32_t jump_count = *MDA_GET_2(sim_context->db_model.transition_model.jump_count_table, it->pos_vector.d, it->particle_id);
            it->dir_pool_id = ARRAY_GET(sim_context->jump_pool.dir_count_to_pool_id, jump_count);
            dir_pool_t* dir_pool = ARRAY_GET_PTR(sim_context->jump_pool.dir_pools, it->dir_pool_id);
            LIST_ADD(dir_pool->env_state_ids, it->env_state_id);
            it->rel_pool_id = dir_pool->pos_count;
            dir_pool->pos_count++;
        }
        else
        {
            it->dir_pool_id = NO_JUMP_POOL_ENTRY;
            it->rel_pool_id = NO_JUMP_POOL_ENTRY;
        }
    }

    return sync_jump_pool_counters(&sim_context->jump_pool);
}

error_t init_jump_pool(sim_context_t* restrict sim_context)
{
    // Build and initialize a pool to the required sizes
    if (build_new_pool(sim_context) != MC_NO_ERROR)
    {
        MC_DUMP_ERROR_AND_EXIT(MC_SIM_ERROR, "Error during jump pool construction");
    }

    // Synchronize the env states with the pool 
    if (sync_env_states_and_jump_pool(sim_context) != MC_NO_ERROR)
    {
        MC_DUMP_ERROR_AND_EXIT(MC_SIM_ERROR, "Error during synchronization of env states and jump pool");
    }
    return MC_NO_ERROR;
}

void roll_kmc_transition_info(jump_pool_t* restrict jump_pool, roll_info_t* restrict roll_info)
{
    int32_t rn_value = (int32_t)(pcg32_global_next() % jump_pool->jump_count);   
    
    for(dir_pool_t* it = jump_pool->dir_pools.start_it; it < jump_pool->dir_pools.end_it; it++)
    {
        if (rn_value >= it->jump_count)
        {
            rn_value -= it->jump_count;
        }
        else
        {
            int32_t entry_id = rn_value / it->dir_count;
            roll_info->id_0 = ARRAY_GET(it->env_state_ids, entry_id);
            roll_info->id_1 = rn_value % it->dir_count;
            break;
        }
    }   
}

void roll_mmc_transition_info(jump_pool_t* restrict jump_pool, const int32_t env_state_count, roll_info_t* restrict roll_info)
{
    // Roll start and direction analog to kmc and additional one of all existing env states to use as a random offset source
    roll_kmc_transition_info(jump_pool, roll_info);
    roll_info->id_2 = (int32_t)(pcg32_global_next() % env_state_count);
}

static inline int32_t get_dir_pool_id(const env_state_t* restrict env_state, const int32_array2_t* restrict dir_count_table, const jump_pool_t* restrict jump_pool)
{
    int32_t dir_count = *MDA_GET_2(*dir_count_table, env_state->pos_vector.d, env_state->particle_id);
    return ARRAY_GET(jump_pool->dir_count_to_pool_id, dir_count);
}

static int32_t change_env_pool_entry(env_state_t* restrict env_state, const int32_t new_pool_id, jump_pool_t* restrict jump_pool)
{
    // Take the last entry in the original pool to replace the original pool entry and add the new entry to the list of the new pool id
    dir_pool_t* old_pool = ARRAY_GET_PTR(jump_pool->dir_pools, env_state->dir_pool_id);
    dir_pool_t* new_pool = ARRAY_GET_PTR(jump_pool->dir_pools, new_pool_id);
    ARRAY_GET(old_pool->env_state_ids, env_state->rel_pool_id) = *LIST_POP_BACK(old_pool->env_state_ids);
    LIST_ADD(new_pool->env_state_ids, env_state->env_state_id);

    // Correct the direction pool counter values and the jump pool total size
    new_pool->pos_count++;
    old_pool->pos_count--;
    new_pool->jump_count += new_pool->dir_count;
    old_pool->jump_count -= old_pool->dir_count;
    jump_pool->jump_count = jump_pool->jump_count + new_pool->dir_count - old_pool->dir_count;

    // Set the new id values on the env state and return the change im jump counts
    env_state->dir_pool_id = new_pool_id;
    env_state->rel_pool_id = LIST_GET_LAST_INDEX(new_pool->env_state_ids, sizeof(int32_t));

    return new_pool->dir_count - old_pool->dir_count;
}

bool_t update_jump_pool(sim_context_t* restrict sim_context)
{
    int32_t jump_count_delta = 0;
    for (size_t i = 0; i < sim_context->transition_state.cur_jump_dir->jump_length; i++)
    {
        env_state_t* cur_env = sim_context->transition_state.path_env_states[i];
        if (cur_env->dir_pool_id != NO_JUMP_POOL_ENTRY)
        {
            int32_t new_pool_id = get_dir_pool_id(cur_env, &sim_context->db_model.transition_model.jump_count_table, &sim_context->jump_pool);
            if (new_pool_id != cur_env->dir_pool_id)
            {
                jump_count_delta += change_env_pool_entry(cur_env, new_pool_id, &sim_context->jump_pool);
            }
        }
    }
    return (jump_count_delta != 0);
}