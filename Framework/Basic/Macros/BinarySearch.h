//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	BinarySearch.h        		//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Binary search macros        //
//////////////////////////////////////////

#pragma once

#include "Framework/Basic/Macros/Macros.h"
#include "Framework/Errors/McErrors.h"

// Function declaration template macro for basic comparison functions
#define FUNCDECL_COMPARER(NAME, VTYPE) int32_t NAME(const VTYPE* lhs, const VTYPE* rhs)

// Function implementation template macro for basic comparison functions
#define FUNCIMPL_COMPARER(NAME, VTYPE, GETTERBUILDER, ...) int32_t NAME(const VTYPE* lhs, const VTYPE* rhs)\
{\
  return compareLhsToRhs(GETTERBUILDER(lhs, __VA_ARGS__), GETTERBUILDER(rhs, __VA_ARGS__));\
}

// Function declaration template macro for CPP-Style lower bound search on span types
#define FUNCDECL_CPPLOWERBOUND(NAME, SPANTYPE, VTYPE) int32_t NAME(SPANTYPE* span, const VTYPE* value)

// Function implementation template macro for CPP-Style lower bound search on span types
#define FUNCIMPL_CPPLOWERBOUND(NAME, SPANTYPE, VTYPE, COMP) int32_t NAME(SPANTYPE* span, const VTYPE* value)\
{\
    int32_t firstIndex = 0, counter = span_GetSize(*span);\
    while (counter > 0)\
    {\
        int32_t step = counter / 2;\
        int32_t currentIndex = firstIndex + step;\
        if (COMP(&span_Get(*span, currentIndex), value) == -1)\
        {\
            firstIndex = ++currentIndex;\
            counter -= step + 1;\
        }\
        else counter = step;\
    }\
    return firstIndex;\
}

// Function declaration template macro for CPP style binary search on span types
#define FUNCDECL_BINARYSEARCH(NAME, SPANTYPE, VTYPE) int32_t NAME(SPANTYPE* span, const VTYPE* value)

// Function implementation template macro for CPP style binary search on span types
#define FUNCIMPL_BINARYSEARCH(NAME, SPANTYPE, VTYPE, COMP) int32_t NAME(SPANTYPE* span, const VTYPE* value)\
{\
    int32_t firstIndex = 0, counter = span_GetSize(*span);\
    while (counter > 0)\
    {\
        int32_t step = counter / 2;\
        int32_t currentIndex = firstIndex + step;\
        if (COMP(&span_Get(*span, currentIndex), value) == -1)\
        {\
            firstIndex = ++currentIndex;\
            counter -= step + 1;\
        }\
        else counter = step;\
    }\
    firstIndex = (span_GetSize(*span) != firstIndex) ? firstIndex : -1;\
    debug_assert(firstIndex != -1);\
    return firstIndex;\
}
