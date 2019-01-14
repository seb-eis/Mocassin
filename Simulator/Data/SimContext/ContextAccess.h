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

#include <stdint.h>
#include "Framework/Math/Random/PcgRandom.h"
#include "Framework/Basic/BaseTypes/BaseTypes.h"
#include "Framework/Math/Types/Vector.h"
#include "SimContext.h"

/* Context access defines */

#define SCONTEXT        simContext
#define JUMPPATH        SCONTEXT->CycleState.ActivePathEnvironments
#define SIMERROR        SCONTEXT->ErrorCode
#define __SCONTEXT_PAR  SimulationContext_t* restrict SCONTEXT

/* Context getter/setter */

// Get the main simulation state from the context
static inline SimulationState_t* getSimulationState(__SCONTEXT_PAR)
{
    return &SCONTEXT->MainState;
}

// Get the database model from the context
static inline DbModel_t* getDatabaseModel(__SCONTEXT_PAR)
{
    return &SCONTEXT->DbModel;
}

// Get the dynamic simulation model from the context
static inline DynamicModel_t* getDynamicModel(__SCONTEXT_PAR)
{
    return &SCONTEXT->DynamicModel;
}

// Get the jump selection pool from the context
static inline JumpSelectionPool_t* getJumpSelectionPool(__SCONTEXT_PAR)
{
    return &SCONTEXT->SelectionPool;
}

// Get the cycle state from the context
static inline CycleState_t* getCycleState(__SCONTEXT_PAR)
{
    return &SCONTEXT->CycleState;
}

// Get the random number generator from the context
static inline Pcg32_t* getRandomNumberGenerator(__SCONTEXT_PAR)
{
    return &SCONTEXT->Rng;
}

// Get the simulation plugins from the context
static inline SimulationPlugins_t* getPluginCollection(__SCONTEXT_PAR)
{
    return &SCONTEXT->Plugins;
}

// Get the jump status array that contains information of all existing jumps in the lattice
static inline JumpStatusArray_t* getJumpStatusArray(__SCONTEXT_PAR)
{
    return &getDynamicModel(SCONTEXT)->JumpStatusArray;
}

// Get a jump status from the context by a 4D vector [A,B,C,JumpDirId]
static inline JumpStatus_t* getJumpStatusByVector4(__SCONTEXT_PAR, const Vector4_t*restrict vector)
{
    return &array_Get(*getJumpStatusArray(SCONTEXT), vector->a,vector->b,vector->c,vector->d);
}

/* Simulation model getter/setter */

// Get the file information from the context
static inline FileInfo_t* getFileInformation(__SCONTEXT_PAR)
{
    return &getDynamicModel(SCONTEXT)->FileInfo;
}

// Get the physical factor collection from the context
static inline PhysicalInfo_t* getPhysicalFactors(__SCONTEXT_PAR)
{
    return &getDynamicModel(SCONTEXT)->PhysicalFactors;
}

// Get the lattice energy buffer from the context
static inline Flp64Buffer_t* getLatticeEnergyBuffer(__SCONTEXT_PAR)
{
    return &getDynamicModel(SCONTEXT)->LatticeEnergyBuffer;
}

// Get the simulation runtime information from the context
static inline SimulationRunInfo_t* getRuntimeInformation(__SCONTEXT_PAR)
{
    return &getDynamicModel(SCONTEXT)->RuntimeInfo;
}

// Get the environment lattice from the context
static inline EnvironmentLattice_t* getEnvironmentLattice(__SCONTEXT_PAR)
{
    return &getDynamicModel(SCONTEXT)->EnvironmentLattice;
}

// Set the environment lattice on the context
static inline void setEnvironmentLattice(__SCONTEXT_PAR, EnvironmentLattice_t value)
{
    *getEnvironmentLattice(SCONTEXT) = value;
}

// Get an environment state by its linearized id from the context
static inline EnvironmentState_t* getEnvironmentStateById(__SCONTEXT_PAR, const int32_t id)
{
    debug_assert(!span_IndexIsOutOfRange(*getEnvironmentLattice(SCONTEXT), id));
    return &span_Get(*getEnvironmentLattice(SCONTEXT), id);
}

