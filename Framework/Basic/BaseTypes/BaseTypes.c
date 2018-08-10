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

bool_t HaveSameBufferContent(const buffer_t* lhs, const buffer_t* rhs)
{
    if (lhs->Start == rhs->Start && lhs->End == rhs->End)
    {
        return true;
    }
    if (GetBufferSize(lhs) != GetBufferSize(rhs))
    {
        return false;
    }
    byte_t* lhsit = lhs->Start;
    byte_t* rhsit = rhs->Start;
    while (lhsit != lhs->End)
    {
        if (*(++lhsit) != *(++rhsit))
        {
            return false;
        }
    }
    return true;
}

void GetMdaDimensions(const int32_t* restrict inHeader, int32_t* restrict outBuffer)
{    
    const int32_t rank = *inHeader, * size = inHeader + 1, * blocks = inHeader + 2;
    outBuffer[0] = *size / blocks[0];
    outBuffer[rank-1] = blocks[rank-2];

    for(int32_t i = 1; i < rank - 1; i++)
    {
        outBuffer[i] = blocks[i-1] / blocks[i];
    }
}

void GetMdaBlockSizes(const int32_t rank, const int32_t* restrict dimensions, int32_t* restrict outBuffer)
{   
    for(int32_t i = rank - 2; i >= 0; i--)
    {
        int32_t multiplier = (i == (rank - 2)) ? 1 : outBuffer[i+1];
        outBuffer[i] = dimensions[i+1] * multiplier;
    }
}

int32_t GetMdaSize(const size_t rank, const int32_t* restrict dimensions)
{
    int32_t size = 1;  
    for(size_t i = 0; i < rank; i++)
    {
        size *= dimensions[i];
    }
    return size;
}

blob_t AllocateMdaUnchecked(const int32_t rank, const size_t itemSize, const int32_t* restrict dimensions)
{
    int32_t size = GetMdaSize(rank, dimensions);
    blob_t blob = AllocateBlobUnchecked(size * itemSize, (1 + rank) * sizeof(int32_t));
    int32_t* header = (int32_t*) blob.Header;

    header[0] = rank;
    header[1] = size;
    GetMdaBlockSizes(rank, dimensions, &header[2]);

    return blob;
}

error_t AllocateMdaChecked(const int32_t rank, const size_t itemSize, const int32_t* restrict dimensions, blob_t* restrict outBlob)
{
    *outBlob = AllocateMdaUnchecked(rank, itemSize, dimensions);
    if (outBlob->Header == NULL)
    {
        return MC_MEM_ALLOCATION_ERROR;
    }
    return MC_NO_ERROR;
}