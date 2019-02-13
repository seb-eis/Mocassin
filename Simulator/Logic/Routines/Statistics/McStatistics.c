//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	McStatistics.c   		    //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   MC parameter calculations   //
//////////////////////////////////////////

#include <math.h>
#include "Simulator/Logic/Constants/Constants.h"
#include "Simulator/Logic/Routines/Statistics/McStatistics.h"
#include "Simulator/Logic/Routines/Helper/HelperRoutines.h"

error_t SetCycleCounterToDefaultStatus(SCONTEXT_PARAM, CycleCounterState_t *counters)
{
    return_if (counters == NULL, ERR_NULLPOINTER);

    counters->TotalGoalMcs = getDbModelJobInfo(SCONTEXT)->TargetMcsp * getNumberOfMobiles(SCONTEXT);

    let rem = counters->TotalGoalMcs % CYCLE_BLOCKCOUNT;
    if (rem != 0)
        counters->TotalGoalMcs += CYCLE_BLOCKCOUNT - rem;

    counters->CyclesPerBlock = CYCLE_BLOCKSIZE_MIN;
    counters->McsPerBlock = counters->TotalGoalMcs / CYCLE_BLOCKCOUNT;
    return ERR_OK;
}

error_t SetPhysicalSimulationFactorsToDefault(SCONTEXT_PARAM, PhysicalInfo_t *factors)
{
    return_if (factors == NULL, ERR_NULLPOINTER);

    factors->EnergyConversionFactor = CalcEnergyConversionFactor(SCONTEXT);
    factors->InverseEnergyConversionFactor = 1.0 / factors->EnergyConversionFactor;

    return_if (!JobInfoFlagsAreSet(SCONTEXT, INFO_FLG_KMC), ERR_OK);

    if (isfinite(CalcBasicJumpNormalization(SCONTEXT)))
        factors->TotalNormalizationFactor = CalcTotalJumpNormalization(SCONTEXT);
    else
        factors->TotalNormalizationFactor = getDbModelJobHeaderAsKMC(SCONTEXT)->FixedNormFactor;

    factors->CurrentTimeStepping = CalcTimeStepPerJump(SCONTEXT);
    return ERR_OK;
}