//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	ContextInitializer.h   		//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Context initializer logic   //
//////////////////////////////////////////

#pragma once
#include "Framework/Basic/BaseTypes/BaseTypes.h"
#include "Simulator/Data/Model/DbModel/DbModel.h"
#include "Simulator/Data/Model/SimContext/SimContext.h"

void ConstructSimulationContext(__SCONTEXT_PAR);

void PopulateSimulationContext(__SCONTEXT_PAR);