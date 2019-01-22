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

// Allocate a new voi span with the passed number of elements and size of elements
VoidSpan_t AllocateSpan(size_t numOfElements, size_t sizeOfElement);

// Tries to allocate a new void span with the passed parameters and returns an error code about the success
error_t TryAllocateSpan(size_t numOfElements, size_t sizeOfElement, VoidSpan_t*restrict outSpan);

// Construct a new void span (With allocation failure handle) with the given parameter set and returns the span access as a pointer.
void* ConstructVoidSpan(size_t numOfElements, size_t sizeOfElement, VoidSpan_t *restrict outSpan);

// Copies a specific number of bytes from the passed buffer into a newly constructed span. Does not free the original buffer!
void* ConstructSpanFromBlob(void *restrict buffer, size_t numOfBytes, VoidSpan_t *restrict outSpan);

// Allocates a new span by constructing and casting a nw void type span
#define new_Span(SPAN, SIZE) *(typeof(SPAN)*) ConstructVoidSpan((SIZE), sizeof(typeof(*(SPAN).Begin)), (VoidSpan_t*) &(SPAN))

// Deletes a span by freeing the dynamic memory the span is addressing (Do not call on a subspan)
#define delete_Span(SPAN) free((SPAN).Begin)

// Get the value type of a span
#define vtypeof_span(SPAN) typeof(span_Get(SPAN, 0))

// Get the size of the passed span
#define span_GetSize(SPAN) ((SPAN).End-(SPAN).Begin)

// Get the first entry of the span
#define span_Front(SPAN) (SPAN).Begin

// Get the last entry of the span
#define span_Back(SPAN) ((SPAN).End - 1)

// Creates a sub-access span with new boundary info for the passed parent span
#define span_Split(SPAN, NEWBEGIN, NEWEND) { (void*) ((SPAN).Begin + (NEWBEGIN)), (void*) ((SPAN).Begin + (NEWEND)) }

// Access span by index. Works for getting and setting the value
#define span_Get(SPAN, INDEX) (SPAN).Begin[(INDEX)]

// Makes a void access type for the passed span
#define span_AsVoid(SPAN) { (void*) (SPAN).Begin, (void*) (SPAN).End }

// Macro function that will return true if the passed index value is out of range of the passed span
#define span_IndexIsOutOfRange(SPAN, INDEX) ((size_t) (INDEX) >= span_GetSize(SPAN))

// Macro to in-place construct a new span from the passed blob and number of elements information
#define span_FromBlob(SPAN,BUFFER,SIZE) *(typeof(SPAN)*) ConstructSpanFromBlob((BUFFER), (SIZE)*sizeof(typeof(*(SPAN).Begin)), (VoidSpan_t*) &(SPAN))

/* Buffer definition */

// Type for enclosing a byte buffer. Contains ptr to begin and end in a cpp style
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(byte_t, Buffer) Buffer_t;

// Constructs the define buffer in place and returns an error when the request fails
#define ctor_Buffer(BUFFER, SIZE) TryAllocateSpan((SIZE), 1, (VoidSpan_t*) &(BUFFER))

// Sets all bytes specified by a start and a counter to 0
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

// Defines a CPP style foreach iteration over any access type that defines .Begin and .End pointers
#define cpp_foreach(ITER, SPAN) for(vtypeof_span(SPAN)* (ITER) = &(SPAN).Begin[0]; (ITER) < (SPAN).End; ++(ITER))

// Defines a CPP style reverse foreach iteration over any access type that defines .Begin and .End pointers
#define cpp_rforeach(ITER, SPAN) for(vtypeof_span(SPAN)* (ITER) = &(SPAN).End[-1]; (ITER) >= (SPAN).Begin; --(ITER))

// Defines a CPP style foreach iteration over any fixed size C array
#define c_foreach(ITER, ARRAY) for(typeof((ARRAY)[0])* (ITER) = &(ARRAY)[0]; (ITER) < &(ARRAY)[sizeof((ARRAY))/sizeof(typeof((ARRAY)[0]))]; ++(ITER))

