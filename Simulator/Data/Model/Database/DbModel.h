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
#include "Framework/Basic/BaseTypes/Buffers.h"

/* General */

typedef int64_t occode_t;

// Type for index redirection lists
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(int32_t) id_redirect_t;

// Type for tracking movement (Supports 16 bit alignment)
// Layout@ggc_x86_64 => 32@[8,8,8,4,{4}]
typedef struct
{
    vector3_t   Vector;
    int32_t     TrackerId;

    int32_t     Padding:32;

} move_t;

// Type for defining a range of unit cells (Supports 16 bit alignment)
// Layout@ggc_x86_64 => 16@[16]
typedef struct 
{ 
    vector4_t   Vector;

} int_range_t;

// Type for blob loading from the database
// Layout@ggc_x86_64 => 24@[8,4,4,4,{4}]
typedef struct 
{
    void*       Buffer;
    int32_t     Key; 
    int32_t     HeaderSize;
    int32_t     BlobSize;
    int32_t     Padding:32;

} db_blob_t;

/* Structure model */

// Type for pair interaction definitions (Supports 16 bit alignment)
// Layout@ggc_x86_64 => 32@[20,4,{8}]
typedef struct 
{
    vector4_t   RelativeVector;
    int32_t     TableId;

    int64_t     Padding:64;

} pair_def_t;

// Type for cluster interaction definitions
// Layout@ggc_x86_64 => 40@[8x4,4,{4}]
typedef struct 
{ 
    int32_t     EnvironmentPairIds[8];
    int32_t     TableId;

    int32_t     Padding:32;

} clu_def_t;

// List type for pair definitions
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(pair_def_t) pair_defs_t;

// List type for cluster definitions
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(clu_def_t) clu_defs_t;

// Type for full environment definitions
// Layout@ggc_x86_64 => 168@[4,{4},16,16,64,64]
typedef struct 
{
    int32_t     ObjId;

    int32_t     Padding:32;

    pair_defs_t PairDefinitions;
    clu_defs_t  ClusterDefinitions;
    byte_t      PositionParticleIds[64];
    byte_t      UpdateParticleIds[64];

} env_def_t;

// Type for lists of environment definitions
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(env_def_t) env_defs_t;

// Type for the structure model
// Layout@ggc_x86_64 => 56@[4,4,16,16,16]
typedef struct
{
    int32_t         NumOfTrackersPerCell;
    int32_t         NumOfGlobalTrackers;
    int_range_t     InteractionRange;
    id_redirect_t   PositionIdToCellTrackeroffset;
    env_defs_t      EnvironmentDefinitions;
    
} str_model_t;

/* Energy model */

// Type for 2d rectangular energy tables
// Layout@ggc_x86_64 => 24@[8,8,8]
typedef Array_t(double, 2) eng_table_t;

// Type for lists of occupation codes
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(occode_t) occ_codes_t;

// Type for pair tables to store energies of pair interactions
// Layout@ggc_x86_64 => 32@[24,4,{4}]
typedef struct 
{
    eng_table_t EnergyTable;
    int32_t     ObjectId;

    int32_t     Padding:32;

} pair_table_t;

// Type for cluster tables to store energies of pair interactions
// Layout@ggc_x86_64 => 112@[16,24,4,64,{4}]
typedef struct 
{
    occ_codes_t OccupationCodes;
    eng_table_t EnergyTable;
    int32_t     ObjectId;
    byte_t      ParticleToTableId[64];

    int32_t     Padding:32;
    
} clu_table_t;

// List type for pair table access
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(pair_table_t) pair_tables_t;

// List type for cluster table access
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(clu_table_t) clu_tables_t;

// Type for the energy model
// Layout@ggc_x86_64 => 32@[16,16]
typedef struct
{
    pair_tables_t   PairTables;
    clu_tables_t    ClusterTables;
    
} eng_model_t;

/* Transition model */

// Type for 2d rectangular jump count tables
// Layout@ggc_x86_64 => 24@[8,8,8]
typedef Array_t(int32_t, 2) jump_counts_t;

// Type for 3d jump id assignment tables
// Layout@ggc_x86_64 => 24@[8,8,8]
typedef Array_t(int32_t, 3) jump_assign_t;

// Type for jump sequence lists
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(vector4_t) jump_seq_t;

// Type for jump links
// Layout@ggc_x86_64 => 8@[4,4]
typedef struct
{
    int32_t     PathId;
    int32_t     LinkId;
    
} jump_link_t;

