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
#include "Simulator/Logic/Routines/EnvRoutines.h"
#include "Simulator/Logic/Routines/HelperRoutines.h"
#include "Simulator/Data/Model/SimContext/ContextAccess.h"

/* Local helper routines */

static inline int32_t SaveFindClusterCodeIdByLinearSearch(const clu_table_t* restrict clusterTable, const occode_t occupationCode)
{
    int32_t index = 0;
    while ((clusterTable->OccupationCodes.Start[index] != occupationCode) && (index < clusterTable->OccupationCodes.Count))
    {
        index++;
    }
    return (index < clusterTable->OccupationCodes.Count) ? index : INVALID_INDEX;
}

static inline int32_t FindClusterCodeIdByLinearSearch(const clu_table_t* restrict clusterTable, const occode_t occupationCode)
{
    int32_t index = 0;
    while (clusterTable->OccupationCodes.Start[index++] != occupationCode) {};
    return index;
}

static inline int32_t BinaryLookupCluCodeId(const clu_table_t* restrict clusterTable, const occode_t occupationCode)
{
    // Placeholder, implement on optimization
    return -1;
}

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

static error_t SortAndBuildClusterLinks(clu_link_t* restrict linkBuffer, const int32_t count, clu_links_t* restrict clusterLinks)
{
    error_t error;
    buffer_t tmpBuffer;

    qsort(linkBuffer, count, sizeof(clu_link_t), (f_compare_t) CompareClusterLinks);

    error = AllocateBufferChecked(count, sizeof(clu_link_t), &tmpBuffer);
    return_if(error, error);

    CopyBuffer((byte_t*) linkBuffer, tmpBuffer.Start, sizeof(clu_link_t) * count);
    *clusterLinks = BUFFER_TO_ARRAY_WCOUNT(tmpBuffer, count, clu_links_t);

    return ERR_OK;
}

