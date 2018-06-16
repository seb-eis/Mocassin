#include <stdio.h>
#include <stdlib.h>
#include "Framework/Math/Random/PcgRandom.h"
#include "Framework/Math/Types/Vector.h"
#include "Framework/Structs/Array/Array.h"
#include "Framework/Errors/McErrors.h"
#include "Simulator/States/SimStates.h"

int main(int argc, char const * const *argv)
{ 
    size_t lattice_size = 16*16*16*32;
    size_t num_of_species = 4;
    byte_array_t sim_buffer = alloc_sim_state_buffer(lattice_size, num_of_species);
    size_t buffer_size = get_buffer_size(&sim_buffer);
    sim_state_t sim_state = create_sim_state_access(&sim_buffer, lattice_size, num_of_species);

    for (uint32_t* it = sim_buffer.start_it; it < sim_buffer.end_it; it++)
    {
        *it = pcg32_global_next();
    }

    FILE* file_stream = fopen("./Debug/state_hex.log", "w");
    formatted_buffer_dump(&sim_buffer, file_stream, 16);
    fclose(file_stream);
    
    return (0);
}