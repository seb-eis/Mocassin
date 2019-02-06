//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	SqliteReader.c              //
// Author:	John Arnold 			    //
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 John Arnold          //
// Short:   Db sqlite reader interface  //
//////////////////////////////////////////

#include <Simulator/Data/Database/DbModel.h>
#include "SqliteReader.h"

static int32_t PrepareSqlStatement(char *sqlQuery, sqlite3 *db, sqlite3_stmt **sqlStatement, int32_t contextId)
{
    check_Sql(sqlite3_prepare_v2(db, sqlQuery, -1, &(*sqlStatement), NULL), SQLITE_OK);
    check_Sql(sqlite3_bind_int(*sqlStatement, ID_POS_IN_SQLSTMT, contextId), SQLITE_OK);
    check_Sql(sqlite3_step(*sqlStatement), SQLITE_ROW);
    return SQLITE_ROW;
}


static int32_t AssignProjectIds(sqlite3 *db, DbLoadIndices_t* loadIndices)
{
    char *sqlQuery = "select StructureModelId, EnergyModelId, TransitionModelId, LatticeModelId, PackageId "
                     "from JobModels where Id = ?1";

    sqlite3_stmt *sqlStatement = NULL;
    check_Sql(PrepareSqlStatement(sqlQuery, db, &sqlStatement, loadIndices->PackageContextId), SQLITE_ROW)

    loadIndices->StructureContextId = sqlite3_column_int(sqlStatement, 0);
    loadIndices->EnergyContextId = sqlite3_column_int(sqlStatement, 1);
    loadIndices->TransitionContextId = sqlite3_column_int(sqlStatement, 2);
    loadIndices->LatticeContextId = sqlite3_column_int(sqlStatement, 3);
    loadIndices->PackageContextId = sqlite3_column_int(sqlStatement, 4);

    return SQLITE_OK;
}

static int32_t AssignParentObjects(DbModel_t *dbModel, sqlite3 *db, const DbLoadIndices_t* loadIndices)
{
    ObjectOperationSet_t operationSet = GetParentOperationSet(dbModel);

    cpp_foreach (item, operationSet)
    {
        check_Sql(item->Operation("", db, item->Object, loadIndices), SQLITE_OK);
    }

    delete_Span(operationSet);
    return SQLITE_OK;
}


static int32_t AssignStructureModel(char* sqlQuery, sqlite3* db, void* obj, const DbLoadIndices_t* loadIndices)
{
    sqlite3_stmt *sqlStatement = NULL;
    sqlQuery = "select NumOfTrackersPerCell, NumOfGlobalTrackers, InteractionRange, NumOfEnvironmentDefinitions "
                "from StructureModels where Id = ?1";

    check_Sql(PrepareSqlStatement(sqlQuery, db, &sqlStatement, loadIndices->StructureContextId), SQLITE_ROW)

    StructureModel_t *structureModel = (StructureModel_t*) obj;

    structureModel->NumOfTrackersPerCell = sqlite3_column_int(sqlStatement, 0);
    structureModel->NumOfGlobalTrackers = sqlite3_column_int(sqlStatement, 1);
    memcpy(&structureModel->InteractionRange, sqlite3_column_blob(sqlStatement, 2),(size_t) sqlite3_column_bytes(sqlStatement, 2));

    int32_t numberOfEnvironments = sqlite3_column_int(sqlStatement, 3);
    structureModel->EnvironmentDefinitions = new_Span(structureModel->EnvironmentDefinitions, (size_t) numberOfEnvironments);

    check_Sql(sqlite3_finalize(sqlStatement), SQLITE_OK);
    return SQLITE_OK;
}


static int32_t AssignEnergyModel(char* sqlQuery, sqlite3 *db, void *obj, const DbLoadIndices_t *loadIndices)
{
    sqlite3_stmt *sqlStatement = NULL;
    sqlQuery = "select NumOfPairTables, NumOfClusterTables from EnergyModels where Id = ?1";

    check_Sql(PrepareSqlStatement(sqlQuery, db, &sqlStatement, loadIndices->PackageContextId), SQLITE_ROW)

    EnergyModel_t *energyModel = (EnergyModel_t*) obj;
    int32_t numberOfPairTables = sqlite3_column_int(sqlStatement, 0);
    int32_t numberOfClusterTables = sqlite3_column_int(sqlStatement, 1);
    energyModel->PairTables = new_Span(energyModel->PairTables, (size_t) numberOfPairTables);
    energyModel->ClusterTables = new_Span(energyModel->ClusterTables, (size_t) numberOfClusterTables);

    check_Sql(sqlite3_finalize(sqlStatement), SQLITE_OK);
    return SQLITE_OK;
}


