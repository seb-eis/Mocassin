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

#define NO_JUMP_POOL_ENTRY -1

// Initializes the jump pool in size and start status on the passed simulation context
error_t init_jump_pool(sim_context_t* restrict sim_context);

// Performs the random rolling of the kmc transition base info using the provided pool. Results in a start env state id and a jump direction id
void roll_kmc_transition_info(jump_pool_t* restrict jump_pool, roll_info_t* restrict roll_info);

// Performs the random rolling of the mmc transition base info using the provided pool and total env state count. Results in a start env state id and an env state id used as target offset
void roll_mmc_transition_info(jump_pool_t* restrict jump_pool, const int32_t env_state_count, roll_info_t* restrict roll_info);

// Updates the selection pool of the passed simulation context after a successful mmc/kmc transition. Returns true if the total number of possible jumps has changed
bool_t update_jump_pool(sim_context_t* restrict sim_context);

