//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	DbModel.h        		    //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Db model data types         //
//////////////////////////////////////////

#pragma once
#include "Framework/Errors/McErrors.h"
#include "Framework/Basic/BaseTypes/BaseTypes.h"
#include "Framework/Math/Types/Vector.h"

// Defines the occupation code type to be a 64 bit unsigned integer
typedef int64_t occode_t;

// Defines the db raw blob type that carries a db primary key, the header and total sizes and a pointer to the blob start
typedef struct { int32_t prim_key; int32_t header_size, blob_size; byte_t* data_ptr; } db_blob_t;

// Defines the movement vector type to be a 3D fractional double vector with an attached tracker id
typedef struct { int32_t trc_id; vector_t value; } mov_vector_t;

// Defines the basic pair interaction type with ine relative 4D vector and a pair table id for energy lookup
typedef struct { int_vector_t rel_vector; int32_t table_id; } interaction_t;

// Defines the basic cluster interaction type with 8 relative interaction ids and a cluster table id for energy lookup
typedef struct { int16_t rel_pos_ids[8]; int32_t table_id; } cluster_t;

// Defines the interaction array for storing a set of pair interactions of an environment
DEFINE_DYNAMIC_ARRAY(interaction_array_t, interaction_t);

// Defines the cluster array for storing a set of cluster interactions of an environment
DEFINE_DYNAMIC_ARRAY(cluster_array_t, cluster_t);

// Defines the environment type that describes the pair and cluster interactions for a specific unit cell position
typedef struct { int32_t obj_id; buffer_t particle_ids; interaction_array_t pair_list; cluster_array_t cluster_list; } environment_t;

// Defines the access struct for the environment array
DEFINE_DYNAMIC_ARRAY(environment_array_t, environment_t);

// Defines the energy table matrix as a two dimensional double array
DEFINE_MD_ARRAY(energy_table_t, double, 2);

// Defines the occupation code array type to a one dimensional array
DEFINE_DYNAMIC_ARRAY(occode_array_t, occode_t);

// Defines the pair energy table that describes the pair interaction energies for one pair interaction
typedef struct { int32_t obj_id; energy_table_t energy_table; } pair_table_t;

// Defines the cluster energy table that describes the energy values for one cluster type
typedef struct { int32_t obj_id; occode_array_t code_list; int32_array_t particle_id_list; energy_table_t energy_table; } cluster_table_t;

// Defines the jump sequence type to be an array of relative 4D integer vectors
DEFINE_DYNAMIC_ARRAY(jump_seq_t, int_vector_t);

// Defines the movement sequence type to be an array of movement vectors with id information
DEFINE_DYNAMIC_ARRAY(move_seq_t, mov_vector_t);

// Defines the jump direction type that describes an actual jump for a specific cell position
typedef struct { int32_t obj_id, pos_id, col_id, jump_length; double field_proj; jump_seq_t jump_sequence; move_seq_t local_mov_list, global_mov_list; } jump_dir_t;

// Defines the access struct for an array of jump directions
DEFINE_DYNAMIC_ARRAY(jump_dir_array_t, jump_dir_t);

// Defines the jump rule that describes the state change, tracking reorder and physical factors for a specific jump start state
typedef struct { occode_t state_0, state_1, state_2; double jump_rate, field_factor; byte_t trc_id_code[8]; } jump_rule_t;

// Defines the access struct for an array of jump rules
DEFINE_DYNAMIC_ARRAY(jump_rule_array_t, jump_rule_t);

// Defines the jump collection type that bundles all jumps that share the same rule set and uspported particle set during simulation
typedef struct { int32_t obj_id; bitmask_t particle_mask; jump_dir_array_t jump_dir_list; jump_rule_array_t jump_rule_list; } jump_col_t;

// Defines the structure model type that carries all shared structure related database information for the simulation
typedef struct { int32_t cell_trc_count, global_trc_count; int32_array_t cell_trc_offset_list; environment_array_t environment_list; } structure_model_t;

// Defines the access struct for the pair energy table array
DEFINE_DYNAMIC_ARRAY(pair_table_array_t, pair_table_t);

// Defines the access struct for the cluster energy table array
DEFINE_DYNAMIC_ARRAY(cluster_table_array_t, cluster_table_t);

// Defines the energy model type that carries all required cluster and energy tables for the simulation
typedef struct { pair_table_array_t pair_table_list; cluster_table_array_t cluster_table_list; } energy_model_t;

// Defines the access struct for an array of jump collection types
DEFINE_DYNAMIC_ARRAY(jump_col_array_t, jump_col_t);

// Defines the simulation db loaded lattice to be a 4D byte array
DEFINE_MD_ARRAY(md_lattice_t, byte_t, 4);

// Defines the simulation db loaded energy background to be a 5D double array
DEFINE_MD_ARRAY(md_background_t, double, 5);

// Defines the lattice db model item that carries sizes, the lattice occupation and the energy background information
typedef struct { int32_t size_a, size_b, size_c, size_d, size_e; md_lattice_t lattice; md_background_t energy_background; } lattice_info_t;

// Defines the transition model type that carries all required transition information for the simulation
typedef struct { jump_col_array_t jump_col_list; jump_dir_array_t jump_dir_list; int32_array2_t jump_count_table; int32_array3_t jump_dir_assign_table; } transition_model_t;

// Defines the basic mcs job type to carry the basic job information and a pointer to the mmc or kmc specific header buffer
typedef struct { bitmask_t job_flags, status_flags; int64_t target_mcsp, time_limit; double temperature, abort_rate; void* job_header; } job_info_t;

// Defines the mmc specific job header that carries information about dynamic simulation break criteria
typedef struct { bitmask_t job_flags; double abort_tolerance; int32_t abort_seq_length, abort_sample_length, abort_sample_interval; } mmc_job_header_t;

// Defines the kmc specific job header that carries information about field, dynamic tracking and normalization
typedef struct { bitmask_t job_flag; int32_t dyn_trc_count; double field_value, base_frequency, norm_factor; } kmc_job_header_t;

// Defines the full simulation model type with structure, energy, transition and job information
typedef struct
{
    lattice_info_t lattice_info;;
    job_info_t job_info;
    structure_model_t structure_model;
    energy_model_t energy_model;
    transition_model_t transition_model;
} db_model_t;