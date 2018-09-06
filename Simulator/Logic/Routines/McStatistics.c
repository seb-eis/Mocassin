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
#include "Simulator/Logic/Routines/McStatistics.h"

error_t CalcCycleCounterDefaultStatus(__SCONTEXT_PAR, cycle_cnt_t* counters)
{
    if (counters == NULL)
    {
        return ERR_NULLPOINTER;
    }

    Set_BufferByteValues(counters, sizeof(cycle_cnt_t), 0);
    counters->TotalGoalMcs = Get_JobInformation(SCONTEXT)->TargetMcsp * Get_LatticeInformation(SCONTEXT)->NumOfMobiles;

    int64_t rem = counters->TotalGoalMcs % CYCLE_BLOCKCOUNT;
    if (rem != 0)
    {
        counters->TotalGoalMcs += CYCLE_BLOCKCOUNT - rem;
    }

    counters->CyclesPerBlock = CYCLE_BLOCKSIZE_MIN;
    counters->McsPerBlock = counters->TotalGoalMcs / CYCLE_BLOCKCOUNT;
    return ERR_OK;
}

error_t CalcPhysicalSimulationFactors(__SCONTEXT_PAR, phys_val_t* factors)
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
        factors->TotalNormalizationFactor = Get_JobHeaderAsKmc(SCONTEXT)->FixedNormFactor;
    }

    return ERR_OK;
}