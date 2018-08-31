//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	JumpSelection.c        		//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Jump selection logic        //
//////////////////////////////////////////

#include "Simulator/Logic/Constants/Constants.h"
#include "Simulator/Logic/Objects/JumpSelection.h"
#include "Simulator/Logic/Routines/HelperRoutines.h"
#include "Simulator/Data/Model/SimContext/ContextAccess.h" 

/* Local helper routines */

static inline int32_t LookupEnvironmentPoolId(jump_pool_t* restrict jmpPool, const jump_counts_t* restrict cntTable, const env_state_t* restrict env)
{
    return jmpPool->DirCountToPoolId.Start[*MDA_GET_2(*cntTable, env->PosVector.d, env->ParId)];
}

static inline void PushBackEnvIdToDirectionPool(dir_pool_t *restrict pool, const int32_t entry)
{
    LIST_ADD(pool->EnvPool, entry);
}

static inline bool_t TryPushBackEnvIdToDirectionPool(dir_pool_t* restrict pool, const int32_t entry)
{
    if ((pool->EnvPool.CurEnd + 1) > pool->EnvPool.End)
    {
        return false;
    }
    PushBackEnvIdToDirectionPool(pool, entry);
    return true;
}

static inline int32_t PopBackDirectionEnvPool(dir_pool_t *restrict pool)
{
    return *LIST_POP_BACK(pool->EnvPool);
}

static inline void UpdateEnvStateSelectionStatus(env_state_t* restrict envState, const int32_t poolId, const int32_t poolPosId)
{
    envState->PoolId = poolId;
    envState->PoolPosId = poolPosId;
}

/* Initializer routines*/

static error_t AddEnvStateToSelectionPool(__SCONTEXT_PAR, env_state_t* restrict envState, const int32_t jumpCount)
{
    int32_t poolId = Get_DirectionPoolIdByJumpCount(SCONTEXT, jumpCount);
    jump_pool_t* jumpPool = Get_JumpSelectionPool(SCONTEXT);
    dir_pool_t* dirPool = Get_DirectionPoolById(SCONTEXT, poolId);

    if (!TryPushBackEnvIdToDirectionPool(dirPool, envState->EnvId))
    {
        return ERR_BUFFEROVERFLOW;
    }

    dirPool->PosCount++;
    dirPool->JumpCount += jumpCount;
    jumpPool->TotJumpCount += jumpCount;

    UpdateEnvStateSelectionStatus(envState, poolId, dirPool->PosCount - 1);

    return ERR_OK;
}

error_t HandleEnvStatePoolRegistration(__SCONTEXT_PAR, const int32_t envId)
{
    env_state_t* envState = Get_EnvironmentStateById(SCONTEXT, envId);
    int32_t jumpCount = Get_JumpCountByPositionStatus(SCONTEXT, envState->PosVector.d, envState->ParId);
    
    if (!envState->IsStable || (jumpCount <= JPOOL_DIRCOUNT_STATIC))
    {
        envState->IsMobile = false;
        UpdateEnvStateSelectionStatus(envState, JPOOL_NOT_SELECTABLE, JPOOL_NOT_SELECTABLE);
        return ERR_OK;
    }
    if (jumpCount == JPOOL_DIRCOUNT_PASSIVE)
    {
        envState->IsMobile = true;
        UpdateEnvStateSelectionStatus(envState, JPOOL_NOT_SELECTABLE, JPOOL_NOT_SELECTABLE);
        return ERR_OK;
    }
    if (jumpCount > JPOOL_DIRCOUNT_PASSIVE)
    {
        envState->IsMobile = true;
        return AddEnvStateToSelectionPool(SCONTEXT, envState, jumpCount);
    }

    return ERR_UNKNOWN;
}

/* Simulation routines*/

static inline void RollPosAndDirFromPool(__SCONTEXT_PAR)
{
    int32_t rnv = GetNextCeiledRnd(SCONTEXT, SCONTEXT->JumpPool.TotJumpCount);
    FOR_EACH(dir_pool_t, dirPool, SCONTEXT->JumpPool.DirPools)
    {
        if (rnv >= dirPool->JumpCount)
        {
            rnv -= dirPool->JumpCount;
            continue;
        }
        Get_JumpSelectionInfo(SCONTEXT)->EnvId = Get_EnvironmentPoolEntryById(dirPool, rnv);
        Get_JumpSelectionInfo(SCONTEXT)->RelId = rnv % dirPool->DirCount;
        return;
    }
    SIMERROR = ERR_UNKNOWN;
}

static inline void RollMmcOffsetEnvId(__SCONTEXT_PAR)
{
    Get_JumpSelectionInfo(SCONTEXT)->OffId = GetNextCeiledRnd(SCONTEXT, Get_EnvironmentLattice(SCONTEXT)->Header->Size);
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

static inline void CorrectEnvPoolIds(env_state_t *restrict env, const dir_pool_t *restrict newPool, const int32_t newId)
{
    env->PoolId = newId;
    env->PoolPosId = newPool->PosCount;
}

static inline void CorrectPoolCounters(dir_pool_t *restrict oldPool, dir_pool_t *restrict newPool, jump_pool_t *restrict jmpPool)
{
    oldPool->PosCount--;
    oldPool->JumpCount -= oldPool->DirCount;
    newPool->PosCount++;
    newPool->JumpCount += newPool->DirCount;
    jmpPool->TotJumpCount += newPool->DirCount - oldPool->DirCount;
}

static inline bool_t MakeEnvironmentPoolEntriesUpdate(__SCONTEXT_PAR, const jump_counts_t *restrict jmpCntTable, env_state_t *restrict env)
{
    jump_pool_t* jumpPool = Get_JumpSelectionPool(SCONTEXT);
    int32_t newPoolId = LookupEnvironmentPoolId(jumpPool, jmpCntTable, env);

    if (env->PoolId != newPoolId)
    {
        dir_pool_t *oldPool = Get_DirectionPoolById(SCONTEXT, env->PoolId);
        dir_pool_t *newPool = Get_DirectionPoolById(SCONTEXT, newPoolId);

        PushBackEnvIdToDirectionPool(newPool, Get_EnvironmentPoolEntryById(oldPool, env->PoolPosId));
        Set_EnvironmentPoolEntryById(oldPool, env->PoolPosId, PopBackDirectionEnvPool(oldPool));

        CorrectEnvPoolIds(env, newPool, newPoolId);
        CorrectPoolCounters(oldPool, newPool, jumpPool);

        return (newPool->DirCount - oldPool->DirCount) != 0;
    }
    return false;
}

bool_t MakeJumpPoolUpdateKmc(__SCONTEXT_PAR)
{
    bool_t jumpCountChanged = false;
    for (int32_t i = 0; i < Get_ActiveJumpDirection(SCONTEXT)->JumpLength; i++)
    {
        jumpCountChanged |= MakeEnvironmentPoolEntriesUpdate(SCONTEXT, Get_JumpDirectionsPerPositionTable(SCONTEXT), JUMPPATH[i]);
    }
    return jumpCountChanged;
}

bool_t MakeJumpPoolUpdateMmc(__SCONTEXT_PAR)
{
    bool_t jumpCountChanged = false;
    jumpCountChanged |= MakeEnvironmentPoolEntriesUpdate(SCONTEXT, Get_JumpDirectionsPerPositionTable(SCONTEXT), JUMPPATH[0]);
    jumpCountChanged |= MakeEnvironmentPoolEntriesUpdate(SCONTEXT, Get_JumpDirectionsPerPositionTable(SCONTEXT), JUMPPATH[1]);
    return jumpCountChanged;
}