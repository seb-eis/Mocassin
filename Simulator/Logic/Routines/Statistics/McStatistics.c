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

error_t SetCycleCounterStateToDefault(SCONTEXT_PARAM, CycleCounterState_t *counters)
{
    return_if (counters == NULL, ERR_NULLPOINTER);

    let jobInfo = getDbModelJobInfo(SCONTEXT);
    let mobileCount = getNumberOfMobiles(SCONTEXT);

    counters->TotalSimulationGoalMcs = jobInfo->TargetMcsp * mobileCount;
    let rem = counters->TotalSimulationGoalMcs % CYCLE_BLOCKCOUNT;
    if (rem != 0) counters->TotalSimulationGoalMcs += CYCLE_BLOCKCOUNT - rem;
    counters->McsPerExecutionPhase = counters->TotalSimulationGoalMcs / CYCLE_BLOCKCOUNT;

    counters->CyclesPerExecutionLoop = counters->McsPerExecutionPhase * CYCLE_BLOCKSIZE_MUL;
    counters->CyclesPerExecutionLoop = getMinOfTwo(counters->CyclesPerExecutionLoop, CYCLE_BLOCKSIZE_MAX);
    counters->CyclesPerExecutionLoop = getMinOfTwo(counters->CyclesPerExecutionLoop, CYCLE_BLOCKSIZE_MIN);

    return ERR_OK;
}

error_t SetPhysicalSimulationFactorsToDefault(SCONTEXT_PARAM, PhysicalInfo_t *factors)
{
    return_if (factors == NULL, ERR_NULLPOINTER);

    factors->EnergyFactorEvToKt = GetEnergyFactorEvToKt(SCONTEXT);
    factors->EnergyFactorKtToEv = 1.0 / factors->EnergyFactorEvToKt;

    return_if (!JobInfoFlagsAreSet(SCONTEXT, INFO_FLG_KMC), ERR_OK);

    let jobHeader = getDbModelJobHeaderAsKMC(SCONTEXT);
    if (isfinite(GetBasicJumpNormalization(SCONTEXT)))
        factors->TotalJumpNormalization = GetTotalJumpNormalization(SCONTEXT);
    else
        factors->TotalJumpNormalization = jobHeader->FixedNormalizationFactor;

    factors->TimeStepPerJumpAttempt = GetCurrentTimeStepPerJumpAttempt(SCONTEXT);
    return ERR_OK;
}