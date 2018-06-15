#include <stdio.h>
#include <stdlib.h>
#include "Framework/Math/Random/PcgRandom.h"
#include "Framework/Math/Types/Vector.h"
#include "Framework/Structs/Array/Array.h"
#include "Framework/Errors/McErrors.h"

int main(int argc, char const * const *argv)
{ 
    byte_array_t block_buffer = allocate_buffer(10, 2);
    for(uint32_t* it = (uint32_t*)block_buffer.start_it; it < (uint32_t*)block_buffer.end_it; it++)
    {
        *it = (uint32_t)pcg32_global_next();
    }
    
    if (block_dump_memory(&block_buffer, stdout) == MC_NO_ERROR)
    {
        formatted_buffer_dump(&block_buffer, stdout, 24);
        free_buffer(&block_buffer);
    }
    
    return (0);
}