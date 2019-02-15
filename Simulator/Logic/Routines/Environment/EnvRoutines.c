//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	EnvRoutines.h        		//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Env routines for simulation //
//////////////////////////////////////////

#include <math.h>
#include <string.h>
#include "Simulator/Logic/Constants/Constants.h"
#include "Simulator/Logic/Routines/Environment/EnvRoutines.h"
#include "Simulator/Logic/Routines/Helper/HelperRoutines.h"
#include "Simulator/Data/SimContext/ContextAccess.h"
#include "Framework/Basic/Macros/BinarySearch.h"

/* Local helper routines */

// Finds a cluster code id by linear searching the passed occupation code (Safe search, returns an invalid index if the code cannot be found)
static inline int32_t SaveLinearSearchClusterCodeId(const ClusterTable_t *restrict clusterTable, const OccupationCode_t occupationCode)
{
    int32_t index = 0;
    let numOfCodes = span_GetSize(clusterTable->OccupationCodes);
    while ((span_Get(clusterTable->OccupationCodes, index) != occupationCode) && (index < numOfCodes))
        index++;

    return (index < numOfCodes) ? index : INVALID_INDEX;
}

// Finds a cluster code id by linear searching the passed occupation code (Unsafe search, infinite loop if code does not exist)
static inline int32_t LinearSearchClusterCodeId(const ClusterTable_t *restrict clusterTable, const OccupationCode_t occupationCode)
{
    int32_t index = 0;
    while (span_Get(clusterTable->OccupationCodes, index++) != occupationCode);
    return index;
}

// Set the cluster state backup on the passed cluster state to the current value fields
static inline void SetClusterStateBackup(ClusterState_t* restrict cluster)
{
    cluster->CodeIdBackup = cluster->CodeId;
    cluster->OccupationCodeBackup = cluster->OccupationCode;
}

// Loads the cluster state backup on the passed cluster state into the current value fields
static inline void LoadClusterStateBackup(ClusterState_t *restrict cluster)
{
    cluster->CodeId = cluster->CodeIdBackup;
    cluster->OccupationCode = cluster->OccupationCodeBackup;
}

/* Initializer routines */

// Compares two cluster links by cluster id and code byte id
static int32_t CompareClusterLinks(const ClusterLink_t* lhs, const ClusterLink_t* rhs)
{
    int32_t value = compareLhsToRhs(lhs->ClusterId, rhs->ClusterId);
    return (value == 0) ? compareLhsToRhs(lhs->CodeByteId, rhs->CodeByteId) : value;
}

// Sorts all cluster links in the link buffer, creates the cluster link span and sets the access struct to the span
static error_t SortAndBuildClusterLinks(ClusterLink_t* restrict linkBuffer, const size_t count, ClusterLinks_t* restrict clusterLinks)
{
    let byteCount = count * sizeof(ClusterLink_t);
    *clusterLinks = new_Span(*clusterLinks, byteCount);

    qsort(linkBuffer, count, sizeof(ClusterLink_t), (FComparer_t) CompareClusterLinks);
    memcpy(clusterLinks->Begin, linkBuffer, byteCount);

    return ERR_OK;
}

// Builds a cluster linking for the provided pair interaction id in the context of the passed environment definition
static error_t BuildClusterLinkingByPairId(const EnvironmentDefinition_t* environmentDefinition, const int32_t pairId, ClusterLinks_t* restrict clusterLinks)
{
    // ToDo: Implement the limit this as a #define
    ClusterLink_t tmpLinkBuffer[sizeof(ClusterLink_t) * 256];
    byte_t clusterId = 0, relativeId = 0;
    size_t linkCount = 0;

    cpp_foreach(clusterDefinition, environmentDefinition->ClusterDefinitions)
    {
        relativeId = 0;
        c_foreach(environmentPairId, clusterDefinition->EnvironmentPairIds)
        {
            if (*environmentPairId == pairId)
            {
                tmpLinkBuffer[linkCount] = (ClusterLink_t) { clusterId , relativeId++ };
                linkCount++;
            }
        }
        clusterId++;
    }

    return SortAndBuildClusterLinks(tmpLinkBuffer, linkCount, clusterLinks);
}

// Constructs an environment link with its set of cluster links at the provided target pointer
static error_t InPlaceConstructEnvironmentLink(const EnvironmentDefinition_t* restrict environmentDefinition, const int32_t environmentId, const int32_t pairId, EnvironmentLink_t* restrict environmentLink)
{
    error_t error;

    environmentLink->EnvironmentId = environmentId;
    environmentLink->PairId = pairId;
    error = BuildClusterLinkingByPairId(environmentDefinition, pairId, &environmentLink->ClusterLinks);

    return error;
}

