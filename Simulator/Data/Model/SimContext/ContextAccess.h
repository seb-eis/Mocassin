//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	ContextAccess.h        		//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Get/Set for sim context     //
//////////////////////////////////////////

#include "Framework/Math/Random/PcgRandom.h"
#include "Framework/Basic/BaseTypes/BaseTypes.h"
#include "Framework/Math/Types/Vector.h"
#include "Simulator/Data/Model/SimContext/SimContext.h"

/* Context getter/setter */

static inline mc_state_t* Get_SimulationState(__SCONTEXT_PAR)
{
    return &SCONTEXT->SimState;
}

static inline db_model_t* Get_DatabaseModel(__SCONTEXT_PAR)
{
    return &SCONTEXT->SimDbModel;
}

static inline sim_model_t* Get_DynamicModel(__SCONTEXT_PAR)
{
    return &SCONTEXT->SimDynModel;
}

static inline jump_pool_t* Get_JumpPool(__SCONTEXT_PAR)
{
    return &SCONTEXT->JumpPool;
}

static inline cycle_state_t* Get_CycleState(__SCONTEXT_PAR)
{
    return &SCONTEXT->CycleState;
}

static inline pcg32_random_t* Get_RandomNumberGen(__SCONTEXT_PAR)
{
    return &SCONTEXT->RnGen;
}

static inline plugin_col_t* Get_PluginCollection(__SCONTEXT_PAR)
{
    return &SCONTEXT->Plugins;
}

/* Simulation model getter/setter */

static inline file_info_t* Get_FileInformation(__SCONTEXT_PAR)
{
    return &Get_DynamicModel(SCONTEXT)->FileInfo;
}

static inline phys_val_t* Get_PhysicalFactors(__SCONTEXT_PAR)
{
    return &Get_DynamicModel(SCONTEXT)->PhysFactors;
}

static inline flp_buffer_t* Get_LatticeEnergyBuffer(__SCONTEXT_PAR)
{
    return &Get_DynamicModel(SCONTEXT)->EngBuffer;
}

static inline run_info_t* Get_RuntimeInformation(__SCONTEXT_PAR)
{
    return &Get_DynamicModel(SCONTEXT)->RunInfo;
}

static inline env_lattice_t* Get_EnvironmentLattice(__SCONTEXT_PAR)
{
    return &Get_DynamicModel(SCONTEXT)->EnvLattice;
}

/* Cycle state getter/setter */

static inline cycle_cnt_t* Get_MainCycleCounters(__SCONTEXT_PAR)
{
    return &Get_CycleState(SCONTEXT)->MainCnts;
}

static inline occode_t Get_PathStateCode(__SCONTEXT_PAR)
{
    return Get_CycleState(SCONTEXT)->ActStateCode;
}

static inline void Set_PathStateCode(__SCONTEXT_PAR, const occode_t code)
{
    Get_CycleState(SCONTEXT)->ActStateCode = code;
}

static inline roll_info_t* Get_JumpSelectionInfo(__SCONTEXT_PAR)
{
    return &Get_CycleState(SCONTEXT)->ActRollInfo;
}

static inline eng_info_t* Get_JumpEnergyInfo(__SCONTEXT_PAR)
{
    return &Get_CycleState(SCONTEXT)->ActEngInfo;
}

static inline env_backup_t* Get_EnvironmentBackup(__SCONTEXT_PAR)
{
    return &Get_CycleState(SCONTEXT)->ActEnvBackup;
}

static inline jump_dir_t* Get_ActiveJumpDirection(__SCONTEXT_PAR)
{
    return Get_CycleState(SCONTEXT)->ActJumpDir;
}

static inline void Set_ActiveJumpDirection(__SCONTEXT_PAR, jump_dir_t* value)
{
    Get_CycleState(SCONTEXT)->ActJumpDir = value;
}

static inline jump_col_t* Get_ActiveJumpCollection(__SCONTEXT_PAR)
{
    return Get_CycleState(SCONTEXT)->ActJumpCol;
}

static inline void Set_ActiveJumpCollection(__SCONTEXT_PAR, jump_col_t* value)
{
    Get_CycleState(SCONTEXT)->ActJumpCol = value;
}

static inline jump_rule_t* Get_ActiveJumpRule(__SCONTEXT_PAR)
{
    return Get_CycleState(SCONTEXT)->ActJumpRule;
}

static inline void Set_ActiveJumpRule(__SCONTEXT_PAR, jump_rule_t* value)
{
    Get_CycleState(SCONTEXT)->ActJumpRule = value;
}

static inline cnt_col_t* Get_ActiveCounters(__SCONTEXT_PAR)
{
    return Get_CycleState(SCONTEXT)->ActCntCol;
}

