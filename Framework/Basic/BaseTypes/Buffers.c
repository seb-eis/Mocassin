//
// Created by hims-user on 07.09.2018.
//

#include "Framework/Basic/BaseTypes/Buffers.h"

VoidSpan_t AllocateSpan(const size_t numOfElements, const size_t sizeOfElement)
{
    size_t numOfBytes = numOfElements*sizeOfElement;
    void* ptr = malloc(numOfBytes);
    return (VoidSpan_t) { ptr, ptr +  numOfBytes };
}

error_t TryAllocateSpan(const size_t numOfElements, const size_t sizeOfElement, VoidSpan_t*restrict outSpan)
{
    *outSpan = AllocateSpan(numOfElements, sizeOfElement);
    return  (outSpan->Begin == NULL) ? ERR_MEMALLOCATION : ERR_OK;
}

void* VoidSpanConstructor(const size_t numOfElements, const size_t sizeOfElement, VoidSpan_t*restrict outSpan)
{
    error_t error = TryAllocateSpan(numOfElements, sizeOfElement, outSpan);
    ASSERT_ERROR(error, "Out of memory on span construction");
    return outSpan;
}