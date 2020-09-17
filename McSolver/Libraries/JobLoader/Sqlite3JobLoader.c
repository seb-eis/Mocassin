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
#include "Sqlite3JobLoader.h"

void LoadMocassinSimulationDatabaseModelToContext(SCONTEXT_PARAMETER)
{
    int32_t jobContextId = -1;
    if (sscanf(getFileInformation(simContext)->JobDbQuery, "%i", &jobContextId) != 1)
        error_exit(ERR_VALIDATION, "Job context id is invalid");

    error_t error = PopulateDbModelFromDatabaseFilePath(&simContext->DbModel, getFileInformation(simContext)->JobDbPath,
                                                        jobContextId);
    assert_success(error != ERR_OK, "Failed to load the job from the database.");
}

static error_t PrepareSqlStatement(char *sqlQuery, sqlite3 *db, sqlite3_stmt **sqlStatement, int32_t id)
{
    const int variableIndexInStmt = 1;

    error_t error = sqlite3_prepare_v2(db, sqlQuery, -1, &(*sqlStatement), NULL);
    return_if(error != SQLITE_OK, error);

    error = sqlite3_bind_int(*sqlStatement, variableIndexInStmt, id);
    return_if(error != SQLITE_OK, error);

    error = sqlite3_step(*sqlStatement);
    return error;
}

static error_t GetJobModelFromDb_Deprecated(char *sqlQuery, sqlite3 *db, DbModel_t *dbModel)
{
    let localQuery = "select StructureModelId, EnergyModelId, TransitionModelId, LatticeModelId, PackageId, JobInfo, JobHeader "
                     "from JobModels where Id = ?1";
    sqlQuery = localQuery;

    sqlite3_stmt *sqlStatement = NULL;

    error_t error = PrepareSqlStatement(sqlQuery, db, &sqlStatement, dbModel->JobModel.ContextId);
    SQLFinalizeAndReturnIf(error != SQLITE_ROW, sqlStatement, error);

    dbModel->JobModel.StructureModelId = sqlite3_column_int(sqlStatement, 0);
    dbModel->JobModel.EnergyModelId = sqlite3_column_int(sqlStatement, 1);
    dbModel->JobModel.TransitionModelId = sqlite3_column_int(sqlStatement, 2);
    dbModel->JobModel.LatticeModelId = sqlite3_column_int(sqlStatement, 3);
    dbModel->JobModel.PackageId = sqlite3_column_int(sqlStatement, 4);
    dbModel->JobModel.JobInfo = *(JobInfo_t*) sqlite3_column_blob(sqlStatement, 5);

    let jobHeaderSize = (size_t) sqlite3_column_bytes(sqlStatement, 6);
    dbModel->JobModel.JobHeader = malloc(jobHeaderSize);
    dbModel->JobModel.JobInfo.JobHeader = dbModel->JobModel.JobHeader;
    memcpy(dbModel->JobModel.JobHeader, sqlite3_column_blob(sqlStatement, 6), jobHeaderSize);
    memset(dbModel->JobModel.RoutineData.Guid, 0, sizeof(MocsimUuid_t));

    error = sqlite3_finalize(sqlStatement);
    return error;
}