// Get an environment state by (A,B,C,D) coordinate access from the context
static inline EnvironmentState_t* getEnvironmentStateByIds(__SCONTEXT_PAR, const int32_t a, const int32_t b, const int32_t c, const int32_t d)
{
    return &array_Get(*getEnvironmentLattice(SCONTEXT), a, b, c, d);
}

// Get an environment state by a 4D vector access from the context
static inline EnvironmentState_t* getEnvironmentStateByVector4(__SCONTEXT_PAR, const Vector4_t* restrict vector)
{
    return getEnvironmentStateByIds(SCONTEXT, vector->a, vector->b, vector->c, vector->d);
}

// Get the lattice block sizes from the context
static inline int32_t* getLatticeBlockSizes(__SCONTEXT_PAR)
{
    return getEnvironmentLattice(SCONTEXT)->Header->Blocks;
}

// Get the static tracker mapping table from the context
static inline TrackerMappingTable_t* getStaticTrackerMappingTable(__SCONTEXT_PAR)
{
    return &getDatabaseModel(SCONTEXT)->TransitionModel.StaticTrackerMappingTable;
}

// Get the global tracker mapping table from the context
static inline TrackerMappingTable_t* getGlobalTrackerMappingTable(__SCONTEXT_PAR)
{
    return &getDatabaseModel(SCONTEXT)->TransitionModel.GlobalTrackerMappingTable;
}

/* Cycle state getter/setter */

// Get the counters of the simulation cycle state
static inline CycleCounterState_t* getMainCycleCounters(__SCONTEXT_PAR)
{
    return &getCycleState(SCONTEXT)->MainCounters;
}

// Get the active state code for the jump path
static inline OccCode_t getPathStateCode(__SCONTEXT_PAR)
{
    return getCycleState(SCONTEXT)->ActiveStateCode;
}

// Set the active state code for the jump path
static inline void setPathStateCode(__SCONTEXT_PAR, const OccCode_t code)
{
    getCycleState(SCONTEXT)->ActiveStateCode = code;
}

// Get the active jump selection info
static inline JumpSelectionInfo_t* getJumpSelectionInfo(__SCONTEXT_PAR)
{
    return &getCycleState(SCONTEXT)->ActiveSelectionInfo;
}

// Get the active jump energy info
static inline JumpEnergyInfo_t* getJumpEnergyInfo(__SCONTEXT_PAR)
{
    return &getCycleState(SCONTEXT)->ActiveEnergyInfo;
}

// Get the active environment backup
static inline EnvironmentBackup_t* getEnvironmentBackup(__SCONTEXT_PAR)
{
    return &getCycleState(SCONTEXT)->ActiveEnvironmentBackup;
}

// Get the currently active jump direction
static inline JumpDirection_t* getActiveJumpDirection(__SCONTEXT_PAR)
{
    return getCycleState(SCONTEXT)->ActiveJumpDirection;
}

// Set the currently active jump direction
static inline void setActiveJumpDirection(__SCONTEXT_PAR, JumpDirection_t* value)
{
    getCycleState(SCONTEXT)->ActiveJumpDirection = value;
}

// Get the currently active jump collection
static inline JumpCollection_t* getActiveJumpCollection(__SCONTEXT_PAR)
{
    return getCycleState(SCONTEXT)->ActiveJumpCollection;
}

// Set the currently active jump collection
static inline void setActiveJumpCollection(__SCONTEXT_PAR, JumpCollection_t* value)
{
    getCycleState(SCONTEXT)->ActiveJumpCollection = value;
}

// Get the currently active jump rule
static inline JumpRule_t* getActiveJumpRule(__SCONTEXT_PAR)
{
    return getCycleState(SCONTEXT)->ActiveJumpRule;
}

// Set the currently active jump rule
static inline void setActiveJumpRule(__SCONTEXT_PAR, JumpRule_t* value)
{
    getCycleState(SCONTEXT)->ActiveJumpRule = value;
}

// Get the currently active state counter collection
static inline StateCounterCollection_t* getActiveCounters(__SCONTEXT_PAR)
{
    return getCycleState(SCONTEXT)->ActiveCounterCollection;
}

