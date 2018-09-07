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

#define SCONTEXT        simContext
#define JUMPPATH        SCONTEXT->CycleState.ActivePathEnvironments
#define SIMERROR        SCONTEXT->ErrorCode
#define __SCONTEXT_PAR  sim_context_t* restrict SCONTEXT

/* Context getter/setter */

static inline mc_state_t* getSimulationState(__SCONTEXT_PAR)
{
    return &SCONTEXT->MainState;
}

static inline db_model_t* getDatabaseModel(__SCONTEXT_PAR)
{
    return &SCONTEXT->DbModel;
}

static inline sim_model_t* getDynamicModel(__SCONTEXT_PAR)
{
    return &SCONTEXT->DynamicModel;
}

static inline jump_pool_t* getJumpSelectionPool(__SCONTEXT_PAR)
{
    return &SCONTEXT->SelectionPool;
}

static inline cycle_state_t* getCycleState(__SCONTEXT_PAR)
{
    return &SCONTEXT->CycleState;
}

static inline pcg32_random_t* getRandomNumberGen(__SCONTEXT_PAR)
{
    return &SCONTEXT->RandomNumberGenerator;
}

static inline plugin_col_t* getPluginCollection(__SCONTEXT_PAR)
{
    return &SCONTEXT->Plugins;
}

/* Simulation model getter/setter */

static inline file_info_t* getFileInformation(__SCONTEXT_PAR)
{
    return &getDynamicModel(SCONTEXT)->FileInfo;
}

static inline phys_val_t* getPhysicalFactors(__SCONTEXT_PAR)
{
    return &getDynamicModel(SCONTEXT)->PhysicalFactors;
}

static inline flp_buffer_t* getLatticeEnergyBuffer(__SCONTEXT_PAR)
{
    return &getDynamicModel(SCONTEXT)->LatticeEnergyBuffer;
}

static inline run_info_t* getRuntimeInformation(__SCONTEXT_PAR)
{
    return &getDynamicModel(SCONTEXT)->RuntimeInfo;
}

static inline env_lattice_t* getEnvironmentLattice(__SCONTEXT_PAR)
{
    return &getDynamicModel(SCONTEXT)->EnvironmentLattice;
}

static inline void setEnvironmentLattice(__SCONTEXT_PAR, env_lattice_t value)
{
    *getEnvironmentLattice(SCONTEXT) = value;
}

static inline env_state_t* getEnvironmentStateById(__SCONTEXT_PAR, const int32_t id)
{
    return &getEnvironmentLattice(SCONTEXT)->Start[id];
}

static inline env_state_t* getEnvironmentStateByVector4(__SCONTEXT_PAR, const vector4_t* restrict vector)
{
    return MDA_GET_4(*getEnvironmentLattice(SCONTEXT), vector->a, vector->b, vector->c, vector->d);
}

static inline int32_t* getLatticeBlockSizes(__SCONTEXT_PAR)
{
    return getEnvironmentLattice(SCONTEXT)->Header->Blocks;
}

/* Cycle state getter/setter */

static inline cycle_cnt_t* getMainCycleCounters(__SCONTEXT_PAR)
{
    return &getCycleState(SCONTEXT)->MainCounters;
}

static inline occode_t getPathStateCode(__SCONTEXT_PAR)
{
    return getCycleState(SCONTEXT)->ActiveStateCode;
}

static inline void setPathStateCode(__SCONTEXT_PAR, const occode_t code)
{
    getCycleState(SCONTEXT)->ActiveStateCode = code;
}

static inline roll_info_t* getJumpSelectionInfo(__SCONTEXT_PAR)
{
    return &getCycleState(SCONTEXT)->ActiveSelectionInfo;
}

static inline eng_info_t* getJumpEnergyInfo(__SCONTEXT_PAR)
{
    return &getCycleState(SCONTEXT)->ActiveEnergyInfo;
}

static inline env_backup_t* getEnvironmentBackup(__SCONTEXT_PAR)
{
    return &getCycleState(SCONTEXT)->ActiveEnvironmentBackup;
}

static inline jump_dir_t* getActiveJumpDirection(__SCONTEXT_PAR)
{
    return getCycleState(SCONTEXT)->ActiveJumpDirection;
}