// Defines a CPP style reverse foreach iteration over any fixed size C array
#define c_rforeach(ITER, ARRAY) for(typeof((ARRAY)[0])* (ITER) = &(ARRAY)[sizeof((ARRAY))/sizeof(typeof((ARRAY)[0]))]; (ITER) >= &(ARRAY)[0]; --(ITER))


/* List definitions */

// Generic type macro for 1d lists that are enclosed by pointers and support cpp style push_back/pop_back operations
// Layout@ggc_x86_64 => 24@[8,8,8]
#define List_t(TYPE, NAMING...) struct NAMING { TYPE* Begin, * End, * CapacityEnd; }

// Defines the undefined list with void ptr to begin, end and end of capacity
// Layout@ggc_x86_64 => 24@[8,8,8]
typedef List_t(void, VoidList) VoidList_t;

// Allocates a new list with the given capacity and size of elements
VoidList_t AllocateList(size_t capacity, size_t sizeOfElement);

// Tries to allocate a new list with the given capacity and size of elements or returns an error code
error_t TryAllocateList(size_t capacity, size_t sizeOfElement, VoidList_t*restrict outList);

// Construct a new list with the given capacity and size of elements and returns the list pointer (Handles allocation errors)
void* ConstructVoidList(size_t capacity, size_t sizeOfElement, VoidList_t *restrict outList);

// Macro to allocate a new list with the specified capacity and type
#define new_List(LIST, CAPACITY) *(typeof(LIST)*) ConstructVoidList((CAPACITY), sizeof(typeof(*(LIST).Begin)), (VoidList_t*) &(LIST))

// Macro to free the dynamic memory the list access refers to
#define delete_List(LIST) free((LIST).Begin)

// Get the capacity of the passed list
#define list_GetCapacity(LIST) ((LIST).CapacityEnd-(LIST).Begin)

// Pushes the passed value to the end of the passed list without checking for overflow
#define list_PushBack(LIST, VALUE) *((LIST).End++) = (VALUE)

// Removes the last entry from the by decreasing the current end without checking for underflow
#define list_PopBack(LIST) *(--(LIST).End)

// Macro to check if the passed list is full
#define list_IsFull(LIST) (((LIST).End) != (LIST).CapacityEnd)

// Macro to check if the list contains any entries
#define list_IsEmpty(LIST) (((LIST).End) != (LIST).Begin)

// Creates a list access from a span access
#define span_AsList(SPAN) { (void*) (SPAN).Begin, (void*) (SPAN).Begin, (void*) (SPAN).End }

/* Rectangular array definitions */

// Generic type macro for rectangular array access to a span of data supporting multiple index access
// Layout@ggc_x86_64 => 24@[8,8,8]
#define Array_t(TYPE, RANK, NAMING...) struct NAMING { struct { int32_t Rank, Size; int32_t Blocks[(RANK)-1]; }* Header; TYPE* Begin, * End; }

// Type for the undefined void array access
// Layout@ggc_x86_64 => 48@[8,8,4,4,4,4,4,4,4,4,4,{4}]
typedef struct { int32_t * Header; void * Begin, * End; } VoidArray_t;

// Allocate a new array of given rank, size of elements and dimensions
VoidArray_t AllocateArray(int32_t rank, size_t sizeOfElement, const int32_t dimensions[rank]);

// Tries to allocate a new array of given rank, size of elements and dimensions or returns an error code
error_t TryAllocateArray(int32_t rank, size_t sizeOfElement, const int32_t dimensions[rank], VoidArray_t*restrict outArray);

// Constructs a new array of given rank, size of elements and dimensions and returns the pointer to the access struct (Handles allocation errors)
void* ConstructVoidArray(int32_t rank, size_t sizeOfElement, const int32_t dimensions[rank], VoidArray_t*restrict outArray);

// Get the dimensions of the passed array and writes them into the passed out buffer
void GetArrayDimensions(const VoidArray_t*restrict array, int32_t*restrict outBuffer);

