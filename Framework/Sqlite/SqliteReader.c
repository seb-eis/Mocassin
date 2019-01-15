//
// Created by john on 18.09.2018.
//

#include "SqliteReader.h"

int GetSqlQuery(char* keyword, sqlite3 *db, char* sqlQuery)
{
    sqlite3_stmt *sql_statement = NULL;

    char* sql_query = GET_SQL_QUERY;
    CHECK_SQL(sqlite3_prepare_v2(db, sql_query, -1, &sql_statement, NULL), SQLITE_OK);
    CHECK_SQL(sqlite3_bind_text(sql_statement, 1, keyword, -1, SQLITE_STATIC), SQLITE_OK);
    CHECK_SQL(sqlite3_step(sql_statement), SQLITE_ROW);
    strcpy(sqlQuery, (char *)(sqlite3_column_text(sql_statement, 0)));

    CHECK_SQL(sqlite3_finalize(sql_statement), SQLITE_OK);

    return 0;
}

int PrepareSqlStatement(char *sql_query, sqlite3 *db, sqlite3_stmt **sql_statement, int id)
{
    CHECK_SQL(sqlite3_prepare_v2(db, sql_query, -1, &(*sql_statement), NULL), SQLITE_OK);
    CHECK_SQL(sqlite3_bind_int(*sql_statement, ID_POS_IN_SQLSTMT, id), SQLITE_OK);
    CHECK_SQL(sqlite3_step(*sql_statement), SQLITE_ROW);
    return SQLITE_ROW;
}


int AssignProjectIds(sqlite3 *db, struct ProjectIds* projectIds)
{
    char *sql_query = malloc(sizeof(char) * MAX_QUERY_LENGTH);

    char *keywords[NUMBER_OF_PARENT_OBJECTS] = PROJECT_ID_KEYWORDS;
    int *ids[NUMBER_OF_PARENT_OBJECTS] = PROJECT_IDS(&(*projectIds));

    for (int opId = 0; opId < NUMBER_OF_PARENT_OBJECTS; opId++)
    {
        CHECK_SQL(GetSqlQuery(keywords[opId], db, sql_query), 0);
        CHECK_SQL(GetIdFromDatabase(sql_query, db, ids[opId]), SQLITE_OK);
    }

    return SQLITE_OK;
}

int GetIdFromDatabase(char* sql_query, sqlite3 *db, int* id)
{
    sqlite3_stmt *sql_statement = NULL;
    CHECK_SQL(sqlite3_prepare_v2(db, sql_query, -1, &sql_statement, NULL), SQLITE_OK);
    CHECK_SQL(sqlite3_step(sql_statement), SQLITE_ROW);

    *id = sqlite3_column_int(sql_statement, 0);

    CHECK_SQL(sqlite3_finalize(sql_statement), SQLITE_OK);
    return SQLITE_OK;
}


int AssignParentObjects(DbModel_t *dbModel, sqlite3 *db, const struct ProjectIds* ids)
{
    char *sql_query = malloc(sizeof(char) * MAX_QUERY_LENGTH);

    int (*operations[NUMBER_OF_PARENT_OBJECTS]) (char*, sqlite3*, void*, const struct ProjectIds*) = PARENT_OPERATIONS;
    void *objects[NUMBER_OF_PARENT_OBJECTS] = PARENT_OBJECTS(&(*dbModel));
    char *keywords[NUMBER_OF_PARENT_OBJECTS] = PARENT_KEYWORDS;

    for (int opId = 0; opId < NUMBER_OF_PARENT_OBJECTS; opId++)
    {
        CHECK_SQL(GetSqlQuery(keywords[opId], db, sql_query), 0);
        CHECK_SQL(operations[opId](sql_query, db, objects[opId], ids), SQLITE_OK);
    }

    free(sql_query);

    return SQLITE_OK;
}


int AssignStructureModel(char* sql_query, sqlite3* db, void* obj, const struct ProjectIds* projectIds)
{
    sqlite3_stmt *sql_statement = NULL;
    CHECK_SQL(PrepareSqlStatement(sql_query, db, &sql_statement, projectIds->ProjectId), SQLITE_ROW)

    StructureModel_t *structureModel = (StructureModel_t*) obj;
    structureModel->NumOfTrackersPerCell = sqlite3_column_int(sql_statement, 0);
    structureModel->NumOfGlobalTrackers = sqlite3_column_int(sql_statement, 1);
    memcpy(&structureModel->InteractionRange, sqlite3_column_blob(sql_statement, 2),
            (size_t) sqlite3_column_bytes(sql_statement, 2));
    int numberOfEnvironments =1; //TODO:delete this
    new_Span(structureModel->EnvironmentDefinitions, (size_t) numberOfEnvironments);

    CHECK_SQL(sqlite3_finalize(sql_statement), SQLITE_OK);
    return SQLITE_OK;
}


