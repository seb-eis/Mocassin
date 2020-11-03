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

#include "Libraries/Sqlite/sqlite3.h"
#include "Extensions/MocassinSolverExtension.h"
#include "Libraries/Simulator/Logic/Routines/MainRoutines.h"
#include "Libraries/Simulator/Logic/Routines/TransitionTrackingRoutines.h"

/* Routine type definitions */

// Type for storage of MMCFE routine parameters
// Layout@ggc_x86_64 => 56@[4,4,8,8,8,8,8,8]
typedef struct MmcfeParams
{
    // The size of the energy sampling histogram
    int32_t HistogramSize;

    //  The number of steps the system should divide the alpha range into
    int32_t AlphaCount;

    //  The minimum alpha value
    double  AlphaMin;

    //  The maximum alpha value
    double  AlphaMax;

    //  The current alpha value
    double  AlphaCurrent;

    //  The range of the histogram around the affiliated average energy
    double  HistogramRange;

    // The number of cycles in each relaxation phase
    int64_t RelaxPhaseCycleCount;

    //  The number of cycles in each logging phase
    int64_t LogPhaseCycleCount;

} MmcfeParams_t;

// Type for holding MMCFE runtime information
// Layout@ggc_x86_64 => 16@[8,8]
typedef struct MmcfeRunInfo
{
    // The clock when the alpha phase was started
    clock_t     PhaseStartClock;

    // The clock when the alpha phase was finished
    clock_t     PhaseEndClock;

} MmcfeRunInfo_t;

// Type for holding MMCFE log entry information
// Layout@ggc_x86_64 => 120@[24,24,16,56]
typedef struct MmcfeLog
{
    // The simulation state buffer
    Buffer_t                StateBuffer;

    // The state of the dynamic energy histogram
    DynamicJumpHistogram_t  Histogram;

    // The current run information
    MmcfeRunInfo_t          RunInfo;

    // The state of the parameter struct as checkpoint data
    MmcfeParams_t           ParamsState;

} MmcfeLog_t;


// Public routine start for MMCFE that accepts a simulation context as a void pointer
void StartMmcfeRoutine(void* context);

// Opens an sqlite3 MMCFE-Log database and ensures its existence. If the database existed, the last log entry row is provided as an out parameter
sqlite3* OpenMmcfeLogDatabase(const char* dbPath, MmcfeLog_t*restrict outLog);

// Adds an MMCFE log entry to the passed sqlite3 database connection
error_t WriteMmcfeEntryToLogDb(sqlite3* db, const MmcfeLog_t*restrict logEntry);