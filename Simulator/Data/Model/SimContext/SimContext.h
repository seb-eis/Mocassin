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
#include "Simulator/Data/Model/State/StateModel.h"

typedef struct
{
    byte_t ClusterId;
    byte_t CodeByteId;
    
} clu_link_t;

typedef struct
{
    int32_t     Count;
    clu_link_t* Start;
    clu_link_t* End;
    
} clu_links_t;

typedef struct
{
    int32_t     EnvironmentId;
    int32_t     PairId;
    clu_links_t ClusterLinks;
    
} env_link_t;

typedef struct
{
    int32_t     Count;
    env_link_t* Start;
    env_link_t* End;
    env_link_t* CurrentEnd;
    
} env_links_t;

typedef struct
{
    int32_t     CodeId;
    int32_t     CodeIdBackup;
    occode_t    OccupationCode;
    occode_t    OccupationCodeBackup;
    
} clu_state_t;

typedef struct
{
    byte_t          Count;
    clu_state_t*    Start;
    clu_state_t*    End;
    
} clu_states_t;

typedef struct
{
    byte_t  Count;
    double* Start;
    double* End;
    
} eng_states_t;

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

DEFINE_MD_ARRAY(env_lattice_t, env_state_t, 4);

typedef struct
{
    int32_t EnvironmentId;
    int32_t JumpId;
    int32_t RelativeId;
    int32_t OffsetId;
    
} roll_info_t;

typedef struct
{
    double Energy0;
    double Energy1;
    double Energy2;
    double ConformationDelta;
    double Probability0to2;
    double Probability2to0;
    
} eng_info_t;

typedef struct
{
    int64_t Cycles;
    int64_t Mcs;
    int64_t CyclesPerBlock;
    int64_t McsPerBlock;
    int64_t StepGoalMcs;
    int64_t TotalGoalMcs;
    
} cycle_cnt_t;

typedef struct
{
    double PathEnergies[8];
    
} env_backup_t;

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

typedef struct
{
    int32_t* Start;
    int32_t* End;
    int32_t* CurrentEnd;
    
} env_pool_t;

typedef struct
{
    int32_t     NumOfPositions;
    int32_t     NumOfDirections;
    int32_t     NumOfJumps;
    env_pool_t  EnvironmentPool;
    
} dir_pool_t; 

typedef struct
{
    int32_t     Count;
    dir_pool_t* Start;
    dir_pool_t* End;
    
} dir_pools_t;

typedef struct
{
    int32_t         NumOfSelectableJumps;
    id_redirect_t   NumOfDirectionsToPoolId;
    dir_pools_t     DirectionPools;
    
} jump_pool_t;

typedef struct
{
    clock_t StartClock;
    clock_t LastClock;
} run_info_t;

typedef struct
{
    double EnergyConversionFactor;
    double TotalNormalizationFactor;
    double CurrentTimeStepping;
    
} phys_val_t;

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

typedef struct
{
    int32_t Count;
    double  LastAverage;
    double* Start;
    double* CurrentEnd;
    double* End;
    
} flp_buffer_t;

typedef struct
{
    file_info_t     FileInfo;
    phys_val_t      PhysicalFactors;
    flp_buffer_t    LatticeEnergyBuffer;
    run_info_t      RuntimeInfo;
    env_lattice_t   EnvironmentLattice;
} sim_model_t;

typedef void (*f_plugin_t)(void* restrict);

typedef struct
{ 
    f_plugin_t OnDataOutput;
    f_plugin_t OnSetJumpProbabilities;
    
} plugin_col_t;

typedef struct
{
    int32_t             Count;
    char const* const*  Values;

} cmd_args_t;

typedef struct
{
    error_t         ErrorCode;
    mc_state_t      MainState;
    cycle_state_t   CycleState;
    db_model_t      DbModel;
    sim_model_t     DynamicModel;
    jump_pool_t     SelectionPool;
    pcg32_random_t  RandomNumberGenerator;
    plugin_col_t    Plugins;
    cmd_args_t      CommandArguments;
    
} sim_context_t;