// Set the currently active state counter collection
static inline void setActiveCounters(__SCONTEXT_PAR, StateCounterCollection_t* value)
{
    getCycleState(SCONTEXT)->ActiveCounterCollection = value;
}

// Get the path environment by a path id value
static inline EnvironmentState_t* getPathEnvironmentAt(__SCONTEXT_PAR, const byte_t pathId)
{
    debug_assert((pathId > 7) || (pathId < 0));
    return getCycleState(SCONTEXT)->ActivePathEnvironments[pathId];
}

// Set the path environment at the given id
static inline void setPathEnvironmentAt(__SCONTEXT_PAR, const byte_t pathId, EnvironmentState_t* value)
{
    debug_assert((pathId > 7) || (pathId < 0));
    getCycleState(SCONTEXT)->ActivePathEnvironments[pathId] = value;
}

// Get the currently active work environment
static inline EnvironmentState_t* getActiveWorkEnvironment(__SCONTEXT_PAR)
{
    return getCycleState(SCONTEXT)->WorkEnvironment;
}

// Set the currently active work environment
static inline void setActiveWorkEnvironment(__SCONTEXT_PAR, EnvironmentState_t* value)
{
    getCycleState(SCONTEXT)->WorkEnvironment = value;
}

// Get the currently active work cluster
static inline ClusterState_t* getActiveWorkCluster(__SCONTEXT_PAR)
{
    return getCycleState(SCONTEXT)->WorkCluster;
}

// Set the currently active work cluster
static inline void setActiveWorkCluster(__SCONTEXT_PAR, ClusterState_t* value)
{
    getCycleState(SCONTEXT)->WorkCluster = value;
}

// Get the currently active pair energy table
static inline PairTable_t* getActivePairTable(__SCONTEXT_PAR)
{
    return getCycleState(SCONTEXT)->WorkPairTable;
}

// Set the currently active pair energy table
static inline void setActivePairTable(__SCONTEXT_PAR, PairTable_t* value)
{
    getCycleState(SCONTEXT)->WorkPairTable = value;
}

// Get the currently active cluster energy table
static inline ClusterTable_t* getActiveClusterTable(__SCONTEXT_PAR)
{
    return getCycleState(SCONTEXT)->WorkClusterTable;
}

// Setthe currently active cluster energy table
static inline void setActiveClusterTable(__SCONTEXT_PAR, ClusterTable_t* value)
{
    getCycleState(SCONTEXT)->WorkClusterTable = value;
}

/* Database model getter/setter */

// Get the lattice model from the database model
static inline LatticeModel_t* getDbLatticeModel(__SCONTEXT_PAR)
{
    return &getDatabaseModel(SCONTEXT)->LatticeModel;
}

// Get the particle lattice from the database model data
static inline Lattice_t* getDbModelLattice(__SCONTEXT_PAR)
{
    return &getDbLatticeModel(SCONTEXT)->Lattice;
}

// Get the job info from the database model data
static inline JobInfo_t* getDbModelJobInfo(__SCONTEXT_PAR)
{
    return &getDatabaseModel(SCONTEXT)->JobModel.JobInfo;
}

// Get the job header from the database model as a KMC header
static inline KmcHeader_t* getDbModelJobHeaderAsKMC(__SCONTEXT_PAR)
{
    return (KmcHeader_t*) getDbModelJobInfo(SCONTEXT)->JobHeader;
}

// Get the job header from the database model as an MMC header
static inline MmcHeader_t* getDbModelJobHeaderAsMMC(__SCONTEXT_PAR)
{
    return (MmcHeader_t*) getDbModelJobInfo(SCONTEXT)->JobHeader;
}

// Get the structure model from the database model
static inline StructureModel_t* getDbStructureModel(__SCONTEXT_PAR)
{
    return &getDatabaseModel(SCONTEXT)->StructureModel;
}

// Get the energy model from the database model
static inline EnergyModel_t* getDbEnergyModel(__SCONTEXT_PAR)
{
    return &getDatabaseModel(SCONTEXT)->EnergyModel;
}

// Get the transition model from the database model
static inline TransitionModel_t* getDbTransitionModel(__SCONTEXT_PAR)
{
    return &getDatabaseModel(SCONTEXT)->TransitionModel;
}

