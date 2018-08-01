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

// Check if a file name points to an existing file
bool_t file_exists(const char* restrict file_name);

// Calculates the size of a file. Returns the number of bytes or MC_FILE_ERROR on failure
cerror_t get_file_size(file_t* restrict f_stream);

// Print a buffer as binary to a file stream. The file has to be opened in "wb" mode
error_t write_buffer_to_stream(file_t* restrict f_stream, const buffer_t* restrict buffer_in);

// Print any array of bytes to the target stream in hexadecimal unsigend bytes with the provided number of bytes per line
error_t write_buffer_hex_to_stream(file_t* restrict f_stream, const buffer_t* restrict buffer_in, size_t bytes_per_line);

// Print any array of bytes to the target stream in hexadecimal unsigend integers with the provided number of blocks per line
error_t write_block_hex_to_stream(file_t* restrict f_stream, const memblock_array_t* restrict block_array, size_t blocks_per_line);

// Loads a file as binary into the memory. Size is autodetermined. The file has to be opened in "rb" mode
error_t load_buffer_from_stream(file_t* restrict f_Stream, buffer_t* restrict byte_array);

// Loads the contents of the provided file as binary into memory and creates the buffer access struct. Returna MC_NO_ERROR on success or error-code otherwise
error_t load_buffer_from_file(const char* restrict file_name, buffer_t* restrict buffer_out);

// Binary write of the provided buffer to the file. Supports file modes "wb" and "ab"
error_t write_buffer_to_file(const char* restrict file_name, const char* restrict file_mode, const buffer_t* restrict buffer_in);