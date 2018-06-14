#include <stdio.h>
#include <stdlib.h>
#include "Math/Random/PcgRandom.h"
#include "Math/Types/Vector.h"
#include "Structs/Array.h"

int main(int argc, char const * const *argv)
{ 
    byte_array_t block_buffer = allocate_array(1000000, 4);
    size_t size = get_array_size(&block_buffer, 4);
    for(uint32_t* it = (uint32_t*)block_buffer.start_it; it < (uint32_t*)block_buffer.end_it; it++)
    {
        *it = (uint32_t)pcg32_global_next();
    }
    print_array_blocks(&block_buffer);
    return (0);
}