// Get the next environment link pointer from the target environment for in place construction of the link
static EnvironmentLink_t* GetNextLinkFromTargetEnvironment(SCONTEXT_PARAM, const PairDefinition_t *restrict pairDefinition, EnvironmentState_t *restrict environment)
{
    var targetEnvironment = GetPairDefinitionTargetEnvironment(SCONTEXT, pairDefinition, environment);

    // Immobility OPT Part 2 - Providing outgoing updates through immobiles is not required, the link will not be triggered during the mc routine
    // Sideffects:  None at this point (ref. to OPT part 1)

    #if defined(OPT_LINK_ONLY_MOBILES)
        return (targetEnvironment->IsMobile) ? targetEnvironment->EnvironmentLinks.End++ : NULL;
    #else
        return targetEnvironment->EnvironmentLinks.CurrentEnd++;
    #endif
}

// Resolves the target environment of a pair interaction and counts the link counter up by one
static void ResolvePairTargetAndIncreaseLinkCounter(SCONTEXT_PARAM, const EnvironmentState_t* restrict environment, const PairDefinition_t* restrict pairDefinition)
{
    var targetEnvironment = GetPairDefinitionTargetEnvironment(SCONTEXT, pairDefinition, environment);

    // Immobility OPT Part 1 and 2 - No incoming or outgoing updates for immobiles are required
    #if defined(OPT_LINK_ONLY_MOBILES)
        return_if(!environment->IsMobile || !targetEnvironment->IsMobile);
    #endif

    // Use the uninitialized span access struct to count the elements before allocation!
    targetEnvironment->EnvironmentLinks.End++;
}

// Sets all link counters of the environment state lattice to the required number of linkers
static error_t SetAllLinkListCountersToRequiredSize(SCONTEXT_PARAM)
{
    cpp_foreach(environment, *getEnvironmentLattice(SCONTEXT))
        cpp_foreach(pairDefinition, environment->EnvironmentDefinition->PairDefinitions)
            ResolvePairTargetAndIncreaseLinkCounter(SCONTEXT, environment, pairDefinition);

    return ERR_OK;
}

// Allocates the environment linker lists to the size defined by their previously set counter status
static error_t AllocateEnvLinkListBuffersByPresetCounters(SCONTEXT_PARAM)
{
    EnvironmentLinks_t tmpBuffer;

    cpp_foreach(environment, *getEnvironmentLattice(SCONTEXT))
    {
        // Link is counted using the NULL initialized span access struct
        let linkCount = span_GetSize(environment->EnvironmentLinks);
        tmpBuffer = new_List(tmpBuffer, linkCount);
        environment->EnvironmentLinks = tmpBuffer;
    }

    return ERR_OK;
}


// Prepares the linking system construction by counting and allocation the required space for the system
static error_t PrepareLinkingSystemConstruction(SCONTEXT_PARAM)
{
    error_t error;

    // Note: This function uses the NULL initialized span pointers to actually count the required links!
    error = SetAllLinkListCountersToRequiredSize(SCONTEXT);
    return_if(error, error);

    error = AllocateEnvLinkListBuffersByPresetCounters(SCONTEXT);
    return error;
}

// Links an environment to its surroundings by sending a link to each one that requires one
static error_t LinkEnvironmentToSurroundings(SCONTEXT_PARAM, EnvironmentState_t* restrict environment)
{
    error_t error;
    int32_t pairId = 0;

    cpp_foreach(pairDefinition, environment->EnvironmentDefinition->PairDefinitions)
    {
        var environmentLink = GetNextLinkFromTargetEnvironment(SCONTEXT, pairDefinition, environment);
        if (environmentLink != NULL)
        {
            error = InPlaceConstructEnvironmentLink(environment->EnvironmentDefinition, environment->EnvironmentId, pairId, environmentLink);
            return_if(error, error);
        }
        pairId++;
    }

    return ERR_OK;
}

// Compares two environment links by their affiliated pair id
static inline int32_t CompareEnvironmentLink(const EnvironmentLink_t* restrict lhs, const EnvironmentLink_t* restrict rhs)
{
    return compareLhsToRhs(lhs->PairId, rhs->PairId);
}

// Sort the linking system of an environment state to the unit cell independent order
static void SortEnvironmentLinkingSystem(SCONTEXT_PARAM, EnvironmentState_t* environment)
{
    var sortBase = environment->EnvironmentLinks.Begin;
    let elementCount = span_GetSize(environment->EnvironmentLinks);
    qsort(sortBase, elementCount, sizeof(EnvironmentLink_t), (FComparer_t) CompareEnvironmentLink);
}

// Constructs the prepared linking system by linking all environments and sorting the linkers to the required order
static error_t ConstructPreparedLinkingSystem(SCONTEXT_PARAM)
{
    error_t error;

    cpp_foreach(environment, *getEnvironmentLattice(SCONTEXT))
    {
        // Immobility OPT Part 1 -> Incoming updates are not required, the state energy of immobile particles is not used during mc routine
        // Effect:    Causes all immobile particles to remain at their initial energy state during simulation (can be resynchronized by dynamic lookup)
        #if defined(OPT_LINK_ONLY_MOBILES)
            continue_if(!environment->IsMobile);
        #endif

        error = LinkEnvironmentToSurroundings(SCONTEXT, environment);
        return_if(error, error);
    }

    cpp_foreach(environment, *getEnvironmentLattice(SCONTEXT))
        SortEnvironmentLinkingSystem(SCONTEXT, environment);

    return ERR_OK;
}

