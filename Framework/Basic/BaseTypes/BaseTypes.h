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
#define ARRAY_OF(...) struct { __VA_ARGS__* Start; __VA_ARGS__* End; }

// Defines a list of a specific type with a start pointer, an end pointer and a pointer to the current end of the list
#define LIST_OF(...) struct { __VA_ARGS__* Start; __VA_ARGS__* End; __VA_ARGS__* CurEnd; }

// Macro to define a new named dynamic sized array that supports startd and end iterators
#define DEFINE_DYNAMIC_ARRAY(__NAME, __TYPE) typedef struct { __TYPE* Start; __TYPE* End; } __NAME;

// MAcro to define a new named dynamic sized buffer that supports start and end iterators
#define DEFINE_DYNAMIC_BUFFER(name) DEFINE_DYNAMIC_ARRAY(name, byte_t);

// Allocate a new object of the specfified type on the heap
#define MALLOC_OBJECT(type) (type*) malloc(sizeof(type));

// Casts any kind of array buffer access struct to another accessor type without checking for save conversion
#define BUFFER_TO_ARRAY(__BUFFER, __TYPE) (__TYPE) { (void*) (__BUFFER).Start, (void*) (__BUFFER).End }

// Creates a list access struct from the passed buffer where the current item ptr points to the first element
#define BUFFER_TO_LIST(__BUFFER, __TYPE) (__TYPE) { (void*) (__BUFFER).Start, (void*) (__BUFFER).End, (void*) (__BUFFER).Start }

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
typedef struct { byte_t* Start; byte_t* End; } buffer_t;

// Basic dynamic signed byte array definition. Carries start and end iterator pointers
typedef struct { sbyte_t* Start; sbyte_t* End; } sbuffer_t;

// Basic dynamic memblock array definition. Carries start and end iterator pointers
typedef struct { memblock_t* Start; memblock_t* End; } memblock_array_t;

// Basic dynamic int16_t array definition. Carries start and end iterator pointers
typedef struct { int16_t* Start; int16_t* End; } int16_array_t;

// Basic dynamic int32_t array definition. Carries start and end iterator pointers
typedef struct { int32_t* Start; int32_t* End; } int32_array_t;

// Basic dynamic int64_t array definition. Carries start and end iterator pointers
typedef struct { int64_t* Start; int64_t* End; } int64_array_t;

// Basic dynamic uint16_t array definition. Carries start and end iterator pointers
typedef struct { uint16_t* Start; uint16_t* End; } uint16_array_t;

// Basic dynamic uint32_t array definition. Carries start and end iterator pointers
typedef struct { uint32_t* Start; uint32_t* End; } uint32_array_t;

// Basic dynamic uint64_t array definition. Carries start and end iterator pointers
typedef struct { uint64_t* Start; uint64_t* End; } uint64_array_t;

// Basic dynamic size_t array definition. Carries start and end iterator pointers
typedef struct { size_t* Start; size_t* End; } size_array_t;

// Basic dynamic double array definition. Carries startd and end iterator pointers
typedef struct { double* Start; double* End; } double_array_t;

// Defines the id redirect type to be an int32_t array
typedef int32_array_t id_redirect_t;

// Basic dynamic int32_t list definition. Supports start, end and current end iterator 
typedef LIST_OF(int32_t) int32_list_t;

// Define a new list of the given type with a specified type name
#define DEFINE_LIST(__NAME, __TYPE) typedef LIST_OF(__TYPE) __NAME

// Get the pointer to the current last entry of a list
#define LIST_GET_LAST_PTR(__LIST) ((__LIST).CurEnd - 1)

// Moves the entry iterator for the last entry one down so it points to the actual last value
#define LIST_POP_BACK(__LIST) (--(__LIST).CurEnd);

// Sets the current end iterator the the passed value and advances the iterator to the new end
#define LIST_ADD(__LIST, __VALUE) *((__LIST).CurEnd++) = (__VALUE)

// Calculates the size of a list from start iterator, end iterator and the number of bytes per entry
#define LIST_GET_SIZE(__LIST, __BYTES_PER_ENTRY) (((__LIST).CurEnd - (__LIST).Start) / __BYTES_PER_ENTRY)

// Get the last index of a list
#define LIST_GET_LAST_INDEX(__LIST, __BYTES_PER_ENTRY) (LIST_GET_SIZE(__LIST, __BYTES_PER_ENTRY) - 1)

// Access an array by index and gets the value (Can also be used to set the value)
#define ARRAY_GET(__ARRAY, __INDEX) (__ARRAY).Start[__INDEX]

// Access an array and returns the pointer to the specified index
#define ARRAY_GET_PTR(__ARRAY, __INDEX) ((__ARRAY).Start + __INDEX)

// Checks if the start iterator of an array is a null pointer
#define IS_NULL_ARRAY(__ARRAY) ((__ARRAY).Start == NULL)

// Get the number of bytes accessible thorugh the probided byte buffer access struct
static inline size_t GetBufferSize(const buffer_t * restrict inBuffer)
{
    return ((size_t) inBuffer->End - (size_t) inBuffer->Start);
}

// Calculates how many overflow bytes exist if a buffer is used as an array of with specfified entry block size
static inline size_t GetOverflowByteCount(const buffer_t* restrict inBuffer, size_t blockSize)
{
    return GetBufferSize(inBuffer) % blockSize;
}

// Calculates the size of a byte buffer with the provided block size. Does not check for under or oversize
static inline size_t GetBufferItemCount(const buffer_t* restrict inBuffer, size_t blockSize)
{
    return GetBufferSize(inBuffer) / blockSize;
}

