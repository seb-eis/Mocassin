//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	Array.h         		    //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Array struct + alloc macro  //
//////////////////////////////////////////

#pragma once
#include <stdint.h>
#include <stdio.h>

// Defines the default byte to be of unsigned int8 type
typedef uint8_t byte_t;

// Defines the signed byte to be of unsigned int8 type
typedef uint8_t sbyte_t;

// Defines the default byte block to an uint32 value
typedef uint32_t block_t;

// Defines the undefined dynamic array boudary type with byte pointers to start and end point
typedef struct {byte_t* start_it; byte_t* end_it;} byte_array_t;

// Define a new named dynamic sized array that supports startd and end iterators
#define DEFINE_DYNAMIC_ARRAY(name, type) typedef struct { type* start_it; type* end_it; } name;

// Define a new named fixed size array that supports start and end iterators
#define DEFINE_FIXED_ARRAY(name, type, size) typedef struct { type* start_it; type* end_it; type buffer[size]; } name;

// Define a new named fixed size buffer that supports start and end iterators
#define DEFINE_FIXED_BUFFER(name, size) DEFINE_FIXED_ARRAY(name, byte_t, size);

// Define a new named dynamic sized buffer that supports start and end iterators
#define DEFINE_DYNAMIC_BUFFER(name) DEFINE_DYNAMIC_ARRAY(name, byte_t);

// Allocate a new object of the specfified type on the heap
#define MALLOC_OBJECT(type) (type*) malloc(sizeof(type));

// Basic dynamic byte buffer definition. Carries start and end iterator pointers
DEFINE_DYNAMIC_BUFFER(buffer_t);

// Basic dynamic int16_t array definition. Carries start and end iterator pointers
DEFINE_DYNAMIC_ARRAY(int16_array_t, int16_t);

// Basic dynamic int32_t array definition. Carries start and end iterator pointers
DEFINE_DYNAMIC_ARRAY(int32_array_t, int32_t);

// Basic dynamic int64_t array definition. Carries start and end iterator pointers
DEFINE_DYNAMIC_ARRAY(int64_array_t, int64_t);

// Basic dynamic uint16_t array definition. Carries start and end iterator pointers
DEFINE_DYNAMIC_ARRAY(uint16_array_t, uint16_t);

// Basic dynamic uint32_t array definition. Carries start and end iterator pointers
DEFINE_DYNAMIC_ARRAY(uint32_array_t, uint32_t);

// Basic dynamic uint64_t array definition. Carries start and end iterator pointers
DEFINE_DYNAMIC_ARRAY(uint64_array_t, uint64_t);

// Basic dynamic size_t array definition. Carries start and end iterator pointers
DEFINE_DYNAMIC_ARRAY(size_array_t, size_t);

// Allocate a new array of size and byte count per entry. Returns a byte array type with pointers to entries 0 and End+1
byte_array_t allocate_array(size_t array_size, size_t entry_bytes)
{
    size_t buffer_size = array_size * entry_bytes;
    byte_t* start = malloc(buffer_size);
    byte_t* end = start +buffer_size;
    return (byte_array_t) {start,end};
}

// Allocate a block buffer with the specfified number of blocks. Each block features 8 bytes
byte_array_t allocate_block_buffer(size_t num_of_blocks)
{
    return allocate_array(sizeof(byte_t) * num_of_blocks, 1);
}

// Calculates the size of a byte array type with the passed block size
size_t get_array_size(const byte_array_t* byte_array, size_t block_size)
{
    return ((size_t) byte_array->end_it - (size_t) byte_array->start_it) / block_size;
}

// Print any array of bytes. Always possible
void print_array_bytes(const byte_array_t* array_def)
{
    for(byte_t* it = (byte_t*)array_def->start_it; it < (byte_t*)array_def->end_it; it++)
    {
        printf("%02x ", *it);
    }   
}

// Print any array as blocks of int32_t. Terminates program if buffer byte size of array is not 4*N
void print_array_blocks(const byte_array_t* array_def)
{
    for(block_t* it = (block_t*)array_def->start_it; it < (block_t*)array_def->end_it; it++)
    {
        printf("%08x ", *it);
    }
}

