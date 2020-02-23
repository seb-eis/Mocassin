//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	MmcFERoutine.c        		//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   MMC free energy routine     //
//////////////////////////////////////////

#include "Extensions/Mmcfe/MmcFERoutine.h"
#include "InternalLibraries/Interfaces/ProgressPrint.h"
#include "Framework/Math/Random/Approx.h"
#include <math.h>

#define MMCFE_RELAXBUFFER_SIZE  100000
#define MMCFE_LOGTABLE_NAME     "LogEntries"
#define MMCFE_STATECOL_NAME     "State"
#define MMCFE_PARAMSCOL_NAME    "ParamState"
#define MMCFE_HISTOCOL_NAME     "Histogram"
#define MMCFE_TIMECOL_NAME      "TimeStamp"
#define MMCFE_ALPHACOL_NAME     "Alpha"

/* Extension interface implementation */

const mocuuid_t* MOCEXTENSION_GET_IDENTIFICATION_FUNC()
{
    const static mocuuid_t routineGuid = {.A = 0xb7f2dded, .B =0xdaf1, .C =0x40c0, .D = {0x4d, 0x4d, 0x43, 0x46, 0x45, 0x00, 0x00, 0x00}};
    return &routineGuid;
}

FMocassinRoutine_t MOCEXTENSION_GET_ROUTINE_FUNC()
{
    return StartMmcfeRoutine;
}

/* Internal routine implementation */

// Builds the default log database file path using the provided simulation context (Has to be freed manually)
static const char* BuildDefaultLogDbFilePath(SCONTEXT_PARAMETER)
{
    let ioPath = getFileInformation(simContext)->IODirectoryPath;
    let fileName = "/mmcfelog.db";
    char* result;
    var error = ConcatStrings(ioPath, fileName, &result);
    assert_success(error, "Fatal error on building the log database file path.");
    return result;
}

// Tries to get the last routine log entry parameter state from the database
static error_t TryGetLastDbLogEntry(sqlite3* db, MmcfeLog_t*restrict outLog)
{
    debug_assert(outLog != NULL);
    let sqlQuery = "SELECT Id, " MMCFE_PARAMSCOL_NAME " FROM " MMCFE_LOGTABLE_NAME " ORDER BY Id DESC";
    sqlite3_stmt* sqlStmt;

    var error = sqlite3_prepare_v2(db, sqlQuery, -1,&sqlStmt,NULL);
    return_if(error != SQLITE_OK, (sqlite3_finalize(sqlStmt), ERR_USEDEFAULT));

    error = sqlite3_step(sqlStmt);
    return_if(error != SQLITE_ROW, (sqlite3_finalize(sqlStmt), ERR_USEDEFAULT));

    outLog->ParamsState = *(MmcfeParams_t*) sqlite3_column_blob(sqlStmt, 1);

    error= sqlite3_finalize(sqlStmt);
    return error == SQLITE_OK ? ERR_OK : ERR_DATABASE;
}

// Ensures that the log database is actually created an usable, if the database already existed it returns the last log entry
static error_t EnsureLogDbCreated(sqlite3* db, MmcfeLog_t*restrict outLog)
{
    let createQuery = "CREATE TABLE IF NOT EXISTS "MMCFE_LOGTABLE_NAME" ("
                      "Id INTEGER PRIMARY KEY, "
                      MMCFE_TIMECOL_NAME    " TEXT NOT NULL, "
                      MMCFE_STATECOL_NAME   " BLOB NOT NULL, "
                      MMCFE_HISTOCOL_NAME   " BLOB NOT NULL, "
                      MMCFE_PARAMSCOL_NAME  " BLOB NOT NULL, "
                      MMCFE_ALPHACOL_NAME   " REAL NOT NULL);";

    var error = TryGetLastDbLogEntry(db, outLog);
    return_if(error != ERR_USEDEFAULT, error);

    sqlite3_stmt* sqlStmt;

    error = sqlite3_prepare_v2(db, createQuery, -1, &sqlStmt, NULL);
    return_if(error != SQLITE_OK, (sqlite3_finalize(sqlStmt), ERR_DATABASE));

    error = sqlite3_step(sqlStmt);
    return_if(error != SQLITE_DONE, (sqlite3_finalize(sqlStmt), ERR_DATABASE));

    error = sqlite3_finalize(sqlStmt);

    return error == SQLITE_OK ? ERR_OK : ERR_DATABASE;
}

