//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	FileIO.h        		    //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Basic file IO functions     //
//////////////////////////////////////////

#include <stdio.h>
#include <stdint.h>
#include <stdlib.h>
#include <unistd.h>
#include <string.h>
#include "Framework/Errors/McErrors.h"
#include "Framework/Basic/BaseTypes/BaseTypes.h"

// Defines the file stream type
typedef FILE file_t;

// Calculates the size of a file. Returns the number of bytes or MC_FILE_ERROR on failure
int64_t get_file_size(file_t* f_stream);

// Check if a file name points to an existing file
bool_t file_exists(const char* file_name);

// Print a buffer as binary to a file stream. The file has to be opened in "wb" mode
int32_t write_buffer_to_stream(file_t* f_stream, buffer_t* buffer_in);

// Print any array of bytes to the target stream in hexadecimal unsigend bytes with the provided number of bytes per line
int32_t write_buffer_hex_to_stream(file_t* f_stream, const buffer_t* buffer_in, size_t bytes_per_line);

// Print any array of bytes to the target stream in hexadecimal unsigend integers with the provided number of blocks per line
int32_t write_block_hex_to_stream(file_t* f_stream, const memblock_array_t* block_array, size_t blocks_per_line);

// Loads a file as binary into the memory. Size is autodetermined. The file has to be opened in "rb" mode
int32_t load_buffer_from_stream(file_t* source_stream, buffer_t* byte_array);

// Loads the contents of the provided file as binary into memory and creates the buffer access struct. Returna MC_NO_ERROR on success or error-code otherwise
int32_t load_buffer_from_file(const char* file_name, buffer_t* buffer_out);

// Binary write of the provided buffer to the file. Supports file modes "wb" and "ab"
int32_t write_buffer_to_file(const char* file_name, const char* file_mode, buffer_t* buffer_in);