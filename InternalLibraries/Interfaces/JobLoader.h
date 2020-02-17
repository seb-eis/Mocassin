//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	ModelSqLite.h      	        //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   SQL functions for db model  //
//////////////////////////////////////////

#pragma once
#include "Simulator/Data/SimContext/ContextAccess.h"

// Loads the database model of the job to the passed simulation context
void JobLoader_LoadDatabaseModelToContext(SCONTEXT_PARAMETER);