int AssignEnergyModel(char* sql_query, sqlite3 *db, void *obj, const struct ProjectIds *projectIds)
{
    sqlite3_stmt *sql_statement = NULL;
    CHECK_SQL(PrepareSqlStatement(sql_query, db, &sql_statement, projectIds->ProjectId), SQLITE_ROW)

    EnergyModel_t *energyModel = (EnergyModel_t*) obj;
    int numberOfPairTables = sqlite3_column_int(sql_statement, 0);
    int numberOfClusterTables = sqlite3_column_int(sql_statement, 1);
    new_Span(energyModel->PairTables, (size_t) numberOfPairTables);
    new_Span(energyModel->PairTables, (size_t) numberOfClusterTables);

    CHECK_SQL(sqlite3_finalize(sql_statement), SQLITE_OK);
    return SQLITE_OK;
}


int AssignChildObjects(DbModel_t *dbModel, sqlite3 *db, const struct ProjectIds *projectIds)
{
    char *sql_query = malloc(sizeof(char) * MAX_QUERY_LENGTH);

    int (*operations[NUMBER_OF_CHILD_OBJECTS]) (char*, sqlite3*, void*, const struct ProjectIds*) = CHILD_OPERATIONS;
    void *objects[NUMBER_OF_CHILD_OBJECTS] = CHILD_OBJECTS(&(*dbModel));
    char *keywords[NUMBER_OF_CHILD_OBJECTS] = CHILD_KEYWORDS;

    for (int opId = 0; opId < NUMBER_OF_CHILD_OBJECTS; opId++)
    {
        CHECK_SQL(GetSqlQuery(keywords[opId], db, sql_query), 0);
        CHECK_SQL(operations[opId](sql_query, db, objects[opId], projectIds), SQLITE_OK);
    }

    free(sql_query);

    return SQLITE_OK;
}


int AssignEnvironmentDefinitions(char* sql_query, sqlite3 *db, void *obj, const struct ProjectIds *projectIds)
{
    sqlite3_stmt *sql_statement = NULL;
    CHECK_SQL(PrepareSqlStatement(sql_query, db, &sql_statement, projectIds->StructureId), SQLITE_ROW)

    EnvironmentDefinitions_t *environmentDefinitions = (EnvironmentDefinitions_t*) obj;

    cpp_foreach(current, *environmentDefinitions)
    {
        current->ObjectId = sqlite3_column_int(sql_statement, 0);
        current->SelectionParticleMask = sqlite3_column_int(sql_statement, 1);
        memcpy(current->UpdateParticleIds, sqlite3_column_blob(sql_statement, 2),
                (size_t) sqlite3_column_bytes(sql_statement, 2));
        memcpy(&current->PairDefinitions, sqlite3_column_blob(sql_statement, 3),
                (size_t) sqlite3_column_bytes(sql_statement, 3));
        memcpy(&current->ClusterDefinitions, sqlite3_column_blob(sql_statement, 4),
                (size_t) sqlite3_column_bytes(sql_statement,4));
        memcpy(current->PositionParticleIds, sqlite3_column_blob(sql_statement, 5),
               (size_t) sqlite3_column_bytes(sql_statement, 5));
        CHECK_SQL(sqlite3_step(sql_statement), SQLITE_ROW);
    }

    CHECK_SQL(sqlite3_finalize(sql_statement), SQLITE_OK);

    return SQLITE_OK;
}

int AssignPairEnergyTables(char* sql_query, sqlite3 *db, void *obj, const struct ProjectIds *projectIds)
{
    sqlite3_stmt *sql_statement = NULL;
    CHECK_SQL(PrepareSqlStatement(sql_query, db, &sql_statement, projectIds->EnergyId), SQLITE_ROW)

    PairTables_t *pairTables = (PairTables_t*) obj;
    cpp_foreach(current, *pairTables)
    {
        current->ObjectId = sqlite3_column_int(sql_statement, 0);
        memcpy(&current->EnergyTable, sqlite3_column_blob(sql_statement, 1),
                (size_t) sqlite3_column_bytes(sql_statement, 1));
        CHECK_SQL(sqlite3_step(sql_statement), SQLITE_ROW)
    }

    CHECK_SQL(sqlite3_finalize(sql_statement), SQLITE_OK);

    return SQLITE_OK;
}

