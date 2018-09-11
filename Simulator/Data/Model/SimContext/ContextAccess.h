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
#define __SCONTEXT_PAR  SimulationContext_t* restrict SCONTEXT

/* Context getter/setter */

static inline SimulationState_t* getSimulationState(__SCONTEXT_PAR)
{
    return &SCONTEXT->MainState;
}

static inline DbModel_t* getDatabaseModel(__SCONTEXT_PAR)
{
    return &SCONTEXT->DbModel;
}

static inline DynamicModel_t* getDynamicModel(__SCONTEXT_PAR)
{
    return &SCONTEXT->DynamicModel;
}

static inline JumpSelectionPool_t* getJumpSelectionPool(__SCONTEXT_PAR)
{
    return &SCONTEXT->SelectionPool;
}

static inline CycleState_t* getCycleState(__SCONTEXT_PAR)
{
    return &SCONTEXT->CycleState;
}

static inline Pcg32_t* getRandomNumberGen(__SCONTEXT_PAR)
{
    return &SCONTEXT->RandomNumberGenerator;
}

static inline SimulationPlugins_t* getPluginCollection(__SCONTEXT_PAR)
{
    return &SCONTEXT->Plugins;
}

/* Simulation model getter/setter */

static inline FileInfo_t* getFileInformation(__SCONTEXT_PAR)
{
    return &getDynamicModel(SCONTEXT)->FileInfo;
}

static inline PhysicalInfo_t* getPhysicalFactors(__SCONTEXT_PAR)
{
    return &getDynamicModel(SCONTEXT)->PhysicalFactors;
}

static inline Flp64Buffer_t* getLatticeEnergyBuffer(__SCONTEXT_PAR)
{
    return &getDynamicModel(SCONTEXT)->LatticeEnergyBuffer;
}

static inline SimulationRunInfo_t* getRuntimeInformation(__SCONTEXT_PAR)
{
    return &getDynamicModel(SCONTEXT)->RuntimeInfo;
}

static inline EnvironmentLattice* getEnvironmentLattice(__SCONTEXT_PAR)
{
    return &getDynamicModel(SCONTEXT)->EnvironmentLattice;
}

static inline void setEnvironmentLattice(__SCONTEXT_PAR, EnvironmentLattice value)
{
    *getEnvironmentLattice(SCONTEXT) = value;
}

static inline EnvironmentState_t* getEnvironmentStateById(__SCONTEXT_PAR, const int32_t id)
{
    return &getEnvironmentLattice(SCONTEXT)->Begin[id];
}

static inline EnvironmentState_t* getEnvironmentStateByVector4(__SCONTEXT_PAR, const Vector4_t* restrict vector)
{
    EnvironmentLattice* envLattice = getEnvironmentLattice(SCONTEXT);
    return &array_Get(*envLattice, vector->a, vector->b, vector->c, vector->d);
}

static inline int32_t* getLatticeBlockSizes(__SCONTEXT_PAR)
{
    return getEnvironmentLattice(SCONTEXT)->Header->Blocks;
}

/* Cycle state getter/setter */

static inline CycleCounterState_t* getMainCycleCounters(__SCONTEXT_PAR)
{
    return &getCycleState(SCONTEXT)->MainCounters;
}

static inline OccCode_t getPathStateCode(__SCONTEXT_PAR)
{
    return getCycleState(SCONTEXT)->ActiveStateCode;
}

static inline void setPathStateCode(__SCONTEXT_PAR, const OccCode_t code)
{
    getCycleState(SCONTEXT)->ActiveStateCode = code;
}

static inline JumpSelectionInfo_t* getJumpSelectionInfo(__SCONTEXT_PAR)
{
    return &getCycleState(SCONTEXT)->ActiveSelectionInfo;
}

static inline JumpEnergyInfo_t* getJumpEnergyInfo(__SCONTEXT_PAR)
{
    return &getCycleState(SCONTEXT)->ActiveEnergyInfo;
}