// Builds the environment linking system of the environment state lattice
void BuildEnvironmentLinkingSystem(SCONTEXT_PARAM)
{
    error_t error;

    error = PrepareLinkingSystemConstruction(SCONTEXT);
    error_assert(error, "Failed to prepare the environment linking system for construction.");

    error = ConstructPreparedLinkingSystem(SCONTEXT);
    error_assert(error, "Failed to construct the environment linking system.");
}

// Allocates the dynamic environment occupation buffer for dynamic lookup of environment occupations (Size fits the largest environment definition)
static error_t AllocateDynamicEnvOccupationBuffer(SCONTEXT_PARAM, Buffer_t* restrict buffer)
{
    size_t bufferSize = 0;

    cpp_foreach(environmentDefinition, *getEnvironmentModels(SCONTEXT))
        bufferSize = getMaxOfTwo(bufferSize, span_GetSize(environmentDefinition->PairDefinitions));

    *buffer = new_Span(*buffer, bufferSize);
    return ERR_OK;
}

// Find an environment state by resolving the passed pair id in the context of the start environment state
static EnvironmentState_t* PullEnvStateByInteraction(SCONTEXT_PARAM, EnvironmentState_t* restrict startEnvironment, const int32_t pairId)
{
    let pairDefinition = getEnvironmentPairDefinitionAt(startEnvironment, pairId);
    return GetPairDefinitionTargetEnvironment(SCONTEXT, pairDefinition, startEnvironment);
}

// Writes the current environment occupation of the passed environment state to the passed occupation buffer
static error_t WriteEnvOccupationToBuffer(SCONTEXT_PARAM, EnvironmentState_t* environment, Buffer_t* restrict occupationBuffer)
{
    for (int32_t i = 0; i < getEnvironmentPairDefinitionCount(environment); i++)
    {
        span_Get(*occupationBuffer, i) = PullEnvStateByInteraction(SCONTEXT, environment, i)->ParticleId;
        return_if(span_Get(*occupationBuffer,i) == PARTICLE_VOID, ERR_DATACONSISTENCY);
    }

    return ERR_OK;
}

// Resets all environment state buffers entries to a value of zero
static void NullEnvironmentStateBuffers(EnvironmentState_t *restrict environment)
{
    memset(environment->EnergyStates.Begin, 0, span_ByteCount(environment->EnergyStates));
    memset(environment->ClusterStates.Begin, 0, span_ByteCount(environment->ClusterStates));
}

// Adds all environment pair energies of the passed environment state to its internal energy buffers using the passed occupation buffer as the occupation source
static void AddEnvPairEnergyByOccupation(SCONTEXT_PARAM, EnvironmentState_t* restrict environment, Buffer_t* restrict occupationBuffer)
{
    for (size_t i = 0; i < span_GetSize(environment->EnvironmentDefinition->PairDefinitions); i++)
    {
        let tableId = span_Get(environment->EnvironmentDefinition->PairDefinitions, i).EnergyTableId;
        let pairTable = getPairEnergyTableAt(SCONTEXT, tableId);
        for (size_t j = 0; environment->EnvironmentDefinition->PositionParticleIds[j] != PARTICLE_NULL; j++)
        {
            let positionParticleId = environment->EnvironmentDefinition->PositionParticleIds[j];
            let partnerParticleId = span_Get(*occupationBuffer, i);
            let energy = getPairEnergyAt(pairTable, positionParticleId, partnerParticleId);
            span_Get(environment->EnergyStates, positionParticleId) += energy;
        }
    } 
}

// Initializes the passed cluster state status (code id) and sets the backups to the current values
static error_t InitializeClusterStateStatus(SCONTEXT_PARAM, ClusterState_t* restrict cluster, const ClusterTable_t* restrict clusterTable)
{
    cluster->CodeId = SaveLinearSearchClusterCodeId(clusterTable, cluster->OccupationCode);
    return_if(cluster->CodeId == INVALID_INDEX, ERR_DATACONSISTENCY);

    SetClusterStateBackup(cluster);
    return ERR_OK;
}

