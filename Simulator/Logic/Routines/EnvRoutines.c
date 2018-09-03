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

static inline int32_t SaveLinearLookupCluCodeId(const clu_table_t* restrict table, const occode_t code)
{
    int32_t index = 0;
    while ((table->OccCodes.Start[index] != code) && (index < table->OccCodes.Count))
    {
        index++;
    }
    return (index < table->OccCodes.Count) ? index : INVALID_INDEX;
}

static inline int32_t LinearLookupCluCodeId(const clu_table_t* restrict table, const occode_t code)
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
    cluState->CodeId = SaveLinearLookupCluCodeId(table, cluState->OccCode);
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

static inline void SetActWorkEnv(__SCONTEXT_PAR, const env_link_t* restrict envLink)
{
    SCONTEXT->CycleState.ActWorkEnv = RefLatticeEnvAt(SCONTEXT, envLink->EnvId);
}

static inline void SetActWorkClu(__SCONTEXT_PAR, const env_state_t* restrict env, const byte_t cluId)
{
    SCONTEXT->CycleState.ActWorkClu = RefCluStateAt(env, cluId);
}

static inline void SetActWorkPairTable(__SCONTEXT_PAR, const env_state_t* restrict env, const env_link_t* restrict envLink)
{
    SCONTEXT->CycleState.ActPairTable = RefPairTableAt(SCONTEXT, RefEnvPairDefAt(env, envLink->EnvPosId)->TabId);
}

static inline void SetActWorkCluTable(__SCONTEXT_PAR, const env_state_t* restrict env, const clu_link_t* restrict cluLink)
{
    SCONTEXT->CycleState.ActCluTable = RefCluTableAt(SCONTEXT, RefEnvCluDefAt(env, cluLink->CluId)->TabId);
}

static inline clu_state_t GetCluStateCopy(const env_state_t* restrict env, const int32_t cluId)
{
    return env->ClusterStates.Start[cluId];
}

static inline int32_t LookupCluTableCodeId(const clu_table_t* restrict table, const occode_t code)
{
    return LinearLookupCluCodeId(table, code);
}

static inline double GetPairDelta(const pair_table_t* restrict table, const byte_t mainId, const byte_t oldId, const byte_t newId)
{
    return GetPairTableEntry(table, mainId, oldId) - GetPairTableEntry(table, mainId, newId);
}

static inline double GetCluDelta(const clu_table_t* restrict table, const clu_state_t* restrict clu, const byte_t parId)
{
    return GetCluTableEntry(table, parId, clu->CodeId) - GetCluTableEntry(table, parId, clu->CodeIdBackup);
}

static inline void UpdateCluState(const clu_table_t* restrict table, const clu_link_t* restrict cluLink, clu_state_t* restrict clu, const byte_t newId)
{
    SetCodeByteAt(&clu->OccCode, cluLink->RelId, newId);
    clu->CodeId = LookupCluTableCodeId(table, clu->OccCode);
}

static inline void InvokeDeltaOfActivePair(__SCONTEXT_PAR, const byte_t uptId, const byte_t oldId, const byte_t newId)
{
    *RefActStateEngAt(SCONTEXT, uptId) += GetPairDelta(RefActPairTable(SCONTEXT), uptId, oldId, newId);
}

static inline void InvokeDeltaOfActiveClu(__SCONTEXT_PAR, const byte_t uptId)
{
    *RefActStateEngAt(SCONTEXT, uptId) += GetCluDelta(RefActCluTable(SCONTEXT), RefActWorkClu(SCONTEXT), uptId);
}

static void InvokeEnvLinkCluUpdates(__SCONTEXT_PAR, const env_link_t* restrict envLink, const byte_t newId)
{
    FOR_EACH(clu_link_t, cluLink, envLink->CluLinks)
    {
        SetActWorkClu(SCONTEXT, RefActWorkEnv(SCONTEXT), cluLink->CluId);
        SetActWorkCluTable(SCONTEXT, RefActWorkEnv(SCONTEXT), cluLink);

        UpdateCluState(RefActCluTable(SCONTEXT), cluLink, RefActWorkClu(SCONTEXT), newId);

        for (byte_t i = 0; GetActUpdateIdAt(SCONTEXT, i) != PARTICLE_NULL; i++)
        {
            InvokeDeltaOfActiveClu(SCONTEXT, i);
        }

        SetCluStateBackup(RefActWorkClu(SCONTEXT));
    }
}

