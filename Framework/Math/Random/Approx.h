//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	Approx.h     		    //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Function approximations     //
//////////////////////////////////////////

#pragma once

#include "Framework/Errors/McErrors.h"
#include <time.h>
#include <stdint.h>
#include <limits.h>
#include <math.h>

//  Type for little endian approximation of exp() based on the solution of Nicol N. Schraudolph
typedef union SchraudolphExpUnion
{
    double Value;
    struct {int32_t j, i; } n;
} SchraudolphExpUnion_t;

// Exp approximation using the Nicol N. Schraudolph approach with c = 60801 for minimized mean root error
static inline double ApproxExp_Schraudolph_RMS(const double exponent)
{
    const int32_t correction = 60801;
    SchraudolphExpUnion_t approx = {0};
    return (approx.n.i = (1048576/M_LN2) * exponent + (1072693248 - correction), approx.Value);
}

//  Calculates the result of the exponential function where a
static inline double CalculateExpResult(const double exponent)
{
    #if defined(OPT_APPROXIMATE_EXP)
    return ApproxExp_Schraudolph_RMS(exponent);
    #else
    return exp(exponent);
    #endif
}