// Synchronizes the all cluster states of the passed environment state and adds the resulting energies to the environment state energy buffer
static error_t AddEnvClusterEnergyByOccupation(SCONTEXT_PARAM, EnvironmentState_t* restrict environment, Buffer_t* restrict occupationBuffer)
{
    return_if(span_GetSize(environment->ClusterStates) != span_GetSize(environment->EnvironmentDefinition->ClusterDefinitions), ERR_DATACONSISTENCY);

    error_t error;
    var clusterState = environment->ClusterStates.Begin;
    let clusterStateEnd = environment->ClusterStates.End;

    cpp_foreach(clusterDefinition, environment->EnvironmentDefinition->ClusterDefinitions)
    {
        return_if(clusterState == clusterStateEnd, ERR_DATACONSISTENCY);
        let clusterTable = getClusterEnergyTableAt(SCONTEXT, clusterDefinition->EnergyTableId);
        for (byte_t i = 0; clusterDefinition->EnvironmentPairIds[i] != POSITION_NULL; i++)
        {
            let codeByteId = clusterDefinition->EnvironmentPairIds[i];
            SetCodeByteAt(&clusterState->OccupationCode, i, span_Get(*occupationBuffer, codeByteId));
        }
        error = InitializeClusterStateStatus(SCONTEXT, clusterState, clusterTable);
        return_if(error != ERR_OK, ERR_DATACONSISTENCY);

        for (int32_t j = 0; environment->EnvironmentDefinition->PositionParticleIds[j] != PARTICLE_NULL; j++)
        {
            let positionParticleId = environment->EnvironmentDefinition->PositionParticleIds[j];
            let energy = getClusterEnergyAt(clusterTable, positionParticleId, clusterState->CodeId);
            span_Get(environment->EnergyStates, positionParticleId) += energy;
        }
        ++clusterState;
    }

    return ERR_OK;
}

// Sets the environment state energy buffers to the value that results from the passed occupation buffer entries
static error_t SetEnvStateEnergyByOccupation(SCONTEXT_PARAM, EnvironmentState_t* restrict environment, Buffer_t* restrict occupationBuffer)
{
    NullEnvironmentStateBuffers(environment);
    AddEnvPairEnergyByOccupation(SCONTEXT, environment, occupationBuffer);
    return AddEnvClusterEnergyByOccupation(SCONTEXT, environment, occupationBuffer);
}

// Dynamically calculates the environment status (energies and cluster states) of the passed environment id using the provided occupation buffer
static error_t DynamicLookupEnvironmentStatus(SCONTEXT_PARAM, const int32_t environmentId, Buffer_t* restrict occupationBuffer)
{
    error_t error;
    var environment = getEnvironmentStateAt(SCONTEXT, environmentId);

    error = WriteEnvOccupationToBuffer(SCONTEXT, environment, occupationBuffer);
    return_if(error, error);

    error = SetEnvStateEnergyByOccupation(SCONTEXT, environment, occupationBuffer);
    return_if(error, error);

    return ERR_OK;
}

// Dynamically synchronizes the environment lattice energy status to the current status (Cluster states and energy states)
// and sets the current energy value on the main state
// Resynchronizes potential lattice energy status errors caused by linking system optimization
void ResynchronizeEnvironmentEnergyStatus(SCONTEXT_PARAM)
{
    error_t error;
    double energy = 0;
    Buffer_t occupationBuffer;
    var metaData = getMainStateMetaData(SCONTEXT);
    let physicalFactors = getPhysicalFactors(SCONTEXT);

    error = AllocateDynamicEnvOccupationBuffer(SCONTEXT, &occupationBuffer);
    error_assert(error, "Buffer creation for environment occupation lookup failed.");

    cpp_foreach (envState, *getEnvironmentLattice(SCONTEXT))
    {
        error = DynamicLookupEnvironmentStatus(SCONTEXT, envState->EnvironmentId, &occupationBuffer);
        error_assert(error, "Dynamic lookup of environment occupation and energy failed.");
        energy += GetEnvironmentStateEnergy(envState);
    }
    metaData->LatticeEnergy = energy * physicalFactors->EnergyFactorKtToEv;
    delete_Span(occupationBuffer);
}

// Sets the status of the environment state with the passed id to the default status using the passed occupation particle id
void SetEnvironmentStateToDefault(SCONTEXT_PARAM, const int32_t environmentId, const byte_t particleId)
{
    var environment = getEnvironmentStateAt(SCONTEXT, environmentId);
    environment->ParticleId = particleId;
    environment->EnvironmentId = environmentId;
    environment->IsMobile = false;
    environment->IsStable = (particleId == PARTICLE_VOID) ? false : true;
    environment->PositionVector = Vector4FromInt32(environmentId, getLatticeBlockSizes(SCONTEXT));
    environment->EnvironmentDefinition = getEnvironmentModelAt(SCONTEXT, environment->PositionVector.D);
    environment->MobileTrackerId = INVALID_INDEX;
}

/* Simulation routines KMC and MMC */

// Sets the active work environment by an environment link
static inline void SetActiveWorkEnvironment(SCONTEXT_PARAM, EnvironmentLink_t *restrict environmentLink)
{
    SCONTEXT->CycleState.WorkEnvironment = getEnvironmentStateAt(SCONTEXT, environmentLink->EnvironmentId);
}