int AssignClusterEnergyTables(char* sql_query, sqlite3 *db, void *obj, const struct ProjectIds *projectIds)
{
    sqlite3_stmt *sql_statement = NULL;
    CHECK_SQL(PrepareSqlStatement(sql_query, db, &sql_statement, projectIds->EnergyId), SQLITE_ROW)

    ClusterTables_t *clusterTables = (ClusterTables_t*) obj;
    cpp_foreach(current, *clusterTables)
    {
        current->ObjectId = sqlite3_column_int(sql_statement, 0);
        memcpy(&current->EnergyTable, sqlite3_column_blob(sql_statement, 1),
                (size_t) sqlite3_column_bytes(sql_statement, 1));
        memcpy(&current->OccupationCodes, sqlite3_column_blob(sql_statement, 2),
                (size_t) sqlite3_column_bytes(sql_statement, 2));
        memcpy(current->ParticleToTableId, sqlite3_column_blob(sql_statement, 3),
                (size_t) sqlite3_column_bytes(sql_statement, 3));
        CHECK_SQL(sqlite3_step(sql_statement), SQLITE_ROW)
    }

    CHECK_SQL(sqlite3_finalize(sql_statement), SQLITE_OK);

    return SQLITE_OK;
}


int AssignTransitionModel(char* sql_query, sqlite3 *db, void *obj, const struct ProjectIds *projectIds)
{
    sqlite3_stmt *sql_statement = NULL;
    CHECK_SQL(PrepareSqlStatement(sql_query, db, &sql_statement, projectIds->ProjectId), SQLITE_ROW)

    TransitionModel_t *transitionModel = (TransitionModel_t*) obj;
    memcpy(&transitionModel->JumpAssignTable, sqlite3_column_blob(sql_statement, 0),
            (size_t) sqlite3_column_bytes(sql_statement, 0));
    memcpy(&transitionModel->JumpCountTable, sqlite3_column_blob(sql_statement, 1),
            (size_t) sqlite3_column_bytes(sql_statement, 1));
    memcpy(&transitionModel->StaticTrackerAssignTable, sqlite3_column_blob(sql_statement, 2),
           (size_t) sqlite3_column_bytes(sql_statement, 2));
    memcpy(&transitionModel->GlobalTrackerAssignTable, sqlite3_column_blob(sql_statement, 3),
           (size_t) sqlite3_column_bytes(sql_statement, 3));

    int numberOfJumpCollections = 0; //TODO: delete this
    int numberOfJumpDirections = 0; //TODO: delete this
    new_Span(transitionModel->JumpCollections, (size_t) numberOfJumpCollections);
    new_Span(transitionModel->JumpDirections, (size_t) numberOfJumpDirections);

    CHECK_SQL(sqlite3_finalize(sql_statement), SQLITE_OK);
    return SQLITE_OK;
}

int AssignJumpCollections(char* sql_query, sqlite3 *db, void *obj, const struct ProjectIds *projectIds)
{
    sqlite3_stmt *sql_statement = NULL;
    CHECK_SQL(PrepareSqlStatement(sql_query, db, &sql_statement, projectIds->TransitionId), SQLITE_ROW)

    JumpCollections_t *jumpCollections = (JumpCollections_t*) obj;


    cpp_foreach(current, *jumpCollections)
    {
        current->ObjectId = sqlite3_column_int(sql_statement, 1);
        current->MobileParticlesMask = sqlite3_column_int64(sql_statement, 2);
        memcpy(&current->JumpRules, sqlite3_column_blob(sql_statement, 3),
                (size_t) sqlite3_column_bytes(sql_statement, 3));

        CHECK_SQL(sqlite3_step(sql_statement), SQLITE_ROW)
    }

    CHECK_SQL(sqlite3_finalize(sql_statement), SQLITE_OK);

    return SQLITE_OK;
}

