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

error_t ConstructEnvLattice(__SCONTEXT_PAR);

error_t PrepareEnvLattice(__SCONTEXT_PAR);

error_t GetEnvReadyStatusEval(__SCONTEXT_PAR);


void CreateLocalJumpDeltaKmc(__SCONTEXT_PAR);

void RollbackLocalJumpDeltaKmc(__SCONTEXT_PAR);

void SetAllStateEnergiesKmc(__SCONTEXT_PAR);

void SetState0And1EnergiesKmc(__SCONTEXT_PAR);

void SetState2EnergyKmc(__SCONTEXT_PAR);

void AdvanceKmcSystemToState2(__SCONTEXT_PAR);


void CreateLocalJumpDeltaMmc(__SCONTEXT_PAR);

void RollbackLocalJumpDeltaMmc(__SCONTEXT_PAR);

void SetAllStateEnergiesMmc(__SCONTEXT_PAR);

void SetState0EnergyMmc(__SCONTEXT_PAR);

void SetState2EnergyMmc(__SCONTEXT_PAR);

void AdvanceMmcSystemToState2(__SCONTEXT_PAR);