// Opens an sqlite3 MMCFE-Log database and ensures its existance. If the database existed, the last log entry row is provided as an out parameter
sqlite3* MMCFE_OpenLogDb(const char* dbPath, MmcfeLog_t*restrict outLog)
{
    debug_assert(outLog != NULL);

    sqlite3* db;
    if (sqlite3_open(dbPath, &db) != SQLITE_OK)
    {
        sqlite3_close(db);
        assert_success(ERR_DATABASE, "Fatal error while trying to create the MMCFE log database connection.");
    }

    var error = EnsureLogDbCreated(db, outLog);
    assert_success(error, "Fatal error while creating or loading the log database.");
    return db;
}

error_t WriteMmcfeEntryToLogDb(sqlite3* db, const MmcfeLog_t*restrict logEntry)
{
    let sqlQuery = "INSERT INTO " MMCFE_LOGTABLE_NAME " ("
                   MMCFE_TIMECOL_NAME       ", "
                   MMCFE_STATECOL_NAME    ", "
                   MMCFE_HISTOCOL_NAME      ", "
                   MMCFE_PARAMSCOL_NAME     ", "
                   MMCFE_ALPHACOL_NAME      ") "
                   "VALUES (?1, ?2, ?3, ?4, ?5)";

    sqlite3_stmt* sqlStmt = NULL;
    var error = sqlite3_prepare_v2(db, sqlQuery, -1, &sqlStmt, NULL);
    return_if(error != SQLITE_OK, (sqlite3_finalize(sqlStmt), ERR_DATABASE));

    char timeStamp[TIME_ISO8601_BYTECOUNT];
    GetCurrentIso8601UtcTimeStamp(timeStamp);
    error = sqlite3_bind_text(sqlStmt, 1, timeStamp, -1, NULL);
    return_if(error != SQLITE_OK, (sqlite3_finalize(sqlStmt), ERR_DATABASE));

    let stateByteCount = span_ByteCount(logEntry->StateBuffer);
    return_if(stateByteCount > INT32_MAX, ERR_DATABASE);

    error = sqlite3_bind_blob(sqlStmt, 2, logEntry->StateBuffer.Begin, stateByteCount, NULL);
    return_if(error != SQLITE_OK, (sqlite3_finalize(sqlStmt), ERR_DATABASE));

    let histogramByteCount = (void*) logEntry->Histogram.Counters.End - (void*) logEntry->Histogram.Header;
    return_if(histogramByteCount > INT32_MAX, ERR_DATABASE);

    error = sqlite3_bind_blob(sqlStmt, 3, logEntry->Histogram.Header, histogramByteCount, NULL);
    return_if(error != SQLITE_OK, (sqlite3_finalize(sqlStmt), ERR_DATABASE));

    error = sqlite3_bind_blob(sqlStmt, 4, &logEntry->ParamsState, sizeof(MmcfeParams_t), NULL);
    return_if(error != SQLITE_OK, (sqlite3_finalize(sqlStmt), ERR_DATABASE));

    error = sqlite3_bind_double(sqlStmt, 5, logEntry->ParamsState.AlphaCurrent);
    return_if(error != SQLITE_OK, (sqlite3_finalize(sqlStmt), ERR_DATABASE));

    error = sqlite3_step(sqlStmt);
    return_if(error != SQLITE_DONE, (sqlite3_finalize(sqlStmt), ERR_DATABASE));

    sqlite3_finalize(sqlStmt);
    return ERR_OK;
}

