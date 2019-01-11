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
#include "Framework/Basic/BaseTypes/BaseTypes.h"

/* Span definition */

// Generic type macro for spans of memory enclosed by pointers to begin and end in a cpp style
// Layout@ggc_x86_64 => 16@[8,8]
#define Span_t(TYPE, NAMING...) struct NAMING { TYPE* Begin, * End; }

// Type for enclosing an undefined buffer. Contains ptr to begin and end in a cpp style
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(void, VoidSpan) VoidSpan_t;

VoidSpan_t AllocateSpan(size_t numOfElements, size_t sizeOfElement);

error_t TryAllocateSpan(size_t numOfElements, size_t sizeOfElement, VoidSpan_t*restrict outSpan);

void* ConstructVoidSpan(size_t numOfElements, size_t sizeOfElement, VoidSpan_t *restrict outSpan);

#define new_Span(SPAN, SIZE) *(typeof(SPAN)*) ConstructVoidSpan((SIZE), sizeof(typeof(*(SPAN).Begin)), (VoidSpan_t*) &(SPAN))

#define delete_Span(SPAN) free((SPAN).Begin)

#define vtypeof_span(SPAN) typeof(span_Get(SPAN, 0))

#define span_GetSize(SPAN) ((SPAN).End-(SPAN).Begin)

#define span_Split(SPAN, NEWBEGIN, NEWEND) { (void*) ((SPAN).Begin + (NEWBEGIN)), (void*) ((SPAN).Begin + (NEWEND)) }

// Access span by index. Works for getting and setting the value
#define span_Get(SPAN, INDEX) (SPAN).Begin[(INDEX)]

#define span_AsVoid(SPAN) { (void*) (SPAN).Begin, (void*) (SPAN).End }

/* Buffer definition */

// Type for enclosing a byte buffer. Contains ptr to begin and end in a cpp style
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(byte_t, Buffer) Buffer_t;

// Constructs the define buffer in place and returns an error when the request fails
#define ctor_Buffer(BUFFER, SIZE) TryAllocateSpan((SIZE), 1, (VoidSpan_t*) &(BUFFER))

// Sets all bytes specified by a start and a conter to 0
static inline void setBufferByteValues(void* restrict start, const size_t byteCount, const byte_t value)
{
    for(size_t i = 0; i < byteCount; i++)
    {
        ((byte_t*)start)[i] = value;
    }
}

// Copies the passed number of bytes from ten source buffer to the target buffer. Does not check for potential buffer overflow
void CopyBuffer(byte_t const* source, byte_t* target, size_t size);

// Copies the contents of the source buffer into the target buffer. Returns buffer overflow error if target is smaller than source
error_t SaveCopyBuffer(Buffer_t* restrict sourceBuffer, Buffer_t* restrict targetBuffer);

// Moves the contents of the source buffer into the target buffer and frees the source buffer
error_t SaveMoveBuffer(Buffer_t* restrict sourceBuffer, Buffer_t* restrict targetBuffer);

// Compares if two buffers contain identical binary values (1) or not (0)
bool_t HaveSameBufferContent(const Buffer_t* lhs, const Buffer_t* rhs);


/* Foreach macro definitions */

#define cpp_foreach(ITER, SPAN) for(vtypeof_span(SPAN)* (ITER) = &(SPAN).Begin[0]; (ITER) < (SPAN).End; ++(ITER))

#define cpp_rforeach(ITER, SPAN) for(vtypeof_span(SPAN)* (ITER) = &(SPAN).End[-1]; (ITER) >= (SPAN).Begin; --(ITER))

#define c_foreach(ITER, ARRAY) for(typeof((ARRAY)[0])* (ITER) = &(ARRAY)[0]; (ITER) < &(ARRAY)[sizeof((ARRAY))/sizeof(typeof((ARRAY)[0]))]; ++(ITER))

#define c_rforeach(ITER, ARRAY) for(typeof((ARRAY)[0])* (ITER) = &(ARRAY)[sizeof((ARRAY))/sizeof(typeof((ARRAY)[0]))]; (ITER) >= &(ARRAY)[0]; --(ITER))


/* List definitions */

// Generic type macro for 1d lists that are enclosed by pointers and support cpp style push_back/pop_back operations
// Layout@ggc_x86_64 => 24@[8,8,8]
#define List_t(TYPE, NAMING...) struct NAMING { TYPE* Begin, * End, * CapacityEnd; }

// Defines the undefined list with void ptr to begin, end and end of capacity
// Layout@ggc_x86_64 => 24@[8,8,8]
typedef List_t(void, VoidList) VoidList_t;

VoidList_t AllocateList(size_t capacity, size_t sizeOfElement);

error_t TryAllocateList(size_t capacity, size_t sizeOfElement, VoidList_t*restrict outList);

void* ConstructVoidList(size_t capacity, size_t sizeOfElement, VoidList_t *restrict outList);

#define new_List(LIST, CAPACITY) *(typeof(LIST)*) ConstructVoidList((CAPACITY), sizeof(typeof(*(LIST).Begin)), (VoidList_t*) &(LIST))

#define delete_List(LIST) free((LIST).Begin)

#define list_GetCapacity(LIST) ((LIST).CapacityEnd-(LIST).Begin)

#define list_PushBack(LIST, VALUE) *((LIST).End++) = (VALUE)

#define list_PopBack(LIST) *(--(LIST).End)

#define list_CanPushBack(LIST) (((LIST).End) != (LIST).CapacityEnd)

#define span_AsList(SPAN) { (void*) (SPAN).Begin, (void*) (SPAN).Begin, (void*) (SPAN).End }

/* Rectangular array definitions */

// Generic type macro for rectangular array access to a span of data supporting multiple index access
// Layout@ggc_x86_64 => 24@[8,8,8]
#define Array_t(TYPE, RANK, NAMING...) struct NAMING { struct { int32_t Rank, Size; int32_t Blocks[(RANK)-1]; }* Header; TYPE* Begin, * End; }

// Type for the undefined void array access
// Layout@ggc_x86_64 => 48@[8,8,4,4,4,4,4,4,4,4,4,{4}]
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

#define array_SkipBlock_5(ARRAY, RANK, VAL, ...) ((ARRAY).Header->Blocks[RANK-5] * (VAL) + array_SkipBlock_4((ARRAY), RANK, __VA_ARGS__))

// Access a multidimensional rectangular array by a set of index values
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