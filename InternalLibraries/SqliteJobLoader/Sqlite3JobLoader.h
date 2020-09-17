//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	SqliteReader.h              //
// Author:	John Arnold 			    //
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 John Arnold          //
// Short:   Db sqlite reader interface  //
//////////////////////////////////////////

#pragma once
#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <stdint.h>
#include "Simulator/Data/Database/DbModel.h"
#include "ExternalLibraries/sqlite3.h"
#include "InternalLibraries/Interfaces/JobLoader.h"

// Macro that call finalize on a sql statement and returns an error code if the condition is true
#define SQLFinalizeAndReturnIf(COND, STMT, ERR) if (COND) return (sqlite3_finalize(STMT), ERR)

// Macro that closes the database file and returns the close error code
#define SQLCloseAndReturnIf(COND, DB) if (COND) return sqlite3_close(DB)

// Function pointer for database load operations that get a query, database and target database model
typedef error_t (*FDbModelLoad_t)(char* sqlQuery, sqlite3* db, DbModel_t* dbModel);

// Function pointer for operations that should be performed on the loaded model after successful loading from the database
typedef error_t (*FDbOnModelLoaded_t)(DbModel_t* dbModel);

// List for model load operation function pointers
// Layout@ggc_x86_64 => 24@[8,8,8]
typedef Span_t(FDbModelLoad_t, DbModelLoadOperations) DbModelLoadOperations_t;

// List for model after model loading operations
// Layout@ggc_x86_64 => 24@[8,8,8]
typedef Span_t(FDbOnModelLoaded_t, DbModelOnLoadedOperations) DbModelOnLoadedOperations_t;

// Get an access struct for the set of parent object load operations
DbModelLoadOperations_t GetParentObjectLoadOperations();

// Get an access struct for the set of child object load operations
DbModelLoadOperations_t GetChildObjectLoadOperations();

// Get an access struct for the set of after loading operations
DbModelOnLoadedOperations_t GetDataLoadedPostOperations();

// Main function - assign the provided DbModel object with the provided database and job context id
error_t PopulateDbModelFromDatabaseFilePath(DbModel_t *dbModel, const char *dbFile, int32_t jobContextId);