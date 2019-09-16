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
#include "Simulator/Logic/JumpSelection/JumpSelection.h"
#include "Simulator/Logic/Routines/Helper/HelperRoutines.h"
#include "Simulator/Data/SimContext/ContextAccess.h"
#include "Framework/Basic/BaseTypes/Buffers.h"

/* Local helper routines */

// Checks if the current occupation status of an environment defines a potentially selectable position
static inline bool_t EnvironmentIsSelectable(const EnvironmentState_t*restrict environment)
{
    return flagsAreTrue(environment->EnvironmentDefinition->SelectionParticleMask, 1 << environment->ParticleId);
}

// Translates the passed environment state into the index of the required environment pool or an invalid index if not selectable
static inline int32_t GetEnvironmentPoolId(JumpSelectionPool_t *restrict selectionPool, const JumpCountTable_t *restrict jumpCountTable, const EnvironmentState_t *restrict environment)
{
    return_if(!EnvironmentIsSelectable(environment), INVALID_INDEX);
    let jumpCount = array_Get(*jumpCountTable, environment->PositionVector.D, environment->ParticleId);
    return span_Get(selectionPool->DirectionPoolMapping, jumpCount);
}

// Adds the passed id to the enf of the passed direction pool without any counter updates
static inline void AddDirectionPoolEntry(DirectionPool_t *restrict directionPool, const int32_t entry)
{
    debug_assert(!list_IsFull(directionPool->EnvironmentPool));
    list_PushBack(directionPool->EnvironmentPool, entry);
}

// Tries to push back the passed entry to the direction pool without any counter updates. Returns false if failed
static inline bool_t TryAddDirectionPoolEntry(DirectionPool_t *restrict directionPool, const int32_t entry)
{
    return_if(list_IsFull(directionPool->EnvironmentPool), false);
    AddDirectionPoolEntry(directionPool, entry);
    return true;
}

// Removes the last entry of the passed direction pool and returns the removed entry
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

static error_t AddEnvStateToSelectionPool(SCONTEXT_PARAM, EnvironmentState_t* restrict environment, const int32_t jumpCount)
{
    let poolId = getDirectionPoolIdByJumpCount(SCONTEXT, jumpCount);
    var selectionPool = getJumpSelectionPool(SCONTEXT);
    var directionPool = getDirectionPoolAt(SCONTEXT, poolId);

    return_if(!TryAddDirectionPoolEntry(directionPool, environment->EnvironmentId), ERR_BUFFEROVERFLOW);

    directionPool->PositionCount++;
    directionPool->JumpCount += jumpCount;
    selectionPool->SelectableJumpCount += jumpCount;

    UpdateEnvStateSelectionStatus(environment, poolId, directionPool->PositionCount - 1);

    return ERR_OK;
}

error_t HandleEnvStatePoolRegistration(SCONTEXT_PARAM, const int32_t environmentId)
{
    var environment = getEnvironmentStateAt(SCONTEXT, environmentId);
    let directionCount = getJumpCountAt(SCONTEXT, environment->PositionVector.D, environment->ParticleId);
    
    if (!environment->IsStable || (directionCount <= JPOOL_DIRCOUNT_STATIC))
    {
        environment->IsMobile = false;
        UpdateEnvStateSelectionStatus(environment, JPOOL_NOT_SELECTABLE, JPOOL_NOT_SELECTABLE);
        return ERR_OK;
    }

    if (directionCount == JPOOL_DIRCOUNT_PASSIVE)
    {
        environment->IsMobile = true;
        UpdateEnvStateSelectionStatus(environment, JPOOL_NOT_SELECTABLE, JPOOL_NOT_SELECTABLE);
        return ERR_OK;
    }

    if (directionCount > JPOOL_DIRCOUNT_PASSIVE)
    {
        environment->IsMobile = true;
        if (EnvironmentIsSelectable(environment))
            return AddEnvStateToSelectionPool(SCONTEXT, environment, directionCount);

        UpdateEnvStateSelectionStatus(environment, JPOOL_NOT_SELECTABLE, JPOOL_NOT_SELECTABLE);
        return ERR_OK;
    }

    return ERR_UNKNOWN;
}

