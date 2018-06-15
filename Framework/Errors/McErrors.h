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
#include <stdint.h>
#include <stdio.h>
#include <stdlib.h>

// Defines the path to the debug stderr dump folder
#define MC_STDERR_FILE_PATH "./Debug/stderr.log"

// Defines the error code for no error
#define MC_NO_ERROR 0

// Defines the error code for errors during stream usage
#define MC_STREAM_ERROR 0x1

// Defines the error code for errors during memory block dumping due to incompatible dump buffer length
#define MC_BLOCK_DUMP_ERROR 0x2

// Defines the error code for errors during array access type casting where accessing thorugh the new type would cause a buffer overflow
#define MC_BUFFER_OVERFLOW_CAST 0x3

// Defines the error code for buffer overflows
#define MC_BUFFER_OVERFLOW 0x4;

// Defines the simulator error dump macro. Dumps error information to stderr and quits programm with error code
#define MC_DUMP_ERROR_AND_EXIT(code, message, buffer) on_error_exit(code, __FILE__, __LINE__, message, buffer);

// Defines the simulator error and memory dump macro. Dumps error information to stderr and quits programm with error code
#define MC_DUMP_ERROR_MEMORY_AND_EXIT(code, message, b_start, b_end) on_error_exit_mem_dump(code, __FILE__, __LINE__, message, b_start, b_end);

// Dumps the passed error information to stderr and exists the program with the provided code. 
int on_error_exit(int error_code, const char* error_file, int error_line, const char* error_string);

// Dumps the passed error information to stderr and exists the program with the provided code. Dumps memory bytes between passed byte pointers
int on_error_exit_mem_dump(int error_code, const char* error_file, int error_line, const char* error_string, uint8_t* mem_start, uint8_t* mem_end);