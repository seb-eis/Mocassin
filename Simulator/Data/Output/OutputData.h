//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	OutputData.h   	    	    //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Ouput Data Objects          //
//////////////////////////////////////////

#include <stdint.h>
#include "Framework/Basic/BaseTypes/BaseTypes.h"
#include "Simulator/Data/States/SimStates.h"

// Defines the data object for readable output of the simulation tracking data
typedef struct mc_tracking_out
{
    tracking_state_t stat_track_state;
    tracking_state_t dyn_track_state;
    index_state_t stat_track_index_state;
    index_state_t dyn_track_index_state;

} tracking_out_t;

// Defines the data object for readable output of the simulation lattice data
typedef struct mc_lattice_out
{
    lattice_state_t lattice_state;

} lattice_out_t;

// Defines the data object for readable output of the simulation misc data (counters, timers, ...)
typedef struct mc_misc_out
{
    timer_state_t   timer_state;
    counter_state_t counter_state;

} misc_out_t;

// Defines the data object for output of the simulation result data collection
typedef struct mc_data_out_col
{  
    misc_out_t      misc_stats;
    lattice_out_t   lattice_stats;
    tracking_out_t  tracking_stats;

} mc_results_t;

// Links a simulation data ouput object to an existing and initialized simulation state
error_t link_simout_to_state(const mc_state_t sim_state, mc_results_t* out_results);

// Write the simulation results to a stream. Uses readable formatting for the ouput and returns MC_NO_ERROR on success
error_t write_simout_to_stream(file_t* restrict f_stream, const mc_results_t* restrict sim_results);

// Write the simulation results to a file, supported modes are "w" and "a". Uses readable formatting for the ouput and returns MC_NO_ERROR on success
error_t write_simout_to_file(const char* restrict f_path, const char* restrict f_mode, const mc_results_t* restrict sim_results);

// Write the simulation results to an sqlite database. Returns MC_NO_ERROR on success
error_t write_simout_to_sqlite_db(const char* restrict db_path, const mc_results_t* restrict sim_results);