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
#include "Simulator/Logic/Routines/Helper/HelperRoutines.h"
#include "Simulator/Data/Model/SimContext/ContextAccess.h"
#include "Framework/Basic/BaseTypes/Buffers.h"

/* Local helper routines */

static inline int32_t LookupEnvironmentPoolId(jump_pool_t* restrict selectionPool, const jump_counts_t* restrict jumpCountTable, const env_state_t* restrict environment)
{
    int32_t idRedirect = array_Get(*jumpCountTable, environment->PositionVector.d, environment->ParticleId);
    return span_Get(selectionPool->NumOfDirectionsToPoolId, idRedirect);
}

static inline void PushBackEnvIdToDirectionPool(dir_pool_t *restrict directionPool, const int32_t entry)
{
    list_PushBack(directionPool->EnvironmentPool, entry);
}

static inline bool_t TryPushBackEnvIdToDirectionPool(dir_pool_t* restrict directionPool, const int32_t entry)
{
    if (!list_CanPushBack(directionPool->EnvironmentPool))
    {
        return false;
    }
    PushBackEnvIdToDirectionPool(directionPool, entry);
    return true;
}

static inline int32_t PopBackDirectionEnvPool(dir_pool_t *restrict directionPool)
{
    return list_PopBack(directionPool->EnvironmentPool);
}

static inline void UpdateEnvStateSelectionStatus(env_state_t* restrict environment, const int32_t poolId, const int32_t poolPositionId)
{
    environment->PoolId = poolId;
    environment->PoolPositionId = poolPositionId;
}

/* Initializer routines*/

static error_t AddEnvStateToSelectionPool(__SCONTEXT_PAR, env_state_t* restrict environment, const int32_t numOfJumps)
{
    int32_t poolId = getDirectionPoolIdByJumpCount(SCONTEXT, numOfJumps);
    jump_pool_t* selectionPool = getJumpSelectionPool(SCONTEXT);
    dir_pool_t* directionPool = getDirectionPoolById(SCONTEXT, poolId);

    if (!TryPushBackEnvIdToDirectionPool(directionPool, environment->EnvironmentId))
    {
        return ERR_BUFFEROVERFLOW;
    }

    directionPool->NumOfPositions++;
    directionPool->NumOfJumps += numOfJumps;
    selectionPool->NumOfSelectableJumps += numOfJumps;

    UpdateEnvStateSelectionStatus(environment, poolId, directionPool->NumOfPositions - 1);

    return ERR_OK;
}

error_t HandleEnvStatePoolRegistration(__SCONTEXT_PAR, const int32_t environmentId)
{
    env_state_t* environment = getEnvironmentStateById(SCONTEXT, environmentId);
    int32_t numOfJumps = getJumpCountByPositionStatus(SCONTEXT, environment->PositionVector.d, environment->ParticleId);
    
    if (!environment->IsStable || (numOfJumps <= JPOOL_DIRCOUNT_STATIC))
    {
        environment->IsMobile = false;
        UpdateEnvStateSelectionStatus(environment, JPOOL_NOT_SELECTABLE, JPOOL_NOT_SELECTABLE);
        return ERR_OK;
    }
    if (numOfJumps == JPOOL_DIRCOUNT_PASSIVE)
    {
        environment->IsMobile = true;
        UpdateEnvStateSelectionStatus(environment, JPOOL_NOT_SELECTABLE, JPOOL_NOT_SELECTABLE);
        return ERR_OK;
    }
    if (numOfJumps > JPOOL_DIRCOUNT_PASSIVE)
    {
        environment->IsMobile = true;
        return AddEnvStateToSelectionPool(SCONTEXT, environment, numOfJumps);
    }

    return ERR_UNKNOWN;
}

/* Simulation routines*/

