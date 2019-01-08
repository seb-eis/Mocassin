//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	Buffers.c        		    //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Container definitions       //
//////////////////////////////////////////

#include "Framework/Basic/BaseTypes/Buffers.h"

VoidSpan_t AllocateSpan(const size_t numOfElements, const size_t sizeOfElement)
{
    size_t numOfBytes = numOfElements*sizeOfElement;
    void* ptr = malloc(numOfBytes);
    return (VoidSpan_t) { .Begin = ptr, .End = ptr +  numOfBytes };
}

error_t TryAllocateSpan(const size_t numOfElements, const size_t sizeOfElement, VoidSpan_t*restrict outSpan)
{
    *outSpan = AllocateSpan(numOfElements, sizeOfElement);
    return  (outSpan->Begin == NULL) ? ERR_MEMALLOCATION : ERR_OK;
}

void* ConstructVoidSpan(const size_t numOfElements, const size_t sizeOfElement, VoidSpan_t *restrict outSpan)
{
    error_t error = TryAllocateSpan(numOfElements, sizeOfElement, outSpan);
    error_assert(error, "Out of memory on span construction.");
    return outSpan;
}

VoidList_t AllocateList(const size_t capacity, const size_t sizeOfElement)
{
    VoidSpan_t span = AllocateSpan(capacity, sizeOfElement);
    return  (VoidList_t) { .Begin = span.Begin, .End = span.Begin, .CapacityEnd = span.End };
}

error_t TryAllocateList(const size_t capacity, const size_t sizeOfElement, VoidList_t*restrict outList)
{
    *outList = AllocateList(capacity, sizeOfElement);
    return (outList->Begin == NULL) ? ERR_MEMALLOCATION : ERR_OK;
}

void* ConstructVoidList(const size_t capacity, const size_t sizeOfElement, VoidList_t *restrict outList)
{
    error_t error = TryAllocateList(capacity, sizeOfElement, outList);
    error_assert(error, "Out of memory on list construction.");
    return outList;
}

VoidArray_t AllocateArray(const int32_t rank, const size_t sizeOfElement, const int32_t dimensions[rank])
{
    int32_t numOfHeaderBytes = (2 + rank -1) * sizeof(int32_t);
    int32_t numOfDataBytes = dimensions[0] * sizeOfElement;

    for (int i = 1; i < rank; ++i)
    {
        numOfDataBytes *= dimensions[i];
    }

    int32_t totalNumOfBytes = numOfHeaderBytes + numOfDataBytes;
    void* ptr = malloc(totalNumOfBytes);
    return (VoidArray_t) { .Header = ptr, .Begin = ptr + numOfHeaderBytes, .End = ptr + totalNumOfBytes };
}

error_t TryAllocateArray(const int32_t rank, const size_t sizeOfElement, const int32_t dimensions[rank], VoidArray_t*restrict outArray)
{
    *outArray = AllocateArray(rank, sizeOfElement, dimensions);
    return (outArray->Header == NULL) ? ERR_MEMALLOCATION : ERR_OK;
}

void* ConstructVoidArray(const int32_t rank, const size_t sizeOfElement, const int32_t dimensions[rank], VoidArray_t*restrict outArray)
{
    error_t error = TryAllocateArray(rank, sizeOfElement, dimensions, outArray);
    error_assert(error, "Out of memory on array construct");

    MakeArrayBlocks(rank, dimensions, &outArray->Header[2]);
    outArray->Header[1] = span_GetSize(*outArray) / sizeOfElement;
    outArray->Header[0] = rank;

    return outArray;
}


void GetArrayDimensions(const VoidArray_t*restrict array, int32_t*restrict outBuffer)
{
    const int32_t rank = array->Header[0];
    const int32_t size = array->Header[1];
    const int32_t * blocks = &array->Header[2];

    outBuffer[0] = size / blocks[0];
    outBuffer[rank-1] = blocks[rank-2];

    for (int32_t i = 1; i < rank - 1; i++)
    {
        outBuffer[i] = blocks[i-1] / blocks[i];
    }
}

void MakeArrayBlocks(const int32_t rank, const int32_t dimensions[rank], int32_t*restrict outBuffer)
{
    for (int32_t i = rank - 2; i >= 0; i--)
    {
        int32_t multiplier = (i == (rank - 2)) ? 1 : outBuffer[i+1];
        outBuffer[i] = dimensions[i+1] * multiplier;
    }
}

bool_t HaveSameBufferContent(const Buffer_t* lhs, const Buffer_t* rhs)
{
    if (lhs->Begin == rhs->Begin && lhs->End == rhs->End)
    {
        return true;
    }
    if (span_GetSize(*lhs) != span_GetSize(*rhs))
    {
        return false;
    }
    byte_t* lhsit = lhs->Begin;
    byte_t* rhsit = rhs->Begin;
    while (lhsit != lhs->End)
    {
        if (*(++lhsit) != *(++rhsit))
        {
            return false;
        }
    }
    return true;
}

void CopyBuffer(byte_t const* source, byte_t* target, const size_t size)
{
    for (size_t i = 0; i < size; i++)
    {
        target[i] = source[i];
    }
}

error_t SaveCopyBuffer(Buffer_t* restrict sourceBuffer, Buffer_t* restrict targetBuffer)
{
    size_t sourceSize = span_GetSize(*sourceBuffer);

    if (sourceSize > span_GetSize(*targetBuffer))
    {
        return ERR_BUFFEROVERFLOW;
    }

    CopyBuffer(sourceBuffer->Begin, targetBuffer->Begin, sourceSize);
    return ERR_OK;
}

error_t SaveMoveBuffer(Buffer_t* restrict sourceBuffer, Buffer_t* restrict targetBuffer)
{
    if (SaveCopyBuffer(sourceBuffer, targetBuffer) != ERR_OK)
    {
        return ERR_BUFFEROVERFLOW;
    }

    delete_Span(*sourceBuffer);
    return ERR_OK;
}