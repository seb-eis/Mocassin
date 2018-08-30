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

void BuildEnvironmentLinkingSystem(__SCONTEXT_PAR)
{

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

static error_t WriteEnvOccupationToBuffer(__SCONTEXT_PAR, const env_state_t* envState, buffer_t* restrict occBuffer)
{
    const int32_t * blockSizes = Get_LatticeBlockSizes(SCONTEXT);

    for (int32_t i = 0; i < envState->EnvDef->PairDefs.Count; i++)
    {
        pair_def_t* pairDef = &envState->EnvDef->PairDefs.Start[i];
        int32_t envId = Int32FromVector4Pair(&envState->PosVector, &pairDef->RelVector, blockSizes);
        if((occBuffer->Start[i] = Get_EnvironmentStateById(SCONTEXT, envId)->ParId) == 0)
        {
            return ERR_DATACONSISTENCY;
        }
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
    if((cluState->CodeId = SaveLinearLookupCluCodeId(table, cluState->OccCode)) == INVALID_INDEX)
    {
        return ERR_DATACONSISTENCY;
    }

    SetCluStateBackup(cluState);
    return ERR_OK;
}

static error_t AddEnvClusterEnergyByOccupation(__SCONTEXT_PAR, env_state_t* restrict envState, buffer_t* restrict occBuffer)
{
    if (envState->ClusterStates.Count != envState->EnvDef->CluDefs.Count)
    {
        return ERR_DATACONSISTENCY;
    }

    clu_state_t * cluState = envState->ClusterStates.Start;
    FOR_EACH(clu_def_t, cluDef, envState->EnvDef->CluDefs)
    {
        const clu_table_t* cluTable = Get_ClusterEnergyTableById(SCONTEXT, cluDef->TabId);
        for (byte_t i = 0; cluDef->RelPosIds[i] != POSITION_NULL; i++)
        {
            byte_t posId = cluDef->RelPosIds[i];
            SetCodeByteAt(&cluState->OccCode, i, occBuffer->Start[posId]);
        }

        if (InitializeClusterStateStatus(SCONTEXT, cluState, cluTable) != ERR_OK)
        {
            return ERR_DATACONSISTENCY;
        }

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

    if ((error = WriteEnvOccupationToBuffer(SCONTEXT, envState, occBuffer)) != ERR_OK)
    {
        return error;
    }

    if ((error = SetEnvStateEnergyByOccupation(SCONTEXT, envState, occBuffer)) != ERR_OK)
    {
        return error;
    }

    return ERR_OK;
}

void SyncEnvironmentEnergyStatus(__SCONTEXT_PAR)
{
    error_t error;
    buffer_t occBuffer;

    error = AllocateDynamicEnvOccupationBuffer(SCONTEXT, &occBuffer);
    RUNTIME_ASSERT(error == ERR_OK, error, "Buffer creation for environment occupation lookup failed.");

    for (int32_t i = 0; i < Get_MainStateLattice(SCONTEXT)->Count; i++)
    {
        error = DynamicLookupEnvironmentStatus(SCONTEXT, i, &occBuffer);
        RUNTIME_ASSERT(error == ERR_OK, error, "Dynamic lookup of environment occupation and energy failed.");
    }

    FreeBuffer(&occBuffer);
}

void SetEnvStateStatusToDefault(__SCONTEXT_PAR, const int32_t envId, const byte_t parId)
{
    env_state_t* envState = Get_EnvironmentStateById(SCONTEXT, envId);

    envState->ParId = parId;
    envState->EnvId = envId;
    envState->IsMobile = false;
    envState->IsStable = (parId == 0) ? false : true;
    envState->PosVector = Vector4FromInt32(envId, Get_LatticeBlockSizes(SCONTEXT));
    envState->EnvDef = Get_EnvironmentModelById(SCONTEXT, envState->PosVector.d);
}

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