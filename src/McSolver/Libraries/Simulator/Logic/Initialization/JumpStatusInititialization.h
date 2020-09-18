//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	JumpStatusInit.h       		//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Jump status initializer     //
//////////////////////////////////////////

#pragma once

#include "Libraries/Framework/Errors/McErrors.h"
#include "Libraries/Simulator/Data/SimContext/SimulationContextAccess.h"

// Builds the jump status collection on the passed initialized simulation context
void BuildJumpStatusCollection(SCONTEXT_PARAMETER);