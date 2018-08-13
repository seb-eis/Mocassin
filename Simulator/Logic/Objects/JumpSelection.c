//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	JumpSelection.c        		//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Jump selection logic        //
//////////////////////////////////////////

#include "Simulator/Logic/Objects/JumpSelection.h"

error_t ConstructJumpPool(sim_context_t* restrict simContext)
{
    return MC_NO_ERROR;
}

error_t PrepareJumpPool(sim_context_t* restrict simContext)
{
    return MC_NO_ERROR;
}

static inline void RollPosAndDirFromPool(sim_context_t* restrict simContext)
{
    int32_t rnv = (int32_t) (Pcg32Next(&simContext->RnGen) % simContext->JumpPool.TotJumpCount);
    for(dir_pool_t* dirPool = simContext->JumpPool.DirPools.Start; dirPool < simContext->JumpPool.DirPools.End; dirPool++)
    {
        if(rnv >= dirPool->JumpCount)
        {
            rnv -= dirPool->JumpCount;
            continue;
        }
        simContext->CycleState.ActRollInfo.EnvId = dirPool->EnvPool.Start[rnv];
        simContext->CycleState.ActRollInfo.RelId = rnv % dirPool->DirCount;
        return;
    }
    MC_DUMP_ERROR_AND_EXIT(MC_SIM_ERROR, "Selection routine reached end of function. Invalid or corrupted jump pool.");
}

static inline void RollMmcOffsetEnvId(sim_context_t* restrict simContext)
{
    simContext->CycleState.ActRollInfo.OffId = (int32_t) (Pcg32Next(&simContext->RnGen) % simContext->SimDynModel.EnvLattice.Header->Size);
}

void RollNextKmcSelect(sim_context_t* restrict simContext)
{
    RollPosAndDirFromPool(simContext);
}

void RollNextMmcSelect(sim_context_t* restrict simContext)
{
    RollPosAndDirFromPool(simContext);
    RollMmcOffsetEnvId(simContext);
}

static inline bool_t GetEnvPoolEntryUpdate(jump_pool_t* restrict jmpPool, const jump_counts_t* restrict jmpCntTable, env_state_t* restrict env)
{
    int32_t newPoolId = jmpPool->DirCountToPoolId.Start[*MDA_GET_2(*jmpCntTable, env->PosVector.d, env->ParId)];
    if (env->PoolId != newPoolId)
    {
        dir_pool_t* oldPool = &jmpPool->DirPools.Start[env->PoolId];
        dir_pool_t* newPool = &jmpPool->DirPools.Start[newPoolId];

        LIST_ADD(newPool->EnvPool, oldPool->EnvPool.Start[env->PoolPosId]);
        oldPool->EnvPool.Start[env->PoolPosId] = *LIST_POP_BACK(oldPool->EnvPool);

        env->PoolId = newPoolId;
        env->PoolPosId = newPool->PosCount;

        oldPool->PosCount--;
        oldPool->JumpCount -= oldPool->DirCount;
        newPool->PosCount++;
        newPool->JumpCount += newPool->DirCount;
        jmpPool->TotJumpCount += newPool->DirCount - oldPool->DirCount;

        return ((newPool->DirCount - oldPool->DirCount) != 0);
    }
    return false;
}

bool_t GetJumpPoolUpdateKmc(sim_context_t* restrict simContext)
{
    bool_t jmpCntChange = false;
    for(int32_t i = 0; i < simContext->CycleState.ActJumpDir->JumpLength; i++)
    {
        jmpCntChange |= GetEnvPoolEntryUpdate(&simContext->JumpPool, &simContext->SimDbModel.Transition.JumpCountTable, simContext->CycleState.ActPathEnvs[i]);
    }
    return jmpCntChange;
}

bool_t GetJumpPoolUpdateMmc(sim_context_t* restrict simContext)
{
    bool_t jmpCntChange = false;
    jmpCntChange |= GetEnvPoolEntryUpdate(&simContext->JumpPool, &simContext->SimDbModel.Transition.JumpCountTable, simContext->CycleState.ActPathEnvs[0]);
    jmpCntChange |= GetEnvPoolEntryUpdate(&simContext->JumpPool, &simContext->SimDbModel.Transition.JumpCountTable, simContext->CycleState.ActPathEnvs[1]);
    return jmpCntChange;
}