static inline EnvironmentBackup_t* getEnvironmentBackup(__SCONTEXT_PAR)
{
    return &getCycleState(SCONTEXT)->ActiveEnvironmentBackup;
}

static inline JumpDirection_t* getActiveJumpDirection(__SCONTEXT_PAR)
{
    return getCycleState(SCONTEXT)->ActiveJumpDirection;
}

static inline void setActiveJumpDirection(__SCONTEXT_PAR, JumpDirection_t* value)
{
    getCycleState(SCONTEXT)->ActiveJumpDirection = value;
}

static inline JumpCollection_t* getActiveJumpCollection(__SCONTEXT_PAR)
{
    return getCycleState(SCONTEXT)->ActiveJumpCollection;
}

static inline void setActiveJumpCollection(__SCONTEXT_PAR, JumpCollection_t* value)
{
    getCycleState(SCONTEXT)->ActiveJumpCollection = value;
}

static inline JumpRule_t* getActiveJumpRule(__SCONTEXT_PAR)
{
    return getCycleState(SCONTEXT)->ActiveJumpRule;
}

static inline OccCode_t getActiveStateCode(__SCONTEXT_PAR)
{
    return getCycleState(SCONTEXT)->ActiveStateCode;
}

static inline void setActiveJumpRule(__SCONTEXT_PAR, JumpRule_t* value)
{
    getCycleState(SCONTEXT)->ActiveJumpRule = value;
}

static inline StateCounterCollection_t* getActiveCounters(__SCONTEXT_PAR)
{
    return getCycleState(SCONTEXT)->ActiveCounterCollection;
}

static inline void setActiveCounters(__SCONTEXT_PAR, StateCounterCollection_t* value)
{
    getCycleState(SCONTEXT)->ActiveCounterCollection = value;
}

static inline EnvironmentState_t* getPathEnvironmentAt(__SCONTEXT_PAR, const byte_t id)
{
    return getCycleState(SCONTEXT)->ActivePathEnvironments[id]; 
}

static inline void setPathEnvironmentAt(__SCONTEXT_PAR, const byte_t id, EnvironmentState_t* value)
{
    getCycleState(SCONTEXT)->ActivePathEnvironments[id] = value;
}

static inline EnvironmentState_t* getActiveWorkEnvironment(__SCONTEXT_PAR)
{
    return getCycleState(SCONTEXT)->WorkEnvironment;
}

static inline void setActiveWorkEnvironment(__SCONTEXT_PAR, EnvironmentState_t* value)
{
    getCycleState(SCONTEXT)->WorkEnvironment = value;
}

static inline ClusterState_t* getActiveWorkCluster(__SCONTEXT_PAR)
{
    return getCycleState(SCONTEXT)->WorkCluster;
}

static inline void setActiveWorkCluster(__SCONTEXT_PAR, ClusterState_t* value)
{
    getCycleState(SCONTEXT)->WorkCluster = value;
}

static inline PairTable_t* getActivePairTable(__SCONTEXT_PAR)
{
    return getCycleState(SCONTEXT)->WorkPairTable;
}

static inline void setActivePairTable(__SCONTEXT_PAR, PairTable_t* value)
{
    getCycleState(SCONTEXT)->WorkPairTable = value;
}

static inline ClusterTable_t* getActiveClusterTable(__SCONTEXT_PAR)
{
    return getCycleState(SCONTEXT)->WorkClusterTable;
}

static inline void setActiveClusterTable(__SCONTEXT_PAR, ClusterTable_t* value)
{
    getCycleState(SCONTEXT)->WorkClusterTable = value;
}

/* Database model getter/setter */

static inline LatticeInfo_t* getLatticeInformation(__SCONTEXT_PAR)
{
    return &getDatabaseModel(SCONTEXT)->LattInfo;
}

static inline Lattice_t* getDatabaseModelLattice(__SCONTEXT_PAR)
{
    return &getLatticeInformation(SCONTEXT)->Lattice;
}