// Checks if the routine parameters are valid
static inline bool_t RoutineParametersAreValid(MmcfeParams_t* restrict params)
{
    return_if(params->AlphaCount <= 0, false);
    return_if(params->AlphaMax <= params->AlphaMin || params->AlphaMax > 1, false);
    return_if(params->HistogramRange <= 0.0 || params->HistogramSize <= 0, false);
    return_if(params->RelaxPhaseCycleCount < 0, false);
    return_if(params->LogPhaseCycleCount < 0, false);
    return true;
}

// Checks if the routine has already completed
static inline bool_t RoutineIsAlreadyCompleted(MmcfeParams_t* restrict params)
{
    return (params->AlphaCurrent >= params->AlphaMax && params->LogPhaseCycleCount != 0) ? true : false;
}

// Verifies that the MMCFE routine parameter data exists in the context and the right UUID is set
static error_t TryLoadRoutineParameters(SCONTEXT_PARAMETER, MmcfeParams_t*restrict outParams)
{
    let routineData = getCustomRoutineData(simContext);

    let testGuid = MocExtRoutine_GetUUID();
    return_if(CompareMocuuid(routineData->Guid, testGuid) != 0, ERR_DATACONSISTENCY);

    let length = span_Length(routineData->ParamData);
    return_if(length != sizeof(MmcfeParams_t), ERR_DATACONSISTENCY);

    return_if(!RoutineParametersAreValid((MmcfeParams_t*) routineData->ParamData.Begin), ERR_DATACONSISTENCY);

    *outParams = *(MmcfeParams_t*) routineData->ParamData.Begin;
    return ERR_OK;
}

// Initializes the routine log after the parameter state was loaded from either log-db or simulation database
static error_t InitializeRoutineLog(SCONTEXT_PARAMETER, MmcfeLog_t*restrict log)
{
    debug_assert(log != NULL);

    log->StateBuffer = getSimulationState(simContext)->Buffer;
    log->Histogram = ctor_DynamicJumpHistogram(log->ParamsState.HistogramSize);
    return ERR_OK;
}

// Executes simulation cycles till the next log relevant event (Either accepted or rejected cycle, site blocks are skipped)
static inline void CycleSimulationTillNextLogEvent(SCONTEXT_PARAMETER, const MmcfeLog_t*restrict log, double* latticeEnergy)
{
    var counters = getMainCycleCounters(simContext);

    // Cycle till the next accepted or rejected case
    for (int32_t test = MC_BLOCKED_CYCLE; test == MC_BLOCKED_CYCLE; test = simContext->CycleResult)
    {
        ExecuteMmcSimulationCycleWithAlpha(simContext, log->ParamsState.AlphaCurrent);
    }

    // Update energy if required and count the cycle
    if (simContext->CycleResult == MC_ACCEPTED_CYCLE)
    {
        let factors = getPhysicalFactors(simContext);
        let jumpInfo = getJumpEnergyInfo(simContext);
        *latticeEnergy += factors->EnergyFactorKtToEv * jumpInfo->S0toS2DeltaEnergy;
    }
    counters->CycleCount++;
}

// Performs the log action of a cycle outcome and writes the new data to the routine log
static inline void LogCycleOutcome(SCONTEXT_PARAMETER, MmcfeLog_t*restrict log, const double latticeEnergy)
{
    AddEnergyValueToDynamicJumpHistogram(&log->Histogram, latticeEnergy);
}

// Calculates the expected average cycle rate for the remaining simulation alpha values
static double CalculateExpectedAverageCycleRate(SCONTEXT_PARAMETER, const MmcfeLog_t*restrict log)
{
    // Estimates the average cycle rate using the fact that the success rate can be expressed as f(a) = k_0(a) * f(0)*exp(k_1 * a)
    // where the factor k_1 is the logarithm of the average best-rate/worst-rate scenario in MMC (approx. 11)
    // and the factor k_0(a) is an arbitrary exponential correction for the increasing rate bias when calculating f(0)

    let meta = getMainStateMetaData(simContext);
    let logValue = 2.398;
    let frqIntegral = 4.1703;
    let frqFactor = exp(-logValue * log->ParamsState.AlphaCurrent);
    let expCorrection = exp(-log->ParamsState.AlphaCurrent);
    let baseCycleRate = meta->CycleRate * frqFactor * (1.398 + (1.0-pow(expCorrection, logValue)) * logValue);

    return frqIntegral * baseCycleRate * 1.5;
}

