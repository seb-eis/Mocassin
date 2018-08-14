//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	StateLogic.c        	    //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Simulation state logic      //
//////////////////////////////////////////

#include "Simulator/Data/Model/State/StateLogic.h"
#include "Simulator/Data/Model/SimContext/SimContext.h"

error_t ConstructSimulationState(sim_context_t* restrict simContext)
{
    return MC_NO_ERROR;
}

error_t PrepareSimulationState(sim_context_t* restrict simContext)
{
    return MC_NO_ERROR;
}

error_t WriteSimulationStateToFile(const sim_context_t* restrict simContext)
{
    return MC_NO_ERROR;
}

error_t LoadSimulationStateFromFile(sim_context_t* restrict simContext)
{
    return MC_NO_ERROR;
}

error_t GetSimulationStateEval(const sim_context_t* restrict simContext)
{
    return MC_NO_ERROR;
}

error_t DumpStateStatsToStream(sim_context_t* restrict simContext, file_t* restrict stream)
{
    if(stream == NULL)
    {
        simContext->ErrorCode |= MC_STREAM_ERROR;
        return MC_STREAM_ERROR;
    }
    return MC_NO_ERROR;
}

error_t DumpStateStatsToFile(sim_context_t* restrict simContext, const char* restrict filePath)
{
    file_t* stream = fopen(filePath, "a");
    simContext->ErrorCode |= DumpStateStatsToStream(simContext, stream);

    if(stream != NULL)
    {
        fclose(stream);
    } 

    return simContext->ErrorCode;
}