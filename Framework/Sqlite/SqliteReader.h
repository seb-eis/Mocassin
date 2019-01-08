//
// Created by john on 18.09.2018.
//

#ifndef ICON_SIMULATOR_SQLITEREADER_H
#define ICON_SIMULATOR_SQLITEREADER_H

#endif //ICON_SIMULATOR_SQLITEREADER_H

#include <stdio.h>
#include "ExternalLibraries/sqlite3.h"
#include <string.h>
#include <stdlib.h>
#include <stdint.h>
#include "Simulator/Data/Model/Database/DbModel.h"

/*
 * How to use the SqliteReader:
 * Use the "AssignDatabaseModel" to assign your DatabaseModel object. You need to provide the database location
 * and the project ID. Every other function is only for internal usage. If an error occurs at any instance the
 * operation stops and a SQLITE3 error number (always positive) or a custom error number (always negative) is returned.
 *
 * How the SqliteReader works internally:
 * When the "AssignDatabaseModel" function is called, the provided database is accessed and the required data is
 * fetched from the database by the provided Project ID. This is accomplished in three steps:
 * 1) The IDs of the parent objects are fetched from the database. The Parent objects are the member variables within
 *    the database object, e.g. the Structure object.
 * 2) The member variables of the parent objects are assigned in case of simple types of BLOBs. These can be easily read
 *    from the database. In case of child objects (spans) memory is allocated.
 * 3) The member variables of the child objects are assigned. Child objects are spans within the parent objects, which
 *    have to be fetched from different sql tables than the parent objects.
 *
 * For a new parent or child objects the macros below have to be updated. The _KEYWORD macro needs to specify the
 * keyword by which the sql query is fetched from the database. This sql query is then used to access the needed data.
 * The *_OPERATION macros need to hold the function by which the data is assigned to the object. Mind that the function
 * arguments have to be (DbModel_t*, sqlite3*, const struct ProjectIds*).
 * The sql queries to get the data from the database are also stored within the database and accessed via the keywords
 * provided here as macros. The *_OBJECTS macros define the actual object which are assigned during the method.
 * PLEASE MIND: These macros have to be in the same order.
 *
 * The functions AssignParentObjects and AssignChildObjects automatically assign the objects with the described macros.
 *
 */

// Macros for sql query fetching from database
#define MAX_QUERY_LENGTH 1024
#define ID_POS_IN_SQLSTMT 1
#define GET_SQL_QUERY "select Statement from SqlStatements where Keyword = ?1"

// Macros for fetching project IDs from database
#define PROJECT_ID_KEYWORDS { "StructureID", "EnergyID", "TransitionID", "LatticeID" }
#define PROJECT_IDS(projectIds) { projectIds.StructureId, projectIds.EnergyId, projectIds.TransitionId, \
                                    projectIds.LatticeId }

// Macros for operations related to the parend objects defined in PARENT_OBJECTS
// For new Parent Objects add sql KEYWORD, OPERATION and OBJECT
#define NUMBER_OF_PARENT_OBJECTS 4
#define PARENT_KEYWORDS { "Structure", "Energy", "Transition", "Lattice" }
#define PARENT_OPERATIONS { AssignStructureModel, AssignEnergyModel, AssignTransitionModel, AssignLatticeModel }
#define PARENT_OBJECTS(dbModel) { dbModel.Structure, dbModel.Energy, dbModel.Transition, dbModel.Lattice }

// Macros for operations related to the child objects defined in CHILD_OBJECTS
// For new child objects add sql KEYWORD, OPERATION and OBJECT
#define NUMBER_OF_CHILD_OBJECTS 5
#define CHILD_KEYWORDS { "EnvironmentDefinitions", "PairEnergyTables", \
                         "ClusterEnergyTables", "JumpColletions", "JumpDirections" }
#define CHILD_OPERATIONS {  AssignEnvironmentDefinitions, \
                            AssignPairEnergyTables, \
                            AssignClusterEnergyTables, \
                            AssignJumpCollections, \
                            AssignJumpDirections}
#define CHILD_OBJECTS(dbModel) {dbModel.Structure.EnvironmentDefinitions, \
                                dbModel.Energy.PairTables, \
                                dbModel.Energy.ClusterTables, \
                                dbModel.Transition.JumpCollections, \
                                dbModel.Transition.JumpDirections}

// Macro the return the function in case something goes wrong
#define CHECK_SQL(X, Y) if (X != Y) { return X; }


struct ProjectIds{
    int ProjectId;
    int StructureId;
    int EnergyId;
    int TransitionId;
    int LatticeId;
};

// Main function - assign the provided DbModel object with the provided database and project number
int AssignDatabaseModel(DbModel_t* dbModel, char* dbFile, int projectNumber);

// Get Project IDs from database and store them in projectIds
int AssignProjectIds(sqlite3 *db, struct ProjectIds* projectIds);

// Prepare SQL Statement and assign project or parent ID to sql statement
int PrepareSqlStatement(char *sql_query, sqlite3 *db, sqlite3_stmt **sql_statement, int id);

// Get the sql query for the objects from the database by the provided keyword
int GetSqlQuery(char* keyword, sqlite3 *db, char* sqlQuery);

// Get Id from database with provided sql_query. Result is stored in id. Return value is error code.
int GetIdFromDatabase(char* sql_query, sqlite3 *db, int* id);

// Assignment functions

int AssignParentObjects(DbModel_t *dbModel, sqlite3 *db, const struct ProjectIds* ids);

int AssignChildObjects(DbModel_t *dbModel, sqlite3 *db, const struct ProjectIds *projectIds);


int AssignStructureModel(char* sql_query, sqlite3 *db, void *obj, const struct ProjectIds *projectIds);

int AssignEnvironmentDefinitions(char* sql_query, sqlite3 *db, void *obj, const struct ProjectIds *projectIds);


int AssignEnergyModel(char* sql_query, sqlite3 *db, void *obj, const struct ProjectIds *projectIds);

int AssignPairEnergyTables(char* sql_query, sqlite3 *db, void *obj, const struct ProjectIds *projectIds);

int AssignClusterEnergyTables(char* sql_query, sqlite3 *db, void *obj, const struct ProjectIds *projectIds);


int AssignTransitionModel(char* sql_query, sqlite3 *db, void *obj, const struct ProjectIds *projectIds);

int AssignJumpCollections(char* sql_query, sqlite3 *db, void *obj, const struct ProjectIds *projectIds);

int AssignJumpDirections(char* sql_query, sqlite3 *db, void *obj, const struct ProjectIds *projectIds);


int AssignLatticeModel(char* sql_query, sqlite3 *db, void *obj, const struct ProjectIds *projectIds);


// Test functions

int TestDatabase( char* dbFile );

int TestQuery(char* dbFile);