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

static inline int32_t SaveFindClusterCodeIdByLinearSearch(const clu_table_t* restrict table, const occode_t code)
{
    int32_t index = 0;
    while ((table->OccCodes.Start[index] != code) && (index < table->OccCodes.Count))
    {
        index++;
    }
    return (index < table->OccCodes.Count) ? index : INVALID_INDEX;
}

static inline int32_t FindClusterCodeIdByLinearSearch(const clu_table_t* restrict table, const occode_t code)
{
    int32_t index = 0;
    while (table->OccCodes.Start[index++] != code) {};
    return index;
}

static inline int32_t BinaryLookupCluCodeId(const clu_table_t* restrict table, const occode_t code)
{
    // Placeholder, implement on optimization
    return -1;
}

static inline void SetCluStateBackup(clu_state_t* restrict clu)
{
    clu->CodeIdBackup = clu->CodeId;
    clu->OccCodeBackup = clu->OccCode;
}

static inline void LoadCluStateBackup(clu_state_t* restrict clu)
{
    clu->CodeId = clu->CodeIdBackup;
    clu->OccCode = clu->OccCodeBackup;
}

/* Initializer routines */

static int32_t CompareClusterLinks(const clu_link_t* lhs, const clu_link_t* rhs)
{
    int32_t value = get_compare(lhs->CluId, rhs->CluId);
    if (value)
    {
        return get_compare(lhs->RelId, rhs->RelId);
    }
    return value;
}

static error_t SortAndBuildClusterLinks(clu_link_t* restrict linkBuffer, const int32_t count, clu_links_t* restrict cluLinks)
{
    error_t error;
    buffer_t tmpBuffer;

    qsort(linkBuffer, count, sizeof(clu_link_t), (f_compare_t) CompareClusterLinks);

    error = AllocateBufferChecked(count, sizeof(clu_link_t), &tmpBuffer);
    return_if(error, error);

    CopyBuffer((byte_t*) linkBuffer, tmpBuffer.Start, sizeof(clu_link_t) * count);
    *cluLinks = BUFFER_TO_ARRAY_WCOUNT(tmpBuffer, count, clu_links_t);

    return ERR_OK;
}

static error_t BuildClusterLinkingByPairId(const env_def_t* envDef, const int32_t pairId, clu_links_t* restrict cluLinks)
{
    clu_link_t tmpLinkBuffer[sizeof(bitmask_t) * 256];
    byte_t cluId = 0, relId = 0;
    int32_t linkCount = 0;

    FOR_EACH(const clu_def_t, cluDef, envDef->CluDefs)
    {
        relId = 0;
        C_FOR_EACH(const int32_t, relPosId, cluDef->RelPosIds)
        {
            if (*relPosId == pairId)
            {
                tmpLinkBuffer[linkCount] = (clu_link_t) { cluId , relId++ };
                linkCount++;
            }
        }
        cluId++;
    }

    return SortAndBuildClusterLinks(tmpLinkBuffer, linkCount, cluLinks);
}

static error_t InPlaceConstructEnvLink(const env_def_t* restrict envDef, const int32_t envId, const int32_t pairId, env_link_t* restrict envLink)
{
    error_t error;

    envLink->EnvId = envId;
    envLink->EnvPosId = pairId;
    error = BuildClusterLinkingByPairId(envDef, pairId, &envLink->CluLinks);

    return error;
}

static env_link_t* GetNextLinkFromTargetEnv(__SCONTEXT_PAR, const pair_def_t* restrict pairDef, env_state_t* restrict envState)
{
    env_state_t* targetEnv = ResolvePairDefTargetEnvironment(SCONTEXT, pairDef, envState);

    // Immobility OPT Part 2 - Providing outgoing updates through immobiles is not required, the link will not be triggered during the mc routine
    // Sideffects:  None at this point (ref. to OPT part 1)

    #if defined(OPT_LINK_ONLY_MOBILES)
        return (targetEnv->IsMobile) ? targetEnv->EnvLinks.CurEnd++ : NULL;
    #else
        return targetEnv->EnvLinks.CurEnd++;
    #endif
}

