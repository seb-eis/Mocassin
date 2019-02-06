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

void LoadSimulationModelFromDatabase(__SCONTEXT_PAR)
{
    int32_t jobContextId = -1;
    if (sscanf("%i", getFileInformation(SCONTEXT)->DbQueryString, &jobContextId) != 1)
    {
        error_exit(ERR_VALIDATION, "Job context id is invalid");
    }

    error_t error = AssignDatabaseModel(&SCONTEXT->DbModel, getFileInformation(SCONTEXT)->DatabasePath, jobContextId);
    if (error != ERR_OK)
    {
        error_exit(error, "Failed to load the information from the database");
    }
}