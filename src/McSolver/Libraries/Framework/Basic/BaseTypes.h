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
#include "Libraries/Framework/Errors/McErrors.h"
#include "Fundamental.h"

// Defines the void prt compare function type for qsort calls
typedef int32_t (*FComparer_t)(const void* lhs, const void* rhs);

// Defines the function for cmd argument setters
typedef void (*FCmdCallback_t)(void* obj, char const * value);

// Defines a validator function for an arbitrary pointer
typedef error_t (*FValidator_t)(void const * value);

// Defines a general command function that has a set of string arguments as parameters
typedef void (*FCmdFunction_t)(const int argc, const char*const* argv);

//   Defines the type for named command functions
typedef struct NamedCmdFunction {char const* Name; FCmdFunction_t Callback; } NamedCmdFunction_t;

// Defines a command line argument lookup with expected value in the validator and a callback function for the affiliated value
typedef struct CmdArgResolver { char const * KeyArgument; const FValidator_t ValueValidator; const FCmdCallback_t ValueCallback; } CmdArgResolver_t;

// Defines the command line argument lookup table that contains all supported cmd arg lookups
typedef struct CmdArgLookup { const CmdArgResolver_t * Begin, * End; } CmdArgLookup_t;