static error_t GetJobModelFromDb(char *sqlQuery, sqlite3 *db, DbModel_t *dbModel)
{
    let localQuery = "select StructureModelId, EnergyModelId, TransitionModelId, LatticeModelId, PackageId, JobInfo, JobHeader, RoutineData "
                     "from JobModels where Id = ?1";
    sqlQuery = localQuery;

    sqlite3_stmt *sqlStatement = NULL;

    error_t error = PrepareSqlStatement(sqlQuery, db, &sqlStatement, dbModel->JobModel.ContextId);
    // Todo: Remove this compatibility call as soon as it is no longer needed
    if (error != SQLITE_ROW)
    {
        sqlite3_finalize(sqlStatement);
        return GetJobModelFromDb_Deprecated(sqlQuery, db , dbModel);
    }

    dbModel->JobModel.StructureModelId = sqlite3_column_int(sqlStatement, 0);
    dbModel->JobModel.EnergyModelId = sqlite3_column_int(sqlStatement, 1);
    dbModel->JobModel.TransitionModelId = sqlite3_column_int(sqlStatement, 2);
    dbModel->JobModel.LatticeModelId = sqlite3_column_int(sqlStatement, 3);
    dbModel->JobModel.PackageId = sqlite3_column_int(sqlStatement, 4);
    dbModel->JobModel.JobInfo = *(JobInfo_t*) sqlite3_column_blob(sqlStatement, 5);

    let jobHeaderSize = (size_t) sqlite3_column_bytes(sqlStatement, 6);
    dbModel->JobModel.JobHeader = malloc(jobHeaderSize);
    dbModel->JobModel.JobInfo.JobHeader = dbModel->JobModel.JobHeader;
    memcpy(dbModel->JobModel.JobHeader, sqlite3_column_blob(sqlStatement, 6), jobHeaderSize);

    let routineDataSize = sqlite3_column_bytes(sqlStatement, 7);
    if (routineDataSize < 16)
    {
        memset(dbModel->JobModel.RoutineData.Guid, 0, sizeof(MocsimUuid_t));
        error = sqlite3_finalize(sqlStatement);
        return error;
    }
    let routineBlob = sqlite3_column_blob(sqlStatement, 7);
    dbModel->JobModel.RoutineData.ParamData = span_ConstructFromBlob(dbModel->JobModel.RoutineData.ParamData,
            routineBlob + sizeof(MocsimUuid_t), routineDataSize - sizeof(MocsimUuid_t));
    memcpy(dbModel->JobModel.RoutineData.Guid, routineBlob, sizeof(MocsimUuid_t));

    error = sqlite3_finalize(sqlStatement);
    return error;
}

// Invokes the passed load operation set using the provided database and model
static error_t InvokeLoadOperations(sqlite3 *db, DbModel_t *dbModel, const DbModelLoadOperations_t operations)
{
    return_if(dbModel == NULL || db == NULL, ERR_NULLPOINTER);

    char queryBuffer[250];

    cpp_foreach (item, operations)
    {
        error_t error = (*item)(queryBuffer, db, dbModel);
        return_if(error != SQLITE_OK, error);
    }

    return SQLITE_OK;
}

// Invokes the passed on load operation set using the provided database and model
static error_t InvokeOnLoadedOperations(DbModel_t* dbModel, const DbModelOnLoadedOperations_t operations)
{
    return_if(dbModel == NULL, ERR_NULLPOINTER);

    cpp_foreach (item, operations)
    {
        error_t error = (*item)(dbModel);
        return_if(error != ERR_OK, error);
    }

    return ERR_OK;
}



static error_t GetStructureModelFromDb(char *sqlQuery, sqlite3 *db, DbModel_t *dbModel)
{
    let localQuery = "select NumOfTrackersPerCell, NumOfGlobalTrackers, InteractionRange, NumOfEnvironmentDefinitions"
                     ", MetaData from StructureModels where Id = ?1";
    sqlQuery = localQuery;

    sqlite3_stmt *sqlStatement = NULL;
    var model = &dbModel->StructureModel;

    error_t error = PrepareSqlStatement(sqlQuery, db, &sqlStatement, dbModel->JobModel.StructureModelId);
    SQLFinalizeAndReturnIf(error != SQLITE_ROW, sqlStatement, error);

    let environmentCount = sqlite3_column_int(sqlStatement, 3);

    model->StaticTrackersPerCellCount = sqlite3_column_int(sqlStatement, 0);
    model->GlobalTrackerCount = sqlite3_column_int(sqlStatement, 1);
    model->InteractionRange = *(InteractionRange_t*) sqlite3_column_blob(sqlStatement, 2);
    model->EnvironmentDefinitions = span_New(model->EnvironmentDefinitions, environmentCount);

    model->MetaData = malloc(sizeof(StructureMetaData_t));
    SQLFinalizeAndReturnIf(model->MetaData == NULL, sqlStatement, ERR_MEMALLOCATION);

    memcpy(model->MetaData, sqlite3_column_blob(sqlStatement, 4), sizeof(StructureMetaData_t));

    error = sqlite3_finalize(sqlStatement);
    return  error;
}