static inline void setActiveJumpDirection(__SCONTEXT_PAR, jump_dir_t* value)
{
    getCycleState(SCONTEXT)->ActiveJumpDirection = value;
}

static inline jump_col_t* getActiveJumpCollection(__SCONTEXT_PAR)
{
    return getCycleState(SCONTEXT)->ActiveJumpCollection;
}

static inline void setActiveJumpCollection(__SCONTEXT_PAR, jump_col_t* value)
{
    getCycleState(SCONTEXT)->ActiveJumpCollection = value;
}

static inline jump_rule_t* getActiveJumpRule(__SCONTEXT_PAR)
{
    return getCycleState(SCONTEXT)->ActiveJumpRule;
}

static inline occode_t getActiveStateCode(__SCONTEXT_PAR)
{
    return getCycleState(SCONTEXT)->ActiveStateCode;
}

static inline void setActiveJumpRule(__SCONTEXT_PAR, jump_rule_t* value)
{
    getCycleState(SCONTEXT)->ActiveJumpRule = value;
}

static inline cnt_col_t* getActiveCounters(__SCONTEXT_PAR)
{
    return getCycleState(SCONTEXT)->ActiveCounterCollection;
}

static inline void setActiveCounters(__SCONTEXT_PAR, cnt_col_t* value)
{
    getCycleState(SCONTEXT)->ActiveCounterCollection = value;
}

static inline env_state_t* getPathEnvironmentAt(__SCONTEXT_PAR, const byte_t id)
{
    return getCycleState(SCONTEXT)->ActivePathEnvironments[id]; 
}

static inline void setPathEnvironmentAt(__SCONTEXT_PAR, const byte_t id, env_state_t* value)
{
    getCycleState(SCONTEXT)->ActivePathEnvironments[id] = value;
}

static inline env_state_t* getActiveWorkEnvironment(__SCONTEXT_PAR)
{
    return getCycleState(SCONTEXT)->WorkEnvironment;
}

static inline void setActiveWorkEnvironment(__SCONTEXT_PAR, env_state_t* value)
{
    getCycleState(SCONTEXT)->WorkEnvironment = value;
}

static inline clu_state_t* getActiveWorkCluster(__SCONTEXT_PAR)
{
    return getCycleState(SCONTEXT)->WorkCluster;
}

static inline void setActiveWorkCluster(__SCONTEXT_PAR, clu_state_t* value)
{
    getCycleState(SCONTEXT)->WorkCluster = value;
}

static inline pair_table_t* getActivePairTable(__SCONTEXT_PAR)
{
    return getCycleState(SCONTEXT)->WorkPairTable;
}

static inline void setActivePairTable(__SCONTEXT_PAR, pair_table_t* value)
{
    getCycleState(SCONTEXT)->WorkPairTable = value;
}

static inline clu_table_t* getActiveClusterTable(__SCONTEXT_PAR)
{
    return getCycleState(SCONTEXT)->WorkClusterTable;
}

static inline void setActiveClusterTable(__SCONTEXT_PAR, clu_table_t* value)
{
    getCycleState(SCONTEXT)->WorkClusterTable = value;
}

/* Database model getter/setter */

static inline lat_info_t* getLatticeInformation(__SCONTEXT_PAR)
{
    return &getDatabaseModel(SCONTEXT)->LattInfo;
}

static inline lattice_t* getDatabaseModelLattice(__SCONTEXT_PAR)
{
    return &getLatticeInformation(SCONTEXT)->Lattice;
}

static inline job_info_t* getJobInformation(__SCONTEXT_PAR)
{
    return &getDatabaseModel(SCONTEXT)->JobInfo;
}

static inline kmc_header_t* getJobHeaderAsKmc(__SCONTEXT_PAR)
{
    return (kmc_header_t*) getJobInformation(SCONTEXT)->JobHeader;
}

static inline mmc_header_t* getJobHeaderAsMmc(__SCONTEXT_PAR)
{
    return (mmc_header_t*) getJobInformation(SCONTEXT)->JobHeader;
}

static inline str_model_t* getStructureModel(__SCONTEXT_PAR)
{
    return &getDatabaseModel(SCONTEXT)->Structure;
}