/* Simulation routines*/

// Rolls a start position and jump direction from the jump selection pool
static inline void RollPositionAndDirectionFromPool(SCONTEXT_PARAM)
{
    var selectionInfo = getJumpSelectionInfo(SCONTEXT);
    var random = GetNextCeiledRandom(SCONTEXT, SCONTEXT->SelectionPool.SelectableJumpCount);

    cpp_offset_foreach(directionPool, SCONTEXT->SelectionPool.DirectionPools, 1)
    {
        if (random >= directionPool->JumpCount)
        {
            random -= directionPool->JumpCount;
            continue;
        }

        selectionInfo->EnvironmentId = getEnvironmentPoolEntryAt(directionPool, random / directionPool->DirectionCount);
        selectionInfo->RelativeJumpId = random % directionPool->DirectionCount;

        return;
    }

    SIMERROR = ERR_UNKNOWN;
}

// Roll an environment offset id for the MMC selection process
static inline void MMC_RollEnvironmentOffsetId(SCONTEXT_PARAM)
{
    getJumpSelectionInfo(SCONTEXT)->MmcOffsetSourceId = GetNextCeiledRandom(SCONTEXT, getEnvironmentLattice(SCONTEXT)->Header->Size);
}

// Replaces the direction pool entry at the passed id by the last entry and updates the id set of the moved environment
static inline void RemoveDirectionPoolEntryAt(SCONTEXT_PARAM, DirectionPool_t *restrict directionPool, const int32_t id)
{
    debug_assert(!span_IndexIsOutOfRange(directionPool->EnvironmentPool, id));

    let newEnvironmentId = PopBackDirectionEnvPool(directionPool);
    span_Get(directionPool->EnvironmentPool, id) = newEnvironmentId;
    getEnvironmentStateAt(SCONTEXT, newEnvironmentId)->PoolPositionId = id;
}

// Environment pool entries update reaction to an environment change from not-selectable to selectable
static inline void OnPoolUpdateInvalidToSelectable(SCONTEXT_PARAM, JumpSelectionPool_t*restrict selectionPool,  EnvironmentState_t *restrict environment, const int32_t newPoolId)
{
    var newDirectionPool = getDirectionPoolAt(SCONTEXT, newPoolId);

    AddDirectionPoolEntry(newDirectionPool, environment->EnvironmentId);
    environment->PoolId = newPoolId;
    environment->PoolPositionId = newDirectionPool->PositionCount;

    newDirectionPool->PositionCount++;
    newDirectionPool->JumpCount += newDirectionPool->DirectionCount;
    selectionPool->SelectableJumpCount += newDirectionPool->DirectionCount;

}

// Environment pool entries update reaction to an environment change from selectable to not-selectable
static inline void OnPoolUpdateSelectableToInvalid(SCONTEXT_PARAM, JumpSelectionPool_t*restrict selectionPool,  EnvironmentState_t *restrict environment)
{
    var oldDirectionPool = getDirectionPoolAt(SCONTEXT, environment->PoolId);

    RemoveDirectionPoolEntryAt(SCONTEXT, oldDirectionPool, environment->PoolPositionId);
    oldDirectionPool->PositionCount--;
    oldDirectionPool->JumpCount -= oldDirectionPool->DirectionCount;
    selectionPool->SelectableJumpCount -= oldDirectionPool->DirectionCount;

    environment->PoolId = JPOOL_NOT_SELECTABLE;
    environment->PoolPositionId = JPOOL_NOT_SELECTABLE;
}

