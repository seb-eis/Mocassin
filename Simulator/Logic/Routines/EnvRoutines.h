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
#include "Simulator/Data/Model/SimContext/SimContext.h"

/* Initializer routines */

void BuildEnvironmentLinkingSystem(__SCONTEXT_PAR);

void SyncEnvironmentEnergyStatus(__SCONTEXT_PAR);

void SetEnvStateStatusToDefault(__SCONTEXT_PAR, const int32_t envId, const byte_t parId);

/* Simulation routines KMC */

void CreateLocalJumpDeltaKmc(__SCONTEXT_PAR);

void RollbackLocalJumpDeltaKmc(__SCONTEXT_PAR);

void SetAllStateEnergiesKmc(__SCONTEXT_PAR);

void SetState0And1EnergiesKmc(__SCONTEXT_PAR);

void SetState2EnergyKmc(__SCONTEXT_PAR);

void AdvanceKmcSystemToState2(__SCONTEXT_PAR);

/* Simulation routines MMC */

void CreateLocalJumpDeltaMmc(__SCONTEXT_PAR);

void RollbackLocalJumpDeltaMmc(__SCONTEXT_PAR);

void SetAllStateEnergiesMmc(__SCONTEXT_PAR);

void SetState0EnergyMmc(__SCONTEXT_PAR);

void SetState2EnergyMmc(__SCONTEXT_PAR);

void AdvanceMmcSystemToState2(__SCONTEXT_PAR);