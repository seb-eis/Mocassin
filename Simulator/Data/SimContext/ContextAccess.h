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
#include "Extensions/Interface/MocsimExtesionTypes.h"
#include "Framework/Math/Random/PcgRandom.h"
#include "Framework/Basic/BaseTypes/BaseTypes.h"
#include "Framework/Math/Types/Vector.h"
#include "Simulator/Data/SimContext/SimContext.h"

/* Context access defines */

// Defines the name of simulation context access parameter
#define SCONTEXT_PARAMETER_NAME simContext

// Defines the simulation context function parameter
#define SCONTEXT_PARAMETER      SimulationContext_t* restrict SCONTEXT_PARAMETER_NAME

// Defines the simulation context jumppath access macro
#define JUMPPATH                simContext->CycleState.ActivePathEnvironments

// Defines the simulation context simulation error access macro
#define SIMERROR                simContext->ErrorCode

/* Context getter/setter */

// Get the main simulation state from the context
static inline SimulationState_t* getSimulationState(SCONTEXT_PARAMETER)
{
    return &simContext->MainState;
}

// Get the database model from the context
static inline DbModel_t* getDatabaseModel(SCONTEXT_PARAMETER)
{
    return &simContext->DbModel;
}

// Get the dynamic simulation model from the context
static inline DynamicModel_t* getDynamicModel(SCONTEXT_PARAMETER)
{
    return &simContext->DynamicModel;
}

// Get the jump selection pool from the context
static inline JumpSelectionPool_t* getJumpSelectionPool(SCONTEXT_PARAMETER)
{
    return &simContext->SelectionPool;
}

// Get the cycle state from the context
static inline CycleState_t* getCycleState(SCONTEXT_PARAMETER)
{
    return &simContext->CycleState;
}

// Get the random number generator from the context
static inline Pcg32_t* getMainRng(SCONTEXT_PARAMETER)
{
    return &simContext->Rng;
}

// Get the simulation plugins from the context
static inline SimulationPlugins_t* getPluginCollection(SCONTEXT_PARAMETER)
{
    return &simContext->Plugins;
}

// Get the jump status array that contains information of all existing jumps in the lattice
static inline JumpStatusArray_t* getJumpStatusArray(SCONTEXT_PARAMETER)
{
    return &getDynamicModel(simContext)->JumpStatusArray;
}

// Get a jump status from the context by a 4D vector [A,B,C,JumpDirId]
static inline JumpStatus_t* getJumpStatusByVector4(SCONTEXT_PARAMETER, const Vector4_t*restrict vector)
{
    debug_assert(!array_IsIndexOutOfRange(*getJumpStatusArray(simContext), vecCoorSet4(*vector)));
    return &array_Get(*getJumpStatusArray(simContext), vecCoorSet4(*vector));
}


/* Simulation model getter/setter */

// Get the file information from the context
static inline FileInfo_t* getFileInformation(SCONTEXT_PARAMETER)
{
    return &getDynamicModel(simContext)->FileInfo;
}

// Get the physical factor collection from the context
static inline PhysicalInfo_t* getPhysicalFactors(SCONTEXT_PARAMETER)
{
    return &getDynamicModel(simContext)->PhysicalFactors;
}

// Get the lattice energy buffer from the context
static inline Flp64Buffer_t* getLatticeEnergyBuffer(SCONTEXT_PARAMETER)
{
    return &getDynamicModel(simContext)->LatticeEnergyBuffer;
}

// Get the simulation runtime information from the context
static inline SimulationRunInfo_t* getRuntimeInformation(SCONTEXT_PARAMETER)
{
    return &getDynamicModel(simContext)->RuntimeInfo;
}

// Get the environment lattice from the context
static inline EnvironmentLattice_t* getEnvironmentLattice(SCONTEXT_PARAMETER)
{
    return &getDynamicModel(simContext)->EnvironmentLattice;
}

// Set the environment lattice on the context
static inline void setEnvironmentLattice(SCONTEXT_PARAMETER, EnvironmentLattice_t value)
{
    *getEnvironmentLattice(simContext) = value;
}

// Get an environment state by its linearized environment id from the context
static inline EnvironmentState_t* getEnvironmentStateAt(SCONTEXT_PARAMETER, const int32_t environmentId)
{
    debug_assert(!span_IsIndexOutOfRange(*getEnvironmentLattice(simContext), environmentId));
    return &span_Get(*getEnvironmentLattice(simContext), environmentId);
}

// Get an environment state by (A,B,C,D) coordinate access from the context
static inline EnvironmentState_t* getEnvironmentStateByIds(SCONTEXT_PARAMETER, const int32_t a, const int32_t b, const int32_t c, const int32_t d)
{
    debug_assert(!array_IsIndexOutOfRange(*getEnvironmentLattice(simContext), a, b, c, d));
    return &array_Get(*getEnvironmentLattice(simContext), a, b, c, d);
}

