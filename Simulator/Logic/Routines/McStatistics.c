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
    counters->TotTargetMcs = Get_JobInformation(SCONTEXT)->TargetMcsp * Get_LatticeInformation(SCONTEXT)->MobilesCount;

    int64_t rem = counters->TotTargetMcs % CYCLE_BLOCKCOUNT;
    if (rem != 0)
    {
        counters->TotTargetMcs += CYCLE_BLOCKCOUNT - rem;
    }

    counters->MinCyclesPerBlock = CYCLE_BLOCKSIZE_MIN;
    counters->McsPerBlock = counters->TotTargetMcs / CYCLE_BLOCKCOUNT;
    return ERR_OK;
}

error_t CalcPhysicalSimulationFactors(__SCONTEXT_PAR, phys_val_t* factors)
{
    if (factors == NULL)
    {
        return ERR_NULLPOINTER;
    }

    factors->EngConvFac = CalcEnergyConversionFactor(SCONTEXT);
    factors->CurTimeStep = CalcTimeStepPerJump(SCONTEXT);
    if (isfinite(CalcBasicJumpNormalization(SCONTEXT)))
    {
        factors->TotJumpNorm = CalcTotalJumpNormalization(SCONTEXT);
    }
    else
    {
        factors->TotJumpNorm = Get_JobHeaderAsKmc(SCONTEXT)->FixNormFac;
    }

    return ERR_OK;
}