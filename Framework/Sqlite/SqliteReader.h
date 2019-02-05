#ifndef ICON_SIMULATOR_SQLITEREADER_H
#define ICON_SIMULATOR_SQLITEREADER_H

#endif //ICON_SIMULATOR_SQLITEREADER_H

#include <stdio.h>
#include "ExternalLibraries/sqlite3.h"
#include <string.h>
#include <stdlib.h>
#include <stdint.h>
#include "Simulator/Data/Database/DbModel.h"

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
#define CHECK_SQL(X, Y) if (X != Y) { return X; }


struct ProjectIds{
    int ProjectId;
    int StructureId;
    int EnergyId;
    int TransitionId;
    int LatticeId;
};


struct objectOperationPair{
    void *Object;
    int (*Operation)(char*, sqlite3*, void*, const struct ProjectIds*);
};

// object operation pairs for the parent operations. provide the dbModel should be a pointer to database model.
#define PARENT_OPERATIONS(dbModel) {{.Object = &dbModel->StructureModel, .Operation = &AssignStructureModel},     \
                                    {.Object = &dbModel->EnergyModel, .Operation = &AssignEnergyModel},           \
                                    {.Object = &dbModel->TransitionModel, .Operation = &AssignTransitionModel},   \
                                    {.Object = &dbModel->LatticeModel, .Operation = &AssignLatticeModel}};

#define CHILD_OPERATIONS(dbModel) {{.Object = &dbModel->StructureModel.EnvironmentDefinitions,    \
                                    .Operation = &AssignEnvironmentDefinitions},                 \
                                   {.Object = &dbModel->EnergyModel.PairTables,                   \
                                    .Operation = &AssignPairEnergyTables},                       \
                                   {.Object = &dbModel->EnergyModel.ClusterTables,                \
                                    .Operation = AssignClusterEnergyTables},                     \
                                   {.Object = &dbModel->TransitionModel.JumpCollections,          \
                                    .Operation = AssignJumpCollections},                         \
                                   {.Object = &dbModel->TransitionModel.JumpDirections,           \
                                    .Operation = AssignJumpDirections}}


// Main function - assign the provided DbModel object with the provided database and project number
int AssignDatabaseModel(DbModel_t* dbModel, char* dbFile, int projectNumber);

// Get Project IDs from database and store them in projectIds
int AssignProjectIds(sqlite3 *db, struct ProjectIds* projectIds);

// Prepare SQL Statement and assign project or parent ID to sql statement
int PrepareSqlStatement(char *sql_query, sqlite3 *db, sqlite3_stmt **sql_statement, int id);

// Assignment functions

int AssignParentObjects(DbModel_t *dbModel, sqlite3 *db, const struct ProjectIds* projectIds);

int AssignChildObjects(DbModel_t *dbModel, sqlite3 *db, const struct ProjectIds *projectIds);


int AssignStructureModel(char* sql_query, sqlite3 *db, void *obj, const struct ProjectIds *projectIds);

int AssignEnvironmentDefinitions(char* sql_query, sqlite3 *db, void *obj, const struct ProjectIds *projectIds);


int AssignEnergyModel(char* sqlQuery, sqlite3 *db, void *obj, const struct ProjectIds *projectIds);

int AssignPairEnergyTables(char* sql_query, sqlite3 *db, void *obj, const struct ProjectIds *projectIds);

int AssignClusterEnergyTables(char* sql_query, sqlite3 *db, void *obj, const struct ProjectIds *projectIds);


int AssignTransitionModel(char* sql_query, sqlite3 *db, void *obj, const struct ProjectIds *projectIds);

int AssignJumpCollections(char* sql_query, sqlite3 *db, void *obj, const struct ProjectIds *projectIds);

int AssignJumpDirections(char* sql_query, sqlite3 *db, void *obj, const struct ProjectIds *projectIds);

int AssignLatticeModel(char* sql_query, sqlite3 *db, void *obj, const struct ProjectIds *projectIds);

// Assign the JumpDirections to their corresponding JumpCollections
int DistributeJumpDirections(DbModel_t* dbModel);
