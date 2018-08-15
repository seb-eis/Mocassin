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

error_t ConstructEnvLattice(_SCTPARAM);

error_t PrepareEnvLattice(_SCTPARAM);

error_t GetEnvReadyStatusEval(_SCTPARAM);


void CreateLocalJumpDeltaKmc(_SCTPARAM);

void RollbackLocalJumpDeltaKmc(_SCTPARAM);

void SetState0And1EnergiesKmc(_SCTPARAM);

void SetState2EnergyKmc(_SCTPARAM);

void CreateFullStateDeltaKmc(_SCTPARAM);


void CreateLocalJumpDeltaMmc(_SCTPARAM);

void RollbackLocalJumpDeltaMmc(_SCTPARAM);

void SetState0And1EnergiesMmc(_SCTPARAM);

void SetState2EnergyMmc(_SCTPARAM);

void CreateFullStateDeltaMmc(_SCTPARAM);