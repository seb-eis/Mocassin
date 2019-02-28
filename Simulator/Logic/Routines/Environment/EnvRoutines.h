//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	EnvRoutines.h        		//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Env routines for simulation //
//////////////////////////////////////////

#pragma once
#include "Framework/Errors/McErrors.h"
#include "Simulator/Data/SimContext/ContextAccess.h"

/* Initializer routines */

// Builds the environment linking system of the dynamic simulation environment lattice
void BuildEnvironmentLinkingSystem(SCONTEXT_PARAM);

// Synchronizes all energy status options for the dynamic simulation lattice
void ResynchronizeEnvironmentEnergyStatus(SCONTEXT_PARAM);

// Set the status of environment state at the provided id to default conditions and the passed particle id
void SetEnvironmentStateToDefault(SCONTEXT_PARAM, const int32_t environmentId, const byte_t particleId);

/* Simulation routines KMC */

// Backups required data and creates the local jump delta for KMC transitions
void KMC_CreateBackupAndJumpDelta(SCONTEXT_PARAM);

// Restores the pre-delta status of the system in KMC by loading the affiliated backups
void KMC_LoadJumpDeltaBackup(SCONTEXT_PARAM);

// Sets all required state energies for the current KMC transition on the main cycle state
void KMC_SetStateEnergies(SCONTEXT_PARAM);

// Sets the state energies 0 and 1 for the current KMC transition on the main cycle state
void KMC_SetStartAndTransitionStateEnergies(SCONTEXT_PARAM);

// Sets the state energy 2 for the current KMC transition on the main cycle state
void KMC_SetFinalStateEnergy(SCONTEXT_PARAM);

// Advances the system to the final state using the currently active KMC transition
void KMC_AdvanceSystemToFinalState(SCONTEXT_PARAM);

/* Simulation routines MMC */

// Backups required data and creates the local jump delta for MMC transitions if required, returns true if backup was required
bool_t MMC_TryCreateBackupAndJumpDelta(SCONTEXT_PARAM);

// Restores the pre-delta status of the system in MMC by loading the affiliated backups
void MMC_LoadJumpDeltaBackup(SCONTEXT_PARAM);

// Sets all required state energies for the current MMC transition on the main cycle state
void MMC_SetStateEnergies(SCONTEXT_PARAM);

// Sets the state energy 0 for the current MMC transition on the main cycle state
void MMC_SetStartStateEnergy(SCONTEXT_PARAM);

// Sets the state energy 2 for the current MMC transition on the main cycle state
void MMC_SetFinalStateEnergy(SCONTEXT_PARAM);

// Advances the system to the final state using the currently active MMC transition
void MMC_AdvanceSystemToFinalState(SCONTEXT_PARAM);