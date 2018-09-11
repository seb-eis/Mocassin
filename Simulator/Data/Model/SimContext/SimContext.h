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
#include "Framework/Basic/BaseTypes/Buffers.h"
#include "Framework/Math/Random/PcgRandom.h"
#include "Simulator/Data/Model/Database/DbModel.h"
#include "Simulator/Data/Model/State/StateModel.h"

// Type for cluster links
// Layout@ggc_x86_64 => 2@[1,1]
typedef struct
{
    byte_t ClusterId;
    byte_t CodeByteId;
    
} clu_link_t;

// Type for cluster link lists
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(clu_link_t) clu_links_t;

// Type for an environment link
// Layout@ggc_x86_64 => 24@[4,4,16]
typedef struct
{
    int32_t     EnvironmentId;
    int32_t     PairId;
    clu_links_t ClusterLinks;
    
} env_link_t;

// Type for env link list that supports push back operation
// Layout@ggc_x86_64 => 24@[8,8,8]
typedef List_t(env_link_t) env_links_t;

// Type for cluster states and affiliated backups
// Layout@ggc_x86_64 => 24@[4,4,8,8]
typedef struct
{
    int32_t     CodeId;
    int32_t     CodeIdBackup;
    occode_t    OccupationCode;
    occode_t    OccupationCodeBackup;
    
} clu_state_t;

// Type for lists of cluster states
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(clu_state_t) clu_states_t;

// Type for lists of energy states
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(double) eng_states_t;

// Type for a full environment state definition (Supports 16 bit alignment)
// Layout@ggc_x86_64 => 96@[1,1,1,1,4,4,4,16,16,16,24,8]
typedef struct
{
    bool_t          IsMobile;
    byte_t          IsStable;
    byte_t          ParticleId;
    byte_t          PathId;
    int32_t         EnvironmentId;
    int32_t         PoolId;
    int32_t         PoolPositionId;
    vector4_t       PositionVector;
    eng_states_t    EnergyStates;
    clu_states_t    ClusterStates;
    env_links_t     EnvironmentLinks;
    env_def_t*      EnvironmentDefinition;

} env_state_t;

// Type for the 4d rectangular environment state lattice access
// Layout@ggc_x86_64 => 24@[8,8,8]
typedef Array_t(env_state_t, 4) env_lattice_t;

// Type for the jump selection index information
// Layout@ggc_x86_64 => 16@[4,4,4,4]
typedef struct
{
    int32_t EnvironmentId;
    int32_t JumpId;
    int32_t RelativeId;
    int32_t OffsetId;
    
} roll_info_t;

// Type for the transition energy information
// Layout@ggc_x86_64 => 48@[8,8,8,8,8,8]
typedef struct
{
    double Energy0;
    double Energy1;
    double Energy2;
    double ConformationDelta;
    double Probability0to2;
    double Probability2to0;
    
} eng_info_t;

// Type for the internal simulation cycle counters
// Layout@ggc_x86_64 => 48@[8,8,8,8,8,8]
typedef struct
{
    int64_t Cycles;
    int64_t Mcs;
    int64_t CyclesPerBlock;
    int64_t McsPerBlock;
    int64_t StepGoalMcs;
    int64_t TotalGoalMcs;
    
} cycle_cnt_t;

// Type for the path energy backups
// Layout@ggc_x86_64 => 64@[8x8]
typedef struct
{
    double PathEnergies[8];
    
} env_backup_t;

// Type for the cycle state storage
// Layout@ggc_x86_64 => 208@[48,8,16,64,8,8,8,8,8,8,8,8]
typedef struct
{
    cycle_cnt_t     MainCounters;
    occode_t        ActiveStateCode;
    roll_info_t     ActiveSelectionInfo;
    eng_info_t      ActiveEnergyInfo;
    env_backup_t    ActiveEnvironmentBackup;
    jump_dir_t*     ActiveJumpDirection;
    jump_col_t*     ActiveJumpCollection;
    jump_rule_t*    ActiveJumpRule;
    cnt_col_t*      ActiveCounterCollection;
    env_state_t*    ActivePathEnvironments[8];
    env_state_t*    WorkEnvironment;
    clu_state_t*    WorkCluster;
    pair_table_t*   WorkPairTable;
    clu_table_t*    WorkClusterTable;
} cycle_state_t;

