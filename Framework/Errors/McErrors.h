//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	Errors.h         		    //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Error codes + messages      //
//////////////////////////////////////////

#pragma once
#include <time.h>
#include <stdint.h>
#include <stdio.h>
#include <stdlib.h>

// Defines error codes to be a 32 bit signed integer. Returns type of functions that can return error codes
typedef int32_t error_t;

// Defines long error codes to be 64 bit signed integer. Returns type of functions that perform count operations and might return negatice error codes
typedef int64_t cerror_t;

// Defines the path to the debug stderr dump folder
#define MC_STDERR_FILE_PATH "./Debug/stderr.log"

// Defines the error code for no error
#define MC_NO_ERROR 0

// Defines the error code for errors during stream usage
#define MC_STREAM_ERROR -1

// Defines the error code for errors during memory block dumping due to incompatible dump buffer length
#define MC_BLOCK_DUMP_ERROR -2

// Defines the error code for errors during array access type casting where accessing thorugh the new type would cause a buffer overflow
#define MC_BUFFER_OVERFLOW_CAST -3

// Defines the error code for buffer overflows
#define MC_BUFFER_OVERFLOW -4

// Defines the error code for file errors
#define MC_FILE_ERROR -5

// Defines the error codes for cases where a wrong file mode was passed to a function
#define MC_FILE_MODE_ERROR -6

// Defines the error code for cases where the database access caused an unspecified error
#define MC_DB_ERROR -7

// Defines the error code for cases where simulation routines cause an unspecififed error
#define MC_SIM_ERROR -8

// Defines the error code for cases where memory allocation fails
#define MC_MEM_ALLOCATION_ERROR -9

// Defines the simulator error dump macro. Dumps error information to stderr and quits programm with error code
#define MC_DUMP_ERROR_AND_EXIT(__CODE, __MSG) OnErrorExit(__CODE, __FILE__, __LINE__, __MSG);

// Defines the simulator error and memory dump macro. Dumps error information to stderr and quits programm with error code
#define MC_DUMP_ERROR_MEMORY_AND_EXIT(__CODE, __MSG, __BSTART, __BEND) OnErrorExitWithMemDump(__CODE, __FILE__, __LINE__, __MSG, __BSTART, __BEND);

// Dumps the passed error information to stderr and exists the program with the provided code. 
void OnErrorExit(int32_t errCode, const char* errFile, int32_t errLine, const char* errMsg);

// Dumps the passed error information to stderr and exists the program with the provided code. Dumps memory bytes between passed byte pointers
void OnErrorExitWithMemDump(int32_t errCode, const char* errFile, int32_t errLine, const char* errMsg, uint8_t* memStart, uint8_t* memEnd);

// Invokes the passed function pointer and returns the elapsed time in milliseconds
static inline double InvokeAndProfile(void (*func)(void))
{
    clock_t start = clock();
    (*func)();
    clock_t end = clock();
    return 1000.0*(double)(start - end)/(double)CLOCKS_PER_SEC;
}