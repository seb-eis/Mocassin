//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	Buffers.c        		    //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Container definitions       //
//////////////////////////////////////////

#include <string.h>
#include <stdlib.h>
#include "Buffers.h"

int32_t CompareMocuuid(const void* lhs, const void* rhs)
{
    var comp = compareLhsToRhs(*(int64_t*)lhs, *(int64_t*)rhs);
    if (comp != 0) return comp;
    return compareLhsToRhs(*(int64_t*)(lhs+8), *(int64_t*)(rhs+8));
}

VoidSpan_t AllocateSpan(const size_t numOfElements, const size_t sizeOfElement)
{
    let numOfBytes = numOfElements*sizeOfElement;
    let ptr = malloc(numOfBytes);
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
    assert_success(error, "Out of memory on span construction.");
    memset(outSpan->Begin, 0, numOfElements * sizeOfElement);
    return outSpan;
}

void* ConstructSpanFromBlob(const void *restrict buffer, size_t numOfBytes, VoidSpan_t *restrict outSpan)
{
    if (buffer == NULL)
    {
        *outSpan = (VoidSpan_t) {.Begin = NULL, .End = NULL};
        return outSpan;
    }

    *outSpan = span_New(*outSpan, numOfBytes);
    let bufferSpan = (Buffer_t) {.Begin = (void*) buffer, .End = (void*) buffer + numOfBytes};
    var error = SaveCopyBuffer(&bufferSpan, (Buffer_t*) outSpan);
    assert_success(error, "this has not worked \n");

    return outSpan;
}

VoidList_t AllocateList(const size_t capacity, const size_t sizeOfElement)
{
    let span = AllocateSpan(capacity, sizeOfElement);
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
    assert_success(error, "Out of memory on list construction.");
    memset(outList->Begin, 0, capacity * sizeOfElement);
    return outList;
}

VoidArray_t AllocateArray(const int32_t rank, const size_t sizeOfElement, const int32_t dimensions[rank])
{
    let numOfHeaderBytes = CalculateArrayHeaderSize(rank);
    var numOfDataBytes = dimensions[0] * sizeOfElement;

    for (int i = 1; i < rank; ++i)
        numOfDataBytes *= dimensions[i];

    let totalNumOfBytes = numOfHeaderBytes + numOfDataBytes;
    let ptr = malloc(totalNumOfBytes);
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
    assert_success(error, "Out of memory on array construct");

    MakeArrayBlocks(rank, dimensions, &outArray->Header->FirstBlockEntry);
    outArray->Header->Size = (int32_t) (span_Length(*outArray) / sizeOfElement);
    outArray->Header->Rank = rank;

    memset(outArray->Begin, 0, span_ByteCount(*outArray));

    return outArray;
}

void* ConstructArrayFromBlob(const void *restrict buffer,const size_t sizeOfElements, VoidArray_t *restrict outArray)
{
    if (buffer == NULL)
    {
        *outArray = (VoidArray_t) {.Header = NULL, .Begin = NULL, .End = NULL};
        return outArray;
    }

    let bufferArray = (VoidArray_t) {.Header = (void*) buffer};
    let headerByteCount = array_HeaderByteCount(bufferArray);
    let dataByteCount = array_Length(bufferArray) * sizeOfElements;
    let totalByteCount = headerByteCount + dataByteCount;

    outArray->Header = malloc(totalByteCount);
    assert_success((outArray->Header != NULL) ? ERR_OK : ERR_MEMALLOCATION, "Failed to build array from passed blob");

    outArray->Begin = ((void*)outArray->Header) + headerByteCount;
    outArray->End = outArray->Begin + dataByteCount;
    memcpy(outArray->Header, buffer, totalByteCount);
    return outArray;
}

void GetArrayDimensions(const VoidArray_t*restrict array, int32_t*restrict outBuffer)
{
    let rank = array_Rank(*array);
    let size = array_Length(*array);
    let blocks = &array->Header->FirstBlockEntry;

    outBuffer[0] = size / blocks[0];
    outBuffer[rank-1] = blocks[rank-2];

    for (var i = 1; i < rank - 1; i++)
        outBuffer[i] = blocks[i-1] / blocks[i];
}

void MakeArrayBlocks(const int32_t rank, const int32_t dimensions[rank], int32_t*restrict outBuffer)
{
    for (var i = rank - 2; i >= 0; i--)
    {
        let multiplier = (i == (rank - 2)) ? 1 : outBuffer[i+1];
        outBuffer[i] = dimensions[i+1] * multiplier;
    }
}

bool_t HaveSameBufferContent(const Buffer_t* lhs, const Buffer_t* rhs)
{
    return_if(lhs->Begin == rhs->Begin && lhs->End == rhs->End, true);
    return_if(span_Length(*lhs) != span_Length(*rhs), false);

    byte_t * lhsit = lhs->Begin;
    byte_t * rhsit = rhs->Begin;
    while (lhsit != lhs->End)
        if (*(lhsit++) != *(rhsit++))
            return false;

    return true;
}

void CopyBuffer(byte_t const* source, byte_t* target, const size_t size)
{
    for (var i = 0; i < size; i++)
        target[i] = source[i];
}

error_t SaveCopyBuffer(const Buffer_t* restrict sourceBuffer, Buffer_t* restrict targetBuffer)
{
    let sourceSize = span_Length(*sourceBuffer);
    return_if(sourceSize > span_Length(*targetBuffer), ERR_BUFFEROVERFLOW);
    CopyBuffer(sourceBuffer->Begin, targetBuffer->Begin, sourceSize);
    return ERR_OK;
}

error_t SaveMoveBuffer(Buffer_t* restrict sourceBuffer, Buffer_t* restrict targetBuffer)
{
    if (SaveCopyBuffer(sourceBuffer, targetBuffer) != ERR_OK)
        return ERR_BUFFEROVERFLOW;

    span_Delete(*sourceBuffer);
    return ERR_OK;
}