static inline void RollPosAndDirFromPool(__SCONTEXT_PAR)
{
    int32_t randomNumber = MakeNextCeiledRnd(SCONTEXT, SCONTEXT->SelectionPool.NumOfSelectableJumps);

    cpp_foreach(directionPool, SCONTEXT->SelectionPool.DirectionPools)
    {
        if (randomNumber >= directionPool->NumOfJumps)
        {
            randomNumber -= directionPool->NumOfJumps;
            continue;
        }

        getJumpSelectionInfo(SCONTEXT)->EnvironmentId = getEnvironmentPoolEntryById(directionPool, randomNumber);
        getJumpSelectionInfo(SCONTEXT)->RelativeId = randomNumber % directionPool->NumOfDirections;

        return;
    }

    SIMERROR = ERR_UNKNOWN;
}

static inline void RollMmcOffsetEnvId(__SCONTEXT_PAR)
{
    getJumpSelectionInfo(SCONTEXT)->OffsetId = MakeNextCeiledRnd(SCONTEXT, getEnvironmentLattice(SCONTEXT)->Header->Size);
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

static inline void CorrectEnvPoolIds(env_state_t *restrict environment, const dir_pool_t *restrict newDirectionPool, const int32_t newPoolId)
{
    environment->PoolId = newPoolId;
    environment->PoolPositionId = newDirectionPool->NumOfPositions;
}

static inline void CorrectPoolCounters(dir_pool_t *restrict oldDirectionPool, dir_pool_t *restrict newDirectionPool, jump_pool_t *restrict selectionPool)
{
    oldDirectionPool->NumOfPositions--;
    oldDirectionPool->NumOfJumps -= oldDirectionPool->NumOfDirections;
    newDirectionPool->NumOfPositions++;
    newDirectionPool->NumOfJumps += newDirectionPool->NumOfDirections;
    selectionPool->NumOfSelectableJumps += newDirectionPool->NumOfDirections - oldDirectionPool->NumOfDirections;
}

static inline bool_t MakeEnvironmentPoolEntriesUpdate(__SCONTEXT_PAR, const jump_counts_t *restrict jumpCountTable, env_state_t *restrict environment)
{
    jump_pool_t* selectionPool = getJumpSelectionPool(SCONTEXT);
    int32_t newPoolId = LookupEnvironmentPoolId(selectionPool, jumpCountTable, environment);

    if (environment->PoolId != newPoolId)
    {
        dir_pool_t *oldDirectionPool = getDirectionPoolById(SCONTEXT, environment->PoolId);
        dir_pool_t *newDirectionPool = getDirectionPoolById(SCONTEXT, newPoolId);

        PushBackEnvIdToDirectionPool(newDirectionPool, getEnvironmentPoolEntryById(oldDirectionPool, environment->PoolPositionId));
        setEnvironmentPoolEntryById(oldDirectionPool, environment->PoolPositionId, PopBackDirectionEnvPool(oldDirectionPool));

        CorrectEnvPoolIds(environment, newDirectionPool, newPoolId);
        CorrectPoolCounters(oldDirectionPool, newDirectionPool, selectionPool);

        return (newDirectionPool->NumOfDirections - oldDirectionPool->NumOfDirections) != 0;
    }
    return false;
}

bool_t MakeJumpPoolUpdateKmc(__SCONTEXT_PAR)
{
    bool_t numOfJumpsChanged = false;
    for (int32_t i = 0; i < getActiveJumpDirection(SCONTEXT)->JumpLength; i++)
    {
        numOfJumpsChanged |= MakeEnvironmentPoolEntriesUpdate(SCONTEXT, getJumpDirectionsPerPositionTable(SCONTEXT), JUMPPATH[i]);
    }
    return numOfJumpsChanged;
}

bool_t MakeJumpPoolUpdateMmc(__SCONTEXT_PAR)
{
    bool_t numOfJumpsChanged = false;
    numOfJumpsChanged |= MakeEnvironmentPoolEntriesUpdate(SCONTEXT, getJumpDirectionsPerPositionTable(SCONTEXT), JUMPPATH[0]);
    numOfJumpsChanged |= MakeEnvironmentPoolEntriesUpdate(SCONTEXT, getJumpDirectionsPerPositionTable(SCONTEXT), JUMPPATH[1]);
    return numOfJumpsChanged;
}