// Allocate a new buffer that holds the minmum number of memory blocks to hold the requested number of bytes and returns a buffer access struct to them.
static inline buffer_t AllocateBufferUnchecked(const size_t arraySize, const size_t itemSize)
{
    size_t bufferSize = arraySize * itemSize;
    byte_t* start = malloc(bufferSize + (sizeof(memblock_t) - bufferSize % sizeof(memblock_t)));
    byte_t* end = start + bufferSize;
    return (buffer_t) {start,end};
}

// Allocates a new buffer savely. Returns a memory allocation error code if the malloc operation returns a null pointer
static inline error_t AllocateBufferChecked(const size_t arraySize, const size_t itemSize, buffer_t* restrict outBuffer)
{
    *outBuffer = AllocateBufferUnchecked(arraySize, itemSize);
    if (outBuffer->Start == NULL)
    {
        return MC_MEM_ALLOCATION_ERROR;
    }
    return MC_NO_ERROR;
}

// Allocate a new buffer that holds the specfified number of 4 byte memory blocks. Returns a byte array access struct to the buffer
static inline buffer_t AllocateBlockBufferUnchecked(size_t blockCount)
{
    return AllocateBufferUnchecked(blockCount, sizeof(memblock_t));
}

// Free the memory allocation defined by the passed byte array buffer access struct
static inline void FreeBuffer(buffer_t* inBuffer)
{
    free((byte_t*)inBuffer->Start);
}

// Compares if two buffers contain identical binary values (1) or not (0)
bool_t HaveSameBufferContent(const buffer_t* lhs, const buffer_t* rhs);

// Defines the basic blob type with a pointer to the Header and a buffer access struct
typedef struct { byte_t* Header; buffer_t Buffer; } blob_t;

// Defines the blob array type to store access structs for multiple blobs
DEFINE_DYNAMIC_ARRAY(blob_array_t, blob_t);

// Creates a basic blob type from a buffer and Header size information
static inline blob_t BufferToBlob(const buffer_t* restrict inBuffer, size_t headerSize)
{
    return (blob_t) { inBuffer->Start, { inBuffer->Start + headerSize, inBuffer->End } };
}

// ALlocates a blob on the heap with the specified buffer size and Header size
static inline blob_t AllocateBlobUnchecked(const size_t bufferSize, const size_t headerSize)
{
    byte_t* ptr = malloc(bufferSize + headerSize);
    return (blob_t) { ptr, { ptr + headerSize, ptr + (headerSize + bufferSize) } };
}

// Free the memory of a blob type
static inline void FreeBlob(const blob_t* restrict inBlob)
{
    free(inBlob->Header);
}

// Defines the cast of a blob type to another blb type
#define BLOB_CAST(__TYPE, __BLOB) (__TYPE) { (void*)__BLOB.Header, (void*)__BLOB.Buffer.Start, (void*)__BLOB.Buffer.End };

// Defines a new multidimensional array type that carries a pointer Header information (rank, size, blocks) and pointers to buffer start and end
#define DEFINE_MD_ARRAY(__NAME, __TYPE, __RANK) typedef struct { struct { int32_t Rank, Size; int32_t Blocks[__RANK-1]; }* Header; __TYPE* Start, * End; } __NAME;

// Calculate the 1D number of skipped items for an index and dimension combination
#define MDA_SKIP(__ARRAY, __INDEX, __DIM) ((__ARRAY).Header->Blocks[__DIM-1] * __INDEX)

// Returns the pointer to the multidimensional array entry at the specified indices
#define MDA_GET_2(__ARRAY, _A, _B) ((__ARRAY).Start + MDA_SKIP((__ARRAY), _A, 1) + _B)

// Returns the pointer to the multidimensional array entry at the specified indices
#define MDA_GET_3(__ARRAY, _A, _B, _C) ((__ARRAY).Start + MDA_SKIP((__ARRAY), _A, 1) + MDA_SKIP((__ARRAY), _B, 2) + _C)

// Returns the pointer to the multidimensional array entry at the specified indices
#define MDA_GET_4(__ARRAY, _A, _B, _C, _D) ((__ARRAY).Start + MDA_SKIP((__ARRAY), _A, 1) + MDA_SKIP((__ARRAY), _B, 2) + MDA_SKIP((__ARRAY), _C, 2) + _D)

// Defines the access struct for a 2 dimensional array of 32 bit signed integers with a Header information
DEFINE_MD_ARRAY(int32_array2_t, int32_t, 2);

// Defines the access struct for a 3 dimensional array of 32 bit signed integers with a Header information
DEFINE_MD_ARRAY(int32_array3_t, int32_t, 3);

// Determines the dimensions of an md array from the passed Header pointer and writes the results into the passed buffer
void GetMdaDimensions(const int32_t* restrict inHeader, int32_t* restrict outBuffer);

// Calculates the block sizes of an md array from the given dimension and rank information and writes it to the passed buffer
void GetMdaBlockSizes(const int32_t rank, const int32_t* restrict dimensions, int32_t* restrict outBuffer);

// Calcualtes the total size of an md array (without Header) based upon the passed rank and dimensions pointer
int32_t GetMdaSize(const size_t rank, const int32_t* restrict inHeader);

// Allocates a buffer for a multidimensional array with the passed rank, bytes per item and dimensions and returns a blob access struct to the allocated buffer
blob_t AllocateMdaUnchecked(const int32_t rank, const size_t itemSize, const int32_t* restrict dimensions);

// Allocates a buffer for a multidimensional array with the passed rank, bytes per item and dimensions. Returns an error code if the allocation returned a null ptr
error_t AllocateMdaChecked(const int32_t rank, const size_t itemSize, const int32_t* restrict dimensions, blob_t* restrict outBlob);