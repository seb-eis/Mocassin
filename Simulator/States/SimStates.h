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
#include "Framework/Structs/Array/Array.h"

// Defines the simulation species type to be one unsigned byte
typedef byte_t species_t;

// Defines the tracker type to be of type vector with 24 bytes
typedef vector_t tracker_t;

// Defines the lattice state buffer that stores the simulation lattice
typedef struct { species_t* start_it; species_t* end_it; } lattice_state_t;

// Defines the tracking state buffer to store individual position movement vectors
typedef struct { tracker_t* start_it; tracker_t* end_it; } tracking_state_t;

// Defines the simulation counters collection that contains the cycle count and all possible cycle outcomes for one species type
typedef struct { long mc_cycles, mc_steps, mc_rejects, mc_blocks, mc_on_unstable, mc_to_unstable; } counter_col_t;

// Defines the couznter state of the simulation containing a counter collection for each property type
typedef struct { counter_col_t* start_it; counter_col_t* end_it; } counter_state_t;

// Defines the timer collection for the simulation timers
typedef struct { double mc_sim_time; long mc_run_timer; } timer_col_t;

// Defines the timer state as a pointer to a timer collection
typedef struct { timer_col_t* timer_col; } timer_state_t;

// Defines the complete simulation state memory access type
typedef struct
{
    timer_state_t timer_state;
    lattice_state_t lattice_state;
    counter_state_t counter_state;
    tracking_state_t type_tracking_state;
    tracking_state_t dynamic_tracking_state;
    tracking_state_t static_tracking_state;
} sim_state_t;

// Calculates the required simulation state size in bytes from the simulation lattice size and the number of simulated species
size_t calc_sim_state_size(size_t sim_lattice_size, size_t num_of_species);

// Allocates the simulation state as a continous block of memory and returns a byte array access to it. Total buffer size is corrected to even memblock_t count
byte_array_t alloc_sim_state_buffer(size_t sim_lattice_size, size_t num_of_species);

// Creates a sim state access struct using the provided simulation state buffer, lattice size and num of simulated species
sim_state_t create_sim_state_access(const byte_array_t* sim_state_buffer, size_t sim_lattice_size, size_t num_of_species);

// Loads the simulation state buffer from a dump file into the provided state buffer. Returns an error code if the loaded buffer dump has the wrong size
int load_sim_state_buffer(const char* file_path, byte_array_t sim_state_buffer);