// Get a linearized environment state id by performing pointer arithmetic on the state environment buffer
static inline int32_t getEnvironmentStateIdByPointer(SCONTEXT_PARAMETER, const EnvironmentState_t*restrict environmentState)
{
    let id = environmentState - getEnvironmentLattice(simContext)->Begin;
    debug_assert(!span_IsIndexOutOfRange(*getEnvironmentLattice(simContext), id));
    return id;
}

// Get an environment state by a 4D vector access from the context
static inline EnvironmentState_t* getEnvironmentStateByVector4(SCONTEXT_PARAMETER, const Vector4_t* restrict vector)
{
    return getEnvironmentStateByIds(simContext, vecCoorSet4(*vector));
}

// Get the lattice block sizes from the context
static inline int32_t* getLatticeBlockSizes(SCONTEXT_PARAMETER)
{
    return getEnvironmentLattice(simContext)->Header->Blocks;
}

// Get the static tracker mapping table from the context
static inline TrackerMappingTable_t* getStaticTrackerMappingTable(SCONTEXT_PARAMETER)
{
    return &getDatabaseModel(simContext)->TransitionModel.StaticTrackerMappingTable;
}

// Get the global tracker mapping table from the context
static inline TrackerMappingTable_t* getGlobalTrackerMappingTable(SCONTEXT_PARAMETER)
{
    return &getDatabaseModel(simContext)->TransitionModel.GlobalTrackerMappingTable;
}

/* Cycle state getter/setter */

// Get the counters of the simulation cycle state
static inline CycleCounterState_t* getMainCycleCounters(SCONTEXT_PARAMETER)
{
    return &getCycleState(simContext)->MainCounters;
}

// Get the active state code for the jump path
static inline OccupationCode64_t getPathStateCode(SCONTEXT_PARAMETER)
{
    return getCycleState(simContext)->ActiveStateCode;
}

// Set the active state code for the jump path
static inline void setPathStateCode(SCONTEXT_PARAMETER, const OccupationCode64_t code)
{
    getCycleState(simContext)->ActiveStateCode = code;
}

// Get the active jump selection info
static inline JumpSelectionInfo_t* getJumpSelectionInfo(SCONTEXT_PARAMETER)
{
    return &getCycleState(simContext)->ActiveSelectionInfo;
}

// Get the active jump energy info
static inline JumpEnergyInfo_t* getJumpEnergyInfo(SCONTEXT_PARAMETER)
{
    return &getCycleState(simContext)->ActiveEnergyInfo;
}

// Get the active environment backup
static inline EnvironmentBackup_t* getEnvironmentBackup(SCONTEXT_PARAMETER)
{
    return &getCycleState(simContext)->ActiveEnvironmentBackup;
}

// Get the currently active jump direction
static inline JumpDirection_t* getActiveJumpDirection(SCONTEXT_PARAMETER)
{
    return getCycleState(simContext)->ActiveJumpDirection;
}

// Set the currently active jump direction
static inline void setActiveJumpDirection(SCONTEXT_PARAMETER, JumpDirection_t* value)
{
    getCycleState(simContext)->ActiveJumpDirection = value;
}

// Get the currently active jump collection
static inline JumpCollection_t* getActiveJumpCollection(SCONTEXT_PARAMETER)
{
    return getCycleState(simContext)->ActiveJumpCollection;
}

// Set the currently active jump collection
static inline void setActiveJumpCollection(SCONTEXT_PARAMETER, JumpCollection_t* value)
{
    getCycleState(simContext)->ActiveJumpCollection = value;
}

// Get the currently active jump rule
static inline JumpRule_t* getActiveJumpRule(SCONTEXT_PARAMETER)
{
    return getCycleState(simContext)->ActiveJumpRule;
}

// Set the currently active jump rule
static inline void setActiveJumpRule(SCONTEXT_PARAMETER, JumpRule_t* value)
{
    getCycleState(simContext)->ActiveJumpRule = value;
}

// Get the currently active state counter collection
static inline StateCounterCollection_t* getActiveCounters(SCONTEXT_PARAMETER)
{
    return getCycleState(simContext)->ActiveCounterCollection;
}

// Set the currently active state counter collection
static inline void setActiveCounters(SCONTEXT_PARAMETER, StateCounterCollection_t* value)
{
    getCycleState(simContext)->ActiveCounterCollection = value;
}

// Get the path environment by a path id value
static inline EnvironmentState_t* getPathEnvironmentAt(SCONTEXT_PARAMETER, const byte_t pathId)
{
    debug_assert((pathId < JUMPS_JUMPLENGTH_MAX));
    return getCycleState(simContext)->ActivePathEnvironments[pathId];
}

