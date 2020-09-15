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
void PrepareSimulationContextForMainRoutine(SCONTEXT_PARAMETER);

// Synchronizes the main simulation state to the current simulation status
error_t SyncSimulationStateToRunStatus(SCONTEXT_PARAMETER);

// Synchronizes the main state lattice to the dynamic environment lattice status
error_t SyncMainStateLatticeToRunStatus(SCONTEXT_PARAMETER);

// Synchronizes the main state counters to the dynamic counters run status
error_t SyncMainStateCountersToRunStatus(SCONTEXT_PARAMETER);

// Synchronizes the main state meta data to the dynamic run status
error_t SyncMainStateMetaDataToRunStatus(SCONTEXT_PARAMETER);

// Saves the simulation state to the currently set I/O folder (as run or pre-run file)
error_t SaveCurrentSimulationStateToOutDirectory(SCONTEXT_PARAMETER);

/* Main simulation routine functions with errors */

// Top level entry point for the main simulation routine including a potential pre-run
error_t StartMainSimulationRoutine(SCONTEXT_PARAMETER);

/* Shared routine components */

// Executes a common block finish that synchronize the simulation state to the execution state, saves the state and calls a potential output plugin
void ExecuteSharedMcBlockFinisher(SCONTEXT_PARAMETER);

/* KMC routine */

// Start the KMC pre run routine
error_t StartKmcPreRunRoutine(SCONTEXT_PARAMETER);

// Run the kmc simulation for one execution block using the self optimizing pre run routine
error_t RunOneKmcAutoOptimizationExecutionBlock(SCONTEXT_PARAMETER);

// Finishes the KMC pre run routine
error_t FinishKmcPreRunRoutine(SCONTEXT_PARAMETER);

// Start the main kinetic monte carlo routine
error_t StartKmcMainRoutine(SCONTEXT_PARAMETER);

// Run the kmc simulation for one execution block
error_t RunOneKmcExecutionBlock(SCONTEXT_PARAMETER);

// Finishes a kmc execution phase
error_t FinishKmcExecutionBlock(SCONTEXT_PARAMETER);

// Updates and evaluates the abort conditions for a kmc simulation
error_t UpdateAndEvaluateKmcAbortConditions(SCONTEXT_PARAMETER);

// Finishes the main kmc routine
error_t FinishMainKmcRoutine(SCONTEXT_PARAMETER);

/* MMC routine */

// Start the MMC pre run routine (Not implemented)
error_t StartMmcPreRunRoutine(SCONTEXT_PARAMETER);

// Finishes the MMC pre run routine (Not implemented)
error_t FinishMmcPreRunRoutine(SCONTEXT_PARAMETER);

// Run the mmc simulation for one execution block
error_t RunOneMmcExecutionBlock(SCONTEXT_PARAMETER);

// Start the main metropolis monte carlo routine
error_t StartMmcMainRoutine(SCONTEXT_PARAMETER);

// Finishes the mmc execution phase
error_t FinishMmcExecutionBlock(SCONTEXT_PARAMETER);

// Updates and evaluates the abort conditions for a nmc simulation
error_t UpdateAndEvaluateMmcAbortConditions(SCONTEXT_PARAMETER);

// Finishes the main mmc routine
error_t FinishMmcMainRoutine(SCONTEXT_PARAMETER);

/* KMC simulation non-error sub-routines */

// Executes one cycle of the KMC simulation routine with the passed simulation context
void ExecuteKmcSimulationCycle(SCONTEXT_PARAMETER);

// Executes one self optimizing cycle of the KMC simulation routine with the passed simulation context
void ExecuteKmcAutoOptimizingSimulationCycle(SCONTEXT_PARAMETER);

// Set the next KMC jump selection on the context
void SetNextKmcJumpSelectionOnContext(SCONTEXT_PARAMETER);

// Set the next KMC jump path properties on the context
void SetKmcJumpPathPropertiesOnContext(SCONTEXT_PARAMETER);

// Tries to set the active KMC jump rule in the simulation context. Returns NULL or the found rule
JumpRule_t* TrySetActiveKmcJumpRuleOnContext(SCONTEXT_PARAMETER);

// Set the energetic KMC jump properties on the context
void SetKmcJumpPropertiesOnContext(SCONTEXT_PARAMETER);

// Set the KMC jump evaluation results on the context for cases where energetic evaluation is required
void SetEnergeticKmcEventEvaluationOnContext(SCONTEXT_PARAMETER);

// Set the KMC jump probabilities on the context by the default model calculation
void SetKmcJumpProbabilitiesOnContext(SCONTEXT_PARAMETER);

/* MMC simulation non-error sub-routines */

// Executes one cycle of the MMC simulation routine with the passed simulation context
void ExecuteMmcSimulationCycle(SCONTEXT_PARAMETER);

// Set the next MMC jump selection on the context
void SetNextMmcJumpSelectionOnContext(SCONTEXT_PARAMETER);

// Set the MMC jump path properties on the context
void SetMmcJumpPathPropertiesOnContext(SCONTEXT_PARAMETER);

// Tries to set the active MMC jump rule in the simulation context. Returns NULL or the found rule
JumpRule_t* TrySetActiveMmcJumpRuleOnContext(SCONTEXT_PARAMETER);

// Set the energetic MMC jump properties on the the context
void SetMmcJumpPropertiesOnContext(SCONTEXT_PARAMETER);

// Set the MMC jump evaluation results on the context for cases where energetic evaluation is required
void SetEnergeticMmcEventEvaluationOnContext(SCONTEXT_PARAMETER);

// Set the MMC jump probabilities on the context by the default model calculation
void SetMmcJumpProbabilitiesOnContext(SCONTEXT_PARAMETER);

/* Special MMC simulation non-error sub-routines */

// Executes one cycle of the MMC simulation routine with the passed simulation context (Exp factor is multiplied with alpha)
void ExecuteMmcSimulationCycleWithAlpha(SCONTEXT_PARAMETER, double alpha);

// Set the MMC jump probabilities on the context by the default model calculation (Exp factor is multiplied with alpha)
void SetMmcJumpProbabilitiesOnContextWithAlpha(SCONTEXT_PARAMETER, double alpha);

// Set the MMC jump evaluation results on the context for cases where energetic evaluation is required (Exp factor is multiplied with alpha)
void OnEnergeticMmcJumpEvaluationWithAlpha(SCONTEXT_PARAMETER, double alpha);