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
error_t HandleEnvStatePoolRegistration(__SCONTEXT_PAR, int32_t environmentId);

/* Simulation routines */

// Rolls the next jump selection data for a KMC simulation on the passed context
void RollNextKmcSelect(__SCONTEXT_PAR);

// Rolls the next jump selection data for an MMC simulation on the passed context
void RollNextMmcSelect(__SCONTEXT_PAR);

// Makes the jump pool update on the passed context after a KMC transition. Returns true if the number of jumps has changed
bool_t MakeJumpPoolUpdateKmc(__SCONTEXT_PAR);

// Makes the jump pool update on the passed context after a MMC transition. Returns true if the number of jumps has changed
bool_t MakeJumpPoolUpdateMmc(__SCONTEXT_PAR);