static int32_t AssignChildObjects(DbModel_t *dbModel, sqlite3 *db, const DbLoadIndices_t *loadIndices)
{
    ObjectOperationSet_t operationSet = GetChildOperationSet(dbModel);

    cpp_foreach (item, operationSet)
    {
        check_Sql(item->Operation("", db, item->Object, loadIndices), SQLITE_OK);
    }

    delete_Span(operationSet);
    return SQLITE_OK;
}


static int32_t AssignEnvironmentDefinitions(char* sqlQuery, sqlite3 *db, void *obj, const DbLoadIndices_t *loadIndices)
{
    sqlite3_stmt *sqlStatement = NULL;
    sqlQuery = "select ObjectId, SelectionMask, UpdateParticleIds, PairDefinitions, ClusterDefinitions, "
                "PositionParticleIds from EnvironmentDefinitions where StructureModelId = ?1 order by ObjectId";

    check_Sql(PrepareSqlStatement(sqlQuery, db, &sqlStatement, loadIndices->StructureContextId), SQLITE_ROW)

    EnvironmentDefinitions_t *environmentDefinitions = (EnvironmentDefinitions_t*) obj;

    size_t numberOfEnvironments = span_GetSize(*environmentDefinitions);
    for(size_t j=0; j<numberOfEnvironments; j++)
    {
        EnvironmentDefinition_t* current = &span_Get(*environmentDefinitions, j);
        current->ObjectId = sqlite3_column_int(sqlStatement, 0);
        current->SelectionParticleMask = sqlite3_column_int(sqlStatement, 1);
        memcpy(current->UpdateParticleIds, sqlite3_column_blob(sqlStatement, 2), (size_t) sqlite3_column_bytes(sqlStatement, 2));

        size_t numberOfPairDefinitions = sqlite3_column_bytes(sqlStatement, 3) / sizeof(PairDefinition_t);
        current->PairDefinitions = span_FromBlob(current->PairDefinitions, sqlite3_column_blob(sqlStatement, 3), numberOfPairDefinitions);

        size_t numberOfClusterDefinitions = sqlite3_column_bytes(sqlStatement, 4) / sizeof(ClusterDefinition_t);
        current->ClusterDefinitions = span_FromBlob(current->ClusterDefinitions, sqlite3_column_blob(sqlStatement, 4), numberOfClusterDefinitions);

        memcpy(current->PositionParticleIds, sqlite3_column_blob(sqlStatement, 5), (size_t) sqlite3_column_bytes(sqlStatement, 5));

        if (j < (numberOfEnvironments - 1))
        {
            check_Sql(sqlite3_step(sqlStatement), SQLITE_ROW);
        }
    }

    check_Sql(sqlite3_finalize(sqlStatement), SQLITE_OK);

    return SQLITE_OK;
}

static int32_t AssignPairEnergyTables(char* sqlQuery, sqlite3 *db, void *obj, const DbLoadIndices_t *loadIndices)
{
    sqlite3_stmt *sqlStatement = NULL;
    sqlQuery = "select ObjectId, EnergyTable "
                "from PairEnergyTables where EnergyModelId = ?1 order by ObjectId";

    check_Sql(PrepareSqlStatement(sqlQuery, db, &sqlStatement, loadIndices->EnergyContextId), SQLITE_ROW)

    PairTables_t *pairTables = (PairTables_t*) obj;
    size_t numberOfPairTables = span_GetSize(*pairTables);
    for (size_t j = 0; j < numberOfPairTables; j++)
    {
        PairTable_t* current = &span_Get(*pairTables, j);
        current->ObjectId = sqlite3_column_int(sqlStatement, 0);

        current->EnergyTable = array_FromBlob(current->EnergyTable, sqlite3_column_blob(sqlStatement, 1));

        if (j < (numberOfPairTables - 1))
        {
            check_Sql(sqlite3_step(sqlStatement), SQLITE_ROW);
        }
    }
    check_Sql(sqlite3_step(sqlStatement), SQLITE_DONE);
    check_Sql(sqlite3_finalize(sqlStatement), SQLITE_OK);

    return SQLITE_OK;
}

