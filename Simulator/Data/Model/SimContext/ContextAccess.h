//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	ContextAccess.h        		//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Get/Set for sim context     //
//////////////////////////////////////////

#pragma once
#include "Framework/Math/Random/PcgRandom.h"
#include "Framework/Basic/BaseTypes/BaseTypes.h"
#include "Framework/Math/Types/Vector.h"
#include "Simulator/Data/Model/SimContext/SimContext.h"

/* Context access defines */

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

static inline jump_pool_t* Get_JumpSelectionPool(__SCONTEXT_PAR)
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
    return &Get_DynamicModel(SCONTEXT)->LatticeEnergyBuffer;
}

static inline run_info_t* Get_RuntimeInformation(__SCONTEXT_PAR)
{
    return &Get_DynamicModel(SCONTEXT)->RunInfo;
}

static inline env_lattice_t* Get_EnvironmentLattice(__SCONTEXT_PAR)
{
    return &Get_DynamicModel(SCONTEXT)->EnvLattice;
}

static inline void Set_EnvironmentLattice(__SCONTEXT_PAR, env_lattice_t value)
{
    *Get_EnvironmentLattice(SCONTEXT) = value;
}

static inline env_state_t* Get_EnvironmentStateById(__SCONTEXT_PAR, const int32_t id)
{
    return &Get_DynamicModel(SCONTEXT)->EnvLattice.Start[id];
}

static inline env_state_t* Get_EnvironmentStateByVector4(__SCONTEXT_PAR, const vector4_t* restrict vector)
{
    return MDA_GET_4(*Get_EnvironmentLattice(SCONTEXT), vector->a, vector->b, vector->c, vector->d);
}