// Set the path environment at the given id
static inline void setPathEnvironmentAt(SCONTEXT_PARAMETER, const byte_t pathId, EnvironmentState_t* value)
{
    debug_assert(pathId < JUMPS_JUMPLENGTH_MAX);
    getCycleState(simContext)->ActivePathEnvironments[pathId] = value;
}

// Get the currently active work environment
static inline EnvironmentState_t* getActiveWorkEnvironment(SCONTEXT_PARAMETER)
{
    return getCycleState(simContext)->WorkEnvironment;
}

// Get the currently active work cluster
static inline ClusterState_t* getActiveWorkCluster(SCONTEXT_PARAMETER)
{
    return getCycleState(simContext)->WorkCluster;
}

// Get the currently active cluster energy table
static inline ClusterTable_t* getActiveClusterTable(SCONTEXT_PARAMETER)
{
    return getCycleState(simContext)->WorkClusterTable;
}

#if defined(OPT_USE_3D_PAIRTABLES)
// Get the currently active pair energy delta table
static inline PairDeltaTable_t* getActivePairTable(SCONTEXT_PARAMETER)
{
    return getCycleState(simContext)->WorkPairTable;
}
#else
// Get the currently active pair energy table
static inline PairTable_t* getActivePairTable(SCONTEXT_PARAMETER)
{
    return getCycleState(simContext)->WorkPairTable;
}
#endif

/* Database model getter/setter */

// Get the lattice model from the database model
static inline LatticeModel_t* getDbLatticeModel(SCONTEXT_PARAMETER)
{
    return &getDatabaseModel(simContext)->LatticeModel;
}

// Get the lattice model meta info from the database model
static inline LatticeInfo_t* getDbLatticeInfo(SCONTEXT_PARAMETER)
{
    return &getDbLatticeModel(simContext)->LatticeInfo;
}

// Get the particle lattice from the database model data
static inline Lattice_t* getDbModelLattice(SCONTEXT_PARAMETER)
{
    return &getDbLatticeModel(simContext)->Lattice;
}

// Get the job info from the database model data
static inline JobInfo_t* getDbModelJobInfo(SCONTEXT_PARAMETER)
{
    return &getDatabaseModel(simContext)->JobModel.JobInfo;
}

// Get the custom routine data from the database model data
static inline RoutineData_t* getCustomRoutineData(SCONTEXT_PARAMETER)
{
    return &getDatabaseModel(simContext)->JobModel.RoutineData;
}

// Get the custom routine 16 byte uuid from the database model data
static inline moc_uuid_t* getCustomRoutineUuid(SCONTEXT_PARAMETER)
{
    return (moc_uuid_t*) getCustomRoutineData(simContext)->Guid;
}

// Get the job header from the database model as a KMC header
static inline KmcHeader_t* getDbModelJobHeaderAsKMC(SCONTEXT_PARAMETER)
{
    return (KmcHeader_t*) getDbModelJobInfo(simContext)->JobHeader;
}

// Get the job header from the database model as an MMC header
static inline MmcHeader_t* getDbModelJobHeaderAsMMC(SCONTEXT_PARAMETER)
{
    return (MmcHeader_t*) getDbModelJobInfo(simContext)->JobHeader;
}

// Get the structure model from the database model
static inline StructureModel_t* getDbStructureModel(SCONTEXT_PARAMETER)
{
    return &getDatabaseModel(simContext)->StructureModel;
}

// Get the structure meta data from the model data
static inline StructureMetaData_t* getDbStructureModelMetaData(SCONTEXT_PARAMETER)
{
    return getDbStructureModel(simContext)->MetaData;
}

// Get the energy model from the database model
static inline EnergyModel_t* getDbEnergyModel(SCONTEXT_PARAMETER)
{
    return &getDatabaseModel(simContext)->EnergyModel;
}

// Get the transition model from the database model
static inline TransitionModel_t* getDbTransitionModel(SCONTEXT_PARAMETER)
{
    return &getDatabaseModel(simContext)->TransitionModel;
}

// Get the environment models from the database model data
static inline EnvironmentDefinitions_t* getEnvironmentModels(SCONTEXT_PARAMETER)
{
    return &getDbStructureModel(simContext)->EnvironmentDefinitions;
}

// Get the environment model from the database model data at the given index
static inline EnvironmentDefinition_t* getEnvironmentModelAt(SCONTEXT_PARAMETER, const int32_t id)
{
    debug_assert(!span_IsIndexOutOfRange(*getEnvironmentModels(simContext), id));
    return &span_Get(*getEnvironmentModels(simContext), id);
}

// Get the lattice size vector from the database model data
static inline Vector4_t* getLatticeSizeVector(SCONTEXT_PARAMETER)
{
    return &getDbLatticeInfo(simContext)->SizeVector;
}

// Get the number of mobiles from the db lattice meta info
static inline int32_t getNumberOfMobiles(SCONTEXT_PARAMETER)
{
    return getDbLatticeInfo(simContext)->MobileParticleCount;
}

