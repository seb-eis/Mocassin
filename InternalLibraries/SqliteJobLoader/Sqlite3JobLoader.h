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

/*
 * How to use the SqliteReader:
 * Use the "AssignDatabaseModel" to assign your DatabaseModel object. You need to provide the database location
 * and the project ID. Every other function is only for internal usage. If an error occurs at any instance the
 * operation stops and a SQLITE3 error number (always positive) or a custom error number (always negative) is returned.
 *
 * How the SqliteReader works internally:
 * When the "PopulateDbModelFromDatabase" function is called, the provided database is accessed and the required data is
 * fetched from the database by the provided Project ID. This is accomplished in three steps:
 * 1) The IDs of the parent objects are fetched from the database. The parent objects are the member variables within
 *    the database object, e.g. the Structure object.
 * 2) The member variables of the parent objects are assigned in case of simple types or BLOBs. These can be easily read
 *    from the database. In case of child objects (spans) memory is allocated.
 * 3) The member variables of the child objects are assigned. Child objects are spans within the parent objects, which
 *    have to be fetched from different sql tables than the parent objects.
 *
 * For a new parent or child objects, define a new assignment function and update the macros PARENT_OPERATIONS and/or
 * CHILD_OPARTIONS. Add the object that should be assigned and the new assignment function. Mind that within the
 * assignment function you also need to provide the SQL command to fetch the data you need.
 *
 * The functions AssignParentObjects and AssignChildObjects automatically assign the objects with the described macros.
 *
 */

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