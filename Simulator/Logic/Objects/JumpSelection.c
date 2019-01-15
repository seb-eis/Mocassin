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
#include "Simulator/Data/SimContext/ContextAccess.h"
#include "Framework/Basic/BaseTypes/Buffers.h"

/* Local helper routines */

// Translates the passed environment state into the index of the required environment pool
static inline int32_t LookupEnvironmentPoolId(JumpSelectionPool_t* restrict selectionPool, const JumpCountTable_t* restrict jumpCountTable, const EnvironmentState_t* restrict environment)
{
    int32_t idRedirect = array_Get(*jumpCountTable, environment->PositionVector.d, environment->ParticleId);
    return span_Get(selectionPool->DirectionPoolMapping, idRedirect);
}

// Adds the passed id to the enf of the passed direction pool
static inline void PushBackEnvIdToDirectionPool(DirectionPool_t *restrict directionPool, const int32_t entry)
{
    debug_assert(!list_IsFull(directionPool->EnvironmentPool));
    list_PushBack(directionPool->EnvironmentPool, entry);
}

// Tries to push back the passed if to the direction pool. Returns false if failed
static inline bool_t TryPushBackEnvIdToDirectionPool(DirectionPool_t* restrict directionPool, const int32_t entry)
{
    if (!list_IsFull(directionPool->EnvironmentPool))
    {
        return false;
    }
    PushBackEnvIdToDirectionPool(directionPool, entry);
    return true;
}

// Removes the last entry of the passed direction pool
static inline int32_t PopBackDirectionEnvPool(DirectionPool_t *restrict directionPool)
{
    debug_assert(!list_IsEmpty(directionPool->EnvironmentPool));
    return list_PopBack(directionPool->EnvironmentPool);
}

// Updates the selection status of the passed environment to the passed information
static inline void UpdateEnvStateSelectionStatus(EnvironmentState_t* restrict environment, const int32_t poolId, const int32_t poolPositionId)
{
    environment->PoolId = poolId;
    environment->PoolPositionId = poolPositionId;
}

/* Initializer routines*/

static error_t AddEnvStateToSelectionPool(__SCONTEXT_PAR, EnvironmentState_t* restrict environment, const int32_t numOfJumps)
{
    int32_t poolId = getDirectionPoolIdByJumpCount(SCONTEXT, numOfJumps);
    JumpSelectionPool_t* selectionPool = getJumpSelectionPool(SCONTEXT);
    DirectionPool_t* directionPool = getDirectionPoolAt(SCONTEXT, poolId);

    if (!TryPushBackEnvIdToDirectionPool(directionPool, environment->EnvironmentId))
    {
        return ERR_BUFFEROVERFLOW;
    }

    directionPool->NumOfPositions++;
    directionPool->NumOfJumps += numOfJumps;
    selectionPool->SelectableJumpCount += numOfJumps;

    UpdateEnvStateSelectionStatus(environment, poolId, directionPool->NumOfPositions - 1);

    return ERR_OK;
}

error_t HandleEnvStatePoolRegistration(__SCONTEXT_PAR, const int32_t environmentId)
{
    EnvironmentState_t* environment = getEnvironmentStateById(SCONTEXT, environmentId);
    int32_t numOfJumps = getJumpCountAt(SCONTEXT, environment->PositionVector.d, environment->ParticleId);
    int64_t selectionMask = environment->EnvironmentDefinition->SelectionParticleMask;
    
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
        if (flagsAreTrue(selectionMask, 1LL << environment->ParticleId))
        {
            return AddEnvStateToSelectionPool(SCONTEXT, environment, numOfJumps);
        }

        UpdateEnvStateSelectionStatus(environment, JPOOL_NOT_SELECTABLE, JPOOL_NOT_SELECTABLE);
        return ERR_OK;
    }

    return ERR_UNKNOWN;
}

/* Simulation routines*/