// Compatibility function to enable the system to load older databases without the defect background column
static error_t GetEnergyModelFromDb_Deprecated(char *sqlQuery, sqlite3 *db, DbModel_t *dbModel)
{
    let localQuery = "select NumOfPairTables, NumOfClusterTables from EnergyModels where Id = ?1";
    sqlQuery = localQuery;

    sqlite3_stmt *sqlStatement = NULL;
    var model = &dbModel->EnergyModel;

    error_t error = PrepareSqlStatement(sqlQuery, db, &sqlStatement, dbModel->JobModel.EnergyModelId);
    SQLFinalizeAndReturnIf(error != SQLITE_ROW, sqlStatement, error);

    let pairTableCount = sqlite3_column_int(sqlStatement, 0);
    let clusterTableCount = sqlite3_column_int(sqlStatement, 1);

    model->PairTables = span_New(model->PairTables, pairTableCount);
    model->ClusterTables = span_New(model->ClusterTables, clusterTableCount);

    error = sqlite3_finalize(sqlStatement);
    return error;
}

static error_t GetEnergyModelFromDb(char *sqlQuery, sqlite3 *db, DbModel_t *dbModel)
{
    let localQuery = "select NumOfPairTables, NumOfClusterTables, DefectBackground from EnergyModels where Id = ?1";
    sqlQuery = localQuery;

    sqlite3_stmt *sqlStatement = NULL;
    var model = &dbModel->EnergyModel;

    error_t error = PrepareSqlStatement(sqlQuery, db, &sqlStatement, dbModel->JobModel.EnergyModelId);

    // Todo: Remove this compatibility call as soon as it is no longer needed
    if (error != SQLITE_ROW)
    {
        sqlite3_finalize(sqlStatement);
        return GetEnergyModelFromDb_Deprecated(sqlQuery, db , dbModel);
    }

    SQLFinalizeAndReturnIf(error != SQLITE_ROW, sqlStatement, error);

    let pairTableCount = sqlite3_column_int(sqlStatement, 0);
    let clusterTableCount = sqlite3_column_int(sqlStatement, 1);

    model->PairTables = span_New(model->PairTables, pairTableCount);
    model->ClusterTables = span_New(model->ClusterTables, clusterTableCount);

    model->DefectBackground = array_ConstructFromBlob(model->DefectBackground, sqlite3_column_blob(sqlStatement, 2));

    error = sqlite3_finalize(sqlStatement);
    return error;
}

static error_t GetTransitionModelFromDb(char *sqlQuery, sqlite3 *db, DbModel_t *dbModel)
{
    let localQuery = "select JumpMappingTable, JumpCountTable, StaticTrackerMapping, GlobalTrackerMapping, NumOfCollections, NumOfDirections "
                     "from TransitionModels where Id = ?1";
    sqlQuery = localQuery;

    sqlite3_stmt *sqlStatement = NULL;
    var model = &dbModel->TransitionModel;

    error_t error = PrepareSqlStatement(sqlQuery, db, &sqlStatement, dbModel->JobModel.TransitionModelId);
    SQLFinalizeAndReturnIf(error != SQLITE_ROW, sqlStatement, error);

    let jumpCollectionCount = sqlite3_column_int(sqlStatement, 4);
    let jumpDirectionCount = sqlite3_column_int(sqlStatement, 5);

    model->JumpDirectionMappingTable = array_ConstructFromBlob(model->JumpDirectionMappingTable, sqlite3_column_blob(sqlStatement, 0));
    model->JumpCountMappingTable = array_ConstructFromBlob(model->JumpCountMappingTable, sqlite3_column_blob(sqlStatement, 1));
    model->StaticTrackerMappingTable = array_ConstructFromBlob(model->StaticTrackerMappingTable, sqlite3_column_blob(sqlStatement, 2));
    model->GlobalTrackerMappingTable = array_ConstructFromBlob(model->GlobalTrackerMappingTable, sqlite3_column_blob(sqlStatement, 3));
    model->JumpCollections = span_New(model->JumpCollections, jumpCollectionCount);
    model->JumpDirections = span_New(model->JumpDirections, jumpDirectionCount);

    error = sqlite3_finalize(sqlStatement);
    return error;
}