static inline eng_model_t* getEnergyModel(__SCONTEXT_PAR)
{
    return &getDatabaseModel(SCONTEXT)->Energy;
}

static inline tra_model_t* getTransitionModel(__SCONTEXT_PAR)
{
    return &getDatabaseModel(SCONTEXT)->Transition;
}

static inline env_defs_t* getEnvironmentModels(__SCONTEXT_PAR)
{
    return &getStructureModel(SCONTEXT)->EnvironmentDefinitions;
}

static inline env_def_t* getEnvironmentModelById(__SCONTEXT_PAR, const int32_t id)
{
    return &getEnvironmentModels(SCONTEXT)->Start[id];
}

static inline vector4_t* getLatticeSizeVector(__SCONTEXT_PAR)
{
    return &getLatticeInformation(SCONTEXT)->SizeVector;
}

static inline jump_counts_t* getJumpDirectionsPerPositionTable(__SCONTEXT_PAR)
{
    return &getTransitionModel(SCONTEXT)->JumpCountTable;
}

static inline int32_t getJumpCountByPositionStatus(__SCONTEXT_PAR, const int32_t posId, const byte_t parId)
{
    return *MDA_GET_2(*getJumpDirectionsPerPositionTable(SCONTEXT), posId, parId);
}

static inline jump_assign_t* getJumpIdToPositionsAssignmentTable(__SCONTEXT_PAR)
{
    return &getTransitionModel(SCONTEXT)->JumpAssignTable;
}

static inline jump_dirs_t* getJumpDirections(__SCONTEXT_PAR)
{
    return &getTransitionModel(SCONTEXT)->JumpDirections;
}

static inline jump_dir_t* getJumpDirectionById(__SCONTEXT_PAR, const int32_t id)
{
    return &getJumpDirections(SCONTEXT)->Start[id];
}

static inline jump_cols_t* getJumpCollections(__SCONTEXT_PAR)
{
    return &getTransitionModel(SCONTEXT)->JumpCollections;
}

static inline jump_col_t* getJumpCollectionById(__SCONTEXT_PAR, const int32_t id)
{
    return &getJumpCollections(SCONTEXT)->Start[id];
}

static inline pair_tables_t* getPairEnergyTables(__SCONTEXT_PAR)
{
    return &getEnergyModel(SCONTEXT)->PairTables;
}

static inline pair_table_t* getPairEnergyTableById(__SCONTEXT_PAR, const int32_t id)
{
    return &getPairEnergyTables(SCONTEXT)->Start[id];
}

static inline clu_tables_t* getClusterEnergyTables(__SCONTEXT_PAR)
{
    return &getEnergyModel(SCONTEXT)->ClusterTables;
}

static inline clu_table_t* getClusterEnergyTableById(__SCONTEXT_PAR, const int32_t id)
{
    return &getClusterEnergyTables(SCONTEXT)->Start[id];
}

/* Main state getter/setter */

static inline buffer_t* getMainStateBuffer(__SCONTEXT_PAR)
{
    return &getSimulationState(SCONTEXT)->Buffer;
}

static inline void* getMainStateBufferAddress(__SCONTEXT_PAR, const int32_t offsetBytes)
{
    return &getMainStateBuffer(SCONTEXT)->Start[offsetBytes];
}

static inline hdr_state_t* getMainStateHeader(__SCONTEXT_PAR)
{
    return &getSimulationState(SCONTEXT)->Header;
}

static inline mta_state_t* getMainStateMetaInfo(__SCONTEXT_PAR)
{
    return &getSimulationState(SCONTEXT)->Meta;
}

static inline meta_info_t* getMainStateMetaData(__SCONTEXT_PAR)
{
    return getMainStateMetaInfo(SCONTEXT)->Data;
}

static inline lat_state_t* getMainStateLattice(__SCONTEXT_PAR)
{
    return &getSimulationState(SCONTEXT)->Lattice;
}

static inline byte_t getStateLatticeEntryById(__SCONTEXT_PAR, const int32_t id)
{
    return getMainStateLattice(SCONTEXT)->Start[id];
}

static inline cnt_state_t* getMainStateCounters(__SCONTEXT_PAR)
{
    return &getSimulationState(SCONTEXT)->Counters;
}

