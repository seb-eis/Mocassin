//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	InputData.h    	    	    //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Input Data Objects          //
//////////////////////////////////////////

#include "Framework/Errors/McErrors.h"
#include "Framework/Basic/BaseTypes/BaseTypes.h"
#include "Framework/Basic/FileIO/FileIO.h"

// Defines the static read-in lattice simulation data provided by the creation program
typedef struct mc_lattice_data
{

} lattice_data_t;

// Defines the static read-in transition simulation data provided by the creation program
typedef struct mc_transition_data
{

} transition_data_t;

// Defines the static read-in energy simulation data provided by the creation program
typedef struct mc_energy_data
{

} energy_data_t;

// Defines the static read-in settings simulation data provided by the creation program
typedef struct mc_settings_data
{

} settings_data_t;

// Defines the static read-in simulation data collection provided by the creation program
typedef struct mc_data_col
{
    lattice_data_t      lattice_data;
    transition_data_t   transition_data;
    energy_data_t       energy_data;
    settings_data_t     settings_data;

} mc_data_t;

// Load the simulation data from the provided stream. Uses readable formatting and returns MC_NO_ERROR on success
error_t load_simdata_from_stream(file_t* restrict f_stream, mc_data_t* restrict out_data);

// Load the simulation data from the provided file. Uses readable formatting and returns MC_NO_ERROR on success
error_t load_simdata_from_file(const char* restrict f_path, mc_data_t* restrict out_data);

// Load the simulation data from the provided sqlite database. Returns
error_t load_simdata_from_sqlite_db(const char* restrict db_path, mc_data_t* restrict out_data)