// Type for jump link lists
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(jump_link_t) jump_links_t;

// Type for movement sequence lists
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(move_t) move_seq_t;

// Type for jump direction definitions
// Layout@ggc_x86_64 => 88@[4,4,4,4,8,16,16,16,16]
typedef struct
{
    int32_t         ObjectId;
    int32_t         PositionId;
    int32_t         CollectionId;
    int32_t         JumpLength;
    double          FieldProjectionFactor;
    jump_seq_t      JumpSequence;
    jump_links_t    JumpLinkSequence;
    move_seq_t      LocalMoveSequence;
    move_seq_t      GlobalMoveSequence;
    
} jump_dir_t;

// Type for jump direction lists
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(jump_dir_t) jump_dirs_t;

// Type for a transition jump rule
// Layout@ggc_x86_64 => 48@[8,8,8,8,8,8]
typedef struct
{
    occode_t    StateCode0;
    occode_t    StateCode1;
    occode_t    StateCode2;
    double      FrequencyFactor;
    double      FieldFactor;
    byte_t      TrackerOrderCode[8];
    
} jump_rule_t;

// Type for jump rule lists
// Layout@ggc_x86_64 => 16@[8,8}]
typedef Span_t(jump_rule_t) jump_rules_t;

// Type for jump collections
// Layout@ggc_x86_64 => 48@[8,16,16,4,{4}]
typedef struct
{
    bitmask_t       ParticleMask;
    jump_dirs_t     JumpDirections;
    jump_rules_t    JumpRules;
    int32_t         ObjectId;

    int32_t         Padding:32;
} jump_col_t;

// Type for jump collection lists
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(jump_col_t) jump_cols_t;

// Type for the transition model
// Layout@ggc_x86_64 => 80@[16,16,24,24]
typedef struct
{
    jump_cols_t     JumpCollections;
    jump_dirs_t     JumpDirections;
    jump_counts_t   JumpCountTable;
    jump_assign_t   JumpAssignTable;

} tra_model_t;

/* Job model */

// Type for the mmc job header
// Layout@ggc_x86_64 => 32@[8,8,4,4,4,{4}]
typedef struct
{
    bitmask_t   JobFlags;
    double      AbortTolerance;
    int32_t     AbortSequenceLength;
    int32_t     AbortSampleLength;
    int32_t     AbortSampleInterval;

    int32_t     Padding:32;
    
} mmc_header_t;

// Type for the kmc job header
// Layout@ggc_x86_64 => 40@[8,8,8,8,4,{4}]
typedef struct
{
    bitmask_t   JobFlags;
    double      FieldMagnitude;
    double      BaseFrequency;
    double      FixedNormFactor;
    int32_t     NumOfDynamicTrackers;

    int32_t     Padding:32;

} kmc_header_t;

// Type for the job info
// Layout@ggc_x86_64 => 72@[8,8,8,8,8,8,8,8,4,{4}]
typedef struct
{
    bitmask_t   JobFlags;
    bitmask_t   StatusFlags;
    int64_t     StateSize;
    int64_t     TargetMcsp;
    int64_t     TimeLimit;
    double      Temperature;
    double      MinimalSuccessRate;
    void *      JobHeader;
    int32_t     ObjectId;

    int32_t     Padding:32;
    
} job_info_t;

/* Lattice model */

// Type for the byte based 4d rectangular lattice access
// Layout@ggc_x86_64 => 24@[8,8,8]
typedef Array_t(byte_t, 4) lattice_t;

// Type for the double 5D rectangular energy background access
typedef Array_t(double, 5) eng_background_t;

// Type for the lattice information (Supports 16 bit alignment)
// Layout@ggc_x86_64 => 80@[16,4,4,24,24,{8}]
typedef struct
{
    vector4_t           SizeVector;
    int32_t             NumOfMobiles;
    int32_t             NumOfSelectables;
    lattice_t           Lattice;
    eng_background_t    EnergyBackground;

    int64_t             Padding:64;

} lat_info_t;

/* Database model */

// Type for the database model context
// Layout@ggc_x86_64 => 320@[80,72,56,32,80]
typedef struct
{
    lat_info_t  LattInfo;
    job_info_t  JobInfo;
    str_model_t Structure;
    eng_model_t Energy;
    tra_model_t Transition;
} db_model_t;