// Get the number of selectable particles from the db lattice meta info
static inline int32_t getNumberOfSelectables(SCONTEXT_PARAMETER)
{
    return getDbLatticeInfo(simContext)->SelectParticleCount;
}

// Get the jump count mapping that assigns each [positionId][particleId] combination its jump count
static inline JumpCountTable_t* getJumpCountMapping(SCONTEXT_PARAMETER)
{
    return &getDbTransitionModel(simContext)->JumpCountMappingTable;
}

// Get the jump count for the passed [positionId][particleId] combination
static inline int32_t getJumpCountAt(SCONTEXT_PARAMETER, const int32_t positionId, const byte_t particleId)
{
    debug_assert(!array_IsIndexOutOfRange(*getJumpCountMapping(simContext), positionId, particleId));
    return array_Get(*getJumpCountMapping(simContext), positionId, particleId);
}

// Get the jump direction mapping that assigns each [positionId][particleId][relJumpId] its valid [jumpDirectionId]
static inline JumpMappingTable_t* getJumpDirectionMapping(SCONTEXT_PARAMETER)
{
    return &getDbTransitionModel(simContext)->JumpDirectionMappingTable;
}

// Get all jump directions from the database model data
static inline JumpDirections_t* getJumpDirections(SCONTEXT_PARAMETER)
{
    return &getDbTransitionModel(simContext)->JumpDirections;
}

// Get the jump direction at the specified [jumpDirectionId]
static inline JumpDirection_t* getJumpDirectionAt(SCONTEXT_PARAMETER, const int32_t jumpDirectionId)
{
    debug_assert(!span_IsIndexOutOfRange(*getJumpDirections(simContext), jumpDirectionId));
    return &span_Get(*getJumpDirections(simContext), jumpDirectionId);
}

// Get all jump collections from the database model data
static inline JumpCollections_t* getJumpCollections(SCONTEXT_PARAMETER)
{
    return &getDbTransitionModel(simContext)->JumpCollections;
}

// Get the jump collection at the specified [jumpCollectionId]
static inline JumpCollection_t* getJumpCollectionAt(SCONTEXT_PARAMETER, const int32_t jumpCollectionId)
{
    debug_assert(!span_IsIndexOutOfRange(*getJumpCollections(simContext), jumpCollectionId));
    return &span_Get(*getJumpCollections(simContext), jumpCollectionId);
}

// Get all pair energy tables from the database model data
static inline PairTables_t* getPairEnergyTables(SCONTEXT_PARAMETER)
{
    return &getDbEnergyModel(simContext)->PairTables;
}

//  Get the defect background that defines a defect energy foreach [PositionId][ParticleId] combination
static inline DefectBackground_t* getDefectBackground(SCONTEXT_PARAMETER)
{
    return &getDbEnergyModel(simContext)->DefectBackground;
}

//  Get the energy background that defines a defect energy foreach [A,B,C,D,ParticleId]] lattice option
static inline EnergyBackground_t* getLatticeEnergyBackground(SCONTEXT_PARAMETER)
{
    return &getDbLatticeModel(simContext)->EnergyBackground;
}

// Get the pair energy table at the specified [pairTableId]
static inline PairTable_t* getPairEnergyTableAt(SCONTEXT_PARAMETER, const int32_t pairTableId)
{
    debug_assert(!span_IsIndexOutOfRange(*getPairEnergyTables(simContext), pairTableId));
    return &span_Get(*getPairEnergyTables(simContext), pairTableId);
}

// Get all cluster energy tables from the database model data
static inline ClusterTables_t* getClusterEnergyTables(SCONTEXT_PARAMETER)
{
    return &getDbEnergyModel(simContext)->ClusterTables;
}

// Get the cluster energy table at the specified [clusterTableId]
static inline ClusterTable_t* getClusterEnergyTableAt(SCONTEXT_PARAMETER, const int32_t clusterTableId)
{
    debug_assert(!span_IsIndexOutOfRange(*getClusterEnergyTables(simContext), clusterTableId));
    return &span_Get(*getClusterEnergyTables(simContext), clusterTableId);
}

#if defined(OPT_USE_3D_PAIRTABLES)
// Get the pair delta tables from the passed context. Access by [TableId][OriginalParticleId][NewParticleId][CenterParticleId]
static inline PairDeltaTables_t* getPairDeltaTables(SCONTEXT_PARAMETER)
{
    return &getDynamicModel(simContext)->PairDeltaTables;
}

// Get the pair delta table from the passed context that belongs to the passed table id. Access by [OriginalParticleId][NewParticleId][CenterParticleId]
static inline PairDeltaTable_t* getPairDeltaTableAt(SCONTEXT_PARAMETER, const int32_t pairTableId)
{
    debug_assert(!span_IsIndexOutOfRange(*getPairDeltaTables(simContext), pairTableId));
    return &span_Get(*getPairDeltaTables(simContext), pairTableId);
}
#endif

