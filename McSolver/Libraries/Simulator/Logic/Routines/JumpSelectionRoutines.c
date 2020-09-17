//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	JumpSelection.c        		//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Jump selection logic        //
//////////////////////////////////////////

#include "Libraries/Simulator/Logic/Helper/Constants.h"
#include "Libraries/Simulator/Logic/Routines/Helper/HelperRoutines.h"
#include "JumpSelection.h"

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
    let jumpCount = array_Get(*jumpCountTable, environment->LatticeVector.D, environment->ParticleId);
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

static error_t AddEnvStateToSelectionPool(SCONTEXT_PARAMETER, EnvironmentState_t* restrict environment, const int32_t jumpCount)
{
    let poolId = getDirectionPoolIdByJumpCount(simContext, jumpCount);
    var selectionPool = getJumpSelectionPool(simContext);
    var directionPool = getDirectionPoolAt(simContext, poolId);
    let envId = getEnvironmentStateIdByPointer(simContext, environment);
    return_if(!TryAddDirectionPoolEntry(directionPool, envId), ERR_BUFFEROVERFLOW);

    directionPool->PositionCount++;
    directionPool->JumpCount += jumpCount;
    selectionPool->SelectableJumpCount += jumpCount;

    UpdateEnvStateSelectionStatus(environment, poolId, directionPool->PositionCount - 1);

    return ERR_OK;
}

error_t RegisterEnvironmentStateInTransitionPool(SCONTEXT_PARAMETER, int32_t environmentId)
{
    var environment = getEnvironmentStateAt(simContext, environmentId);
    let directionCount = getJumpCountAt(simContext, environment->LatticeVector.D, environment->ParticleId);
    
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
            return AddEnvStateToSelectionPool(simContext, environment, directionCount);

        UpdateEnvStateSelectionStatus(environment, JPOOL_NOT_SELECTABLE, JPOOL_NOT_SELECTABLE);
        return ERR_OK;
    }

    return ERR_UNKNOWN;
}

/* Simulation routines*/

// Rolls a start position and jump direction from the jump selection pool
static inline void RollPositionAndDirectionFromPool(SCONTEXT_PARAMETER)
{
    var selectionInfo = getJumpSelectionInfo(simContext);
    var random = GetNextCeiledRandomFromContextRng(simContext, simContext->SelectionPool.SelectableJumpCount);

    cpp_offset_foreach(directionPool, simContext->SelectionPool.DirectionPools, 1)
    {
        if (random < directionPool->JumpCount)
        {
            let rdiv = div(random, directionPool->DirectionCount);
            selectionInfo->EnvironmentId = getEnvironmentPoolEntryAt(directionPool, rdiv.quot);
            selectionInfo->RelativeJumpId = rdiv.rem;
            return;
        }
        random -= directionPool->JumpCount;
    }

    SIMERROR = ERR_UNKNOWN;
}

// Roll an environment offset id for the MMC selection process
static inline void RollMmcEnvironmentOffsetId(SCONTEXT_PARAMETER)
{
    getJumpSelectionInfo(simContext)->MmcOffsetSourceId = GetNextCeiledRandomFromContextRng(simContext,
                                                                                            getEnvironmentLattice(
                                                                                                    simContext)->Header->Size);
}

// Replaces the direction pool entry at the passed id by the last entry and updates the id set of the moved environment
static inline void RemoveDirectionPoolEntryAt(SCONTEXT_PARAMETER, DirectionPool_t *restrict directionPool, const int32_t id)
{
    debug_assert(!span_IsIndexOutOfRange(directionPool->EnvironmentPool, id));

    let newEnvironmentId = PopBackDirectionEnvPool(directionPool);
    span_Get(directionPool->EnvironmentPool, id) = newEnvironmentId;
    getEnvironmentStateAt(simContext, newEnvironmentId)->PoolPositionId = id;
}

// Environment pool entries update reaction to an environment change from not-selectable to selectable
static inline void OnPoolUpdateInvalidToSelectable(SCONTEXT_PARAMETER, JumpSelectionPool_t*restrict selectionPool, EnvironmentState_t *restrict environment, const int32_t newPoolId)
{
    var newDirectionPool = getDirectionPoolAt(simContext, newPoolId);
    let envId = getEnvironmentStateIdByPointer(simContext, environment);
    AddDirectionPoolEntry(newDirectionPool, envId);
    environment->PoolId = newPoolId;
    environment->PoolPositionId = newDirectionPool->PositionCount;

    newDirectionPool->PositionCount++;
    newDirectionPool->JumpCount += newDirectionPool->DirectionCount;
    selectionPool->SelectableJumpCount += newDirectionPool->DirectionCount;

}