// Calculates an estimated time till completion of the routine using the current log data
static int64_t CalculateRuntimeEtaInSeconds(SCONTEXT_PARAMETER, const MmcfeLog_t*restrict log)
{
    let cycleCountPerBlock = log->ParamsState.RelaxPhaseCycleCount + log->ParamsState.LogPhaseCycleCount;
    let alphaStep = (log->ParamsState.AlphaMax - log->ParamsState.AlphaMin) / log->ParamsState.AlphaCount;
    let remainingAlphaCount = (int32_t) round((log->ParamsState.AlphaMax - log->ParamsState.AlphaCurrent) / alphaStep);

    let avgCycleRate = CalculateExpectedAverageCycleRate(simContext, log);
    return (cycleCountPerBlock * remainingAlphaCount) / (int64_t) avgCycleRate;
}

// Prints the progress of the MMCFE routine
static inline void PrintRoutineProgress(SCONTEXT_PARAMETER, const MmcfeLog_t*restrict log)
{
    let meta = getMainStateMetaData(simContext);
    let peakEnergy = FindDynamicJumpHistogramMaxValue(&log->Histogram);
    let tempEquiv = getDbModelJobInfo(simContext)->Temperature / log->ParamsState.AlphaCurrent;

    char stampBuffer[TIME_ISO8601_BYTECOUNT], runBuffer[TIME_ISO8601_BYTECOUNT], etaBuffer[TIME_ISO8601_BYTECOUNT];
    GetCurrentIso8601UtcTimeStamp(stampBuffer);
    SecondsToIso8601FormattedTimePeriod(runBuffer, meta->ProgramRunTime);
    let timeEta = CalculateRuntimeEtaInSeconds(simContext, log);
    SecondsToIso8601FormattedTimePeriod(etaBuffer, timeEta);

    fprintf(stdout, "MMCFE  => Logtime: %s [  ] (Runtime = %s, ETA = %s)\n", stampBuffer, runBuffer, etaBuffer);
    fprintf(stdout, "MMCFE  => Lograte: %+.6e [Hz] (Succesrate = %+.6e [Hz])\n", meta->CycleRate, meta->SuccessRate);
    fprintf(stdout, "MMCFE  => Log created for E_latt = %+.6e [eV], Alpha = %+.2e, T_eq = %.2f [K]\n\n", peakEnergy, log->ParamsState.AlphaCurrent, tempEquiv);
    fflush(stdout);
}

// Finishes one logging phase of the MMCFE routine
static inline void FinishLoggingPhase(SCONTEXT_PARAMETER, MmcfeLog_t*restrict log, sqlite3*restrict db)
{
    UpdateAndEvaluateMmcAbortConditions(simContext);
    ExecuteSharedMcBlockFinisher(simContext);
    WriteMmcfeEntryToLogDb(db, log);
    PrintRoutineProgress(simContext, log);
}

// Prepares the MMCFE log for the next execution phase using the provide energy buffer information
static inline void PrepareLogForNextLoggingPhase(SCONTEXT_PARAMETER, MmcfeLog_t*restrict log, const Flp64Buffer_t*restrict engBuffer)
{
    var avgEnergy = 0.0;
    cpp_foreach(item, *engBuffer) avgEnergy+= *item;
    avgEnergy /= (double) list_Capacity(*engBuffer);
    avgEnergy = round(avgEnergy);
    ChangeDynamicJumpHistogramSamplingAreaByRange(&log->Histogram, avgEnergy, log->ParamsState.HistogramRange);
}

