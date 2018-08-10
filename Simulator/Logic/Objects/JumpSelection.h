//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	JumpSelection.h        		//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Jump selection logic        //
//////////////////////////////////////////

#pragma once
#include "Framework/Errors/McErrors.h"
#include "Framework/Math/Random/PcgRandom.h"
#include "Framework/Basic/BaseTypes/BaseTypes.h"
#include "Simulator/Data/Model/SimContext/SimContext.h"

#define MC_NO_POOL 0

error_t ConstructJumpPool(sim_context_t* restrict simContext);

error_t PrepareJumpPool(sim_context_t* restrict simContext);

void RollNextKmcSelect(sim_context_t* restrict simContext);

void RollNextMmcSelect(sim_context_t* restrict simContext);

bool_t GetJumpPoolUpdateKmc(sim_context_t* restrict sim_context);

bool_t GetJumpPoolUpdateMmc(sim_context_t* restrict sim_context);

