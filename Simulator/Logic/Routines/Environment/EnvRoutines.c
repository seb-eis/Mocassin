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
#include "Simulator/Logic/Constants/Constants.h"
#include "Simulator/Logic/Routines/Environment/EnvRoutines.h"
#include "Simulator/Logic/Routines/Helper/HelperRoutines.h"
#include "Simulator/Data/SimContext/ContextAccess.h"
#include "Framework/Basic/Macros/BinarySearch.h"

/* Local helper routines */

// Finds a cluster code id by linear searching the passed occupation code (Safe search, returns an invalid index if the code cannot be found)
static inline int32_t SaveFindClusterCodeIdByLinearSearch(const ClusterTable_t* restrict clusterTable, const OccCode_t occupationCode)
{
    int32_t index = 0;
    size_t numOfCodes = span_GetSize(clusterTable->OccupationCodes);
    while ((span_Get(clusterTable->OccupationCodes, index) != occupationCode) && (index < numOfCodes))
    {
        index++;
    }
    return (index < numOfCodes) ? index : INVALID_INDEX;
}

// Finds a cluster code id by linear searching the passed occupation code (Unsafe search, infinite loop if code does not exist)
static inline int32_t FindClusterCodeIdByLinearSearch(const ClusterTable_t* restrict clusterTable, const OccCode_t occupationCode)
{
    int32_t index = 0;
    while (span_Get(clusterTable->OccupationCodes, index++) != occupationCode) {};
    return index;
}

//static inline int32_t BinaryLookupCluCodeId(const ClusterTable_t* restrict clusterTable, const OccCode_t occupationCode)
//{
//    // Placeholder, implement on optimization
//    return -1;
//}

// Set the cluster state backup on the passed cluster state to the current value fields
static inline void SetCluStateBackup(ClusterState_t* restrict cluster)
{
    cluster->CodeIdBackup = cluster->CodeId;
    cluster->OccupationCodeBackup = cluster->OccupationCode;
}

// Loads the cluster state backup on the passed cluster state into the current value fields
static inline void LoadCluStateBackup(ClusterState_t* restrict cluster)
{
    cluster->CodeId = cluster->CodeIdBackup;
    cluster->OccupationCode = cluster->OccupationCodeBackup;
}

/* Initializer routines */

// Compares two cluster links by cluster id and code byte id
static int32_t CompareClusterLinks(const ClusterLink_t* lhs, const ClusterLink_t* rhs)
{
    int32_t value = compareLhsToRhs(lhs->ClusterId, rhs->ClusterId);
    if (value == 0)
    {
        return compareLhsToRhs(lhs->CodeByteId, rhs->CodeByteId);
    }
    return value;
}

// Sorts all cluster links in the link buffer, creates the cluster link span and sets the access struct to the span
static error_t SortAndBuildClusterLinks(ClusterLink_t* restrict linkBuffer, const size_t count, ClusterLinks_t* restrict clusterLinks)
{
    error_t error;
    Buffer_t tmpBuffer;

    error = ctor_Buffer(tmpBuffer, count* sizeof(ClusterLink_t));
    return_if(error, error);

    qsort(linkBuffer, count, sizeof(ClusterLink_t), (FComparer_t) CompareClusterLinks);

    CopyBuffer((byte_t*) linkBuffer, tmpBuffer.Begin, sizeof(ClusterLink_t) * count);
    *clusterLinks = (ClusterLinks_t) span_AsVoid(tmpBuffer);

    return ERR_OK;
}

