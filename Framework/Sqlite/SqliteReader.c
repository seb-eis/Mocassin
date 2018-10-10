//
// Created by john on 18.09.2018.
//

#include "SqliteReader.h"

int GetSqlQuery(char* key, sqlite3 *db, char* sqlQuery)
{
    sqlite3_stmt *sqlite3Stmt = NULL;

    char* sql = GET_SQL_QUERY;
    CHECK_SQL(sqlite3_prepare_v2(db, sql, -1, &sqlite3Stmt, NULL), SQLITE_OK);
    CHECK_SQL(sqlite3_bind_text(sqlite3Stmt, 1, key, -1, SQLITE_STATIC), SQLITE_OK);
    CHECK_SQL(sqlite3_step(sqlite3Stmt), SQLITE_ROW);
    strcpy(sqlQuery, (char *)(sqlite3_column_text(sqlite3Stmt, 0)));

    CHECK_SQL(sqlite3_finalize(sqlite3Stmt), SQLITE_OK);

    return 0;
}

int PrepareSqlStatement(char *sql, sqlite3 *db, sqlite3_stmt **sqlite3Stmt, int queryValue)
{
    CHECK_SQL(sqlite3_prepare_v2(db, sql, -1, &(*sqlite3Stmt), NULL), SQLITE_OK);
    CHECK_SQL(sqlite3_bind_int(*sqlite3Stmt, POSITION_OF_QUERY_VALUE, queryValue), SQLITE_OK);
    CHECK_SQL(sqlite3_step(*sqlite3Stmt), SQLITE_ROW);
    return SQLITE_ROW;
}


int AssignProjectIds(sqlite3 *db, struct ProjectIds* projectIds)
{
    char *sql = malloc(sizeof(char) * MAX_QUERY_LENGTH);

    char *keywords[NUMBER_OF_PARENT_OBJECTS] = PROJECT_ID_KEYWORDS;
    int *ids[NUMBER_OF_PARENT_OBJECTS] = PROJECT_IDS(&(*projectIds));

    for (int opId = 0; opId < NUMBER_OF_PARENT_OBJECTS; opId++)
    {
        CHECK_SQL(GetSqlQuery(keywords[opId], db, sql), 0);
        CHECK_SQL(GetIdFromDatabase(sql, db, ids[opId]), SQLITE_OK);
    }

    return SQLITE_OK;
}

int GetIdFromDatabase(char* sql, sqlite3 *db, int* id)
{
    sqlite3_stmt *sqlite3Stmt = NULL;
    CHECK_SQL(sqlite3_prepare_v2(db, sql, -1, &sqlite3Stmt, NULL), SQLITE_OK);
    CHECK_SQL(sqlite3_step(sqlite3Stmt), SQLITE_ROW);

    *id = sqlite3_column_int(sqlite3Stmt, 0);

    CHECK_SQL(sqlite3_finalize(sqlite3Stmt), SQLITE_OK);
    return SQLITE_OK;
}


int AssignParentObjects(DbModel_t *dbModel, sqlite3 *db, const struct ProjectIds* ids)
{
    char *sql = malloc(sizeof(char) * MAX_QUERY_LENGTH);

    int (*operations[NUMBER_OF_PARENT_OBJECTS]) (char*, sqlite3*, void*, const struct ProjectIds*) = PARENT_OPERATIONS;
    void *objects[NUMBER_OF_PARENT_OBJECTS] = PARENT_OBJECTS(&(*dbModel));
    char *keywords[NUMBER_OF_PARENT_OBJECTS] = PARENT_KEYWORDS;

    for (int opId = 0; opId < NUMBER_OF_PARENT_OBJECTS; opId++)
    {
        CHECK_SQL(GetSqlQuery(keywords[opId], db, sql), 0);
        CHECK_SQL(operations[opId](sql, db, objects[opId], ids), SQLITE_OK);
    }

    free(sql);

    return SQLITE_OK;
}


int AssignStructureModel(char* sql, sqlite3* db, void* obj, const struct ProjectIds* projectIds)
{
    sqlite3_stmt *sqlite3Stmt = NULL;
    CHECK_SQL(PrepareSqlStatement(sql, db, &sqlite3Stmt, projectIds->ProjectId), SQLITE_ROW)

    StructureModel_t *structureModel = (StructureModel_t*) obj;
    structureModel->NumOfTrackersPerCell = sqlite3_column_int(sqlite3Stmt, 0);
    structureModel->NumOfGlobalTrackers = sqlite3_column_int(sqlite3Stmt, 1);
    int numberOfEnvironments =1;
    new_Span(structureModel->EnvironmentDefinitions, (size_t) numberOfEnvironments);

    CHECK_SQL(sqlite3_finalize(sqlite3Stmt), SQLITE_OK);
    return SQLITE_OK;
}