// Environment pool entries update reaction to an environment change from selectable to not-selectable
static inline void OnPoolUpdateSelectableToInvalid(SCONTEXT_PARAMETER, JumpSelectionPool_t*restrict selectionPool, EnvironmentState_t *restrict environment)
{
    var oldDirectionPool = getDirectionPoolAt(simContext, environment->PoolId);

    RemoveDirectionPoolEntryAt(simContext, oldDirectionPool, environment->PoolPositionId);
    oldDirectionPool->PositionCount--;
    oldDirectionPool->JumpCount -= oldDirectionPool->DirectionCount;
    selectionPool->SelectableJumpCount -= oldDirectionPool->DirectionCount;

    environment->PoolId = JPOOL_NOT_SELECTABLE;
    environment->PoolPositionId = JPOOL_NOT_SELECTABLE;
}

// Environment pool entries update reaction to an environment change from selectable to not-selectable
static inline void OnPoolUpdateSelectableToSelectable(SCONTEXT_PARAMETER, JumpSelectionPool_t*restrict selectionPool, EnvironmentState_t *restrict environment, const int32_t newPoolId)
{
    debug_assert(environment->PoolId != newPoolId);

    var oldDirectionPool = getDirectionPoolAt(simContext, environment->PoolId);
    var newDirectionPool = getDirectionPoolAt(simContext, newPoolId);
    let envId = getEnvironmentStateIdByPointer(simContext, environment);
    AddDirectionPoolEntry(newDirectionPool, envId);
    RemoveDirectionPoolEntryAt(simContext, oldDirectionPool, environment->PoolPositionId);

    environment->PoolId = newPoolId;
    environment->PoolPositionId = newDirectionPool->PositionCount;

    oldDirectionPool->PositionCount--;
    oldDirectionPool->JumpCount -= oldDirectionPool->DirectionCount;
    newDirectionPool->PositionCount++;
    newDirectionPool->JumpCount += newDirectionPool->DirectionCount;
    selectionPool->SelectableJumpCount += newDirectionPool->DirectionCount - oldDirectionPool->DirectionCount;
}


// Creates the environment pool entry update using the provided environment state
static inline void MakeEnvironmentPoolEntriesUpdate(SCONTEXT_PARAMETER, const JumpCountTable_t *restrict jumpCountTable, EnvironmentState_t *restrict environment)
{
    return_if(!environment->IsStable);
    var selectionPool = getJumpSelectionPool(simContext);
    let newPoolId = GetEnvironmentPoolId(selectionPool, jumpCountTable, environment);

    // Case: The pool id has not changed or both old and new are not selectable -> do nothing
    return_if(environment->PoolId == newPoolId);

    // Case: The pool id changed from not-selectable to selectable -> push new entry to affiliated pool
    if (environment->PoolId == JPOOL_NOT_SELECTABLE)
    {
        OnPoolUpdateInvalidToSelectable(simContext, selectionPool, environment, newPoolId);
        return;
    }
    // Case: The pool id changed from selectable to not-selectable -> remove old entry from affiliated pool
    if (newPoolId == JPOOL_NOT_SELECTABLE)
    {
        OnPoolUpdateSelectableToInvalid(simContext, selectionPool, environment);
        return;
    }
    // Case: The pool id changed from selectable to another selectable -> remove old entry and push new entry
    OnPoolUpdateSelectableToSelectable(simContext, selectionPool, environment, newPoolId);
}

bool_t UpdateTransitionPoolAfterKmcSystemAdvance(SCONTEXT_PARAMETER)
{
    let oldSelectableJumpCount = getJumpSelectionPool(simContext)->SelectableJumpCount;
    let jumpCountMapping = getJumpCountMapping(simContext);

    for (int32_t i = 0; i < getActiveJumpDirection(simContext)->JumpLength; i++)
        MakeEnvironmentPoolEntriesUpdate(simContext, jumpCountMapping, JUMPPATH[i]);

    return oldSelectableJumpCount != getJumpSelectionPool(simContext)->SelectableJumpCount;
}

void UpdateTransitionPoolAfterMmcSystemAdvance(SCONTEXT_PARAMETER)
{
    let jumpCountMapping = getJumpCountMapping(simContext);
    MakeEnvironmentPoolEntriesUpdate(simContext, jumpCountMapping, JUMPPATH[0]);
    MakeEnvironmentPoolEntriesUpdate(simContext, jumpCountMapping, JUMPPATH[1]);
}

void UniformSelectNextKmcJumpSelection(SCONTEXT_PARAMETER)
{
    RollPositionAndDirectionFromPool(simContext);
}

void UniformSelectNextMmcJumpSelection(SCONTEXT_PARAMETER)
{
    RollPositionAndDirectionFromPool(simContext);
    RollMmcEnvironmentOffsetId(simContext);
}