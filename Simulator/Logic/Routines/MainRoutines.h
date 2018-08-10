//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	EnvRoutines.h        		//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Main simulation routines    //
//////////////////////////////////////////

#pragma once
#include <math.h>
#include "Simulator/Data/Model/SimContext/SimContext.h"
#include "Simulator/Logic/Routines/EnvRoutines.h"

#define MC_KMC_ROUTINE_FLAG 0b1
#define MC_MMC_ROUTINE_FLAG 0b10
#define MC_ERR_ROUTINE_FLAG 1LL << 63

error_t LoadCommandLineArgs(sim_context_t* restrict simContext, int32_t argc, char const* const* argv);

error_t LoadSimulationPlugins(sim_context_t* restrict simContext);

error_t LoadSimulationModel(sim_context_t* restrict simContext);

error_t LoadSimulationState(sim_context_t* restrict simContext);

error_t PrepareDynamicModel(sim_context_t* restrict simContext);

error_t PrepareForMainRoutine(sim_context_t* restrict simContext);

error_t StartMainRoutine(sim_context_t* restrict simContext);

error_t StartMainKmcRoutine(sim_context_t* restrict simContext);

error_t StartMainMmcRoutine(sim_context_t* restrict simContext);

error_t DoNextKmcCycleBlock(sim_context_t* restrict simContext);

error_t DoNextMmcCycleBlock(sim_context_t* restrict simContext);

error_t GetKmcAbortCondEval(sim_context_t* restrict simContext);

error_t GetMmcAbortCondEval(sim_context_t* restrict simContext);

error_t SaveSimulationState(sim_context_t* restrict simContext);

error_t FinishMainRoutine(sim_context_t* restrict simContext);


void GetKmcJumpSelection(sim_context_t* restrict simContext);

void GetKmcJumpPathState(sim_context_t* restrict simContext);

bool_t GetKmcJumpRuleEval(sim_context_t* restrict simContext);

void GetKmcJumpProbs(sim_context_t* restrict simContext);

bool_t GetKmcJumpEval(sim_context_t* restrict simContext);

void AdvanceKmcState(sim_context_t* restrict simContext);

void RollbackKmcState(sim_context_t* restrict simContext);


void GetMmcJumpSelection(sim_context_t* restrict simContext);

void GetMmcJumpPathState(sim_context_t* restrict simContext);

bool_t GetMmcJumpRuleEval(sim_context_t* restrict simContext);

void GetMmcJumpProbs(sim_context_t* restrict simContext);

bool_t GetMmcJumpEval(sim_context_t* restrict simContext);

void AdvanceMmcState(sim_context_t* restrict simContext);

void RollbackMmcState(sim_context_t* restrict simContext);