int AssignEnergyModel(char* sql, sqlite3 *db, void *obj, const struct ProjectIds *projectIds)
{
    sqlite3_stmt *sqlite3Stmt = NULL;
    CHECK_SQL(PrepareSqlStatement(sql, db, &sqlite3Stmt, projectIds->ProjectId), SQLITE_ROW)

    EnergyModel_t *energyModel = (EnergyModel_t*) obj;
    int numberOfPairTables = sqlite3_column_int(sqlite3Stmt, 0);
    int numberOfClusterTables = sqlite3_column_int(sqlite3Stmt, 1);
    new_Span(energyModel->PairTables, (size_t) numberOfPairTables);
    new_Span(energyModel->PairTables, (size_t) numberOfClusterTables);

    CHECK_SQL(sqlite3_finalize(sqlite3Stmt), SQLITE_OK);
    return SQLITE_OK;
}

int AssignChildObjects(DbModel_t *dbModel, sqlite3 *db, const struct ProjectIds *projectIds)
{
    char *sql = malloc(sizeof(char) * MAX_QUERY_LENGTH);

    int (*operations[NUMBER_OF_CHILD_OBJECTS]) (char*, sqlite3*, void*, const struct ProjectIds*) = CHILD_OPERATIONS;
    void *objects[NUMBER_OF_CHILD_OBJECTS] = CHILD_OBJECTS(&(*dbModel));
    char *keywords[NUMBER_OF_CHILD_OBJECTS] = CHILD_KEYWORDS;

    for (int opId = 0; opId < NUMBER_OF_CHILD_OBJECTS; opId++)
    {
        CHECK_SQL(GetSqlQuery(keywords[opId], db, sql), 0);
        CHECK_SQL(operations[opId](sql, db, objects[opId], projectIds), SQLITE_OK);
    }

    free(sql);

    return SQLITE_OK;
}


int AssignEnvironmentDefinitions(char* sql, sqlite3 *db, void *obj, const struct ProjectIds *projectIds)
{
    sqlite3_stmt *sqlite3Stmt = NULL;
    CHECK_SQL(PrepareSqlStatement(sql, db, &sqlite3Stmt, projectIds->StructureId), SQLITE_ROW)

    EnvironmentDefinitions_t *environmentDefinitions = (EnvironmentDefinitions_t*) obj;

    cpp_foreach(current, *environmentDefinitions)
    {
        current->ObjId = sqlite3_column_int(sqlite3Stmt, 0);
        memcpy(current->PositionParticleIds, sqlite3_column_blob(sqlite3Stmt, 1), (size_t) sqlite3_column_bytes(sqlite3Stmt, 1));
        memcpy(current->UpdateParticleIds, sqlite3_column_blob(sqlite3Stmt, 2), (size_t) sqlite3_column_bytes(sqlite3Stmt, 2));
        memcpy(&current->PairDefinitions, sqlite3_column_blob(sqlite3Stmt, 3), (size_t) sqlite3_column_bytes(sqlite3Stmt, 3));
        memcpy(&current->ClusterDefinitions, sqlite3_column_blob(sqlite3Stmt, 4), (size_t) sqlite3_column_bytes(sqlite3Stmt,4));
        CHECK_SQL(sqlite3_step(sqlite3Stmt), SQLITE_ROW);
    }

    CHECK_SQL(sqlite3_finalize(sqlite3Stmt), SQLITE_OK);

    return SQLITE_OK;
}

int AssignPairEnergyTables(char* sql, sqlite3 *db, void *obj, const struct ProjectIds *projectIds)
{
    sqlite3_stmt *sqlite3Stmt = NULL;
    CHECK_SQL(PrepareSqlStatement(sql, db, &sqlite3Stmt, projectIds->EnergyId), SQLITE_ROW)

    PairTables_t *pairTables = (PairTables_t*) obj;
    cpp_foreach(current, *pairTables)
    {
        current->ObjectId = sqlite3_column_int(sqlite3Stmt, 0);
        memcpy(&current->EnergyTable, sqlite3_column_blob(sqlite3Stmt, 1), (size_t) sqlite3_column_bytes(sqlite3Stmt, 1));
        CHECK_SQL(sqlite3_step(sqlite3Stmt), SQLITE_ROW)
    }

    CHECK_SQL(sqlite3_finalize(sqlite3Stmt), SQLITE_OK);

    return SQLITE_OK;
}

