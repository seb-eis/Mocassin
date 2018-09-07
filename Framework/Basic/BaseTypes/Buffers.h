//
// Created by hims-user on 07.09.2018.
//

#pragma once

#include <stdint.h>
#include "Framework/Errors/McErrors.h"

/* Foreach macro definitions */

#define cpp_foreach(SPAN, ITER) for(typeof((SPAN).Begin[0])* (ITER) = (SPAN.Begin); (ITER) < (SPAN).End; ++(ITER))

#define cpp_rforeach(SPAN, ITER) for(typeof((SPAN).Begin[0])* (ITER) = (SPAN.End[-1]); (ITER) >= (SPAN).Begin; --(ITER))

#define c_foreach(ARRAY, ITER) for(typeof((ARRAY)[0])* ITER = &(ARRAY)[0]; ITER < &(ARRAY)[sizeof((ARRAY))/sizeof(typeof((ARRAY)[0]))]; ++(ITER))

#define c_rforeach(ARRAY, ITER) for(typeof((ARRAY)[0])* ITER = &(ARRAY)[sizeof((ARRAY))/sizeof(typeof((ARRAY)[0]))]; ITER >= &(ARRAY)[0]; --(ITER))

/* Span definition */

#define Span_t(TYPE) struct { TYPE* Begin, * End; }

typedef Span_t(void) VoidSpan_t;

#define newSpan(SPAN, SIZE) *(typeof(SPAN)*) VoidSpanConstructor((SIZE), sizeof(typeof(*(SPAN).Begin)), (VoidSpan_t*) &(SPAN))

#define deleteSpan(SPAN) free((SPAN).Begin)

#define sizeofSpan(SPAN) (int32_t) ((SPAN).End-(SPAN).Begin)

#define getSubSpan(SPAN, NEWBEGIN, NEWEND) { (void*) ((SPAN).Begin + (NEWBEGIN)), (void*) ((SPAN).Begin + (NEWEND)) }

#define spanAt(SPAN, INDEX) (SPAN).Begin[(INDEX)]

VoidSpan_t AllocateSpan(const size_t numOfElements, const size_t sizeOfElement);

error_t TryAllocateSpan(const size_t numOfElements, const size_t sizeOfElement, VoidSpan_t*restrict outSpan);

void* VoidSpanConstructor(const size_t numOfElements, const size_t sizeOfElement, VoidSpan_t*restrict outSpan);

/* List definitions */

#define List_t(TYPE) struct { TYPE* Begin, * End, * CapacityEnd; }

typedef List_t(void) VoidList_t;

#define newList(LIST, CAPACITY) *(typeof(LIST)*) VoidListConstructor((CAPACITY), sizeof(typeof(*(LIST).Begin)), (VoidList_t*) &(LIST))

#define deleteList(LIST) free((LIST).Begin)

#define capacityofList(LIST) ((LIST).CapacityEnd-(LIST).Begin)

#define pushBackList(LIST, VALUE) *((LIST).End++) = (VALUE)

#define popBackList(LIST) *(--(LIST).End)

VoidList_t AllocateList(const size_t capacity, const size_t sizeOfElement);

error_t TryAllocateList(const size_t capacity, const size_t sizeOfElement, VoidList_t*restrict outList);

void* VoidListConstructor(const size_t capacity, const size_t sizeOfElement, VoidList_t*restrict outList);