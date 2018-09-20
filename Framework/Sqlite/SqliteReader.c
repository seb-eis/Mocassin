//
// Created by john on 18.09.2018.
//

#include "SqliteReader.h"

int GetSqlQuery(char* key, const sqlite3 *db, char* sqlQuery)
{
    sqlite3_stmt *res = NULL;

    char* sql = GET_SQL_QUERY;
    CHECK_SQL(sqlite3_prepare_v2(db, sql, -1, &res, NULL), SQLITE_OK);
    CHECK_SQL(sqlite3_bind_text(res, 1, key, -1, SQLITE_STATIC), SQLITE_OK);
    sqlite3_step(res);
    strcpy(sqlQuery, (char *)(sqlite3_column_text(res, 0)));

    CHECK_SQL(sqlite3_finalize(res), SQLITE_OK);

    return 0;
}

int PrepareSQL(char* sql, const sqlite3 *db, sqlite3_stmt **res, int queryValue)
{
    CHECK_SQL(sqlite3_prepare_v2(db, sql, -1, &(*res), NULL), SQLITE_OK);
    CHECK_SQL(sqlite3_bind_int(*res, POSITION_OF_QUERY_VALUE, queryValue), SQLITE_OK);
    CHECK_SQL(sqlite3_step(*res), SQLITE_ROW);
    return SQLITE_ROW;
}


int AssignDbBlob(char* sql, sqlite3 *db, void *obj, int queryValue)
{
    sqlite3_stmt *res = NULL;
    CHECK_SQL(PrepareSQL(sql, db, &res, queryValue), SQLITE_ROW)

    DbBlob_t *dbBlob = (DbBlob_t*) obj;
    dbBlob->Key = sqlite3_column_int(res, 0);
    dbBlob->BlobSize = sqlite3_column_int(res, 1);
    dbBlob->HeaderSize = sqlite3_column_int(res, 2);

    CHECK_SQL(sqlite3_finalize(res), SQLITE_OK);
    return SQLITE_OK;
}

int AssignProjectIds(const sqlite3 *db, struct ProjectIds* projectIds)
{
    char *sql = malloc(sizeof(char) * MAX_LINE_LENGTH);

    char *keywords[NUMBER_OF_PARENT_OBJECTS] = PROJECT_ID_KEYWORDS;
    int *ids[NUMBER_OF_PARENT_OBJECTS] = PROJECT_IDS(&(*projectIds));

    for (int opId = 0; opId < NUMBER_OF_PARENT_OBJECTS; opId++)
    {
        CHECK_SQL(GetSqlQuery(keywords[opId], db, sql), 0);
        CHECK_SQL(GetIdFromDatabase(sql, db, ids[opId]), SQLITE_OK);
    }

    return SQLITE_OK;
}

int GetIdFromDatabase(char* sql, const sqlite3 *db, int* id)
{
    sqlite3_stmt *res = NULL;
    CHECK_SQL(sqlite3_prepare_v2(db, sql, -1, &res, NULL), SQLITE_OK);
    CHECK_SQL(sqlite3_step(res), SQLITE_ROW);

    *id = sqlite3_column_int(res, 0);

    CHECK_SQL(sqlite3_finalize(res), SQLITE_OK);
    return SQLITE_OK;
}


