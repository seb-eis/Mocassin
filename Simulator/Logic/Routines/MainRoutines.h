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

error_t PrepareForMainRoutine(__SCONTEXT_PAR);

error_t FinishRoutinePrerun(__SCONTEXT_PAR);

error_t ResetContextAfterPrerun(__SCONTEXT_PAR);

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