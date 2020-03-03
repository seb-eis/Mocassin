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
#include "Simulator/Logic/Routines/Statistics/McStatistics.h"

/* Local helper routines */

// Finds a cluster code id by linear searching the passed occupation code (Unsafe search, infinite loop if code does not exist, very fast for small cluster code sets)
static inline int32_t LinearSearchClusterCodeId(const ClusterTable_t *restrict clusterTable, const OccupationCode64_t occupationCode)
{
    int32_t index = 0;
    while (span_Get(clusterTable->OccupationCodes, index).Value != occupationCode.Value) index++;
    return index;
}

// Finds a cluster code id by binary searching the passed occupation code (Save search, faster for large cluster code sets)
static inline int32_t BinarySearchClusterCodeId(const ClusterTable_t *restrict clusterTable, const OccupationCode64_t occupationCode)
{
    int32_t length = span_Length(clusterTable->OccupationCodes);
    int32_t firstIndex = 0;
    int32_t counter = length;
    while (counter > 0)
    {
        int32_t step = counter / 2;
        int32_t currentIndex = firstIndex + step;
        let currentValue = span_Get(clusterTable->OccupationCodes, currentIndex).Value;
        if (currentValue < occupationCode.Value)
        {
            firstIndex = ++currentIndex;
            counter -= step + 1;
        }
        else counter = step;
    }\
    firstIndex = (length != firstIndex) ? firstIndex : -1;
    debug_assert(firstIndex != -1);
    return firstIndex;
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
    *clusterLinks = span_New(*clusterLinks, byteCount);

    qsort(linkBuffer, count, sizeof(ClusterLink_t), (FComparer_t) CompareClusterLinks);
    memcpy(clusterLinks->Begin, linkBuffer, byteCount);

    return ERR_OK;
}

