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
#include "Simulator/Data/Model/DbModel/DbModel.h"

error_t db_open_context(const char* db_path, void* restrict db_context);

error_t db_load_full_model(const char* db_path, mcs_model_t* restrict model_target);

error_t db_load_structure_model(const void* restrict db_context, structure_model_t* restrict model_target);

error_t db_load_energy_model(const void* restrict db_context, energy_model_t* restrict model_target);

error_t db_load_transition_model(const void* restrict db_context, transition_model_t* restrict model_target);