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
#include "Simulator/Data/Model/SimContext/ContextAccess.h"

/* Local helper routines */

static inline int32_t SaveFindClusterCodeIdByLinearSearch(const clu_table_t* restrict clusterTable, const occode_t occupationCode)
{
    int32_t index = 0;
    size_t numOfCodes = span_GetSize(clusterTable->OccupationCodes);
    while ((span_Get(clusterTable->OccupationCodes, index) != occupationCode) && (index < numOfCodes))
    {
        index++;
    }
    return (index < numOfCodes) ? index : INVALID_INDEX;
}

static inline int32_t FindClusterCodeIdByLinearSearch(const clu_table_t* restrict clusterTable, const occode_t occupationCode)
{
    int32_t index = 0;
    while (span_Get(clusterTable->OccupationCodes, index++) != occupationCode) {};
    return index;
}

//static inline int32_t BinaryLookupCluCodeId(const clu_table_t* restrict clusterTable, const occode_t occupationCode)
//{
//    // Placeholder, implement on optimization
//    return -1;
//}

static inline void SetCluStateBackup(clu_state_t* restrict cluster)
{
    cluster->CodeIdBackup = cluster->CodeId;
    cluster->OccupationCodeBackup = cluster->OccupationCode;
}

static inline void LoadCluStateBackup(clu_state_t* restrict cluster)
{
    cluster->CodeId = cluster->CodeIdBackup;
    cluster->OccupationCode = cluster->OccupationCodeBackup;
}

/* Initializer routines */

static int32_t CompareClusterLinks(const clu_link_t* lhs, const clu_link_t* rhs)
{
    int32_t value = get_compare(lhs->ClusterId, rhs->ClusterId);
    if (value)
    {
        return get_compare(lhs->CodeByteId, rhs->CodeByteId);
    }
    return value;
}

static error_t SortAndBuildClusterLinks(clu_link_t* restrict linkBuffer, const size_t count, clu_links_t* restrict clusterLinks)
{
    error_t error;
    buffer_t tmpBuffer;

    error = ctor_Buffer(tmpBuffer, count* sizeof(clu_link_t));
    return_if(error, error);

    qsort(linkBuffer, count, sizeof(clu_link_t), (f_compare_t) CompareClusterLinks);

    CopyBuffer((byte_t*) linkBuffer, tmpBuffer.Begin, sizeof(clu_link_t) * count);
    *clusterLinks = (clu_links_t) span_AsVoid(tmpBuffer);

    return ERR_OK;
}

static error_t BuildClusterLinkingByPairId(const env_def_t* environmentDefinition, const int32_t pairId, clu_links_t* restrict clusterLinks)
{
    clu_link_t tmpLinkBuffer[sizeof(clu_link_t) * 256]; // There are a max of 256 clusters a single pair could belong to
    byte_t clusterId = 0, relativeId = 0;
    size_t linkCount = 0;

    cpp_foreach(clusterDefinition, environmentDefinition->ClusterDefinitions)
    {
        relativeId = 0;
        c_foreach(environmentPairId, clusterDefinition->EnvironmentPairIds)
        {
            if (*environmentPairId == pairId)
            {
                tmpLinkBuffer[linkCount] = (clu_link_t) { clusterId , relativeId++ };
                linkCount++;
            }
        }
        clusterId++;
    }

    return SortAndBuildClusterLinks(tmpLinkBuffer, linkCount, clusterLinks);
}

static error_t InPlaceConstructEnvironmentLink(const env_def_t* restrict environmentDefinition, const int32_t environmentId, const int32_t pairId, env_link_t* restrict environmentLink)
{
    error_t error;

    environmentLink->EnvironmentId = environmentId;
    environmentLink->PairId = pairId;
    error = BuildClusterLinkingByPairId(environmentDefinition, pairId, &environmentLink->ClusterLinks);

    return error;
}

static env_link_t* GetNextLinkFromTargetEnv(__SCONTEXT_PAR, const pair_def_t* restrict pairDefinition, env_state_t* restrict environment)
{
    env_state_t* targetEnvironment = ResolvePairDefTargetEnvironment(SCONTEXT, pairDefinition, environment);

    // Immobility OPT Part 2 - Providing outgoing updates through immobiles is not required, the link will not be triggered during the mc routine
    // Sideffects:  None at this point (ref. to OPT part 1)

    #if defined(OPT_LINK_ONLY_MOBILES)
        return (targetEnvironment->IsMobile) ? targetEnvironment->EnvironmentLinks.End++ : NULL;
    #else
        return targetEnvironment->EnvironmentLinks.CurrentEnd++;
    #endif
}

