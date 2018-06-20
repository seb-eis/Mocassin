//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	BaseTypes.h        		    //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Array + type definitions    //
//////////////////////////////////////////

#pragma once
#include <stdint.h>
#include <stdio.h>
#include <stdlib.h>

// Define the restrict keyword for GCC
#define restrict __restrict__

// Macro to define a new named dynamic sized array that supports startd and end iterators
#define DEFINE_DYNAMIC_ARRAY(name, type) typedef struct { type* start_it; type* end_it; } name;

// Macro to define a new named fixed size array that supports start and end iterators
#define DEFINE_FIXED_ARRAY(name, type, size) typedef struct { type* start_it; type* end_it; type buffer[size]; } name;

// MAcro to define a new named dynamic sized buffer that supports start and end iterators
#define DEFINE_DYNAMIC_BUFFER(name) DEFINE_DYNAMIC_ARRAY(name, byte_t);

// Allocate a new object of the specfified type on the heap
#define MALLOC_OBJECT(type) (type*) malloc(sizeof(type));

// Casts any kind of array buffer access struct to another accessor type without checking for save conversion
#define BUFFER_CAST(buffer, type) {(type*) buffer.start_it, (type*) buffer.end_it }

// Defines the default byte to be of unsigned int8 type
typedef uint8_t byte_t;

// Defines the signed byte to be of unsigned int8 type
typedef uint8_t sbyte_t;

// Defines the default memory block to an uint32 value
typedef uint32_t memblock_t;

// Defines the bool to be one unsigned byte
typedef byte_t bool_t;

// Defines the values for true and false
enum { false = 0, true = 1 };

// Defines a dyanmic buffer access struct with const iterators to first byte and last byte + 1
typedef struct { byte_t* start_it; byte_t* end_it; } buffer_t;

// Defines a dynamic blockwise buffer access with const iterators to first block and last block + 1
typedef struct { memblock_t* start_it; memblock_t* end_it; } memblock_array_t;

// Basic dynamic int16_t array definition. Carries start and end iterator pointers
typedef struct { int16_t* start_it; int16_t* end_it; } int16_array_t;

// Basic dynamic int32_t array definition. Carries start and end iterator pointers
typedef struct { int32_t* start_it; int32_t* end_it; } int32_array_t;

// Basic dynamic int64_t array definition. Carries start and end iterator pointers
typedef struct { int64_t* start_it; int64_t* end_it; } int64_array_t;

// Basic dynamic uint16_t array definition. Carries start and end iterator pointers
typedef struct { uint16_t* start_it; uint16_t* end_it; } uint16_array_t;

// Basic dynamic uint32_t array definition. Carries start and end iterator pointers
typedef struct { uint32_t* start_it; uint32_t* end_it; } uint32_array_t;

// Basic dynamic uint64_t array definition. Carries start and end iterator pointers
typedef struct { uint64_t* start_it; uint64_t* end_it; } uint64_array_t;

// Basic dynamic size_t array definition. Carries start and end iterator pointers
typedef struct { size_t* start_it; size_t* end_it; } size_array_t;

// Get the number of bytes accessible thorugh the probided byte buffer access struct
static inline size_t get_buffer_size(const buffer_t * byte_array)
{
    return ((size_t) byte_array->end_it - (size_t) byte_array->start_it);
}

// Calculates how many overflow bytes exist if a buffer is used as an array of with specfified entry block size
static inline size_t get_overflow_byte_count(const buffer_t* byte_array, size_t block_size)
{
    return get_buffer_size(byte_array) % block_size;
}

// Calculates the size of a byte buffer with the provided block size. Does not check for under or oversize
static inline size_t get_unchecked_size(const buffer_t* byte_array, size_t block_size)
{
    return get_buffer_size(byte_array) / block_size;
}

// Allocate a new buffer that holds the minmum number of memory blocks to hold the requested number of bytes and returns a buffer access struct to them.
static inline buffer_t allocate_buffer(size_t array_size, size_t entry_bytes)
{
    size_t buffer_size = array_size * entry_bytes;
    byte_t* start = malloc(buffer_size + (sizeof(memblock_t) - buffer_size % sizeof(memblock_t)));
    byte_t* end = start + buffer_size;
    return (buffer_t) {start,end};
}

// Allocate a new buffer that holds the specfified number of 4 byte memory blocks. Returns a byte array access struct to the buffer
static inline buffer_t allocate_block_buffer(size_t num_of_blocks)
{
    return allocate_buffer(num_of_blocks, sizeof(memblock_t));
}

// Free the memory allocation defined by the passed byte array buffer access struct
static inline void free_buffer(buffer_t* byte_array)
{
    free((byte_t*)byte_array->start_it);
}

// Compares if two buffers contain identical binary values (1) or not (0)
static bool_t buffer_is_identical(const buffer_t* buffer_0, const buffer_t* buffer_1)
{
    if (buffer_0->start_it == buffer_1->start_it && buffer_0->end_it == buffer_1->end_it)
    {
        return true;
    }
    if (get_buffer_size(buffer_0) != get_buffer_size(buffer_1))
    {
        return false;
    }
    byte_t* it_0 = buffer_0->start_it;
    byte_t* it_1 = buffer_1->start_it;
    while (it_0 != buffer_0->end_it)
    {
        if (*(++it_0) != *(++it_1))
        {
            return false;
        }
    }
    return true;
}