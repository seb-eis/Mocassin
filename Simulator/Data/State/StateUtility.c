//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	StateUtility.h        		//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Simulation state utilities  //
//////////////////////////////////////////

#include "Simulator/Data/State/StateUtility.h"
#include "Framework/Basic/FileIO/FileIO.h"


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


error_t CreateSimulationStateAccessToBuffer(Buffer_t* buffer, SimulationState_t* simulationState)
{
    simulationState->Buffer = *buffer;
    cpp_foreach(callback, GetStateRestoreCallbackCollection())
    {
        let error = (*callback)(simulationState);
        return_if(error,error);
    }
    return ERR_OK;
}

error_t LoadContextFreeSimulationState(char const *fileName, SimulationState_t *simulationState)
{
    Buffer_t stateBuffer;

    var error = LoadBufferFromFile(fileName, &stateBuffer);
    return_if(error,error);

    error = CreateSimulationStateAccessToBuffer(&stateBuffer, simulationState);
    return error;
}

static void PrintJumpHistrogramHeader(SimulationState_t* simulationState, file_t* fstream)
{
    let format = "%-10s-%i\t";
    var index = 0;

    cpp_foreach(histogram, simulationState->JumpStatistics)
    {
        fprintf(fstream, format, "sample_energy", index);
        fprintf(fstream, format, "edge_count", index);
        fprintf(fstream, format, "sample_energy", index);
        fprintf(fstream, format, "posconf_count", index);
        fprintf(fstream, format, "sample_energy", index);
        fprintf(fstream, format, "negconf_count", index);
        fprintf(fstream, format, "sample_energy", index);
        fprintf(fstream, format, "total_count", index++);
        fflush(fstream);
    }
    fprintf(fstream, "\n");
}

static void PrintJumpHistogramOverflows(SimulationState_t* simulationState, file_t* fstream)
{
    let tokFormat = "%-12s\t";
    let cntFormat = FORMAT_I64(12) "\t";
    let lowerToken = "below_min";
    let upperToken = "above_max";

    cpp_foreach(histrogram, simulationState->JumpStatistics)
    {
        fprintf(fstream, tokFormat, lowerToken);
        fprintf(fstream, cntFormat, histrogram->EdgeEnergyHistogram.UnderflowCount);

        fprintf(fstream, tokFormat, lowerToken);
        fprintf(fstream, cntFormat, histrogram->PosConfEnergyHistogram.UnderflowCount);

        fprintf(fstream, tokFormat, lowerToken);
        fprintf(fstream, cntFormat, histrogram->NegConfEnergyHistogram.UnderflowCount);

        fprintf(fstream, tokFormat, lowerToken);
        fprintf(fstream, cntFormat, histrogram->TotalEnergyHistogram.UnderflowCount);
        fflush(fstream);
    }
    fprintf(fstream, "\n");

    cpp_foreach(histrogram, simulationState->JumpStatistics)
    {
        fprintf(fstream, tokFormat, upperToken);
        fprintf(fstream, cntFormat, histrogram->EdgeEnergyHistogram.OverflowCount);

        fprintf(fstream, tokFormat, upperToken);
        fprintf(fstream, cntFormat, histrogram->PosConfEnergyHistogram.OverflowCount);

        fprintf(fstream, tokFormat, upperToken);
        fprintf(fstream, cntFormat, histrogram->NegConfEnergyHistogram.OverflowCount);

        fprintf(fstream, tokFormat, upperToken);
        fprintf(fstream, cntFormat, histrogram->TotalEnergyHistogram.OverflowCount);
        fflush(fstream);
    }
    fprintf(fstream, "\n");
}

static void PrintJumpHistogramContent(SimulationState_t* simulationState, file_t* fstream)
{
    let flpFormat = "%+.6e\t";
    let cntFormat = FORMAT_I64(12) "\t";
    for (var i = 0; i < STATE_JUMPSTAT_SIZE; i++)
    {
        cpp_foreach(histrogram, simulationState->JumpStatistics)
        {
            fprintf(fstream, flpFormat, histrogram->EdgeEnergyHistogram.Stepping * (double) i);
            fprintf(fstream, cntFormat, histrogram->EdgeEnergyHistogram.CountBuffer[i]);

            fprintf(fstream, flpFormat, histrogram->PosConfEnergyHistogram.Stepping * (double) i);
            fprintf(fstream, cntFormat, histrogram->PosConfEnergyHistogram.CountBuffer[i]);

            fprintf(fstream, flpFormat, -1 * histrogram->NegConfEnergyHistogram.Stepping * (double) i);
            fprintf(fstream, cntFormat, histrogram->NegConfEnergyHistogram.CountBuffer[i]);

            fprintf(fstream, flpFormat, histrogram->TotalEnergyHistogram.Stepping * (double) i);
            fprintf(fstream, cntFormat, histrogram->TotalEnergyHistogram.CountBuffer[i]);
            fflush(fstream);
        }
        fprintf(fstream, "\n");
    }
}

error_t PrintFormattedJumpHistograms(SimulationState_t* simulationState, file_t* fstream)
{
    return_if(fstream == NULL || simulationState == NULL, ERR_NULLPOINTER);

    PrintJumpHistrogramHeader(simulationState,fstream);
    PrintJumpHistogramOverflows(simulationState,fstream);
    PrintJumpHistogramContent(simulationState,fstream);
    return ERR_OK;
}

void ExtractAndPrintJumpHistograms(char const *stateFileName, char const* outFileName)
{
    SimulationState_t simulationState;
    var error = LoadContextFreeSimulationState(stateFileName,&simulationState);
    error_assert(error, "Could not load the requested file as a simulation state!");
    error_assert(span_Length(simulationState.Buffer) != 0 ? ERR_OK : ERR_FILE, "The loaded state is empty!");

    var fstream = fopen(outFileName, "w");
    error = PrintFormattedJumpHistograms(&simulationState, fstream);
    fclose(fstream);
    error_assert(error, "Failed to write the data to the target file!");
}