static void ResolvePairTargetAndIncreaseLinkCounter(__SCONTEXT_PAR, const env_state_t* restrict environment, const pair_def_t* restrict pairDefinition)
{
    env_state_t* targetEnvironment = ResolvePairDefTargetEnvironment(SCONTEXT, pairDefinition, environment);

    // Immobility OPT Part 1 and 2 - No incoming or outgoing updates for immobiles are required
    #if defined(OPT_LINK_ONLY_MOBILES)
        voidreturn_if(!environment->IsMobile || !targetEnvironment->IsMobile);
    #endif

    // Use the uninitialized span access struct to count the elements before allocation!
    targetEnvironment->EnvironmentLinks.End++;
}

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

static error_t AllocateEnvLinkListBuffersByPresetCounters(__SCONTEXT_PAR)
{
    error_t error;
    buffer_t tmpBuffer;

    cpp_foreach(environment, *getEnvironmentLattice(SCONTEXT))
    {
        // Link is counted using the NULL initialized span access struct
        size_t linkCount = span_GetSize(environment->EnvironmentLinks);

        error = ctor_Buffer(tmpBuffer, linkCount * sizeof(env_link_t));
        return_if(error, error);

        environment->EnvironmentLinks = (env_links_t) span_AsList(tmpBuffer);
    }
    return ERR_OK;
}


static error_t PrepareLinkingSystemConstruction(__SCONTEXT_PAR)
{
    error_t error;

    // Note: This function uses the NULL initialized span pointers to actually count the required links!
    error = SetAllLinkListCountersToRequiredSize(SCONTEXT);
    return_if(error, error);

    error = AllocateEnvLinkListBuffersByPresetCounters(SCONTEXT);
    return error;
}

static error_t LinkEnvironmentToSurroundings(__SCONTEXT_PAR, env_state_t* restrict environment)
{
    error_t error;
    int32_t pairId = 0;

    cpp_foreach(pairDefinition, environment->EnvironmentDefinition->PairDefinitions)
    {
        env_link_t* environmentLink = GetNextLinkFromTargetEnv(SCONTEXT, pairDefinition, environment);
        if (environmentLink != NULL)
        {
            error = InPlaceConstructEnvironmentLink(environment->EnvironmentDefinition, environment->EnvironmentId, pairId, environmentLink);
            return_if(error, error);
        }
        pairId++;
    }

    return ERR_OK;
}

static error_t ConstructPreparedLinkingSystem(__SCONTEXT_PAR)
{
    error_t error;

    cpp_foreach(environment, *getEnvironmentLattice(SCONTEXT))
    {
        // Immobility OPT Part 1 -> Incomming updates are not required, the state energy of immobiles is not used during mc routine
        // Sideffect:   Causes all immobiles to remain at their initial energy state during simulation (can be resynchronized by dynamic lookup)
        #if defined(OPT_LINK_ONLY_MOBILES)
            continue_if(!environment->IsMobile);
        #endif

        error = LinkEnvironmentToSurroundings(SCONTEXT, environment);
        return_if(error, error);
    }

    return ERR_OK;
}

void BuildEnvironmentLinkingSystem(__SCONTEXT_PAR)
{
    error_t error;

    error = PrepareLinkingSystemConstruction(SCONTEXT);
    error_assert(error, "Failed to prepare the environment linking system for construction.");

    error = ConstructPreparedLinkingSystem(SCONTEXT);
    error_assert(error, "Failed to construct the environment linking system.");
}

static error_t AllocateDynamicEnvOccupationBuffer(__SCONTEXT_PAR, buffer_t* restrict buffer)
{
    size_t bufferSize = 0;

    cpp_foreach(environmentDefinition, *getEnvironmentModels(SCONTEXT))
    {
        size_t count = span_GetSize(environmentDefinition->PairDefinitions);
        bufferSize = getMaxOfTwo(bufferSize, count);
    }

    return ctor_Buffer(*buffer, bufferSize *sizeof(byte_t));
}

