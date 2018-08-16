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

error_t ConstructSimulationState(__SCONTEXT_PAR)
{
    return ERR_OK;
}

error_t PrepareSimulationState(__SCONTEXT_PAR)
{
    return ERR_OK;
}

error_t WriteSimulationStateToFile(const __SCONTEXT_PAR)
{
    return ERR_OK;
}

error_t LoadSimulationStateFromFile(__SCONTEXT_PAR)
{
    return ERR_OK;
}

error_t GetSimulationStateEval(const __SCONTEXT_PAR)
{
    return ERR_OK;
}

error_t DumpStateStatsToStream(__SCONTEXT_PAR, file_t* restrict stream)
{
    if(stream == NULL)
    {
        simContext->ErrorCode |= ERR_STREAM;
        return ERR_STREAM;
    }
    return ERR_OK;
}

error_t DumpStateStatsToFile(__SCONTEXT_PAR, const char* restrict filePath)
{
    file_t* stream = fopen(filePath, "a");
    simContext->ErrorCode |= DumpStateStatsToStream(simContext, stream);

    if(stream != NULL)
    {
        fclose(stream);
    } 

    return simContext->ErrorCode;
}