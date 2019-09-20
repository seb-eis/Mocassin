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
#include <math.h>

#define MMCFE_RELAXBUFFER_SIZE  100000
#define MMCFE_LOGTABLE_NAME     "LogEntries"
#define MMCFE_LATTICECOL_NAME   "Lattice"
#define MMCFE_PARAMSCOL_NAME    "ParamState"
#define MMCFE_HISTOCOL_NAME     "Histogram"
#define MMCFE_TIMECOL_NAME      "TimeStamp"
#define MMCFE_ALPHACOL_NAME     "Alpha"

/* Extension interface implementation */

const moc_uuid_t* MocExtRoutine_GetUUID()
{
    const static moc_uuid_t mmcfeGuid = {.A = 0xb7f2dded, .B =0xdaf1, .C =0x40c0, .D = {0xa1, 0xa4, 0xef, 0x9b, 0x85, 0x35, 0x6a, 0xf8}};
    return &mmcfeGuid;
}

FMocExtEntry_t MocExtRoutine_GetEntryPoint()
{
    return MMCFE_StartRoutine;
}

/* Internal routine implementation */

// Builds the default log database file path using the provided simulation context (Has to be freed manually)
static const char* BuildDefaultLogDbFilePath(SCONTEXT_PARAM)
{
    let ioPath = getFileInformation(SCONTEXT)->IODirectoryPath;
    let fileName = "/mmcfelog.db";
    char* result;
    var error = ConcatStrings(ioPath, fileName, &result);
    error_assert(error, "Fatal error on building the log database file path.");
    return result;
}

// Tries to get the last routine log entry parameter state from the database
static error_t TryGetLastDbLogEntry(sqlite3* db, MmcfeLog_t*restrict outLog)
{
    debug_assert(outLog != NULL);
    let sqlQuery = "SELECT TOP 1 Id, " MMCFE_PARAMSCOL_NAME " FROM " MMCFE_LOGTABLE_NAME " ORDER BY Id ASC";
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
    let createQuery = "CREATE TABLE "MMCFE_LOGTABLE_NAME" ("
                      "Id INTEGER PRIMARY KEY, "
                      MMCFE_TIMECOL_NAME    " TEXT NOT NULL, "
                      MMCFE_LATTICECOL_NAME " BLOB NOT NULL, "
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
        error_assert(ERR_DATABASE, "Fatal error while trying to create the MMCFE log database connection.");
    }

    var error = EnsureLogDbCreated(db, outLog);
    error_assert(error, "Fatal error while creating or loading the log database.");
    return db;
}