static error_t BuildClusterLinkingByPairId(const env_def_t* environmentDefinition, const int32_t pairId, clu_links_t* restrict clusterLinks)
{
    clu_link_t tmpLinkBuffer[sizeof(bitmask_t) * 256];
    byte_t clusterId = 0, relativeId = 0;
    int32_t linkCount = 0;

    FOR_EACH(const clu_def_t, clusterDefinition, environmentDefinition->ClusterDefinitions)
    {
        relativeId = 0;
        C_FOR_EACH(const int32_t, environmentPairId, clusterDefinition->EnvironmentPairIds)
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
        return (targetEnvironment->IsMobile) ? targetEnvironment->EnvironmentLinks.CurrentEnd++ : NULL;
    #else
        return targetEnvironment->EnvironmentLinks.CurrentEnd++;
    #endif
}

static void ResolvePairTargetAndIncreaseLinkCounter(__SCONTEXT_PAR, const env_state_t* restrict environment, const pair_def_t* restrict pairDefinition)
{
    env_state_t* targetEnvironment = ResolvePairDefTargetEnvironment(SCONTEXT, pairDefinition, environment);

    // Immobility OPT Part 1 and 2 - No incomming or outgoing updates for immobiles are required
    #if defined(OPT_LINK_ONLY_MOBILES)
        voidreturn_if(!environment->IsMobile || !targetEnvironment->IsMobile);
    #endif

    targetEnvironment->EnvironmentLinks.Count++;
}

static error_t SetAllLinkListCountersToRequiredSize(__SCONTEXT_PAR)
{
    FOR_EACH(env_state_t, environment, *Get_EnvironmentLattice(SCONTEXT))
    {
        FOR_EACH(pair_def_t, pairDefinition, environment->EnvironmentDefinition->PairDefinitions)
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

    FOR_EACH(env_state_t, environment, *Get_EnvironmentLattice(SCONTEXT))
    {
        int32_t linkCount = environment->EnvironmentLinks.Count;
        error = AllocateBufferChecked(linkCount, sizeof(env_link_t), &tmpBuffer);
        return_if(error, error);

        environment->EnvironmentLinks = BUFFER_TO_LIST_WCOUNT(tmpBuffer, linkCount, env_links_t);
    }
    return ERR_OK;
}


static error_t PrepareLinkingSystemConstruction(__SCONTEXT_PAR)
{
    error_t error;

    error = SetAllLinkListCountersToRequiredSize(SCONTEXT);
    return_if(error, error);

    error = AllocateEnvLinkListBuffersByPresetCounters(SCONTEXT);
    return error;
}

static error_t LinkEnvironmentToSurroundings(__SCONTEXT_PAR, env_state_t* restrict environment)
{
    error_t error;
    int32_t pairId = 0;

    FOR_EACH(const pair_def_t, pairDefinition, environment->EnvironmentDefinition->PairDefinitions)
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

    FOR_EACH(env_state_t, environment, *Get_EnvironmentLattice(SCONTEXT))
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
    ASSERT_ERROR(error, "Failed to prepare the environment linking system for construction.");

    error = ConstructPreparedLinkingSystem(SCONTEXT);
    ASSERT_ERROR(error, "Failed to construct the environment linking system.");
}

static error_t AllocateDynamicEnvOccupationBuffer(__SCONTEXT_PAR, buffer_t* restrict buffer)
{
    int32_t bufferSize = 0;

    FOR_EACH(env_def_t, environmentDefinition, *Get_EnvironmentModels(SCONTEXT))
    {       
        bufferSize = GET_MAX(bufferSize, environmentDefinition->PairDefinitions.Count);
    }

    return AllocateBufferChecked(bufferSize, sizeof(byte_t), buffer);
}

static env_state_t* PullEnvStateByInteraction(__SCONTEXT_PAR, env_state_t* restrict startEnvironment, const int32_t pairId)
{
    pair_def_t* pairDefinition = Get_EnvironmentPairDefById(startEnvironment, pairId);
    return ResolvePairDefTargetEnvironment(SCONTEXT, pairDefinition, startEnvironment);
}

static error_t WriteEnvOccupationToBuffer(__SCONTEXT_PAR, env_state_t* environment, buffer_t* restrict occupationBuffer)
{
    for (int32_t i = 0; i < Get_EnvironmentPairDefCount(environment); i++)
    {
        occupationBuffer->Start[i] = PullEnvStateByInteraction(SCONTEXT, environment, i)->ParticleId;
        return_if(occupationBuffer->Start[i] == PARTICLE_VOID, ERR_DATACONSISTENCY);
    }

    return ERR_OK;
}

static void ResetEnvStateBuffersToZero(env_state_t* restrict environment)
{
    FOR_EACH(double, energy, environment->EnergyStates)
    {
        *energy = 0;
    }
    FOR_EACH(clu_state_t, cluster, environment->ClusterStates)
    {
        *cluster = (clu_state_t) { 0, 0, 0ULL, 0ULL };
    }
}

static void AddEnvPairEnergyByOccupation(__SCONTEXT_PAR, env_state_t* restrict environment, buffer_t* restrict occupationBuffer)
{
    for (int32_t i = 0; i < environment->EnvironmentDefinition->PairDefinitions.Count; i++)
    {
        const pair_table_t* pairTable = Get_PairEnergyTableById(SCONTEXT, environment->EnvironmentDefinition->PairDefinitions.Start[i].TableId);       
        for (byte_t j = 0; environment->EnvironmentDefinition->PositionParticleIds[j] != PARTICLE_NULL; j++)
        {
            byte_t positionParticleId = environment->EnvironmentDefinition->PositionParticleIds[j];
            environment->EnergyStates.Start[positionParticleId] += Get_PairEnergyTableEntry(pairTable, positionParticleId, occupationBuffer->Start[i]);
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
    return_if(environment->ClusterStates.Count != environment->EnvironmentDefinition->ClusterDefinitions.Count, ERR_DATACONSISTENCY);

    error_t error;
    clu_state_t * cluster = environment->ClusterStates.Start;

    FOR_EACH(clu_def_t, clusterDefinition, environment->EnvironmentDefinition->ClusterDefinitions)
    {
        const clu_table_t* clusterTable = Get_ClusterEnergyTableById(SCONTEXT, clusterDefinition->TableId);
        for (byte_t i = 0; clusterDefinition->EnvironmentPairIds[i] != POSITION_NULL; i++)
        {
            byte_t codeByteId = clusterDefinition->EnvironmentPairIds[i];
            SetCodeByteAt(&cluster->OccupationCode, i, occupationBuffer->Start[codeByteId]);
        }

        error = InitializeClusterStateStatus(SCONTEXT, cluster, clusterTable);
        return_if(error != ERR_OK, ERR_DATACONSISTENCY);

        for (byte_t j = 0; environment->EnvironmentDefinition->PositionParticleIds[j] != PARTICLE_NULL; j++)
        {
            byte_t positionParticleId = environment->EnvironmentDefinition->PositionParticleIds[j];
            environment->EnergyStates.Start[positionParticleId] += Get_CluEnergyTableEntry(clusterTable, positionParticleId, cluster->CodeId);
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
    env_state_t* environment = Get_EnvironmentStateById(SCONTEXT, environmentId);

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
    ASSERT_ERROR(error, "Buffer creation for environment occupation lookup failed.");

    for (int32_t i = 0; i < Get_MainStateLattice(SCONTEXT)->Count; i++)
    {
        error = DynamicLookupEnvironmentStatus(SCONTEXT, i, &occupationBuffer);
        ASSERT_ERROR(error, "Dynamic lookup of environment occupation and energy failed.");
    }

    FreeBuffer(&occupationBuffer);
}

void SetEnvStateStatusToDefault(__SCONTEXT_PAR, const int32_t environmentId, const byte_t particleId)
{
    env_state_t* environment = Get_EnvironmentStateById(SCONTEXT, environmentId);

    environment->ParticleId = particleId;
    environment->EnvironmentId = environmentId;
    environment->IsMobile = false;
    environment->IsStable = (particleId == PARTICLE_VOID) ? false : true;
    environment->PositionVector = Vector4FromInt32(environmentId, Get_LatticeBlockSizes(SCONTEXT));
    environment->EnvironmentDefinition = Get_EnvironmentModelById(SCONTEXT, environment->PositionVector.d);
}

/* Simulation routines KMC and MMC */

static inline void Set_ActiveWorkEnvironmentByEnvLink(__SCONTEXT_PAR, env_link_t* restrict environmentLink)
{
    SCONTEXT->CycleState.WorkEnvironment = Get_EnvironmentStateById(SCONTEXT, environmentLink->EnvironmentId);
}

static inline void Set_ActiveWorkClusterByEnvAndId(__SCONTEXT_PAR, env_state_t* restrict environment, const byte_t clusterId)
{
    SCONTEXT->CycleState.WorkCluster = Get_EnvironmentCluStateById(environment, clusterId);
}

static inline void Set_ActiveWorkPairEnergyTable(__SCONTEXT_PAR, env_state_t* restrict environment, env_link_t* restrict environmentLink)
{
    SCONTEXT->CycleState.WorkPairTable = Get_PairEnergyTableById(SCONTEXT, Get_EnvironmentPairDefById(environment, environmentLink->PairId)->TableId);
}

static inline void Set_ActiveWorkClusterEnergyTable(__SCONTEXT_PAR, env_state_t* restrict environment, clu_link_t* restrict clusterLink)
{
    SCONTEXT->CycleState.WorkClusterTable = Get_ClusterEnergyTableById(SCONTEXT, Get_EnvironmentCluDefById(environment, clusterLink->ClusterId)->TableId);
}

static inline int32_t FindClusterCodeIdInClusterTable(const clu_table_t* restrict clusterTable, const occode_t code)
{
    return FindClusterCodeIdByLinearSearch(clusterTable, code);
}

static inline double CalcPairEnergyDelta(const pair_table_t* restrict pairTable, const byte_t mainId, const byte_t oldId, const byte_t newId)
{
    return Get_PairEnergyTableEntry(pairTable, mainId, oldId) - Get_PairEnergyTableEntry(pairTable, mainId, newId);
}

static inline double CalcClusterEnergyDelta(const clu_table_t* restrict clusterTable, const clu_state_t* restrict cluster, const byte_t particleId)
{
    return Get_CluEnergyTableEntry(clusterTable, particleId, cluster->CodeId) - Get_CluEnergyTableEntry(clusterTable, particleId, cluster->CodeIdBackup);
}

static inline void UpdateClusterState(const clu_table_t* restrict clusterTable, const clu_link_t* restrict clusterLink, clu_state_t* restrict cluster, const byte_t newParticleId)
{
    SetCodeByteAt(&cluster->OccupationCode, clusterLink->CodeByteId, newParticleId);
    cluster->CodeId = FindClusterCodeIdInClusterTable(clusterTable, cluster->OccupationCode);
}

static inline void InvokeDeltaOfActivePair(__SCONTEXT_PAR, const byte_t updateParticleId, const byte_t oldParticleId, const byte_t newParticleId)
{
    *Get_ActiveStateEnergyById(SCONTEXT, updateParticleId) += CalcPairEnergyDelta(Get_ActivePairTable(SCONTEXT), updateParticleId, oldParticleId, newParticleId);
}

static inline void InvokeDeltaOfActiveCluster(__SCONTEXT_PAR, const byte_t updateParticleId)
{
    *Get_ActiveStateEnergyById(SCONTEXT, updateParticleId) += CalcClusterEnergyDelta(Get_ActiveClusterTable(SCONTEXT), Get_ActiveWorkCluster(SCONTEXT), updateParticleId);
}

static void InvokeEnvLinkCluUpdates(__SCONTEXT_PAR, const env_link_t* restrict environmentLink, const byte_t newParticleId)
{
    FOR_EACH(clu_link_t, clusterLink, environmentLink->ClusterLinks)
    {
        Set_ActiveWorkClusterByEnvAndId(SCONTEXT, Get_ActiveWorkEnvironment(SCONTEXT), clusterLink->ClusterId);
        Set_ActiveWorkClusterEnergyTable(SCONTEXT, Get_ActiveWorkEnvironment(SCONTEXT), clusterLink);

        UpdateClusterState(Get_ActiveClusterTable(SCONTEXT), clusterLink, Get_ActiveWorkCluster(SCONTEXT), newParticleId);

        for (byte_t i = 0; Get_ActiveParticleUpdateIdAt(SCONTEXT, i) != PARTICLE_NULL; i++)
        {
            InvokeDeltaOfActiveCluster(SCONTEXT, i);
        }

        SetCluStateBackup(Get_ActiveWorkCluster(SCONTEXT));
    }
}

static void InvokeAllEnvLinkUpdates(__SCONTEXT_PAR, const env_link_t* restrict environmentLink, const byte_t oldParticleId, const byte_t newParticleId)
{
    for (byte_t i = 0; Get_ActiveParticleUpdateIdAt(SCONTEXT, i) != PARTICLE_NULL; i++)
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
    FOR_EACH(clu_link_t, clusterLink, environmentLink->ClusterLinks)
    {
        Set_ActiveWorkClusterByEnvAndId(SCONTEXT, Get_ActiveWorkEnvironment(SCONTEXT), clusterLink->ClusterId);
        if(Get_ActiveWorkCluster(SCONTEXT)->OccupationCode != Get_ActiveWorkCluster(SCONTEXT)->OccupationCodeBackup)
        {        
            Set_ActiveWorkClusterEnergyTable(SCONTEXT, Get_ActiveWorkEnvironment(SCONTEXT), clusterLink);
            InvokeDeltaOfActiveCluster(SCONTEXT, updateParticleId);
            LoadCluStateBackup(Get_ActiveWorkCluster(SCONTEXT));
        }
    }
}

static inline void PrepareJumpLinkClusterStateChanges(__SCONTEXT_PAR, const jump_link_t* restrict jumpLink)
{
    env_link_t* environmentLink = Get_EnvLinkByJumpLink(SCONTEXT, jumpLink);
    Set_ActiveWorkEnvironmentByEnvLink(SCONTEXT, environmentLink);

    FOR_EACH(clu_link_t, clusterLink, environmentLink->ClusterLinks)
    {
        Set_ActiveWorkClusterByEnvAndId(SCONTEXT, Get_ActiveWorkEnvironment(SCONTEXT), clusterLink->ClusterId);
        byte_t newCodeByte = GetCodeByteAt(&Get_ActiveJumpRule(SCONTEXT)->StateCode2, jumpLink->PathId);
        SetCodeByteAt(&Get_ActiveWorkCluster(SCONTEXT)->OccupationCode, clusterLink->CodeByteId, newCodeByte);
    }
}

static void InvokeJumpLinkDeltas(__SCONTEXT_PAR, const jump_link_t* restrict jumpLink)
{
    env_link_t* environmentLink = Get_EnvLinkByJumpLink(SCONTEXT, jumpLink);

    Set_ActiveWorkEnvironmentByEnvLink(SCONTEXT, environmentLink);
    Set_ActiveWorkPairEnergyTable(SCONTEXT, Get_ActiveWorkEnvironment(SCONTEXT), environmentLink);

    byte_t newId = GetCodeByteAt(&Get_ActiveJumpRule(SCONTEXT)->StateCode2, jumpLink->PathId);
    byte_t updateParticleId = GetCodeByteAt(&Get_ActiveJumpRule(SCONTEXT)->StateCode2, Get_ActiveWorkEnvironment(SCONTEXT)->PathId);

    InvokeActiveLocalPairDelta(SCONTEXT, updateParticleId, JUMPPATH[jumpLink->PathId]->ParticleId, newId);
    InvokeLocalEnvLinkClusterDeltas(SCONTEXT, environmentLink, updateParticleId);
}

static void InvokeEnvUpdateDistribution(__SCONTEXT_PAR, env_state_t* restrict environment, const byte_t newParticleId)
{
    FOR_EACH(env_link_t, environmentLink, environment->EnvironmentLinks)
    {
        Set_ActiveWorkEnvironmentByEnvLink(SCONTEXT, environmentLink);
        Set_ActiveWorkPairEnergyTable(SCONTEXT, Get_ActiveWorkEnvironment(SCONTEXT), environmentLink);
        InvokeAllEnvLinkUpdates(SCONTEXT, environmentLink, environment->ParticleId, newParticleId);
    }
}

void CreateLocalJumpDeltaKmc(__SCONTEXT_PAR)
{
    FOR_EACH(jump_link_t, jumpLink, Get_ActiveJumpDirection(SCONTEXT)->JumpLinkSequence)
    {
        PrepareJumpLinkClusterStateChanges(SCONTEXT, jumpLink);
    }
    FOR_EACH(jump_link_t, jumpLink, Get_ActiveJumpDirection(SCONTEXT)->JumpLinkSequence)
    {
        InvokeJumpLinkDeltas(SCONTEXT, jumpLink);
    }
}

void RollbackLocalJumpDeltaKmc(__SCONTEXT_PAR)
{
    for(byte_t i = 0; i < Get_ActiveJumpDirection(SCONTEXT)->JumpLength; i++)
    {
        *Get_PathStateEnergyByIds(SCONTEXT, i, JUMPPATH[i]->ParticleId) = *Get_EnvStateEnergyBackupById(SCONTEXT, i); 
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
    for(byte_t i = 0; i < Get_ActiveJumpDirection(SCONTEXT)->JumpLength;i++)
    {
        if(JUMPPATH[i]->IsStable)
        {
            byte_t particleId = GetCodeByteAt(&Get_ActiveJumpRule(SCONTEXT)->StateCode0, i);
            Get_JumpEnergyInfo(SCONTEXT)->Energy0 = *Get_PathStateEnergyByIds(SCONTEXT, i, particleId);
        }
        else
        {
            byte_t particleId = GetCodeByteAt(&Get_ActiveJumpRule(SCONTEXT)->StateCode1, i);
            Get_JumpEnergyInfo(SCONTEXT)->Energy1 = *Get_PathStateEnergyByIds(SCONTEXT, i, particleId);
        }
    }
}

void SetState2EnergyKmc(__SCONTEXT_PAR)
{
    for(byte_t i = 0; i < Get_ActiveJumpDirection(SCONTEXT)->JumpLength;i++)
    {
        byte_t particleId = GetCodeByteAt(&Get_ActiveJumpRule(SCONTEXT)->StateCode2, i);
        Get_JumpEnergyInfo(SCONTEXT)->Energy2 = *Get_PathStateEnergyByIds(SCONTEXT, i, particleId); 
    }
}

void AdvanceKmcSystemToState2(__SCONTEXT_PAR)
{
    for(byte_t i = 0; i < Get_ActiveJumpDirection(SCONTEXT)->JumpLength; i++)
    {
        byte_t newId = GetCodeByteAt(&Get_ActiveJumpRule(SCONTEXT)->StateCode2, i);
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
    ASSERT_ERROR(ERR_NOTIMPLEMENTED, "MMC currently not supported");
}

void RollbackLocalJumpDeltaMmc(__SCONTEXT_PAR)
{
    *Get_PathStateEnergyByIds(SCONTEXT, 0, JUMPPATH[0]->ParticleId) = *Get_EnvStateEnergyBackupById(SCONTEXT, 0);
    *Get_PathStateEnergyByIds(SCONTEXT, 1, JUMPPATH[1]->ParticleId) = *Get_EnvStateEnergyBackupById(SCONTEXT, 1);
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
    byte_t parId0 = GetCodeByteAt(&Get_ActiveJumpRule(SCONTEXT)->StateCode0, 0);
    Get_JumpEnergyInfo(SCONTEXT)->Energy0 =  *Get_PathStateEnergyByIds(SCONTEXT, 0, parId0);

    byte_t parId1 = GetCodeByteAt(&Get_ActiveJumpRule(SCONTEXT)->StateCode0, 1);
    Get_JumpEnergyInfo(SCONTEXT)->Energy0 += *Get_PathStateEnergyByIds(SCONTEXT, 1, parId1);
}

void SetState2EnergyMmc(__SCONTEXT_PAR)
{
    byte_t parId0 = GetCodeByteAt(&Get_ActiveJumpRule(SCONTEXT)->StateCode2, 0);
    Get_JumpEnergyInfo(SCONTEXT)->Energy2 =  *Get_PathStateEnergyByIds(SCONTEXT, 0, parId0);

    byte_t parId1 = GetCodeByteAt(&Get_ActiveJumpRule(SCONTEXT)->StateCode2, 1);
    Get_JumpEnergyInfo(SCONTEXT)->Energy2 += *Get_PathStateEnergyByIds(SCONTEXT, 1, parId1);
}

void AdvanceMmcSystemToState2(__SCONTEXT_PAR)
{
    byte_t newParId0 = GetCodeByteAt(&Get_ActiveJumpRule(SCONTEXT)->StateCode2, 0);
    byte_t newParId1 = GetCodeByteAt(&Get_ActiveJumpRule(SCONTEXT)->StateCode2, 1);
    InvokeEnvUpdateDistribution(SCONTEXT, JUMPPATH[0], newParId0);
    InvokeEnvUpdateDistribution(SCONTEXT, JUMPPATH[1], newParId1);
    JUMPPATH[0]->ParticleId = newParId0;
    JUMPPATH[1]->ParticleId = newParId1;
}