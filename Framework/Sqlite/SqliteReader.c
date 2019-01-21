//
// Created by john on 18.09.2018.
//

#include <Simulator/Data/Database/DbModel.h>
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
    char *sqlQuery = "select StructureModelId, EnergyModelId, TransitionModelId, LatticeModelId "
                     "from JobModels where PackageId = ?1";

    sqlite3_stmt *sql_statement = NULL;
    CHECK_SQL(PrepareSqlStatement(sqlQuery, db, &sql_statement, projectIds->ProjectId), SQLITE_ROW)
    projectIds->StructureId = sqlite3_column_int(sql_statement, 0);
    projectIds->EnergyId = sqlite3_column_int(sql_statement, 1);
    projectIds->TransitionId = sqlite3_column_int(sql_statement, 2);
    projectIds->LatticeId = sqlite3_column_int(sql_statement, 3);

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
    sql_query = "";

    int (*operations[NUMBER_OF_PARENT_OBJECTS]) (char*, sqlite3*, void*, const struct ProjectIds*) = PARENT_OPERATIONS;
    void *objects[NUMBER_OF_PARENT_OBJECTS] = PARENT_OBJECTS(&(*dbModel));
    char *keywords[NUMBER_OF_PARENT_OBJECTS] = PARENT_KEYWORDS;

    for (int opId = 0; opId < NUMBER_OF_PARENT_OBJECTS; opId++)
    {
        //CHECK_SQL(GetSqlQuery(keywords[opId], db, sql_query), 0);
        CHECK_SQL(operations[opId](sql_query, db, objects[opId], ids), SQLITE_OK);
    }

    free(sql_query);

    return SQLITE_OK;
}


int AssignStructureModel(char* sql_query, sqlite3* db, void* obj, const struct ProjectIds* projectIds)
{
    sqlite3_stmt *sql_statement = NULL;
    sql_query = "select NumOfTrackersPerCell, NumOfGlobalTrackers, InteractionRange, NumOfEnvironmentDefinitions "
                "from StructureModels where Id = ?1";
    CHECK_SQL(PrepareSqlStatement(sql_query, db, &sql_statement, projectIds->StructureId), SQLITE_ROW)

    StructureModel_t *structureModel = (StructureModel_t*) obj;
    structureModel->NumOfTrackersPerCell = sqlite3_column_int(sql_statement, 0);
    structureModel->NumOfGlobalTrackers = sqlite3_column_int(sql_statement, 1);
    memcpy(&structureModel->InteractionRange, sqlite3_column_blob(sql_statement, 2),
            (size_t) sqlite3_column_bytes(sql_statement, 2));
    int numberOfEnvironments = sqlite3_column_int(sql_statement, 3);
    new_Span(structureModel->EnvironmentDefinitions, (size_t) numberOfEnvironments);

    CHECK_SQL(sqlite3_finalize(sql_statement), SQLITE_OK);
    return SQLITE_OK;
}


int AssignEnergyModel(char* sqlQuery, sqlite3 *db, void *obj, const struct ProjectIds *projectIds)
{
    sqlite3_stmt *sqlStatement = NULL;
    sqlQuery = "select NumOfPairTables, NumOfClusterTables from EnergyModels where Id = ?1";
    CHECK_SQL(PrepareSqlStatement(sqlQuery, db, &sqlStatement, projectIds->ProjectId), SQLITE_ROW)

    EnergyModel_t *energyModel = (EnergyModel_t*) obj;
    int numberOfPairTables = sqlite3_column_int(sqlStatement, 0);
    int numberOfClusterTables = sqlite3_column_int(sqlStatement, 1);
    new_Span(energyModel->PairTables, (size_t) numberOfPairTables);
    new_Span(energyModel->PairTables, (size_t) numberOfClusterTables);

    CHECK_SQL(sqlite3_finalize(sqlStatement), SQLITE_OK);
    return SQLITE_OK;
}