int AssignParentObjects(DbModel_t *dbModel, const sqlite3 *db, const struct ProjectIds* ids)
{
    char *sql = malloc(sizeof(char) * MAX_LINE_LENGTH);

    int (*operations[NUMBER_OF_PARENT_OBJECTS]) (char*, const sqlite3*, void*, const struct ProjectIds*) = PARENT_OPERATIONS;
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


int AssignStructureModel(char* sql, const sqlite3* db, void* obj, const struct ProjectIds* projectIds)
{
    sqlite3_stmt *res = NULL;
    CHECK_SQL(PrepareSQL(sql, db, &res, projectIds->ProjectId), SQLITE_ROW)

    StructureModel_t *structureModel = (StructureModel_t*) obj;
    structureModel->NumOfTrackersPerCell = sqlite3_column_int(res, 0);
    structureModel->NumOfGlobalTrackers = sqlite3_column_int(res, 1);
    int numberOfEnvironments;
    new_Span(structureModel->EnvironmentDefinitions, numberOfEnvironments);

    CHECK_SQL(sqlite3_finalize(res), SQLITE_OK);
    return SQLITE_OK;
}


int AssignEnergyModel(char* sql, const sqlite3 *db, void *obj, const struct ProjectIds *projectIds)
{
    sqlite3_stmt *res = NULL;
    CHECK_SQL(PrepareSQL(sql, db, &res, projectIds->ProjectId), SQLITE_ROW)

    EnergyModel_t *energyModel = (EnergyModel_t*) obj;
    int numberOfPairTables = sqlite3_column_int(res, 0);
    int numberOfClusterTables = sqlite3_column_int(res, 1);
    new_Span(energyModel->PairTables, numberOfPairTables);
    new_Span(energyModel->PairTables, numberOfClusterTables);

    CHECK_SQL(sqlite3_finalize(res), SQLITE_OK);
    return SQLITE_OK;
}

int AssignChildObjects(DbModel_t *dbModel, const sqlite3 *db, const struct ProjectIds *projectIds)
{
    char *sql = malloc(sizeof(char) * MAX_LINE_LENGTH);

    int (*operations[NUMBER_OF_CHILD_OBJECTS]) (char*, const sqlite3*, void*, const struct ProjectIds*) = CHILD_OPERATIONS;
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


int AssignEnvironmentDefinitions(char* sql, const sqlite3 *db, void *obj, const struct ProjectIds *projectIds)
{
    sqlite3_stmt *res = NULL;
    CHECK_SQL(PrepareSQL(sql, db, &res, projectIds->StructureId), SQLITE_ROW)

    EnvironmentDefinitions_t *envDefs = (EnvironmentDefinitions_t*) obj;
    EnvironmentDefinition_t* currentEnvDef = envDefs->Begin;

    do{
        currentEnvDef->ObjId = sqlite3_column_int(res, 0);
        memcpy(currentEnvDef->PositionParticleIds, sqlite3_column_blob(res, 1), sqlite3_column_bytes(res, 1));
        memcpy(currentEnvDef->UpdateParticleIds, sqlite3_column_blob(res, 2), sqlite3_column_bytes(res, 2));
        memcpy(&currentEnvDef->PairDefinitions, sqlite3_column_blob(res, 3), sqlite3_column_bytes(res, 3));
        memcpy(&currentEnvDef->ClusterDefinitions, sqlite3_column_blob(res, 4), sqlite3_column_bytes(res,4));

        if (currentEnvDef == envDefs->End)
        {
            return -1;
        }
        currentEnvDef++;
    }while(sqlite3_step(res) == SQLITE_ROW);

    CHECK_SQL(sqlite3_finalize(res), SQLITE_OK);

    return SQLITE_OK;
}

int AssignPairTables(char* sql, const sqlite3 *db, void *obj, const struct ProjectIds *projectIds)
{
    sqlite3_stmt *res = NULL;
    CHECK_SQL(PrepareSQL(sql, db, &res, projectIds->StructureId), SQLITE_ROW)

    EnvironmentDefinitions_t *envDefs = (DbBlob_t*) obj;
    EnvironmentDefinition_t* currentEnvDef = envDefs->Begin;
    do
    {
        currentEnvDef->ObjId = sqlite3_column_int(res, 0);
        memcpy(currentEnvDef->PositionParticleIds, sqlite3_column_blob(res, 1), sqlite3_column_bytes(res, 1));
        memcpy(currentEnvDef->UpdateParticleIds, sqlite3_column_blob(res, 2), sqlite3_column_bytes(res, 2));
        memcpy(&currentEnvDef->PairDefinitions, sqlite3_column_blob(res, 3), sqlite3_column_bytes(res, 3));
        memcpy(&currentEnvDef->ClusterDefinitions, sqlite3_column_blob(res, 4), sqlite3_column_bytes(res,4));

        if (currentEnvDef == envDefs->End)
        {
            return -1;
        }
        currentEnvDef++;
    }while(sqlite3_step(res) == SQLITE_ROW);

    CHECK_SQL(sqlite3_finalize(res), SQLITE_OK);

    return SQLITE_OK;
}


int AssignDbModel(DbModel_t* dbModel, char* dbFile, char* sqlFile, int projectNumber)
{
    struct ProjectIds projectIds = {
            .ProjectId = projectNumber,
            .StructureId = -1,
            .EnergyId = -1,
            .TransitionId = -1,
            .LatticeId = -1
    };

    sqlite3 *db;
    CHECK_SQL(sqlite3_open(dbFile, &db), SQLITE_OK);

    CHECK_SQL(AssignProjectIds(db, &projectIds), SQLITE_OK);
    CHECK_SQL(AssignParentObjects(dbModel, db, &projectIds), SQLITE_OK);
    CHECK_SQL(AssignChildObjects(dbModel, db, &projectIds), SQLITE_OK);


}

