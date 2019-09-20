//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	MAcros.h        		    //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   general macros              //
//////////////////////////////////////////

#pragma once
#include <stdlib.h>

/* Auto types */

// Macro that defines the variable auto type
#define var __auto_type

#ifdef __INTEL_COMPILER
// Macro that defines the const auto type to var on the intel compiler
#define let var
#else
// Macro that defines the const auto type
#define let var const
#endif

/*Arg count macro from Roland Illig and Laurent Deniau*/

#define __VA_NARG(...) \
        __VA_NARG_(_0, ## __VA_ARGS__, __RSEQ_N())

#define __VA_NARG_(...) \
        __VA_ARG_N(__VA_ARGS__)

#define __VA_ARG_N( \
         _1, _2, _3, _4, _5, _6, _7, _8, _9,_10, \
        _11,_12,_13,_14,_15,_16,_17,_18,_19,_20, \
        _21,_22,_23,_24,_25,_26,_27,_28,_29,_30, \
        _31,_32,_33,_34,_35,_36,_37,_38,_39,_40, \
        _41,_42,_43,_44,_45,_46,_47,_48,_49,_50, \
        _51,_52,_53,_54,_55,_56,_57,_58,_59,_60, \
        _61,_62,_63,N,...) N

#define __RSEQ_N() \
        62, 61, 60,                             \
        59, 58, 57, 56, 55, 54, 53, 52, 51, 50, \
        49, 48, 47, 46, 45, 44, 43, 42, 41, 40, \
        39, 38, 37, 36, 35, 34, 33, 32, 31, 30, \
        29, 28, 27, 26, 25, 24, 23, 22, 21, 20, \
        19, 18, 17, 16, 15, 14, 13, 12, 11, 10, \
         9,  8,  7,  6,  5,  4,  3,  2,  1,  0

/* Macro evaluation */

// Paste a macro string and a set of arguments
#define pasteMacro(MACRO, ...) MACRO(__VA_ARGS__)

// Eval a macro string and set of arguments
#define evalMacro(MACRO, ...) pasteMacro(MACRO, __VA_ARGS__)

// Concat two macro strings
#define concatMacro(A, B) A ## B

/* Ptr usage */

// Use a pointer as a pointer to the given type
#define accessPtrAs(__TYPE, __VALUE) ((__TYPE*) ((void*) __VALUE))

// Access the passed value as the given type
#define accessValAs(__TYPE, __VALUE) ((__TYPE*) ((void*) &__VALUE))

// Compares the left value to the right value
#define compareLhsToRhs(LHS,RHS) ((LHS)==(RHS)) ? 0 : ((LHS)<(RHS)) ? -1 : 1

/* Math macros */

#define getMaxOfTwo(A, B) (((A)>(B))?(A):(B))

#define getMinOfTwo(A, B) (((A)>(B))?(B):(A))

/* Flag macros */

#define flagsAreTrue(__VALUE, __FLAG) ((__VALUE) & (__FLAG)) == (__FLAG)

#define flagsAreFalse(__VALUE, __FLAG) ((__VALUE) & (__FLAG)) != (__FLAG)

#define setFlags(__VALUE, __FLAG) (__VALUE) |= (__FLAG)

#define unsetFlags(__VALUE, __FLAG) (__VALUE) -= ((__VALUE) & (__FLAG))

/* Local function declaration and implementation macros*/

// Builds the default local name for a function
#define name_local_func(NAME) local_##NAME

// Declare a local function using the passed function declaration macro template
#define decl_local_func(FDECLTEMPLATE, NAME, ...) evalMacro(FDECLTEMPLATE, name_local_func(NAME), __VA_ARGS__);

// Implement a local function using the passed function implementation macro template
#define impl_local_func(FIMPLTEMPLATE, FULLNAME, ...) evalMacro(FIMPLTEMPLATE, FULLNAME, __VA_ARGS__)

// Defines the default value getter macro for function templates that expands to the value itself
#define valGetter(VAL, ...) (VAL)

// Defines the default pointer getter macro for function templates that expands to the value of the pointer
#define ptrGetter(PTR, ...) (*PTR)

// Macro that expands to a field getter on a passed pointer for comparer template value getters
#define makeCompGetter(PTR, FIELD) (PTR)->FIELD

// Macro to calculate the percent value of a total or 0 if the value is not finite
#define getPercent(VALUE, TOTAL) isfinite(100.0 * ((double) VALUE / (double) TOTAL)) ? 100.0 * ((double) VALUE / (double) TOTAL) : 0

// Nulls all fields of a variable
#define nullStructContent(VARIABLE) memset(&(VARIABLE), 0, sizeof(typeof(VARIABLE)))

// "Stringyfies" the passed macro contents
#define NAME_OF(MACRO) evalMacro(#MACRO)