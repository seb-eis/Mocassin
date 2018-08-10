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
#include "Framework/Errors/McErrors.h"

// Define the restrict keyword for GCC
#define restrict __restrict__

// Reinterpret pointer as another type
#define REINTERPRET_CAST(__TYPE, __VALUE) ((__TYPE*) (__VALUE))

// Defines a quick macro for arrays of specific types with pointers to end an start of the array
#define ARRAY_OF(...) struct { __VA_ARGS__* start_it; __VA_ARGS__* end_it; }

// Defines a list of a specific type with a start pointer, an end pointer and a pointer to the current end of the list
#define LIST_OF(...) struct { __VA_ARGS__* start_it; __VA_ARGS__* end_it; __VA_ARGS__* cur_end_it; }

// Macro to define a new named dynamic sized array that supports startd and end iterators
#define DEFINE_DYNAMIC_ARRAY(__NAME, __TYPE) typedef struct { __TYPE* start_it; __TYPE* end_it; } __NAME;

// MAcro to define a new named dynamic sized buffer that supports start and end iterators
#define DEFINE_DYNAMIC_BUFFER(name) DEFINE_DYNAMIC_ARRAY(name, byte_t);

// Allocate a new object of the specfified type on the heap
#define MALLOC_OBJECT(type) (type*) malloc(sizeof(type));

// Casts any kind of array buffer access struct to another accessor type without checking for save conversion
#define BUFFER_TO_ARRAY(__BUFFER, __TYPE) (__TYPE) { (void*) (__BUFFER).start_it, (void*) (__BUFFER).end_it }

// Creates a list access struct from the passed buffer where the current item ptr points to the first element
#define BUFFER_TO_LIST(__BUFFER, __TYPE) (__TYPE) { (void*) (__BUFFER).start_it, (void*) (__BUFFER).end_it, (void*) (__BUFFER).start_it }

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

// Defines the basic bitmask to be an unsigned 64 bit integer
typedef int64_t bitmask_t;

// Basic dynamic unsigned byte array definition. Carries start and end iterator pointers
typedef struct { byte_t* start_it; byte_t* end_it; } buffer_t;

// Basic dynamic signed byte array definition. Carries start and end iterator pointers
typedef struct { sbyte_t* start_it; sbyte_t* end_it; } sbuffer_t;

// Basic dynamic memblock array definition. Carries start and end iterator pointers
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

// Basic dynamic double array definition. Carries startd and end iterator pointers
typedef struct { double* start_it; double* end_it; } double_array_t;

// Basic dynamic int32_t list definition. Supports start, end and current end iterator 
typedef LIST_OF(int32_t) int32_list_t;

// Define a new list of the given type with a specified type name
#define DEFINE_LIST(__NAME, __TYPE) typedef LIST_OF(__TYPE) __NAME

// Get the pointer to the current last entry of a list
#define LIST_GET_LAST_PTR(__LIST) ((__LIST).cur_end_it - 1)

// Moves the entry iterator for the last entry one down so it points to the actual last value
#define LIST_POP_BACK(__LIST) (--(__LIST).cur_end_it);

// Sets the current end iterator the the passed value and advances the iterator to the new end
#define LIST_ADD(__LIST, __VALUE) *((__LIST).cur_end_it++) = (__VALUE)

// Calculates the size of a list from start iterator, end iterator and the number of bytes per entry
#define LIST_GET_SIZE(__LIST, __BYTES_PER_ENTRY) (((__LIST).cur_end_it - (__LIST).start_it) / __BYTES_PER_ENTRY)

// Get the last index of a list
#define LIST_GET_LAST_INDEX(__LIST, __BYTES_PER_ENTRY) (LIST_GET_SIZE(__LIST, __BYTES_PER_ENTRY) - 1)

// Access an array by index and gets the value (Can also be used to set the value)
#define ARRAY_GET(__ARRAY, __INDEX) (__ARRAY).start_it[__INDEX]

// Access an array and returns the pointer to the specified index
#define ARRAY_GET_PTR(__ARRAY, __INDEX) ((__ARRAY).start_it + __INDEX)

// Checks if the start iterator of an array is a null pointer
#define IS_NULL_ARRAY(__ARRAY) ((__ARRAY).start_it == NULL)

// Get the number of bytes accessible thorugh the probided byte buffer access struct
static inline size_t get_buffer_size(const buffer_t * restrict byte_array)
{
    return ((size_t) byte_array->end_it - (size_t) byte_array->start_it);
}

// Calculates how many overflow bytes exist if a buffer is used as an array of with specfified entry block size
static inline size_t get_overflow_byte_count(const buffer_t* restrict byte_array, size_t block_size)
{
    return get_buffer_size(byte_array) % block_size;
}

// Calculates the size of a byte buffer with the provided block size. Does not check for under or oversize
static inline size_t get_unchecked_size(const buffer_t* restrict byte_array, size_t block_size)
{
    return get_buffer_size(byte_array) / block_size;
}