static inline int32_t* Get_LatticeBlockSizes(__SCONTEXT_PAR)
{
    return Get_EnvironmentLattice(SCONTEXT)->Header->Blocks;
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

static inline occode_t Get_ActiveStateCode(__SCONTEXT_PAR)
{
    return Get_CycleState(SCONTEXT)->ActStateCode;
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

static inline lattice_t* Get_DatabaseModelLattice(__SCONTEXT_PAR)
{
    return &Get_LatticeInformation(SCONTEXT)->Lattice;
}

static inline job_info_t* Get_JobInformation(__SCONTEXT_PAR)
{
    return &Get_DatabaseModel(SCONTEXT)->JobInfo;
}

static inline kmc_header_t* Get_JobHeaderAsKmc(__SCONTEXT_PAR)
{
    return Get_JobInformation(SCONTEXT)->JobHeader;
}

static inline mmc_header_t* Get_JobHeaderAsMmc(__SCONTEXT_PAR)
{
    return Get_JobInformation(SCONTEXT)->JobHeader;
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

static inline env_defs_t* Get_EnvironmentModels(__SCONTEXT_PAR)
{
    return &Get_StructureModel(SCONTEXT)->EnvDefs;
}

static inline env_def_t* Get_EnvironmentModelById(__SCONTEXT_PAR, const int32_t id)
{
    return &Get_EnvironmentModels(SCONTEXT)->Start[id];
}

static inline vector4_t* Get_LatticeSizeVector(__SCONTEXT_PAR)
{
    return &Get_LatticeInformation(SCONTEXT)->SizeVec;
}

static inline jump_counts_t* Get_JumpDirectionsPerPositionTable(__SCONTEXT_PAR)
{
    return &Get_TransitionModel(SCONTEXT)->JumpCountTable;
}

static inline int32_t Get_JumpCountByPositionStatus(__SCONTEXT_PAR, const int32_t posId, const byte_t parId)
{
    return *MDA_GET_2(*Get_JumpDirectionsPerPositionTable(SCONTEXT), posId, parId);
}

static inline jump_assign_t* Get_JumpIdToPositionsAssignmentTable(__SCONTEXT_PAR)
{
    return &Get_TransitionModel(SCONTEXT)->JumpAssignTable;
}

static inline jump_dirs_t* Get_JumpDirections(__SCONTEXT_PAR)
{
    return &Get_TransitionModel(SCONTEXT)->JumpDirs;
}

static inline jump_dir_t* Get_JumpDirectionById(__SCONTEXT_PAR, const int32_t id)
{
    return &Get_JumpDirections(SCONTEXT)->Start[id];
}

static inline jump_cols_t* Get_JumpCollections(__SCONTEXT_PAR)
{
    return &Get_TransitionModel(SCONTEXT)->JumpCols;
}

static inline jump_col_t* Get_JumpCollectionById(__SCONTEXT_PAR, const int32_t id)
{
    return &Get_JumpCollections(SCONTEXT)->Start[id];
}

static inline pair_tables_t* Get_PairEnergyTables(__SCONTEXT_PAR)
{
    return &Get_EnergyModel(SCONTEXT)->PairTables;
}

static inline pair_table_t* Get_PairEnergyTableById(__SCONTEXT_PAR, const int32_t id)
{
    return &Get_PairEnergyTables(SCONTEXT)->Start[id];
}

static inline clu_tables_t* Get_ClusterEnergyTables(__SCONTEXT_PAR)
{
    return &Get_EnergyModel(SCONTEXT)->CluTables;
}

static inline clu_table_t* Get_ClusterEnergyTableById(__SCONTEXT_PAR, const int32_t id)
{
    return &Get_ClusterEnergyTables(SCONTEXT)->Start[id];
}

/* Main state getter/setter */

static inline buffer_t* Get_MainStateBuffer(__SCONTEXT_PAR)
{
    return &Get_SimulationState(SCONTEXT)->Buffer;
}

static inline void* Get_MainStateBufferAddress(__SCONTEXT_PAR, const int32_t offsetBytes)
{
    return &Get_MainStateBuffer(SCONTEXT)->Start[offsetBytes];
}

static inline hdr_state_t* Get_MainStateHeader(__SCONTEXT_PAR)
{
    return &Get_SimulationState(SCONTEXT)->Header;
}

static inline mta_state_t* Get_MainStateMetaInfo(__SCONTEXT_PAR)
{
    return &Get_SimulationState(SCONTEXT)->Meta;
}

static inline meta_info_t* Get_MainStateMetaData(__SCONTEXT_PAR)
{
    return Get_MainStateMetaInfo(SCONTEXT)->Data;
}

static inline lat_state_t* Get_MainStateLattice(__SCONTEXT_PAR)
{
    return &Get_SimulationState(SCONTEXT)->Lattice;
}

static inline byte_t Get_StateLatticeEntryById(__SCONTEXT_PAR, const int32_t id)
{
    return Get_MainStateLattice(SCONTEXT)->Start[id];
}

static inline cnt_state_t* Get_MainStateCounters(__SCONTEXT_PAR)
{
    return &Get_SimulationState(SCONTEXT)->Counters;
}

static inline cnt_col_t* Get_MainStateCounterById(__SCONTEXT_PAR, const byte_t id)
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

static inline prb_state_t* Get_JumpProbabilityMap(__SCONTEXT_PAR)
{
    return &Get_SimulationState(SCONTEXT)->ProbStatMap;
}

/* Jump selection pool getter/setter */

static inline id_redirect_t* Get_DirectionPoolIndexing(__SCONTEXT_PAR)
{
    return &Get_JumpSelectionPool(SCONTEXT)->DirCountToPoolId;
}

static inline void Set_DirectionPoolIndexing(__SCONTEXT_PAR, id_redirect_t value)
{
    *Get_DirectionPoolIndexing(SCONTEXT) = value;
}

static inline int32_t Get_DirectionPoolIdByJumpCount(__SCONTEXT_PAR, const int32_t count)
{
    return Get_DirectionPoolIndexing(SCONTEXT)->Start[count];
}

static inline void Set_DirectionPoolIdByJumpCount(__SCONTEXT_PAR, const int32_t count, const int32_t value)
{
    Get_DirectionPoolIndexing(SCONTEXT)->Start[count] = value;
}

static inline dir_pools_t* Get_DirectionPools(__SCONTEXT_PAR)
{
    return &Get_JumpSelectionPool(SCONTEXT)->DirPools;
}

static inline void Set_DirectionPools(__SCONTEXT_PAR, dir_pools_t value)
{
    *Get_DirectionPools(SCONTEXT) = value;
}

static inline dir_pool_t* Get_DirectionPoolById(__SCONTEXT_PAR, const int32_t id)
{
    return &Get_DirectionPools(SCONTEXT)->Start[id];
}

static inline dir_pool_t* Get_DirectionPoolByJumpCount(__SCONTEXT_PAR, const int32_t count)
{
    return Get_DirectionPoolById(SCONTEXT, Get_DirectionPoolIdByJumpCount(SCONTEXT, count));
}

/* Command arguments getter/setter */

static inline cmd_args_t* Get_CommandArguments(__SCONTEXT_PAR)
{
    return &SCONTEXT->RunArguments;
}

static inline char const * Get_CommandArgumentStringById(__SCONTEXT_PAR, const int32_t id)
{
    if (id >= Get_CommandArguments(SCONTEXT)->Count)
    {
        return NULL;
    }
    return Get_CommandArguments(SCONTEXT)->Values[id];
}

static inline void Set_CommandArguments(__SCONTEXT_PAR, const int32_t argc, char const * const * argv)
{
    *Get_CommandArguments(SCONTEXT) = (cmd_args_t) { argc, argv };
}

static inline void Set_ProgramRunPath(__SCONTEXT_PAR, char const * value)
{
    Get_FileInformation(SCONTEXT)->RunPath = value;
}

static inline void Set_DatabaseLoadString(__SCONTEXT_PAR, char const * value)
{
    Get_FileInformation(SCONTEXT)->DbLoadString = value;
}

static inline void Set_DatabasePath(__SCONTEXT_PAR, char const * value)
{
    Get_FileInformation(SCONTEXT)->DatabasePath = value;
}

static inline void Set_OutputPluginPath(__SCONTEXT_PAR, char const * value)
{
    Get_FileInformation(SCONTEXT)->OutputPluginPath = value;
}

static inline void Set_OutputPluginSymbol(__SCONTEXT_PAR, char const * value)
{
    Get_FileInformation(SCONTEXT)->OutputPluginSymbol = value;
}

static inline void Set_EnergyPluginPath(__SCONTEXT_PAR, char const * value)
{
    Get_FileInformation(SCONTEXT)->EnergyPluginPath = value;
}

static inline void Set_EnergyPluginSymbol(__SCONTEXT_PAR, char const * value)
{
    Get_FileInformation(SCONTEXT)->EnergyPluginSymbol = value;
}

/* Selection pool getter/setter */

static inline int32_t Get_EnvironmentPoolEntryById(dir_pool_t* restrict dirPool, const int32_t id)
{
    return dirPool->EnvPool.Start[id];
}

static inline void Set_EnvironmentPoolEntryById(dir_pool_t* restrict dirPool, const int32_t id, const int32_t value)
{
    dirPool->EnvPool.Start[id] = value;
}

/* Energy table getter/setter */

static inline double Get_PairEnergyTableEntry(const pair_table_t* restrict table, const byte_t parId0, const byte_t parId1)
{
    return *MDA_GET_2(table->EngTable, parId0, parId1);
}

static inline double Get_CluEnergyTableEntry(const clu_table_t* restrict table, const byte_t parId, const int32_t codeId)
{
    return *MDA_GET_2(table->EngTable, table->ParToTableId[parId], codeId);
}

/* Flag getter/setters */

static inline bitmask_t Get_MainStateFlags(__SCONTEXT_PAR)
{
    return Get_MainStateHeader(SCONTEXT)->Data->Flags;
}

static inline void Set_MainStateFlags(__SCONTEXT_PAR, const bitmask_t flags)
{
    FLG_SET(Get_MainStateHeader(SCONTEXT)->Data->Flags, flags);
}

static inline void UnSet_MainStateFlags(__SCONTEXT_PAR, const bitmask_t flags)
{
    FLG_UNSET(Get_MainStateHeader(SCONTEXT)->Data->Flags, flags);
}

static inline bitmask_t Get_JobInformationFlags(__SCONTEXT_PAR)
{
    return Get_JobInformation(SCONTEXT)->JobFlg;
}

static inline bitmask_t Get_JobHeaderFlagsKmc(__SCONTEXT_PAR)
{
    return Get_JobHeaderAsKmc(SCONTEXT)->JobFlg;
}

static inline bitmask_t Get_JobHeaderFlagsMmc(__SCONTEXT_PAR)
{
    return Get_JobHeaderAsKmc(SCONTEXT)->JobFlg;
}

/* Environment getter/setter */

static inline int32_t Get_EnvironmentPairDefCount(env_state_t* restrict envState)
{
    return envState->EnvDef->PairDefs.Count;
}

static inline pair_def_t* Get_EnvironmentPairDefById(env_state_t* restrict envState, const int32_t id)
{
    return &envState->EnvDef->PairDefs.Start[id];
}

static inline clu_def_t* Get_EnvironmentCluDefById(env_state_t* restrict envState, const int32_t id)
{
    return &envState->EnvDef->CluDefs.Start[id];
}

static inline clu_state_t* Get_EnvironmentCluStateById(env_state_t* restrict envState, const byte_t id)
{
    return &envState->ClusterStates.Start[id];
}

/* Active delta object getter/setter */

static inline double* Get_ActiveStateEnergyById(__SCONTEXT_PAR, const byte_t id)
{
    return &Get_ActiveWorkEnvironment(SCONTEXT)->EnergyStates.Start[id];
}

static inline byte_t Get_ActiveParticleUpdateIdAt(__SCONTEXT_PAR, const byte_t id)
{
    return Get_ActiveWorkEnvironment(SCONTEXT)->EnvDef->UptParIds[id];
}

static inline env_link_t* Get_EnvLinkByJumpLink(__SCONTEXT_PAR, const jump_link_t* restrict link)
{
    return &JUMPPATH[link->PathId]->EnvLinks.Start[link->LinkId];
}

static inline double* Get_PathStateEnergyByIds(__SCONTEXT_PAR, const byte_t pathId, const byte_t parId)
{
    return &JUMPPATH[pathId]->EnergyStates.Start[parId];
}

static inline double* Get_EnvStateEnergyBackupById(__SCONTEXT_PAR, const byte_t pathId)
{
    return &Get_CycleState(SCONTEXT)->ActEnvBackup.PathEnergies[pathId];
}
