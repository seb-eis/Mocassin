//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	Array.h         		    //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Array + buffer definitions  //
//////////////////////////////////////////

#pragma once
#include <stdint.h>
#include <stdio.h>
#include <stdlib.h>
#include "../Errors/McErrors.h"

// Defines the default byte to be of unsigned int8 type
typedef uint8_t byte_t;

// Defines the signed byte to be of unsigned int8 type
typedef uint8_t sbyte_t;

// Defines the default memory block to an uint32 value
typedef uint32_t memblock_t;

// Defines a dynmaic array access with interator pointers to the start byte and first after end byte
typedef struct { byte_t* start_it; byte_t* end_it; } byte_array_t;

// Defines a dynamic array access with interator pointers to the start byte and first after end byte
typedef struct { uint32_t* start_it; uint32_t* end_it; } memblock_array_t;

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

// Checks if casting a byte buffer access struct to an access struct of a new entry byte count could cause invalid memory access. Returns 0 if ok
int cast_is_memory_save(const byte_array_t* byte_array, size_t new_entry_size);

// Allocate a new buffer that holds as many bytes as defined by the number of blocks and bytes per block. Returns a byte array access struct to the buffer
byte_array_t allocate_buffer(size_t num_of_blocks, size_t bytes_per_block);

// Allocate a new buffer that holds the specfified number of 4 byte memory blocks. Returns a byte array access struct to the buffer
byte_array_t allocate_block_buffer(size_t num_of_blocks);

// Get the number of bytes accessible thorugh the probided byte buffer access struct
size_t get_buffer_size(const byte_array_t * byte_array);

// Calculates the size of a byte buffer with the provided block size. Does not check for under or oversize
size_t get_unchecked_size(const byte_array_t* byte_array, size_t block_size);

// Calculates the size remainder of a byte buffer. If the remainder is zero the buffer can be safely accessed using entries of the defined size
size_t get_array_remainder(const byte_array_t* byte_array, size_t block_size);

// Print any array of bytes to the target stream in hexadecimal unsigned 8 bit integer blocks
int byte_dump_memory(const byte_array_t* byte_array, void* target_stream);

// Print any array of bytes to the target stream in hexadecimal unsigned 32 bit integer blocks
int block_dump_memory(const byte_array_t* byte_array, void* target_stream);