// Environment pool entries update reaction to an environment change from selectable to not-selectable
static inline void OnPoolUpdateSelectableToSelectable(SCONTEXT_PARAM, JumpSelectionPool_t*restrict selectionPool, EnvironmentState_t *restrict environment, const int32_t newPoolId)
{
    debug_assert(environment->PoolId != newPoolId);

    var oldDirectionPool = getDirectionPoolAt(SCONTEXT, environment->PoolId);
    var newDirectionPool = getDirectionPoolAt(SCONTEXT, newPoolId);

    AddDirectionPoolEntry(newDirectionPool, environment->EnvironmentId);
    RemoveDirectionPoolEntryAt(SCONTEXT, oldDirectionPool, environment->PoolPositionId);

    environment->PoolId = newPoolId;
    environment->PoolPositionId = newDirectionPool->PositionCount;

    oldDirectionPool->PositionCount--;
    oldDirectionPool->JumpCount -= oldDirectionPool->DirectionCount;
    newDirectionPool->PositionCount++;
    newDirectionPool->JumpCount += newDirectionPool->DirectionCount;
    selectionPool->SelectableJumpCount += newDirectionPool->DirectionCount - oldDirectionPool->DirectionCount;
}


// Creates the environment pool entry update using the provided environment state
static inline void MakeEnvironmentPoolEntriesUpdate(SCONTEXT_PARAM, const JumpCountTable_t *restrict jumpCountTable, EnvironmentState_t *restrict environment)
{
    return_if(!environment->IsStable);
    var selectionPool = getJumpSelectionPool(SCONTEXT);
    let newPoolId = GetEnvironmentPoolId(selectionPool, jumpCountTable, environment);

    // Case: The pool id has not changed or both old and new are not selectable -> do nothing
    return_if(environment->PoolId == newPoolId);

    // Case: The pool id changed from not-selectable to selectable -> push new entry to affiliated pool
    if (environment->PoolId == JPOOL_NOT_SELECTABLE)
    {
        OnPoolUpdateInvalidToSelectable(SCONTEXT,selectionPool, environment, newPoolId);
        return;
    }
    // Case: The pool id changed from selectable to not-selectable -> remove old entry from affiliated pool
    if (newPoolId == JPOOL_NOT_SELECTABLE)
    {
        OnPoolUpdateSelectableToInvalid(SCONTEXT,selectionPool, environment);
        return;
    }
    // Case: The pool id changed from selectable to another selectable -> remove old entry and push new entry
    OnPoolUpdateSelectableToSelectable(SCONTEXT, selectionPool, environment, newPoolId);
}

bool_t KMC_UpdateJumpPool(SCONTEXT_PARAM)
{
    let oldSelectableJumpCount = getJumpSelectionPool(SCONTEXT)->SelectableJumpCount;
    let jumpCountMapping = getJumpCountMapping(SCONTEXT);

    for (int32_t i = 0; i < getActiveJumpDirection(SCONTEXT)->JumpLength; i++)
        MakeEnvironmentPoolEntriesUpdate(SCONTEXT, jumpCountMapping, JUMPPATH[i]);

    return oldSelectableJumpCount != getJumpSelectionPool(SCONTEXT)->SelectableJumpCount;
}

void MMC_UpdateJumpPool(SCONTEXT_PARAM)
{
    let jumpCountMapping = getJumpCountMapping(SCONTEXT);
    MakeEnvironmentPoolEntriesUpdate(SCONTEXT, jumpCountMapping, JUMPPATH[0]);
    MakeEnvironmentPoolEntriesUpdate(SCONTEXT, jumpCountMapping, JUMPPATH[1]);
}

void KMC_RollNextJumpSelection(SCONTEXT_PARAM)
{
    RollPositionAndDirectionFromPool(SCONTEXT);
}

void MMC_RollNextJumpSelection(SCONTEXT_PARAM)
{
    RollPositionAndDirectionFromPool(SCONTEXT);
    MMC_RollEnvironmentOffsetId(SCONTEXT);
}