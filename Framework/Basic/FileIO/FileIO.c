//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	FileIO.c        		    //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Basic file IO functions     //
//////////////////////////////////////////

#include "Framework/Basic/FileIO/FileIO.h"

cerror_t get_file_size(file_t* restrict f_stream)
{
    int64_t file_size;
    if (f_stream == NULL || fseek(f_stream, 0L, SEEK_END) != 0)
    {
        return MC_FILE_ERROR;
    }
    if ((file_size = ftell(f_stream)) < 0)
    {
        return MC_FILE_ERROR;
    }
    rewind(f_stream);
    return file_size;
}

bool_t file_exist(const char* restrict file_name)
{
    return access(file_name, F_OK) != -1;
}

error_t write_buffer_to_stream(file_t* restrict f_stream, const buffer_t* restrict buffer_in)
{
    if (f_stream == NULL)
    {
        return MC_STREAM_ERROR;
    }

    size_t buffer_size = get_buffer_size(buffer_in);
    if (fwrite(buffer_in->start_it, sizeof(byte_t), buffer_size, f_stream) != buffer_size)
    {
        return MC_STREAM_ERROR;
    }
    return MC_NO_ERROR;
}

error_t write_buffer_to_file(const char* restrict file_name, const char* restrict file_mode, const buffer_t* restrict buffer_in)
{
    file_t* f_stream = NULL;
    int32_t write_result; 

    if (strcmp(file_mode, "wb") != 0 && strcmp(file_mode, "ab") != 0)
    {
        return MC_FILE_MODE_ERROR;
    }
    if ((f_stream = fopen(file_name, file_mode)) == NULL)
    {
        return MC_STREAM_ERROR;
    }
    write_result = write_buffer_to_stream(f_stream, buffer_in);
    fclose(f_stream);
    return write_result;
}

error_t load_buffer_from_stream(file_t* restrict f_stream, buffer_t* restrict byte_array)
{
    if (f_stream == NULL)
    {
        return MC_STREAM_ERROR;
    }
    fread(byte_array->start_it, 1, get_buffer_size(byte_array), f_stream);
    return MC_NO_ERROR;
}

error_t load_buffer_from_file(const char* restrict file_name, buffer_t* restrict buffer_out)
{
    file_t* f_stream;
    int64_t buffer_size;
    int32_t load_result;

    if ((f_stream = fopen(file_name, "rb")) == NULL || (buffer_size = get_file_size(f_stream)) < 0)
    {
        return MC_STREAM_ERROR;
    }

    *buffer_out = allocate_buffer(buffer_size, 1);
    load_result = load_buffer_from_stream(f_stream, buffer_out);

    fclose(f_stream);
    return load_result;
}

error_t write_buffer_hex_to_stream(file_t* restrict f_stream, const buffer_t* restrict byte_array, size_t bytes_per_line)
{
    if (f_stream == NULL)
    {
        return MC_STREAM_ERROR;
    }

    size_t line_counter = 0;
    for (byte_t* it = byte_array->start_it; it < byte_array->end_it; it++)
    {
        fprintf(f_stream, "%02x ", *it);
        if  (++line_counter == bytes_per_line)
        {
            fprintf(f_stream, "\n");
            line_counter = 0;
        }
    }
    return MC_NO_ERROR;
}

error_t write_block_hex_to_stream(file_t* restrict f_stream, const memblock_array_t* restrict block_array, size_t blocks_per_line)
{
    if (f_stream == NULL)
    {
        return MC_STREAM_ERROR;
    }

    size_t line_counter = 0;
    for (memblock_t* it = block_array->start_it; it < block_array->end_it; it++)
    {
        fprintf(f_stream, "%08x ", *it);
        if  (++line_counter == blocks_per_line)
        {
            fprintf(f_stream, "\n");
            line_counter = 0;
        }
    }
    return MC_NO_ERROR;
}