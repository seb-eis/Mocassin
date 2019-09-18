//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	MmcFERoutine.h        		//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   MMC free energy routine     //
//////////////////////////////////////////

#pragma once
#include "Simulator/Logic/Routines/Main/MainRoutines.h"

// Type for storage of parameters for the MMC free energy routine
// Layout@ggc_x86_64 => 56@[8,8,8,8,8,8,8]
typedef struct MMCFERoutineParams
{
    int32_t HistogramSize;

    int32_t AlphaCount;

    double  AlphaMin;

    double  AlphaMax;

    double  HistogramRange;

    int64_t RelaxPhaseCycleCount;

    int64_t LogPhaseCycleCount;

} MMCFERoutineParams_t;

error_t MMCFE_StartMainRoutine(SCONTEXT_PARAM, MMCFERoutineParams_t* restrict parameter);

// The
error_t MMCFE_EnterExecutionPhase(SCONTEXT_PARAM, MMCFERoutineParams_t* restrict parameter);

/* MMCFE simulation non-error sub-routines */

// Execute a single cycle of the MMCFE routine
void MMCFE_ExcecuteSimulationCycle(SCONTEXT_PARAM);

// Logs the result of a single simulation cycle of the MMCFE routine
void MMCFE_LogSimulationCycleResult(SCONTEXT_PARAM);