static env_state_t* PullEnvStateByInteraction(__SCONTEXT_PAR, env_state_t* restrict startEnvironment, const int32_t pairId)
{
    pair_def_t* pairDefinition = getEnvironmentPairDefById(startEnvironment, pairId);
    return ResolvePairDefTargetEnvironment(SCONTEXT, pairDefinition, startEnvironment);
}

static error_t WriteEnvOccupationToBuffer(__SCONTEXT_PAR, env_state_t* environment, buffer_t* restrict occupationBuffer)
{
    for (int32_t i = 0; i < getEnvironmentPairDefCount(environment); i++)
    {
        span_Get(*occupationBuffer, i) = PullEnvStateByInteraction(SCONTEXT, environment, i)->ParticleId;
        return_if(span_Get(*occupationBuffer,i) == PARTICLE_VOID, ERR_DATACONSISTENCY);
    }

    return ERR_OK;
}

static void ResetEnvStateBuffersToZero(env_state_t* restrict environment)
{
    cpp_foreach(energy, environment->EnergyStates)
    {
        *energy = 0;
    }
    cpp_foreach(cluster, environment->ClusterStates)
    {
        *cluster = (clu_state_t) { 0, 0, 0ULL, 0ULL };
    }
}

static void AddEnvPairEnergyByOccupation(__SCONTEXT_PAR, env_state_t* restrict environment, buffer_t* restrict occupationBuffer)
{
    for (size_t i = 0; i < span_GetSize(environment->EnvironmentDefinition->PairDefinitions); i++)
    {
        int32_t tableId = span_Get(environment->EnvironmentDefinition->PairDefinitions, i).TableId;
        const pair_table_t* pairTable = getPairEnergyTableById(SCONTEXT, tableId);

        for (size_t j = 0; environment->EnvironmentDefinition->PositionParticleIds[j] != PARTICLE_NULL; j++)
        {
            byte_t positionParticleId = environment->EnvironmentDefinition->PositionParticleIds[j];
            byte_t partnerParticleId = span_Get(*occupationBuffer, i);
            span_Get(environment->EnergyStates, positionParticleId) += getPairEnergyTableEntry(pairTable, positionParticleId, partnerParticleId);
        }
    } 
}

static error_t InitializeClusterStateStatus(__SCONTEXT_PAR, clu_state_t* restrict cluster, const clu_table_t* restrict clusterTable)
{
    cluster->CodeId = SaveFindClusterCodeIdByLinearSearch(clusterTable, cluster->OccupationCode);
    return_if(cluster->CodeId == INVALID_INDEX, ERR_DATACONSISTENCY);

    SetCluStateBackup(cluster);
    return ERR_OK;
}

static error_t AddEnvClusterEnergyByOccupation(__SCONTEXT_PAR, env_state_t* restrict environment, buffer_t* restrict occupationBuffer)
{
    return_if(span_GetSize(environment->ClusterStates) != span_GetSize(environment->EnvironmentDefinition->ClusterDefinitions), ERR_DATACONSISTENCY);

    error_t error;
    clu_state_t * cluster = environment->ClusterStates.Begin;

    cpp_foreach(clusterDefinition, environment->EnvironmentDefinition->ClusterDefinitions)
    {
        const clu_table_t* clusterTable = getClusterEnergyTableById(SCONTEXT, clusterDefinition->TableId);
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
            span_Get(environment->EnergyStates, positionParticleId) += getCluEnergyTableEntry(clusterTable, positionParticleId, cluster->CodeId);
        }
    }

    return ERR_OK;
}

static error_t SetEnvStateEnergyByOccupation(__SCONTEXT_PAR, env_state_t* restrict environment, buffer_t* restrict occupationBuffer)
{
    ResetEnvStateBuffersToZero(environment);
    AddEnvPairEnergyByOccupation(SCONTEXT, environment, occupationBuffer);
    return AddEnvClusterEnergyByOccupation(SCONTEXT, environment, occupationBuffer);
}

static error_t DynamicLookupEnvironmentStatus(__SCONTEXT_PAR, const int32_t environmentId, buffer_t* restrict occupationBuffer)
{
    error_t error;
    env_state_t* environment = getEnvironmentStateById(SCONTEXT, environmentId);

    error = WriteEnvOccupationToBuffer(SCONTEXT, environment, occupationBuffer);
    return_if(error, error);

    error = SetEnvStateEnergyByOccupation(SCONTEXT, environment, occupationBuffer);
    return_if(error, error);

    return ERR_OK;
}

