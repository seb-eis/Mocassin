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

// Macros for sql query fetching from database
#define ID_POS_IN_SQLSTMT 1

// Macro the return the function in case something goes wrong
#define check_Sql(X, Y) if (X != Y) { return X; }

// Macro that call finalize on a sql statement and returns the error code if the condition is true
#define sql_FinalizeAndReturnIf(COND, STMT) if (COND) { return sqlite3_finalize(STMT);  }

// Macro that closes the database file and returns
#define sql_DbCloseAndReturnIf(COND, DB) if (COND) { return sqlite3_close(DB);  }

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

// Get an access struct for the set of parent object operations
DbModelLoadOperations_t GetParentLoadOperations();

// Get an access struct for the set of child object operations
DbModelLoadOperations_t GetChildLoadOperations();

// Get an access struct for the set of after loading operations
DbModelOnLoadedOperations_t GetOnLoadedOperations();

// Main function - assign the provided DbModel object with the provided database and job context id
error_t PopulateDbModelFromDatabase(DbModel_t *dbModel, const char *dbFile, int32_t jobContextId);