static int32_t AssignClusterEnergyTables(char* sqlQuery, sqlite3 *db, void *obj, const DbLoadIndices_t *loadIndices)
{
    sqlite3_stmt *sqlStatement = NULL;
    sqlQuery = "select ObjectId, EnergyTable, OccupationCodes, TableIndexing from ClusterEnergyTables "
                "where EnergyModelId = ?1 order by ObjectId";

    check_Sql(PrepareSqlStatement(sqlQuery, db, &sqlStatement, loadIndices->EnergyContextId), SQLITE_ROW)

    ClusterTables_t *clusterTables = (ClusterTables_t*) obj;

    size_t numberOfClusterTables = span_GetSize(*clusterTables);
    for (size_t j = 0; j < numberOfClusterTables; j++)
    {
        ClusterTable_t* current = &span_Get(*clusterTables, j);

        current->ObjectId = sqlite3_column_int(sqlStatement, 0);

        current->EnergyTable = array_FromBlob(current->EnergyTable, sqlite3_column_blob(sqlStatement, 1));

        size_t numberOfOccupationCodes = sqlite3_column_bytes(sqlStatement, 2) / sizeof(OccCodes_t);
        current->OccupationCodes = span_FromBlob(current->OccupationCodes, sqlite3_column_blob(sqlStatement, 2), numberOfOccupationCodes);

        memcpy(current->ParticleTableMapping, sqlite3_column_blob(sqlStatement, 3), (size_t) sqlite3_column_bytes(sqlStatement, 3));

        if (j < (numberOfClusterTables - 1))
        {
            check_Sql(sqlite3_step(sqlStatement), SQLITE_ROW);
        }
    }
    check_Sql(sqlite3_step(sqlStatement), SQLITE_DONE);
    check_Sql(sqlite3_finalize(sqlStatement), SQLITE_OK);

    return SQLITE_OK;
}


static int32_t AssignTransitionModel(char* sqlQuery, sqlite3 *db, void *obj, const DbLoadIndices_t *loadIndices)
{
    sqlite3_stmt *sqlStatement = NULL;
    sqlQuery = "select JumpMappingTable, JumpCountTable, StaticTrackerMapping, GlobalTrackerMapping, NumOfCollections, NumOfDirections "
                "from TransitionModels where Id = ?1";

    check_Sql(PrepareSqlStatement(sqlQuery, db, &sqlStatement, loadIndices->PackageContextId), SQLITE_ROW)

    TransitionModel_t *transitionModel = obj;
    transitionModel->JumpDirectionMappingTable = array_FromBlob(transitionModel->JumpDirectionMappingTable, sqlite3_column_blob(sqlStatement, 0));
    transitionModel->JumpCountMappingTable = array_FromBlob(transitionModel->JumpCountMappingTable, sqlite3_column_blob(sqlStatement, 1));
    transitionModel->StaticTrackerMappingTable = array_FromBlob(transitionModel->StaticTrackerMappingTable, sqlite3_column_blob(sqlStatement, 2));
    transitionModel->GlobalTrackerMappingTable = array_FromBlob(transitionModel->GlobalTrackerMappingTable, sqlite3_column_blob(sqlStatement, 3));

    int32_t numberOfJumpCollections = sqlite3_column_int(sqlStatement, 4);
    int32_t numberOfJumpDirections = sqlite3_column_int(sqlStatement, 5);
    transitionModel->JumpCollections = new_Span(transitionModel->JumpCollections, (size_t) numberOfJumpCollections);
    transitionModel->JumpDirections = new_Span(transitionModel->JumpDirections, (size_t) numberOfJumpDirections);

    check_Sql(sqlite3_finalize(sqlStatement), SQLITE_OK);
    return SQLITE_OK;
}

