//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	SimContext.h        		//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Sim context (Full access)   //
//////////////////////////////////////////

#pragma once
#include "Framework/Errors/McErrors.h"
#include "Framework/Basic/BaseTypes/BaseTypes.h"
#include "Framework/Math/Random/PcgRandom.h"
#include "Simulator/Data/Model/DbModel/DbModel.h"
#include "Simulator/Data/States/SimStates.h"

// Defines the cluster updater type that carries which atoms in which clusters are affected by an environment change
typedef struct { int32_t update_count; ARRAY_OF(struct {int32_t cluster_id, rel_pos_id; }) update_info;  } cluster_update_t;

// Defines the environment link type to carry the linked env state id and relative position id ( in the linked environment) for updating purposes
typedef struct { int32_t env_state_id, env_pos_id; } env_link_t;

// Defines the cluster state type that carries the current lookup id of the cluster as well as the current occupation state code
typedef struct { int32_t lookup_id; occode_t occ_code; } cluster_state_t;

// The environment update type that carries all information required to perform updates on the state of a local environments
typedef struct 
{
    ARRAY_OF(cluster_update_t) cluster_update_info;
    environment_t* pos_env;
} env_update_t;

// Defines the array of env state update infos
DEFINE_DYNAMIC_ARRAY(env_update_array_t, env_update_t);

// Defines the cluster state array type
DEFINE_DYNAMIC_ARRAY(cluster_state_array_t, cluster_state_t);

// Defines the env link array type
DEFINE_DYNAMIC_ARRAY(env_link_array_t, env_link_t);

// The environment state type. Describes the current state, ll possible states and the linking of a local environment in the lattice
typedef struct
{
    bool_t is_mobile;
    byte_t particle_id;
    int32_t env_state_id;
    int32_t dir_pool_id, rel_pool_id;
    int_vector_t pos_vector;
    env_update_t* env_update_info;
    double_array_t state_energies;
    cluster_state_array_t cluster_states;
    env_link_array_t env_link_info;
} env_state_t;

// Defines the env state lattice to be a 4D array of env states
DEFINE_MD_ARRAY(env_state_lattice_t, env_state_t, 4);

// The roll info type that carries the two rolled id values for kmc or 3 for mmc
typedef struct { int32_t id_0, id_1, id_2 ;} roll_info_t;

// The state enenrgy type that defines the state energies for S_0, S_1 and S_2
typedef struct { double state_0, state_1, state_2; } state_energies_t;

// The transition propability type that stores the forward and backward jump probabilities
typedef struct { double state_0_to_2, state_2_to_0; } jump_probs_t;

// The simulation jump state that describes the current state of the selected transition and transition path
typedef struct
{
    jump_dir_t* cur_jump_dir;
    env_state_t* path_env_states[8];
    roll_info_t roll_info;
    state_energies_t state_energies;
    jump_probs_t jump_probs;
} transition_state_t;

// Defines multiple meta informations for the simulation
typedef struct 
{
    int32_t num_of_mobiles;
} meta_info_t;

// Defines a direction pool as a list of env state ids with the number of positions and jump direction counts
typedef struct { int32_t pos_count, dir_count, jump_count; int32_list_t env_state_ids; } dir_pool_t; 

// Defines the direction pool array type
DEFINE_DYNAMIC_ARRAY(dir_pool_array_t, dir_pool_t);

// Defines the jump direction pointer list
DEFINE_LIST(jump_dir_ptr_list_t, jump_dir_t*);

// Defines the 2D jump direction ptr table that assigns each pos_id + particle_id combination its list of possible jump direction pointers
DEFINE_MD_ARRAY(jump_dir_ptr_table_t, jump_dir_ptr_list_t, 2);

// Defines the jump pool s a set of direction pools and a total jump count. Additional the jump pool has a direction count to pool id translation array
typedef struct { int32_t jump_count; int32_array_t dir_count_to_pool_id; dir_pool_array_t dir_pools; jump_dir_ptr_table_t jump_dir_ptr_table; } jump_pool_t;

// Defines the simulation context struct that carries access pointers for all simulation data and states
typedef struct
{
    int64_t cycle_size, cur_mcs, cur_target_mcs, total_target_mcs;
    db_model_t db_model;
    mc_state_t mc_state;
    meta_info_t meta_info;
    jump_pool_t jump_pool;
    transition_state_t transition_state;
    env_state_lattice_t env_states;
    env_update_array_t env_updates;
} sim_context_t;