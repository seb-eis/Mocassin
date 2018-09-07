//
// Created by hims-user on 07.09.2018.
//

#pragma once

#include <stdint.h>
#include "Framework/Errors/McErrors.h"

#define Span_t(TYPE) struct { TYPE* Begin, * End; }

typedef Span_t(void) VoidSpan_t;

#define newSpan(SPAN, SIZE) *(typeof(SPAN)*) VoidSpanConstructor((SIZE), sizeof(typeof(*(SPAN).Begin)), (VoidSpan_t*) &(SPAN))

#define deleteSpan(SPAN) free((SPAN).Begin)

#define sizeofSpan(SPAN) (int32_t) ((SPAN).End-(SPAN).Begin)

#define splitSpan(SPAN, NEWBEGIN, NEWEND) { (void*) ((SPAN).Begin + (NEWBEGIN)), (void*) ((SPAN).Begin + (NEWEND)) }

#define spanAt(SPAN, INDEX) (SPAN).Begin[(INDEX)]

#define cpp_foreach(SPAN, ITER) for(typeof((SPAN).Begin[0])* (ITER) = (SPAN.Begin); (ITER) < (SPAN).End; ++(ITER))

#define cpp_rforeach(SPAN, ITER) for(typeof((SPAN).Begin[0])* (ITER) = (SPAN.End[-1]); (ITER) >= (SPAN).Begin; --(ITER))

#define c_foreach(ARRAY, ITER) for(typeof((ARRAY)[0])* ITER = &(ARRAY)[0]; ITER < &(ARRAY)[sizeof((ARRAY))/sizeof(typeof((ARRAY)[0]))]; ++ITER)

VoidSpan_t AllocateSpan(const size_t numOfElements, const size_t sizeOfElement);

error_t TryAllocateSpan(const size_t numOfElements, const size_t sizeOfElement, VoidSpan_t*restrict outSpan);

void* VoidSpanConstructor(const size_t numOfElements, const size_t sizeOfElement, VoidSpan_t*restrict outSpan);