/* Main state getter/setter */

// Get the buffer access to the main state binary
static inline Buffer_t* getMainStateBuffer(SCONTEXT_PARAMETER)
{
    return &getSimulationState(simContext)->Buffer;
}

// Get a pointer to the main state buffer at the given offset byte
static inline void* getMainStateBufferAddress(SCONTEXT_PARAMETER, const int32_t offsetBytes)
{
    debug_assert(!span_IsIndexOutOfRange(*getMainStateBuffer(simContext), offsetBytes));
    return &span_Get(*getMainStateBuffer(simContext), offsetBytes);
}

// Get the main state header
static inline StateHeader_t* getMainStateHeader(SCONTEXT_PARAMETER)
{
    return &getSimulationState(simContext)->Header;
}

// Get the main state meta information
static inline StateMetaInfo_t* getMainStateMetaInfo(SCONTEXT_PARAMETER)
{
    return &getSimulationState(simContext)->Meta;
}

// Get the main state meta information data
static inline StateMetaData_t* getMainStateMetaData(SCONTEXT_PARAMETER)
{
    return getMainStateMetaInfo(simContext)->Data;
}

// Get the main state lattice
static inline LatticeState_t* getMainStateLattice(SCONTEXT_PARAMETER)
{
    return &getSimulationState(simContext)->Lattice;
}

// Get a main state lattice entry by linearized id value
static inline byte_t getStateLatticeEntryAt(SCONTEXT_PARAMETER, const int32_t id)
{
    debug_assert(!span_IsIndexOutOfRange(*getMainStateLattice(simContext), id));
    return span_Get(*getMainStateLattice(simContext), id);
}

// Get the main state counters
static inline CountersState_t* getMainStateCounters(SCONTEXT_PARAMETER)
{
    return &getSimulationState(simContext)->Counters;
}

// Get the main state counter collection for the passed [particleId]
static inline StateCounterCollection_t* getMainStateCounterAt(SCONTEXT_PARAMETER, const byte_t particleId)
{
    debug_assert(!span_IsIndexOutOfRange(*getMainStateCounters(simContext), particleId));
    return &span_Get(*getMainStateCounters(simContext), particleId);
}

// Get the global movement trackers that track mean collective movements for [jumpColId][particleId] combinations
static inline TrackersState_t* getGlobalMovementTrackers(SCONTEXT_PARAMETER)
{
    return &getSimulationState(simContext)->GlobalTrackers;
}

// Get the static movement trackers that track mean collective movements for [positionId][particleId] combinations
static inline TrackersState_t* getStaticMovementTrackers(SCONTEXT_PARAMETER)
{
    return &getSimulationState(simContext)->StaticTrackers;
}

// Get the mobile movement trackers that track local movement for each mobile particle of the lattice
static inline TrackersState_t* getMobileMovementTrackers(SCONTEXT_PARAMETER)
{
    return &getSimulationState(simContext)->MobileTrackers;
}

// Get the mobile tracker at the specified [trackerId] from the simulation state
static inline Tracker_t* getMobileTrackerAt(SCONTEXT_PARAMETER, const int32_t trackerId)
{
    debug_assert(!span_IsIndexOutOfRange(*getMobileMovementTrackers(simContext), trackerId));
    return &span_Get(*getMobileMovementTrackers(simContext), trackerId);
}

// Get the mobile tracker mapping that assigns each existing tracker its current host [globalPosId]
static inline MobileTrackerMapping_t* getMobileTrackerMapping(SCONTEXT_PARAMETER)
{
    return &getSimulationState(simContext)->MobileTrackerMapping;
}

// Get the jump statistics that track energy occurrence info for [jumpColId][particleId] combinations
static inline JumpStatisticsState_t* getJumpStatistics(SCONTEXT_PARAMETER)
{
    return &getSimulationState(simContext)->JumpStatistics;
}

// Get the mapped static movement tracker id offset for the passed combination of [positionId] and [particleId]
static inline int32_t  getStaticMovementTrackerIdOffsetAt(SCONTEXT_PARAMETER, const int32_t positionId,
                                                          const byte_t particleId)
{
    debug_assert(!array_IsIndexOutOfRange(*getStaticTrackerMappingTable(simContext), positionId, particleId));
    return array_Get(*getStaticTrackerMappingTable(simContext), positionId, particleId);
}

// Get the cell index of the passed [a,b,c] coordinate set
static inline int32_t getCellIndexAt(SCONTEXT_PARAMETER, const int32_t a, const int32_t b, const int32_t c)
{
    let sizes = getLatticeSizeVector(simContext);
    return c + b * sizes->C + a * sizes->C * sizes->B;
}

// Get the cell index for the passed vector 4
static inline int32_t getCellIndexByVector4(SCONTEXT_PARAMETER, const Vector4_t* vector)
{
    return getCellIndexAt(simContext, vecCoorSet3(*vector));
}