// Get the environment models from the database model data
static inline EnvironmentDefinitions_t* getEnvironmentModels(__SCONTEXT_PAR)
{
    return &getDbStructureModel(SCONTEXT)->EnvironmentDefinitions;
}

// Get the environment model from the database model data at the given index
static inline EnvironmentDefinition_t* getEnvironmentModelAt(__SCONTEXT_PAR, const int32_t id)
{
    debug_assert(!span_IndexIsOutOfRange(*getEnvironmentModels(SCONTEXT), id));
    return &span_Get(*getEnvironmentModels(SCONTEXT), id);
}

// Get the lattice size vector from the database model data
static inline Vector4_t* getLatticeSizeVector(__SCONTEXT_PAR)
{
    return &getDbLatticeModel(SCONTEXT)->SizeVector;
}

// Get the jump count mapping that assigns each [positionId][particleId] combination its jump count
static inline JumpCountTable_t* getJumpCountMapping(__SCONTEXT_PAR)
{
    return &getDbTransitionModel(SCONTEXT)->JumpCountMappingTable;
}

// Get the jump count for the passed [positionId][particleId] combination
static inline int32_t getJumpCountAt(__SCONTEXT_PAR, const int32_t positionId, const byte_t particleId)
{
    return array_Get(*getJumpCountMapping(SCONTEXT), positionId, particleId);
}

// Get the jump direction mapping that assigns each [positionId][particleId][relJumpId] its valid [jumpDirectionId]
static inline JumpMappingTable_t* getJumpDirectionMapping(__SCONTEXT_PAR)
{
    return &getDbTransitionModel(SCONTEXT)->JumpDirectionMappingTable;
}

// Get all jump directions from the database model data
static inline JumpDirections_t* getJumpDirections(__SCONTEXT_PAR)
{
    return &getDbTransitionModel(SCONTEXT)->JumpDirections;
}

// Get the jump direction at the specified [jumpDirectionId]
static inline JumpDirection_t* getJumpDirectionAt(__SCONTEXT_PAR, const int32_t jumpDirectionId)
{
    debug_assert(!span_IndexIsOutOfRange(*getJumpDirections(SCONTEXT), jumpDirectionId));
    return &getJumpDirections(SCONTEXT)->Begin[jumpDirectionId];
}

// Get all jump collections from the database model data
static inline JumpCollections_t* getJumpCollections(__SCONTEXT_PAR)
{
    return &getDbTransitionModel(SCONTEXT)->JumpCollections;
}

// Get the jump collection at the specified [jumpCollectionId]
static inline JumpCollection_t* getJumpCollectionAt(__SCONTEXT_PAR, const int32_t jumpCollectionId)
{
    debug_assert(!span_IndexIsOutOfRange(*getJumpCollections(SCONTEXT), jumpCollectionId));
    return &span_Get(*getJumpCollections(SCONTEXT), jumpCollectionId);
}

// Get all pair energy tables from the database model data
static inline PairTables_t* getPairEnergyTables(__SCONTEXT_PAR)
{
    return &getDbEnergyModel(SCONTEXT)->PairTables;
}

// Get the pair energy table at the specified [pairTableId]
static inline PairTable_t* getPairEnergyTableAt(__SCONTEXT_PAR, const int32_t pairTableId)
{
    debug_assert(!span_IndexIsOutOfRange(*getPairEnergyTables(SCONTEXT), pairTableId));
    return &span_Get(*getPairEnergyTables(SCONTEXT), pairTableId);
}

// Get all cluster energy tables from the database model data
static inline ClusterTables_t* getClusterEnergyTables(__SCONTEXT_PAR)
{
    return &getDbEnergyModel(SCONTEXT)->ClusterTables;
}

// Get the cluster energy table at the specified [clusterTableId]
static inline ClusterTable_t* getClusterEnergyTableAt(__SCONTEXT_PAR, const int32_t clusterTableId)
{
    debug_assert(!span_IndexIsOutOfRange(*getClusterEnergyTables(SCONTEXT), clusterTableId));
    return &span_Get(*getClusterEnergyTables(SCONTEXT), clusterTableId);
}

/* Main state getter/setter */

