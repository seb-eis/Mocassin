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
#include "Simulator/Data/Model/SimContext/ContextAccess.h"

/* Initializer routines*/

error_t HandleEnvStatePoolRegistration(__SCONTEXT_PAR, const int32_t envId);

/* Simulation routines */

void RollNextKmcSelect(__SCONTEXT_PAR);

void RollNextMmcSelect(__SCONTEXT_PAR);

bool_t MakeJumpPoolUpdateKmc(__SCONTEXT_PAR);

bool_t MakeJumpPoolUpdateMmc(__SCONTEXT_PAR);