static int32_t AssignJumpCollections(char* sqlQuery, sqlite3 *db, void *obj, const DbLoadIndices_t *loadIndices)
{
    sqlite3_stmt *sqlStatement = NULL;
    sqlQuery = "select ObjectId, SelectionMask, JumpRules "
                "from JumpCollections where TransitionModelId = ?1 order by ObjectId";

    check_Sql(PrepareSqlStatement(sqlQuery, db, &sqlStatement, loadIndices->TransitionContextId), SQLITE_ROW)

    JumpCollections_t *jumpCollections = (JumpCollections_t*) obj;

    size_t numberOfJumpCollections = span_GetSize(*jumpCollections);
    for (size_t j = 0; j < numberOfJumpCollections; j++)
    {

        JumpCollection_t *current = &span_Get(*jumpCollections, j);

        current->ObjectId = sqlite3_column_int(sqlStatement, 0);
        current->MobileParticlesMask = sqlite3_column_int64(sqlStatement, 1);

        size_t numberOfJumpRules = sqlite3_column_bytes(sqlStatement, 2) / sizeof(JumpRule_t);

        current->JumpRules = span_FromBlob(current->JumpRules, sqlite3_column_blob(sqlStatement, 2), numberOfJumpRules);

         if (j < (numberOfJumpCollections - 1))
        {
            check_Sql(sqlite3_step(sqlStatement), SQLITE_ROW);
        }


    }
    check_Sql(sqlite3_step(sqlStatement), SQLITE_DONE);
    check_Sql(sqlite3_finalize(sqlStatement), SQLITE_OK);

    return SQLITE_OK;
}

static int32_t AssignJumpDirections(char* sqlQuery, sqlite3 *db, void *obj, const DbLoadIndices_t *loadIndices)
{

    sqlite3_stmt *sqlStatement = NULL;
    sqlQuery = "select ObjectId, PositionId, JumpLength, FieldProjection, CollectionId, JumpSequence, LocalMoveSequence "
                "from JumpDirections where TransitionModelId = ?1 order by ObjectId";
    check_Sql(PrepareSqlStatement(sqlQuery, db, &sqlStatement, loadIndices->TransitionContextId), SQLITE_ROW)

    JumpDirections_t *jumpDirections =  obj;

    size_t numberOfJumpDirections = span_GetSize(*jumpDirections);
    for (size_t j = 0; j < numberOfJumpDirections; j++)
    {

        JumpDirection_t *current = &span_Get(*jumpDirections, j);

        current->ObjectId = sqlite3_column_int(sqlStatement, 0);
        current->PositionId = sqlite3_column_int(sqlStatement, 1);
        current->JumpLength = sqlite3_column_int(sqlStatement, 2);
        current->ElectricFieldFactor = sqlite3_column_double(sqlStatement, 3);
        current->JumpCollectionId = sqlite3_column_int(sqlStatement, 4);

        size_t numberOfJumpSequences = sqlite3_column_bytes(sqlStatement, 5) / sizeof(JumpSequence_t);
        current->JumpSequence = span_FromBlob(current->JumpSequence, sqlite3_column_blob(sqlStatement, 5), numberOfJumpSequences);

        size_t numberOfLocalMoveSequences = sqlite3_column_bytes(sqlStatement, 6) / sizeof(MoveSequence_t);
        current->MovementSequence = span_FromBlob(current->MovementSequence, sqlite3_column_blob(sqlStatement, 6), numberOfLocalMoveSequences);

        if (j < (numberOfJumpDirections - 1))
        {
            check_Sql(sqlite3_step(sqlStatement), SQLITE_ROW);
        }

    }
    check_Sql(sqlite3_step(sqlStatement), SQLITE_DONE);
    check_Sql(sqlite3_finalize(sqlStatement), SQLITE_OK);

    return SQLITE_OK;
}

static int32_t AssignLatticeModel(char* sqlQuery, sqlite3 *db, void *obj, const DbLoadIndices_t *loadIndices)
{
    sqlite3_stmt *sqlStatement = NULL;
    sqlQuery = "select EnergyBackground, Lattice, LatticeInfo from LatticeModels where Id = ?1";

    check_Sql(PrepareSqlStatement(sqlQuery, db, &sqlStatement, loadIndices->LatticeContextId), SQLITE_ROW)

    LatticeModel_t *latticeModel = (LatticeModel_t*) obj;

    latticeModel->EnergyBackground = array_FromBlob(latticeModel->EnergyBackground, sqlite3_column_blob(sqlStatement, 0));
    memcpy(&latticeModel->LatticeInfo, sqlite3_column_blob(sqlStatement, 2), (size_t) sqlite3_column_bytes(sqlStatement, 2));

    latticeModel->Lattice = array_FromBlob(latticeModel->Lattice, sqlite3_column_blob(sqlStatement, 1));

    check_Sql(sqlite3_finalize(sqlStatement), SQLITE_OK);

    return SQLITE_OK;
}