static error_t GetLatticeModelFromDb(char *sqlQuery, sqlite3 *db, DbModel_t *dbModel)
{
    let localQuery = "select Lattice, LatticeInfo, EnergyBackground from LatticeModels where Id = ?1";
    sqlQuery = localQuery;

    sqlite3_stmt *sqlStatement = NULL;
    var latticeModel = &dbModel->LatticeModel;

    error_t error = PrepareSqlStatement(sqlQuery, db, &sqlStatement, dbModel->JobModel.LatticeModelId);
    SQLFinalizeAndReturnIf(error != SQLITE_ROW, sqlStatement, error);

    latticeModel->Lattice = array_ConstructFromBlob(latticeModel->Lattice, sqlite3_column_blob(sqlStatement, 0));
    latticeModel->LatticeInfo = *(LatticeInfo_t*) sqlite3_column_blob(sqlStatement, 1);
    latticeModel->EnergyBackground = array_ConstructFromBlob(latticeModel->EnergyBackground, sqlite3_column_blob(sqlStatement, 2));

    error = sqlite3_finalize(sqlStatement);
    return error;
}

static error_t GetEnvironmentDefinitionsFromDb(char *sqlQuery, sqlite3 *db, DbModel_t *dbModel)
{
    let localQuery = "select ObjectId, SelectionMask, UpdateParticleIds, PairDefinitions, ClusterDefinitions, "
                     "PositionParticleIds from EnvironmentDefinitions where StructureModelId = ?1 order by ObjectId";
    sqlQuery = localQuery;

    sqlite3_stmt *sqlStatement = NULL;
    var environmentDefinitions = &dbModel->StructureModel.EnvironmentDefinitions;

    error_t error = PrepareSqlStatement(sqlQuery, db, &sqlStatement, dbModel->JobModel.StructureModelId);
    SQLFinalizeAndReturnIf(error != SQLITE_ROW, sqlStatement, error);

    cpp_foreach(item, *environmentDefinitions)
    {
        let pairDefinitionCount = sqlite3_column_bytes(sqlStatement, 3) / sizeof(PairInteraction_t);
        let clusterDefinitionCount = sqlite3_column_bytes(sqlStatement, 4) / sizeof(ClusterInteraction_t);

        item->PositionId = sqlite3_column_int(sqlStatement, 0);
        item->SelectionParticleMask = sqlite3_column_int64(sqlStatement, 1);
        item->PairInteractions = span_ConstructFromBlob(item->PairInteractions, sqlite3_column_blob(sqlStatement, 3), pairDefinitionCount);
        item->ClusterInteractions = span_ConstructFromBlob(item->ClusterInteractions, sqlite3_column_blob(sqlStatement, 4), clusterDefinitionCount);

        memcpy(item->UpdateParticleIds, sqlite3_column_blob(sqlStatement, 2), (size_t) sqlite3_column_bytes(sqlStatement, 2));
        memcpy(item->PositionParticleIds, sqlite3_column_blob(sqlStatement, 5), (size_t) sqlite3_column_bytes(sqlStatement, 5));

        if (item != environmentDefinitions->End - 1)
        {
            error = sqlite3_step(sqlStatement);
            SQLFinalizeAndReturnIf(error != SQLITE_ROW, sqlStatement, error);
        }
    }

    error = sqlite3_finalize(sqlStatement);
    return error;
}

static error_t GetPairEnergyTablesFromDb(char *sqlQuery, sqlite3 *db, DbModel_t *dbModel)
{
    let localQuery = "select ObjectId, EnergyTable "
                     "from PairEnergyTables where EnergyModelId = ?1 order by ObjectId";
    sqlQuery = localQuery;

    sqlite3_stmt *sqlStatement = NULL;
    var tables = &dbModel->EnergyModel.PairTables;

    error_t error = PrepareSqlStatement(sqlQuery, db, &sqlStatement, dbModel->JobModel.EnergyModelId);
    SQLFinalizeAndReturnIf(error == SQLITE_DONE, sqlStatement, SQLITE_OK);
    SQLFinalizeAndReturnIf(error != SQLITE_ROW, sqlStatement, error);

    cpp_foreach(table, *tables)
    {
        table->ObjectId = sqlite3_column_int(sqlStatement, 0);
        table->EnergyTable = array_ConstructFromBlob(table->EnergyTable, sqlite3_column_blob(sqlStatement, 1));

        if (table != tables->End - 1)
        {
            error = sqlite3_step(sqlStatement);
            SQLFinalizeAndReturnIf(error != SQLITE_ROW, sqlStatement, error);
        }
    }

    error = sqlite3_finalize(sqlStatement);
    return error;
}

