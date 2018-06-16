//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	SimStates.c        		    //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Simulation States           //
//////////////////////////////////////////

#include "Simulator/States/SimStates.h"

size_t calc_sim_state_size(size_t sim_lattice_size, size_t num_of_species)
{
    size_t buffer_size = 0;
    buffer_size += sizeof(timer_col_t);
    buffer_size += sizeof(species_t) * sim_lattice_size;
    buffer_size += sizeof(counter_col_t) * num_of_species;
    buffer_size += sizeof(tracker_t) * num_of_species;
    buffer_size += sizeof(tracker_t) * sim_lattice_size * 2;
    return buffer_size;
}

byte_array_t alloc_sim_state_buffer(size_t sim_lattice_size, size_t num_of_species)
{
    return allocate_buffer(calc_sim_state_size(sim_lattice_size, num_of_species), sizeof(byte_t));
}

sim_state_t create_sim_state_access(const byte_array_t* sim_state_buffer, size_t sim_lattice_size, size_t num_of_species)
{
    sim_state_t sim_state;
    sim_state.timer_state = (timer_state_t) { (timer_col_t*)sim_state_buffer->start_it };

    sim_state.lattice_state.start_it = (species_t*) (((byte_t*)sim_state.timer_state.timer_col) + sizeof(timer_col_t));
    sim_state.lattice_state.end_it = sim_state.lattice_state.start_it + sim_lattice_size;

    sim_state.counter_state.start_it = (counter_col_t*) sim_state.lattice_state.end_it;
    sim_state.counter_state.end_it = sim_state.counter_state.start_it + num_of_species;

    sim_state.type_tracking_state.start_it = (tracker_t*) sim_state.counter_state.end_it;
    sim_state.type_tracking_state.end_it = sim_state.type_tracking_state.start_it + num_of_species;

    sim_state.dynamic_tracking_state.start_it = sim_state.type_tracking_state.end_it;
    sim_state.dynamic_tracking_state.end_it = sim_state.dynamic_tracking_state.start_it + sim_lattice_size;

    sim_state.static_tracking_state.start_it = sim_state.dynamic_tracking_state.end_it;
    sim_state.static_tracking_state.end_it = sim_state.static_tracking_state.start_it + sim_lattice_size;
    return sim_state;
}

int load_sim_state_buffer(const char* file_path, byte_array_t sim_state_buffer)
{

}