// Allocate a new buffer that holds the minmum number of memory blocks to hold the requested number of bytes and returns a buffer access struct to them.
static inline buffer_t allocate_buffer(const size_t array_size, const size_t entry_bytes)
{
    size_t buffer_size = array_size * entry_bytes;
    byte_t* start = malloc(buffer_size + (sizeof(memblock_t) - buffer_size % sizeof(memblock_t)));
    byte_t* end = start + buffer_size;
    return (buffer_t) {start,end};
}

// Allocates a new buffer savely. Returns a memory allocation error code if the malloc operation returns a null pointer
static inline error_t allocate_buffer_checked(const size_t array_size, const size_t entry_bytes, buffer_t* restrict o_buffer)
{
    *o_buffer = allocate_buffer(array_size, entry_bytes);
    if (o_buffer->start_it == NULL)
    {
        return MC_MEM_ALLOCATION_ERROR;
    }
    return MC_NO_ERROR;
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
bool_t buffer_is_identical(const buffer_t* buffer_0, const buffer_t* buffer_1);

// Defines the basic blob type with a pointer to the header and a buffer access struct
typedef struct { byte_t* header; buffer_t buffer; } blob_t;

// Defines the blob array type to store access structs for multiple blobs
DEFINE_DYNAMIC_ARRAY(blob_array_t, blob_t);

// Creates a basic blob type from a buffer and header size information
static inline blob_t buffer_to_blob(const buffer_t* restrict i_buffer, size_t header_size)
{
    return (blob_t) { i_buffer->start_it, { i_buffer->start_it + header_size, i_buffer->end_it } };
}

// ALlocates a blob on the heap with the specified buffer size and header size
static inline blob_t allocate_blob(const size_t buffer_size, const size_t header_size)
{
    byte_t* ptr = malloc(buffer_size + header_size);
    return (blob_t) { ptr, { ptr + header_size, ptr + (header_size + buffer_size) } };
}

// Free the memory of a blob type
static inline void free_blob(const blob_t* restrict i_blob)
{
    free(i_blob->header);
}

// Defines the cast of a blob type to another blb type
#define BLOB_CAST(__TYPE, __BLOB) (__TYPE) { (void*)__BLOB.header, (void*)__BLOB.buffer.start_it, (void*)__BLOB.buffer.end_it };

// Defines a new multidimensional array type that carries a pointer header information (rank, size, blocks) and pointers to buffer start and end
#define DEFINE_MD_ARRAY(__NAME, __TYPE, __RANK) typedef struct { struct { int32_t rank, size; int32_t blocks[__RANK-1]; }* header; __TYPE* start_it, * end_it; } __NAME;

// Calculate the 1D number of skipped items for an index and dimension combination
#define MDA_SKIP(__ARRAY, __INDEX, __DIM) ((__ARRAY).header->blocks[__DIM-1] * __INDEX)

// Returns the pointer to the multidimensional array entry at the specified indices
#define MDA_GET_2(__ARRAY, _A, _B) ((__ARRAY).start_it + MDA_SKIP((__ARRAY), _A, 1) + _B)

// Returns the pointer to the multidimensional array entry at the specified indices
#define MDA_GET_3(__ARRAY, _A, _B, _C) ((__ARRAY).start_it + MDA_SKIP((__ARRAY), _A, 1) + MDA_SKIP((__ARRAY), _B, 2) + _C)

// Returns the pointer to the multidimensional array entry at the specified indices
#define MDA_GET_4(__ARRAY, _A, _B, _C, _D) ((__ARRAY).start_it + MDA_SKIP((__ARRAY), _A, 1) + MDA_SKIP((__ARRAY), _B, 2) + MDA_SKIP((__ARRAY), _C, 2) + _D)

// Defines the access struct for a 2 dimensional array of 32 bit signed integers with a header information
DEFINE_MD_ARRAY(int32_array2_t, int32_t, 2);

// Defines the access struct for a 3 dimensional array of 32 bit signed integers with a header information
DEFINE_MD_ARRAY(int32_array3_t, int32_t, 3);

// Determines the dimensions of an md array from the passed header pointer and writes the results into the passed buffer
void get_md_array_dimensions(const int32_t* restrict i_header, int32_t* restrict o_buffer);

// Calculates the block sizes of an md array from the given dimension and rank information and writes it to the passed buffer
void md_array_dim_to_blocks(const int32_t rank, const int32_t* restrict dimensions, int32_t* restrict o_buffer);

// Calcualtes the total size of an md array (without header) based upon the passed rank and dimensions pointer
int32_t get_md_array_size(const size_t rank, const int32_t* restrict dimensions);

// Allocates a buffer for a multidimensional array with the passed rank, bytes per item and dimensions and returns a blob access struct to the allocated buffer
blob_t allocate_md_array(const int32_t rank, const size_t item_size, const int32_t* restrict dimensions);

// Allocates a buffer for a multidimensional array with the passed rank, bytes per item and dimensions. Returns an error code if the allocation returned a null ptr
error_t allocate_md_array_checked(const int32_t rank, const size_t item_size, const int32_t* restrict dimensions, blob_t* restrict o_blob);