static inline JobInfo_t* getJobInformation(__SCONTEXT_PAR)
{
    return &getDatabaseModel(SCONTEXT)->JobInfo;
}

static inline KmcHeader_t* getJobHeaderAsKmc(__SCONTEXT_PAR)
{
    return (KmcHeader_t*) getJobInformation(SCONTEXT)->JobHeader;
}

static inline MmcHeader_t* getJobHeaderAsMmc(__SCONTEXT_PAR)
{
    return (MmcHeader_t*) getJobInformation(SCONTEXT)->JobHeader;
}

static inline StructureModel_t* getStructureModel(__SCONTEXT_PAR)
{
    return &getDatabaseModel(SCONTEXT)->Structure;
}

static inline EnergyModel_t* getEnergyModel(__SCONTEXT_PAR)
{
    return &getDatabaseModel(SCONTEXT)->Energy;
}

static inline TransitionModel_t* getTransitionModel(__SCONTEXT_PAR)
{
    return &getDatabaseModel(SCONTEXT)->Transition;
}

static inline EnvironmentDefinitions_t* getEnvironmentModels(__SCONTEXT_PAR)
{
    return &getStructureModel(SCONTEXT)->EnvironmentDefinitions;
}

static inline EnvironmentDefinition_t* getEnvironmentModelById(__SCONTEXT_PAR, const int32_t id)
{
    return &getEnvironmentModels(SCONTEXT)->Begin[id];
}

static inline Vector4_t* getLatticeSizeVector(__SCONTEXT_PAR)
{
    return &getLatticeInformation(SCONTEXT)->SizeVector;
}

static inline JumpCountTable_t* getJumpDirectionsPerPositionTable(__SCONTEXT_PAR)
{
    return &getTransitionModel(SCONTEXT)->JumpCountTable;
}

static inline int32_t getJumpCountByPositionStatus(__SCONTEXT_PAR, const int32_t posId, const byte_t parId)
{
    JumpCountTable_t* table = getJumpDirectionsPerPositionTable(SCONTEXT);
    return array_Get(*table, posId, parId);
}

static inline JumpAssignTable_t* getJumpIdToPositionsAssignmentTable(__SCONTEXT_PAR)
{
    return &getTransitionModel(SCONTEXT)->JumpAssignTable;
}

static inline JumpDirections_t* getJumpDirections(__SCONTEXT_PAR)
{
    return &getTransitionModel(SCONTEXT)->JumpDirections;
}

static inline JumpDirection_t* getJumpDirectionById(__SCONTEXT_PAR, const int32_t id)
{
    return &getJumpDirections(SCONTEXT)->Begin[id];
}

static inline JumpCollections_t* getJumpCollections(__SCONTEXT_PAR)
{
    return &getTransitionModel(SCONTEXT)->JumpCollections;
}

static inline JumpCollection_t* getJumpCollectionById(__SCONTEXT_PAR, const int32_t id)
{
    return &getJumpCollections(SCONTEXT)->Begin[id];
}

static inline PairTables_t* getPairEnergyTables(__SCONTEXT_PAR)
{
    return &getEnergyModel(SCONTEXT)->PairTables;
}

static inline PairTable_t* getPairEnergyTableById(__SCONTEXT_PAR, const int32_t id)
{
    return &getPairEnergyTables(SCONTEXT)->Begin[id];
}

static inline ClusterTables_t* getClusterEnergyTables(__SCONTEXT_PAR)
{
    return &getEnergyModel(SCONTEXT)->ClusterTables;
}

static inline ClusterTable_t* getClusterEnergyTableById(__SCONTEXT_PAR, const int32_t id)
{
    return &getClusterEnergyTables(SCONTEXT)->Begin[id];
}

/* Main state getter/setter */

static inline Buffer_t* getMainStateBuffer(__SCONTEXT_PAR)
{
    return &getSimulationState(SCONTEXT)->Buffer;
}

static inline void* getMainStateBufferAddress(__SCONTEXT_PAR, const int32_t offsetBytes)
{
    return &getMainStateBuffer(SCONTEXT)->Begin[offsetBytes];
}

