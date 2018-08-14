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

error_t ConstructEnvLattice(sim_context_t* restrict simContext);

error_t PrepareEnvLattice(sim_context_t* restrict simContext);

error_t GetEnvReadyStatusEval(sim_context_t* restrict simContext);


void CreateLocalJumpDeltaKmc(sim_context_t* restrict simContext);

void RollbackLocalJumpDeltaKmc(sim_context_t* restrict simContext);

void SetState0And1EnergiesKmc(sim_context_t* restrict simContext);

void SetState2EnergyKmc(sim_context_t* restrict simContext);

void DistributeStateDeltaKmc(sim_context_t* restrict simContext);


void CreateLocalJumpDeltaMmc(sim_context_t* restrict simContext);

void RollbackLocalJumpDeltaMmc(sim_context_t* restrict simContext);

void SetState0And1EnergiesMmc(sim_context_t* restrict simContext);

void SetState2EnergyMmc(sim_context_t* restrict simContext);

void DistributeStateDeltaMmc(sim_context_t* restrict simContext);