// Sets the active work cluster by environment and cluster id
static inline void SetActiveWorkCluster(SCONTEXT_PARAM, EnvironmentState_t *restrict environment, const byte_t clusterId)
{
    SCONTEXT->CycleState.WorkCluster = getEnvironmentClusterStateAt(environment, clusterId);
}

// Sets the active work pair energy table by environment and environment link
static inline void SetActiveWorkPairTable(SCONTEXT_PARAM, EnvironmentState_t *restrict environment, EnvironmentLink_t *restrict environmentLink)
{
    let pairDefinition = getEnvironmentPairDefinitionAt(environment, environmentLink->PairId);
    SCONTEXT->CycleState.WorkPairTable = getPairEnergyTableAt(SCONTEXT, pairDefinition->EnergyTableId);
}

// Set the active work cluster energy table by environment and cluster link
static inline void SetActiveWorkClusterTable(SCONTEXT_PARAM, EnvironmentState_t *restrict environment, ClusterLink_t *restrict clusterLink)
{
    let clusterDefinition = getEnvironmentClusterDefinitionAt(environment, clusterLink->ClusterId);
    SCONTEXT->CycleState.WorkClusterTable = getClusterEnergyTableAt(SCONTEXT, clusterDefinition->EnergyTableId);
}

// Finds a cluster code ID in a cluster table
static inline int32_t SearchClusterCodeIdInTable(const ClusterTable_t *restrict clusterTable,const OccupationCode_t code)
{
    return LinearSearchClusterCodeId(clusterTable, code);
}

// Get the delta energy value for the passed pair information
static inline double GetPairEnergyDelta(const PairTable_t *restrict pairTable, const byte_t mainId, const byte_t oldId,const byte_t newId)
{
    let oldEnergy = getPairEnergyAt(pairTable, mainId, oldId);
    let newEnergy = getPairEnergyAt(pairTable, mainId, newId);
    return newEnergy - oldEnergy;
}

// Get the cluster delta energy for the passed cluster information
static inline double GetClusterEnergyDelta(const ClusterTable_t *restrict clusterTable, const ClusterState_t *restrict cluster, const byte_t particleId)
{
    let oldEnergy = getClusterEnergyAt(clusterTable, particleId, cluster->CodeIdBackup);
    let newEnergy = getClusterEnergyAt(clusterTable, particleId, cluster->CodeId);
    return newEnergy - oldEnergy;
}

// Updates the cluster state to a new particle id using the provided cluster link
static inline void UpdateClusterState(const ClusterTable_t* restrict clusterTable, const ClusterLink_t* restrict clusterLink, ClusterState_t* restrict cluster, const byte_t newParticleId)
{
    SetCodeByteAt(&cluster->OccupationCode, clusterLink->CodeByteId, newParticleId);
    cluster->CodeId = SearchClusterCodeIdInTable(clusterTable, cluster->OccupationCode);
}

// Invokes the currently resulting pair energy delta of the work object status
static inline void InvokeDeltaOfActivePair(SCONTEXT_PARAM, const byte_t updateParticleId, const byte_t oldParticleId, const byte_t newParticleId)
{
    let table = getActivePairTable(SCONTEXT);
    let delta = GetPairEnergyDelta(table, updateParticleId, oldParticleId, newParticleId);
    *getActiveStateEnergyAt(SCONTEXT, updateParticleId) += delta;
}

// Invokes the currently resulting cluster energy delta of the work object status
static inline void InvokeDeltaOfActiveCluster(SCONTEXT_PARAM, const byte_t updateParticleId)
{
    let table = getActiveClusterTable(SCONTEXT);
    let cluster = getActiveWorkCluster(SCONTEXT);
    let delta = GetClusterEnergyDelta(table, cluster, updateParticleId);
    *getActiveStateEnergyAt(SCONTEXT, updateParticleId) += delta;
}

// Invokes all changes on the cluster set of the passed environment link
static void InvokeEnvironmentLinkClusterUpdates(SCONTEXT_PARAM, const EnvironmentLink_t *restrict environmentLink, const byte_t newParticleId)
{
    let workEnvironment = getActiveWorkEnvironment(SCONTEXT);
    cpp_foreach(clusterLink, environmentLink->ClusterLinks)
    {
        SetActiveWorkCluster(SCONTEXT, workEnvironment, clusterLink->ClusterId);
        SetActiveWorkClusterTable(SCONTEXT, workEnvironment, clusterLink);
        let clusterTable = getActiveClusterTable(SCONTEXT);
        let workCluster = getActiveWorkCluster(SCONTEXT);

        UpdateClusterState(clusterTable, clusterLink, workCluster, newParticleId);
        for (byte_t i = 0; getActiveParticleUpdateIdAt(SCONTEXT, i) != PARTICLE_NULL; i++)
            InvokeDeltaOfActiveCluster(SCONTEXT, i);

        SetClusterStateBackup(getActiveWorkCluster(SCONTEXT));
    }
}

