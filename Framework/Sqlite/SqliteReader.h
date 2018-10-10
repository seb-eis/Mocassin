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

#define POSITION_OF_QUERY_VALUE 1
#define MAX_QUERY_LENGTH 1024

#define GET_SQL_QUERY "select Statement from SqlStatements where Keyword = ?1"
#define PROJECT_ID_KEYWORDS { "StructureID", "EnergyID", "TransitionID", "LatticeID" }
#define PROJECT_IDS(projectIds) { projectIds.StructureId, projectIds.EnergyId, projectIds.TransitionId, projectIds.LatticeId }


#define NUMBER_OF_PARENT_OBJECTS 4
#define PARENT_KEYWORDS { "Structure", "Energy", "Transition", "Lattice" }
#define PARENT_OPERATIONS { AssignStructureModel, AssignEnergyModel, AssignTransitionModel, AssignLatticeModel }
#define PARENT_OBJECTS(dbModel) { dbModel.Structure, dbModel.Energy, dbModel.Transition, dbModel.Lattice}

#define NUMBER_OF_CHILD_OBJECTS 5
#define CHILD_KEYWORDS { "EnvironmentDefinitions", "PairEnergyTables", "ClusterEnergyTables", "JumpColletions", "JumpDirections" }
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


#define CHECK_SQL(X, Y) if (X != Y) { return X; }


struct ProjectIds{
    int ProjectId;
    int StructureId;
    int EnergyId;
    int TransitionId;
    int LatticeId;
};

int AssignParentObjects(DbModel_t *dbModel, sqlite3 *db, const struct ProjectIds* ids);

int AssignChildObjects(DbModel_t *dbModel, sqlite3 *db, const struct ProjectIds *projectIds);

int AssignStructureModel(char* sql, sqlite3 *db, void *obj, const struct ProjectIds *projectIds);

int AssignEnvironmentDefinitions(char* sql, sqlite3 *db, void *obj, const struct ProjectIds *projectIds);

int AssignEnergyModel(char* sql, sqlite3 *db, void *obj, const struct ProjectIds *projectIds);

int AssignPairEnergyTables(char* sql, sqlite3 *db, void *obj, const struct ProjectIds *projectIds);

int AssignClusterEnergyTables(char* sql, sqlite3 *db, void *obj, const struct ProjectIds *projectIds);

int AssignTransitionModel(char* sql, sqlite3 *db, void *obj, const struct ProjectIds *projectIds);

int AssignJumpCollections(char* sql, sqlite3 *db, void *obj, const struct ProjectIds *projectIds);

int AssignJumpDirections(char* sql, sqlite3 *db, void *obj, const struct ProjectIds *projectIds);

int AssignLatticeModel(char* sql, sqlite3 *db, void *obj, const struct ProjectIds *projectIds);

int GetIdFromDatabase(char* sql, sqlite3 *db, int* id);

int TestDatabase( char* dbFile );

int TestQuery(char* dbFile);