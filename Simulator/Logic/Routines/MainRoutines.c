//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	EnvRoutines.c        		//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Main simulation routines    //
//////////////////////////////////////////

#include "Simulator/Logic/Routines/MainRoutines.h"

error_t LoadCommandLineArgs(sim_context_t* restrict simContext, int32_t argc, char const* const* argv)
{
    return MC_NO_ERROR;
}

error_t LoadSimulationPlugins(sim_context_t* restrict simContext)
{
    return MC_NO_ERROR;
}

error_t LoadSimulationModel(sim_context_t* restrict simContext)
{
    return MC_NO_ERROR;
}

error_t LoadSimulationState(sim_context_t* restrict simContext)
{
    return MC_NO_ERROR;
}

error_t PrepareDynamicModel(sim_context_t* restrict simContext)
{
    return MC_NO_ERROR;
}

error_t PrepareForMainRoutine(sim_context_t* restrict simContext)
{
    return MC_NO_ERROR;
}

error_t StartMainRoutine(sim_context_t* restrict simContext)
{
    error_t simFlags;
    if (simFlags & MC_ERR_ROUTINE_FLAG)
    {
        MC_DUMP_ERROR_AND_EXIT(MC_SIM_ERROR, "Main routine execution not started. The routine error flag is set.");
    }
    if (simFlags & MC_KMC_ROUTINE_FLAG)
    {
        return StartMainKmcRoutine(simContext);
    }
    if (simFlags & MC_MMC_ROUTINE_FLAG)
    {
        return StartMainMmcRoutine(simContext);
    }
    MC_DUMP_ERROR_AND_EXIT(MC_SIM_ERROR, "Unexpected simulation error. No routine selection triggered.");
}

error_t StartMainKmcRoutine(sim_context_t* restrict simContext)
{
    return MC_NO_ERROR;
}

error_t StartMainMmcRoutine(sim_context_t* restrict simContext)
{
    return MC_NO_ERROR;
}

error_t DoNextKmcCycleBlock(sim_context_t* restrict simContext)
{
    return MC_NO_ERROR;
}

error_t DoNextMmcCycleBlock(sim_context_t* restrict simContext)
{
    return MC_NO_ERROR;
}

error_t GetKmcAbortCondEval(sim_context_t* restrict simContext)
{
    return MC_NO_ERROR;
}

error_t GetMmcAbortCondEval(sim_context_t* restrict simContext)
{
    return MC_NO_ERROR;
}

error_t SaveSimulationState(sim_context_t* restrict simContext)
{
    return MC_NO_ERROR;
}

error_t FinishMainRoutine(sim_context_t* restrict simContext)
{
    return MC_NO_ERROR;
}


void GetKmcJumpSelection(sim_context_t* restrict simContext)
{
    
}

void GetKmcJumpPathState(sim_context_t* restrict simContext)
{
    
}

bool_t GetKmcJumpRuleEval(sim_context_t* restrict simContext)
{
    return false;
}

void GetKmcJumpProbs(sim_context_t* restrict simContext)
{
    
}

bool_t GetKmcJumpEval(sim_context_t* restrict simContext)
{
    return false;
}

void AdvanceKmcState(sim_context_t* restrict simContext)
{
    
}

void RollbackKmcState(sim_context_t* restrict simContext)
{
    
}


void GetMmcJumpSelection(sim_context_t* restrict simContext)
{
    
}

void GetMmcJumpPathState(sim_context_t* restrict simContext)
{
    
}

bool_t GetMmcJumpRuleEval(sim_context_t* restrict simContext)
{
    return false;
}

void GetMmcJumpProbs(sim_context_t* restrict simContext)
{
    
}

bool_t GetMmcJumpEval(sim_context_t* restrict simContext)
{
    return false;
}

void AdvanceMmcState(sim_context_t* restrict simContext)
{
    
}

void RollbackMmcState(sim_context_t* restrict simContext)
{
    
}
