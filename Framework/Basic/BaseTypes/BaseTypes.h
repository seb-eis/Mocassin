//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	BaseTypes.h        		    //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Array + type definitions    //
//////////////////////////////////////////

#pragma once
#include <stdint.h>
#include <stdio.h>
#include <stdlib.h>
#include "Framework/Errors/McErrors.h"

// Defines the void prt compare function type for qsort calls
typedef int32_t (*f_compare_t)(const void* lhs, const void* rhs);

// Defines the function for cmd argument setters
typedef void (*f_cmdcallback_t)(void* obj, char const * value);

// Defines a validator function for an arbitrary pointer
typedef error_t (*f_validator_t)(void const * value);

// Defines a command line argument lookup with expected value in the validator and a callback function for the affiliated value
typedef struct { char const * KeyArgument; const f_validator_t ValueValidator; const f_cmdcallback_t ValueCallback; } cmdarg_resolver_t;

// Defines the command line argument lookup table that contains all supported cmd arg lookups
typedef struct { const cmdarg_resolver_t * Begin, * End; } cmdarg_lookup_t;

// Defines the default byte to be of unsigned int8 type
typedef uint8_t byte_t;

// Defines the signed byte to be of unsigned int8 type
typedef int8_t sbyte_t;

// Defines the bool to be one unsigned byte
typedef sbyte_t bool_t;

// Defines the values for true and false
enum boolean_values { false = 0, true = 1 };

// Defines the basic bitmask to be an unsigned 64 bit integer
typedef int64_t bitmask_t;