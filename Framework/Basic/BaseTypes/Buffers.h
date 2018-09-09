//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	Buffers.h        		    //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Container definitions       //
//////////////////////////////////////////

#pragma once

#include <stdint.h>
#include "Framework/Errors/McErrors.h"
#include "Framework/Basic/Macros/Macros.h"

/* Span definition */

#define Span_t(TYPE) struct { TYPE* Begin, * End; }

#define NEW_STRUCTTYPE(MACROTYPE, TYPENAME) typedef MACROTYPE TYPENAME

typedef Span_t(void) VoidSpan_t;

VoidSpan_t AllocateSpan(size_t numOfElements, size_t sizeOfElement);

error_t TryAllocateSpan(size_t numOfElements, size_t sizeOfElement, VoidSpan_t*restrict outSpan);

void* ConstructVoidSpan(size_t numOfElements, size_t sizeOfElement, VoidSpan_t *restrict outSpan);

#define new_Span(SPAN, SIZE) *(typeof(SPAN)*) ConstructVoidSpan((SIZE), sizeof(typeof(*(SPAN).Begin)), (VoidSpan_t*) &(SPAN))

#define delete_Span(SPAN) free((SPAN).Begin)

#define vtypeof_span(SPAN) typeof(span_Get(SPAN, 0))

#define span_GetSize(SPAN) (int32_t) ((SPAN).End-(SPAN).Begin)

#define span_Split(SPAN, NEWBEGIN, NEWEND) { (void*) ((SPAN).Begin + (NEWBEGIN)), (void*) ((SPAN).Begin + (NEWEND)) }

#define span_Get(SPAN, INDEX) (SPAN).Begin[(INDEX)]


/* Foreach macro definitions */

#define cpp_foreach(ITER, SPAN) for(vtypeof_span(SPAN)* (ITER) = &(SPAN).Begin[0]; (ITER) < (SPAN).End; ++(ITER))

#define cpp_rforeach(ITER, SPAN) for(vtypeof_span(SPAN)* (ITER) = &(SPAN).End[-1]; (ITER) >= (SPAN).Begin; --(ITER))

#define c_foreach(ITER, ARRAY) for(typeof((ARRAY)[0])* (ITER) = &(ARRAY)[0]; (ITER) < &(ARRAY)[sizeof((ARRAY))/sizeof(typeof((ARRAY)[0]))]; ++(ITER))

#define c_rforeach(ITER, ARRAY) for(typeof((ARRAY)[0])* (ITER) = &(ARRAY)[sizeof((ARRAY))/sizeof(typeof((ARRAY)[0]))]; (ITER) >= &(ARRAY)[0]; --(ITER))


/* List definitions */

#define List_t(TYPE) struct { TYPE* Begin, * End, * CapacityEnd; }

typedef List_t(void) VoidList_t;

VoidList_t AllocateList(size_t capacity, size_t sizeOfElement);

error_t TryAllocateList(size_t capacity, size_t sizeOfElement, VoidList_t*restrict outList);

void* ConstructVoidList(size_t capacity, size_t sizeOfElement, VoidList_t *restrict outList);

#define new_List(LIST, CAPACITY) *(typeof(LIST)*) ConstructVoidList((CAPACITY), sizeof(typeof(*(LIST).Begin)), (VoidList_t*) &(LIST))

#define delete_List(LIST) free((LIST).Begin)

#define list_GetCapacity(LIST) ((LIST).CapacityEnd-(LIST).Begin)

#define list_PushBack(LIST, VALUE) *((LIST).End++) = (VALUE)

#define list_PopBack(LIST) *(--(LIST).End)

/* Rectangular array definitions */

#define Array_t(TYPE, RANK) struct { struct { int32_t Rank, Size; int32_t Blocks[(RANK)-1]; }* Header; (TYPE)* Begin, * End; }

typedef struct { int32_t * Header; void * Begin, * End; } VoidArray_t;

VoidArray_t AllocateArray(int32_t rank, size_t sizeOfElement, const int32_t dimensions[rank]);

error_t TryAllocateArray(int32_t rank, size_t sizeOfElement, const int32_t dimensions[rank], VoidArray_t*restrict outArray);

void* ConstructVoidArray(int32_t rank, size_t sizeOfElement, const int32_t dimensions[rank], VoidArray_t*restrict outArray);

void GetArrayDimensions(const VoidArray_t*restrict array, int32_t*restrict outBuffer);

void MakeArrayBlocks(int32_t rank, const int32_t dimensions[rank], int32_t*restrict outBuffer);

#define new_Array(ARRAY, ...) *(typeof(ARRAY)*) ConstructVoidArray(sizeof((int32_t[]){__VA_ARGS__})/sizeof(int32_t), sizeof(typeof(*(ARRAY).Begin)), (int32_t[]){ __VA_ARGS__ }, (VoidArray_t*) &(ARRAY))

#define delete_Array(ARRAY) free((ARRAY).Header)

#define array_SkipBlock_1(ARRAY, VAL) (VAL)

#define array_SkipBlock_2(ARRAY, RANK, VAL, ...) ((ARRAY).Header->Blocks[RANK-2] * (VAL) + array_SkipBlock_1((ARRAY), __VA_ARGS__))

#define array_SkipBlock_3(ARRAY, RANK, VAL, ...) ((ARRAY).Header->Blocks[RANK-3] * (VAL) + array_SkipBlock_2((ARRAY), RANK, __VA_ARGS__))

#define array_SkipBlock_4(ARRAY, RANK, VAL, ...) ((ARRAY).Header->Blocks[RANK-4] * (VAL) + array_SkipBlock_3((ARRAY), RANK, __VA_ARGS__))

#define array_Get(ARRAY, ...)\
    __EVAL(\
      span_Get, (ARRAY),__EVAL(\
        __EVAL(\
          __CONCAT, array_SkipBlock_, __VA_NARG(__VA_ARGS__)\
        ),\
        (ARRAY), __VA_NARG(__VA_ARGS__), __VA_ARGS__\
      )\
    )

/* */