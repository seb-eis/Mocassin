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

byte_array_t allocate_buffer(size_t array_size, size_t entry_bytes)
{
    size_t buffer_size = array_size * entry_bytes;
    byte_t* start = malloc(buffer_size);
    byte_t* end = start +buffer_size;
    return (byte_array_t) {start,end};
}

byte_array_t allocate_block_buffer(size_t num_of_blocks)
{
    return allocate_buffer(sizeof(byte_t) * num_of_blocks, 1);
}

size_t get_buffer_size(const byte_array_t * byte_array)
{
    return ((size_t) byte_array->end_it - (size_t) byte_array->start_it);
}

size_t get_array_remainder(const byte_array_t* byte_array, size_t block_size)
{
    return get_buffer_size(byte_array) % block_size;
}

size_t get_unchecked_size(const byte_array_t* byte_array, size_t block_size)
{
    return get_buffer_size(byte_array) / block_size;
}

int byte_dump_memory(const byte_array_t* byte_array, void* target_stream)
{
    for(byte_t* it = (byte_t*)byte_array->start_it; it < (byte_t*)byte_array->end_it; it++)
    {
        fprintf(target_stream, "%02x ", *it);
    }
    return MC_NO_ERROR;
}

int block_dump_memory(const byte_array_t* byte_array, void* target_stream)
{
    if (cast_is_memory_save(byte_array, sizeof(memblock_t)) != 0)
    {
        return MC_INVALID_ARRAY_CAST;
    }
    for(memblock_t* it = (memblock_t*)byte_array->start_it; it < (memblock_t*)byte_array->end_it; it++)
    {
        fprintf(target_stream, "%08x ", *it);
    }
    return MC_NO_ERROR;
}

int cast_is_memory_save(const byte_array_t* byte_array, size_t new_entry_size)
{
    if (get_array_remainder(byte_array, new_entry_size) != 0)
    {
        return 1;
    }
    return 0;
}