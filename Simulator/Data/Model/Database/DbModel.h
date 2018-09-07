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

#define __DBMODEL_PAR db_model_t* restrict dbModel

/* General */

typedef int64_t occode_t;

typedef struct
{
    int32_t     TrackerId;
    vector3_t   Vector;

} move_t;

typedef struct 
{ 
    int32_t     A;
    int32_t     B;
    int32_t     C;

} int_range_t;

typedef struct 
{ 
    int32_t     Key; 
    int32_t     HeaderSize;
    int32_t     BlobSize;
    byte_t*     Buffer; 

} db_blob_t;

/* Structure model */

typedef struct 
{ 
    vector4_t   RelativeVector; 
    int32_t     TableId; 

} pair_def_t;

typedef struct 
{ 
    int32_t     EnvironmentPairIds[8];
    int32_t     TableId;

} clu_def_t;

typedef struct 
{ 
    int32_t     Count; 
    pair_def_t* Start;
    pair_def_t* End; 
    
} pair_defs_t;

typedef struct 
{ 
    int32_t     Count;
    clu_def_t*  Start;
    clu_def_t*  End; 

} clu_defs_t;

typedef struct 
{ 
    int32_t     ObjId; 
    byte_t      PositionParticleIds[64];
    byte_t      UpdateParticleIds[64];
    pair_defs_t PairDefinitions;
    clu_defs_t  ClusterDefinitions;

} env_def_t;

typedef struct
{
    int32_t     Count;
    env_def_t*  Start;
    env_def_t*  End;

} env_defs_t;

typedef struct
{
    int32_t         NumOfTrackersPerCell;
    int32_t         NumOfGlobalTrackers;
    int_range_t     InteractionRange;
    id_redirect_t   PositionIdToCellTrackeroffset;
    env_defs_t      EnvironmentDefinitions;
    
} str_model_t;

/* Energy model */

DEFINE_MD_ARRAY(eng_table_t, double, 2);

typedef struct
{
    int32_t     Count; 
    occode_t*   Start;
    occode_t*   End;
    
} occ_codes_t;

typedef struct 
{ 
    int32_t     ObjectId; 
    eng_table_t EnergyTable;
    
} pair_table_t;

typedef struct 
{ 
    int32_t     ObjectId; 
    occ_codes_t OccupationCodes; 
    byte_t      ParticleToTableId[64]; 
    eng_table_t EnergyTable; 
    
} clu_table_t;

typedef struct 
{
    int32_t         Count;
    pair_table_t*   Start;
    pair_table_t*   End;
    
} pair_tables_t;

typedef struct 
{
    int32_t      Count;
    clu_table_t* Start;
    clu_table_t* End;
    
} clu_tables_t;

typedef struct
{
    pair_tables_t   PairTables;
    clu_tables_t    ClusterTables;
    
} eng_model_t;

/* Transition model */

DEFINE_MD_ARRAY(jump_counts_t, int32_t, 2);

DEFINE_MD_ARRAY(jump_assign_t, int32_t, 3);

typedef struct 
{
    byte_t          Count;
    vector4_t*      Start;
    vector4_t*      End;

} jump_seq_t;

typedef struct
{
    byte_t      PathId;
    int32_t     LinkId;
    
} jump_link_t;

typedef struct
{
    byte_t          Count;
    jump_link_t*    Start;
    jump_link_t*    End;
    
} jump_links_t;

typedef struct
{
    byte_t      Count;
    move_t*     Start;
    move_t*     End;
    
} move_seq_t;

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

typedef struct
{
    int32_t     Count;
    jump_dir_t* Start;
    jump_dir_t* End;
    
} jump_dirs_t;

typedef struct
{
    occode_t    StateCode0;
    occode_t    StateCode1;
    occode_t    StateCode2;
    double      FrequencyFactor;
    double      FieldFactor;
    byte_t      TrackerOrderCode[8];
    
} jump_rule_t;

typedef struct
{
    int32_t         Count;
    jump_rule_t*    Start;
    jump_rule_t*    End;
    
} jump_rules_t;

typedef struct
{
    int32_t         ObjectId;
    bitmask_t       ParticleMask;
    jump_dirs_t     JumpDirections;
    jump_rules_t    JumpRules;
    
} jump_col_t;

typedef struct
{
    int32_t     Count;
    jump_col_t* Start;
    jump_col_t* End;
    
} jump_cols_t;

typedef struct
{
    jump_cols_t     JumpCollections;
    jump_dirs_t     JumpDirections;
    jump_counts_t   JumpCountTable;
    jump_assign_t   JumpAssignTable;

} tra_model_t;

/* Job model */

typedef struct
{
    bitmask_t   JobFlags;
    double      AbortTolerance;
    int32_t     AbortSequenceLength;
    int32_t     AbortSampleLength;
    int32_t     AbortSampleInterval;
    
} mmc_header_t;

typedef struct
{
    bitmask_t   JobFlags;
    int32_t     NumOfDynamicTrackers;
    double      FieldMagnitude;
    double      BaseFrequency;
    double      FixedNormFactor;

} kmc_header_t;

typedef struct
{
    int32_t     ObjectId;
    bitmask_t   JobFlags;
    bitmask_t   StatusFlags;
    int64_t     StateSize;
    int64_t     TargetMcsp;
    int64_t     TimeLimit;
    double      Temperature;
    double      MinimalSuccessRate;
    void *      JobHeader;
    
} job_info_t;

/* Lattice model */

DEFINE_MD_ARRAY(lattice_t, byte_t, 4);

DEFINE_MD_ARRAY(eng_background_t, double, 5);

typedef struct
{
    vector4_t           SizeVector;
    int32_t             NumOfMobiles;
    int32_t             NumOfSelectables;
    lattice_t           Lattice;
    eng_background_t    EnergyBackground;

} lat_info_t;

/* Database model */

typedef struct
{
    lat_info_t  LattInfo;
    job_info_t  JobInfo;
    str_model_t Structure;
    eng_model_t Energy;
    tra_model_t Transition;
} db_model_t;