int AssignClusterEnergyTables(char* sql, sqlite3 *db, void *obj, const struct ProjectIds *projectIds)
{
    sqlite3_stmt *sqlite3Stmt = NULL;
    CHECK_SQL(PrepareSqlStatement(sql, db, &sqlite3Stmt, projectIds->EnergyId), SQLITE_ROW)

    ClusterTables_t *clusterTables = (ClusterTables_t*) obj;
    cpp_foreach(current, *clusterTables)
    {
        current->ObjectId = sqlite3_column_int(sqlite3Stmt, 0);
        memcpy(&current->EnergyTable, sqlite3_column_blob(sqlite3Stmt, 1), (size_t) sqlite3_column_bytes(sqlite3Stmt, 1));
        memcpy(&current->OccupationCodes, sqlite3_column_blob(sqlite3Stmt, 2), (size_t) sqlite3_column_bytes(sqlite3Stmt, 2));
        CHECK_SQL(sqlite3_step(sqlite3Stmt), SQLITE_ROW)
    }

    CHECK_SQL(sqlite3_finalize(sqlite3Stmt), SQLITE_OK);

    return SQLITE_OK;
}


int AssignTransitionModel(char* sql, sqlite3 *db, void *obj, const struct ProjectIds *projectIds)
{
    sqlite3_stmt *sqlite3Stmt = NULL;
    CHECK_SQL(PrepareSqlStatement(sql, db, &sqlite3Stmt, projectIds->ProjectId), SQLITE_ROW)

    TransitionModel_t *transitionModel = (TransitionModel_t*) obj;
    memcpy(&transitionModel->JumpAssignTable, sqlite3_column_blob(sqlite3Stmt, 0), (size_t) sqlite3_column_bytes(sqlite3Stmt, 0));
    memcpy(&transitionModel->JumpCountTable, sqlite3_column_blob(sqlite3Stmt, 0), (size_t) sqlite3_column_bytes(sqlite3Stmt, 0));

    int numberOfJumpCollections = 0; //TODO: delete this
    int numberOfJumpDirections = 0; //TODO: delete this
    new_Span(transitionModel->JumpCollections, (size_t) numberOfJumpCollections);
    new_Span(transitionModel->JumpDirections, (size_t) numberOfJumpDirections);

    CHECK_SQL(sqlite3_finalize(sqlite3Stmt), SQLITE_OK);
    return SQLITE_OK;
}

int AssignJumpCollections(char* sql, sqlite3 *db, void *obj, const struct ProjectIds *projectIds)
{
    sqlite3_stmt *sqlite3Stmt = NULL;
    CHECK_SQL(PrepareSqlStatement(sql, db, &sqlite3Stmt, projectIds->TransitionId), SQLITE_ROW)

    JumpCollections_t *jumpCollections = (JumpCollections_t*) obj;


    cpp_foreach(current, *jumpCollections)
    {
        current->ObjectId = sqlite3_column_int(sqlite3Stmt, 1);
        current->ParticleMask = sqlite3_column_int64(sqlite3Stmt, 2);
        memcpy(&current->JumpRules, sqlite3_column_blob(sqlite3Stmt, 3), (size_t) sqlite3_column_bytes(sqlite3Stmt, 3));

        CHECK_SQL(sqlite3_step(sqlite3Stmt), SQLITE_ROW)
    }

    CHECK_SQL(sqlite3_finalize(sqlite3Stmt), SQLITE_OK);

    return SQLITE_OK;
}