// Calculates the required array block skips fro the passed dimension set and writes the to the out buffer
void MakeArrayBlocks(int32_t rank, const int32_t dimensions[rank], int32_t*restrict outBuffer);

// Constructs a new array from the passed buffer by interpreting it as a previously build array. Does not free the original buffer!
void* ConstructArrayFromBlob(void *restrict buffer, size_t sizeOfElements, VoidArray_t *restrict outArray);

// Macro to construct a new array of the passed type and dimensions
#define new_Array(ARRAY, ...) *(typeof(ARRAY)*) ConstructVoidArray(sizeof((int32_t[]){__VA_ARGS__})/sizeof(int32_t), sizeof(typeof(*(ARRAY).Begin)), (int32_t[]){ __VA_ARGS__ }, (VoidArray_t*) &(ARRAY))

// Frees the memory of any array
#define delete_Array(ARRAY) free((ARRAY).Header)

// Get the total number of elements in any type of array
#define array_GetSize(ARRAY) (accessValAs(VoidArray_t, ARRAY))->Header[1]

// Get the rank of any type of array
#define array_GetRank(ARRAY) (accessValAs(VoidArray_t, ARRAY))->Header[0]

// Get the header size in bytes of any array
#define array_GetHeaderSize(ARRAY) (1 + array_GetRank(ARRAY)) * sizeof(int32_t)

// Allocates a new array by interpreting the passed buffer pointer as a formatted array and copies the data. Does not free original buffer!
#define array_FromBlob(ARRAY, BUFFER) *(typeof(ARRAY)*) ConstructArrayFromBlob((BUFFER), sizeof(typeof(*(ARRAY).Begin)), (VoidArray_t*) &(ARRAY))

// Get the number of elements that need to be skipped to advance the passed number of steps in the 1. dimension of an array
#define array_SkipBlock_1(ARRAY, VAL) (VAL)

// Get the number of elements that need to be skipped to advance the passed number of steps in the 2. dimension of an array
#define array_SkipBlock_2(ARRAY, RANK, VAL, ...) ((ARRAY).Header->Blocks[RANK-2] * (VAL) + array_SkipBlock_1((ARRAY), __VA_ARGS__))

// Get the number of elements that need to be skipped to advance the passed number of steps in the 3. dimension of an array
#define array_SkipBlock_3(ARRAY, RANK, VAL, ...) ((ARRAY).Header->Blocks[RANK-3] * (VAL) + array_SkipBlock_2((ARRAY), RANK, __VA_ARGS__))

// Get the number of elements that need to be skipped to advance the passed number of steps in the 4. dimension of an array
#define array_SkipBlock_4(ARRAY, RANK, VAL, ...) ((ARRAY).Header->Blocks[RANK-4] * (VAL) + array_SkipBlock_3((ARRAY), RANK, __VA_ARGS__))

// Get the number of elements that need to be skipped to advance the passed number of steps in the 5. dimension of an array
#define array_SkipBlock_5(ARRAY, RANK, VAL, ...) ((ARRAY).Header->Blocks[RANK-5] * (VAL) + array_SkipBlock_4((ARRAY), RANK, __VA_ARGS__))

// Access a multidimensional rectangular array by a set of index values
#define array_Get(ARRAY, ...)\
    evalMacro(\
      span_Get, (ARRAY),evalMacro(\
        evalMacro(\
          concatMacro, array_SkipBlock_, __VA_NARG(__VA_ARGS__)\
        ),\
        (ARRAY), __VA_NARG(__VA_ARGS__), __VA_ARGS__\
      )\
    )

// Get a boolean value indicating if the passed index set is out of the array access range
#define array_IndicesAreOutOfRange(ARRAY, ...) ((array_GetRank(ARRAY) != __VA_NARG(__VA_ARGS__)) || (&array_Get(ARRAY, __VA_ARGS__) > span_Back(ARRAY)))

/* */