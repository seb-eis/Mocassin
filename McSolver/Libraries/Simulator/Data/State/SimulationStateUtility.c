//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	StateUtility.h        		//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Simulation state utilities  //
//////////////////////////////////////////

#include "SimulationStateUtility.h"


static error_t RestoreStateHeaderAccess(SimulationState_t* simulationState)
{
    return_if(simulationState->Buffer.Begin == NULL, ERR_DATACONSISTENCY);
    simulationState->Header = (StateHeader_t) {.Data = (void*) simulationState->Buffer.Begin};
    return ERR_OK;
}

static error_t RestoreStateMetaAccess(SimulationState_t* simulationState)
{
    void* ptr = simulationState->Buffer.Begin + simulationState->Header.Data->MetaStartByte;
    return_if(ptr >= (void*) simulationState->Buffer.End, ERR_DATACONSISTENCY);
    simulationState->Meta.Data = ptr;
    return ERR_OK;
}

static error_t RestoreStateLatticeAccess(SimulationState_t* simulationState)
{
    void* startPtr = simulationState->Buffer.Begin + simulationState->Header.Data->LatticeStartByte;
    return_if(startPtr >= (void*) simulationState->Buffer.End, ERR_DATACONSISTENCY);

    void* endPtr = simulationState->Buffer.Begin + simulationState->Header.Data->CountersStartByte;
    return_if(endPtr < startPtr || endPtr >= (void*) simulationState->Buffer.End, ERR_DATACONSISTENCY);

    simulationState->Lattice.Begin = startPtr;
    simulationState->Lattice.End = endPtr;
    return ERR_OK;
}

static error_t RestoreStateCounterAccess(SimulationState_t* simulationState)
{
    void* startPtr = simulationState->Buffer.Begin + simulationState->Header.Data->CountersStartByte;
    return_if(startPtr >= (void*) simulationState->Buffer.End, ERR_DATACONSISTENCY);

    void* endPtr = simulationState->Buffer.Begin + simulationState->Header.Data->GlobalTrackerStartByte;
    return_if(endPtr < startPtr || endPtr >= (void*) simulationState->Buffer.End, ERR_DATACONSISTENCY);

    simulationState->Counters.Begin = startPtr;
    simulationState->Counters.End = endPtr;
    return ERR_OK;
}

static error_t RestoreStateGlobalTrackerAccess(SimulationState_t* simulationState)
{
    void* startPtr = simulationState->Buffer.Begin + simulationState->Header.Data->GlobalTrackerStartByte;
    return_if(startPtr >= (void*) simulationState->Buffer.End, ERR_DATACONSISTENCY);

    void* endPtr = simulationState->Buffer.Begin + simulationState->Header.Data->MobileTrackerStartByte;
    return_if(endPtr < startPtr || endPtr >= (void*) simulationState->Buffer.End, ERR_DATACONSISTENCY);

    simulationState->GlobalTrackers.Begin = startPtr;
    simulationState->GlobalTrackers.End = endPtr;
    return ERR_OK;
}

static error_t RestoreStateMobileTrackerAccess(SimulationState_t* simulationState)
{
    void* startPtr = simulationState->Buffer.Begin + simulationState->Header.Data->MobileTrackerStartByte;
    return_if(startPtr >= (void*) simulationState->Buffer.End, ERR_DATACONSISTENCY);

    void* endPtr = simulationState->Buffer.Begin + simulationState->Header.Data->StaticTrackerStartByte;
    return_if(endPtr < startPtr || endPtr >= (void*) simulationState->Buffer.End, ERR_DATACONSISTENCY);

    simulationState->MobileTrackers.Begin = startPtr;
    simulationState->MobileTrackers.End = endPtr;
    return ERR_OK;
}

static error_t RestoreStateStaticTrackerAccess(SimulationState_t* simulationState)
{
    void* startPtr = simulationState->Buffer.Begin + simulationState->Header.Data->StaticTrackerStartByte;
    return_if(startPtr >= (void*) simulationState->Buffer.End, ERR_DATACONSISTENCY);

    void* endPtr = simulationState->Buffer.Begin + simulationState->Header.Data->MobileTrackerIdxStartByte;
    return_if(endPtr < startPtr || endPtr >= (void*) simulationState->Buffer.End, ERR_DATACONSISTENCY);

    simulationState->StaticTrackers.Begin = startPtr;
    simulationState->StaticTrackers.End = endPtr;
    return ERR_OK;
}

static error_t RestoreStateMobileTrackerMappingAccess(SimulationState_t* simulationState)
{
    void* startPtr = simulationState->Buffer.Begin + simulationState->Header.Data->MobileTrackerIdxStartByte;
    return_if(startPtr >= (void*) simulationState->Buffer.End, ERR_DATACONSISTENCY);

    void* endPtr = simulationState->Buffer.Begin + simulationState->Header.Data->JumpStatisticsStartByte;
    return_if(endPtr < startPtr || endPtr >= (void*) simulationState->Buffer.End, ERR_DATACONSISTENCY);

    simulationState->MobileTrackerMapping.Begin = startPtr;
    simulationState->MobileTrackerMapping.End = endPtr;
    return ERR_OK;
}

static error_t RestoreStateJumpStatisticsAccess(SimulationState_t* simulationState)
{
    void* startPtr = simulationState->Buffer.Begin + simulationState->Header.Data->JumpStatisticsStartByte;
    return_if(startPtr >= (void*) simulationState->Buffer.End, ERR_DATACONSISTENCY);

    void* endPtr = simulationState->Buffer.End;

    simulationState->JumpStatistics.Begin = startPtr;
    simulationState->JumpStatistics.End = endPtr;
    return ERR_OK;
}

StateRestoreCallbacks_t GetStateRestoreCallbackCollection()
{
    static FStateRestoreCallback_t callbacks[] =
    {
        (FStateRestoreCallback_t) RestoreStateHeaderAccess,
        (FStateRestoreCallback_t) RestoreStateMetaAccess,
        (FStateRestoreCallback_t) RestoreStateLatticeAccess,
        (FStateRestoreCallback_t) RestoreStateCounterAccess,
        (FStateRestoreCallback_t) RestoreStateGlobalTrackerAccess,
        (FStateRestoreCallback_t) RestoreStateMobileTrackerAccess,
        (FStateRestoreCallback_t) RestoreStateStaticTrackerAccess,
        (FStateRestoreCallback_t) RestoreStateMobileTrackerMappingAccess,
        (FStateRestoreCallback_t) RestoreStateJumpStatisticsAccess
    };
    return (StateRestoreCallbacks_t) span_CArrayToSpan(callbacks);
}


error_t RestoreSimulationStateAccessToBuffer(Buffer_t *buffer, SimulationState_t *simulationState)
{
    simulationState->Buffer = *buffer;
    cpp_foreach(callback, GetStateRestoreCallbackCollection())
    {
        let error = (*callback)(simulationState);
        return_if(error,error);
    }
    return ERR_OK;
}

error_t LoadContextFreeSimulationStateFromFile(char const *fileName, SimulationState_t *simulationState)
{
    Buffer_t stateBuffer;

    var error = LoadBufferFromFile(fileName, &stateBuffer);
    return_if(error,error);

    error = RestoreSimulationStateAccessToBuffer(&stateBuffer, simulationState);
    return error;
}