//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	BaseTypes.c        		    //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Array + type definitions    //
//////////////////////////////////////////

#include "Framework/Basic/BaseTypes/BaseTypes.h"

bool_t buffer_is_identical(const buffer_t* buffer_0, const buffer_t* buffer_1)
{
    if (buffer_0->start_it == buffer_1->start_it && buffer_0->end_it == buffer_1->end_it)
    {
        return true;
    }
    if (get_buffer_size(buffer_0) != get_buffer_size(buffer_1))
    {
        return false;
    }
    byte_t* it_0 = buffer_0->start_it;
    byte_t* it_1 = buffer_1->start_it;
    while (it_0 != buffer_0->end_it)
    {
        if (*(++it_0) != *(++it_1))
        {
            return false;
        }
    }
    return true;
}

void get_md_array_dimensions(const int32_t* restrict i_header, int32_t* restrict o_buffer)
{    
    const int32_t rank = *i_header, * size = i_header + 1, * blocks = i_header + 2;
    o_buffer[0] = *size / blocks[0];
    o_buffer[rank-1] = blocks[rank-2];

    for(int32_t i = 1; i < rank - 1; i++)
    {
        o_buffer[i] = blocks[i-1] / blocks[i];
    }
}

void md_array_dim_to_blocks(const int32_t rank, const int32_t* restrict dimensions, int32_t* restrict o_buffer)
{   
    for(int32_t i = rank - 2; i >= 0; i--)
    {
        int32_t multiplier = (i == (rank - 2)) ? 1 : o_buffer[i+1];
        o_buffer[i] = dimensions[i+1] * multiplier;
    }
}

int32_t get_md_array_size(const size_t rank, const int32_t* restrict dimensions)
{
    int32_t size = 1;  
    for(size_t i = 0; i < rank; i++)
    {
        size *= dimensions[i];
    }
    return size;
}

blob_t allocate_md_array(const int32_t rank, const size_t item_size, const int32_t* restrict dimensions)
{
    int32_t size = get_md_array_size(rank, dimensions);
    blob_t blob = allocate_blob(size, (1 + rank) * sizeof(int32_t));
    int32_t* header_ptr = (int32_t*) blob.header;

    header_ptr[0] = rank;
    header_ptr[1] = size;
    md_array_dim_to_blocks(rank, dimensions, &header_ptr[2]);

    return blob;
}

error_t allocate_md_array_checked(const int32_t rank, const size_t item_size, const int32_t* restrict dimensions, blob_t* restrict o_blob)
{
    *o_blob = allocate_md_array(rank, item_size, dimensions);
    if (o_blob->header == NULL)
    {
        return MC_MEM_ALLOCATION_ERROR;
    }
    return MC_NO_ERROR;
}