// Invokes all link updates defined on the passed environment link with the passed particle information
static void InvokeEnvironmentLinkUpdates(SCONTEXT_PARAM, const EnvironmentLink_t *restrict environmentLink, const byte_t oldParticleId, const byte_t newParticleId)
{
    for (byte_t i = 0; getActiveParticleUpdateIdAt(SCONTEXT, i) != PARTICLE_NULL; i++)
        InvokeDeltaOfActivePair(SCONTEXT, i, oldParticleId, newParticleId);

    InvokeEnvironmentLinkClusterUpdates(SCONTEXT, environmentLink, newParticleId);
}

// Invokes the currently set active work pair for local delta data with the provided particle information
static inline void InvokeActiveLocalPairDelta(SCONTEXT_PARAM, const byte_t updateParticleId, const byte_t oldParticleId, const byte_t newParticleId)
{
    InvokeDeltaOfActivePair(SCONTEXT, updateParticleId, oldParticleId, newParticleId);
}

// Invokes the cluster changes of the passed environment link for local delta data with the provided particle information
static inline void InvokeLocalEnvironmentLinkClusterDeltas(SCONTEXT_PARAM, const EnvironmentLink_t *restrict environmentLink, const byte_t updateParticleId)
{
    let workEnvironment = getActiveWorkEnvironment(SCONTEXT);
    cpp_foreach(clusterLink, environmentLink->ClusterLinks)
    {
        SetActiveWorkCluster(SCONTEXT, workEnvironment, clusterLink->ClusterId);
        let workCluster = getActiveWorkCluster(SCONTEXT);
        if (workCluster->OccupationCode != workCluster->OccupationCodeBackup)
        {
            SetActiveWorkClusterTable(SCONTEXT, workEnvironment, clusterLink);
            InvokeDeltaOfActiveCluster(SCONTEXT, updateParticleId);
            LoadClusterStateBackup(getActiveWorkCluster(SCONTEXT));
        }
    }
}

// Prepares all jump link cluster changes for evaluation of the local delta generation
static inline void PrepareJumpLinkClusterStateChanges(SCONTEXT_PARAM, const JumpLink_t* restrict jumpLink)
{
    let environmentLink = getEnvLinkByJumpLink(SCONTEXT, jumpLink);
    SetActiveWorkEnvironment(SCONTEXT, environmentLink);

    let workEnvironment = getActiveWorkEnvironment(SCONTEXT);
    var jumpRule = getActiveJumpRule(SCONTEXT);
    cpp_foreach(clusterLink, environmentLink->ClusterLinks)
    {
        let newCodeByte = GetCodeByteAt(&jumpRule->StateCode2, jumpLink->SenderPathId);
        SetActiveWorkCluster(SCONTEXT, workEnvironment, clusterLink->ClusterId);
        var workCluster = getActiveWorkCluster(SCONTEXT);
        SetCodeByteAt(&workCluster->OccupationCode, clusterLink->CodeByteId, newCodeByte);
    }
}

// Invoke the local state delta that results from the passed jump link
static void InvokeJumpLinkDeltas(SCONTEXT_PARAM, const JumpLink_t* restrict jumpLink)
{
    let environmentLink = getEnvLinkByJumpLink(SCONTEXT, jumpLink);
    let workEnvironment = getActiveWorkEnvironment(SCONTEXT);
    let jumpRule = getActiveJumpRule(SCONTEXT);

    SetActiveWorkEnvironment(SCONTEXT, environmentLink);
    SetActiveWorkPairTable(SCONTEXT, workEnvironment, environmentLink);

    let newParticleId = GetCodeByteAt(&jumpRule->StateCode2, jumpLink->SenderPathId);
    let updateParticleId = GetCodeByteAt(&jumpRule->StateCode2, getActiveWorkEnvironment(SCONTEXT)->PathId);

    InvokeActiveLocalPairDelta(SCONTEXT, updateParticleId, JUMPPATH[jumpLink->SenderPathId]->ParticleId, newParticleId);
    InvokeLocalEnvironmentLinkClusterDeltas(SCONTEXT, environmentLink, updateParticleId);
}

// Distributes the update of the particle state of an environment to all linked environments
static void DistributeEnvironmentUpdate(SCONTEXT_PARAM, EnvironmentState_t *restrict environment, const byte_t newParticleId)
{
    cpp_foreach(environmentLink, environment->EnvironmentLinks)
    {
        SetActiveWorkEnvironment(SCONTEXT, environmentLink);
        let workEnvironment = getActiveWorkEnvironment(SCONTEXT);
        SetActiveWorkPairTable(SCONTEXT, workEnvironment, environmentLink);
        InvokeEnvironmentLinkUpdates(SCONTEXT, environmentLink, environment->ParticleId, newParticleId);
    }
}

