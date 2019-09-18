//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	MainRoutines.h        		//
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

/* Initializer and synchronization routines */

// Prepares the simulation context for the main simulation routine
void PrepareForMainRoutine(SCONTEXT_PARAM);

// Synchronizes the main simulation state to the current simulation status
error_t SyncSimulationStateToRunStatus(SCONTEXT_PARAM);

// Synchronizes the main state lattice to the dynamic environment lattice status
error_t SyncMainStateLatticeToRunStatus(SCONTEXT_PARAM);

// Synchronizes the main state counters to the dynamic counters run status
error_t SyncMainStateCountersToRunStatus(SCONTEXT_PARAM);

// Synchronizes the main state meta data to the dynamic run status
error_t SyncMainStateMetaDataToRunStatus(SCONTEXT_PARAM);

// Saves the simulation state
error_t SaveSimulationState(SCONTEXT_PARAM);

/* Main simulation routine functions with errors */

// Starts the main simulation routine including a potential pre-run
error_t StartMainSimulationRoutine(SCONTEXT_PARAM);

/* KMC routine */

// Start the KMC pre run routine
error_t KMC_StartPreRunRoutine(SCONTEXT_PARAM);

// Run the kmc simulation for one execution phase using the self optimizing pre run routine
error_t KMC_EnterSOPExecutionPhase(SCONTEXT_PARAM);

// Finishes the KMC pre run routine
error_t KMC_FinishPreRun(SCONTEXT_PARAM);

// Start the main kinetic monte carlo routine
error_t KMC_StartMainRoutine(SCONTEXT_PARAM);

// Run the kmc simulation for one execution phase
error_t KMC_EnterExecutionPhase(SCONTEXT_PARAM);

// Finishes a kmc execution phase
error_t KMC_FinishExecutionPhase(SCONTEXT_PARAM);

// Evaluate the abort conditions for a kmc simulation
error_t KMC_CheckAbortConditions(SCONTEXT_PARAM);

// Finishes the main kmc routine
error_t KMC_FinishMainRoutine(SCONTEXT_PARAM);

/* MMC routine */

// Start the KMC pre run routine
error_t MMC_StartPreRunRoutine(SCONTEXT_PARAM);

// Finishes the MMC pre run routine
error_t MMC_FinishPreRun(SCONTEXT_PARAM);

// Run the mmc simulation for one execution phase
error_t MMC_EnterExecutionPhase(SCONTEXT_PARAM);

// Start the main metropolis monte carlo routine
error_t MMC_StartMainRoutine(SCONTEXT_PARAM);

// Finishes the mmc execution phase
error_t MMC_FinishExecutionPhase(SCONTEXT_PARAM);

// Evaluate the abort condition for an mmc simulation
error_t MMC_CheckAbortConditions(SCONTEXT_PARAM);

// Finishes the main mmc routine
error_t MMC_FinishMainRoutine(SCONTEXT_PARAM);

/* KMC simulation non-error sub-routines */

// Executes one cycle of the KMC simulation routine with the passed simulation context
void KMC_ExecuteSimulationCycle(SCONTEXT_PARAM);

// Executes one self optimizing cycle of the KMC simulation routine with the passed simulation context
void KMC_ExecuteSOPSimulationCycle(SCONTEXT_PARAM);

// Set the next KMC jump selection on the context
void KMC_SetNextJumpSelection(SCONTEXT_PARAM);

// Set the next KMC jump path properties on the context
void KMC_SetJumpPathProperties(SCONTEXT_PARAM);

// Tries to set the active KMC jump rule in the simulation context. Returns false if no rule for the current path exists
JumpRule_t* KMC_TrySetActiveJumpRule(SCONTEXT_PARAM);

// Set the energetic KMC jump properties on the context
void KMC_SetJumpProperties(SCONTEXT_PARAM);

// Set the KMC jump evaluation results on the context for cases where energetic evaluation is required
void KMC_OnEnergeticJumpEvaluation(SCONTEXT_PARAM);

// Set the KMC jump probabilities on the context by the default model calculation
void KMC_SetJumpProbabilities(SCONTEXT_PARAM);

/* MMC simulation non-error sub-routines */

// Executes one cycle of the MMC simulation routine with the passed simulation context
void MMC_ExecuteSimulationCycle(SCONTEXT_PARAM);

// Set the next MMC jump selection on the context
void MMC_SetNextJumpSelection(SCONTEXT_PARAM);

// Set the MMC jump path properties on the context
void MMC_SetJumpPathProperties(SCONTEXT_PARAM);

// Tries to set the active MMC jump rule in the simulation context. Returns false if no rule for the current path exists
JumpRule_t* MMC_TrySetActiveJumpRule(SCONTEXT_PARAM);

// Set the energetic MMC jump properties on the the context
void MMC_SetJumpProperties(SCONTEXT_PARAM);

// Set the MMC jump evaluation results on the context for cases where energetic evaluation is required
void MMC_OnEnergeticJumpEvaluation(SCONTEXT_PARAM);

// Set the MMC jump probabilities on the context by the default model calculation
void MMC_SetJumpProbabilities(SCONTEXT_PARAM);