static inline cnt_col_t* getMainStateCounterById(__SCONTEXT_PAR, const byte_t id)
{
    return &getMainStateCounters(SCONTEXT)->Start[id];
}

static inline trc_state_t* getAbstractMovementTrackers(__SCONTEXT_PAR)
{
    return &getSimulationState(SCONTEXT)->GlobalTrackers;
}

static inline trc_state_t* getStaticMovementTrackers(__SCONTEXT_PAR)
{
    return &getSimulationState(SCONTEXT)->StaticTrackers;
}

static inline trc_state_t* getMobileMovementTrackers(__SCONTEXT_PAR)
{
    return &getSimulationState(SCONTEXT)->MobileTrackers;
}

static inline idx_state_t* getMobileTrackerIndexing(__SCONTEXT_PAR)
{
    return &getSimulationState(SCONTEXT)->MobileTrackerIndexing;
}

static inline prb_state_t* getJumpProbabilityMap(__SCONTEXT_PAR)
{
    return &getSimulationState(SCONTEXT)->ProbabilityTrackMap;
}

/* Jump selection pool getter/setter */

static inline id_redirect_t* getDirectionPoolIndexing(__SCONTEXT_PAR)
{
    return &getJumpSelectionPool(SCONTEXT)->NumOfDirectionsToPoolId;
}

static inline void setDirectionPoolIndexing(__SCONTEXT_PAR, id_redirect_t value)
{
    *getDirectionPoolIndexing(SCONTEXT) = value;
}

static inline int32_t getDirectionPoolIdByJumpCount(__SCONTEXT_PAR, const int32_t count)
{
    return getDirectionPoolIndexing(SCONTEXT)->Start[count];
}

static inline void setDirectionPoolIdByJumpCount(__SCONTEXT_PAR, const int32_t count, const int32_t value)
{
    getDirectionPoolIndexing(SCONTEXT)->Start[count] = value;
}

static inline dir_pools_t* getDirectionPools(__SCONTEXT_PAR)
{
    return &getJumpSelectionPool(SCONTEXT)->DirectionPools;
}

static inline void setDirectionPools(__SCONTEXT_PAR, dir_pools_t value)
{
    *getDirectionPools(SCONTEXT) = value;
}

static inline dir_pool_t* getDirectionPoolById(__SCONTEXT_PAR, const int32_t id)
{
    return &getDirectionPools(SCONTEXT)->Start[id];
}

static inline dir_pool_t* getDirectionPoolByJumpCount(__SCONTEXT_PAR, const int32_t count)
{
    return getDirectionPoolById(SCONTEXT, getDirectionPoolIdByJumpCount(SCONTEXT, count));
}

/* Command arguments getter/setter */

static inline cmd_args_t* getCommandArguments(__SCONTEXT_PAR)
{
    return &SCONTEXT->CommandArguments;
}

static inline char const * getCommandArgumentStringById(__SCONTEXT_PAR, const int32_t id)
{
    if (id >= getCommandArguments(SCONTEXT)->Count)
    {
        return NULL;
    }
    return getCommandArguments(SCONTEXT)->Values[id];
}

static inline void setCommandArguments(__SCONTEXT_PAR, const int32_t argc, char const * const * argv)
{
    *getCommandArguments(SCONTEXT) = (cmd_args_t) { argc, argv };
}

static inline void setProgramRunPath(__SCONTEXT_PAR, char const * value)
{
    getFileInformation(SCONTEXT)->ExecutionPath = value;
}

static inline void setDatabaseLoadString(__SCONTEXT_PAR, char const * value)
{
    getFileInformation(SCONTEXT)->DbQueryString = value;
}

static inline void setDatabasePath(__SCONTEXT_PAR, char const * value)
{
    getFileInformation(SCONTEXT)->DatabasePath = value;
}

static inline void setOutputPluginPath(__SCONTEXT_PAR, char const * value)
{
    getFileInformation(SCONTEXT)->OutputPluginPath = value;
}

static inline void setOutputPluginSymbol(__SCONTEXT_PAR, char const * value)
{
    getFileInformation(SCONTEXT)->OutputPluginSymbol = value;
}

