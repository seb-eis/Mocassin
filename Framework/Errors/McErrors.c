//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	Errors.h         		    //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Error codes + messages      //
//////////////////////////////////////////

#include "McErrors.h"
#include "Framework/Basic/FileIO/FileIO.h"

int on_error_exit(int error_code, const char* error_file, int error_line, const char* error_string)
{
    FILE* file_stream = fopen(MC_STDERR_FILE_PATH, "w");
    fprintf(file_stream, "MC Runtime Error: 0x%08x\n File: %s\n Line: %d\n Info: %s", error_code, error_file, error_line, error_string);
    fclose(file_stream);
    exit(error_code);
}

int on_error_exit_mem_dump(int error_code, const char* error_file, int error_line, const char* error_string, byte_t* mem_start, byte_t* mem_end)
{
    FILE* file_stream = fopen(MC_STDERR_FILE_PATH, "w");
    fprintf(file_stream, "MC Runtime Error: 0x%08x\n File: %s\n Line: %d\n Info: %s\n Buffer:\n\n", error_code, error_file, error_line, error_string);

    buffer_t buffer = {mem_start, mem_end};
    write_buffer_hex_to_stream(file_stream, &buffer, 24);
    fclose(file_stream);
    exit(error_code);
}