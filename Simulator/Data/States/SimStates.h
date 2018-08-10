//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	SimStates.h        		    //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Simulation States           //
//////////////////////////////////////////

#pragma once
#include "Framework/Math/Types/Vector.h"
#include "Framework/Basic/BaseTypes/BaseTypes.h"
#include "Framework/Basic/FileIO/FileIO.h"

// Defines the simulation species type to be one unsigned byte
typedef byte_t species_t;

// Defines the tracker type to be of type vector with 24 bytes
typedef vector3_t tracker_t;

// Defines the integral type for indexing to the int32_t
typedef int32_t index_t;

// Defines the lattice state buffer that stores the simulation lattice
typedef struct lattice_state { species_t* start_it; species_t* end_it; } lattice_state_t;

// Defines the tracking state buffer to store individual position movement vectors
typedef struct tracking_state { tracker_t* start_it; tracker_t* end_it; } tracking_state_t;

// Defines the simulation counters collection that contains the cycle count and all possible cycle outcomes for one species type
typedef struct counter_col { long mc_cycles, mc_steps, mc_rejects, mc_blocks, mc_on_unstable, mc_to_unstable; } counter_col_t;

// Defines the counter state of the simulation containing a counter collection for each particle type of the simulation
typedef struct counter_state { counter_col_t* start_it; counter_col_t* end_it; } counter_state_t;

// Defines the timer collection for the simulation timers
typedef struct timer_col { double mc_sim_time; long mc_run_timer; } timer_col_t;

// Defines the timer state as a pointer to a timer collection
typedef struct timer_state { timer_col_t* timer_col; } timer_state_t;

// Defines the indexing type for state redirection
typedef struct index_state { index_t* start_it; index_t* end_it; } index_state_t;

// Defines the simulation state attribute type that defines the attributes defining the state size
typedef struct mc_state_attribute { size_t num_of_atoms, num_of_species, num_of_trackers; } mc_state_attr_t;

// Defines the complete simulation state memory access type
typedef struct mc_state
{
    buffer_t            state_buffer;
    timer_state_t       timer_state;
    lattice_state_t     lattice_state;
    counter_state_t     counter_state;
    tracking_state_t    type_tracking_state;
    tracking_state_t    dynamic_tracking_state;
    tracking_state_t    static_tracking_state;
    index_state_t       dyn_track_index_state;
    index_state_t       stat_track_index_state;

} mc_state_t;

// Calculates the required simulation state size in bytes using the provided state attributes
size_t calc_sim_state_size(mc_state_attr_t* restrict state_attr);

// Allocates the required minimum number of memory blocks to host the full simulation state
buffer_t alloc_sim_state_buffer(mc_state_attr_t* restrict state_attr);

// Creates the simulation state from the provided simulation state buffer and state attributes
mc_state_t create_sim_state(const buffer_t* sim_state_buffer, const mc_state_attr_t* state_attr);

// Loads the simulation state from a file stream. Terminates program if an error occures
mc_state_t load_sim_state(file_t* restrict f_stream, mc_state_attr_t* restrict state_attr);