static void InvokeAllEnvLinkUpdates(__SCONTEXT_PAR, const env_link_t* restrict envLink, const byte_t oldId, const byte_t newId)
{
    for (byte_t i = 0; GetActUpdateIdAt(SCONTEXT, i) != PARTICLE_NULL; i++)
    {
        InvokeDeltaOfActivePair(SCONTEXT, i, oldId, newId);
    }
    InvokeEnvLinkCluUpdates(SCONTEXT, envLink, newId);
}

static inline void InvokeActLocalPairDelta(__SCONTEXT_PAR, const byte_t uptId, const byte_t oldId, const byte_t newId)
{
    InvokeDeltaOfActivePair(SCONTEXT, uptId, oldId, newId);
}

static inline void InvokeLocalEnvLinkCluDeltas(__SCONTEXT_PAR, const env_link_t* restrict envLink, const byte_t uptId)
{
    FOR_EACH(clu_link_t, cluLink, envLink->CluLinks)
    {
        SetActWorkClu(SCONTEXT, RefActWorkEnv(SCONTEXT), cluLink->CluId);
        if(RefActWorkClu(SCONTEXT)->OccCode != RefActWorkClu(SCONTEXT)->OccCodeBackup)
        {        
            SetActWorkCluTable(SCONTEXT, RefActWorkEnv(SCONTEXT), cluLink);
            InvokeDeltaOfActiveClu(SCONTEXT, uptId);
            LoadCluStateBackup(RefActWorkClu(SCONTEXT));
        }
    }
}

static inline void PrepareJumpLinkCluStateChanges(__SCONTEXT_PAR, const jump_link_t* restrict jmpLink)
{
    env_link_t* envLink = RefEnvLinkByJmpLink(SCONTEXT, jmpLink);
    SetActWorkEnv(SCONTEXT, envLink);

    FOR_EACH(clu_link_t, cluLink, envLink->CluLinks)
    {
        SetActWorkClu(SCONTEXT, RefActWorkEnv(SCONTEXT), cluLink->CluId);
        SetCodeByteAt(&RefActWorkClu(SCONTEXT)->OccCode, cluLink->RelId, GetCodeByteAt(&RefActJumpRule(SCONTEXT)->StCode2, jmpLink->PathId));
    }
}

static void InvokeJumpLinkDeltas(__SCONTEXT_PAR, const jump_link_t* restrict jmpLink)
{
    env_link_t* envLink = RefEnvLinkByJmpLink(SCONTEXT, jmpLink);

    SetActWorkEnv(SCONTEXT, envLink);
    SetActWorkPairTable(SCONTEXT, RefActWorkEnv(SCONTEXT), envLink);

    byte_t newId = GetCodeByteAt(&RefActJumpRule(SCONTEXT)->StCode2, jmpLink->PathId);
    byte_t uptId = GetCodeByteAt(&RefActJumpRule(SCONTEXT)->StCode2, RefActWorkEnv(SCONTEXT)->PathId);

    InvokeActLocalPairDelta(SCONTEXT, uptId, JUMPPATH[jmpLink->PathId]->ParId, newId);
    InvokeLocalEnvLinkCluDeltas(SCONTEXT, envLink, uptId);
}

static void InvokeEnvUpdateDistribution(__SCONTEXT_PAR, env_state_t* restrict env, const byte_t newId)
{
    FOR_EACH(env_link_t, envLink, env->EnvLinks)
    {
        SetActWorkEnv(SCONTEXT, envLink);
        SetActWorkPairTable(SCONTEXT, RefActWorkEnv(SCONTEXT), envLink);
        InvokeAllEnvLinkUpdates(SCONTEXT, envLink, env->ParId, newId);
    }
}

void CreateLocalJumpDeltaKmc(__SCONTEXT_PAR)
{
    FOR_EACH(jump_link_t, jmpLink, RefActJumpDir(SCONTEXT)->JumpLinkSeq)
    {
        PrepareJumpLinkCluStateChanges(SCONTEXT, jmpLink);
    }
    FOR_EACH(jump_link_t, jmpLink, RefActJumpDir(SCONTEXT)->JumpLinkSeq)
    {
        InvokeJumpLinkDeltas(SCONTEXT, jmpLink);
    }
}