void SyncEnvironmentEnergyStatus(__SCONTEXT_PAR)
{
    error_t error;
    buffer_t occupationBuffer;

    error = AllocateDynamicEnvOccupationBuffer(SCONTEXT, &occupationBuffer);
    error_assert(error, "Buffer creation for environment occupation lookup failed.");

    for (int32_t i = 0; i < (int32_t) span_GetSize(*getMainStateLattice(SCONTEXT)); i++)
    {
        error = DynamicLookupEnvironmentStatus(SCONTEXT, i, &occupationBuffer);
        error_assert(error, "Dynamic lookup of environment occupation and energy failed.");
    }

    delete_Span(occupationBuffer);
}

void SetEnvStateStatusToDefault(__SCONTEXT_PAR, const int32_t environmentId, const byte_t particleId)
{
    env_state_t* environment = getEnvironmentStateById(SCONTEXT, environmentId);

    environment->ParticleId = particleId;
    environment->EnvironmentId = environmentId;
    environment->IsMobile = false;
    environment->IsStable = (particleId == PARTICLE_VOID) ? false : true;
    environment->PositionVector = Vector4FromInt32(environmentId, getLatticeBlockSizes(SCONTEXT));
    environment->EnvironmentDefinition = getEnvironmentModelById(SCONTEXT, environment->PositionVector.d);
}

/* Simulation routines KMC and MMC */

static inline void Set_ActiveWorkEnvironmentByEnvLink(__SCONTEXT_PAR, env_link_t* restrict environmentLink)
{
    SCONTEXT->CycleState.WorkEnvironment = getEnvironmentStateById(SCONTEXT, environmentLink->EnvironmentId);
}

static inline void Set_ActiveWorkClusterByEnvAndId(__SCONTEXT_PAR, env_state_t* restrict environment, const byte_t clusterId)
{
    SCONTEXT->CycleState.WorkCluster = getEnvironmentCluStateById(environment, clusterId);
}

static inline void Set_ActiveWorkPairEnergyTable(__SCONTEXT_PAR, env_state_t* restrict environment, env_link_t* restrict environmentLink)
{
    SCONTEXT->CycleState.WorkPairTable = getPairEnergyTableById(SCONTEXT, getEnvironmentPairDefById(environment, environmentLink->PairId)->TableId);
}

static inline void Set_ActiveWorkClusterEnergyTable(__SCONTEXT_PAR, env_state_t* restrict environment, clu_link_t* restrict clusterLink)
{
    SCONTEXT->CycleState.WorkClusterTable = getClusterEnergyTableById(SCONTEXT, getEnvironmentCluDefById(environment, clusterLink->ClusterId)->TableId);
}

static inline int32_t FindClusterCodeIdInClusterTable(const clu_table_t* restrict clusterTable, const occode_t code)
{
    return FindClusterCodeIdByLinearSearch(clusterTable, code);
}

static inline double CalcPairEnergyDelta(const pair_table_t* restrict pairTable, const byte_t mainId, const byte_t oldId, const byte_t newId)
{
    return getPairEnergyTableEntry(pairTable, mainId, oldId) - getPairEnergyTableEntry(pairTable, mainId, newId);
}

static inline double CalcClusterEnergyDelta(const clu_table_t* restrict clusterTable, const clu_state_t* restrict cluster, const byte_t particleId)
{
    return getCluEnergyTableEntry(clusterTable, particleId, cluster->CodeId) - getCluEnergyTableEntry(clusterTable, particleId, cluster->CodeIdBackup);
}

static inline void UpdateClusterState(const clu_table_t* restrict clusterTable, const clu_link_t* restrict clusterLink, clu_state_t* restrict cluster, const byte_t newParticleId)
{
    SetCodeByteAt(&cluster->OccupationCode, clusterLink->CodeByteId, newParticleId);
    cluster->CodeId = FindClusterCodeIdInClusterTable(clusterTable, cluster->OccupationCode);
}

static inline void InvokeDeltaOfActivePair(__SCONTEXT_PAR, const byte_t updateParticleId, const byte_t oldParticleId, const byte_t newParticleId)
{
    *getActiveStateEnergyById(SCONTEXT, updateParticleId) += CalcPairEnergyDelta(getActivePairTable(SCONTEXT), updateParticleId, oldParticleId, newParticleId);
}

