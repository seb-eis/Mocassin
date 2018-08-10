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

typedef struct { byte_t CluId, CluPosId; } clu_link_t;

typedef struct { byte_t Count; clu_link_t* Start, * End; } clu_links_t;

typedef struct { int32_t EnvId, EnvPosId; clu_links_t ClusterLinks; } env_link_t;

typedef struct { int32_t Count; env_link_t* Start, * End; } env_links_t;

typedef struct { int32_t CurTableId; occode_t OccCode; } clu_state_t;

typedef struct { byte_t Count; clu_state_t* Start, * End; } clu_states_t;

typedef struct { byte_t Count; double* Start, * End; } eng_states_t;

typedef struct
{
    bool_t IsMobile, IsStable;
    byte_t ParId;
    int32_t EnvId, PoolId, PoolPosId;
    vector4_t PosVector;
    eng_states_t EnergyStates;
    clu_states_t ClusterStates;
    env_links_t EnvLinks;
} env_state_t;

DEFINE_MD_ARRAY(env_lattice_t, env_state_t, 4);

typedef struct { int32_t EnvId, RelId, OffId; } roll_info_t;

typedef struct { double Eng0, Eng1, Eng2, Prob0to2, Prob2to0; } eng_info_t;

typedef struct { int64_t CycleSize, CurMcs, CurTargetMcs, TotTargetMcs; } cycle_counts_t;

typedef struct
{
    cycle_counts_t ActCounters;
    jump_dir_t* ActJumpDir;
    jump_col_t* ActJumpCol;
    jump_rule_t* ActJumpRule;
    counter_col_t* ActCounterCol;
    env_state_t* ActPathEnvs[8];
    occode_t ActStateCode;
    roll_info_t ActRollInfo;
    eng_info_t ActEngInfo;
} cycle_state_t;

typedef struct { int32_t * Start, * End, * CurEnd; } env_pool_t;

typedef struct { int32_t PosCount, DirCount, JumpCount; env_pool_t EnvPool; } dir_pool_t; 

typedef struct { int32_t Count; dir_pool_t * Start, * End; } dir_pools_t;

typedef struct { int32_t TotJumpCount; id_redirect_t DirCountToPoolId; dir_pools_t DirPools; } jump_pool_t;

typedef struct
{
    env_lattice_t EnvLattice;
} sim_model_t;

typedef struct
{
    mc_state_t SimState;
    db_model_t SimDbModel;
    sim_model_t SimDynModel;
    jump_pool_t JumpPool;
    cycle_state_t CycleState;
    pcg32_random_t RnGen;
} sim_context_t;