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
#include <stdio.h>
#include <stdlib.h>

// Defines the error code for no error
#define MC_NO_ERROR 0;

// Defines the error code for errors during stream usage
#define MC_STREAM_ERROR 0x1;

// Defines the error code for errors during memory block dumping due to incompatible dump buffer length
#define MC_BLOCK_DUMP_ERROR 0x2;

// Defines the error code for errors during array access type casting where accessing thorugh the new type might cause an access to invalid memory
#define MC_INVALID_ARRAY_CAST 0x3;

// Defines the simulator error dump macro. Dumps error information to stderr and quits programm with error code
#define MC_DUMP_AND_EXIT(code, message) on_error_exit(code, __FILE__, __LINE__, message);

// Dumps the passed information to stderr and exists the program with the provided code
inline int on_error_exit(int error_code, const char* error_file, const char* error_line, const char* error_string)
{
    fprintf(stderr, "MC Runtime Error: 0x%08x\n File: %s\n Line: %s\n Dump: %s", error_code, error_file, error_line, error_string);
    exit(error_code);
}