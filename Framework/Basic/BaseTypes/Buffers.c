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
    ASSERT_ERROR(error, "Out of memory on span construction.");
    return outSpan;
}

VoidList_t AllocateList(const size_t capacity, const size_t sizeOfElement)
{
    VoidSpan_t span = AllocateSpan(capacity, sizeOfElement);
    return  (VoidList_t) { span.Begin, span.Begin, span.End };
}

error_t TryAllocateList(const size_t capacity, const size_t sizeOfElement, VoidList_t*restrict outList)
{
    *outList = AllocateList(capacity, sizeOfElement);
    return (outList->Begin == NULL) ? ERR_MEMALLOCATION : ERR_OK;
}

void* VoidListConstructor(const size_t capacity, const size_t sizeOfElement, VoidList_t*restrict outList)
{
    error_t error = TryAllocateList(capacity, sizeOfElement, outList);
    ASSERT_ERROR(error, "Out of memory on list construction.");
    return outList;
}