// Writes the state energy entry that is subjected to jump link changes to the environment energy backup buffer
static inline void SetFinalStateEnergyBackup(SCONTEXT_PARAM, const byte_t pathId)
{
    let jumpRule = getActiveJumpRule(SCONTEXT);
    let updateParticleId = GetCodeByteAt(&jumpRule->StateCode2, pathId);
    *getEnvStateEnergyBackupById(SCONTEXT, pathId) = *getPathStateEnergyByIds(SCONTEXT, pathId, updateParticleId);
}

// Loads the state energy entry that was subjected to jump link changes from the environment energy backup buffer
static inline void LoadFinalStateEnergyBackup(SCONTEXT_PARAM, const byte_t pathId)
{
    let stateCode = &getActiveJumpRule(SCONTEXT)->StateCode2;
    let updateParticleId = GetCodeByteAt(stateCode, pathId);
    *getPathStateEnergyByIds(SCONTEXT, pathId, updateParticleId) = *getEnvStateEnergyBackupById(SCONTEXT, pathId);
}

/* Simulation sub routines */

void KMC_CreateBackupAndJumpDelta(SCONTEXT_PARAM)
{
    let jumpStatus = getActiveJumpStatus(SCONTEXT);
    let JumpDirection = getActiveJumpDirection(SCONTEXT);

    // Backup all required energy states
    for (byte_t i = 0; i < JumpDirection->JumpLength; ++i)
        SetFinalStateEnergyBackup(SCONTEXT, i);

    // Prepare the cluster state changes to avoid multiple code lookups
    cpp_foreach(jumpLink, jumpStatus->JumpLinks)
        PrepareJumpLinkClusterStateChanges(SCONTEXT, jumpLink);

    // Invoke the local pair and prepared cluster deltas
    cpp_foreach(jumpLink, jumpStatus->JumpLinks)
        InvokeJumpLinkDeltas(SCONTEXT, jumpLink);
}

void KMC_LoadJumpDeltaBackup(SCONTEXT_PARAM)
{
    let jumpDirection = getActiveJumpDirection(SCONTEXT);
    for(byte_t i = 0; i < jumpDirection->JumpLength; i++)
        LoadFinalStateEnergyBackup(SCONTEXT, i);
}

void KMC_SetStateEnergies(SCONTEXT_PARAM)
{
    KMC_SetStartAndTransitionStateEnergies(SCONTEXT);
    KMC_CreateBackupAndJumpDelta(SCONTEXT);
    KMC_SetFinalStateEnergy(SCONTEXT);
    KMC_LoadJumpDeltaBackup(SCONTEXT);
}

void KMC_SetStartAndTransitionStateEnergies(SCONTEXT_PARAM)
{
    let jumpDirection = getActiveJumpDirection(SCONTEXT);
    let jumpRule = getActiveJumpRule(SCONTEXT);
    var energyInfo = getJumpEnergyInfo(SCONTEXT);
    var particleId = GetCodeByteAt(&jumpRule->StateCode0, 0);

    // Set the values of the first entry, the first transition state energy is always zero
    energyInfo->Energy0 = *getPathStateEnergyByIds(SCONTEXT, 0, particleId);
    energyInfo->Energy1 = 0;
    // Add all remaining components to start and transition state
    for (byte_t i = 1; i < jumpDirection->JumpLength;i++)
    {
            // ToDo: Verify that leaving out the stability check branching performs better
            particleId = GetCodeByteAt(&jumpRule->StateCode0, i);
            energyInfo->Energy0 += *getPathStateEnergyByIds(SCONTEXT, i, particleId);
            particleId = GetCodeByteAt(&jumpRule->StateCode1, i);
            energyInfo->Energy1 += *getPathStateEnergyByIds(SCONTEXT, i, particleId);
    }
}

void KMC_SetFinalStateEnergy(SCONTEXT_PARAM)
{
    let jumpRule = getActiveJumpRule(SCONTEXT);
    let jumpDirection = getActiveJumpDirection(SCONTEXT);
    var energyInfo = getJumpEnergyInfo(SCONTEXT);
    var particleId = GetCodeByteAt(&jumpRule->StateCode2, 0);

    // Set the values of the first entry
    energyInfo->Energy2 = *getPathStateEnergyByIds(SCONTEXT, 0, particleId);

    for(byte_t i = 1; i < jumpDirection->JumpLength;i++)
    {
        particleId = GetCodeByteAt(&jumpRule->StateCode2, i);
        energyInfo->Energy2 += *getPathStateEnergyByIds(SCONTEXT, i, particleId);
    }
}

void KMC_AdvanceSystemToFinalState(SCONTEXT_PARAM)
{
    let stateCode = &getActiveJumpRule(SCONTEXT)->StateCode2;
    let jumpDirection = getActiveJumpDirection(SCONTEXT);

    for(byte_t i = 0; i < jumpDirection->JumpLength; i++)
    {
        let newParticleId = GetCodeByteAt(stateCode, i);
        DistributeEnvironmentUpdate(SCONTEXT, JUMPPATH[i], newParticleId);
        JUMPPATH[i]->ParticleId = newParticleId;
    }
}