static int32_t DistributeJumpDirections(DbModel_t* dbModel)
{
    int32_t newBegin = 0;
    int32_t newEnd = 0;
    int32_t collectionID = 0;

    cpp_foreach(iter, dbModel->TransitionModel.JumpDirections)
    {
        if (iter->JumpCollectionId != collectionID)
        {
            newEnd--;

            JumpCollection_t* currentJumpCollection = &span_Get(dbModel->TransitionModel.JumpCollections, collectionID);
            JumpDirections_t* currentJumpDirection = &(JumpDirections_t) span_Split(dbModel->TransitionModel.JumpDirections, newBegin, newEnd);

            currentJumpCollection->JumpDirections = new_Span(currentJumpCollection->JumpDirections, span_GetSize(*currentJumpDirection));
            JumpDirections_t subSpan = (JumpDirections_t) span_Split(dbModel->TransitionModel.JumpDirections, newBegin, newEnd);

            span_Get(dbModel->TransitionModel.JumpCollections, collectionID).JumpDirections = subSpan;

            newEnd++;
            newBegin = newEnd;
            collectionID = iter->JumpCollectionId;
        }
        newEnd++;
    }

    newEnd--;
    JumpCollection_t* currentJumpCollection = &span_Get(dbModel->TransitionModel.JumpCollections, collectionID);
    JumpDirections_t* currentJumpDirection = &(JumpDirections_t) span_Split(dbModel->TransitionModel.JumpDirections, newBegin, newEnd);
    currentJumpCollection->JumpDirections = new_Span(currentJumpCollection->JumpDirections, span_GetSize(*currentJumpDirection));

    JumpDirections_t subDirections = (JumpDirections_t) span_Split(dbModel->TransitionModel.JumpDirections, newBegin, newEnd);
    span_Get(dbModel->TransitionModel.JumpCollections, collectionID).JumpDirections = subDirections;

    return 0;
}

int32_t AssignDatabaseModel(DbModel_t* dbModel, const char* dbFile, int32_t jobContextId)
{
    DbLoadIndices_t projectIds =
    {
            .PackageContextId = -1,
            .StructureContextId = -1,
            .EnergyContextId = -1,
            .TransitionContextId = -1,
            .LatticeContextId = -1,
            .JobContextId = jobContextId
    };

    sqlite3 *db;
    check_Sql(sqlite3_open(dbFile, &db), SQLITE_OK)

    check_Sql(AssignProjectIds(db, &projectIds), SQLITE_OK)
    check_Sql(AssignParentObjects(dbModel, db, &projectIds), SQLITE_OK)
    check_Sql(AssignChildObjects(dbModel, db, &projectIds), SQLITE_OK)

    DistributeJumpDirections(dbModel);

    return 0;
}

ObjectOperationSet_t GetChildOperationSet(DbModel_t * dbModel)
{
    ObjectOperationSet_t operationSet = new_Span(operationSet, 5);
    span_Get(operationSet, 0) = (ObjectOperationPair_t)
            {.Object = &dbModel->StructureModel.EnvironmentDefinitions, .Operation = AssignEnvironmentDefinitions};

    span_Get(operationSet, 1) = (ObjectOperationPair_t)
            {.Object = &dbModel->EnergyModel.PairTables, .Operation = AssignPairEnergyTables};

    span_Get(operationSet, 2) = (ObjectOperationPair_t)
            {.Object = &dbModel->EnergyModel.ClusterTables, .Operation = AssignClusterEnergyTables};

    span_Get(operationSet, 3) = (ObjectOperationPair_t)
            {.Object = &dbModel->TransitionModel.JumpCollections, .Operation = AssignJumpCollections};

    span_Get(operationSet, 4) = (ObjectOperationPair_t)
            {.Object = &dbModel->TransitionModel.JumpDirections, .Operation = AssignJumpDirections};

    return operationSet;
}

ObjectOperationSet_t GetParentOperationSet(DbModel_t * dbModel)
{
    ObjectOperationSet_t operationSet = new_Span(operationSet, 5);
    span_Get(operationSet, 0) = (ObjectOperationPair_t)
            {.Object = &dbModel->StructureModel, .Operation = AssignStructureModel};

    span_Get(operationSet, 1) = (ObjectOperationPair_t)
            {.Object = &dbModel->EnergyModel, .Operation = AssignEnergyModel};

    span_Get(operationSet, 2) = (ObjectOperationPair_t)
            {.Object = &dbModel->TransitionModel, .Operation = AssignTransitionModel};

    span_Get(operationSet, 3) = (ObjectOperationPair_t)
            {.Object = &dbModel->LatticeModel, .Operation = AssignLatticeModel};

    return operationSet;
}
