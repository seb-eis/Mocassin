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
#include "Simulator/Logic/Routines/HelperRoutines.h"

#define MC_KMC_ROUTINE_FLAG     0x1
#define MC_MMC_ROUTINE_FLAG     0x2
#define MC_PRE_ROUTINE_FLAG     0x4
#define MC_SIM_CONTINUE_FLAG    0x8
#define MC_SIM_COMPLETED_FLAG   0x10
#define MC_SIM_TIME_ABORT_FLAG  0x20
#define MC_SIM_COND_ABORT_FLAG  0x40
#define MC_SIM_RATE_ABORT_FLAG  0x80
#define MC_SIM_FIRST_CYCLE_FLAG 0x100
#define MC_STATE_INIT_FLAG      0x20000000
#define MC_SIM_ABORT_FLAG       0x40000000
#define MC_STATE_ERROR_FLAG     0x80000000

error_t LoadCommandLineArgs(sim_context_t* restrict simContext, int32_t argc, char const* const* argv);

error_t LoadSimulationPlugins(sim_context_t* restrict simContext);

error_t LoadSimulationModel(sim_context_t* restrict simContext);

error_t LoadSimulationState(sim_context_t* restrict simContext);

error_t ResetContextToDefault(sim_context_t* restrict simContext);

error_t PrepareDynamicModel(sim_context_t* restrict simContext);

error_t PrepareForMainRoutine(sim_context_t* restrict simContext);

error_t FinishRoutinePrerun(sim_context_t* restrict simContext);

error_t StartMainRoutine(sim_context_t* restrict simContext);

error_t StartMainKmcRoutine(sim_context_t* restrict simContext);

error_t StartMainMmcRoutine(sim_context_t* restrict simContext);

error_t DoNextKmcCycleBlock(sim_context_t* restrict simContext);

error_t FinishKmcCycleBlock(sim_context_t* restrict simContext);

error_t DoNextMmcCycleBlock(sim_context_t* restrict simContext);

error_t FinishMmcCycleBlock(sim_context_t* restrict simContext);

error_t GetKmcAbortCondEval(sim_context_t* restrict simContext);

error_t GetMmcAbortCondEval(sim_context_t* restrict simContext);

error_t SyncSimulationState(sim_context_t* restrict simContext);

error_t SaveSimulationState(sim_context_t* restrict simContext);

error_t FinishMainRoutineKmc(sim_context_t* restrict simContext);

error_t FinishMainRoutineMmc(sim_context_t* restrict simContext);


void SetKmcJumpSelection(sim_context_t* restrict simContext);

void SetKmcJumpPathProperties(sim_context_t* restrict simContext);

bool_t GetKmcJumpRuleEval(sim_context_t* restrict simContext);

void SetKmcJumpEvalFail(sim_context_t* restrict simContext);

void SetKmcJumpProperties(sim_context_t* restrict simContext);

void SetKmcJumpEvaluation(sim_context_t* restrict simContext);

void SetKmcJumpProbsDefault(sim_context_t* restrict simContext);


void SetMmcJumpSelection(sim_context_t* restrict simContext);

void SetMmcJumpPathProperties(sim_context_t* restrict simContext);

bool_t GetMmcJumpRuleEval(sim_context_t* restrict simContext);

void SetMmcJumpEvalFail(sim_context_t* restrict simContext);

void SetMmcJumpProperties(sim_context_t* restrict simContext);

void SetMmcJumpEvaluation(sim_context_t* restrict simContext);

void SetMmcJumpProbsDefault(sim_context_t* restrict simContext);