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

#include "Libraries/Framework/Basic/BaseTypes/Fundamental.h"
#include <stdlib.h>

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