// Get the buffer access to the main state binary
static inline Buffer_t* getMainStateBuffer(__SCONTEXT_PAR)
{
    return &getSimulationState(SCONTEXT)->Buffer;
}

// Get a pointer to the main state buffer at the given offset byte
static inline void* getMainStateBufferAddress(__SCONTEXT_PAR, const int32_t offsetBytes)
{
    debug_assert(!span_IndexIsOutOfRange(*getMainStateBuffer(SCONTEXT), offsetBytes));
    return &span_Get(*getMainStateBuffer(SCONTEXT), offsetBytes);
}

// Get the main state header
static inline StateHeader_t* getMainStateHeader(__SCONTEXT_PAR)
{
    return &getSimulationState(SCONTEXT)->Header;
}

// Get the main state meta information
static inline StateMetaInfo_t* getMainStateMetaInfo(__SCONTEXT_PAR)
{
    return &getSimulationState(SCONTEXT)->Meta;
}

// Get the main state meta information data
static inline StateMetaData_t* getMainStateMetaData(__SCONTEXT_PAR)
{
    return getMainStateMetaInfo(SCONTEXT)->Data;
}

// Get the main state lattice
static inline LatticeState_t* getMainStateLattice(__SCONTEXT_PAR)
{
    return &getSimulationState(SCONTEXT)->Lattice;
}

// Get a main state lattice entry by linearized id value
static inline byte_t getStateLatticeEntryAt(__SCONTEXT_PAR, const int32_t id)
{
    debug_assert(!span_IndexIsOutOfRange(*getMainStateLattice(SCONTEXT), id));
    return span_Get(*getMainStateLattice(SCONTEXT), id);
}

// Get the main state counters
static inline CountersState_t* getMainStateCounters(__SCONTEXT_PAR)
{
    return &getSimulationState(SCONTEXT)->Counters;
}

// Get the main state counter collection for the passed jump collection id
static inline StateCounterCollection_t* getMainStateCounterAt(__SCONTEXT_PAR, const byte_t jumpCollectionId)
{
    debug_assert(!span_IndexIsOutOfRange(*getMainStateCounters(SCONTEXT), jumpCollectionId));
    return &span_Get(*getMainStateCounters(SCONTEXT), jumpCollectionId);
}

// Get the global movement trackers that track mean collective movements for [jumpColId][particleId] combinations
static inline TrackersState_t* getGlobalMovementTrackers(__SCONTEXT_PAR)
{
    return &getSimulationState(SCONTEXT)->GlobalTrackers;
}

// Get the static movement trackers that track mean collective movements for [positionId][particleId] combinations
static inline TrackersState_t* getStaticMovementTrackers(__SCONTEXT_PAR)
{
    return &getSimulationState(SCONTEXT)->StaticTrackers;
}

// Get the mobile movement trackers that track local movement for each mobile particle of the lattice
static inline TrackersState_t* getMobileMovementTrackers(__SCONTEXT_PAR)
{
    return &getSimulationState(SCONTEXT)->MobileTrackers;
}

// Get the mobile tracker mapping that assigns each existing tracker its current host [globalPosId]
static inline MobileTrackerMapping_t* getMobileTrackerMapping(__SCONTEXT_PAR)
{
    return &getSimulationState(SCONTEXT)->MobileTrackerMapping;
}

// Get the jump statistics that track energy occurrence info for [jumpColId][particleId] combinations
static inline JumpStatisticsState_t* getJumpStatistics(__SCONTEXT_PAR)
{
    return &getSimulationState(SCONTEXT)->JumpStatistics;
}

// Get a static movement tracker for the passed combination of [positionId] and [particleId]
static inline Tracker_t* getStaticMovementTrackerAt(__SCONTEXT_PAR, const int32_t positionId, const byte_t particleId)
{
    int32_t trackerId = array_Get(*getStaticTrackerMappingTable(SCONTEXT), positionId, particleId);
    debug_assert(!span_IndexIsOutOfRange(*getStaticMovementTrackers(SCONTEXT), trackerId));
    return &span_Get(*getStaticMovementTrackers(SCONTEXT), trackerId);
}

