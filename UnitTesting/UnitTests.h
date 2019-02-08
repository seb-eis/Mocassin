#ifndef ICON_SIMULATOR_UNITTESTS_H
#define ICON_SIMULATOR_UNITTESTS_H

#endif //ICON_SIMULATOR_UNITTESTS_H

#include "MinimalUnitTest.h"
#include "Framework/Sqlite/SqliteReader.h"
#include <stdio.h>
#include "ExternalLibraries/sqlite3.h"
#include <string.h>
#include <stdlib.h>
#include <stdint.h>

/**

int tests_run = 0;
char error_message[265];

typedef Span_t(int32_t, IntegerSpan) IntergerSpan_t;

static char* TestSpans()
{
    IntergerSpan_t integerSpan;
    integerSpan = new_Span(integerSpan, 10);

    int i = 0;
    cpp_foreach(intIter, integerSpan)
    {
        *intIter = i;
        i++;
    }

    i = 0;
    cpp_foreach(intIter, integerSpan)
    {
        mu_assert("error! span does not contain correct numbers", *intIter == i);
        i++;
    }

    delete_Span(integerSpan);

    return 0;
}

static char* TestArrays()
{
    Array_t(int, 4, intArray);
    struct intArray arr;
    arr = new_Array(arr, 4);
    printf("%d", array_Get(arr, 0,0,0,0));

    return 0;
}


char *dbFile = "../Database/InteropTestJohn.db";

static char* DatabaseTest()
{

    char *error_message = malloc(sizeof(char) * 258);
    int error_code = 0;

    sqlite3 *db;

    mu_assert("error! could not execute sql query", sqlite3_open(dbFile, &db) == SQLITE_OK);

    char* sql_query = "select PackageId from JobModels where PackageId = ?1";
    sqlite3_stmt *sql_statement = NULL;
    int packageid = 1;
    error_code = PrepareSqlStatement(sql_query, db, &sql_statement, packageid);
    sprintf(error_message, "Could not prepare sql statement. sql error code: %i", error_code);
    mu_assert(error_message, error_code == SQLITE_ROW);

    mu_assert("Could not get right projcect ID", sqlite3_column_int(sql_statement, 0) == packageid);

    sqlite3_finalize(sql_statement);

    sqlite3_close(db);

    free(error_message);

    return 0;
};

struct DbLoadIndices projectIds = {
        .PackageContextId = 1,
        .StructureContextId = -1,
        .EnergyContextId = -1,
        .TransitionContextId = -1,
        .LatticeContextId = -1
};

static char *FetchProjectIDs()
{

    int error_code = 0;

    sqlite3 *db;

    char *database = "../Database/InteropTestJohn.db";
    sqlite3_open(database, &db);

    error_code = AssignProjectIds(db, &projectIds);
    sprintf(error_message, "Could not assign project ids. Sql error code: %i", error_code);
    mu_assert(error_message, error_code == SQLITE_OK);

    int expectedStructureId = 1;
    sprintf(error_message, "Did not get right structure id. Expected: %i , Received: %i",
            expectedStructureId, projectIds.StructureContextId);
    mu_assert(error_message, projectIds.StructureContextId == expectedStructureId);
    int expectedEnergyId = 1;
    sprintf(error_message, "Did not get right energy id. Expected: %i , Received: %i",
            expectedEnergyId, projectIds.EnergyContextId);
    mu_assert(error_message, projectIds.EnergyContextId == expectedEnergyId);
    int expectedTransitionId = 1;
    sprintf(error_message, "Did not get right transition id. Expected: %i , Received: %i",
            expectedTransitionId, projectIds.TransitionContextId);
    mu_assert(error_message, projectIds.TransitionContextId == expectedTransitionId);


    sqlite3_close(db);

    return 0;
};

DbModel_t dbModel;

static char *TestStructureModelAssignment()
{
    int error_code = 0;

    sqlite3 *db;

    char *database = "../Database/InteropTestJohn.db";
    sqlite3_open(database, &db);

    StructureModel_t* structureModel = &dbModel.StructureModel;

    error_code = AssignStructureModel("", db, structureModel, &projectIds);
    sprintf(error_message, "Could not assign structureID. Sql error code: %i", error_code);
    mu_assert(error_message, error_code == SQLITE_OK);

    int expectedValue = 16;
    sprintf(error_message, "Did not get right NumOfTrackersPerCell. Expected: %i , Received: %i",
            expectedValue, structureModel->NumOfTrackersPerCell);
    mu_assert(error_message, structureModel->NumOfTrackersPerCell == expectedValue);

    expectedValue = 36;
    unsigned long span_size = span_GetSize(structureModel->EnvironmentDefinitions);
    sprintf(error_message, "Did not get right size of environment span. Expected: %i , Received: %lu",
            expectedValue, span_size);
    mu_assert(error_message, span_size == expectedValue);

    error_code = AssignEnvironmentDefinitions("", db, &structureModel->EnvironmentDefinitions, &projectIds);
    sprintf(error_message, "Could not assign environment definitions. Sql error code: %i", error_code);
    mu_assert(error_message, error_code == SQLITE_OK);

    sqlite3_close(db);

    return 0;
}

static char *TestLatticeAssignment()
{
    int error_code = 0;

    sqlite3 *db;

    char *database = "../Database/InteropTestJohn.db";
    sqlite3_open(database, &db);

    LatticeModel_t* latticeModel = &dbModel.LatticeModel;

    error_code = AssignLatticeModel("", db, latticeModel, &projectIds);
    sprintf(error_message, "Could not assign structureID. Sql error code: %i", error_code);
    mu_assert(error_message, error_code == SQLITE_OK);

    sqlite3_close(db);

    return 0;
}

static char *TestTransitionAssignment()
{
    int error_code = 0;

    sqlite3 *db;

    char *database = "../Database/InteropTestJohn.db";
    sqlite3_open(database, &db);

    TransitionModel_t* transitionModel = &dbModel.TransitionModel;

    error_code = AssignTransitionModel("", db, transitionModel, &projectIds);
    sprintf(error_message, "Could not assign transition model. Sql error code: %i", error_code);
    mu_assert(error_message, error_code == SQLITE_OK);

    error_code = AssignJumpCollections("", db, &transitionModel->JumpCollections, &projectIds);
    sprintf(error_message, "Could not assign jump collection. Sql error code: %i", error_code);
    mu_assert(error_message, error_code == SQLITE_OK);

    error_code = AssignJumpDirections("", db, &transitionModel->JumpDirections, &projectIds);
    sprintf(error_message, "Could not assign jump direction. Sql error code: %i", error_code);
    mu_assert(error_message, error_code == SQLITE_OK);

    error_code = DistributeJumpDirections(&dbModel);
    sprintf(error_message, "Could not distribute jump directions. Error code: %d", error_code);
    mu_assert(error_message, error_code == 0);


    sqlite3_close(db);

    return 0;
}

static char *TestEnergyAssigment()
{
    int error_code = 0;

    sqlite3 *db;

    char *database = "../Database/InteropTestJohn.db";
    sqlite3_open(database, &db);

    EnergyModel_t* energyModel = &dbModel.EnergyModel;

    error_code = AssignEnergyModel("", db, energyModel, &projectIds);
    sprintf(error_message, "Could not assign energy model. Sql error code: %i", error_code);
    mu_assert(error_message, error_code == SQLITE_OK);

    error_code = AssignPairEnergyTables("", db, &energyModel->PairTables, &projectIds);
    sprintf(error_message, "Could not assign pair energy tables. Sql error code: %i", error_code);
    mu_assert(error_message, error_code == SQLITE_OK);

    error_code = AssignClusterEnergyTables("", db, &energyModel->ClusterTables, &projectIds);
    sprintf(error_message, "Could not assign cluster energy tables. Sql error code: %i", error_code);
    mu_assert(error_message, error_code == SQLITE_OK);

    DistributeJumpDirections(&dbModel);

    sqlite3_close(db);

    return 0;
}

char *TestCompleteAssignment()
{
    DbModel_t dbModel;

    int error_code = 0;

    char *database = "../Database/InteropTestJohn.db";

    error_code = AssignDatabaseModel(&dbModel, database, 1);

    sprintf(error_message, "Could not assign databasemodel. Sql error code: %i", error_code);
    mu_assert(error_message, error_code == 0);

    return 0;
}

static char *all_tests() {

    printf("Testing spans...");
    mu_run_test(TestSpans);
    printf("Done\n");

    //printf("Testing arrays...");
    //mu_run_test(TestArrays);
    //printf("Done\n");

    printf("Testing general database access and query...");
    mu_run_test(DatabaseTest);
    printf("Done\n");

    printf("Testing project id fetching...");
    mu_run_test(FetchProjectIDs);
    printf("Done\n");


    printf("Testing structure assignment...");
    mu_run_test(TestStructureModelAssignment);
    printf("Done\n");

    printf("Testing lattice assignment...");
    mu_run_test(TestLatticeAssignment);
    printf("Done\n");

    printf("Testing transition assignment...");
    mu_run_test(TestTransitionAssignment);
    printf("Done\n");

    printf("Testing energy model assignment...");
    mu_run_test(TestEnergyAssigment);
    printf("Done\n");

    printf("Testing complete assignment...");
    mu_run_test(TestCompleteAssignment);
    printf("Done\n");

    return 0;
};

 **/