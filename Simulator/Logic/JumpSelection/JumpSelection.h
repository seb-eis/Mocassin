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
#include "Simulator/Data/SimContext/ContextAccess.h"

/* Initializer routines*/

// Handles the environment state registration in the pool for the passed environment id on the passed context
error_t HandleEnvStatePoolRegistration(SCONTEXT_PARAM, int32_t environmentId);

/* Simulation required routines */

// Rolls the next jump selection data for a KMC simulation on the passed context
void KMC_RollNextJumpSelection(SCONTEXT_PARAM);

// Rolls the next jump selection data for an MMC simulation on the passed context
void MMC_RollNextJumpSelection(SCONTEXT_PARAM);

// Makes the jump pool update on the passed context after a KMC transition. Returns true if the number of jumps has changed
bool_t KMC_UpdateJumpPool(SCONTEXT_PARAM);

// Makes the jump pool update on the passed context after a MMC transition
void MMC_UpdateJumpPool(SCONTEXT_PARAM);