// Get a global movement tracker for the passed combination of [jumpColId] and [particleId]
static inline Tracker_t* getGlobalMovementTrackerAt(__SCONTEXT_PAR, const int32_t jumpColId, const byte_t particleId)
{
    int32_t trackerId = array_Get(*getGlobalTrackerMappingTable(SCONTEXT), jumpColId, particleId);
    debug_assert(!span_IndexIsOutOfRange(*getGlobalMovementTrackers(SCONTEXT), trackerId));
    return &span_Get(*getGlobalMovementTrackers(SCONTEXT), trackerId);
}

// Get a jump statistic for the passed combination of [jumpColId] and [particleId]
static inline JumpStatistic_t* getJumpStatisticAt(__SCONTEXT_PAR, const int32_t jumpColId, const byte_t particleId)
{
    int32_t trackerId = array_Get(*getGlobalTrackerMappingTable(SCONTEXT), jumpColId, particleId);
    debug_assert(!span_IndexIsOutOfRange(*getGlobalMovementTrackers(SCONTEXT), trackerId));
    return &span_Get(*getJumpStatistics(SCONTEXT), trackerId);
}

/* Jump selection pool getter/setter */

// Get the direction pool mapping that maps number of directions to a [directionPoolId]
static inline IdRedirection_t* getDirectionPoolMapping(__SCONTEXT_PAR)
{
    return &getJumpSelectionPool(SCONTEXT)->DirectionPoolMapping;
}

// Set the direction pool mapping to the passed value
static inline void setDirectionPoolMapping(__SCONTEXT_PAR, IdRedirection_t value)
{
    *getDirectionPoolMapping(SCONTEXT) = value;
}

// Get the direction pool id that is mapped to the passed jump count
static inline int32_t getDirectionPoolIdByJumpCount(__SCONTEXT_PAR, const int32_t jumpCount)
{
    debug_assert(!span_IndexIsOutOfRange(*getDirectionPoolMapping(SCONTEXT), jumpCount));
    return span_Get(*getDirectionPoolMapping(SCONTEXT), jumpCount);
}

// Set the direction pool id of the passed jump count in the direction pool mapping
static inline void setDirectionPoolIdByJumpCount(__SCONTEXT_PAR, const int32_t jumpCount, const int32_t directionPoolId)
{
    debug_assert(!span_IndexIsOutOfRange(*getDirectionPoolMapping(SCONTEXT), jumpCount));
    span_Get(*getDirectionPoolMapping(SCONTEXT), jumpCount) = directionPoolId;
}

// Get all jump direction pools from the simulation context
static inline DirectionPools_t* getDirectionPools(__SCONTEXT_PAR)
{
    return &getJumpSelectionPool(SCONTEXT)->DirectionPools;
}

// Set the jump directions pools on the simulation context to the passed value
static inline void setDirectionPools(__SCONTEXT_PAR, DirectionPools_t value)
{
    *getDirectionPools(SCONTEXT) = value;
}

// Get the jump direction pool at the specified [directionPoolId]
static inline DirectionPool_t* getDirectionPoolAt(__SCONTEXT_PAR, const int32_t directionPoolId)
{
    debug_assert(!span_IndexIsOutOfRange(*getDirectionPools(SCONTEXT), directionPoolId));
    return &span_Get(*getDirectionPools(SCONTEXT), directionPoolId);
}

// Get the jump direction pool that is mapped to the passed [jumpCount]
static inline DirectionPool_t* getDirectionPoolByJumpCount(__SCONTEXT_PAR, const int32_t jumpCount)
{
    return getDirectionPoolAt(SCONTEXT, getDirectionPoolIdByJumpCount(SCONTEXT, jumpCount));
}

/* Command arguments getter/setter */

// Get the command arguments passed to the simulation context
static inline CmdArguments_t* getCommandArguments(__SCONTEXT_PAR)
{
    return &SCONTEXT->CommandArguments;
}

// Get the command argument string at te specified input id
static inline char const * getCommandArgumentStringAt(__SCONTEXT_PAR, const int32_t id)
{
    if (id >= getCommandArguments(SCONTEXT)->Count)
    {
        return NULL;
    }
    return getCommandArguments(SCONTEXT)->Values[id];
}