static void ResolvePairTargetAndIncreaseLinkCounter(__SCONTEXT_PAR, const env_state_t* restrict envState, const pair_def_t* restrict pairDef)
{
    env_state_t* targetEnv = ResolvePairDefTargetEnvironment(SCONTEXT, pairDef, envState);

    // Immobility OPT Part 1 and 2 - No incomming or outgoing updates for immobiles are required
    #if defined(OPT_LINK_ONLY_MOBILES)
        voidreturn_if(!envState->IsMobile || !targetEnv->IsMobile);
    #endif

    targetEnv->EnvLinks.Count++;
}

static error_t SetAllLinkListCountersToRequiredSize(__SCONTEXT_PAR)
{
    FOR_EACH(env_state_t, envState, *Get_EnvironmentLattice(SCONTEXT))
    {
        FOR_EACH(pair_def_t, pairDef, envState->EnvDef->PairDefs)
        {
            ResolvePairTargetAndIncreaseLinkCounter(SCONTEXT, envState, pairDef);
        }
    }
    return ERR_OK;
}

static error_t AllocateEnvLinkListBuffersByPresetCounters(__SCONTEXT_PAR)
{
    error_t error;
    buffer_t tmpBuffer;

    FOR_EACH(env_state_t, envState, *Get_EnvironmentLattice(SCONTEXT))
    {
        int32_t linkCount = envState->EnvLinks.Count;
        error = AllocateBufferChecked(linkCount, sizeof(env_link_t), &tmpBuffer);
        return_if(error, error);

        envState->EnvLinks = BUFFER_TO_LIST_WCOUNT(tmpBuffer, linkCount, env_links_t);
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

static error_t LinkEnvironmentToSurroundings(__SCONTEXT_PAR, env_state_t* restrict envState)
{
    error_t error;
    int32_t pairId = 0;

    FOR_EACH(const pair_def_t, pairDef, envState->EnvDef->PairDefs)
    {
        env_link_t* envLink = GetNextLinkFromTargetEnv(SCONTEXT, pairDef, envState);
        if (envLink != NULL)
        {
            error = InPlaceConstructEnvLink(envState->EnvDef, envState->EnvId, pairId, envLink);
            return_if(error, error);
        }
        pairId++;
    }

    return ERR_OK;
}

static error_t ConstructPreparedLinkingSystem(__SCONTEXT_PAR)
{
    error_t error;

    FOR_EACH(env_state_t, envState, *Get_EnvironmentLattice(SCONTEXT))
    {
        // Immobility OPT Part 1 -> Incomming updates are not required, the state energy of immobiles is not used during mc routine
        // Sideffect:   Causes all immobiles to remain at their initial energy state during simulation (can be resynchronized by dynamic lookup)
        #if defined(OPT_LINK_ONLY_MOBILES)
            continue_if(!envState->IsMobile);
        #endif

        error = LinkEnvironmentToSurroundings(SCONTEXT, envState);
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

    FOR_EACH(env_def_t, envDef, *Get_EnvironmentModels(SCONTEXT))
    {       
        bufferSize = GET_MAX(bufferSize, envDef->PairDefs.Count);
    }

    return AllocateBufferChecked(bufferSize, sizeof(byte_t), buffer);
}

static env_state_t* PullEnvStateByInteraction(__SCONTEXT_PAR, env_state_t* restrict startEnv, const int32_t pairId)
{
    pair_def_t* pairDef = Get_EnvironmentPairDefById(startEnv, pairId);
    return ResolvePairDefTargetEnvironment(SCONTEXT, pairDef, startEnv);
}

static error_t WriteEnvOccupationToBuffer(__SCONTEXT_PAR, env_state_t* envState, buffer_t* restrict occBuffer)
{
    for (int32_t i = 0; i < Get_EnvironmentPairDefCount(envState); i++)
    {
        occBuffer->Start[i] = PullEnvStateByInteraction(SCONTEXT, envState, i)->ParId;
        return_if(occBuffer->Start[i] == PARTICLE_VOID, ERR_DATACONSISTENCY);
    }

    return ERR_OK;
}

static void ResetEnvStateBuffersToZero(env_state_t* restrict envState)
{
    FOR_EACH(double, it, envState->EnergyStates)
    {
        *it = 0;
    }
    FOR_EACH(clu_state_t, it, envState->ClusterStates)
    {
        *it = (clu_state_t) { 0, 0, 0ULL, 0ULL };
    }
}

static void AddEnvPairEnergyByOccupation(__SCONTEXT_PAR, env_state_t* restrict envState, buffer_t* restrict occBuffer)
{
    for (int32_t i = 0; i < envState->EnvDef->PairDefs.Count; i++)
    {
        const pair_table_t* pairTable = Get_PairEnergyTableById(SCONTEXT, envState->EnvDef->PairDefs.Start[i].TabId);       
        for (byte_t j = 0; envState->EnvDef->PosParIds[j] != PARTICLE_NULL; j++)
        {
            byte_t posParId = envState->EnvDef->PosParIds[j];
            envState->EnergyStates.Start[posParId] += Get_PairEnergyTableEntry(pairTable, posParId, occBuffer->Start[i]);
        }
    } 
}

static error_t InitializeClusterStateStatus(__SCONTEXT_PAR, clu_state_t* restrict cluState, const clu_table_t* restrict table)
{
    cluState->CodeId = SaveFindClusterCodeIdByLinearSearch(table, cluState->OccCode);
    return_if(cluState->CodeId == INVALID_INDEX, ERR_DATACONSISTENCY);

    SetCluStateBackup(cluState);
    return ERR_OK;
}

static error_t AddEnvClusterEnergyByOccupation(__SCONTEXT_PAR, env_state_t* restrict envState, buffer_t* restrict occBuffer)
{
    return_if(envState->ClusterStates.Count != envState->EnvDef->CluDefs.Count, ERR_DATACONSISTENCY);

    error_t error;
    clu_state_t * cluState = envState->ClusterStates.Start;

    FOR_EACH(clu_def_t, cluDef, envState->EnvDef->CluDefs)
    {
        const clu_table_t* cluTable = Get_ClusterEnergyTableById(SCONTEXT, cluDef->TabId);
        for (byte_t i = 0; cluDef->RelPosIds[i] != POSITION_NULL; i++)
        {
            byte_t posId = cluDef->RelPosIds[i];
            SetCodeByteAt(&cluState->OccCode, i, occBuffer->Start[posId]);
        }

        error = InitializeClusterStateStatus(SCONTEXT, cluState, cluTable);
        return_if(error != ERR_OK, ERR_DATACONSISTENCY);

        for (byte_t j = 0; envState->EnvDef->PosParIds[j] != PARTICLE_NULL; j++)
        {
            byte_t posParId = envState->EnvDef->PosParIds[j];
            envState->EnergyStates.Start[posParId] += Get_CluEnergyTableEntry(cluTable, posParId, cluState->CodeId);
        }
    }

    return ERR_OK;
}

static error_t SetEnvStateEnergyByOccupation(__SCONTEXT_PAR, env_state_t* restrict envState, buffer_t* restrict occBuffer)
{
    ResetEnvStateBuffersToZero(envState);
    AddEnvPairEnergyByOccupation(SCONTEXT, envState, occBuffer);
    return AddEnvClusterEnergyByOccupation(SCONTEXT, envState, occBuffer);
}

static error_t DynamicLookupEnvironmentStatus(__SCONTEXT_PAR, const int32_t envId, buffer_t* restrict occBuffer)
{
    error_t error;
    env_state_t* envState = Get_EnvironmentStateById(SCONTEXT, envId);

    error = WriteEnvOccupationToBuffer(SCONTEXT, envState, occBuffer);
    return_if(error, error);

    error = SetEnvStateEnergyByOccupation(SCONTEXT, envState, occBuffer);
    return_if(error, error);

    return ERR_OK;
}

void SyncEnvironmentEnergyStatus(__SCONTEXT_PAR)
{
    error_t error;
    buffer_t occBuffer;

    error = AllocateDynamicEnvOccupationBuffer(SCONTEXT, &occBuffer);
    ASSERT_ERROR(error, "Buffer creation for environment occupation lookup failed.");

    for (int32_t i = 0; i < Get_MainStateLattice(SCONTEXT)->Count; i++)
    {
        error = DynamicLookupEnvironmentStatus(SCONTEXT, i, &occBuffer);
        ASSERT_ERROR(error, "Dynamic lookup of environment occupation and energy failed.");
    }

    FreeBuffer(&occBuffer);
}

void SetEnvStateStatusToDefault(__SCONTEXT_PAR, const int32_t envId, const byte_t parId)
{
    env_state_t* envState = Get_EnvironmentStateById(SCONTEXT, envId);

    envState->ParId = parId;
    envState->EnvId = envId;
    envState->IsMobile = false;
    envState->IsStable = (parId == PARTICLE_VOID) ? false : true;
    envState->PosVector = Vector4FromInt32(envId, Get_LatticeBlockSizes(SCONTEXT));
    envState->EnvDef = Get_EnvironmentModelById(SCONTEXT, envState->PosVector.d);
}

/* Simulation routines KMC and MMC */

static inline void Set_ActiveWorkEnvironmentByEnvLink(__SCONTEXT_PAR, env_link_t* restrict envLink)
{
    SCONTEXT->CycleState.ActWorkEnv = Get_EnvironmentStateById(SCONTEXT, envLink->EnvId);
}

static inline void Set_ActiveWorkClusterByEnvAndId(__SCONTEXT_PAR, env_state_t* restrict env, const byte_t cluId)
{
    SCONTEXT->CycleState.ActWorkClu = Get_EnvironmentCluStateById(env, cluId);
}

static inline void Set_ActiveWorkPairEnergyTable(__SCONTEXT_PAR, env_state_t* restrict env, env_link_t* restrict envLink)
{
    SCONTEXT->CycleState.ActPairTable = Get_PairEnergyTableById(SCONTEXT, Get_EnvironmentPairDefById(env, envLink->EnvPosId)->TabId);
}

static inline void Set_ActiveWorkClusterEnergyTable(__SCONTEXT_PAR, env_state_t* restrict env, clu_link_t* restrict cluLink)
{
    SCONTEXT->CycleState.ActCluTable = Get_ClusterEnergyTableById(SCONTEXT, Get_EnvironmentCluDefById(env, cluLink->CluId)->TabId);
}

static inline int32_t FindClusterCodeIdInClusterTable(const clu_table_t* restrict table, const occode_t code)
{
    return FindClusterCodeIdByLinearSearch(table, code);
}

static inline double CalcPairEnergyDelta(const pair_table_t* restrict table, const byte_t mainId, const byte_t oldId, const byte_t newId)
{
    return Get_PairEnergyTableEntry(table, mainId, oldId) - Get_PairEnergyTableEntry(table, mainId, newId);
}

static inline double CalcClusterEnergyDelta(const clu_table_t* restrict table, const clu_state_t* restrict clu, const byte_t parId)
{
    return Get_CluEnergyTableEntry(table, parId, clu->CodeId) - Get_CluEnergyTableEntry(table, parId, clu->CodeIdBackup);
}

static inline void UpdateClusterState(const clu_table_t* restrict table, const clu_link_t* restrict cluLink, clu_state_t* restrict clu, const byte_t newId)
{
    SetCodeByteAt(&clu->OccCode, cluLink->RelId, newId);
    clu->CodeId = FindClusterCodeIdInClusterTable(table, clu->OccCode);
}

static inline void InvokeDeltaOfActivePair(__SCONTEXT_PAR, const byte_t uptId, const byte_t oldId, const byte_t newId)
{
    *Get_ActiveStateEnergyById(SCONTEXT, uptId) += CalcPairEnergyDelta(Get_ActivePairTable(SCONTEXT), uptId, oldId, newId);
}

static inline void InvokeDeltaOfActiveCluster(__SCONTEXT_PAR, const byte_t uptId)
{
    *Get_ActiveStateEnergyById(SCONTEXT, uptId) += CalcClusterEnergyDelta(Get_ActiveClusterTable(SCONTEXT), Get_ActiveWorkCluster(SCONTEXT), uptId);
}

static void InvokeEnvLinkCluUpdates(__SCONTEXT_PAR, const env_link_t* restrict envLink, const byte_t newId)
{
    FOR_EACH(clu_link_t, cluLink, envLink->CluLinks)
    {
        Set_ActiveWorkClusterByEnvAndId(SCONTEXT, Get_ActiveWorkEnvironment(SCONTEXT), cluLink->CluId);
        Set_ActiveWorkClusterEnergyTable(SCONTEXT, Get_ActiveWorkEnvironment(SCONTEXT), cluLink);

        UpdateClusterState(Get_ActiveClusterTable(SCONTEXT), cluLink, Get_ActiveWorkCluster(SCONTEXT), newId);

        for (byte_t i = 0; Get_ActiveParticleUpdateIdAt(SCONTEXT, i) != PARTICLE_NULL; i++)
        {
            InvokeDeltaOfActiveCluster(SCONTEXT, i);
        }

        SetCluStateBackup(Get_ActiveWorkCluster(SCONTEXT));
    }
}

static void InvokeAllEnvLinkUpdates(__SCONTEXT_PAR, const env_link_t* restrict envLink, const byte_t oldId, const byte_t newId)
{
    for (byte_t i = 0; Get_ActiveParticleUpdateIdAt(SCONTEXT, i) != PARTICLE_NULL; i++)
    {
        InvokeDeltaOfActivePair(SCONTEXT, i, oldId, newId);
    }
    InvokeEnvLinkCluUpdates(SCONTEXT, envLink, newId);
}

static inline void InvokeActiveLocalPairDelta(__SCONTEXT_PAR, const byte_t uptId, const byte_t oldId, const byte_t newId)
{
    InvokeDeltaOfActivePair(SCONTEXT, uptId, oldId, newId);
}

static inline void InvokeLocalEnvLinkClusterDeltas(__SCONTEXT_PAR, const env_link_t* restrict envLink, const byte_t uptId)
{
    FOR_EACH(clu_link_t, cluLink, envLink->CluLinks)
    {
        Set_ActiveWorkClusterByEnvAndId(SCONTEXT, Get_ActiveWorkEnvironment(SCONTEXT), cluLink->CluId);
        if(Get_ActiveWorkCluster(SCONTEXT)->OccCode != Get_ActiveWorkCluster(SCONTEXT)->OccCodeBackup)
        {        
            Set_ActiveWorkClusterEnergyTable(SCONTEXT, Get_ActiveWorkEnvironment(SCONTEXT), cluLink);
            InvokeDeltaOfActiveCluster(SCONTEXT, uptId);
            LoadCluStateBackup(Get_ActiveWorkCluster(SCONTEXT));
        }
    }
}

static inline void PrepareJumpLinkClusterStateChanges(__SCONTEXT_PAR, const jump_link_t* restrict jmpLink)
{
    env_link_t* envLink = Get_EnvLinkByJumpLink(SCONTEXT, jmpLink);
    Set_ActiveWorkEnvironmentByEnvLink(SCONTEXT, envLink);

    FOR_EACH(clu_link_t, cluLink, envLink->CluLinks)
    {
        Set_ActiveWorkClusterByEnvAndId(SCONTEXT, Get_ActiveWorkEnvironment(SCONTEXT), cluLink->CluId);
        byte_t newCodeByte = GetCodeByteAt(&Get_ActiveJumpRule(SCONTEXT)->StCode2, jmpLink->PathId);
        SetCodeByteAt(&Get_ActiveWorkCluster(SCONTEXT)->OccCode, cluLink->RelId, newCodeByte);
    }
}

static void InvokeJumpLinkDeltas(__SCONTEXT_PAR, const jump_link_t* restrict jmpLink)
{
    env_link_t* envLink = Get_EnvLinkByJumpLink(SCONTEXT, jmpLink);

    Set_ActiveWorkEnvironmentByEnvLink(SCONTEXT, envLink);
    Set_ActiveWorkPairEnergyTable(SCONTEXT, Get_ActiveWorkEnvironment(SCONTEXT), envLink);

    byte_t newId = GetCodeByteAt(&Get_ActiveJumpRule(SCONTEXT)->StCode2, jmpLink->PathId);
    byte_t uptId = GetCodeByteAt(&Get_ActiveJumpRule(SCONTEXT)->StCode2, Get_ActiveWorkEnvironment(SCONTEXT)->PathId);

    InvokeActiveLocalPairDelta(SCONTEXT, uptId, JUMPPATH[jmpLink->PathId]->ParId, newId);
    InvokeLocalEnvLinkClusterDeltas(SCONTEXT, envLink, uptId);
}

static void InvokeEnvUpdateDistribution(__SCONTEXT_PAR, env_state_t* restrict env, const byte_t newId)
{
    FOR_EACH(env_link_t, envLink, env->EnvLinks)
    {
        Set_ActiveWorkEnvironmentByEnvLink(SCONTEXT, envLink);
        Set_ActiveWorkPairEnergyTable(SCONTEXT, Get_ActiveWorkEnvironment(SCONTEXT), envLink);
        InvokeAllEnvLinkUpdates(SCONTEXT, envLink, env->ParId, newId);
    }
}

void CreateLocalJumpDeltaKmc(__SCONTEXT_PAR)
{
    FOR_EACH(jump_link_t, jmpLink, Get_ActiveJumpDirection(SCONTEXT)->JumpLinkSeq)
    {
        PrepareJumpLinkClusterStateChanges(SCONTEXT, jmpLink);
    }
    FOR_EACH(jump_link_t, jmpLink, Get_ActiveJumpDirection(SCONTEXT)->JumpLinkSeq)
    {
        InvokeJumpLinkDeltas(SCONTEXT, jmpLink);
    }
}

void RollbackLocalJumpDeltaKmc(__SCONTEXT_PAR)
{
    for(byte_t i = 0; i < Get_ActiveJumpDirection(SCONTEXT)->JumpLength; i++)
    {
        *Get_PathStateEnergyByIds(SCONTEXT, i, JUMPPATH[i]->ParId) = *Get_EnvStateEnergyBackupById(SCONTEXT, i); 
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
            byte_t parId = GetCodeByteAt(&Get_ActiveJumpRule(SCONTEXT)->StCode0, i);
            Get_JumpEnergyInfo(SCONTEXT)->Eng0 = *Get_PathStateEnergyByIds(SCONTEXT, i, parId);
        }
        else
        {
            byte_t parId = GetCodeByteAt(&Get_ActiveJumpRule(SCONTEXT)->StCode1, i);
            Get_JumpEnergyInfo(SCONTEXT)->Eng1 = *Get_PathStateEnergyByIds(SCONTEXT, i, parId);
        }
    }
}

void SetState2EnergyKmc(__SCONTEXT_PAR)
{
    for(byte_t i = 0; i < Get_ActiveJumpDirection(SCONTEXT)->JumpLength;i++)
    {
        byte_t parId = GetCodeByteAt(&Get_ActiveJumpRule(SCONTEXT)->StCode2, i);
        Get_JumpEnergyInfo(SCONTEXT)->Eng2 = *Get_PathStateEnergyByIds(SCONTEXT, i, parId); 
    }
}

void AdvanceKmcSystemToState2(__SCONTEXT_PAR)
{
    for(byte_t i = 0; i < Get_ActiveJumpDirection(SCONTEXT)->JumpLength; i++)
    {
        byte_t newId = GetCodeByteAt(&Get_ActiveJumpRule(SCONTEXT)->StCode2, i);
        if(JUMPPATH[i]->IsStable)
        {
            InvokeEnvUpdateDistribution(SCONTEXT, JUMPPATH[i], newId);
        }
        JUMPPATH[i]->ParId = newId;
    }
}


void CreateLocalJumpDeltaMmc(__SCONTEXT_PAR)
{
    FOR_EACH(jump_link_t, jmpLink, Get_ActiveJumpDirection(SCONTEXT)->JumpLinkSeq)
    {
        PrepareJumpLinkClusterStateChanges(SCONTEXT, jmpLink);
    }
    FOR_EACH(jump_link_t, jmpLink, Get_ActiveJumpDirection(SCONTEXT)->JumpLinkSeq)
    {
        InvokeJumpLinkDeltas(SCONTEXT, jmpLink);
    }
}

void RollbackLocalJumpDeltaMmc(__SCONTEXT_PAR)
{
    *Get_PathStateEnergyByIds(SCONTEXT, 0, JUMPPATH[0]->ParId) = *Get_EnvStateEnergyBackupById(SCONTEXT, 0);
    *Get_PathStateEnergyByIds(SCONTEXT, 1, JUMPPATH[1]->ParId) = *Get_EnvStateEnergyBackupById(SCONTEXT, 1);
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
    byte_t parId0 = GetCodeByteAt(&Get_ActiveJumpRule(SCONTEXT)->StCode0, 0);
    Get_JumpEnergyInfo(SCONTEXT)->Eng0 =  *Get_PathStateEnergyByIds(SCONTEXT, 0, parId0);

    byte_t parId1 = GetCodeByteAt(&Get_ActiveJumpRule(SCONTEXT)->StCode0, 1);
    Get_JumpEnergyInfo(SCONTEXT)->Eng0 += *Get_PathStateEnergyByIds(SCONTEXT, 1, parId1);
}

void SetState2EnergyMmc(__SCONTEXT_PAR)
{
    byte_t parId0 = GetCodeByteAt(&Get_ActiveJumpRule(SCONTEXT)->StCode2, 0);
    Get_JumpEnergyInfo(SCONTEXT)->Eng2 =  *Get_PathStateEnergyByIds(SCONTEXT, 0, parId0);

    byte_t parId1 = GetCodeByteAt(&Get_ActiveJumpRule(SCONTEXT)->StCode2, 1);
    Get_JumpEnergyInfo(SCONTEXT)->Eng2 += *Get_PathStateEnergyByIds(SCONTEXT, 1, parId1);
}

void AdvanceMmcSystemToState2(__SCONTEXT_PAR)
{
    byte_t newParId0 = GetCodeByteAt(&Get_ActiveJumpRule(SCONTEXT)->StCode2, 0);
    byte_t newParId1 = GetCodeByteAt(&Get_ActiveJumpRule(SCONTEXT)->StCode2, 1);
    InvokeEnvUpdateDistribution(SCONTEXT, JUMPPATH[0], newParId0);
    InvokeEnvUpdateDistribution(SCONTEXT, JUMPPATH[1], newParId1);
    JUMPPATH[0]->ParId = newParId0;
    JUMPPATH[1]->ParId = newParId1;
}