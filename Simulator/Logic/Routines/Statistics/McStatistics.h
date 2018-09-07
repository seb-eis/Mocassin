//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	McStatistics.h   		    //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   MC parameter calculations   //
//////////////////////////////////////////

#pragma once
#include "Framework/Errors/McErrors.h"
#include "Simulator/Logic/Constants/Constants.h"
#include "Framework/Basic/BaseTypes/BaseTypes.h"
#include "Simulator/Data/Model/SimContext/ContextAccess.h"
#include "Simulator/Data/Model/SimContext/SimContext.h"

static inline double CalcTimeStepPerJump(__SCONTEXT_PAR)
{
    return 1.0 / (getJobHeaderAsKmc(SCONTEXT)->BaseFrequency * (double) getJumpSelectionPool(SCONTEXT)->NumOfSelectableJumps);
}

static inline double CalcBasicJumpNormalization(__SCONTEXT_PAR)
{
    return 1.0 / getMainStateMetaInfo(SCONTEXT)->Data->MaxJumpProbability;
}

static inline double CalcTotalJumpNormalization(__SCONTEXT_PAR)
{
    return CalcBasicJumpNormalization(SCONTEXT) * getJobHeaderAsKmc(SCONTEXT)->FixedNormFactor;
}

static inline double CalcEnergyConversionFactor(__SCONTEXT_PAR)
{
    return 1.0 / (getJobInformation(SCONTEXT)->Temperature * NATCONST_BLOTZMANN);
}

error_t CalcCycleCounterDefaultStatus(__SCONTEXT_PAR, cycle_cnt_t* counters);

error_t CalcPhysicalSimulationFactors(__SCONTEXT_PAR, phys_val_t* factors);