// Enters the relaxation phase of the MMCFE that will prepare the lattice and the histogram system fro the next logging phase
static inline void EnterRelaxationPhase(SCONTEXT_PARAMETER, MmcfeLog_t*restrict log)
{
    let latticeEnergy = &getMainStateMetaData(simContext)->LatticeEnergy;
    let bufferSize = log->ParamsState.RelaxPhaseCycleCount > MMCFE_RELAXBUFFER_SIZE ? MMCFE_RELAXBUFFER_SIZE : log->ParamsState.RelaxPhaseCycleCount;
    Flp64Buffer_t energyBuffer = list_New(energyBuffer, bufferSize);

    for (int64_t i = 0; i < log->ParamsState.RelaxPhaseCycleCount; i++)
    {
        CycleSimulationTillNextLogEvent(simContext, log, latticeEnergy);
        list_PushBack(energyBuffer, *latticeEnergy);
        if (list_IsFull(energyBuffer)) list_Clear(energyBuffer);
    }

    energyBuffer.End = energyBuffer.CapacityEnd;
    PrepareLogForNextLoggingPhase(simContext, log, &energyBuffer);
    list_Delete(energyBuffer);
}

// Enters the logging phase of the MMCFE that produces the histogram data
static inline void EnterLoggingPhase(SCONTEXT_PARAMETER, MmcfeLog_t*restrict log)
{
    let latticeEnergy = &getMainStateMetaData(simContext)->LatticeEnergy;

    for (int64_t i = 0; i < log->ParamsState.LogPhaseCycleCount; i++)
    {
        CycleSimulationTillNextLogEvent(simContext, log, latticeEnergy);
        LogCycleOutcome(simContext, log, *latticeEnergy);
    }
}

// Enters the actual outer MMCFE routine execution phase that performs the simulation with the provided routine log and database
static error_t EnterExecutionLoop(SCONTEXT_PARAMETER, MmcfeLog_t*restrict log, sqlite3*restrict db, const bool_t logLoaded)
{
    let alphaStep = (log->ParamsState.AlphaMax - log->ParamsState.AlphaMin) / log->ParamsState.AlphaCount;
    if (logLoaded) log->ParamsState.AlphaCurrent += alphaStep;

    UpdateAndEvaluateMmcAbortConditions(simContext);

    for (;log->ParamsState.AlphaCurrent <= log->ParamsState.AlphaMax + 1.0e-6;)
    {
        EnterRelaxationPhase(simContext, log);
        EnterLoggingPhase(simContext, log);
        FinishLoggingPhase(simContext, log, db);
        log->ParamsState.AlphaCurrent += alphaStep;
    }

    return ERR_OK;
}

// The internal MMCFE routine entry point
static error_t StartRoutineInternal(SCONTEXT_PARAMETER)
{
    var error = ERR_OK;
    MmcfeLog_t routineLog;
    nullStructContent(routineLog);

    let logPath = BuildDefaultLogDbFilePath(simContext);
    var db = MMCFE_OpenLogDb(logPath, &routineLog);
    return_if(RoutineIsAlreadyCompleted(&routineLog.ParamsState), (sqlite3_close(db), ERR_ALREADYCOMPLETED));

    let logLoaded = RoutineParametersAreValid(&routineLog.ParamsState);
    if (!logLoaded)
    {
        error = TryLoadRoutineParameters(simContext, &routineLog.ParamsState);
        return_if(error, error);
    }

    error = InitializeRoutineLog(simContext, &routineLog);
    return_if(error, error);

    error = EnterExecutionLoop(simContext, &routineLog, db, logLoaded);
    return_if(error, error);

    return sqlite3_close(db) != SQLITE_OK ? ERR_DATABASE : ERR_OK;
}

void StartMmcfeRoutine(void* context)
{
    let error = StartRoutineInternal((SimulationContext_t *) context);
    assert_success(error, "Unhandled internal error in MMCFE execution routine.");
}