// Type for the environment pool access
// Layout@ggc_x86_64 => 24@[8,8,8]
typedef List_t(int32_t) env_pool_t;

// Type for the direction pools
// Layout@ggc_x86_64 => 40@[24,4,4,4,{4}]
typedef struct
{
    env_pool_t  EnvironmentPool;
    int32_t     NumOfPositions;
    int32_t     NumOfDirections;
    int32_t     NumOfJumps;

    int32_t     Padding:32;
    
} dir_pool_t;

// Type for lists of direction pools
// Layout@ggc_x86_64 => 40@[24,4,4,4,{4}]
typedef Span_t(dir_pool_t) dir_pools_t;

// Type for the jump selection pool
// Layout@ggc_x86_64 => 64@[4,4,16,40]
typedef struct
{
    int32_t         NumOfSelectableJumps;
    int32_t         NumOfDirectionPools;
    id_redirect_t   NumOfDirectionsToPoolId;
    dir_pools_t     DirectionPools;
    
} jump_pool_t;

// Type for the program run information
// Layout@ggc_x86_64 => 16@[8,8]
typedef struct
{
    int64_t StartClock;
    int64_t LastClock;
} run_info_t;

// Type for physical simulation values
// Layout@ggc_x86_64 => 24@[8,8,8]
typedef struct
{
    double EnergyConversionFactor;
    double TotalNormalizationFactor;
    double CurrentTimeStepping;
    
} phys_val_t;

// Type for the file string information
// Layout@ggc_x86_64 => 56@[8,8,8,8,8,8,8]
typedef struct
{
    char const* DbQueryString;
    char const* ExecutionPath;
    char const* DatabasePath;
    char const* OutputPluginPath;
    char const* OutputPluginSymbol;
    char const* EnergyPluginPath;
    char const* EnergyPluginSymbol;
    
} file_info_t;

// Type for floating point buffers with storage of last average
// Layout@ggc_x86_64 => 32@[8,8,8,8]
typedef struct
{
    double* Begin;
    double* End;
    double* CapacityEnd;
    double  LastAverage;
    
} flp_buffer_t;

// Type for the simulation dynamic model
// Layout@ggc_x86_64 => 152@[56,24,32,16,24]
typedef struct
{
    file_info_t     FileInfo;
    phys_val_t      PhysicalFactors;
    flp_buffer_t    LatticeEnergyBuffer;
    run_info_t      RuntimeInfo;
    env_lattice_t   EnvironmentLattice;
} sim_model_t;

// Type for plugin function pointers
typedef void (*f_plugin_t)(void* restrict);

// Type for storing multiple plugin function pointers
// Layout@ggc_x86_64 => 16@[8,8]
typedef struct
{ 
    f_plugin_t OnDataOutput;
    f_plugin_t OnSetJumpProbabilities;
    
} plugin_col_t;

// Type for storing the programs cmd arguments
// Layout@ggc_x86_64 => 16@[8,4,{4}]
typedef struct
{
    char const* const*  Values;
    int32_t             Count;

    int32_t             Padding:32;

} cmd_args_t;

// Type for the full simulation context that provides access to all simulation data structures
// Layout@ggc_x86_64 => 32@[4,]
typedef struct
{
    mc_state_t      MainState;
    cycle_state_t   CycleState;
    db_model_t      DbModel;
    sim_model_t     DynamicModel;
    jump_pool_t     SelectionPool;
    pcg32_random_t  RandomNumberGenerator;
    plugin_col_t    Plugins;
    cmd_args_t      CommandArguments;
    error_t         ErrorCode;
    
} sim_context_t;