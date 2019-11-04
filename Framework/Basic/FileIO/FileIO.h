//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	FileIO.h        		    //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Basic file IO functions     //
//////////////////////////////////////////

#pragma once
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

// Defines the string type
typedef char* string_t;

// Defines the list type for multiple character arrays
// Layout@ggc_x86_64 => 24@[8,8,8]
typedef List_t(string_t, StringList) StringList_t;

// Wrapper around fopen() that handles the input arguments as utf8 encoded strings
file_t* fopen_utf8(const char* restrict fileName, const char* restrict fileMode);

// Wrapper around remove() that handles the input arguments as utf8 encoded strings
error_t remove_utf8(const char* restrict fileName);

// Wrapper around rename() that handles the input arguments as utf8 encoded strings
error_t rename_utf8(const char* restrict fileName, const char* restrict newfileName);

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

// Ensures that the passed file is deleted. Returns true if the file was not already deleted
bool_t EnsureFileIsDeleted(char const * restrict filePath);

// Clears the pending stdint input buffer
void ClearStdintBuffer();

// Creates  a list of all files in the passed root path that match a pattern (With optional flag for search of subdirectories)
error_t ListAllFilesByPattern(const char* root, const char* pattern, bool_t includeSubdirs, StringList_t*restrict outList);

#if defined(WIN32)
#include <wchar.h>
#include <windows.h>
// Uses the Win32 API to convert from UTF8 to UTF16. Returns the number of characters or a negative value on failure
static inline error_t Win32ConvertUtf8ToUtf16(const char* utf8, wchar_t** utf16)
{
    let charCount = MultiByteToWideChar(CP_UTF8, 0, utf8, -1, *utf16, 0);
    return_if(charCount <= 0, charCount);
    *utf16 = malloc(charCount * sizeof(wchar_t));
    return MultiByteToWideChar(CP_UTF8, 0, utf8, -1, *utf16, charCount);
}

// Uses the Win32 API to convert from UTF16 to UTF8. Returns the number of characters or a negative value on failure
static inline error_t Win32ConvertUtf16ToUtf8(const wchar_t* utf16, char** utf8)
{
    let charCount = WideCharToMultiByte(CP_UTF8, 0, utf16, -1, *utf8, 0, NULL, NULL);
    return_if(charCount <= 0, charCount);
    *utf8 = malloc(charCount);
    return WideCharToMultiByte(CP_UTF8, 0, utf16, -1, *utf8, charCount, NULL, NULL);
}
#endif