// Builds a cluster linking for the provided pair interaction id in the context of the passed environment definition
static error_t BuildClusterLinkingByPairId(const EnvironmentDefinition_t* environmentDefinition, const int32_t pairId, ClusterLinks_t* restrict clusterLinks)
{
    ClusterLink_t tmpLinkBuffer[sizeof(ClusterLink_t) * CLUSTER_MAXLINK_COUNT];
    byte_t clusterId = 0, relativeId = 0;
    size_t linkCount = 0;

    cpp_foreach(clusterDefinition, environmentDefinition->ClusterInteractions)
    {
        relativeId = 0;
        c_foreach(environmentPairId, clusterDefinition->PairInteractionIds)
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

    environmentLink->TargetEnvironmentId = environmentId;
    environmentLink->TargetPairId = pairId;
    error = BuildClusterLinkingByPairId(environmentDefinition, pairId, &environmentLink->ClusterLinks);

    return error;
}

// Get the next environment link pointer from the target environment for in place construction of the link
static EnvironmentLink_t* GetNextLinkFromTargetEnvironment(SCONTEXT_PARAMETER, const PairInteraction_t *restrict pairDefinition, EnvironmentState_t *restrict environment)
{
    var targetEnvironment = GetPairDefinitionTargetEnvironment(simContext, pairDefinition, environment);

    // Immobility OPT Part 2 - Providing outgoing updates through immobiles is not required, the link will not be triggered during the mc routine
    // Sideffects:  None at this point (ref. to OPT part 1)

    #if defined(OPT_LINK_ONLY_MOBILES)
        return (targetEnvironment->IsMobile) ? targetEnvironment->EnvironmentLinks.End++ : NULL;
    #else
        return targetEnvironment->EnvironmentLinks.End++;
    #endif
}

// Resolves the target environment of a pair interaction and counts the link counter up by one
static void ResolvePairTargetAndIncreaseLinkCounter(SCONTEXT_PARAMETER, const EnvironmentState_t* restrict environment, const PairInteraction_t* restrict pairDefinition)
{
    var targetEnvironment = GetPairDefinitionTargetEnvironment(simContext, pairDefinition, environment);

    // Immobility OPT Part 1 and 2 - Immobile or unstable targets do not need to provide updates to their surroundings
    #if defined(OPT_LINK_ONLY_MOBILES)
        return_if(!targetEnvironment->IsMobile || !targetEnvironment->IsStable);
    #endif

    // Use the uninitialized span access struct to count the elements before allocation!
    targetEnvironment->EnvironmentLinks.End++;
}

// Sets all link counters of the environment state lattice to the required number of linkers
static error_t SetAllLinkListCountersToRequiredSize(SCONTEXT_PARAMETER)
{
    var isMMC = JobInfoFlagsAreSet(simContext, INFO_FLG_MMC);
    cpp_foreach(environment, *getEnvironmentLattice(simContext))
    {
        // Immobility OPT Part 1 and 2 - Immobile centers (or unstables in MMC) to not need to receive updates from their surroundings
        #if defined(OPT_LINK_ONLY_MOBILES)
            continue_if(isMMC && !environment->IsStable);
            continue_if(environment->IsStable && !environment->IsMobile);
        #endif

        int32_t pairId = -1;
        cpp_foreach(pairDefinition, environment->EnvironmentDefinition->PairInteractions)
        {
            pairId++;
            let isLinkIrrelevant = CheckPairInteractionIsLinkIrrelevantByIndex(simContext, environment->EnvironmentDefinition, pairId);
            continue_if(isLinkIrrelevant);
            ResolvePairTargetAndIncreaseLinkCounter(simContext, environment, pairDefinition);
        }
    }

    return ERR_OK;
}

// Allocates the environment linker lists to the size defined by their previously set counter status
static error_t AllocateEnvLinkListBuffersByPresetCounters(SCONTEXT_PARAMETER)
{
    EnvironmentLinks_t tmpBuffer;

    cpp_foreach(environment, *getEnvironmentLattice(simContext))
    {
        // Link is counted using the NULL initialized span access struct
        let linkCount = span_Length(environment->EnvironmentLinks);
        tmpBuffer = list_New(tmpBuffer, linkCount);
        environment->EnvironmentLinks = tmpBuffer;
    }

    return ERR_OK;
}


// Prepares the linking system construction by counting and allocation the required space for the system
static error_t PrepareLinkingSystemConstruction(SCONTEXT_PARAMETER)
{
    error_t error;

    // Note: This function uses the NULL initialized span pointers to actually count the required links!
    error = SetAllLinkListCountersToRequiredSize(simContext);
    return_if(error, error);

    error = AllocateEnvLinkListBuffersByPresetCounters(simContext);
    return error;
}

// Links an environment to its surroundings by sending a link to each one that requires one and counts the passed ignore counters up depending on the reason for ignoring a link
static error_t LinkEnvironmentToSurroundings(SCONTEXT_PARAMETER, EnvironmentState_t* restrict environment, int32_t* mobiliyIgnoreCount, int32_t* energyIgnoreCount)
{
    error_t error;
    int32_t pairId = -1;
    let envId = getEnvironmentStateIdByPointer(simContext, environment);
    cpp_foreach(pairDefinition, environment->EnvironmentDefinition->PairInteractions)
    {
        pairId++;
        let isLinkIrrelevant = CheckPairInteractionIsLinkIrrelevantByIndex(simContext, environment->EnvironmentDefinition, pairId);
        if (isLinkIrrelevant)
        {
            (*energyIgnoreCount)++;
            continue;
        }

        var environmentLink = GetNextLinkFromTargetEnvironment(simContext, pairDefinition, environment);
        if (environmentLink == NULL)
        {
            (*mobiliyIgnoreCount)++;
            continue;
        }
        error = InPlaceConstructEnvironmentLink(environment->EnvironmentDefinition, envId, pairId, environmentLink);
        return_if(error, error);
    }

    return ERR_OK;
}

// Compares two environment links by their affiliated pair id
static inline int32_t CompareEnvironmentLink(const EnvironmentLink_t* restrict lhs, const EnvironmentLink_t* restrict rhs)
{
    return compareLhsToRhs(lhs->TargetPairId, rhs->TargetPairId);
}

// Sort the linking system of an environment state to the unit cell independent order
static void SortEnvironmentLinkingSystem(SCONTEXT_PARAMETER, EnvironmentState_t* environment)
{
    var sortBase = environment->EnvironmentLinks.Begin;
    let elementCount = span_Length(environment->EnvironmentLinks);
    qsort(sortBase, elementCount, sizeof(EnvironmentLink_t), (FComparer_t) CompareEnvironmentLink);
}

// Constructs the prepared linking system by linking all environments and sorting the linkers to the required order
static error_t ConstructPreparedLinkingSystem(SCONTEXT_PARAMETER)
{
    error_t error;
    int32_t linkCount = 0;
    int32_t mobilityIgnoredCount = 0;
    int32_t energyIgnoredCount = 0;
    bool_t isMMC = JobInfoFlagsAreSet(simContext, INFO_FLG_MMC);
    var environmentLattice = getEnvironmentLattice(simContext);
    cpp_foreach(environment, *environmentLattice)
    {
        // Immobility OPT Part 1 -> Incoming updates are not required, the state energy of immobile particles is not used during mc routine
        // Effect:    Causes all immobile particles to remain at their initial energy state during simulation (can be resynchronized by dynamic lookup)
        #if defined(OPT_LINK_ONLY_MOBILES)
        if((!environment->IsMobile && environment->IsStable) || (isMMC && !environment->IsStable))
        {
            mobilityIgnoredCount++;
            continue;
        }
        #endif

        error = LinkEnvironmentToSurroundings(simContext, environment, &mobilityIgnoredCount, &energyIgnoredCount);
        return_if(error, error);
    }

    cpp_foreach(environment, *environmentLattice)
    {
        SortEnvironmentLinkingSystem(simContext, environment);
        linkCount += span_Length(environment->EnvironmentLinks);
    }
    printf("[Init-Info]: Optimized required state dependency network [TOTAL_LINKS_CREATED=%i, SKIPPED_CONST_ENERGY_LINKS=%i, SKIPPED_CONST_PARTICLE_LINKS=%i]\n",
            linkCount, energyIgnoredCount, mobilityIgnoredCount);
    return ERR_OK;
}

// Checks all pair interactions and cluster interactions for constant tables and sets the required flags if required
static error_t DetectAndTagConstantInteractionDefinitions(SCONTEXT_PARAMETER)
{
    error_t error = ERR_OK;
    let energyModel = getDbEnergyModel(simContext);
    var fixPairCount = 0;
    var fixClusterCount = 0;
    cpp_foreach(table, energyModel->PairTables)
    {
        let isConst = CheckPairEnergyTableIsConstant(simContext, table);
        continue_if(!isConst);
        setFlags(table->Padding, ENERGY_FLG_CONST_TABLE);
        fixPairCount++;
    }
    if (fixPairCount != 0) printf("[Init-Info]: Optimized pair energy table behavior => No dynamic effect on energy landscape [%i of %i].\n",
            fixPairCount, (int32_t) span_Length(energyModel->PairTables));

    cpp_foreach(table, energyModel->ClusterTables)
    {
        let isConst = CheckClusterEnergyTableIsConstant(simContext, table);
        continue_if(!isConst);
        setFlags(table->Padding, ENERGY_FLG_CONST_TABLE);
        fixClusterCount++;
    }

    if (fixClusterCount != 0) printf("[Init-Info]: Optimized cluster energy table behavior => No dynamic effect on energy landscape [%i of %i].\n",
            fixClusterCount, (int32_t) span_Length(energyModel->ClusterTables));
    return error;
}

// Builds the environment linking system of the environment state lattice
void InitializeEnvironmentLinkingSystem(SCONTEXT_PARAMETER)
{
    error_t error;

    error = DetectAndTagConstantInteractionDefinitions(simContext);
    assert_success(error, "Failed to detect and tag constant energy tables.");

    error = PrepareLinkingSystemConstruction(simContext);
    assert_success(error, "Failed to prepare the environment linking system for construction.");

    error = ConstructPreparedLinkingSystem(simContext);
    assert_success(error, "Failed to construct the environment linking system.");
}

// Allocates the dynamic environment occupation buffer for dynamic lookup of environment occupations (Size fits the largest environment definition)
static error_t AllocateDynamicEnvOccupationBuffer(SCONTEXT_PARAMETER, Buffer_t* restrict buffer)
{
    size_t bufferSize = 0;

    cpp_foreach(environmentDefinition, *getEnvironmentModels(simContext))
        bufferSize = getMaxOfTwo(bufferSize, span_Length(environmentDefinition->PairInteractions));

    *buffer = span_New(*buffer, bufferSize);
    return ERR_OK;
}

// Find an environment state by resolving the passed pair id in the context of the start environment state
static EnvironmentState_t* PullEnvStateByInteraction(SCONTEXT_PARAMETER, EnvironmentState_t* restrict startEnvironment, const int32_t pairId)
{
    let pairDefinition = getEnvironmentPairDefinitionAt(startEnvironment, pairId);
    return GetPairDefinitionTargetEnvironment(simContext, pairDefinition, startEnvironment);
}

// Writes the current environment occupation of the passed environment state to the passed occupation buffer
static error_t WriteEnvOccupationToBuffer(SCONTEXT_PARAMETER, EnvironmentState_t* environment, Buffer_t* restrict occupationBuffer)
{
    let pairCount = getEnvironmentPairDefinitionCount(environment);

    for (int32_t i = 0; i < pairCount; i++)
    {
        let targetEnvironment = PullEnvStateByInteraction(simContext, environment, i);
        return_if(targetEnvironment->ParticleId == PARTICLE_VOID, ERR_DATACONSISTENCY);
        span_Get(*occupationBuffer, i) = targetEnvironment->ParticleId;
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
static void AddEnvPairEnergyByOccupation(SCONTEXT_PARAMETER, EnvironmentState_t* restrict environment, Buffer_t* restrict occupationBuffer)
{
    for (size_t i = 0; i < span_Length(environment->EnvironmentDefinition->PairInteractions); i++)
    {
        let tableId = span_Get(environment->EnvironmentDefinition->PairInteractions, i).EnergyTableId;
        let pairTable = getPairEnergyTableAt(simContext, tableId);
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
static error_t InitializeClusterStateStatus(SCONTEXT_PARAMETER, ClusterState_t* restrict cluster, const ClusterTable_t* restrict clusterTable)
{
    cluster->CodeId = BinarySearchClusterCodeId(clusterTable, cluster->OccupationCode);
    return_if(cluster->CodeId == INVALID_INDEX, ERR_DATACONSISTENCY);
    SetClusterStateBackup(cluster);
    return ERR_OK;
}

// Synchronizes the all cluster states of the passed environment state and adds the resulting energies to the environment state energy buffer
static error_t AddEnvClusterEnergyByOccupation(SCONTEXT_PARAMETER, EnvironmentState_t* restrict environment, Buffer_t* restrict occupationBuffer)
{
    return_if(span_Length(environment->ClusterStates) != span_Length(environment->EnvironmentDefinition->ClusterInteractions), ERR_DATACONSISTENCY);

    error_t error;
    var clusterState = environment->ClusterStates.Begin;
    let clusterStateEnd = environment->ClusterStates.End;

    cpp_foreach(clusterDefinition, environment->EnvironmentDefinition->ClusterInteractions)
    {
        return_if(clusterState == clusterStateEnd, ERR_DATACONSISTENCY);
        let clusterTable = getClusterEnergyTableAt(simContext, clusterDefinition->EnergyTableId);
        for (byte_t i = 0; clusterDefinition->PairInteractionIds[i] != POSITION_NULL; i++)
        {
            let codeByteId = clusterDefinition->PairInteractionIds[i];
            SetOccupationCodeByteAt(&clusterState->OccupationCode, i, span_Get(*occupationBuffer, codeByteId));
        }
        error = InitializeClusterStateStatus(simContext, clusterState, clusterTable);
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

// Adds the static environment background energies defined as defect table and lattice background to the passed environment state energies
static void AddStaticEnvBackgroundStateEnergies(SCONTEXT_PARAMETER, EnvironmentState_t* restrict environment)
{
    let cellBackground = getDefectBackground(simContext);
    let latticeBackground = getLatticeEnergyBackground(simContext);

    for (size_t j = 0; environment->EnvironmentDefinition->PositionParticleIds[j] != PARTICLE_NULL; j++)
    {
        let vector = environment->LatticeVector;
        let particleId = environment->EnvironmentDefinition->PositionParticleIds[j];
        let cellEntry = cellBackground->Begin == NULL? 0.0 : array_Get(*cellBackground, vector.D, particleId);
        let latticeEntry = latticeBackground->Begin == NULL ? 0.0 : array_Get(*latticeBackground, vecCoorSet4(vector), particleId);
        span_Get(environment->EnergyStates, particleId) += cellEntry + latticeEntry;
    }
}

// Sets the environment state energy buffers to the value that results from the passed occupation buffer entries
static error_t SetEnvStateEnergyByOccupation(SCONTEXT_PARAMETER, EnvironmentState_t* restrict environment, Buffer_t* restrict occupationBuffer)
{
    NullEnvironmentStateBuffers(environment);
    AddStaticEnvBackgroundStateEnergies(simContext, environment);
    AddEnvPairEnergyByOccupation(simContext, environment, occupationBuffer);
    return AddEnvClusterEnergyByOccupation(simContext, environment, occupationBuffer);
}

// Dynamically calculates the environment status (energies and cluster states) of the passed environment id using the provided occupation buffer
static error_t DynamicLookupEnvironmentStatus(SCONTEXT_PARAMETER, const int32_t environmentId, Buffer_t* restrict occupationBuffer)
{
    error_t error;
    var environment = getEnvironmentStateAt(simContext, environmentId);

    error = WriteEnvOccupationToBuffer(simContext, environment, occupationBuffer);
    return_if(error, error);

    error = SetEnvStateEnergyByOccupation(simContext, environment, occupationBuffer);
    return_if(error, error);

    return ERR_OK;
}

// Dynamically synchronizes the environment lattice energy status to the current status (Cluster states and energy states)
// and sets the current energy value on the main state
// Resynchronizes potential lattice energy status errors caused by linking system optimization
void ResynchronizeEnvironmentEnergyStatus(SCONTEXT_PARAMETER)
{
    error_t error;
    double energy = 0;
    Buffer_t occupationBuffer;
    var metaData = getMainStateMetaData(simContext);
    let physicalFactors = getPhysicalFactors(simContext);

    error = AllocateDynamicEnvOccupationBuffer(simContext, &occupationBuffer);
    assert_success(error, "Buffer creation for environment occupation lookup failed.");

    cpp_foreach (envState, *getEnvironmentLattice(simContext))
    {
        let envId = getEnvironmentStateIdByPointer(simContext, envState);
        error = DynamicLookupEnvironmentStatus(simContext, envId, &occupationBuffer);
        assert_success(error, "Dynamic lookup of environment occupation and energy failed.");
        continue_if(!envState->IsStable);
        energy += GetEnvironmentStateEnergy(envState);
    }
    metaData->LatticeEnergy = energy * physicalFactors->EnergyFactorKtToEv * 0.5;
    span_Delete(occupationBuffer);
}

// Sets the status of the environment state with the passed id to the default status using the passed occupation particle id
void SetEnvironmentStateToDefault(SCONTEXT_PARAMETER, const int32_t environmentId, const byte_t particleId)
{
    var environment = getEnvironmentStateAt(simContext, environmentId);
    environment->ParticleId = particleId;
    environment->IsMobile = false;
    environment->IsStable = (particleId == PARTICLE_VOID) ? false : true;
    environment->LatticeVector = Vector4FromInt32(environmentId, getLatticeBlockSizes(simContext));
    environment->EnvironmentDefinition = getEnvironmentModelAt(simContext, environment->LatticeVector.D);
    environment->MobileTrackerId = INVALID_INDEX;
}

/* Simulation routines KMC and MMC */

// Sets the active work environment by an environment link
static inline void SetActiveWorkEnvironment(SCONTEXT_PARAMETER, EnvironmentLink_t *restrict environmentLink)
{
    simContext->CycleState.WorkEnvironment = getEnvironmentStateAt(simContext, environmentLink->TargetEnvironmentId);
}

// Sets the active work cluster by environment and cluster id
static inline void SetActiveWorkCluster(SCONTEXT_PARAMETER, EnvironmentState_t *restrict environment, const byte_t clusterId)
{
    simContext->CycleState.WorkCluster = getEnvironmentClusterStateAt(environment, clusterId);
}

// Sets the active work pair energy table by environment and environment link
static inline void SetActiveWorkPairTable(SCONTEXT_PARAMETER, EnvironmentState_t *restrict environment, EnvironmentLink_t *restrict environmentLink)
{
    let pairDefinition = getEnvironmentPairDefinitionAt(environment, environmentLink->TargetPairId);
    #if defined(OPT_USE_3D_PAIRTABLES)
    simContext->CycleState.WorkPairTable = getPairDeltaTableAt(simContext, pairDefinition->EnergyTableId);
    #else
    simContext->CycleState.WorkPairTable = getPairEnergyTableAt(simContext, pairDefinition->EnergyTableId);
    #endif
}

// Set the active work cluster energy table by environment and cluster link
static inline void SetActiveWorkClusterTable(SCONTEXT_PARAMETER, EnvironmentState_t *restrict environment, ClusterLink_t *restrict clusterLink)
{
    let clusterDefinition = getEnvironmentClusterDefinitionAt(environment, clusterLink->ClusterId);
    simContext->CycleState.WorkClusterTable = getClusterEnergyTableAt(simContext, clusterDefinition->EnergyTableId);
}

// Finds a cluster code ID in a cluster table. Search is linear for very small occupation sets and binary for larger ones
static inline int32_t SearchClusterCodeIdInTable(const ClusterTable_t *restrict clusterTable,const OccupationCode64_t code)
{
    return (span_Length(clusterTable->OccupationCodes) < CLUSTER_MAXSIZE_LINEAR_SERACH)
        ? LinearSearchClusterCodeId(clusterTable, code)
        : BinarySearchClusterCodeId(clusterTable, code);
}

#if defined(OPT_USE_3D_PAIRTABLES)
// Get the delta energy value for the passed pair information (Pair delta table version)
static inline double GetPairEnergyDelta(const PairDeltaTable_t *restrict pairTable, const byte_t mainId, const byte_t oldId,const byte_t newId)
{
    return array_Get(*pairTable, oldId, newId, mainId);
}
#else
// Get the delta energy value for the passed pair information (Pair table version)
static inline double GetPairEnergyDelta(const PairTable_t *restrict pairTable, const byte_t mainId, const byte_t oldId,const byte_t newId)
{
    let oldEnergy = getPairEnergyAt(pairTable, mainId, oldId);
    let newEnergy = getPairEnergyAt(pairTable, mainId, newId);
    return newEnergy - oldEnergy;
}
#endif

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
    SetOccupationCodeByteAt(&cluster->OccupationCode, clusterLink->CodeByteId, newParticleId);
    cluster->CodeId = SearchClusterCodeIdInTable(clusterTable, cluster->OccupationCode);
}

// Invokes the currently resulting pair energy delta of the work object status
static inline void InvokeDeltaOfActivePair(SCONTEXT_PARAMETER, const byte_t updateParticleId, const byte_t oldParticleId, const byte_t newParticleId)
{
    let table = getActivePairTable(simContext);
    let delta = GetPairEnergyDelta(table, updateParticleId, oldParticleId, newParticleId);
    *getActiveStateEnergyAt(simContext, updateParticleId) += delta;
}

// Invokes the currently resulting cluster energy delta of the work object status
static inline void InvokeDeltaOfActiveCluster(SCONTEXT_PARAMETER, const byte_t updateParticleId)
{
    let table = getActiveClusterTable(simContext);
    let cluster = getActiveWorkCluster(simContext);
    let delta = GetClusterEnergyDelta(table, cluster, updateParticleId);
    *getActiveStateEnergyAt(simContext, updateParticleId) += delta;
}

// Invokes all changes on the cluster set of the passed environment link
static void InvokeEnvironmentLinkClusterUpdates(SCONTEXT_PARAMETER, const EnvironmentLink_t *restrict environmentLink, const byte_t newParticleId)
{
    let workEnvironment = getActiveWorkEnvironment(simContext);
    cpp_foreach(clusterLink, environmentLink->ClusterLinks)
    {
        SetActiveWorkCluster(simContext, workEnvironment, clusterLink->ClusterId);
        SetActiveWorkClusterTable(simContext, workEnvironment, clusterLink);
        let clusterTable = getActiveClusterTable(simContext);
        let workCluster = getActiveWorkCluster(simContext);

        UpdateClusterState(clusterTable, clusterLink, workCluster, newParticleId);
        continue_if(workCluster->CodeId == workCluster->CodeIdBackup);

        for (byte_t i = 0;; i++)
        {
            let updateParticleId = getActiveParticleUpdateIdAt(simContext, i);
            if (updateParticleId == PARTICLE_NULL) break;
            InvokeDeltaOfActiveCluster(simContext, updateParticleId);
        }

        SetClusterStateBackup(workCluster);
    }
}

// Invokes all link updates defined on the passed environment link with the passed particle information
static void InvokeEnvironmentLinkUpdates(SCONTEXT_PARAMETER, const EnvironmentLink_t *restrict environmentLink, const byte_t oldParticleId, const byte_t newParticleId)
{
    for (byte_t i = 0;; i++)
    {
        let updateParticleId = getActiveParticleUpdateIdAt(simContext, i);
        if (updateParticleId == PARTICLE_NULL) break;
        InvokeDeltaOfActivePair(simContext, updateParticleId, oldParticleId, newParticleId);
    }

    InvokeEnvironmentLinkClusterUpdates(simContext, environmentLink, newParticleId);
}

// Invokes the currently set active work pair for local delta data with the provided particle information
static inline void InvokeActiveLocalPairDelta(SCONTEXT_PARAMETER, const byte_t updateParticleId, const byte_t oldParticleId, const byte_t newParticleId)
{
    InvokeDeltaOfActivePair(simContext, updateParticleId, oldParticleId, newParticleId);
}

// Invokes the cluster changes of the passed environment link for local delta data with the provided particle information
static inline void InvokeLocalEnvironmentLinkClusterDeltas(SCONTEXT_PARAMETER, const EnvironmentLink_t *restrict environmentLink, const byte_t updateParticleId)
{
    let workEnvironment = getActiveWorkEnvironment(simContext);
    cpp_foreach(clusterLink, environmentLink->ClusterLinks)
    {
        SetActiveWorkCluster(simContext, workEnvironment, clusterLink->ClusterId);
        let workCluster = getActiveWorkCluster(simContext);
        if (workCluster->OccupationCode.Value != workCluster->OccupationCodeBackup.Value)
        {
            SetActiveWorkClusterTable(simContext, workEnvironment, clusterLink);
            InvokeDeltaOfActiveCluster(simContext, updateParticleId);
            LoadClusterStateBackup(getActiveWorkCluster(simContext));
        }
    }
}

// Prepares all jump link cluster changes for evaluation of the local delta generation
static inline void PrepareJumpLinkClusterStateChanges(SCONTEXT_PARAMETER, const JumpLink_t* restrict jumpLink)
{
    let environmentLink = getEnvLinkByJumpLink(simContext, jumpLink);
    SetActiveWorkEnvironment(simContext, environmentLink);

    let workEnvironment = getActiveWorkEnvironment(simContext);
    var jumpRule = getActiveJumpRule(simContext);
    cpp_foreach(clusterLink, environmentLink->ClusterLinks)
    {
        let newCodeByte = GetOccupationCodeByteAt(&jumpRule->StateCode2, jumpLink->SenderPathId);
        SetActiveWorkCluster(simContext, workEnvironment, clusterLink->ClusterId);
        var workCluster = getActiveWorkCluster(simContext);
        SetOccupationCodeByteAt(&workCluster->OccupationCode, clusterLink->CodeByteId, newCodeByte);
    }
}

// Invoke the local state delta that results from the passed jump link
static void InvokeJumpLinkDeltas(SCONTEXT_PARAMETER, const JumpLink_t* restrict jumpLink)
{
    let environmentLink = getEnvLinkByJumpLink(simContext, jumpLink);
    let sourceWorkEnvironment = getActiveWorkEnvironment(simContext);
    let jumpRule = getActiveJumpRule(simContext);

    //  Set the work pair table based on the environment link of the sender and switch active work environment to receiver
    SetActiveWorkPairTable(simContext, sourceWorkEnvironment, environmentLink);
    SetActiveWorkEnvironment(simContext, environmentLink);

    let newParticleId = GetOccupationCodeByteAt(&jumpRule->StateCode2, jumpLink->SenderPathId);
    let updateParticleId = GetOccupationCodeByteAt(&jumpRule->StateCode2, getActiveWorkEnvironment(simContext)->PathId);

    InvokeActiveLocalPairDelta(simContext, updateParticleId, JUMPPATH[jumpLink->SenderPathId]->ParticleId, newParticleId);
    InvokeLocalEnvironmentLinkClusterDeltas(simContext, environmentLink, updateParticleId);
}

// Distributes the update of the particle state of an environment to all linked environments
static void DistributeEnvironmentUpdate(SCONTEXT_PARAMETER, EnvironmentState_t *restrict environment, const byte_t newParticleId)
{
    cpp_foreach(environmentLink, environment->EnvironmentLinks)
    {
        SetActiveWorkEnvironment(simContext, environmentLink);
        let workEnvironment = getActiveWorkEnvironment(simContext);

        SetActiveWorkPairTable(simContext, workEnvironment, environmentLink);
        InvokeEnvironmentLinkUpdates(simContext, environmentLink, environment->ParticleId, newParticleId);
    }
}

// Writes the state energy entry that is subjected to jump link changes to the environment energy backup buffer
static inline void SetFinalStateEnergyBackup(SCONTEXT_PARAMETER, const byte_t pathId)
{
    let jumpRule = getActiveJumpRule(simContext);
    let updateParticleId = GetOccupationCodeByteAt(&jumpRule->StateCode2, pathId);
    *getEnvStateEnergyBackupById(simContext, pathId) = *getPathStateEnergyByIds(simContext, pathId, updateParticleId);
}

// Loads the state energy entry that was subjected to jump link changes from the environment energy backup buffer
static inline void LoadFinalStateEnergyBackup(SCONTEXT_PARAMETER, const byte_t pathId)
{
    let stateCode = &getActiveJumpRule(simContext)->StateCode2;
    let updateParticleId = GetOccupationCodeByteAt(stateCode, pathId);
    *getPathStateEnergyByIds(simContext, pathId, updateParticleId) = *getEnvStateEnergyBackupById(simContext, pathId);
}

/* Simulation sub routines */

void CreateAndBackupKmcTransitionDelta(SCONTEXT_PARAMETER)
{
    let jumpStatus = getActiveJumpStatus(simContext);
    let JumpDirection = getActiveJumpDirection(simContext);

    // Backup all required energy states
    for (int32_t i = 0; i < JumpDirection->JumpLength; ++i)
        SetFinalStateEnergyBackup(simContext, i);

    // Prepare the cluster state changes to avoid multiple code lookups
    cpp_foreach(jumpLink, jumpStatus->JumpLinks)
        PrepareJumpLinkClusterStateChanges(simContext, jumpLink);

    // Invoke the local pair and prepared cluster deltas
    cpp_foreach(jumpLink, jumpStatus->JumpLinks)
        InvokeJumpLinkDeltas(simContext, jumpLink);
}

void LoadKmcTransitionDeltaBackup(SCONTEXT_PARAMETER)
{
    let jumpDirection = getActiveJumpDirection(simContext);
    for(int32_t i = 0; i < jumpDirection->JumpLength; i++)
        LoadFinalStateEnergyBackup(simContext, i);
}

// Performs the action to set all KMC states in cases where the S2 bias correction is not static and requires dynamic calculation
static void inline KMC_SetStateEnergiesWithDynamicCorrection(SCONTEXT_PARAMETER)
{
    SetKmcStartTransitionBaseAndFieldEnergyStatesOnContext(simContext);
    CreateAndBackupKmcTransitionDelta(simContext);
    SetFinalKmcStateEnergyOnContext(simContext);
    LoadKmcTransitionDeltaBackup(simContext);
}

// Performs the action to set all KMC states in cases where the S2 bias correction is a known constant value
static void inline KMC_SetStateEnergiesWithStaticCorrection(SCONTEXT_PARAMETER)
{
    SetKmcStartTransitionBaseAndFieldEnergyStatesOnContext(simContext);
    SetFinalKmcStateEnergyOnContext(simContext);
    let jumpRule = getActiveJumpRule(simContext);
    let energies = getJumpEnergyInfo(simContext);
    energies->S2Energy += jumpRule->StaticVirtualJumpEnergyCorrection;
}

void SetKmcStateEnergiesOnContext(SCONTEXT_PARAMETER)
{
    let jumpRule = getActiveJumpRule(simContext);
    if (jumpRule->StaticVirtualJumpEnergyCorrection == JUMPS_JUMPCORRECTION_NOTSTATIC)
        KMC_SetStateEnergiesWithDynamicCorrection(simContext);
    else
        KMC_SetStateEnergiesWithStaticCorrection(simContext);
}

//  Adds the current energy contribution to state S0 and S1 for a path id to the energy info
static inline void AddPathStateS0AndS1EnergyByPathId(SCONTEXT_PARAMETER, const int32_t pathId, JumpRule_t*restrict jumpRule, JumpEnergyInfo_t*restrict energyInfo)
{
    var particleId = GetOccupationCodeByteAt(&jumpRule->StateCode0, pathId);
    energyInfo->S0Energy += *getPathStateEnergyByIds(simContext, pathId, particleId);
    particleId = GetOccupationCodeByteAt(&jumpRule->StateCode1, pathId);
    energyInfo->S1Energy += *getPathStateEnergyByIds(simContext, pathId, particleId);
}

void SetKmcStartTransitionBaseAndFieldEnergyStatesOnContext(SCONTEXT_PARAMETER)
{
    let jumpDirection = getActiveJumpDirection(simContext);
    let jumpRule = getActiveJumpRule(simContext);
    var energyInfo = getJumpEnergyInfo(simContext);
    var particleId = GetOccupationCodeByteAt(&jumpRule->StateCode0, 0);

    // Set the field influence energy for the jump
    energyInfo->ElectricFieldEnergy = GetCurrentElectricFieldJumpInfluence(simContext);

    // Set the values of the first entry, the first transition state energy is always zero
    energyInfo->S0Energy = *getPathStateEnergyByIds(simContext, 0, particleId);
    energyInfo->S1Energy = 0;

    //  Fallthrough switch of jump length cases
    switch (jumpDirection->JumpLength)
    {
        case 8:
            AddPathStateS0AndS1EnergyByPathId(simContext, 7, jumpRule, energyInfo);
        case 7:
            AddPathStateS0AndS1EnergyByPathId(simContext, 6, jumpRule, energyInfo);
        case 6:
            AddPathStateS0AndS1EnergyByPathId(simContext, 5, jumpRule, energyInfo);
        case 5:
            AddPathStateS0AndS1EnergyByPathId(simContext, 4, jumpRule, energyInfo);
        case 4:
            AddPathStateS0AndS1EnergyByPathId(simContext, 3, jumpRule, energyInfo);
        case 3:
            AddPathStateS0AndS1EnergyByPathId(simContext, 2, jumpRule, energyInfo);
            AddPathStateS0AndS1EnergyByPathId(simContext, 1, jumpRule, energyInfo);
        default:
            break;
    }
}

//  Adds the current energy contribution to state S2 for a path id to the energy info
static inline void AddPathStateS2EnergyByPathId(SCONTEXT_PARAMETER, const int32_t pathId, JumpRule_t*restrict jumpRule, JumpEnergyInfo_t*restrict energyInfo)
{
    var particleId = GetOccupationCodeByteAt(&jumpRule->StateCode2, pathId);
    energyInfo->S2Energy += *getPathStateEnergyByIds(simContext, pathId, particleId);
}

void SetFinalKmcStateEnergyOnContext(SCONTEXT_PARAMETER)
{
    let jumpRule = getActiveJumpRule(simContext);
    let jumpDirection = getActiveJumpDirection(simContext);
    var energyInfo = getJumpEnergyInfo(simContext);

    // Set the values of the first entry
    let particleId = GetOccupationCodeByteAt(&jumpRule->StateCode2, 0);
    energyInfo->S2Energy = *getPathStateEnergyByIds(simContext, 0, particleId);

    //  Fallthrough switch of jump length cases
    switch (jumpDirection->JumpLength)
    {
        case 8:
            AddPathStateS2EnergyByPathId(simContext, 7, jumpRule, energyInfo);
        case 7:
            AddPathStateS2EnergyByPathId(simContext, 6, jumpRule, energyInfo);
        case 6:
            AddPathStateS2EnergyByPathId(simContext, 5, jumpRule, energyInfo);
        case 5:
            AddPathStateS2EnergyByPathId(simContext, 4, jumpRule, energyInfo);
        case 4:
            AddPathStateS2EnergyByPathId(simContext, 3, jumpRule, energyInfo);
        case 3:
            AddPathStateS2EnergyByPathId(simContext, 2, jumpRule, energyInfo);
            AddPathStateS2EnergyByPathId(simContext, 1, jumpRule, energyInfo);
        default:
            break;
    }
}

// Advances a single path id step to the final state
static inline void KMC_AdvanceSystemToFinalStateByPathId(SCONTEXT_PARAMETER, const int32_t pathId, OccupationCode64_t*restrict stateCode)
{
    let envState = JUMPPATH[pathId];
    let newParticleId = GetOccupationCodeByteAt(stateCode, pathId);
    DistributeEnvironmentUpdate(simContext, envState, newParticleId);
    envState->ParticleId = newParticleId;
}

void AdvanceKmcSystemToFinalState(SCONTEXT_PARAMETER)
{
    let stateCode = &getActiveJumpRule(simContext)->StateCode2;
    let jumpDirection = getActiveJumpDirection(simContext);

    //  Fallthrough switch of jump length cases
    switch (jumpDirection->JumpLength)
    {
        case 8:
            KMC_AdvanceSystemToFinalStateByPathId(simContext, 7, stateCode);
        case 7:
            KMC_AdvanceSystemToFinalStateByPathId(simContext, 6, stateCode);
        case 6:
            KMC_AdvanceSystemToFinalStateByPathId(simContext, 5, stateCode);
        case 5:
            KMC_AdvanceSystemToFinalStateByPathId(simContext, 4, stateCode);
        case 4:
            KMC_AdvanceSystemToFinalStateByPathId(simContext, 3, stateCode);
        case 3:
            KMC_AdvanceSystemToFinalStateByPathId(simContext, 2, stateCode);
            KMC_AdvanceSystemToFinalStateByPathId(simContext, 1, stateCode);
            KMC_AdvanceSystemToFinalStateByPathId(simContext, 0, stateCode);
        default:
            break;
    }
}

// Searches the passed environment state link collection for a link to the passed environment id and builds a matching jump link object
static inline JumpLink_t MMC_BuildJumpLink(const EnvironmentState_t *restrict envState, const int32_t targetEnvId)
{
    var result = (JumpLink_t) { .SenderPathId = envState->PathId, .LinkId = 0 };
    cpp_foreach(envLink, envState->EnvironmentLinks)
    {
        return_if(envLink->TargetEnvironmentId == targetEnvId, result);
        ++result.LinkId;
    }
    return (JumpLink_t){ .SenderPathId = INVALID_INDEX, .LinkId = INVALID_INDEX };
}

bool_t TryCreateAndBackupMmcTransitionDelta(SCONTEXT_PARAMETER)
{
    // Check if the positions are potentially close enough to be linked
    return_if(!PositionAreInInteractionRange(simContext, &JUMPPATH[0]->LatticeVector, &JUMPPATH[1]->LatticeVector), false);

    // Find the required environment links and build matching temporary jump links if they exist
    // If the first is not found the second can by definition not exist as well
    let envState0 = JUMPPATH[0];
    let envState1 = JUMPPATH[1];
    let path0JumpLink = MMC_BuildJumpLink(envState0, getEnvironmentStateIdByPointer(simContext, envState1));
    return_if(path0JumpLink.LinkId == INVALID_INDEX, false);

    let path1JumpLink = MMC_BuildJumpLink(envState1, getEnvironmentStateIdByPointer(simContext, envState0));

    // Backup the final state energies
    SetFinalStateEnergyBackup(simContext, 0);
    SetFinalStateEnergyBackup(simContext, 1);

    // Prepare the potential cluster state changes on both environments and invoke the link deltas
    PrepareJumpLinkClusterStateChanges(simContext, &path0JumpLink);
    PrepareJumpLinkClusterStateChanges(simContext, &path1JumpLink);
    InvokeJumpLinkDeltas(simContext, &path0JumpLink);
    InvokeJumpLinkDeltas(simContext, &path1JumpLink);
    return true;
}

void LoadMmcTransitionDeltaBackup(SCONTEXT_PARAMETER)
{
    LoadFinalStateEnergyBackup(simContext, 0);
    LoadFinalStateEnergyBackup(simContext, 1);
}


void SetMmcStateEnergiesOnContext(SCONTEXT_PARAMETER)
{
    SetMmcStartStateEnergyOnContext(simContext);

    // Try to create a backup if required, else the positions do not interact and the final energy can be directly set
    if (TryCreateAndBackupMmcTransitionDelta(simContext))
    {
        SetMmcFinalStateEnergyOnContext(simContext);
        LoadMmcTransitionDeltaBackup(simContext);
        return;
    }
    SetMmcFinalStateEnergyOnContext(simContext);
}


void SetMmcStartStateEnergyOnContext(SCONTEXT_PARAMETER)
{
    let jumpRule = getActiveJumpRule(simContext);
    var jumpEnergyInfo = getJumpEnergyInfo(simContext);

    let particleId0 = GetOccupationCodeByteAt(&jumpRule->StateCode0, 0);
    jumpEnergyInfo->S0Energy =  *getPathStateEnergyByIds(simContext, 0, particleId0);
    let particleId1 = GetOccupationCodeByteAt(&jumpRule->StateCode0, 1);
    jumpEnergyInfo->S0Energy += *getPathStateEnergyByIds(simContext, 1, particleId1);
}

void SetMmcFinalStateEnergyOnContext(SCONTEXT_PARAMETER)
{
    let jumpRule = getActiveJumpRule(simContext);
    var jumpEnergyInfo = getJumpEnergyInfo(simContext);

    let particleId0 = GetOccupationCodeByteAt(&jumpRule->StateCode2, 0);
    jumpEnergyInfo->S2Energy =  *getPathStateEnergyByIds(simContext, 0, particleId0);
    let particleId1 = GetOccupationCodeByteAt(&jumpRule->StateCode2, 1);
    jumpEnergyInfo->S2Energy += *getPathStateEnergyByIds(simContext, 1, particleId1);
}

void AdvanceMmcSystemToFinalState(SCONTEXT_PARAMETER)
{
    let jumpRule = getActiveJumpRule(simContext);
    let newParticleId0 = GetOccupationCodeByteAt(&jumpRule->StateCode2, 0);
    let newParticleId1 = GetOccupationCodeByteAt(&jumpRule->StateCode2, 1);
    var envState0 = JUMPPATH[0];
    var envState1 = JUMPPATH[1];

    DistributeEnvironmentUpdate(simContext, envState0, newParticleId0);
    envState0->ParticleId = newParticleId0;
    DistributeEnvironmentUpdate(simContext, envState1, newParticleId1);
    envState1->ParticleId = newParticleId1;
}