// Set the command arguments on the simulation context
static inline void setCommandArguments(__SCONTEXT_PAR, const int32_t argc, char const * const * argv)
{
    *getCommandArguments(SCONTEXT) = (CmdArguments_t) {  argv, argc };
}

// Set the program run path on the simulation context
static inline void setProgramRunPath(__SCONTEXT_PAR, char const * value)
{
    getFileInformation(SCONTEXT)->ExecutionPath = value;
}

// Set the database load string in the simulation context
static inline void setDatabaseLoadString(__SCONTEXT_PAR, char const * value)
{
    getFileInformation(SCONTEXT)->DbQueryString = value;
}

// Set the database path on the simulation context
static inline void setDatabasePath(__SCONTEXT_PAR, char const * value)
{
    getFileInformation(SCONTEXT)->DatabasePath = value;
}

// Sets the path string of the output plugin on the simulation context
static inline void setOutputPluginPath(__SCONTEXT_PAR, char const * path)
{
    getFileInformation(SCONTEXT)->OutputPluginPath = path;
}

// Sets the symbol string of the output plugin on the simulation context
static inline void setOutputPluginSymbol(__SCONTEXT_PAR, const char* symbol)
{
    getFileInformation(SCONTEXT)->OutputPluginSymbol = symbol;
}

// Sets the path string of the energy plugin on the simulation context
static inline void setEnergyPluginPath(__SCONTEXT_PAR, char const * path)
{
    getFileInformation(SCONTEXT)->EnergyPluginPath = path;
}

// Sets the symbol string of the energy plugin on the simulation context
static inline void setEnergyPluginSymbol(__SCONTEXT_PAR, const char* symbol)
{
    getFileInformation(SCONTEXT)->EnergyPluginSymbol = symbol;
}


/* Selection pool getter/setter */

// Get the environment pool entry at the specified id
static inline int32_t getEnvironmentPoolEntryAt(DirectionPool_t *restrict directionPool, const int32_t id)
{
    debug_assert(!span_IndexIsOutOfRange(directionPool->EnvironmentPool, id));
    return span_Get(directionPool->EnvironmentPool, id);
}

// Set the environment pool entry at the specified id to the given value
static inline void setEnvironmentPoolEntryAt(DirectionPool_t *restrict directionPool, const int32_t id, const int32_t value)
{
    debug_assert(!span_IndexIsOutOfRange(directionPool->EnvironmentPool, id));
    span_Get(directionPool->EnvironmentPool, id) = value;
}

/* Energy table getter/setter */

// Get the pair energy table entry from the passed table at the specified [particleId0, particleId1] combination
static inline double getPairEnergyAt(const PairTable_t *restrict table, const byte_t particleId0, const byte_t particleId1)
{
    return array_Get(table->EnergyTable, particleId0, particleId1);
}

// Get the cluster energy table entry from the passed table at the specified [particleId, codeId] combination
static inline double getClusterEnergyAt(const ClusterTable_t *restrict table, const byte_t particleId, const int32_t codeId)
{
    debug_assert(particleId < PARTICLE_IDLIMIT);
    return array_Get(table->EnergyTable, table->ParticleTableMapping[particleId], codeId);
}

/* Flag getter/setters */

// Get the bitmask for the main state flags
static inline Bitmask_t getMainStateFlags(__SCONTEXT_PAR)
{
    return getMainStateHeader(SCONTEXT)->Data->Flags;
}

// set the passed flags on the main state
static inline void setMainStateFlags(__SCONTEXT_PAR, const Bitmask_t flags)
{
    setFlags(getMainStateHeader(SCONTEXT)->Data->Flags, flags);
}

// Unsets the passed flags on the main state
static inline void UnsetMainStateFlags(__SCONTEXT_PAR, const Bitmask_t flags)
{
    unsetFlags(getMainStateHeader(SCONTEXT)->Data->Flags, flags);
}

// Get the job header flags in the MMC case
static inline Bitmask_t getJobHeaderFlagsMmc(__SCONTEXT_PAR)
{
    return getDbModelJobHeaderAsMMC(SCONTEXT)->JobFlags;
}

// Get the job header flags in the KMC case
static inline Bitmask_t getJobHeaderFlagsKmc(__SCONTEXT_PAR)
{
    return getDbModelJobHeaderAsKMC(SCONTEXT)->JobFlags;
}

