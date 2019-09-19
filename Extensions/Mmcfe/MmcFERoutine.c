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

/* Extension interface implementation */

const char* MocExtRoutine_GetUUID()
{
    const char* uuid = "b7f2dded-daf1-40c0-a1a4-ef9b85356af8";
    return uuid;
}

FMocExtEntry_t MocExtRoutine_GetEntryPoint()
{
    return MMCFE_StartRoutine;
}

/* Internal routine implementation */

static bool_t RoutineParametersAreValid(MmcfeParams_t* restrict params)
{
    return_if(params->AlphaCount <= 0, false);
    return_if(params->AlphaMax <= params->AlphaMin, false);
    return_if(params->HistogramRange <= 0.0 || params->HistogramSize <= 0, false);
    return_if(params->RelaxPhaseCycleCount < 0, false);
    return_if(params->LogPhaseCycleCount < 0, false);
    return true;
}

// Verifies that the MMCFE routine parameter data exists in the context and the right UUID is set
static error_t TryLoadRoutineParameters(SCONTEXT_PARAM, MmcfeParams_t*restrict outParams)
{
    let routineData = getCustomRoutineData(SCONTEXT);
    return_if(strcmp(routineData->Guid, MocExtRoutine_GetUUID()) != 0, ERR_DATACONSISTENCY);
    return_if(span_Length(routineData->ParamData) != sizeof(MmcfeParams_t), ERR_DATACONSISTENCY);

    return_if(!RoutineParametersAreValid((MmcfeParams_t*) routineData->ParamData.Begin), ERR_DATACONSISTENCY);

    *outParams = *(MmcfeParams_t*) routineData->ParamData.Begin;
    return ERR_OK;
}

// Tries to lod a checkpoint from an already existing routine result database
static error_t TryLoadRoutineCheckpoint(SCONTEXT_PARAM, MmcfeParams_t*restrict params)
{

}

// Enters the actual outer MMCFE routine execution phase that performs the simulation
static error_t MMCFE_EnterExecutionPhase(SCONTEXT_PARAM, MmcfeParams_t*restrict params)
{

}

// The internal MMCFE routine entry point
static error_t MMCFE_InternalStartRoutine(SCONTEXT_PARAM)
{
    MmcfeParams_t routineParams;
    var error = TryLoadRoutineParameters(SCONTEXT, &routineParams);
    error_assert(error, "MMCFE routine parameters are invalid or missing.");

    error = TryLoadRoutineCheckpoint(SCONTEXT, &routineParams);
    error_assert(error, "MMCFE routine checkpoint loading caused an unexpected error.");

    error = MMCFE_EnterExecutionPhase(SCONTEXT, &routineParams);
    return error;
}

// Ensures that the log database is actually created an usable
static error_t MMCFE_EnsureLogDbCreated(sqlite3* db)
{
    let createQuery = "CREATE TABLE LogEntries (Id int NOT NULL AUTO_INCREMENT, TimeStamp TINYTEXT, Lattice MEDIUMBLOB, Histogram MEDIUMBLOB, ParamState TINYBLOB)";
    return ERR_NOTIMPLEMENTED;
}

// Opens an sqlite3 MMCFE-Log database. The method ensures that the database is created if it doesnt exist
sqlite3* MMCFE_OpenLogDb(const char* dbPath)
{
    sqlite3* dbContext;
    if (sqlite3_open(dbPath, &dbContext) != SQLITE_OK)
    {
        sqlite3_close(dbContext);
    }

    return NULL;
}

error_t MMCFE_AddLogEntryToDatabase(sqlite3* db, const MmcfeLog_t*restrict logEntry)
{
    let sqlQuery = "insert into LogEntries (TimeStamp, Lattice, Histogram, ParamState) values (?1, ?2, ?3, ?4)";
    sqlite3_stmt* sqlStmt = NULL;
    var error = sqlite3_prepare_v2(db, sqlQuery, -1, &sqlStmt, NULL);
    return_if(error != SQLITE_OK, (sqlite3_finalize(sqlStmt), ERR_DATABASE));

    char timeStamp[TIME_ISO8601_BYTECOUNT];
    GetCurrentTimeStampISO8601UTC(timeStamp);
    error = sqlite3_bind_text(sqlStmt, 1, timeStamp, -1, NULL);
    return_if(error != SQLITE_OK, (sqlite3_finalize(sqlStmt), ERR_DATABASE));

    let latticeByteCount = array_ByteCount(logEntry->Lattice);
    return_if(latticeByteCount > INT32_MAX, ERR_DATABASE);

    error = sqlite3_bind_blob(sqlStmt, 2, logEntry->Lattice.Header, latticeByteCount, NULL);
    return_if(error != SQLITE_OK, (sqlite3_finalize(sqlStmt), ERR_DATABASE));

    let histogramByteCount = (void*) logEntry->Histogram.Counters.End - (void*) logEntry->Histogram.Header;
    return_if(histogramByteCount > INT32_MAX, ERR_DATABASE);

    error = sqlite3_bind_blob(sqlStmt, 3, logEntry->Histogram.Header, histogramByteCount, NULL);
    return_if(error != SQLITE_OK, (sqlite3_finalize(sqlStmt), ERR_DATABASE));

    error = sqlite3_bind_blob(sqlStmt, 4, &logEntry->ParamsState, sizeof(MmcfeParams_t), NULL);
    return_if(error != SQLITE_OK, (sqlite3_finalize(sqlStmt), ERR_DATABASE));

    error = sqlite3_step(sqlStmt);
    return_if(error != SQLITE_DONE, (sqlite3_finalize(sqlStmt), ERR_DATABASE));

    sqlite3_finalize(sqlStmt);
    return ERR_OK;
}

void MMCFE_StartRoutine(void* context)
{
    let error = MMCFE_InternalStartRoutine((SimulationContext_t*) context);
    error_assert(error, "Unhandled internal error in MMCFE execution routine.");
}
