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

error_t ConstructJumpPool(_SCTPARAM);

error_t PrepareJumpPool(_SCTPARAM);

void RollNextKmcSelect(_SCTPARAM);

void RollNextMmcSelect(_SCTPARAM);

bool_t GetJumpPoolUpdateKmc(_SCTPARAM);

bool_t GetJumpPoolUpdateMmc(_SCTPARAM);