// Get a static movement tracker that belongs to the passed vector 4 and particle id
static inline Tracker_t* getStaticMovementTrackerAt(SCONTEXT_PARAMETER, const Vector4_t* vector, const byte_t particleId)
{
    var index = getStaticMovementTrackerIdOffsetAt(simContext, vector->D, particleId);
    index += getCellIndexByVector4(simContext, vector) * getDbStructureModel(simContext)->StaticTrackersPerCellCount;
    debug_assert(!span_IsIndexOutOfRange(*getStaticMovementTrackers(simContext), index));
    return &span_Get(*getStaticMovementTrackers(simContext), index);
}

// Get the mapped global tracker id from the affiliated tracker mapping that belongs to the passed set of [jumpCollectionId] and [particleId]
static inline int32_t getGlobalTrackerIdAt(SCONTEXT_PARAMETER, const int32_t jumpColId, const byte_t particleId)
{
    debug_assert(!array_IsIndexOutOfRange(*getGlobalTrackerMappingTable(simContext), jumpColId, particleId));
    return array_Get(*getGlobalTrackerMappingTable(simContext), jumpColId, particleId);
}

// Get a global movement tracker for the passed combination of [jumpColId] and [particleId]
static inline Tracker_t* getGlobalMovementTrackerAt(SCONTEXT_PARAMETER, const int32_t jumpColId, const byte_t particleId)
{
    var trackerId = getGlobalTrackerIdAt(simContext, jumpColId, particleId);
    debug_assert(!span_IsIndexOutOfRange(*getGlobalMovementTrackers(simContext), trackerId));
    return &span_Get(*getGlobalMovementTrackers(simContext), trackerId);
}

// Get a jump statistic for the passed combination of [jumpColId] and [particleId]
static inline JumpStatistic_t* getJumpStatisticAt(SCONTEXT_PARAMETER, const int32_t jumpColId, const byte_t particleId)
{
    int32_t trackerId = getGlobalTrackerIdAt(simContext, jumpColId, particleId);
    debug_assert(!span_IsIndexOutOfRange(*getJumpStatistics(simContext), trackerId))
    return &span_Get(*getJumpStatistics(simContext), trackerId);
}

/* Jump selection pool getter/setter */

// Get the direction pool mapping that maps number of directions to a [directionPoolId]
static inline IdMappingSpan_t* getDirectionPoolMapping(SCONTEXT_PARAMETER)
{
    return &getJumpSelectionPool(simContext)->DirectionPoolMapping;
}

// Set the direction pool mapping to the passed value
static inline void setDirectionPoolMapping(SCONTEXT_PARAMETER, IdMappingSpan_t value)
{
    *getDirectionPoolMapping(simContext) = value;
}

// Get the direction pool id that is mapped to the passed jump count
static inline int32_t getDirectionPoolIdByJumpCount(SCONTEXT_PARAMETER, const int32_t jumpCount)
{
    debug_assert(!span_IsIndexOutOfRange(*getDirectionPoolMapping(simContext), jumpCount));
    return span_Get(*getDirectionPoolMapping(simContext), jumpCount);
}

// Set the direction pool id of the passed jump count in the direction pool mapping
static inline void setDirectionPoolIdByJumpCount(SCONTEXT_PARAMETER, const int32_t jumpCount, const int32_t directionPoolId)
{
    debug_assert(!span_IsIndexOutOfRange(*getDirectionPoolMapping(simContext), jumpCount));
    span_Get(*getDirectionPoolMapping(simContext), jumpCount) = directionPoolId;
}

// Get all jump direction pools from the simulation context
static inline DirectionPools_t* getDirectionPools(SCONTEXT_PARAMETER)
{
    return &getJumpSelectionPool(simContext)->DirectionPools;
}

// Set the jump directions pools on the simulation context to the passed value
static inline void setDirectionPools(SCONTEXT_PARAMETER, DirectionPools_t value)
{
    *getDirectionPools(simContext) = value;
}

// Get the jump direction pool at the specified [directionPoolId]
static inline DirectionPool_t* getDirectionPoolAt(SCONTEXT_PARAMETER, const int32_t directionPoolId)
{
    debug_assert(!span_IsIndexOutOfRange(*getDirectionPools(simContext), directionPoolId));
    return &span_Get(*getDirectionPools(simContext), directionPoolId);
}

// Get the jump direction pool that is mapped to the passed [jumpCount]
static inline DirectionPool_t* getDirectionPoolByJumpCount(SCONTEXT_PARAMETER, const int32_t jumpCount)
{
    return getDirectionPoolAt(simContext, getDirectionPoolIdByJumpCount(simContext, jumpCount));
}

/* Command arguments getter/setter */