int AssignChildObjects(DbModel_t *dbModel, sqlite3 *db, const struct ProjectIds *projectIds)
{
    char *sql_query = malloc(sizeof(char) * MAX_QUERY_LENGTH);
    sql_query = "";

    int (*operations[NUMBER_OF_CHILD_OBJECTS]) (char*, sqlite3*, void*, const struct ProjectIds*) = CHILD_OPERATIONS;
    void *objects[NUMBER_OF_CHILD_OBJECTS] = CHILD_OBJECTS(&(*dbModel));
    char *keywords[NUMBER_OF_CHILD_OBJECTS] = CHILD_KEYWORDS;

    for (int opId = 0; opId < NUMBER_OF_CHILD_OBJECTS; opId++)
    {
        //CHECK_SQL(GetSqlQuery(keywords[opId], db, sql_query), 0);
        CHECK_SQL(operations[opId](sql_query, db, objects[opId], projectIds), SQLITE_OK);
    }

    free(sql_query);

    return SQLITE_OK;
}


int AssignEnvironmentDefinitions(char* sql_query, sqlite3 *db, void *obj, const struct ProjectIds *projectIds)
{
    sqlite3_stmt *sql_statement = NULL;
    sql_query = "select ObjectId, SelectionMask, UpdateParticleIds, PairDefinitions, ClusterDefinitions, "
                "PositionParticleIds from EnvironmentDefinitions where StructureModelId = ?1 order by ObjectId";
    CHECK_SQL(PrepareSqlStatement(sql_query, db, &sql_statement, projectIds->StructureId), SQLITE_ROW)

    EnvironmentDefinitions_t *environmentDefinitions = (EnvironmentDefinitions_t*) obj;

    unsigned long numberOfEnvironments = span_GetSize(*environmentDefinitions);
    for(int j=0; j<numberOfEnvironments; j++)
    {
        EnvironmentDefinition_t* current = &span_Get(*environmentDefinitions, j);
        current->ObjectId = sqlite3_column_int(sql_statement, 0);
        current->SelectionParticleMask = sqlite3_column_int(sql_statement, 1);
        memcpy(current->UpdateParticleIds, sqlite3_column_blob(sql_statement, 2),
               (size_t) sqlite3_column_bytes(sql_statement, 2));

        size_t numberOfPairDefinitions = sqlite3_column_bytes(sql_statement, 3) / sizeof(PairDefinition_t);
        new_Span(current->PairDefinitions, numberOfPairDefinitions);
        memcpy(current->PairDefinitions.Begin, sqlite3_column_blob(sql_statement, 3),
               (size_t) sqlite3_column_bytes(sql_statement, 3));

        size_t numberOfClusterDefinitions = sqlite3_column_bytes(sql_statement, 4) / sizeof(ClusterDefinition_t);
        new_Span(current->ClusterDefinitions, numberOfClusterDefinitions);
        memcpy(current->ClusterDefinitions.Begin, sqlite3_column_blob(sql_statement, 4),
               (size_t) sqlite3_column_bytes(sql_statement,4));

        memcpy(current->PositionParticleIds, sqlite3_column_blob(sql_statement, 5),
               (size_t) sqlite3_column_bytes(sql_statement, 5));

        if (j < (numberOfEnvironments - 1))
        {
            CHECK_SQL(sqlite3_step(sql_statement), SQLITE_ROW);
        }
    }

    CHECK_SQL(sqlite3_finalize(sql_statement), SQLITE_OK);

    return SQLITE_OK;
}

