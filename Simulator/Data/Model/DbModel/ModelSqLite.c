//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	ModelSqLite.c      	        //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   SQL functions for db model  //
//////////////////////////////////////////

#include "Simulator/Data/Model/DbModel/ModelSqLite.h"

error_t db_load_blobs(const void* restrict db_context, blob_array_t* restrict o_blobs)
{
    return -1;
}

error_t db_text_to_blob(const char* restrict db_result, size_t blob_size, blob_t* blob_target)
{
    return -1;
}

error_t db_open_context(const char* db_path, void* restrict db_context)
{
    return MC_DB_ERROR;
}

error_t db_load_full_model(const char* db_path, mcs_model_t* restrict model_target)
{
    return MC_DB_ERROR;
}

error_t db_load_structure_model(const void* restrict db_context, structure_model_t* restrict model_target)
{
    return MC_DB_ERROR;
}

error_t db_load_energy_model(const void* restrict db_context, energy_model_t* restrict model_target)
{
    return MC_DB_ERROR;
}

error_t db_load_transition_model(const void* restrict db_context, transition_model_t* restrict model_target)
{
    return MC_DB_ERROR;
}