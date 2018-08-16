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

error_t ConstructJumpPool(__SCONTEXT_PAR)
{
    return ERR_OK;
}

error_t PrepareJumpPool(__SCONTEXT_PAR)
{
    return ERR_OK;
}

static inline void AddEnvIdToDirPool(dir_pool_t* restrict pool, const int32_t entry)
{
    LIST_ADD(pool->EnvPool, entry);
}

static inline int32_t PopBackDirEnvPool(dir_pool_t* restrict pool)
{
    return *LIST_POP_BACK(pool->EnvPool);
}

static inline void RollPosAndDirFromPool(__SCONTEXT_PAR)
{
    int32_t rnv = GetNextCeiledRnd(SCONTEXT, SCONTEXT->JumpPool.TotJumpCount);
    FOR_EACH(dir_pool_t, dirPool, SCONTEXT->JumpPool.DirPools)
    {
        if(rnv >= dirPool->JumpCount)
        {
            rnv -= dirPool->JumpCount;
            continue;
        }
        RefActRollInfo(SCONTEXT)->EnvId = GetEnvPoolEntry(dirPool, rnv);
        RefActRollInfo(SCONTEXT)->RelId = rnv % dirPool->DirCount;
        return;
    }
    SIMERROR = ERR_UNKNOWN;
}

static inline void RollMmcOffsetEnvId(__SCONTEXT_PAR)
{
    RefActRollInfo(SCONTEXT)->OffId = GetNextCeiledRnd(SCONTEXT, RefEnvLattice(SCONTEXT)->Header->Size);
}

void RollNextKmcSelect(__SCONTEXT_PAR)
{
    RollPosAndDirFromPool(SCONTEXT);
}

void RollNextMmcSelect(__SCONTEXT_PAR)
{
    RollPosAndDirFromPool(SCONTEXT);
    RollMmcOffsetEnvId(SCONTEXT);
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

bool_t GetJumpPoolUpdateKmc(__SCONTEXT_PAR)
{
    bool_t jmpCntChange = false;
    for(int32_t i = 0; i < RefActJumpDir(SCONTEXT)->JumpLength; i++)
    {
        jmpCntChange |= GetEnvPoolEntryUpdate(&SCONTEXT->JumpPool, RefJmpDirCountTable(SCONTEXT), JUMPPATH[i]);
    }
    return jmpCntChange;
}

bool_t GetJumpPoolUpdateMmc(__SCONTEXT_PAR)
{
    bool_t jmpCntChange = false;
    jmpCntChange |= GetEnvPoolEntryUpdate(&SCONTEXT->JumpPool, RefJmpDirCountTable(SCONTEXT), JUMPPATH[0]);
    jmpCntChange |= GetEnvPoolEntryUpdate(&SCONTEXT->JumpPool, RefJmpDirCountTable(SCONTEXT), JUMPPATH[1]);
    return jmpCntChange;
}