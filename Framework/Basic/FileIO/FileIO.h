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
bool_t IsAccessibleFile(const char* restrict fileName);

// Calculates the size of a file. Returns the number of bytes or MC_FILE_ERROR on failure
cerror_t GetFileSize(file_t* restrict fileStream);

// Print a buffer as binary to a file stream. The file has to be opened in "wb" mode
error_t WriteBufferToStream(file_t* restrict fileStream, const buffer_t* restrict buffer);

// Print any array of bytes to the target stream in hexadecimal unsigend bytes with the provided number of bytes per line
error_t WriteBufferHexToStream(file_t* restrict fileStream, const buffer_t* restrict buffer, size_t bytesPerLine);

// Print any array of bytes to the target stream in hexadecimal unsigend integers with the provided number of blocks per line
error_t WriteBlockHexToStream(file_t* restrict fileStream, const memblock_array_t* restrict blockArray, size_t blocksPerLine);

// Loads a file as binary into the memory. Size is autodetermined. The file has to be opened in "rb" mode
error_t LoadBufferFromStream(file_t* restrict fileStream, buffer_t* restrict buffer);

// Loads the contents of the provided file as binary into memory and creates the buffer access struct. Returna MC_NO_ERROR on success or error-code otherwise
error_t LoadBufferFromFile(const char* restrict fileName, buffer_t* restrict outBuffer);

// Binary write of the provided buffer to the file. Supports file modes "wb" and "ab"
error_t WriteBufferToFile(const char* restrict fileName, const char* restrict fileMode, const buffer_t* restrict buffer);

// Binary write of the provided buffer to the file. Supports file modes "wb" and "ab". Backups original and deletes backup only if the write is successfully completed
error_t SaveWriteBufferToFile(const char* restrict fileName, const char* restrict fileMode, const buffer_t* restrict buffer);

// Concats two strings into a new buffer without freeing the originals. Retruns out of memory flag if allocation fails
error_t ConcatStrings(const char* lhs, const char* rhs, char** result);

// Ensures that the passed file is delted. Returns true if the file was not already deleted
bool_t EnsureFileIsDeleted(char const * restrict filePath);