static error_t GetClusterEnergyTablesFromDb(char *sqlQuery, sqlite3 *db, DbModel_t *dbModel)
{
    let localQuery = "select ObjectId, EnergyTable, OccupationCodes, TableIndexing from ClusterEnergyTables "
                     "where EnergyModelId = ?1 order by ObjectId";
    sqlQuery = localQuery;

    sqlite3_stmt *sqlStatement = NULL;
    var tables = &dbModel->EnergyModel.ClusterTables;

    error_t error = PrepareSqlStatement(sqlQuery, db, &sqlStatement, dbModel->JobModel.EnergyModelId);
    SQLFinalizeAndReturnIf(error == SQLITE_DONE, sqlStatement, SQLITE_OK);
    SQLFinalizeAndReturnIf(error != SQLITE_ROW, sqlStatement, error);

    cpp_foreach(table, *tables)
    {
        let occupationCodeCount = sqlite3_column_bytes(sqlStatement, 2) / sizeof(OccupationCode64_t);

        table->ObjectId = sqlite3_column_int(sqlStatement, 0);
        table->EnergyTable = array_ConstructFromBlob(table->EnergyTable, sqlite3_column_blob(sqlStatement, 1));
        table->OccupationCodes = span_ConstructFromBlob(table->OccupationCodes, sqlite3_column_blob(sqlStatement, 2), occupationCodeCount);

        memcpy(table->ParticleTableMapping, sqlite3_column_blob(sqlStatement, 3), (size_t) sqlite3_column_bytes(sqlStatement, 3));

        if (table != tables->End - 1)
        {
            error = sqlite3_step(sqlStatement);
            SQLFinalizeAndReturnIf(error != SQLITE_ROW, sqlStatement, error);
        }
    }

    error = sqlite3_finalize(sqlStatement);
    return error;
}

static error_t GetJumpCollectionsFromDb(char *sqlQuery, sqlite3 *db, DbModel_t *dbModel)
{
    let localQuery = "select ObjectId, SelectionMask, JumpRules "
                     "from JumpCollections where TransitionModelId = ?1 order by ObjectId";
    sqlQuery = localQuery;

    sqlite3_stmt *sqlStatement = NULL;
    var collections = &dbModel->TransitionModel.JumpCollections;

    error_t error = PrepareSqlStatement(sqlQuery, db, &sqlStatement, dbModel->JobModel.TransitionModelId);
    SQLFinalizeAndReturnIf(error != SQLITE_ROW, sqlStatement, error);

    cpp_foreach(collection, *collections)
    {
        let jumpRuleCount = sqlite3_column_bytes(sqlStatement, 2) / sizeof(JumpRule_t);

        collection->ObjectId = sqlite3_column_int(sqlStatement, 0);
        collection->MobileParticlesMask = sqlite3_column_int64(sqlStatement, 1);
        collection->JumpRules = span_ConstructFromBlob(collection->JumpRules, sqlite3_column_blob(sqlStatement, 2), jumpRuleCount);

        if (collection != collections->End - 1)
        {
            error = sqlite3_step(sqlStatement);
            SQLFinalizeAndReturnIf(error != SQLITE_ROW, sqlStatement, error);
        }
    }

    error = sqlite3_finalize(sqlStatement);
    return error;
}

static error_t GetJumpDirectionsFromDb(char *sqlQuery, sqlite3 *db, DbModel_t *dbModel)
{
    let localQuery = "select ObjectId, PositionId, JumpLength, FieldProjection, CollectionId, JumpSequence, LocalMoveSequence "
                     "from JumpDirections where TransitionModelId = ?1 order by ObjectId";
    sqlQuery = localQuery;

    sqlite3_stmt *sqlStatement = NULL;
    var directions = &dbModel->TransitionModel.JumpDirections;

    error_t error = PrepareSqlStatement(sqlQuery, db, &sqlStatement, dbModel->JobModel.TransitionModelId);
    SQLFinalizeAndReturnIf(error != SQLITE_ROW, sqlStatement, error);

    cpp_foreach(direction, *directions)
    {
        let moveSequenceCount = sqlite3_column_bytes(sqlStatement, 6) / sizeof(MoveSequence_t);
        let jumpSequenceCount = sqlite3_column_bytes(sqlStatement, 5) / sizeof(JumpSequence_t);

        direction->ObjectId = sqlite3_column_int(sqlStatement, 0);
        direction->PositionId = sqlite3_column_int(sqlStatement, 1);
        direction->JumpLength = sqlite3_column_int(sqlStatement, 2);
        direction->ElectricFieldFactor = sqlite3_column_double(sqlStatement, 3);
        direction->JumpCollectionId = sqlite3_column_int(sqlStatement, 4);
        direction->JumpSequence = span_ConstructFromBlob(direction->JumpSequence, sqlite3_column_blob(sqlStatement, 5), jumpSequenceCount);
        direction->MovementSequence = span_ConstructFromBlob(direction->MovementSequence, sqlite3_column_blob(sqlStatement, 6), moveSequenceCount);

        if (direction != directions->End - 1)
        {
            error = sqlite3_step(sqlStatement);
            SQLFinalizeAndReturnIf(error != SQLITE_ROW, sqlStatement, error);
        }
    }

    error = sqlite3_finalize(sqlStatement);
    return error;
}