/* Environment getter/setter */

// Get the number of pair definitions on the passed environment state
static inline int32_t getEnvironmentPairDefinitionCount(EnvironmentState_t *restrict envState)
{
    return (int32_t) span_GetSize(envState->EnvironmentDefinition->PairDefinitions);
}

// Get the pair definition at the passed [relPairId] from an environment state
static inline PairDefinition_t* getEnvironmentPairDefinitionAt(EnvironmentState_t *restrict envState, const int32_t relPairId)
{
    debug_assert(!span_IndexIsOutOfRange(envState->EnvironmentDefinition->PairDefinitions, relPairId));
    return &span_Get(envState->EnvironmentDefinition->PairDefinitions, relPairId);
}

// Get the cluster definition at the passed [relClusterId] from an environment state
static inline ClusterDefinition_t* getEnvironmentClusterDefinitionAt(EnvironmentState_t *restrict envState, const int32_t relClusterId)
{
    debug_assert(!span_IndexIsOutOfRange(envState->EnvironmentDefinition->ClusterDefinitions, relClusterId));
    return &span_Get(envState->EnvironmentDefinition->ClusterDefinitions, relClusterId);
}

// Get the cluster state at the passed [relClusterId] from an environment state
static inline ClusterState_t* getEnvironmentClusterStateAt(EnvironmentState_t *restrict envState, const byte_t relClusterId)
{
    debug_assert(!span_IndexIsOutOfRange(envState->ClusterStates, relClusterId));
    return &span_Get(envState->ClusterStates, relClusterId);
}

/* Active delta object getter/setter */

// Get the active state energy that belongs to the passed [particleId] from the currently set active work environment
static inline double* getActiveStateEnergyAt(__SCONTEXT_PAR, const byte_t particleId)
{
    debug_assert(!span_IndexIsOutOfRange(getActiveWorkEnvironment(SCONTEXT)->EnergyStates, particleId));
    return &span_Get(getActiveWorkEnvironment(SCONTEXT)->EnergyStates, particleId);
}

// Get the active particle update id by an offset id
static inline byte_t getActiveParticleUpdateIdAt(__SCONTEXT_PAR, const byte_t id)
{
    debug_assert(id < PARTICLE_IDLIMIT);
    return getActiveWorkEnvironment(SCONTEXT)->EnvironmentDefinition->UpdateParticleIds[id];
}

// Get an environment link by a jump link pointer
static inline EnvironmentLink_t* getEnvLinkByJumpLink(__SCONTEXT_PAR, const JumpLink_t* restrict jumpLink)
{
    debug_assert(jumpLink->PathId < 8);
    debug_assert(!span_IndexIsOutOfRange(JUMPPATH[jumpLink->PathId]->EnvironmentLinks, jumpLink->LinkId));

    return &span_Get(JUMPPATH[jumpLink->PathId]->EnvironmentLinks, jumpLink->LinkId);
}

// Gte a path state energy pointer by path id and particle id
static inline double* getPathStateEnergyByIds(__SCONTEXT_PAR, const byte_t pathId, const byte_t particleId)
{
    debug_assert(pathId < 8);
    debug_assert(!span_IndexIsOutOfRange(JUMPPATH[pathId]->EnergyStates, particleId));

    return &span_Get(JUMPPATH[pathId]->EnergyStates, particleId);
}

// Get the environment state energy backup pointer by a path id
static inline double* getEnvStateEnergyBackupById(__SCONTEXT_PAR, const byte_t pathId)
{
    debug_assert(pathId < 8);
    return &getCycleState(SCONTEXT)->ActiveEnvironmentBackup.PathEnergies[pathId];
}

// Get the jump links that are currently required for delta generation
static inline JumpLinks_t* getActiveLocalJumpLinks(__SCONTEXT_PAR)
{
    return &array_Get(*getJumpStatusArray(SCONTEXT),
            JUMPPATH[0]->PositionVector.a,
            JUMPPATH[0]->PositionVector.b,
            JUMPPATH[0]->PositionVector.c,
            getActiveJumpDirection(SCONTEXT)->ObjectId).JumpLinks;
}