static inline void Set_ActiveCounters(__SCONTEXT_PAR, cnt_col_t* value)
{
    Get_CycleState(SCONTEXT)->ActCntCol = value;
}

static inline env_state_t* Get_PathEnvironmentAt(__SCONTEXT_PAR, const byte_t id)
{
    return Get_CycleState(SCONTEXT)->ActPathEnvs[id]; 
}

static inline void Set_PathEnvironmentAt(__SCONTEXT_PAR, const byte_t id, env_state_t* value)
{
    Get_CycleState(SCONTEXT)->ActPathEnvs[id] = value;
}

static inline env_state_t* Get_ActiveWorkEnvironment(__SCONTEXT_PAR)
{
    return Get_CycleState(SCONTEXT)->ActWorkEnv;
}

static inline void Set_ActiveWorkEnvironment(__SCONTEXT_PAR, env_state_t* value)
{
    Get_CycleState(SCONTEXT)->ActWorkEnv = value;
}

static inline clu_state_t* Get_ActiveWorkCluster(__SCONTEXT_PAR)
{
    return Get_CycleState(SCONTEXT)->ActWorkClu;
}

static inline void Set_ActiveWorkCluster(__SCONTEXT_PAR, clu_state_t* value)
{
    Get_CycleState(SCONTEXT)->ActWorkClu = value;
}

static inline pair_table_t* Get_ActivePairTable(__SCONTEXT_PAR)
{
    return Get_CycleState(SCONTEXT)->ActPairTable;
}

static inline void Set_ActivePairTable(__SCONTEXT_PAR, pair_table_t* value)
{
    Get_CycleState(SCONTEXT)->ActPairTable = value;
}

static inline clu_table_t* Get_ActiveClusterTable(__SCONTEXT_PAR)
{
    return Get_CycleState(SCONTEXT)->ActCluTable;
}

static inline void Set_ActiveClusterTable(__SCONTEXT_PAR, clu_table_t* value)
{
    Get_CycleState(SCONTEXT)->ActCluTable = value;
}

/* Database model getter/setter */

static inline lat_info_t* Get_LatticeInformation(__SCONTEXT_PAR)
{
    return &Get_DatabaseModel(SCONTEXT)->LattInfo;
}

static inline job_info_t* Get_JobInformation(__SCONTEXT_PAR)
{
    return &Get_DatabaseModel(SCONTEXT)->JobInfo;
}

static inline str_model_t* Get_StructureModel(__SCONTEXT_PAR)
{
    return &Get_DatabaseModel(SCONTEXT)->Structure;
}

static inline eng_model_t* Get_EnergyModel(__SCONTEXT_PAR)
{
    return &Get_DatabaseModel(SCONTEXT)->Energy;
}

static inline tra_model_t* Get_TransitionModel(__SCONTEXT_PAR)
{
    return &Get_DatabaseModel(SCONTEXT)->Transition;
}

/* Main state getter/setter */

static inline buffer_t* Get_MainStateBuffer(__SCONTEXT_PAR)
{
    return &Get_SimulationState(SCONTEXT)->Buffer;
}

static inline hdr_state_t* Get_MainStateHeader(__SCONTEXT_PAR)
{
    return &Get_SimulationState(SCONTEXT)->Header;
}

static inline mta_state_t* Get_MainStateMetaInfo(__SCONTEXT_PAR)
{
    return &Get_SimulationState(SCONTEXT)->Meta;
}

static inline lat_state_t* Get_MainStateLattice(__SCONTEXT_PAR)
{
    return &Get_SimulationState(SCONTEXT)->Lattice;
}

static inline cnt_state_t* Get_MainStateCounters(__SCONTEXT_PAR)
{
    return &Get_SimulationState(SCONTEXT)->Counters;
}

static inline cnt_col_t* Get_MainStateCounterAt(__SCONTEXT_PAR, const byte_t id)
{
    return &Get_MainStateCounters(SCONTEXT)->Start[id];
}

static inline trc_state_t* Get_AbstractMovementTrackers(__SCONTEXT_PAR)
{
    return &Get_SimulationState(SCONTEXT)->GlobalTrackers;
}

static inline trc_state_t* Get_StaticMovementTrackers(__SCONTEXT_PAR)
{
    return &Get_SimulationState(SCONTEXT)->StaticTrackers;
}

static inline trc_state_t* Get_MobileMovementTrackers(__SCONTEXT_PAR)
{
    return &Get_SimulationState(SCONTEXT)->MobileTrackers;
}

static inline idx_state_t* Get_MobileTrackerIndexing(__SCONTEXT_PAR)
{
    return &Get_SimulationState(SCONTEXT)->MobileTrackerIdx;
}

static inline prb_state_t* Get_JumpProbabilityHistogram(__SCONTEXT_PAR)
{
    return &Get_SimulationState(SCONTEXT)->ProbStatMap;
}