void RollbackLocalJumpDeltaKmc(__SCONTEXT_PAR)
{
    for(byte_t i = 0; i < RefActJumpDir(SCONTEXT)->JumpLength; i++)
    {
        *RefPathStateEngAt(SCONTEXT, i, JUMPPATH[i]->ParId) = GetStateEnvBackupEngAt(SCONTEXT, i); 
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
    for(byte_t i = 0; i < RefActJumpDir(SCONTEXT)->JumpLength;i++)
    {
        if(JUMPPATH[i]->IsStable)
        {
            RefActEngInfo(SCONTEXT)->Eng0 =  GetPathStateEngAt(SCONTEXT, i, GetCodeByteAt(&RefActJumpRule(SCONTEXT)->StCode0, i));
        }
        else
        {
            RefActEngInfo(SCONTEXT)->Eng1 =  GetPathStateEngAt(SCONTEXT, i, GetCodeByteAt(&RefActJumpRule(SCONTEXT)->StCode1, i));
        }
    }
}

void SetState2EnergyKmc(__SCONTEXT_PAR)
{
    for(byte_t i = 0; i < RefActJumpDir(SCONTEXT)->JumpLength;i++)
    {
        RefActEngInfo(SCONTEXT)->Eng2 =  GetPathStateEngAt(SCONTEXT, i, GetCodeByteAt(&RefActJumpRule(SCONTEXT)->StCode2, i)); 
    }
}

void AdvanceKmcSystemToState2(__SCONTEXT_PAR)
{
    for(byte_t i = 0; i < RefActJumpDir(SCONTEXT)->JumpLength; i++)
    {
        byte_t newId = GetCodeByteAt(&RefActJumpRule(SCONTEXT)->StCode2, i);
        if(JUMPPATH[i]->IsStable)
        {
            InvokeEnvUpdateDistribution(SCONTEXT, JUMPPATH[i], newId);
        }
        JUMPPATH[i]->ParId = newId;
    }
}


void CreateLocalJumpDeltaMmc(__SCONTEXT_PAR)
{
    FOR_EACH(jump_link_t, jmpLink, RefActJumpDir(SCONTEXT)->JumpLinkSeq)
    {
        PrepareJumpLinkCluStateChanges(SCONTEXT, jmpLink);
    }
    FOR_EACH(jump_link_t, jmpLink, RefActJumpDir(SCONTEXT)->JumpLinkSeq)
    {
        InvokeJumpLinkDeltas(SCONTEXT, jmpLink);
    }
}

void RollbackLocalJumpDeltaMmc(__SCONTEXT_PAR)
{
    *RefPathStateEngAt(SCONTEXT, 0, JUMPPATH[0]->ParId) = GetStateEnvBackupEngAt(SCONTEXT, 0);
    *RefPathStateEngAt(SCONTEXT, 1, JUMPPATH[1]->ParId) = GetStateEnvBackupEngAt(SCONTEXT, 1);
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
    Get_JumpEnergyInfo(SCONTEXT)->Eng0 =  GetPathStateEngAt(SCONTEXT, 0, GetCodeByteAt(&Get_ActiveJumpRule(SCONTEXT)->StCode0, 0));
    Get_JumpEnergyInfo(SCONTEXT)->Eng0 += GetPathStateEngAt(SCONTEXT, 1, GetCodeByteAt(&Get_ActiveJumpRule(SCONTEXT)->StCode0, 1));
}

void SetState2EnergyMmc(__SCONTEXT_PAR)
{
    Get_JumpEnergyInfo(SCONTEXT)->Eng2 =  GetPathStateEngAt(SCONTEXT, 0, GetCodeByteAt(&Get_ActiveJumpRule(SCONTEXT)->StCode2, 0));
    Get_JumpEnergyInfo(SCONTEXT)->Eng2 += GetPathStateEngAt(SCONTEXT, 1, GetCodeByteAt(&Get_ActiveJumpRule(SCONTEXT)->StCode2, 1));
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