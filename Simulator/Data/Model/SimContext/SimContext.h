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

#define SCONTEXT        simContext
#define JUMPPATH        SCONTEXT->CycleState.ActPathEnvs
#define SIMERROR        SCONTEXT->ErrorCode
#define __SCONTEXT_PAR  sim_context_t* restrict SCONTEXT

typedef struct { byte_t CluId, RelId; } clu_link_t;

typedef struct { byte_t Count; clu_link_t* Start, * End; } clu_links_t;

typedef struct { int32_t EnvId, EnvPosId; clu_links_t CluLinks; } env_link_t;

typedef struct { int32_t Count; env_link_t* Start, * End; } env_links_t;

typedef struct { int32_t CodeId, CodeIdBackup; occode_t OccCode, OccCodeBackup; } clu_state_t;

typedef struct { byte_t Count; clu_state_t* Start, * End; } clu_states_t;

typedef struct { byte_t Count; double* Start, * End; } eng_states_t;

typedef struct
{
    bool_t          IsMobile, IsStable;
    byte_t          ParId, PathId;
    int32_t         EnvId, PoolId, PoolPosId;
    vector4_t       PosVector;
    eng_states_t    EnergyStates;
    clu_states_t    ClusterStates;
    env_links_t     EnvLinks;
    env_def_t*      EnvDef;
} env_state_t;

DEFINE_MD_ARRAY(env_lattice_t, env_state_t, 4);

typedef struct { int32_t EnvId, JmpId, RelId, OffId; } roll_info_t;

typedef struct { double Eng0, Eng1, Eng2, ConfDel, Prob0to2, Prob2to0; } eng_info_t;

typedef struct { int64_t CurCycles, CurMcs, MinCyclesPerBlock, McsPerBlock, CurTargetMcs, TotTargetMcs; } cycle_cnt_t;

typedef struct { double PathEnergies[8]; } env_backup_t;

typedef struct
{
    cycle_cnt_t     MainCnts;
    occode_t        ActStateCode;
    roll_info_t     ActRollInfo;
    eng_info_t      ActEngInfo;
    env_backup_t    ActEnvBackup;
    jump_dir_t*     ActJumpDir;
    jump_col_t*     ActJumpCol;
    jump_rule_t*    ActJumpRule;
    cnt_col_t*      ActCntCol;
    env_state_t*    ActPathEnvs[8];
    env_state_t*    ActWorkEnv;
    clu_state_t*    ActWorkClu;
    pair_table_t*   ActPairTable;
    clu_table_t*    ActCluTable;
} cycle_state_t;

typedef struct { int32_t * Start, * End, * CurEnd; } env_pool_t;

typedef struct { int32_t PosCount, DirCount, JumpCount; env_pool_t EnvPool; } dir_pool_t; 

typedef struct { int32_t Count; dir_pool_t * Start, * End; } dir_pools_t;

typedef struct { int32_t TotJumpCount; id_redirect_t DirCountToPoolId; dir_pools_t DirPools; } jump_pool_t;

typedef struct { clock_t StartClock, LastClock; } run_info_t;

typedef struct { double EngConvFac, TotJumpNorm, CurTimeStep; } phys_val_t;

typedef struct { char const * RunPath, * DatabasePath, * OutputPluginPath, * OutputPluginSymbol, * EnergyPluginPath, * EnergyPluginSymbol; } file_info_t;

typedef struct { int32_t Count; double LastAvg; double * Start, * CurEnd, * End; } flp_buffer_t;

typedef struct
{
    file_info_t     FileInfo;
    phys_val_t      PhysFactors;
    flp_buffer_t    LatticeEnergyBuffer;
    run_info_t      RunInfo;
    env_lattice_t   EnvLattice;
} sim_model_t;

typedef void (*f_plugin_t)(void* restrict);

typedef struct { f_plugin_t OnDataOut, OnSetJumpProbs; } plugin_col_t;

typedef struct { int32_t Count; char const * const * Values; } cmd_args_t;

typedef struct
{
    error_t         ErrorCode;
    mc_state_t      SimState;
    db_model_t      SimDbModel;
    sim_model_t     SimDynModel;
    jump_pool_t     JumpPool;
    cycle_state_t   CycleState;
    pcg32_random_t  RnGen;
    plugin_col_t    Plugins;
    cmd_args_t      RunArguments;
} sim_context_t;