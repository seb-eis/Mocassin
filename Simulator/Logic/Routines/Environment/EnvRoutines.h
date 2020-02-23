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
void InitializeEnvironmentLinkingSystem(SCONTEXT_PARAMETER);

// Synchronizes all energy status options for the dynamic simulation lattice
void ResynchronizeEnvironmentEnergyStatus(SCONTEXT_PARAMETER);

// Set the status of environment state at the provided id to default conditions and the passed particle id
void SetEnvironmentStateToDefault(SCONTEXT_PARAMETER, int32_t environmentId, byte_t particleId);

/* Simulation routines KMC */

// Backups required data and creates the local jump delta for KMC transitions
void CreateAndBackupKmcTransitionDelta(SCONTEXT_PARAMETER);

// Restores the pre-delta status of the system in KMC by loading the affiliated backups
void LoadKmcTransitionDeltaBackup(SCONTEXT_PARAMETER);

// Sets all required state energies for the current KMC transition on the main cycle state (S0,S1,S2 & E_Field)
void SetKmcStateEnergiesOnContext(SCONTEXT_PARAMETER);

// Sets the state energies S0, S1 base and the electric field energy for the current KMC transition on the main cycle state
void SetKmcStartTransitionBaseAndFieldEnergyStatesOnContext(SCONTEXT_PARAMETER);

// Sets the state energy S2 for the current KMC transition on the main cycle state
void SetFinalKmcStateEnergyOnContext(SCONTEXT_PARAMETER);

// Advances the system to the final state using the currently active KMC transition
void AdvanceKmcSystemToFinalState(SCONTEXT_PARAMETER);

/* Simulation routines MMC */

// Backups required data and creates the local jump delta for MMC transitions if required, returns true if backup was required
bool_t TryCreateAndBackupMmcTransitionDelta(SCONTEXT_PARAMETER);

// Restores the pre-delta status of the system in MMC by loading the affiliated backups
void LoadMmcTransitionDeltaBackup(SCONTEXT_PARAMETER);

// Sets all required state energies for the current MMC transition on the main cycle state
void SetMmcStateEnergiesOnContext(SCONTEXT_PARAMETER);

// Sets the state energy 0 for the current MMC transition on the main cycle state
void SetMmcStartStateEnergyOnContext(SCONTEXT_PARAMETER);

// Sets the state energy 2 for the current MMC transition on the main cycle state
void SetMmcFinalStateEnergyOnContext(SCONTEXT_PARAMETER);

// Advances the system to the final state using the currently active MMC transition
void AdvanceMmcSystemToFinalState(SCONTEXT_PARAMETER);