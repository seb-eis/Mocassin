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

#define MAX_LINE_LENGTH 1024
#define POSITION_OF_QUERY_VALUE 1

#define GET_SQL_QUERY "select SqlCommand from SqlCommands where Keyword = ?1"
#define PROJECT_ID_KEYWORDS { "StructureID", "EnergyID", "TransitionID", "LatticeID" }
#define PROJECT_IDS(projectIds) { projectIds.StructureId, projectIds.EnergyId, projectIds.TransitionId, projectIds.LatticeId }


#define NUMBER_OF_PARENT_OBJECTS 4
#define PARENT_KEYWORDS { "Structure", "Energy", "Transition", "Lattice" }
#define PARENT_OPERATIONS { AssignStructureModel, AssignEnergyModel }
#define PARENT_OBJECTS(dbModel) { dbModel.Structure, dbModel.Energy}

#define NUMBER_OF_CHILD_OBJECTS 1
#define CHILD_KEYWORDS { "EnvironmentDefinitions" }
#define CHILD_OPERATIONS { AssignEnvironmentDefinitions }
#define CHILD_OBJECTS(dbModel) {dbModel.Structure.EnvironmentDefinitions}


#define CHECK_SQL(X, Y) if (X != Y) { return X; }


struct ProjectIds{
    int ProjectId;
    int StructureId;
    int EnergyId;
    int TransitionId;
    int LatticeId;
};

int AssignEnvironmentDefinitions(char* sql, const sqlite3 *db, void *obj, const struct ProjectIds *projectIds);

int AssignStructureModel(char* sql, const sqlite3 *db, void *obj, const struct ProjectIds *projectIds);

int AssignEnergyModel(char* sql, const sqlite3 *db, void *obj, const struct ProjectIds *projectIds);

int GetIdFromDatabase(char* sql, const sqlite3 *db, int* id);