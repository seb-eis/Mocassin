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

error_t ConstructEnvLattice(sim_context_t* restrict simContext)
{
    return MC_NO_ERROR;
}

error_t PrepareEnvLattice(sim_context_t* restrict simContext)
{
    return MC_NO_ERROR;
}

error_t GetEnvReadyStatusEval(sim_context_t* restrict simContext)
{
    return MC_NO_ERROR;
}

static inline env_state_t* GetWorkEnv(const sim_context_t* restrict simContext, const env_link_t* restrict envLink)
{
    return &simContext->SimDynModel.EnvLattice.Start[envLink->EnvId];
}

static inline pair_table_t* GetPairTable(const sim_context_t* restrict simContext, const env_state_t* restrict env, const env_link_t* restrict envLink)
{
    int32_t tabId = env->EnvDef->PairDefs.Start[envLink->EnvPosId].TabId;
    return &simContext->SimDbModel.Energy.PairTables.Start[tabId];
}

static inline clu_table_t* GetClusterTable(const sim_context_t* restrict simContext, const env_state_t* restrict env, const clu_link_t* restrict cluLink)
{
    int32_t tabId = env->EnvDef->CluDefs.Start[cluLink->CluId].TabId;
    return &simContext->SimDbModel.Energy.CluTables.Start[tabId];
}

static inline int32_t FindClusterCodeTableId(const clu_table_t* restrict table, const occode_t code)
{
    // Placeholder, replace by binary search during optimization!
    int32_t index = 0;
    for(occode_t* curCode = table->OccCodes.Start; *curCode != code; curCode++)
    {
        index++;
    }
    return index;
} 

static inline double GetPairDelta(const pair_table_t* restrict table, const byte_t mainId, const byte_t oldId, const byte_t newId)
{
    return *MDA_GET_2(table->EngTable, mainId, oldId) - *MDA_GET_2(table->EngTable, mainId, newId);
}

static inline double GetClusterDelta(const clu_table_t* restrict table, const clu_link_t* restrict cluLink, clu_state_t* restrict cluState, const byte_t mainId, const byte_t newId)
{
    double delta = -(*MDA_GET_2(table->EngTable, table->ParToTableId[mainId], cluState->CurTableId));
    MARSHAL_AS(byte_t, &cluState->OccCode)[cluLink->CluPosId] = newId;
    cluState->CurTableId = FindClusterCodeTableId(table, cluState->OccCode);
    delta += *MDA_GET_2(table->EngTable, table->ParToTableId[mainId], cluState->CurTableId);
    return delta;
}

static void ApplyCluLinkChange(sim_context_t* restrict simContext, const clu_link_t* restrict cluLink, const byte_t newId, const byte_t oldId, const byte_t uptId)
{

}

static void ApplyUptLinkChange(sim_context_t* restrict simContext, const env_link_t* restrict envLink, const byte_t newId, const byte_t oldId, const byte_t uptId)
{

}

static void AplCluLinkChangesToActEnv(sim_context_t* restrict simContext, const clu_link_t* restrict cluLink, const byte_t newParId)
{
    simContext->CycleState.ActCluTable = GetClusterTable(simContext, simContext->CycleState.ActWorkEnv, cluLink);
    for(byte_t* uptParId = &simContext->CycleState.ActWorkEnv->EnvDef->UptParIds[0]; *uptParId != UINT8_MAX; uptParId++)
    {
        
    }
}

static void AplAllEnvLinkChanges(sim_context_t* restrict simContext, const env_link_t* restrict envLink, const byte_t newParId)
{
    simContext->CycleState.ActWorkEnv = GetWorkEnv(simContext, envLink);
    simContext->CycleState.ActPairTable = GetPairTable(simContext, simContext->CycleState.ActWorkEnv, envLink);

    for(byte_t* uptParId = &simContext->CycleState.ActWorkEnv->EnvDef->UptParIds[0]; *uptParId != UINT8_MAX; uptParId++)
    {
        
    }

    FOR_EACH(clu_link_t, cluLink, envLink->ClusterLinks)
    {
        AplCluLinkChangesToActEnv(simContext, cluLink, newParId);
    }
}


void CreateLocalJumpDeltaKmc(sim_context_t* restrict simContext)
{

}

void RollbackLocalJumpDeltaKmc(sim_context_t* restrict simContext)
{

}

void SetState0And1EnergiesKmc(sim_context_t* restrict simContext)
{

}

void SetState2EnergyKmc(sim_context_t* restrict simContext)
{

}

void DistributeStateDeltaKmc(sim_context_t* restrict simContext)
{

}


void CreateLocalJumpDeltaMmc(sim_context_t* restrict simContext)
{

}

void RollbackLocalJumpDeltaMmc(sim_context_t* restrict simContext)
{

}

void SetState0And1EnergiesMmc(sim_context_t* restrict simContext)
{

}

void SetState2EnergyMmc(sim_context_t* restrict simContext)
{
    
}

void DistributeStateDeltaMmc(sim_context_t* restrict simContext)
{

}