// Assigns each jump collection its sub-span access to the main jump direction buffer
static error_t AssignDirectionBuffersToJumpCollections(DbModel_t *dbModel)
{
    int32_t newBegin = 0, newEnd = 0, collectionID = 0;

    cpp_foreach(jumpDirection, dbModel->TransitionModel.JumpDirections)
    {
        if (jumpDirection->JumpCollectionId != collectionID)
        {
            let subSpan = span_Split(dbModel->TransitionModel.JumpDirections, newBegin, newEnd);
            span_Get(dbModel->TransitionModel.JumpCollections, collectionID).JumpDirections = subSpan;

            newBegin = newEnd;
            collectionID = jumpDirection->JumpCollectionId;
        }
        ++newEnd;
    }

    let subDirections = span_Split(dbModel->TransitionModel.JumpDirections, newBegin, newEnd);
    span_Get(dbModel->TransitionModel.JumpCollections, collectionID).JumpDirections = subDirections;

    return SQLITE_OK;
}


error_t PopulateDbModelFromDatabaseFilePath(DbModel_t *dbModel, const char *dbFile, int32_t jobContextId)
{
    error_t error;
    sqlite3 *db;
    dbModel->JobModel.ContextId = jobContextId;

    error = sqlite3_open_v2(dbFile, &db, SQLITE_OPEN_READONLY | SQLITE_OPEN_NOMUTEX, NULL);
    SQLCloseAndReturnIf(error != SQLITE_OK, db);

    error = InvokeLoadOperations(db, dbModel, GetParentObjectLoadOperations());
    SQLCloseAndReturnIf(error != SQLITE_OK, db);

    error = InvokeLoadOperations(db, dbModel, GetChildObjectLoadOperations());
    SQLCloseAndReturnIf(error != SQLITE_OK, db);

    error = InvokeOnLoadedOperations(dbModel, GetDataLoadedPostOperations());

    assert_success(sqlite3_close(db), "Failed to close the database file.");
    return error;
}

DbModelLoadOperations_t GetChildObjectLoadOperations()
{
    static FDbModelLoad_t operations[] =
    {
            (FDbModelLoad_t) GetEnvironmentDefinitionsFromDb,
            (FDbModelLoad_t) GetPairEnergyTablesFromDb,
            (FDbModelLoad_t) GetClusterEnergyTablesFromDb,
            (FDbModelLoad_t) GetJumpCollectionsFromDb,
            (FDbModelLoad_t) GetJumpDirectionsFromDb
    };
    return (DbModelLoadOperations_t) span_CArrayToSpan(operations);
}

DbModelLoadOperations_t GetParentObjectLoadOperations()
{
    static FDbModelLoad_t operations[] =
    {
           (FDbModelLoad_t) GetJobModelFromDb,
           (FDbModelLoad_t) GetStructureModelFromDb,
           (FDbModelLoad_t) GetEnergyModelFromDb,
           (FDbModelLoad_t) GetTransitionModelFromDb,
           (FDbModelLoad_t) GetLatticeModelFromDb
    };
    return (DbModelLoadOperations_t) span_CArrayToSpan(operations);
}

DbModelOnLoadedOperations_t GetDataLoadedPostOperations()
{
    static FDbOnModelLoaded_t operations[] =
    {
            (FDbOnModelLoaded_t) AssignDirectionBuffersToJumpCollections
    };
    return (DbModelOnLoadedOperations_t) span_CArrayToSpan(operations);
}

