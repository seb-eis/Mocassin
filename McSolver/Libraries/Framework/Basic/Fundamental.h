//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	Fundamental.h        		//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2020 Sebastian Eisele     //
// Short:   Fundamental definitions     //
//////////////////////////////////////////

#pragma once
#include <stdint.h>
#define MC_LET_IS_VAR
/* Auto types */

// Macro that defines the variable auto type
#define var __auto_type

#if defined(__INTEL_COMPILER) || defined(MC_LET_IS_VAR)
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

// Defines the default byte to be of unsigned int8 type
typedef uint8_t byte_t;

// Defines the signed byte to be of unsigned int8 type
typedef int8_t sbyte_t;

// Defines the bool to be one unsigned byte
typedef byte_t bool_t;

// Defines the values for true and false
enum BooleanValuesEnum { false = 0, true = 1 };

// Defines the basic bitmask to be a unsigned 64 bit integer
typedef uint64_t Bitmask_t;