static inline void setEnergyPluginPath(__SCONTEXT_PAR, char const * value)
{
    getFileInformation(SCONTEXT)->EnergyPluginPath = value;
}

static inline void setEnergyPluginSymbol(__SCONTEXT_PAR, char const * value)
{
    getFileInformation(SCONTEXT)->EnergyPluginSymbol = value;
}

/* Selection pool getter/setter */

static inline int32_t getEnvironmentPoolEntryById(dir_pool_t* restrict dirPool, const int32_t id)
{
    return dirPool->EnvironmentPool.Start[id];
}

static inline void setEnvironmentPoolEntryById(dir_pool_t* restrict dirPool, const int32_t id, const int32_t value)
{
    dirPool->EnvironmentPool.Start[id] = value;
}

/* Energy table getter/setter */

static inline double getPairEnergyTableEntry(const pair_table_t* restrict table, const byte_t parId0, const byte_t parId1)
{
    return *MDA_GET_2(table->EnergyTable, parId0, parId1);
}

static inline double getCluEnergyTableEntry(const clu_table_t* restrict table, const byte_t parId, const int32_t codeId)
{
    return *MDA_GET_2(table->EnergyTable, table->ParticleToTableId[parId], codeId);
}

/* Flag getter/setters */

static inline bitmask_t getMainStateFlags(__SCONTEXT_PAR)
{
    return getMainStateHeader(SCONTEXT)->Data->Flags;
}

static inline void setMainStateFlags(__SCONTEXT_PAR, const bitmask_t flags)
{
    FLG_SET(getMainStateHeader(SCONTEXT)->Data->Flags, flags);
}

static inline void UnsetMainStateFlags(__SCONTEXT_PAR, const bitmask_t flags)
{
    FLG_UNSET(getMainStateHeader(SCONTEXT)->Data->Flags, flags);
}

static inline bitmask_t getJobInformationFlags(__SCONTEXT_PAR)
{
    return getJobInformation(SCONTEXT)->JobFlags;
}

static inline bitmask_t getJobHeaderFlagsKmc(__SCONTEXT_PAR)
{
    return getJobHeaderAsKmc(SCONTEXT)->JobFlags;
}

static inline bitmask_t getJobHeaderFlagsMmc(__SCONTEXT_PAR)
{
    return getJobHeaderAsKmc(SCONTEXT)->JobFlags;
}

/* Environment getter/setter */

static inline int32_t getEnvironmentPairDefCount(env_state_t* restrict envState)
{
    return envState->EnvironmentDefinition->PairDefinitions.Count;
}

static inline pair_def_t* getEnvironmentPairDefById(env_state_t* restrict envState, const int32_t id)
{
    return &envState->EnvironmentDefinition->PairDefinitions.Start[id];
}

static inline clu_def_t* getEnvironmentCluDefById(env_state_t* restrict envState, const int32_t id)
{
    return &envState->EnvironmentDefinition->ClusterDefinitions.Start[id];
}

static inline clu_state_t* getEnvironmentCluStateById(env_state_t* restrict envState, const byte_t id)
{
    return &envState->ClusterStates.Start[id];
}

/* Active delta object getter/setter */

static inline double* getActiveStateEnergyById(__SCONTEXT_PAR, const byte_t id)
{
    return &getActiveWorkEnvironment(SCONTEXT)->EnergyStates.Start[id];
}

static inline byte_t getActiveParticleUpdateIdAt(__SCONTEXT_PAR, const byte_t id)
{
    return getActiveWorkEnvironment(SCONTEXT)->EnvironmentDefinition->UpdateParticleIds[id];
}

static inline env_link_t* getEnvLinkByJumpLink(__SCONTEXT_PAR, const jump_link_t* restrict link)
{
    return &JUMPPATH[link->PathId]->EnvironmentLinks.Start[link->LinkId];
}

static inline double* getPathStateEnergyByIds(__SCONTEXT_PAR, const byte_t pathId, const byte_t parId)
{
    return &JUMPPATH[pathId]->EnergyStates.Start[parId];
}

static inline double* getEnvStateEnergyBackupById(__SCONTEXT_PAR, const byte_t pathId)
{
    return &getCycleState(SCONTEXT)->ActiveEnvironmentBackup.PathEnergies[pathId];
}
