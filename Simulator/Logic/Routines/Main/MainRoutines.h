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
#include "Framework/Errors/McErrors.h"
#include "Framework/Basic/BaseTypes/BaseTypes.h"
#include "Simulator/Data/SimContext/ContextAccess.h"

/* Initializer routines */

// Prepares the simulation conteyt for the main simulation routine
void PrepareForMainRoutine(__SCONTEXT_PAR);

// Finishes the main routine pre-run and prepares for the actual run
error_t FinishRoutinePreRun(__SCONTEXT_PAR);

/* Main simulation routines */

// Resets the context after a simulation pre-run
error_t ResetContextAfterPrerun(__SCONTEXT_PAR);

// Starts the main simulation routine including a potential pre-run
error_t StartMainRoutine(__SCONTEXT_PAR);

// Start the main kinetic monte carlo routine
error_t StartMainKmcRoutine(__SCONTEXT_PAR);

// Start the main metropolis monte carlo routine
error_t StartMainMmcRoutine(__SCONTEXT_PAR);

// Run the kmc simulation for the next cycle block
error_t DoNextKmcCycleBlock(__SCONTEXT_PAR);

// Finishes a kmc cycle block completion
error_t FinishKmcCycleBlock(__SCONTEXT_PAR);

// Run the mmc simulation for the next cycle block
error_t DoNextMmcCycleBlock(__SCONTEXT_PAR);

// Finishes a mmc cycle block completion
error_t FinishMmcCycleBlock(__SCONTEXT_PAR);

// Evaluate the abort conditions for a kmc simulation
error_t GetKmcAbortCondEval(__SCONTEXT_PAR);

// Evaluate the abort condition for an mmc simulation
error_t GetMmcAbortCondEval(__SCONTEXT_PAR);

// Synchronizes the main simulation state to the current simulation status
error_t SyncSimulationState(__SCONTEXT_PAR);

// Saves the simulation state
error_t SaveSimulationState(__SCONTEXT_PAR);

// Finishes the main kmc routine
error_t FinishMainRoutineKmc(__SCONTEXT_PAR);

// Finishes the main mmc routine
error_t FinishMainRoutineMmc(__SCONTEXT_PAR);

/* KMC simulation routines */

// Set the next KMC jump selection on the context
void SetNextKmcJumpSelection(__SCONTEXT_PAR);

// Set the next KMC jump path properties on the context
void SetKmcJumpPathProperties(__SCONTEXT_PAR);

// Get the KMC rule evaluation of the current path state in the context
bool_t GetKmcJumpRuleEvaluation(__SCONTEXT_PAR);

// Set the energetic KMC jump properties on the context
void SetKmcJumpProperties(__SCONTEXT_PAR);

// Set the KMC jump evaluation results on the context
void SetKmcJumpEvaluationResults(__SCONTEXT_PAR);

// Set the KMC jump probabilities on the context by the default model calculation
void SetKmcJumpProbabilities(__SCONTEXT_PAR);

/* KMC simulation routines */

// Set the next MMC jump selection on the context
void SetNextMmcJumpSelection(__SCONTEXT_PAR);

// Set the MMC jump path properties on the context
void SetMmcJumpPathProperties(__SCONTEXT_PAR);

// Get the MMC jump rule evaluation of the current path state in the context
bool_t GetMmcJumpRuleEvaluation(__SCONTEXT_PAR);

// Set the energetic MMC jump properties on the the context
void SetMmcJumpProperties(__SCONTEXT_PAR);

// Set the MMC jump evaluation results on the context
void SetMmcJumpEvaluationResults(__SCONTEXT_PAR);

// Set the MMC jump probabilities on the context by the default model calculation
void SetMmcJumpProbabilities(__SCONTEXT_PAR);