// Searches the passed environment state link collection for a link to the passed environment id and builds a matching jump link object
static inline JumpLink_t MMC_BuildJumpLink(const EnvironmentState_t *restrict envState, const int32_t targetEnvId)
{
    var result = (JumpLink_t) { .SenderPathId = envState->PathId, .LinkId = 0 };
    cpp_foreach(envLink, envState->EnvironmentLinks)
    {
        return_if(envLink->EnvironmentId == targetEnvId, result);
        ++result.LinkId;
    }
    return (JumpLink_t){ .SenderPathId = INVALID_INDEX, .LinkId = INVALID_INDEX };
}

bool_t MMC_TryCreateBackupAndJumpDelta(SCONTEXT_PARAM)
{
    // Check if the positions are potentially close enough to be linked
    return_if(!PositionAreInInteractionRange(SCONTEXT, &JUMPPATH[0]->PositionVector, &JUMPPATH[1]->PositionVector), false);

    // Find the required environment links and build matching temporary jump links if they exist
    // If the first is not found the second can by definition not exist as well
    let path0JumpLink = MMC_BuildJumpLink(JUMPPATH[0], JUMPPATH[1]->EnvironmentId);
    return_if(path0JumpLink.LinkId == INVALID_INDEX, false);

    let path1JumpLink = MMC_BuildJumpLink(JUMPPATH[1], JUMPPATH[0]->EnvironmentId);

    // Backup the final state energies
    SetFinalStateEnergyBackup(SCONTEXT, 0);
    SetFinalStateEnergyBackup(SCONTEXT, 1);

    // Prepare the potential cluster state changes on both environments and invoke the link deltas
    PrepareJumpLinkClusterStateChanges(SCONTEXT, &path0JumpLink);
    PrepareJumpLinkClusterStateChanges(SCONTEXT, &path1JumpLink);
    InvokeJumpLinkDeltas(SCONTEXT, &path0JumpLink);
    InvokeJumpLinkDeltas(SCONTEXT, &path1JumpLink);
    return true;
}

void MMC_LoadJumpDeltaBackup(SCONTEXT_PARAM)
{
    LoadFinalStateEnergyBackup(SCONTEXT, 0);
    LoadFinalStateEnergyBackup(SCONTEXT, 1);
}


void MMC_SetStateEnergies(SCONTEXT_PARAM)
{
    MMC_SetStartStateEnergy(SCONTEXT);

    // Try to create a backup if required, else the positions do not interact and the final energy can be directly set
    if (MMC_TryCreateBackupAndJumpDelta(SCONTEXT))
    {
        MMC_SetFinalStateEnergy(SCONTEXT);
        MMC_LoadJumpDeltaBackup(SCONTEXT);
        return;
    }
    MMC_SetFinalStateEnergy(SCONTEXT);
}


void MMC_SetStartStateEnergy(SCONTEXT_PARAM)
{
    let jumpRule = getActiveJumpRule(SCONTEXT);
    var jumpEnergyInfo = getJumpEnergyInfo(SCONTEXT);

    let particleId0 = GetCodeByteAt(&jumpRule->StateCode0, 0);
    jumpEnergyInfo->Energy0 =  *getPathStateEnergyByIds(SCONTEXT, 0, particleId0);
    let particleId1 = GetCodeByteAt(&jumpRule->StateCode0, 1);
    jumpEnergyInfo->Energy0 += *getPathStateEnergyByIds(SCONTEXT, 1, particleId1);
}

void MMC_SetFinalStateEnergy(SCONTEXT_PARAM)
{
    let jumpRule = getActiveJumpRule(SCONTEXT);
    var jumpEnergyInfo = getJumpEnergyInfo(SCONTEXT);

    let particleId0 = GetCodeByteAt(&jumpRule->StateCode2, 0);
    jumpEnergyInfo->Energy2 =  *getPathStateEnergyByIds(SCONTEXT, 0, particleId0);
    let particleId1 = GetCodeByteAt(&jumpRule->StateCode2, 1);
    jumpEnergyInfo->Energy2 += *getPathStateEnergyByIds(SCONTEXT, 1, particleId1);
}

void MMC_AdvanceSystemToFinalState(SCONTEXT_PARAM)
{
    let jumpRule = getActiveJumpRule(SCONTEXT);
    let newParticleId0 = GetCodeByteAt(&jumpRule->StateCode2, 0);
    let newParticleId1 = GetCodeByteAt(&jumpRule->StateCode2, 1);

    DistributeEnvironmentUpdate(SCONTEXT, JUMPPATH[0], newParticleId0);
    JUMPPATH[0]->ParticleId = newParticleId0;
    DistributeEnvironmentUpdate(SCONTEXT, JUMPPATH[1], newParticleId1);
    JUMPPATH[1]->ParticleId = newParticleId1;
}