// Get the command arguments passed to the simulation context
static inline CmdArguments_t* getCommandArguments(SCONTEXT_PARAMETER)
{
    return &simContext->CommandArguments;
}

// Get the command argument string at te specified input id
static inline char const * getCommandArgumentStringAt(SCONTEXT_PARAMETER, const int32_t id)
{
    return (id >= getCommandArguments(simContext)->Count) ? NULL : getCommandArguments(simContext)->Values[id];
}

// Set the command arguments on the simulation context
static inline void setCommandArguments(SCONTEXT_PARAMETER, const int32_t argc, char const * const * argv)
{
    *getCommandArguments(simContext) = (CmdArguments_t) {argv, argc };
}

// Set the program run path on the simulation context
static inline void setProgramRunPath(SCONTEXT_PARAMETER, char const * value)
{
    getFileInformation(simContext)->ExecutablePath = value;
}

// Set the program extension lookup path on the simulation context
static inline void setExtensionLookupPath(SCONTEXT_PARAMETER, char const * value)
{
    getFileInformation(simContext)->ExtensionLookupPath = value;
}

// Set the database load string in the simulation context
static inline void setDatabaseLoadString(SCONTEXT_PARAMETER, char const * value)
{
    getFileInformation(simContext)->JobDbQuery = value;
}

// Set the database path on the simulation context
static inline void setDatabasePath(SCONTEXT_PARAMETER, char const * value)
{
    getFileInformation(simContext)->JobDbPath = value;
}

// Set the IO directory path on the simulation context
static inline void setIODirectoryPath(SCONTEXT_PARAMETER, char const * value)
{
    getFileInformation(simContext)->IODirectoryPath = value;
}

// Sets the path string of the output plugin on the simulation context
static inline void setOutputPluginPath(SCONTEXT_PARAMETER, char const * path)
{
    getFileInformation(simContext)->OutputPluginPath = path;
}

//  Get the main run state file target from the simulation context
static inline const char* getMainRunStateFile(SCONTEXT_PARAMETER)
{
    return getFileInformation(simContext)->MainStateFile;
}

//  Get the prerun state file target from the simulation context
static inline const char* getPreRunStateFile(SCONTEXT_PARAMETER)
{
    return getFileInformation(simContext)->PrerunStateFile;
}

// Sets the symbol string of the output plugin on the simulation context
static inline void setOutputPluginSymbol(SCONTEXT_PARAMETER, const char* symbol)
{
    getFileInformation(simContext)->OutputPluginSymbol = symbol;
}

// Sets the path string of the energy plugin on the simulation context
static inline void setEnergyPluginPath(SCONTEXT_PARAMETER, char const * path)
{
    getFileInformation(simContext)->EnergyPluginPath = path;
}

// Sets the symbol string of the energy plugin on the simulation context
static inline void setEnergyPluginSymbol(SCONTEXT_PARAMETER, const char* symbol)
{
    getFileInformation(simContext)->EnergyPluginSymbol = symbol;
}


/* Selection pool getter/setter */

// Get the environment pool entry at the specified id
static inline int32_t getEnvironmentPoolEntryAt(DirectionPool_t *restrict directionPool, const int32_t id)
{
    debug_assert(!span_IsIndexOutOfRange(directionPool->EnvironmentPool, id));
    return span_Get(directionPool->EnvironmentPool, id);
}

// Set the environment pool entry at the specified id to the given value
static inline void setEnvironmentPoolEntryAt(DirectionPool_t *restrict directionPool, const int32_t id, const int32_t value)
{
    debug_assert(!span_IsIndexOutOfRange(directionPool->EnvironmentPool, id));
    span_Get(directionPool->EnvironmentPool, id) = value;
}

/* Energy table getter/setter */

// Get the pair energy table entry from the passed table at the specified [particleId0, particleId1] combination
static inline double getPairEnergyAt(const PairTable_t *restrict table, const byte_t particleId0, const byte_t particleId1)
{
    debug_assert(!array_IsIndexOutOfRange(table->EnergyTable, particleId0, particleId1));
    return array_Get(table->EnergyTable, particleId0, particleId1);
}

// Get the cluster energy table entry from the passed table at the specified [particleId, codeId] combination
static inline double getClusterEnergyAt(const ClusterTable_t *restrict table, const byte_t particleId, const int32_t codeId)
{
    debug_assert(particleId < PARTICLE_IDLIMIT);
    debug_assert(!array_IsIndexOutOfRange(table->EnergyTable, table->ParticleTableMapping[particleId], codeId));
    return array_Get(table->EnergyTable, table->ParticleTableMapping[particleId], codeId);
}

/* Flag getter/setters */

// Get the bitmask for the main state flags
static inline Bitmask_t getMainStateFlags(SCONTEXT_PARAMETER)
{
    return getMainStateHeader(simContext)->Data->Flags;
}

