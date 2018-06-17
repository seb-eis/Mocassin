#include <stdio.h>
#include <stdlib.h>
#include <time.h>
#include "Framework/Math/Random/PcgRandom.h"
#include "Framework/Math/Types/Vector.h"
#include "Framework/Basic/BaseTypes/BaseTypes.h"
#include "Framework/Basic/FileIO/FileIO.h"
#include "Framework/Errors/McErrors.h"
#include "Simulator/States/SimStates.h"

int main(int argc, char const * const *argv)
{ 
    buffer_t buffer = allocate_block_buffer(10000000);
    buffer_t buffer_out;
    
    for(byte_t* it = buffer.start_it; it < buffer.end_it; it++)
    {
        *it = (byte_t) (pcg32_global_next() % UINT8_MAX);
    }
  
    if (write_buffer_to_file("./Debug/test.log", "wb", &buffer) != 0)
    {
        printf("Write failed");
    }
    
    if (load_buffer_from_file("./Debug/test.log", &buffer_out) != 0)
    {
        printf("Load failed");
    }

    if (buffer_is_identical(&buffer, &buffer_out) != true)
    {
        printf("Compare fail");
    }

    return (0);
}