// Rolls a start position and jump direction from the jump selection pool
static inline void RollPositionAndDirectionFromPool(__SCONTEXT_PAR)
{
    int32_t randomNumber = GetNextCeiledRandom(SCONTEXT, SCONTEXT->SelectionPool.SelectableJumpCount);

    cpp_foreach(directionPool, SCONTEXT->SelectionPool.DirectionPools)
    {
        if (randomNumber >= directionPool->NumOfJumps)
        {
            randomNumber -= directionPool->NumOfJumps;
            continue;
        }

        getJumpSelectionInfo(SCONTEXT)->EnvironmentId = getEnvironmentPoolEntryAt(directionPool, randomNumber);
        getJumpSelectionInfo(SCONTEXT)->RelativeId = randomNumber % directionPool->NumOfDirections;

        return;
    }

    SIMERROR = ERR_UNKNOWN;
}

// Roll an environment offset id for the MMC selection process
static inline void RollMmcOffsetEnvId(__SCONTEXT_PAR)
{
    getJumpSelectionInfo(SCONTEXT)->OffsetId = GetNextCeiledRandom(SCONTEXT, getEnvironmentLattice(SCONTEXT)->Header->Size);
}

void RollNextKmcSelect(__SCONTEXT_PAR)
{
    RollPositionAndDirectionFromPool(SCONTEXT);
}

void RollNextMmcSelect(__SCONTEXT_PAR)
{
    RollPositionAndDirectionFromPool(SCONTEXT);
    RollMmcOffsetEnvId(SCONTEXT);
}

// Correct the environment pool ids on the passed environment state to new values
static inline void CorrectEnvPoolIds(EnvironmentState_t *restrict environment, const DirectionPool_t *restrict newDirectionPool, const int32_t newPoolId)
{
    environment->PoolId = newPoolId;
    environment->PoolPositionId = newDirectionPool->NumOfPositions;
}

// Corrects the pool counters on the old and new direction pool and the selection pool
static inline void CorrectPoolCounters(DirectionPool_t *restrict oldDirectionPool, DirectionPool_t *restrict newDirectionPool, JumpSelectionPool_t *restrict selectionPool)
{
    oldDirectionPool->NumOfPositions--;
    oldDirectionPool->NumOfJumps -= oldDirectionPool->NumOfDirections;
    newDirectionPool->NumOfPositions++;
    newDirectionPool->NumOfJumps += newDirectionPool->NumOfDirections;
    selectionPool->SelectableJumpCount += newDirectionPool->NumOfDirections - oldDirectionPool->NumOfDirections;
}

// Creates the environment pool entry update using the provided environment state
static inline bool_t MakeEnvironmentPoolEntriesUpdate(__SCONTEXT_PAR, const JumpCountTable_t *restrict jumpCountTable, EnvironmentState_t *restrict environment)
{
    JumpSelectionPool_t* selectionPool = getJumpSelectionPool(SCONTEXT);
    int32_t newPoolId = LookupEnvironmentPoolId(selectionPool, jumpCountTable, environment);

    if (environment->PoolId != newPoolId)
    {
        DirectionPool_t *oldDirectionPool = getDirectionPoolAt(SCONTEXT, environment->PoolId);
        DirectionPool_t *newDirectionPool = getDirectionPoolAt(SCONTEXT, newPoolId);

        PushBackEnvIdToDirectionPool(newDirectionPool, getEnvironmentPoolEntryAt(oldDirectionPool, environment->PoolPositionId));
        setEnvironmentPoolEntryAt(oldDirectionPool, environment->PoolPositionId, PopBackDirectionEnvPool(oldDirectionPool));

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
        numOfJumpsChanged |= MakeEnvironmentPoolEntriesUpdate(SCONTEXT, getJumpCountMapping(SCONTEXT), JUMPPATH[i]);
    }
    return numOfJumpsChanged;
}

bool_t MakeJumpPoolUpdateMmc(__SCONTEXT_PAR)
{
    bool_t numOfJumpsChanged = false;
    numOfJumpsChanged |= MakeEnvironmentPoolEntriesUpdate(SCONTEXT, getJumpCountMapping(SCONTEXT), JUMPPATH[0]);
    numOfJumpsChanged |= MakeEnvironmentPoolEntriesUpdate(SCONTEXT, getJumpCountMapping(SCONTEXT), JUMPPATH[1]);
    return numOfJumpsChanged;
}