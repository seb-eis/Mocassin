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

// Define a debug flag in the VC style
#ifndef _DEBUG
    #define _DEBUG
#endif

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

// Defines the error code for cases where a matrix is accessed in way it doesnt match the matrix rank
#define MC_MATRIX_RANK_ERROR -7

// Defines the simulator error dump macro. Dumps error information to stderr and quits programm with error code
#define MC_DUMP_ERROR_AND_EXIT(code, message) on_error_exit(code, __FILE__, __LINE__, message);

// Defines the simulator error and memory dump macro. Dumps error information to stderr and quits programm with error code
#define MC_DUMP_ERROR_MEMORY_AND_EXIT(code, message, b_start, b_end) on_error_exit_mem_dump(code, __FILE__, __LINE__, message, b_start, b_end);

// Dumps the passed error information to stderr and exists the program with the provided code. 
int on_error_exit(int error_code, const char* error_file, int error_line, const char* error_string);

// Dumps the passed error information to stderr and exists the program with the provided code. Dumps memory bytes between passed byte pointers
int on_error_exit_mem_dump(int error_code, const char* error_file, int error_line, const char* error_string, uint8_t* mem_start, uint8_t* mem_end);

// Invokes the passed function pointer and returns the elapsed time in milliseconds
static inline double profile_invoke(void (*func_ptr)(void))
{
    clock_t start_time = clock();
    (*func_ptr)();
    clock_t end_time = clock();
    return 1000.0*(double)(end_time - start_time)/(double)CLOCKS_PER_SEC;
}