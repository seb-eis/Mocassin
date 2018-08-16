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

#define MC_NOPOOL 0

error_t ConstructJumpPool(__SCONTEXT_PAR);

error_t PrepareJumpPool(__SCONTEXT_PAR);

void RollNextKmcSelect(__SCONTEXT_PAR);

void RollNextMmcSelect(__SCONTEXT_PAR);

bool_t GetJumpPoolUpdateKmc(__SCONTEXT_PAR);

bool_t GetJumpPoolUpdateMmc(__SCONTEXT_PAR);