// Builds a cluster linking for the provided pair interaction id in the context of the passed environment definition
static error_t BuildClusterLinkingByPairId(const EnvironmentDefinition_t* environmentDefinition, const int32_t pairId, ClusterLinks_t* restrict clusterLinks)
{
    ClusterLink_t tmpLinkBuffer[sizeof(ClusterLink_t) * 256]; // There are a max of 256 clusters a single pair could belong to
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
static EnvironmentLink_t* GetNextLinkFromTargetEnv(__SCONTEXT_PAR, const PairDefinition_t* restrict pairDefinition, EnvironmentState_t* restrict environment)
{
    EnvironmentState_t* targetEnvironment = GetPairDefinitionTargetEnvironment(SCONTEXT, pairDefinition, environment);

    // Immobility OPT Part 2 - Providing outgoing updates through immobiles is not required, the link will not be triggered during the mc routine
    // Sideffects:  None at this point (ref. to OPT part 1)

    #if defined(OPT_LINK_ONLY_MOBILES)
        return (targetEnvironment->IsMobile) ? targetEnvironment->EnvironmentLinks.End++ : NULL;
    #else
        return targetEnvironment->EnvironmentLinks.CurrentEnd++;
    #endif
}

// Resolves the target environment of a pair interaction and counts the link counter up by one
static void ResolvePairTargetAndIncreaseLinkCounter(__SCONTEXT_PAR, const EnvironmentState_t* restrict environment, const PairDefinition_t* restrict pairDefinition)
{
    EnvironmentState_t* targetEnvironment = GetPairDefinitionTargetEnvironment(SCONTEXT, pairDefinition, environment);

    // Immobility OPT Part 1 and 2 - No incoming or outgoing updates for immobiles are required
    #if defined(OPT_LINK_ONLY_MOBILES)
        voidreturn_if(!environment->IsMobile || !targetEnvironment->IsMobile);
    #endif

    // Use the uninitialized span access struct to count the elements before allocation!
    targetEnvironment->EnvironmentLinks.End++;
}

// Sets all link counters of the environment state lattice to the required number of linkers
static error_t SetAllLinkListCountersToRequiredSize(__SCONTEXT_PAR)
{
    cpp_foreach(environment, *getEnvironmentLattice(SCONTEXT))
    {
        cpp_foreach(pairDefinition, environment->EnvironmentDefinition->PairDefinitions)
        {
            ResolvePairTargetAndIncreaseLinkCounter(SCONTEXT, environment, pairDefinition);
        }
    }
    return ERR_OK;
}

// Allocates the environment linker lists to the size defined by their previously set counter status
static error_t AllocateEnvLinkListBuffersByPresetCounters(__SCONTEXT_PAR)
{
    error_t error;
    Buffer_t tmpBuffer;

    cpp_foreach(environment, *getEnvironmentLattice(SCONTEXT))
    {
        // Link is counted using the NULL initialized span access struct
        size_t linkCount = span_GetSize(environment->EnvironmentLinks);

        error = ctor_Buffer(tmpBuffer, linkCount * sizeof(EnvironmentLink_t));
        return_if(error, error);

        environment->EnvironmentLinks = (EnvironmentLinks_t) span_AsList(tmpBuffer);
    }
    return ERR_OK;
}


// Prepares the linking system construction by counting and allocation the required space for the system
static error_t PrepareLinkingSystemConstruction(__SCONTEXT_PAR)
{
    error_t error;

    // Note: This function uses the NULL initialized span pointers to actually count the required links!
    error = SetAllLinkListCountersToRequiredSize(SCONTEXT);
    return_if(error, error);

    error = AllocateEnvLinkListBuffersByPresetCounters(SCONTEXT);
    return error;
}

// Links an environment to its surroundings by sending a link to each one that requires one
static error_t LinkEnvironmentToSurroundings(__SCONTEXT_PAR, EnvironmentState_t* restrict environment)
{
    error_t error;
    int32_t pairId = 0;

    cpp_foreach(pairDefinition, environment->EnvironmentDefinition->PairDefinitions)
    {
        EnvironmentLink_t* environmentLink = GetNextLinkFromTargetEnv(SCONTEXT, pairDefinition, environment);
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
static void SortEnvironmentLinkingSystem(__SCONTEXT_PAR, EnvironmentState_t* environment)
{
    void* sortBase = environment->EnvironmentLinks.Begin;
    size_t numOfElements = span_GetSize(environment->EnvironmentLinks);

    qsort(sortBase, numOfElements, sizeof(EnvironmentLink_t), (FComparer_t) CompareEnvironmentLink);
}

// Constructs the prepared linking system by linking all environments and sorting the linkers to the required order
static error_t ConstructPreparedLinkingSystem(__SCONTEXT_PAR)
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
    {
        SortEnvironmentLinkingSystem(SCONTEXT, environment);
    }

    return ERR_OK;
}

// Builds the environment linking system of the environment state lattice
void BuildEnvironmentLinkingSystem(__SCONTEXT_PAR)
{
    error_t error;

    error = PrepareLinkingSystemConstruction(SCONTEXT);
    error_assert(error, "Failed to prepare the environment linking system for construction.");

    error = ConstructPreparedLinkingSystem(SCONTEXT);
    error_assert(error, "Failed to construct the environment linking system.");
}

// Allocates the dynamic environment occupation buffer for dynamic lookup of environment occupations (Size fits the largets environment definition)
static error_t AllocateDynamicEnvOccupationBuffer(__SCONTEXT_PAR, Buffer_t* restrict buffer)
{
    size_t bufferSize = 0;

    cpp_foreach(environmentDefinition, *getEnvironmentModels(SCONTEXT))
    {
        size_t count = span_GetSize(environmentDefinition->PairDefinitions);
        bufferSize = getMaxOfTwo(bufferSize, count);
    }

    return ctor_Buffer(*buffer, bufferSize *sizeof(byte_t));
}

// Find an environment state by resolving the passed pair id in the context of the start environment state
static EnvironmentState_t* PullEnvStateByInteraction(__SCONTEXT_PAR, EnvironmentState_t* restrict startEnvironment, const int32_t pairId)
{
    PairDefinition_t* pairDefinition = getEnvironmentPairDefinitionAt(startEnvironment, pairId);
    return GetPairDefinitionTargetEnvironment(SCONTEXT, pairDefinition, startEnvironment);
}

// Writes the current environment occupation of the passed environment state to the passed occupation buffer
static error_t WriteEnvOccupationToBuffer(__SCONTEXT_PAR, EnvironmentState_t* environment, Buffer_t* restrict occupationBuffer)
{
    for (int32_t i = 0; i < getEnvironmentPairDefinitionCount(environment); i++)
    {
        span_Get(*occupationBuffer, i) = PullEnvStateByInteraction(SCONTEXT, environment, i)->ParticleId;
        return_if(span_Get(*occupationBuffer,i) == PARTICLE_VOID, ERR_DATACONSISTENCY);
    }

    return ERR_OK;
}

// Resets all environment state buffers entries to a value of zero
static void ResetEnvStateBuffersToZero(EnvironmentState_t* restrict environment)
{
    cpp_foreach(energy, environment->EnergyStates)
    {
        *energy = 0;
    }

    cpp_foreach(cluster, environment->ClusterStates)
    {
        *cluster = (ClusterState_t) { 0, 0, 0ULL, 0ULL };
    }
}

// Adds all environment pair energies of the passed environment state to its internal energy buffers using the passed occupation buffer as the occupation source
static void AddEnvPairEnergyByOccupation(__SCONTEXT_PAR, EnvironmentState_t* restrict environment, Buffer_t* restrict occupationBuffer)
{
    for (size_t i = 0; i < span_GetSize(environment->EnvironmentDefinition->PairDefinitions); i++)
    {
        int32_t tableId = span_Get(environment->EnvironmentDefinition->PairDefinitions, i).EnergyTableId;
        const PairTable_t* pairTable = getPairEnergyTableAt(SCONTEXT, tableId);

        for (size_t j = 0; environment->EnvironmentDefinition->PositionParticleIds[j] != PARTICLE_NULL; j++)
        {
            byte_t positionParticleId = environment->EnvironmentDefinition->PositionParticleIds[j];
            byte_t partnerParticleId = span_Get(*occupationBuffer, i);
            span_Get(environment->EnergyStates, positionParticleId) += getPairEnergyAt(pairTable, positionParticleId,
                                                                                       partnerParticleId);
        }
    } 
}

// Initializes the passed cluster state status (code id) and sets the backups to the current values
static error_t InitializeClusterStateStatus(__SCONTEXT_PAR, ClusterState_t* restrict cluster, const ClusterTable_t* restrict clusterTable)
{
    cluster->CodeId = SaveFindClusterCodeIdByLinearSearch(clusterTable, cluster->OccupationCode);
    return_if(cluster->CodeId == INVALID_INDEX, ERR_DATACONSISTENCY);

    SetCluStateBackup(cluster);
    return ERR_OK;
}

// Synchronizes the all cluster states of the passed environment state and adds the resulting energies to the environment state energy buffer
static error_t AddEnvClusterEnergyByOccupation(__SCONTEXT_PAR, EnvironmentState_t* restrict environment, Buffer_t* restrict occupationBuffer)
{
    return_if(span_GetSize(environment->ClusterStates) != span_GetSize(environment->EnvironmentDefinition->ClusterDefinitions), ERR_DATACONSISTENCY);

    error_t error;
    ClusterState_t * cluster = environment->ClusterStates.Begin;

    cpp_foreach(clusterDefinition, environment->EnvironmentDefinition->ClusterDefinitions)
    {
        const ClusterTable_t* clusterTable = getClusterEnergyTableAt(SCONTEXT, clusterDefinition->EnergyTableId);
        for (byte_t i = 0; clusterDefinition->EnvironmentPairIds[i] != POSITION_NULL; i++)
        {
            int32_t codeByteId = clusterDefinition->EnvironmentPairIds[i];
            SetCodeByteAt(&cluster->OccupationCode, i, span_Get(*occupationBuffer, codeByteId));
        }

        error = InitializeClusterStateStatus(SCONTEXT, cluster, clusterTable);
        return_if(error != ERR_OK, ERR_DATACONSISTENCY);

        for (int32_t j = 0; environment->EnvironmentDefinition->PositionParticleIds[j] != PARTICLE_NULL; j++)
        {
            byte_t positionParticleId = environment->EnvironmentDefinition->PositionParticleIds[j];
            span_Get(environment->EnergyStates, positionParticleId) += getClusterEnergyAt(clusterTable, positionParticleId, cluster->CodeId);
        }
    }

    return ERR_OK;
}

// Sets the environment state energy buffers to the value that results from the passed occupation buffer entries
static error_t SetEnvStateEnergyByOccupation(__SCONTEXT_PAR, EnvironmentState_t* restrict environment, Buffer_t* restrict occupationBuffer)
{
    ResetEnvStateBuffersToZero(environment);
    AddEnvPairEnergyByOccupation(SCONTEXT, environment, occupationBuffer);
    return AddEnvClusterEnergyByOccupation(SCONTEXT, environment, occupationBuffer);
}

// Dynamically calculates the environment status (energies and cluster states) of the passed environment id using the provided occupation buffer
static error_t DynamicLookupEnvironmentStatus(__SCONTEXT_PAR, const int32_t environmentId, Buffer_t* restrict occupationBuffer)
{
    error_t error;
    EnvironmentState_t* environment = getEnvironmentStateAt(SCONTEXT, environmentId);

    error = WriteEnvOccupationToBuffer(SCONTEXT, environment, occupationBuffer);
    return_if(error, error);

    error = SetEnvStateEnergyByOccupation(SCONTEXT, environment, occupationBuffer);
    return_if(error, error);

    return ERR_OK;
}

// Dynamically synchronizes the environment lattice energy status to the current status (Cluster states and energy states)
// Resynchronizes potential lattice energy status errors caused by linking system optimization
void SyncEnvironmentEnergyStatus(__SCONTEXT_PAR)
{
    error_t error;
    Buffer_t occupationBuffer;

    error = AllocateDynamicEnvOccupationBuffer(SCONTEXT, &occupationBuffer);
    error_assert(error, "Buffer creation for environment occupation lookup failed.");

    for (int32_t i = 0; i < (int32_t) span_GetSize(*getMainStateLattice(SCONTEXT)); i++)
    {
        error = DynamicLookupEnvironmentStatus(SCONTEXT, i, &occupationBuffer);
        error_assert(error, "Dynamic lookup of environment occupation and energy failed.");
    }

    delete_Span(occupationBuffer);
}

// Sets the status of the environment state with the passed id to the default status using the passed occupation particle id
void SetEnvStateStatusToDefault(__SCONTEXT_PAR, const int32_t environmentId, const byte_t particleId)
{
    EnvironmentState_t* environment = getEnvironmentStateAt(SCONTEXT, environmentId);
    environment->ParticleId = particleId;
    environment->EnvironmentId = environmentId;
    environment->IsMobile = false;
    environment->IsStable = (particleId == PARTICLE_VOID) ? false : true;
    environment->PositionVector = Vector4FromInt32(environmentId, getLatticeBlockSizes(SCONTEXT));
    environment->EnvironmentDefinition = getEnvironmentModelAt(SCONTEXT, environment->PositionVector.D);
    environment->MobileTrackerId = INVALID_INDEX;
}

/* Simulation routines KMC and MMC */

// Sets the active work environment by evaluation of the provided environment link
static inline void SetActiveWorkEnvironmentByEnvLink(__SCONTEXT_PAR, EnvironmentLink_t *restrict environmentLink)
{
    SCONTEXT->CycleState.WorkEnvironment = getEnvironmentStateAt(SCONTEXT, environmentLink->EnvironmentId);
}

static inline void Set_ActiveWorkClusterByEnvAndId(__SCONTEXT_PAR, EnvironmentState_t* restrict environment, const byte_t clusterId)
{
    SCONTEXT->CycleState.WorkCluster = getEnvironmentClusterStateAt(environment, clusterId);
}

static inline void SetActiveWorkPairEnergyTable(__SCONTEXT_PAR, EnvironmentState_t *restrict environment,
                                                EnvironmentLink_t *restrict environmentLink)
{
    SCONTEXT->CycleState.WorkPairTable = getPairEnergyTableAt(SCONTEXT, getEnvironmentPairDefinitionAt(environment,
                                                                                                       environmentLink->PairId)->EnergyTableId);
}

static inline void Set_ActiveWorkClusterEnergyTable(__SCONTEXT_PAR, EnvironmentState_t* restrict environment, ClusterLink_t* restrict clusterLink)
{
    SCONTEXT->CycleState.WorkClusterTable = getClusterEnergyTableAt(SCONTEXT,
                                                                    getEnvironmentClusterDefinitionAt(environment,
                                                                                                      clusterLink->ClusterId)->EnergyTableId);
}

static inline int32_t FindClusterCodeIdInClusterTable(const ClusterTable_t* restrict clusterTable, const OccCode_t code)
{
    return FindClusterCodeIdByLinearSearch(clusterTable, code);
}

static inline double CalcPairEnergyDelta(const PairTable_t* restrict pairTable, const byte_t mainId, const byte_t oldId, const byte_t newId)
{
    return getPairEnergyAt(pairTable, mainId, oldId) - getPairEnergyAt(pairTable, mainId, newId);
}

static inline double CalcClusterEnergyDelta(const ClusterTable_t* restrict clusterTable, const ClusterState_t* restrict cluster, const byte_t particleId)
{
    return getClusterEnergyAt(clusterTable, particleId, cluster->CodeId) -
            getClusterEnergyAt(clusterTable, particleId, cluster->CodeIdBackup);
}

static inline void UpdateClusterState(const ClusterTable_t* restrict clusterTable, const ClusterLink_t* restrict clusterLink, ClusterState_t* restrict cluster, const byte_t newParticleId)
{
    SetCodeByteAt(&cluster->OccupationCode, clusterLink->CodeByteId, newParticleId);
    cluster->CodeId = FindClusterCodeIdInClusterTable(clusterTable, cluster->OccupationCode);
}

static inline void InvokeDeltaOfActivePair(__SCONTEXT_PAR, const byte_t updateParticleId, const byte_t oldParticleId, const byte_t newParticleId)
{
    *getActiveStateEnergyAt(SCONTEXT, updateParticleId) += CalcPairEnergyDelta(getActivePairTable(SCONTEXT), updateParticleId, oldParticleId, newParticleId);
}

static inline void InvokeDeltaOfActiveCluster(__SCONTEXT_PAR, const byte_t updateParticleId)
{
    *getActiveStateEnergyAt(SCONTEXT, updateParticleId) += CalcClusterEnergyDelta(getActiveClusterTable(SCONTEXT), getActiveWorkCluster(SCONTEXT), updateParticleId);
}

static void InvokeEnvLinkCluUpdates(__SCONTEXT_PAR, const EnvironmentLink_t* restrict environmentLink, const byte_t newParticleId)
{
    cpp_foreach(clusterLink, environmentLink->ClusterLinks)
    {
        Set_ActiveWorkClusterByEnvAndId(SCONTEXT, getActiveWorkEnvironment(SCONTEXT), clusterLink->ClusterId);
        Set_ActiveWorkClusterEnergyTable(SCONTEXT, getActiveWorkEnvironment(SCONTEXT), clusterLink);

        UpdateClusterState(getActiveClusterTable(SCONTEXT), clusterLink, getActiveWorkCluster(SCONTEXT), newParticleId);

        for (byte_t i = 0; getActiveParticleUpdateIdAt(SCONTEXT, i) != PARTICLE_NULL; i++)
        {
            InvokeDeltaOfActiveCluster(SCONTEXT, i);
        }

        SetCluStateBackup(getActiveWorkCluster(SCONTEXT));
    }
}

static void InvokeAllEnvLinkUpdates(__SCONTEXT_PAR, const EnvironmentLink_t* restrict environmentLink, const byte_t oldParticleId, const byte_t newParticleId)
{
    for (byte_t i = 0; getActiveParticleUpdateIdAt(SCONTEXT, i) != PARTICLE_NULL; i++)
    {
        InvokeDeltaOfActivePair(SCONTEXT, i, oldParticleId, newParticleId);
    }
    InvokeEnvLinkCluUpdates(SCONTEXT, environmentLink, newParticleId);
}

static inline void InvokeActiveLocalPairDelta(__SCONTEXT_PAR, const byte_t updateParticleId, const byte_t oldParticleId, const byte_t newParticleId)
{
    InvokeDeltaOfActivePair(SCONTEXT, updateParticleId, oldParticleId, newParticleId);
}

static inline void InvokeLocalEnvLinkClusterDeltas(__SCONTEXT_PAR, const EnvironmentLink_t* restrict environmentLink, const byte_t updateParticleId)
{
    cpp_foreach(clusterLink, environmentLink->ClusterLinks)
    {
        Set_ActiveWorkClusterByEnvAndId(SCONTEXT, getActiveWorkEnvironment(SCONTEXT), clusterLink->ClusterId);
        if(getActiveWorkCluster(SCONTEXT)->OccupationCode != getActiveWorkCluster(SCONTEXT)->OccupationCodeBackup)
        {        
            Set_ActiveWorkClusterEnergyTable(SCONTEXT, getActiveWorkEnvironment(SCONTEXT), clusterLink);
            InvokeDeltaOfActiveCluster(SCONTEXT, updateParticleId);
            LoadCluStateBackup(getActiveWorkCluster(SCONTEXT));
        }
    }
}

static inline void PrepareJumpLinkClusterStateChanges(__SCONTEXT_PAR, const JumpLink_t* restrict jumpLink)
{
    EnvironmentLink_t* environmentLink = getEnvLinkByJumpLink(SCONTEXT, jumpLink);
    SetActiveWorkEnvironmentByEnvLink(SCONTEXT, environmentLink);

    cpp_foreach(clusterLink, environmentLink->ClusterLinks)
    {
        Set_ActiveWorkClusterByEnvAndId(SCONTEXT, getActiveWorkEnvironment(SCONTEXT), clusterLink->ClusterId);
        byte_t newCodeByte = GetCodeByteAt(&getActiveJumpRule(SCONTEXT)->StateCode2, jumpLink->PathId);
        SetCodeByteAt(&getActiveWorkCluster(SCONTEXT)->OccupationCode, clusterLink->CodeByteId, newCodeByte);
    }
}

static void InvokeJumpLinkDeltas(__SCONTEXT_PAR, const JumpLink_t* restrict jumpLink)
{
    EnvironmentLink_t* environmentLink = getEnvLinkByJumpLink(SCONTEXT, jumpLink);

    SetActiveWorkEnvironmentByEnvLink(SCONTEXT, environmentLink);
    SetActiveWorkPairEnergyTable(SCONTEXT, getActiveWorkEnvironment(SCONTEXT), environmentLink);

    byte_t newId = GetCodeByteAt(&getActiveJumpRule(SCONTEXT)->StateCode2, jumpLink->PathId);
    byte_t updateParticleId = GetCodeByteAt(&getActiveJumpRule(SCONTEXT)->StateCode2, getActiveWorkEnvironment(SCONTEXT)->PathId);

    InvokeActiveLocalPairDelta(SCONTEXT, updateParticleId, JUMPPATH[jumpLink->PathId]->ParticleId, newId);
    InvokeLocalEnvLinkClusterDeltas(SCONTEXT, environmentLink, updateParticleId);
}

static void InvokeEnvUpdateDistribution(__SCONTEXT_PAR, EnvironmentState_t* restrict environment, const byte_t newParticleId)
{
    cpp_foreach(environmentLink, environment->EnvironmentLinks)
    {
        SetActiveWorkEnvironmentByEnvLink(SCONTEXT, environmentLink);
        SetActiveWorkPairEnergyTable(SCONTEXT, getActiveWorkEnvironment(SCONTEXT), environmentLink);
        InvokeAllEnvLinkUpdates(SCONTEXT, environmentLink, environment->ParticleId, newParticleId);
    }
}

void CreateLocalJumpDeltaKmc(__SCONTEXT_PAR)
{
    cpp_foreach(jumpLink, *getActiveLocalJumpLinks(SCONTEXT))
    {
        PrepareJumpLinkClusterStateChanges(SCONTEXT, jumpLink);
    }

    cpp_foreach(jumpLink, *getActiveLocalJumpLinks(SCONTEXT))
    {
        InvokeJumpLinkDeltas(SCONTEXT, jumpLink);
    }
}

void RollbackLocalJumpDeltaKmc(__SCONTEXT_PAR)
{
    for(byte_t i = 0; i < getActiveJumpDirection(SCONTEXT)->JumpLength; i++)
    {
        *getPathStateEnergyByIds(SCONTEXT, i, JUMPPATH[i]->ParticleId) = *getEnvStateEnergyBackupById(SCONTEXT, i); 
    }
}

void SetAllStateEnergiesKmc(__SCONTEXT_PAR)
{
    SetState0And1EnergiesKmc(SCONTEXT);
    CreateLocalJumpDeltaKmc(SCONTEXT);
    SetState2EnergyKmc(SCONTEXT);
    RollbackLocalJumpDeltaKmc(SCONTEXT);
}

void SetState0And1EnergiesKmc(__SCONTEXT_PAR)
{
    for(byte_t i = 0; i < getActiveJumpDirection(SCONTEXT)->JumpLength;i++)
    {
        if(JUMPPATH[i]->IsStable)
        {
            byte_t particleId = GetCodeByteAt(&getActiveJumpRule(SCONTEXT)->StateCode0, i);
            getJumpEnergyInfo(SCONTEXT)->Energy0 = *getPathStateEnergyByIds(SCONTEXT, i, particleId);
        }
        else
        {
            byte_t particleId = GetCodeByteAt(&getActiveJumpRule(SCONTEXT)->StateCode1, i);
            getJumpEnergyInfo(SCONTEXT)->Energy1 = *getPathStateEnergyByIds(SCONTEXT, i, particleId);
        }
    }
}

void SetState2EnergyKmc(__SCONTEXT_PAR)
{
    for(byte_t i = 0; i < getActiveJumpDirection(SCONTEXT)->JumpLength;i++)
    {
        byte_t particleId = GetCodeByteAt(&getActiveJumpRule(SCONTEXT)->StateCode2, i);
        getJumpEnergyInfo(SCONTEXT)->Energy2 = *getPathStateEnergyByIds(SCONTEXT, i, particleId); 
    }
}

void AdvanceKmcSystemToState2(__SCONTEXT_PAR)
{
    for(byte_t i = 0; i < getActiveJumpDirection(SCONTEXT)->JumpLength; i++)
    {
        byte_t newId = GetCodeByteAt(&getActiveJumpRule(SCONTEXT)->StateCode2, i);
        if(JUMPPATH[i]->IsStable)
        {
            InvokeEnvUpdateDistribution(SCONTEXT, JUMPPATH[i], newId);
        }
        JUMPPATH[i]->ParticleId = newId;
    }
}

// Searches the passed environment state link collection for a link to the passed environment id and builds a matching jump link object
static inline JumpLink_t BuildMMCJumpLink(const EnvironmentState_t*restrict envState, const int32_t envId, const int32_t pathId)
{
    JumpLink_t result = { .PathId = pathId, .LinkId = 0 };
    cpp_foreach(envLink, envState->EnvironmentLinks)
    {
        if (envLink->EnvironmentId == envId)
            return result;

        ++result.LinkId;
    }
    return (JumpLink_t){ .PathId = INVALID_INDEX, .LinkId = INVALID_INDEX };
}

void CreateLocalJumpDeltaMmc(__SCONTEXT_PAR)
{
    // Check if positions are within interaction range, if not no local delta has to be created
    if (!PositionAreInInteractionRange(SCONTEXT, &JUMPPATH[0]->PositionVector, &JUMPPATH[1]->PositionVector))
        return;

    // Find the required environment links and build matching temporary jump links
    JumpLink_t path0Link = BuildMMCJumpLink(JUMPPATH[0], JUMPPATH[1]->EnvironmentId, 0);
    JumpLink_t path1Link = BuildMMCJumpLink(JUMPPATH[1], JUMPPATH[0]->EnvironmentId, 1);
    debug_assert(path0Link.LinkId != INVALID_INDEX);
    debug_assert(path1Link.LinkId != INVALID_INDEX);

    // Prepare the potential cluster state changes on both environments and invoke the link deltas
    PrepareJumpLinkClusterStateChanges(SCONTEXT, &path0Link);
    PrepareJumpLinkClusterStateChanges(SCONTEXT, &path1Link);
    InvokeJumpLinkDeltas(SCONTEXT, &path0Link);
    InvokeJumpLinkDeltas(SCONTEXT, &path1Link);
}

void RollbackLocalJumpDeltaMmc(__SCONTEXT_PAR)
{
    *getPathStateEnergyByIds(SCONTEXT, 0, JUMPPATH[0]->ParticleId) = *getEnvStateEnergyBackupById(SCONTEXT, 0);
    *getPathStateEnergyByIds(SCONTEXT, 1, JUMPPATH[1]->ParticleId) = *getEnvStateEnergyBackupById(SCONTEXT, 1);
}


void SetAllStateEnergiesMmc(__SCONTEXT_PAR)
{
    SetState0EnergyMmc(SCONTEXT);
    CreateLocalJumpDeltaMmc(SCONTEXT);
    SetState2EnergyMmc(SCONTEXT);
    RollbackLocalJumpDeltaMmc(SCONTEXT);
}


void SetState0EnergyMmc(__SCONTEXT_PAR)
{
    byte_t parId0 = GetCodeByteAt(&getActiveJumpRule(SCONTEXT)->StateCode0, 0);
    getJumpEnergyInfo(SCONTEXT)->Energy0 =  *getPathStateEnergyByIds(SCONTEXT, 0, parId0);

    byte_t parId1 = GetCodeByteAt(&getActiveJumpRule(SCONTEXT)->StateCode0, 1);
    getJumpEnergyInfo(SCONTEXT)->Energy0 += *getPathStateEnergyByIds(SCONTEXT, 1, parId1);
}

void SetState2EnergyMmc(__SCONTEXT_PAR)
{
    byte_t parId0 = GetCodeByteAt(&getActiveJumpRule(SCONTEXT)->StateCode2, 0);
    getJumpEnergyInfo(SCONTEXT)->Energy2 =  *getPathStateEnergyByIds(SCONTEXT, 0, parId0);

    byte_t parId1 = GetCodeByteAt(&getActiveJumpRule(SCONTEXT)->StateCode2, 1);
    getJumpEnergyInfo(SCONTEXT)->Energy2 += *getPathStateEnergyByIds(SCONTEXT, 1, parId1);
}

void AdvanceMmcSystemToState2(__SCONTEXT_PAR)
{
    byte_t newParId0 = GetCodeByteAt(&getActiveJumpRule(SCONTEXT)->StateCode2, 0);
    byte_t newParId1 = GetCodeByteAt(&getActiveJumpRule(SCONTEXT)->StateCode2, 1);
    InvokeEnvUpdateDistribution(SCONTEXT, JUMPPATH[0], newParId0);
    InvokeEnvUpdateDistribution(SCONTEXT, JUMPPATH[1], newParId1);
    JUMPPATH[0]->ParticleId = newParId0;
    JUMPPATH[1]->ParticleId = newParId1;
}