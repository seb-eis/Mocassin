//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	ModelSqLite.c      	        //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   SQL functions for db model  //
//////////////////////////////////////////

#include "Simulator/Data/Database/DbModelLoad.h"
#include "Framework/Sqlite/SqliteReader.h"

void LoadSimulationModelFromDatabase(SCONTEXT_PARAM)
{
    int32_t jobContextId = -1;
    if (sscanf(getFileInformation(SCONTEXT)->JobDbQuery, "%i", &jobContextId) != 1)
        error_exit(ERR_VALIDATION, "Job context id is invalid");

    error_t error = PopulateDbModelFromDatabase(&SCONTEXT->DbModel, getFileInformation(SCONTEXT)->JobDbPath, jobContextId);
    error_assert(error != ERR_OK, "Failed to load the job from the database.");
}