// set the passed flags on the main state
static inline void setMainStateFlags(SCONTEXT_PARAMETER, const Bitmask_t flags)
{
    setFlags(getMainStateHeader(simContext)->Data->Flags, flags);
}

// Unsets the passed flags on the main state
static inline void unSetMainStateFlags(SCONTEXT_PARAMETER, const Bitmask_t flags)
{
    unsetFlags(getMainStateHeader(simContext)->Data->Flags, flags);
}

// Get the job header flags in the MMC case
static inline Bitmask_t getJobHeaderFlagsMmc(SCONTEXT_PARAMETER)
{
    return getDbModelJobHeaderAsMMC(simContext)->JobFlags;
}

// Get the job header flags in the KMC case
static inline Bitmask_t getJobHeaderFlagsKmc(SCONTEXT_PARAMETER)
{
    return getDbModelJobHeaderAsKMC(simContext)->JobFlags;
}

/* Environment getter/setter */

// Get the number of pair definitions on the passed environment state
static inline int32_t getEnvironmentPairDefinitionCount(EnvironmentState_t *restrict envState)
{
    return (int32_t) span_Length(envState->EnvironmentDefinition->PairInteractions);
}

// Get the pair definition at the passed [relPairId] from an environment state
static inline PairInteraction_t* getEnvironmentPairDefinitionAt(EnvironmentState_t *restrict envState, const int32_t relPairId)
{
    debug_assert(!span_IsIndexOutOfRange(envState->EnvironmentDefinition->PairInteractions, relPairId));
    return &span_Get(envState->EnvironmentDefinition->PairInteractions, relPairId);
}

// Get the cluster definition at the passed [relClusterId] from an environment state
static inline ClusterInteraction_t* getEnvironmentClusterDefinitionAt(EnvironmentState_t *restrict envState, const int32_t relClusterId)
{
    debug_assert(!span_IsIndexOutOfRange(envState->EnvironmentDefinition->ClusterInteractions, relClusterId));
    return &span_Get(envState->EnvironmentDefinition->ClusterInteractions, relClusterId);
}

// Get the cluster state at the passed [relClusterId] from an environment state
static inline ClusterState_t* getEnvironmentClusterStateAt(EnvironmentState_t *restrict envState, const byte_t relClusterId)
{
    debug_assert(!span_IsIndexOutOfRange(envState->ClusterStates, relClusterId));
    return &span_Get(envState->ClusterStates, relClusterId);
}

/* Active delta object getter/setter */

// Get the active state energy that belongs to the passed [particleId] from the currently set active work environment
static inline double* getActiveStateEnergyAt(SCONTEXT_PARAMETER, const byte_t particleId)
{
    debug_assert(!span_IsIndexOutOfRange(getActiveWorkEnvironment(simContext)->EnergyStates, particleId));
    return &span_Get(getActiveWorkEnvironment(simContext)->EnergyStates, particleId);
}

// Get the active particle update id by an offset id
static inline byte_t getActiveParticleUpdateIdAt(SCONTEXT_PARAMETER, const byte_t id)
{
    debug_assert(id < PARTICLE_IDLIMIT);
    return getActiveWorkEnvironment(simContext)->EnvironmentDefinition->UpdateParticleIds[id];
}

// Get an environment link by a jump link pointer. Only works if the JUMPPATH is populated accordingly
static inline EnvironmentLink_t* getEnvLinkByJumpLink(SCONTEXT_PARAMETER, const JumpLink_t* restrict jumpLink)
{
    debug_assert(jumpLink->SenderPathId < JUMPS_JUMPLENGTH_MAX);
    debug_assert(!span_IsIndexOutOfRange(JUMPPATH[jumpLink->SenderPathId]->EnvironmentLinks, jumpLink->LinkId));

    return &span_Get(JUMPPATH[jumpLink->SenderPathId]->EnvironmentLinks, jumpLink->LinkId);
}

// Gte a path state energy pointer by path id and particle id
static inline double* getPathStateEnergyByIds(SCONTEXT_PARAMETER, const byte_t pathId, const byte_t particleId)
{
    debug_assert(pathId < JUMPS_JUMPLENGTH_MAX);
    debug_assert(!span_IsIndexOutOfRange(JUMPPATH[pathId]->EnergyStates, particleId));

    return &span_Get(JUMPPATH[pathId]->EnergyStates, particleId);
}

// Get the environment state energy backup pointer by a path id
static inline double* getEnvStateEnergyBackupById(SCONTEXT_PARAMETER, const byte_t pathId)
{
    debug_assert(pathId < JUMPS_JUMPLENGTH_MAX);
    return &getCycleState(simContext)->ActiveEnvironmentBackup.PathEnergies[pathId];
}

// Get the active jump status of the simulation context
static inline JumpStatus_t* getActiveJumpStatus(SCONTEXT_PARAMETER)
{
    return getCycleState(simContext)->ActiveJumpStatus;
}