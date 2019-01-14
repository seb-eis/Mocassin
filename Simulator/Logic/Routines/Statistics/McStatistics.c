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

error_t CalcCycleCounterDefaultStatus(__SCONTEXT_PAR, CycleCounterState_t* counters)
{
    if (counters == NULL)
    {
        return ERR_NULLPOINTER;
    }

    setBufferByteValues(counters, sizeof(CycleCounterState_t), 0);
    counters->TotalGoalMcs = getDbModelJobInfo(SCONTEXT)->TargetMcsp * getNumberOfMobiles(SCONTEXT);

    int64_t rem = counters->TotalGoalMcs % CYCLE_BLOCKCOUNT;
    if (rem != 0)
    {
        counters->TotalGoalMcs += CYCLE_BLOCKCOUNT - rem;
    }

    counters->CyclesPerBlock = CYCLE_BLOCKSIZE_MIN;
    counters->McsPerBlock = counters->TotalGoalMcs / CYCLE_BLOCKCOUNT;
    return ERR_OK;
}

error_t CalcPhysicalSimulationFactors(__SCONTEXT_PAR, PhysicalInfo_t* factors)
{
    if (factors == NULL)
    {
        return ERR_NULLPOINTER;
    }

    factors->EnergyConversionFactor = CalcEnergyConversionFactor(SCONTEXT);
    factors->CurrentTimeStepping = CalcTimeStepPerJump(SCONTEXT);
    if (isfinite(CalcBasicJumpNormalization(SCONTEXT)))
    {
        factors->TotalNormalizationFactor = CalcTotalJumpNormalization(SCONTEXT);
    }
    else
    {
        factors->TotalNormalizationFactor = getDbModelJobHeaderAsKMC(SCONTEXT)->FixedNormFactor;
    }

    return ERR_OK;
}