static inline StateHeader_t* getMainStateHeader(__SCONTEXT_PAR)
{
    return &getSimulationState(SCONTEXT)->Header;
}

static inline StateMetaInfo_t* getMainStateMetaInfo(__SCONTEXT_PAR)
{
    return &getSimulationState(SCONTEXT)->Meta;
}

static inline StateMetaData_t* getMainStateMetaData(__SCONTEXT_PAR)
{
    return getMainStateMetaInfo(SCONTEXT)->Data;
}

static inline LatticeState_t* getMainStateLattice(__SCONTEXT_PAR)
{
    return &getSimulationState(SCONTEXT)->Lattice;
}

static inline byte_t getStateLatticeEntryById(__SCONTEXT_PAR, const int32_t id)
{
    return getMainStateLattice(SCONTEXT)->Begin[id];
}

static inline CountersState_t* getMainStateCounters(__SCONTEXT_PAR)
{
    return &getSimulationState(SCONTEXT)->Counters;
}

static inline StateCounterCollection_t* getMainStateCounterById(__SCONTEXT_PAR, const byte_t id)
{
    return &getMainStateCounters(SCONTEXT)->Begin[id];
}

static inline TrackersState_t* getAbstractMovementTrackers(__SCONTEXT_PAR)
{
    return &getSimulationState(SCONTEXT)->GlobalTrackers;
}

static inline TrackersState_t* getStaticMovementTrackers(__SCONTEXT_PAR)
{
    return &getSimulationState(SCONTEXT)->StaticTrackers;
}

static inline TrackersState_t* getMobileMovementTrackers(__SCONTEXT_PAR)
{
    return &getSimulationState(SCONTEXT)->MobileTrackers;
}

static inline IndexingState_t* getMobileTrackerIndexing(__SCONTEXT_PAR)
{
    return &getSimulationState(SCONTEXT)->MobileTrackerIndexing;
}

static inline ProbabilityCountMap_t* getJumpProbabilityMap(__SCONTEXT_PAR)
{
    return &getSimulationState(SCONTEXT)->ProbabilityTrackMap;
}

/* Jump selection pool getter/setter */

static inline IdRedirection_t* getDirectionPoolIndexing(__SCONTEXT_PAR)
{
    return &getJumpSelectionPool(SCONTEXT)->NumOfDirectionsToPoolId;
}

static inline void setDirectionPoolIndexing(__SCONTEXT_PAR, IdRedirection_t value)
{
    *getDirectionPoolIndexing(SCONTEXT) = value;
}

static inline int32_t getDirectionPoolIdByJumpCount(__SCONTEXT_PAR, const int32_t count)
{
    return getDirectionPoolIndexing(SCONTEXT)->Begin[count];
}

static inline void setDirectionPoolIdByJumpCount(__SCONTEXT_PAR, const int32_t count, const int32_t value)
{
    getDirectionPoolIndexing(SCONTEXT)->Begin[count] = value;
}

static inline DirectionPools_t* getDirectionPools(__SCONTEXT_PAR)
{
    return &getJumpSelectionPool(SCONTEXT)->DirectionPools;
}

static inline void setDirectionPools(__SCONTEXT_PAR, DirectionPools_t value)
{
    *getDirectionPools(SCONTEXT) = value;
}

static inline DirectionPool_t* getDirectionPoolById(__SCONTEXT_PAR, const int32_t id)
{
    return &getDirectionPools(SCONTEXT)->Begin[id];
}

static inline DirectionPool_t* getDirectionPoolByJumpCount(__SCONTEXT_PAR, const int32_t count)
{
    return getDirectionPoolById(SCONTEXT, getDirectionPoolIdByJumpCount(SCONTEXT, count));
}

/* Command arguments getter/setter */

static inline CmdArguments_t* getCommandArguments(__SCONTEXT_PAR)
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
    *getCommandArguments(SCONTEXT) = (CmdArguments_t) {  argv, argc };
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

