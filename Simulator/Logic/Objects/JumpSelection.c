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
#include "Simulator/Logic/Routines/HelperRoutines.h"

error_t ConstructJumpPool(_SCTPARAM)
{
    return MC_NO_ERROR;
}

error_t PrepareJumpPool(_SCTPARAM)
{
    return MC_NO_ERROR;
}

static inline void AddEnvIdToDirPool(dir_pool_t* restrict pool, const int32_t entry)
{
    LIST_ADD(pool->EnvPool, entry);
}

static inline int32_t PopBackDirEnvPool(dir_pool_t* restrict pool)
{
    return *LIST_POP_BACK(pool->EnvPool);
}

static inline void RollPosAndDirFromPool(_SCTPARAM)
{
    int32_t rnv = GetNextCeiledRnd(SCT, SCT->JumpPool.TotJumpCount);
    FOR_EACH(dir_pool_t, dirPool, SCT->JumpPool.DirPools)
    {
        if(rnv >= dirPool->JumpCount)
        {
            rnv -= dirPool->JumpCount;
            continue;
        }
        RefActRollInfo(SCT)->EnvId = GetEnvPoolEntry(dirPool, rnv);
        RefActRollInfo(SCT)->RelId = rnv % dirPool->DirCount;
        return;
    }
    SCT->ErrorCode = MC_SIM_ERROR;
}

static inline void RollMmcOffsetEnvId(_SCTPARAM)
{
    RefActRollInfo(SCT)->OffId = GetNextCeiledRnd(SCT, RefEnvLattice(SCT)->Header->Size);
}

void RollNextKmcSelect(_SCTPARAM)
{
    RollPosAndDirFromPool(SCT);
}

void RollNextMmcSelect(_SCTPARAM)
{
    RollPosAndDirFromPool(SCT);
    RollMmcOffsetEnvId(SCT);
}

static inline void CorrectEnvPoolIds(env_state_t* restrict env, const dir_pool_t* restrict newPool, const int32_t newId)
{
        env->PoolId = newId;
        env->PoolPosId = newPool->PosCount;
}

static inline void CorrectPoolCounters(dir_pool_t* restrict oldPool, dir_pool_t* restrict newPool, jump_pool_t* restrict jmpPool)
{
        oldPool->PosCount--;
        oldPool->JumpCount -= oldPool->DirCount;
        newPool->PosCount++;
        newPool->JumpCount += newPool->DirCount;
        jmpPool->TotJumpCount += newPool->DirCount - oldPool->DirCount;
}

static inline bool_t GetEnvPoolEntryUpdate(jump_pool_t* restrict jmpPool, const jump_counts_t* restrict jmpCntTable, env_state_t* restrict env)
{
    int32_t newPoolId = GetEnvDirPoolId(jmpPool, jmpCntTable, env);
    if (env->PoolId != newPoolId)
    {
        dir_pool_t* oldPool = RefJmpDirPoolAt(jmpPool, env->PoolId);
        dir_pool_t* newPool = RefJmpDirPoolAt(jmpPool, newPoolId);

        AddEnvIdToDirPool(newPool, GetEnvPoolEntry(oldPool, env->PoolPosId));
        *RefEnvPoolEntry(oldPool, env->PoolPosId) = PopBackDirEnvPool(oldPool);

        CorrectEnvPoolIds(env, newPool, newPoolId);
        CorrectPoolCounters(oldPool, newPool, jmpPool);

        return (newPool->DirCount - oldPool->DirCount) != 0;
    }
    return false;
}

bool_t GetJumpPoolUpdateKmc(_SCTPARAM)
{
    bool_t jmpCntChange = false;
    for(int32_t i = 0; i < RefActJumpDir(SCT)->JumpLength; i++)
    {
        jmpCntChange |= GetEnvPoolEntryUpdate(&SCT->JumpPool, RefJmpDirCountTable(SCT), RefPathEnvAt(SCT, i));
    }
    return jmpCntChange;
}

bool_t GetJumpPoolUpdateMmc(_SCTPARAM)
{
    bool_t jmpCntChange = false;
    jmpCntChange |= GetEnvPoolEntryUpdate(&SCT->JumpPool, RefJmpDirCountTable(SCT), RefPathEnvAt(SCT, 0));
    jmpCntChange |= GetEnvPoolEntryUpdate(&SCT->JumpPool, RefJmpDirCountTable(SCT), RefPathEnvAt(SCT, 1));
    return jmpCntChange;
}