static inline void InvokeDeltaOfActiveCluster(__SCONTEXT_PAR, const byte_t updateParticleId)
{
    *getActiveStateEnergyById(SCONTEXT, updateParticleId) += CalcClusterEnergyDelta(getActiveClusterTable(SCONTEXT), getActiveWorkCluster(SCONTEXT), updateParticleId);
}

static void InvokeEnvLinkCluUpdates(__SCONTEXT_PAR, const env_link_t* restrict environmentLink, const byte_t newParticleId)
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

static void InvokeAllEnvLinkUpdates(__SCONTEXT_PAR, const env_link_t* restrict environmentLink, const byte_t oldParticleId, const byte_t newParticleId)
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

static inline void InvokeLocalEnvLinkClusterDeltas(__SCONTEXT_PAR, const env_link_t* restrict environmentLink, const byte_t updateParticleId)
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

static inline void PrepareJumpLinkClusterStateChanges(__SCONTEXT_PAR, const jump_link_t* restrict jumpLink)
{
    env_link_t* environmentLink = getEnvLinkByJumpLink(SCONTEXT, jumpLink);
    Set_ActiveWorkEnvironmentByEnvLink(SCONTEXT, environmentLink);

    cpp_foreach(clusterLink, environmentLink->ClusterLinks)
    {
        Set_ActiveWorkClusterByEnvAndId(SCONTEXT, getActiveWorkEnvironment(SCONTEXT), clusterLink->ClusterId);
        byte_t newCodeByte = GetCodeByteAt(&getActiveJumpRule(SCONTEXT)->StateCode2, jumpLink->PathId);
        SetCodeByteAt(&getActiveWorkCluster(SCONTEXT)->OccupationCode, clusterLink->CodeByteId, newCodeByte);
    }
}

static void InvokeJumpLinkDeltas(__SCONTEXT_PAR, const jump_link_t* restrict jumpLink)
{
    env_link_t* environmentLink = getEnvLinkByJumpLink(SCONTEXT, jumpLink);

    Set_ActiveWorkEnvironmentByEnvLink(SCONTEXT, environmentLink);
    Set_ActiveWorkPairEnergyTable(SCONTEXT, getActiveWorkEnvironment(SCONTEXT), environmentLink);

    byte_t newId = GetCodeByteAt(&getActiveJumpRule(SCONTEXT)->StateCode2, jumpLink->PathId);
    byte_t updateParticleId = GetCodeByteAt(&getActiveJumpRule(SCONTEXT)->StateCode2, getActiveWorkEnvironment(SCONTEXT)->PathId);

    InvokeActiveLocalPairDelta(SCONTEXT, updateParticleId, JUMPPATH[jumpLink->PathId]->ParticleId, newId);
    InvokeLocalEnvLinkClusterDeltas(SCONTEXT, environmentLink, updateParticleId);
}

static void InvokeEnvUpdateDistribution(__SCONTEXT_PAR, env_state_t* restrict environment, const byte_t newParticleId)
{
    cpp_foreach(environmentLink, environment->EnvironmentLinks)
    {
        Set_ActiveWorkEnvironmentByEnvLink(SCONTEXT, environmentLink);
        Set_ActiveWorkPairEnergyTable(SCONTEXT, getActiveWorkEnvironment(SCONTEXT), environmentLink);
        InvokeAllEnvLinkUpdates(SCONTEXT, environmentLink, environment->ParticleId, newParticleId);
    }
}

void CreateLocalJumpDeltaKmc(__SCONTEXT_PAR)
{
    cpp_foreach(jumpLink, getActiveJumpDirection(SCONTEXT)->JumpLinkSequence)
    {
        PrepareJumpLinkClusterStateChanges(SCONTEXT, jumpLink);
    }

    cpp_foreach(jumpLink, getActiveJumpDirection(SCONTEXT)->JumpLinkSequence)
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


void CreateLocalJumpDeltaMmc(__SCONTEXT_PAR)
{
    // Implement as soon as KMC functionality of delta principle is validated!
    // Note: Cannot be done jump link based, requires lookup based implementation
    //       that finds the environment-links beloging to the environment-Ids of both Path[0] and Path[1], respectively
    // Note: Possibly use a hash system that enables to directly detect if a link could be present or not
    error_assert(ERR_NOTIMPLEMENTED, "MMC currently not supported");
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