static inline int32_t getEnvironmentPoolEntryById(DirectionPool_t* restrict dirPool, const int32_t id)
{
    return dirPool->EnvironmentPool.Begin[id];
}

static inline void setEnvironmentPoolEntryById(DirectionPool_t* restrict dirPool, const int32_t id, const int32_t value)
{
    dirPool->EnvironmentPool.Begin[id] = value;
}

/* Energy table getter/setter */

static inline double getPairEnergyTableEntry(const PairTable_t* restrict table, const byte_t parId0, const byte_t parId1)
{
    return array_Get(table->EnergyTable, parId0, parId1);
}

static inline double getCluEnergyTableEntry(const ClusterTable_t* restrict table, const byte_t parId, const int32_t codeId)
{
    return array_Get(table->EnergyTable, table->ParticleToTableId[parId], codeId);
}

/* Flag getter/setters */

static inline Bitmask_t getMainStateFlags(__SCONTEXT_PAR)
{
    return getMainStateHeader(SCONTEXT)->Data->Flags;
}

static inline void setMainStateFlags(__SCONTEXT_PAR, const Bitmask_t flags)
{
    setFlags(getMainStateHeader(SCONTEXT)->Data->Flags, flags);
}

static inline void UnsetMainStateFlags(__SCONTEXT_PAR, const Bitmask_t flags)
{
    unsetFlags(getMainStateHeader(SCONTEXT)->Data->Flags, flags);
}

static inline Bitmask_t getJobInformationFlags(__SCONTEXT_PAR)
{
    return getJobInformation(SCONTEXT)->JobFlags;
}

static inline Bitmask_t getJobHeaderFlagsKmc(__SCONTEXT_PAR)
{
    return getJobHeaderAsKmc(SCONTEXT)->JobFlags;
}

static inline Bitmask_t getJobHeaderFlagsMmc(__SCONTEXT_PAR)
{
    return getJobHeaderAsKmc(SCONTEXT)->JobFlags;
}

/* Environment getter/setter */

static inline int32_t getEnvironmentPairDefCount(EnvironmentState_t* restrict envState)
{
    return (int32_t) span_GetSize(envState->EnvironmentDefinition->PairDefinitions);
}

static inline PairDefinition_t* getEnvironmentPairDefById(EnvironmentState_t* restrict envState, const int32_t id)
{
    return &envState->EnvironmentDefinition->PairDefinitions.Begin[id];
}

static inline ClusterDefinition_t* getEnvironmentCluDefById(EnvironmentState_t* restrict envState, const int32_t id)
{
    return &envState->EnvironmentDefinition->ClusterDefinitions.Begin[id];
}

static inline ClusterState_t* getEnvironmentCluStateById(EnvironmentState_t* restrict envState, const byte_t id)
{
    return &envState->ClusterStates.Begin[id];
}

/* Active delta object getter/setter */

static inline double* getActiveStateEnergyById(__SCONTEXT_PAR, const byte_t id)
{
    return &getActiveWorkEnvironment(SCONTEXT)->EnergyStates.Begin[id];
}

static inline byte_t getActiveParticleUpdateIdAt(__SCONTEXT_PAR, const byte_t id)
{
    return getActiveWorkEnvironment(SCONTEXT)->EnvironmentDefinition->UpdateParticleIds[id];
}

static inline EnvironmentLink_t* getEnvLinkByJumpLink(__SCONTEXT_PAR, const JumpLink_t* restrict link)
{
    return &JUMPPATH[link->PathId]->EnvironmentLinks.Begin[link->LinkId];
}

static inline double* getPathStateEnergyByIds(__SCONTEXT_PAR, const byte_t pathId, const byte_t parId)
{
    return &JUMPPATH[pathId]->EnergyStates.Begin[parId];
}

static inline double* getEnvStateEnergyBackupById(__SCONTEXT_PAR, const byte_t pathId)
{
    return &getCycleState(SCONTEXT)->ActiveEnvironmentBackup.PathEnergies[pathId];
}
