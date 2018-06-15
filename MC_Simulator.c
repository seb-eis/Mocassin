#include <stdio.h>
#include <stdlib.h>
#include "Math/Random/PcgRandom.h"
#include "Math/Types/Vector.h"
#include "Structs/Array/Array.h"
#include "Errors/McErrors.h"

int main(int argc, char const * const *argv)
{ 
    byte_array_t block_buffer = allocate_buffer(100, 4);
    for(uint32_t* it = (uint32_t*)block_buffer.start_it; it < (uint32_t*)block_buffer.end_it; it++)
    {
        *it = (uint32_t)pcg32_global_next();
    }
    formatted_buffer_dump(&block_buffer, stdout, 10);
    free(block_buffer.start_it);
    return (0);
}