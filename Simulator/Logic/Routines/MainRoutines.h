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

error_t LoadCommandLineArgs(_SCTPARAM, int32_t argc, char const* const* argv);

error_t LoadSimulationPlugins(_SCTPARAM);

error_t LoadSimulationModel(_SCTPARAM);

error_t LoadSimulationState(_SCTPARAM);

error_t ResetContextToDefault(_SCTPARAM);

error_t PrepareDynamicModel(_SCTPARAM);

error_t PrepareForMainRoutine(_SCTPARAM);

error_t FinishRoutinePrerun(_SCTPARAM);

error_t StartMainRoutine(_SCTPARAM);

error_t StartMainKmcRoutine(_SCTPARAM);

error_t StartMainMmcRoutine(_SCTPARAM);

error_t DoNextKmcCycleBlock(_SCTPARAM);

error_t FinishKmcCycleBlock(_SCTPARAM);

error_t DoNextMmcCycleBlock(_SCTPARAM);

error_t FinishMmcCycleBlock(_SCTPARAM);

error_t GetKmcAbortCondEval(_SCTPARAM);

error_t GetMmcAbortCondEval(_SCTPARAM);

error_t SyncSimulationState(_SCTPARAM);

error_t SaveSimulationState(_SCTPARAM);

error_t FinishMainRoutineKmc(_SCTPARAM);

error_t FinishMainRoutineMmc(_SCTPARAM);


void SetKmcJumpSelection(_SCTPARAM);

void SetKmcJumpPathProperties(_SCTPARAM);

bool_t GetKmcJumpRuleEval(_SCTPARAM);

void SetKmcJumpEvalFail(_SCTPARAM);

void SetKmcJumpProperties(_SCTPARAM);

void SetKmcJumpEvaluation(_SCTPARAM);

void SetKmcJumpProbsDefault(_SCTPARAM);


void SetMmcJumpSelection(_SCTPARAM);

void SetMmcJumpPathProperties(_SCTPARAM);

bool_t GetMmcJumpRuleEval(_SCTPARAM);

void SetMmcJumpEvalFail(_SCTPARAM);

void SetMmcJumpProperties(_SCTPARAM);

void SetMmcJumpEvaluation(_SCTPARAM);

void SetMmcJumpProbsDefault(_SCTPARAM);