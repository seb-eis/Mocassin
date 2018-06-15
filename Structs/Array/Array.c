//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	Array.c         		    //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Array + buffer definitions  //
//////////////////////////////////////////

#include "Array.h"
#include "Errors/McErrors.h"

int byte_dump_memory(const byte_array_t* byte_array, void* target_stream)
{
    for(byte_t* it = byte_array->start_it; it < byte_array->end_it; it++)
    {
        fprintf(target_stream, "%02x ", *it);
    }
    return MC_NO_ERROR;
}

int block_dump_memory(const byte_array_t* byte_array, void* target_stream)
{
    if (get_overflow_byte_count(byte_array, sizeof(memblock_t)) != 0)
    {
        return MC_BUFFER_OVERFLOW_CAST;
    }
    for(memblock_t* it = (memblock_t*)byte_array->start_it; it < (memblock_t*)byte_array->end_it; it++)
    {
        fprintf(target_stream, "%08x ", *it);
    }
    return MC_NO_ERROR;
}

void formatted_buffer_dump(const byte_array_t* byte_array, void* target_stream, size_t bytes_per_line)
{
    size_t line_counter = 0;
    for (byte_t* it = byte_array->start_it; it < byte_array->end_it; it++)
    {
        fprintf(target_stream, "%02x ", *it);
        if  (++line_counter == bytes_per_line)
        {
            fprintf(target_stream, "\n");
            line_counter = 0;
        }
    }
}