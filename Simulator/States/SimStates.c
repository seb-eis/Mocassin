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

size_t calc_sim_state_size(const mc_state_attr_t* state_attr)
{
    size_t buffer_size = 0;
    buffer_size += sizeof(timer_col_t);
    buffer_size += sizeof(species_t) * state_attr->num_of_atoms;
    buffer_size += sizeof(counter_col_t) * state_attr->num_of_species;
    buffer_size += sizeof(tracker_t) * state_attr->num_of_species;
    buffer_size += sizeof(tracker_t) * state_attr->num_of_trackers * 2;
    buffer_size += sizeof(index_t) * state_attr->num_of_trackers * 2;
    return buffer_size;
}

buffer_t alloc_sim_state_buffer(const mc_state_attr_t* state_attr)
{
    return allocate_buffer(calc_sim_state_size(state_attr), sizeof(byte_t));
}

mc_state_t create_sim_state(const buffer_t* sim_state_buffer, const mc_state_attr_t* state_attr)
{
    mc_state_t sim_state;
    sim_state.state_buffer = *sim_state_buffer;
    sim_state.timer_state = (timer_state_t) { (timer_col_t*)sim_state_buffer->start_it };

    sim_state.lattice_state.start_it = (species_t*) (((byte_t*)sim_state.timer_state.timer_col) + sizeof(timer_col_t));
    sim_state.lattice_state.end_it = sim_state.lattice_state.start_it + state_attr->num_of_atoms;

    sim_state.counter_state.start_it = (counter_col_t*) sim_state.lattice_state.end_it;
    sim_state.counter_state.end_it = sim_state.counter_state.start_it + state_attr->num_of_species;

    sim_state.type_tracking_state.start_it = (tracker_t*) sim_state.counter_state.end_it;
    sim_state.type_tracking_state.end_it = sim_state.type_tracking_state.start_it + state_attr->num_of_species;

    sim_state.dynamic_tracking_state.start_it = sim_state.type_tracking_state.end_it;
    sim_state.dynamic_tracking_state.end_it = sim_state.dynamic_tracking_state.start_it + state_attr->num_of_trackers;

    sim_state.static_tracking_state.start_it = sim_state.dynamic_tracking_state.end_it;
    sim_state.static_tracking_state.end_it = sim_state.static_tracking_state.start_it + state_attr->num_of_trackers;

    sim_state.dyn_track_index_state.start_it = (index_t*) sim_state.static_tracking_state.end_it;
    sim_state.dyn_track_index_state.end_it = sim_state.dyn_track_index_state.start_it + state_attr->num_of_atoms;

    sim_state.stat_track_index_state.start_it = sim_state.dyn_track_index_state.end_it;
    sim_state.stat_track_index_state.end_it = sim_state.stat_track_index_state.start_it + state_attr->num_of_atoms;

    return sim_state;
}

mc_state_t load_sim_state(FILE* f_stream, const mc_state_attr_t* state_attr)
{
    mc_state_t sim_state;

    return sim_state;
}