error_t MMCFE_WriteEntryToLogDb(sqlite3* db, const MmcfeLog_t*restrict logEntry)
{
    let sqlQuery = "INSERT INTO" MMCFE_LOGTABLE_NAME "("
                   MMCFE_TIMECOL_NAME       ", "
                   MMCFE_LATTICECOL_NAME    ", "
                   MMCFE_HISTOCOL_NAME      ", "
                   MMCFE_PARAMSCOL_NAME     ", "
                   MMCFE_ALPHACOL_NAME      ") "
                   "VALUES (?1, ?2, ?3, ?4, ?5)";

    sqlite3_stmt* sqlStmt = NULL;
    var error = sqlite3_prepare_v2(db, sqlQuery, -1, &sqlStmt, NULL);
    return_if(error != SQLITE_OK, (sqlite3_finalize(sqlStmt), ERR_DATABASE));

    char timeStamp[TIME_ISO8601_BYTECOUNT];
    GetCurrentTimeStampISO8601UTC(timeStamp);
    error = sqlite3_bind_text(sqlStmt, 1, timeStamp, -1, NULL);
    return_if(error != SQLITE_OK, (sqlite3_finalize(sqlStmt), ERR_DATABASE));

    let latticeByteCount = span_ByteCount(logEntry->Lattice);
    return_if(latticeByteCount > INT32_MAX, ERR_DATABASE);

    error = sqlite3_bind_blob(sqlStmt, 2, logEntry->Lattice.Begin, latticeByteCount, NULL);
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
static bool_t RoutineParametersAreValid(MmcfeParams_t* restrict params)
{
    return_if(params->AlphaCount <= 0, false);
    return_if(params->AlphaMax <= params->AlphaMin || params->AlphaMax > 1, false);
    return_if(params->HistogramRange <= 0.0 || params->HistogramSize <= 0, false);
    return_if(params->RelaxPhaseCycleCount < 0, false);
    return_if(params->LogPhaseCycleCount < 0, false);
    return true;
}

// Verifies that the MMCFE routine parameter data exists in the context and the right UUID is set
static error_t TryLoadRoutineParameters(SCONTEXT_PARAM, MmcfeParams_t*restrict outParams)
{
    let routineData = getCustomRoutineData(SCONTEXT);
    return_if(CompareUUID(&routineData->Guid, MocExtRoutine_GetUUID()) != 0, ERR_DATACONSISTENCY);
    return_if(span_Length(routineData->ParamData) != sizeof(MmcfeParams_t), ERR_DATACONSISTENCY);

    return_if(!RoutineParametersAreValid((MmcfeParams_t*) routineData->ParamData.Begin), ERR_DATACONSISTENCY);

    *outParams = *(MmcfeParams_t*) routineData->ParamData.Begin;
    return ERR_OK;
}

// Initializes the routine log after the parameter state was loaded from either log-db or simulation database
static error_t InitializeRoutineLog(SCONTEXT_PARAM, MmcfeLog_t*restrict log)
{
    debug_assert(log != NULL);

    log->Lattice = *getMainStateLattice(SCONTEXT);
    log->Histogram = ctor_DynamicJumpHistogram(log->ParamsState.HistogramSize);
    return ERR_OK;
}

// Executes a single simulation cycle of the MMCFE Routine
static inline void ExecuteSimulationCycle(SCONTEXT_PARAM, const MmcfeLog_t*restrict log)
{
    MMC_ExecuteSimulationCycle_WithAlpha(SCONTEXT, log->ParamsState.AlphaCurrent);
}

// Logs a cycle outcome of the MMCFE routine to the routine log
static inline void LogCycleOutcome(SCONTEXT_PARAM, MmcfeLog_t*restrict log, const double energy)
{
    AddEnergyValueToDynamicJumpHistogram(&log->Histogram, energy);
}

// Finishes one logging phase of the MMCFE routine
static inline void FinishLoggingPhase(SCONTEXT_PARAM, MmcfeLog_t*restrict log, sqlite3*restrict db)
{
    MMC_UpdateAndCheckAbortConditions(SCONTEXT);
    MC_DoCommonPhaseFinish(SCONTEXT);
    MMCFE_WriteEntryToLogDb(db, log);
}

// Prepares the MMCFE log for the next execution phase using the provide energy buffer information
static inline void PrepareLogForNextLoggingPhase(SCONTEXT_PARAM, MmcfeLog_t*restrict log, const Flp64Buffer_t*restrict engBuffer)
{
    let physFactors = getPhysicalFactors(SCONTEXT);
    var avgEnergy = 0.0;
    cpp_foreach(item, *engBuffer) avgEnergy+= *item;
    avgEnergy /= (double) list_Capacity(*engBuffer);
    avgEnergy *= physFactors->EnergyFactorKtToEv;

    ChangeDynamicJumpHistogramSamplingAreaByRange(&log->Histogram, avgEnergy, log->ParamsState.HistogramRange);
}

// Enters the relaxation phase of the MMCFE that will prepare the lattice and the histogram system fro the next logging phase
static inline void EnterRelaxationPhase(SCONTEXT_PARAM, MmcfeLog_t*restrict log)
{
    let latticeEnergy = &getMainStateMetaData(SCONTEXT)->LatticeEnergy;
    Flp64Buffer_t energyBuffer = new_List(energyBuffer, getMaxOfTwo(MMCFE_RELAXBUFFER_SIZE, log->ParamsState.RelaxPhaseCycleCount));

    for (int64_t i = 0; i < log->ParamsState.RelaxPhaseCycleCount; i++)
    {
        ExecuteSimulationCycle(SCONTEXT, log);
        list_PushBack(energyBuffer, *latticeEnergy);
        if (list_IsFull(energyBuffer)) list_Clear(energyBuffer);
    }

    PrepareLogForNextLoggingPhase(SCONTEXT, log, &energyBuffer);
    delete_List(energyBuffer);
}

// Enters the logging phase of the MMCFE that produces the histogram data
static inline void EnterLoggingPhase(SCONTEXT_PARAM, MmcfeLog_t*restrict log, sqlite3*restrict db)
{
    let latticeEnergy = &getMainStateMetaData(SCONTEXT)->LatticeEnergy;

    for (int64_t i = 0; i < log->ParamsState.LogPhaseCycleCount; i++)
    {
        ExecuteSimulationCycle(SCONTEXT, log);
        LogCycleOutcome(SCONTEXT, log, *latticeEnergy);
    }
}

// Enters the actual outer MMCFE routine execution phase that performs the simulation with the provided routine log and database
static error_t EnterExecutionLoop(SCONTEXT_PARAM, MmcfeLog_t*restrict log, sqlite3*restrict db)
{
    let alphaStep = (log->ParamsState.AlphaMax - log->ParamsState.AlphaMin) / log->ParamsState.AlphaCount;

    for (;log->ParamsState.AlphaCurrent <= log->ParamsState.AlphaMax;)
    {
        EnterRelaxationPhase(SCONTEXT, log);
        EnterLoggingPhase(SCONTEXT, log, db);
        log->ParamsState.AlphaCurrent += alphaStep;
    }
}

// The internal MMCFE routine entry point
static error_t StartRoutineInternal(SCONTEXT_PARAM)
{
    var error = ERR_OK;
    MmcfeLog_t routineLog;
    nullStructContent(routineLog);

    let dbPath = BuildDefaultLogDbFilePath(SCONTEXT);
    var db = MMCFE_OpenLogDb(dbPath, &routineLog);

    if (!RoutineParametersAreValid(&routineLog.ParamsState))
    {
        error = TryLoadRoutineParameters(SCONTEXT, &routineLog.ParamsState);
        return_if(error, error);
    }

    error = InitializeRoutineLog(SCONTEXT, &routineLog);
    return_if(error, error);

    error = EnterExecutionLoop(SCONTEXT, &routineLog, db);
    return error;
}

void MMCFE_StartRoutine(void* context)
{
    let error = StartRoutineInternal((SimulationContext_t *) context);
    error_assert(error, "Unhandled internal error in MMCFE execution routine.");
}
