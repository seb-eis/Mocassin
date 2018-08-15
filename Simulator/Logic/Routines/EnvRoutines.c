//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	EnvRoutines.h        		//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Env routines for simulation //
//////////////////////////////////////////

#include "Simulator/Logic/Routines/EnvRoutines.h"
#include "Simulator/Logic/Routines/HelperRoutines.h"

error_t ConstructEnvLattice(_SCTPARAM)
{
    return MC_NO_ERROR;
}

error_t PrepareEnvLattice(_SCTPARAM)
{
    return MC_NO_ERROR;
}

error_t GetEnvReadyStatusEval(_SCTPARAM)
{
    return MC_NO_ERROR;
}

static inline void SetActWorkEnv(_SCTPARAM, const env_link_t* restrict envLink)
{
    SCT->CycleState.ActWorkEnv = RefLatticeEnvAt(SCT, envLink->EnvId);
}

static inline void SetActWorkPairTable(_SCTPARAM, const env_state_t* restrict env, const env_link_t* restrict envLink)
{
    SCT->CycleState.ActPairTable = RefPairTableAt(SCT, RefEnvPairDefAt(env, envLink->EnvPosId)->TabId);
}

static inline void SetActWorkCluTable(_SCTPARAM, const env_state_t* restrict env, const clu_link_t* restrict cluLink)
{
    SCT->CycleState.ActCluTable = RefCluTableAt(SCT, RefEnvCluDefAt(env, cluLink->CluId)->TabId);
}

static inline int32_t FindCluTableCodeId(const clu_table_t* restrict table, const occode_t code)
{
    // Placeholder. Check code pool size and use binary/linear search depending on the pool size
    int32_t index = 0;
    for(occode_t* curCode = table->OccCodes.Start; *curCode != code; curCode++)
    {
        index++;
    }
    return index;
}

static inline double GetPairDelta(const pair_table_t* restrict table, const byte_t mainId, const byte_t oldId, const byte_t newId)
{
    return GetPairTableEntry(table, mainId, oldId) - GetPairTableEntry(table, mainId, newId);
}

static inline double MakeCluDelta(const clu_table_t* restrict table, const clu_link_t* restrict cluLink, clu_state_t* restrict clu, const byte_t mainId, const byte_t newId)
{
    double delta = (-1.0) * GetCluTableEntry(table, mainId, clu->CodeId);

    SetCodeByteAt(&clu->OccCode, cluLink->RelId, newId);
    clu->CodeId = FindCluTableCodeId(table, clu->OccCode);

    delta += GetCluTableEntry(table, mainId, clu->CodeId);
    return delta;
}

static inline void InvokeEnvLinkCluChanges(_SCTPARAM, const clu_link_t* restrict cluLink, const byte_t uptId, const byte_t newId)
{
    *RefActStateEngAt(SCT, uptId) = MakeCluDelta(RefActCluTable(SCT), cluLink, RefActWorkClu(SCT), uptId, newId);
}

static inline void InvokeEnvLinkPairChange(_SCTPARAM, const env_link_t* restrict envLink, const byte_t uptId, const byte_t oldId, const byte_t newId)
{
    *RefActStateEngAt(SCT, uptId) += GetPairDelta(RefActPairTable(SCT), uptId, oldId, newId);
}

static void InvokeEnvLinkOnWorkAllIds(_SCTPARAM, const env_link_t* restrict envLink, const byte_t oldId, const byte_t newId)
{
    for(size_t i = 0; GetActUpdateIdAt(SCT, i) != 0; i++)
    {
        InvokeEnvLinkPairChange(SCT, envLink, GetActUpdateIdAt(SCT, i), oldId, newId);
    }
}

static void InvokeEnvLinking(_SCTPARAM, env_state_t* restrict env, const byte_t newId)
{
    FOR_EACH(env_link_t, envLink, env->EnvLinks)
    {
        SetActWorkEnv(SCT, envLink);
        SetActWorkPairTable(SCT, RefActWorkEnv(SCT), envLink);
        InvokeEnvLinkOnWorkAllIds(SCT, envLink, env->ParId, newId);
    }
}

void CreateLocalJumpDeltaKmc(_SCTPARAM)
{

}

void RollbackLocalJumpDeltaKmc(_SCTPARAM)
{

}

void SetState0And1EnergiesKmc(_SCTPARAM)
{

}

void SetState2EnergyKmc(_SCTPARAM)
{

}

void CreateFullStateDeltaKmc(_SCTPARAM)
{
    for(byte_t i = 0; i < RefActJumpDir(SCT)->JumpLength; i++)
    {
        byte_t newId = GetCodeByteAt(&RefActJumpRule(SCT)->StCode2, i);
        if(RefPathEnvAt(SCT, i)->IsStable)
        {
            InvokeEnvLinking(SCT, RefPathEnvAt(SCT, i), newId);
        }
        RefPathEnvAt(SCT, i)->ParId = newId;
    }
}


void CreateLocalJumpDeltaMmc(_SCTPARAM)
{

}

void RollbackLocalJumpDeltaMmc(_SCTPARAM)
{

}

void SetState0And1EnergiesMmc(_SCTPARAM)
{

}

void SetState2EnergyMmc(_SCTPARAM)
{
    
}

void CreateFullStateDeltaMmc(_SCTPARAM)
{
    byte_t newParId0 = GetCodeByteAt(&RefActJumpRule(SCT)->StCode2, 0);
    byte_t newParId1 = GetCodeByteAt(&RefActJumpRule(SCT)->StCode2, 1);
    InvokeEnvLinking(SCT, RefPathEnvAt(SCT, 0), newParId0);
    InvokeEnvLinking(SCT, RefPathEnvAt(SCT, 1), newParId1);
    RefPathEnvAt(SCT, 0)->ParId = newParId0;
    RefPathEnvAt(SCT, 1)->ParId = newParId1;
}