int AssignJumpDirections(char* sql, sqlite3 *db, void *obj, const struct ProjectIds *projectIds)
{
    sqlite3_stmt *sqlite3Stmt = NULL;
    CHECK_SQL(PrepareSqlStatement(sql, db, &sqlite3Stmt, projectIds->TransitionId), SQLITE_ROW)

    JumpDirections_t *jumpDirections = (JumpDirections_t*) obj;

    cpp_foreach(current, *jumpDirections)
    {
        current->ObjectId = sqlite3_column_int(sqlite3Stmt, 0);
        current->CollectionId = sqlite3_column_int(sqlite3Stmt, 1);
        current->FieldProjectionFactor = sqlite3_column_double(sqlite3Stmt, 2);
        current->JumpLength = sqlite3_column_int(sqlite3Stmt, 3);
        current->PositionId = sqlite3_column_int(sqlite3Stmt, 4);

        memcpy(&current->JumpSequence, sqlite3_column_blob(sqlite3Stmt, 5), (size_t) sqlite3_column_bytes(sqlite3Stmt, 5));
        memcpy(&current->JumpLinkSequence, sqlite3_column_blob(sqlite3Stmt, 6), (size_t) sqlite3_column_bytes(sqlite3Stmt, 6));
        memcpy(&current->GlobalMoveSequence, sqlite3_column_blob(sqlite3Stmt, 7), (size_t) sqlite3_column_bytes(sqlite3Stmt, 7));
        memcpy(&current->LocalMoveSequence, sqlite3_column_blob(sqlite3Stmt, 8), (size_t) sqlite3_column_bytes(sqlite3Stmt, 8));


        CHECK_SQL(sqlite3_step(sqlite3Stmt), SQLITE_ROW)
    }

    CHECK_SQL(sqlite3_finalize(sqlite3Stmt), SQLITE_OK);

    return SQLITE_OK;
}

int AssignLatticeModel(char* sql, sqlite3 *db, void *obj, const struct ProjectIds *projectIds)
{
    sqlite3_stmt *sqlite3Stmt = NULL;
    CHECK_SQL(PrepareSqlStatement(sql, db, &sqlite3Stmt, projectIds->ProjectId), SQLITE_ROW)

    LatticeModel_t *latticeModel = (LatticeModel_t*) obj;

    latticeModel->NumOfMobiles = sqlite3_column_int(sqlite3Stmt, 0);
    latticeModel->NumOfSelectables = sqlite3_column_int(sqlite3Stmt, 1);
    memcpy(&latticeModel->EnergyBackground, sqlite3_column_blob(sqlite3Stmt, 2), (size_t) sqlite3_column_bytes(sqlite3Stmt, 2));
    memcpy(&latticeModel->Lattice, sqlite3_column_blob(sqlite3Stmt, 3), (size_t) sqlite3_column_bytes(sqlite3Stmt, 3));
    memcpy(&latticeModel->SizeVector, sqlite3_column_blob(sqlite3Stmt, 4), (size_t) sqlite3_column_bytes(sqlite3Stmt, 4));

    CHECK_SQL(sqlite3_finalize(sqlite3Stmt), SQLITE_OK);

    return SQLITE_OK;
}

int AssignDatabaseModel(DbModel_t* dbModel, char* dbFile, int projectNumber)
{
    struct ProjectIds projectIds = {
            .ProjectId = projectNumber,
            .StructureId = -1,
            .EnergyId = -1,
            .TransitionId = -1,
            .LatticeId = -1,
    };

    sqlite3 *db;
    CHECK_SQL(sqlite3_open(dbFile, &db), SQLITE_OK)

    CHECK_SQL(AssignProjectIds(db, &projectIds), SQLITE_OK)
    CHECK_SQL(AssignParentObjects(dbModel, db, &projectIds), SQLITE_OK)
    CHECK_SQL(AssignChildObjects(dbModel, db, &projectIds), SQLITE_OK)
    return 0;
}

int TestDatabase( char* dbFile )
{
    sqlite3 *db;
    CHECK_SQL(sqlite3_open(dbFile, &db), SQLITE_OK);
    CHECK_SQL(sqlite3_close(db), SQLITE_OK);
    return SQLITE_OK;
}

int TestQuery(char* dbFile)
{
    sqlite3 *db;

    CHECK_SQL(sqlite3_open(dbFile, &db), SQLITE_OK);
    char* sql = malloc(sizeof(char)*500);
    CHECK_SQL(GetSqlQuery("test", db, sql),0)

    sqlite3_stmt *sqlite3Stmt = NULL;
    CHECK_SQL(PrepareSqlStatement(sql, db, &sqlite3Stmt, 0), SQLITE_ROW)
    printf(sqlite3_column_text(sqlite3Stmt, 0));
    printf("\n");

    sqlite3_finalize(sqlite3Stmt);
    sqlite3_close(db);
    free(sql);

    return 0;
}