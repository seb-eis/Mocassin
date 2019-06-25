//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	FileIO.h        		    //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Basic file IO functions     //
//////////////////////////////////////////

#define __STDC_FORMAT_MACROS
#include <inttypes.h>
#include <stdio.h>
#include <dirent.h>
#include <stdint.h>
#include <stdlib.h>
#include <unistd.h>
#include <string.h>
#include "Framework/Errors/McErrors.h"
#include "Framework/Basic/BaseTypes/BaseTypes.h"
#include "Framework/Basic/BaseTypes/Buffers.h"

#define FORMAT_I8(...)   "%" #__VA_ARGS__ PRIi8
#define FORMAT_I16(...)  "%" #__VA_ARGS__ PRIi16
#define FORMAT_I32(...)  "%" #__VA_ARGS__ PRIi32
#define FORMAT_I64(...)  "%" #__VA_ARGS__ PRIi64

#define FORMAT_U8(...)   "%" #__VA_ARGS__ PRIu8
#define FORMAT_U16(...)  "%" #__VA_ARGS__ PRIu16
#define FORMAT_U32(...)  "%" #__VA_ARGS__ PRIu32
#define FORMAT_U64(...)  "%" #__VA_ARGS__ PRIu64

// Defines the file stream type
typedef FILE file_t;

// Check if a file name points to an existing file that can be accessed
bool_t IsAccessibleFile(const char* restrict fileName);

// Check if a file name points to an existing directory that can be accessed
bool_t IsAccessibleDirectory(const char* restrict dirName);

// Calculates the size of a file. Returns the number of bytes or MC_FILE_ERROR on failure
cerror_t CalculateFileSize(file_t *restrict fileStream);

// Print a buffer as binary to a file stream. The file has to be opened in "wb" mode
error_t WriteBufferToStream(file_t* restrict fileStream, const Buffer_t* restrict buffer);

// Print any array of bytes to the target stream in hexadecimal unsigned bytes with the provided number of bytes per line
error_t WriteBufferHexToStream(file_t* restrict fileStream, const Buffer_t* restrict buffer, size_t bytesPerLine);

// Loads a file as binary into the provided buffer. The file has to be opened in "rb" mode and the buffer
error_t LoadBufferFromStream(file_t* restrict fileStream, Buffer_t* restrict buffer);

// Loads the contents of the provided file as binary into memory and creates the buffer access struct. Returns ERR_OK on success or error-code otherwise
error_t LoadBufferFromFile(const char* restrict fileName, Buffer_t* restrict outBuffer);

// Binary write of the provided buffer to the file. Supports file modes "wb" and "ab"
error_t WriteBufferToFile(const char* restrict fileName, const char* restrict fileMode, const Buffer_t* restrict buffer);

// Binary write of the provided buffer to the file. Supports file modes "wb" and "ab". Backups original and deletes backup only if the write is successfully completed
error_t SaveWriteBufferToFile(const char* restrict fileName, const char* restrict fileMode, const Buffer_t* restrict buffer);

// Concats two strings into a new buffer without freeing the originals. Retruns out of memory flag if allocation fails
error_t ConcatStrings(const char* lhs, const char* rhs, char** result);

// Ensures that the passed file is delted. Returns true if the file was not already deleted
bool_t EnsureFileIsDeleted(char const * restrict filePath);

// Clears the pending stdint input buffer
void ClearStdintBuffer();