int AssignJumpDirections(char* sql_query, sqlite3 *db, void *obj, const struct ProjectIds *projectIds)
{
    sqlite3_stmt *sql_statement = NULL;
    CHECK_SQL(PrepareSqlStatement(sql_query, db, &sql_statement, projectIds->TransitionId), SQLITE_ROW)

    JumpDirections_t *jumpDirections = (JumpDirections_t*) obj;

    cpp_foreach(current, *jumpDirections)
    {
        current->ObjectId = sqlite3_column_int(sql_statement, 0);
        current->JumpCollectionId = sqlite3_column_int(sql_statement, 1);
        current->ElectricFieldFactor = sqlite3_column_double(sql_statement, 2);
        current->JumpLength = sqlite3_column_int(sql_statement, 3);
        current->PositionId = sqlite3_column_int(sql_statement, 4);

        memcpy(&current->JumpSequence, sqlite3_column_blob(sql_statement, 5),
                (size_t) sqlite3_column_bytes(sql_statement, 5));
        memcpy(&current->LocalMoveSequence, sqlite3_column_blob(sql_statement, 8),
                (size_t) sqlite3_column_bytes(sql_statement, 6));


        CHECK_SQL(sqlite3_step(sql_statement), SQLITE_ROW)
    }

    CHECK_SQL(sqlite3_finalize(sql_statement), SQLITE_OK);

    return SQLITE_OK;
}

int AssignLatticeModel(char* sql_query, sqlite3 *db, void *obj, const struct ProjectIds *projectIds)
{
    sqlite3_stmt *sql_statement = NULL;
    CHECK_SQL(PrepareSqlStatement(sql_query, db, &sql_statement, projectIds->ProjectId), SQLITE_ROW)

    LatticeModel_t *latticeModel = (LatticeModel_t*) obj;

    latticeModel->NumOfMobiles = sqlite3_column_int(sql_statement, 0);
    latticeModel->NumOfSelectables = sqlite3_column_int(sql_statement, 1);
    memcpy(&latticeModel->EnergyBackground, sqlite3_column_blob(sql_statement, 2),
            (size_t) sqlite3_column_bytes(sql_statement, 2));
    memcpy(&latticeModel->Lattice, sqlite3_column_blob(sql_statement, 3),
            (size_t) sqlite3_column_bytes(sql_statement, 3));
    memcpy(&latticeModel->Latt, sqlite3_column_blob(sql_statement, 4),
            (size_t) sqlite3_column_bytes(sql_statement, 4));

    CHECK_SQL(sqlite3_finalize(sql_statement), SQLITE_OK);

    return SQLITE_OK;
}

int DistributeJumpDirections(DbModel_t* dbModel)
{
    int span_begin = 0;
    int span_end = 0;

    int currentCollectionID = 0;

    cpp_foreach(iter, dbModel->TransitionModel.JumpDirections)
    {
        if (iter->JumpCollectionId != currentCollectionID)
        {
            span_end--;

            span_Get(dbModel->TransitionModel.JumpCollections, currentCollectionID).JumpDirections = (JumpDirections_t)
                    span_Split(dbModel->TransitionModel.JumpDirections, span_begin, span_end);

            span_end++;
            span_begin = span_end;
            currentCollectionID = iter->JumpCollectionId;
        }
        span_end++;
    }

    span_end--;
    span_Get(dbModel->TransitionModel.JumpCollections, currentCollectionID).JumpDirections = (JumpDirections_t)
            span_Split(dbModel->TransitionModel.JumpDirections, span_begin, span_end);

    return 0;
}

int AssignDatabaseModel(DbModel_t* dbModel, char* dbFile, int projectNumber)
{
    struct ProjectIds projectIds = {
            .ProjectId = projectNumber,
            .StructureId = -1,
            .EnergyId = -1,
            .TransitionId = -1,
            .LatticeId = -1
    };

    sqlite3 *db;
    CHECK_SQL(sqlite3_open(dbFile, &db), SQLITE_OK)

    CHECK_SQL(AssignProjectIds(db, &projectIds), SQLITE_OK)
    CHECK_SQL(AssignParentObjects(dbModel, db, &projectIds), SQLITE_OK)
    CHECK_SQL(AssignChildObjects(dbModel, db, &projectIds), SQLITE_OK)

    DistributeJumpDirections(dbModel);

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
    char* sql_query = malloc(sizeof(char)*500);
    CHECK_SQL(GetSqlQuery("test", db, sql_query),0)

    sqlite3_stmt *sql_statement = NULL;
    CHECK_SQL(PrepareSqlStatement(sql_query, db, &sql_statement, 0), SQLITE_ROW)
    printf(sqlite3_column_text(sql_statement, 0));
    printf("\n");

    sqlite3_finalize(sql_statement);
    sqlite3_close(db);
    free(sql_query);

    return 0;
}