int AssignPairEnergyTables(char* sql_query, sqlite3 *db, void *obj, const struct ProjectIds *projectIds)
{
    sqlite3_stmt *sql_statement = NULL;
    sql_query = "select ObjectId, EnergyTable "
                "from PairEnergyTables where EnergyModelId = ?1 order by ObjectId";
    CHECK_SQL(PrepareSqlStatement(sql_query, db, &sql_statement, projectIds->EnergyId), SQLITE_ROW)

    PairTables_t *pairTables = (PairTables_t*) obj;
    unsigned long numberOfPairTables = span_GetSize(*pairTables);
    for (int j = 0; j < numberOfPairTables; j++)
    {
        PairTable_t* current = &span_Get(*pairTables, j);
        current->ObjectId = sqlite3_column_int(sql_statement, 0);

        size_t numberOfEnergyTables = sqlite3_column_bytes(sql_statement, 1) / sizeof(EnergyTable_t);
        new_Span(current->EnergyTable, numberOfEnergyTables);
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
    sql_query = "select ObjectId, EnergyTable, OccupationCodes, TableIndexing from ClusterEnergyTables "
                "where EnergyModelId = ?1 order by ObjectId";
    CHECK_SQL(PrepareSqlStatement(sql_query, db, &sql_statement, projectIds->EnergyId), SQLITE_ROW)

    ClusterTables_t *clusterTables = (ClusterTables_t*) obj;

    unsigned long numberOfClusterTables = span_GetSize(*clusterTables);
    for (int j = 0; j < numberOfClusterTables; j++)
    {
        ClusterTable_t* current = &span_Get(*clusterTables, j);

        current->ObjectId = sqlite3_column_int(sql_statement, 0);

        size_t numberOfEnergyTables = sqlite3_column_bytes(sql_statement, 1) / sizeof(EnergyTable_t);
        new_Span(current->EnergyTable, numberOfEnergyTables);
        memcpy(current->EnergyTable.Begin, sqlite3_column_blob(sql_statement, 1),
               (size_t) sqlite3_column_bytes(sql_statement, 1));

        size_t numberOfOccupationCodes = sqlite3_column_bytes(sql_statement, 2) / sizeof(OccCodes_t);
        new_Span(current->OccupationCodes, numberOfOccupationCodes);
        memcpy(current->OccupationCodes.Begin, sqlite3_column_blob(sql_statement, 2),
               (size_t) sqlite3_column_bytes(sql_statement, 2));

        memcpy(current->ParticleTableMapping, sqlite3_column_blob(sql_statement, 3),
               (size_t) sqlite3_column_bytes(sql_statement, 3));
        CHECK_SQL(sqlite3_step(sql_statement), SQLITE_ROW)
    }

    CHECK_SQL(sqlite3_finalize(sql_statement), SQLITE_OK);

    return SQLITE_OK;
}


int AssignTransitionModel(char* sql_query, sqlite3 *db, void *obj, const struct ProjectIds *projectIds)
{
    sqlite3_stmt *sql_statement = NULL;
    sql_query = "select JumpMappingTable, JumpCountTable, StaticTrackerMapping, GlobalTrackerMapping "
                "from TransitionModels where Id = ?1";
    CHECK_SQL(PrepareSqlStatement(sql_query, db, &sql_statement, projectIds->ProjectId), SQLITE_ROW)

    TransitionModel_t *transitionModel = (TransitionModel_t*) obj;

    size_t numberOfJumpDirectionMappingTables = sqlite3_column_bytes(sql_statement, 0) / sizeof(JumpMappingTable_t);
    new_Span(transitionModel->JumpDirectionMappingTable, numberOfJumpDirectionMappingTables);
    memcpy(transitionModel->JumpDirectionMappingTable.Begin, sqlite3_column_blob(sql_statement, 0),
            (size_t) sqlite3_column_bytes(sql_statement, 0));

    size_t numberOfJumpCountMappingTables = sqlite3_column_bytes(sql_statement, 1) / sizeof(JumpCountTable_t);
    new_Span(transitionModel->JumpCountMappingTable, numberOfJumpCountMappingTables);
    memcpy(transitionModel->JumpCountMappingTable.Begin, sqlite3_column_blob(sql_statement, 1),
            (size_t) sqlite3_column_bytes(sql_statement, 1));

    size_t numberOfStaticTrackerMappingTables = sqlite3_column_bytes(sql_statement, 2) / sizeof(TrackerMappingTable_t);
    new_Span(transitionModel->StaticTrackerMappingTable, numberOfStaticTrackerMappingTables);
    memcpy(&transitionModel->StaticTrackerMappingTable, sqlite3_column_blob(sql_statement, 2),
           (size_t) sqlite3_column_bytes(sql_statement, 2));

    size_t numberOfGlobalTrackerMappingTables = sqlite3_column_bytes(sql_statement, 3) / sizeof(TrackerMappingTable_t);
    new_Span(transitionModel->GlobalTrackerMappingTable, numberOfGlobalTrackerMappingTables);
    memcpy(&transitionModel->GlobalTrackerMappingTable, sqlite3_column_blob(sql_statement, 3),
           (size_t) sqlite3_column_bytes(sql_statement, 3));

    int numberOfJumpCollections = sqlite3_column_int(sql_statement, 4);
    int numberOfJumpDirections = sqlite3_column_int(sql_statement, 5);
    new_Span(transitionModel->JumpCollections, (size_t) numberOfJumpCollections);
    new_Span(transitionModel->JumpDirections, (size_t) numberOfJumpDirections);

    CHECK_SQL(sqlite3_finalize(sql_statement), SQLITE_OK);
    return SQLITE_OK;
}

int AssignJumpCollections(char* sql_query, sqlite3 *db, void *obj, const struct ProjectIds *projectIds)
{
    sqlite3_stmt *sql_statement = NULL;
    sql_query = "select ObjectId, SelectionMask, JumpRules "
                "from JumpCollections where TransitionModelId = ?1 order by ObjectId";
    CHECK_SQL(PrepareSqlStatement(sql_query, db, &sql_statement, projectIds->TransitionId), SQLITE_ROW)

    JumpCollections_t *jumpCollections = (JumpCollections_t*) obj;

    unsigned long numberOfJumpCollections = span_GetSize(*jumpCollections);
    for (int j = 0; j < numberOfJumpCollections; j++)
    {
        JumpCollection_t *current = &span_Get(*jumpCollections, j);

        current->ObjectId = sqlite3_column_int(sql_statement, 1);
        current->MobileParticlesMask = sqlite3_column_int64(sql_statement, 2);

        size_t numberOfJumpRules = sqlite3_column_bytes(sql_statement, 3) / sizeof(JumpRule_t);
        new_Span(current->JumpRules, numberOfJumpRules);
        memcpy(current->JumpRules.Begin, sqlite3_column_blob(sql_statement, 3),
               (size_t) sqlite3_column_bytes(sql_statement, 3));

        // allocate space for JumpDirections
        {
            sqlite3_stmt *sqlStatement2 = NULL;
            char *sqlQuery2 = "select count(*) as count from JumpDirections where collectionID = ?1";
            CHECK_SQL(PrepareSqlStatement(sqlQuery2, db, &sqlStatement2, current->ObjectId), SQLITE_ROW)
            int numberOfJumpDirections = sqlite3_column_int(sqlStatement2, 0);
            new_Span(current->JumpDirections, (size_t) numberOfJumpDirections);
            CHECK_SQL(sqlite3_finalize(sqlStatement2), SQLITE_OK);
        }

        CHECK_SQL(sqlite3_step(sql_statement), SQLITE_ROW)
    }

    CHECK_SQL(sqlite3_finalize(sql_statement), SQLITE_OK);

    return SQLITE_OK;
}

int AssignJumpDirections(char* sql_query, sqlite3 *db, void *obj, const struct ProjectIds *projectIds)
{
    sqlite3_stmt *sql_statement = NULL;
    sql_query = "select ObjectId, PositionId, JumpLength, FieldProjection, JumpSequence, LocalMoveSequence "
                "from JumpDirections where TransitionModelId = ?1 order by ObjectId";
    CHECK_SQL(PrepareSqlStatement(sql_query, db, &sql_statement, projectIds->TransitionId), SQLITE_ROW)

    JumpDirections_t *jumpDirections = (JumpDirections_t*) obj;

    unsigned long numberOfJumpDirections = span_GetSize(*jumpDirections);
    for (int j = 0; j < numberOfJumpDirections; j++)
    {
        JumpDirection_t *current = &span_Get(*jumpDirections, j);

        current->ObjectId = sqlite3_column_int(sql_statement, 0);
        current->JumpCollectionId = sqlite3_column_int(sql_statement, 1);
        current->ElectricFieldFactor = sqlite3_column_double(sql_statement, 2);
        current->JumpLength = sqlite3_column_int(sql_statement, 3);
        current->PositionId = sqlite3_column_int(sql_statement, 4);

        size_t numberOfJumpSequences = sqlite3_column_bytes(sql_statement, 5) / sizeof(JumpSequence_t);
        new_Span(current->JumpSequence, numberOfJumpSequences);
        memcpy(current->JumpSequence.Begin, sqlite3_column_blob(sql_statement, 5),
               (size_t) sqlite3_column_bytes(sql_statement, 5));

        size_t numberOfLocalMoveSequences = sqlite3_column_bytes(sql_statement, 6) / sizeof(TrackerMappingTable_t);
        new_Span(current->LocalMoveSequence, numberOfLocalMoveSequences);
        memcpy(current->LocalMoveSequence.Begin, sqlite3_column_blob(sql_statement, 6),
               (size_t) sqlite3_column_bytes(sql_statement, 6));


        CHECK_SQL(sqlite3_step(sql_statement), SQLITE_ROW)
    }

    CHECK_SQL(sqlite3_finalize(sql_statement), SQLITE_OK);

    return SQLITE_OK;
}

int AssignLatticeModel(char* sql_query, sqlite3 *db, void *obj, const struct ProjectIds *projectIds)
{
    sqlite3_stmt *sql_statement = NULL;
    sql_query = "select EnergyBackground, Lattice, LatticeInfo from LatticeModels where Id = ?1";
    CHECK_SQL(PrepareSqlStatement(sql_query, db, &sql_statement, projectIds->LatticeId), SQLITE_ROW)

    LatticeModel_t *latticeModel = (LatticeModel_t*) obj;

    size_t numberOfEnergyBackgrounds = sqlite3_column_bytes(sql_statement, 0) / sizeof(EnergyBackground_t);
    new_Span(latticeModel->EnergyBackground, numberOfEnergyBackgrounds);
    memcpy(latticeModel->EnergyBackground.Begin, sqlite3_column_blob(sql_statement, 0),
            (size_t) sqlite3_column_bytes(sql_statement, 0));


    memcpy(&latticeModel->LatticeInfo, sqlite3_column_blob(sql_statement, 2),
           (size_t) sqlite3_column_bytes(sql_statement, 2));


    new_Array(latticeModel->Lattice, latticeModel->LatticeInfo.SizeVector.a, latticeModel->LatticeInfo.SizeVector.b,
              latticeModel->LatticeInfo.SizeVector.c, latticeModel->LatticeInfo.SizeVector.d);
    memcpy(latticeModel->Lattice.Header, sqlite3_column_blob(sql_statement, 1),
            (size_t) sqlite3_column_bytes(sql_statement, 1));


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
            .LatticeId = -1,
    };

    sqlite3 *db;
    CHECK_SQL(sqlite3_open(dbFile, &db), SQLITE_OK)

    CHECK_SQL(AssignProjectIds(db, &projectIds), SQLITE_OK)
    CHECK_SQL(AssignParentObjects(dbModel, db, &projectIds), SQLITE_OK)
    CHECK_SQL(AssignChildObjects(dbModel, db, &projectIds), SQLITE_OK)

    DistributeJumpDirections(dbModel);

    return 0;
}