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

#define FLG_KMC             0x1
#define FLG_MMC             0x2
#define FLG_PRERUN          0x4
#define FLG_CONTINUE        0x8
#define FLG_COMPLETED       0x10
#define FLG_TIMEOUT         0x20
#define FLG_ABORTCONDITION  0x40
#define FLG_RATELIMIT       0x80
#define FLG_FIRSTCYCLE      0x100
#define FLG_INITIALIZED     0x20000000
#define FLG_ABORT           0x40000000
#define FLG_STATEERROR      0x80000000

error_t LoadCommandLineArgs(__SCONTEXT_PAR, int32_t argc, char const* const* argv);

error_t LoadSimulationPlugins(__SCONTEXT_PAR);

error_t LoadSimulationModel(__SCONTEXT_PAR);

error_t LoadSimulationState(__SCONTEXT_PAR);

error_t ResetContextToDefault(__SCONTEXT_PAR);

error_t PrepareDynamicModel(__SCONTEXT_PAR);

error_t PrepareForMainRoutine(__SCONTEXT_PAR);

error_t FinishRoutinePrerun(__SCONTEXT_PAR);

error_t StartMainRoutine(__SCONTEXT_PAR);

error_t StartMainKmcRoutine(__SCONTEXT_PAR);

error_t StartMainMmcRoutine(__SCONTEXT_PAR);

error_t DoNextKmcCycleBlock(__SCONTEXT_PAR);

error_t FinishKmcCycleBlock(__SCONTEXT_PAR);

error_t DoNextMmcCycleBlock(__SCONTEXT_PAR);

error_t FinishMmcCycleBlock(__SCONTEXT_PAR);

error_t GetKmcAbortCondEval(__SCONTEXT_PAR);

error_t GetMmcAbortCondEval(__SCONTEXT_PAR);

error_t SyncSimulationState(__SCONTEXT_PAR);

error_t SaveSimulationState(__SCONTEXT_PAR);

error_t FinishMainRoutineKmc(__SCONTEXT_PAR);

error_t FinishMainRoutineMmc(__SCONTEXT_PAR);


void SetKmcJumpSelection(__SCONTEXT_PAR);

void SetKmcJumpPathProperties(__SCONTEXT_PAR);

bool_t GetKmcJumpRuleEval(__SCONTEXT_PAR);

void SetKmcJumpEvalFail(__SCONTEXT_PAR);

void SetKmcJumpProperties(__SCONTEXT_PAR);

void SetKmcJumpEvaluation(__SCONTEXT_PAR);

void SetKmcJumpProbsDefault(__SCONTEXT_PAR);


void SetMmcJumpSelection(__SCONTEXT_PAR);

void SetMmcJumpPathProperties(__SCONTEXT_PAR);

bool_t GetMmcJumpRuleEval(__SCONTEXT_PAR);

void SetMmcJumpEvalFail(__SCONTEXT_PAR);

void SetMmcJumpProperties(__SCONTEXT_PAR);

void SetMmcJumpEvaluation(__SCONTEXT_PAR);

void SetMmcJumpProbsDefault(__SCONTEXT_PAR);