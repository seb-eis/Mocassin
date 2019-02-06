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

/*
 * How to use the SqliteReader:
 * Use the "AssignDatabaseModel" to assign your DatabaseModel object. You need to provide the database location
 * and the project ID. Every other function is only for internal usage. If an error occurs at any instance the
 * operation stops and a SQLITE3 error number (always positive) or a custom error number (always negative) is returned.
 *
 * How the SqliteReader works internally:
 * When the "AssignDatabaseModel" function is called, the provided database is accessed and the required data is
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

// Type for the database load instruction index set
// Layout@ggc_x86_64 => 24@[4,4,4,4,4,4]
typedef struct DbLoadIndices
{
    // The context id of the job model to load
    int32_t JobContextId;

    // The context id of the package model to load
    int32_t PackageContextId;

    // The context id of the structure model to load
    int32_t StructureContextId;

    // The context id of the energy model to load
    int32_t EnergyContextId;

    // The context id of the transition model to load
    int32_t TransitionContextId;

    // The context id of the lattice model to load
    int32_t LatticeContextId;

} DbLoadIndices_t;


// Type for object operation pair types
// Layout@ggc_x86_64 => 24@[4,4,4,4,4,4]
typedef struct ObjectOperationPair
{
    // The pointer to the object
    void *Object;

    //
    int32_t (*Operation)(char*, sqlite3*, void*, const DbLoadIndices_t*);

} ObjectOperationPair_t;

// Type for object operation pair spans
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(ObjectOperationPair_t, ObjectOperationSet) ObjectOperationSet_t;

// Get an access struct for the set of parent object operations
ObjectOperationSet_t GetParentOperationSet(DbModel_t* dbModel);

// Get an access struct for the set of child object operations
ObjectOperationSet_t GetChildOperationSet(DbModel_t* dbModel);

// Main function - assign the provided DbModel object with the provided database and job context id
int32_t AssignDatabaseModel(DbModel_t* dbModel, const char* dbFile, int32_t jobContextId);