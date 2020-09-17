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

#include "Libraries/Framework/Errors/McErrors.h"
#include <time.h>
#include <stdint.h>
#include <limits.h>
#include <math.h>

//  Type for little endian approximation of exp() based on the solution of Nicol N. Schraudolph
typedef union FastExpUnion
{
    double Value;
    int64_t nl;
    struct {int32_t j, i; } n;
} FastExpUnion_t;

// IEEE754 based fast approximation of exp(x) with lowest possible root mean error
static inline double FastExp32RmsError(const double exponent)
{
    const int32_t correction = 60801;
    const int32_t biasfactor = 1072693248;
    const double expfactor = 1048576 / M_LN2;
    FastExpUnion_t approx = {0};
    return (approx.n.i = expfactor * exponent + (biasfactor - correction), approx.Value);
}

// IEEE754 based fast approximation of exp(x) with lowest possible mean error
static inline double FastExp32MeanError(const double exponent)
{
    const int32_t correction = 68243;
    const int32_t biasfactor = 1072693248;
    const double expfactor = 1048576 / M_LN2;
    FastExpUnion_t approx = {0};
    return (approx.n.i = expfactor * exponent + (biasfactor - correction), approx.Value);
}

// IEEE754 based fast approximation of exp(x) with lowest possible max error greater exact value
static inline double FastExp32UpperError(const double exponent)
{
    const int32_t correction = 90253;
    const int32_t biasfactor = 1072693248;
    const double expfactor = 1048576 / M_LN2;
    FastExpUnion_t approx = {0};
    return (approx.n.i = expfactor * exponent + (biasfactor - correction), approx.Value);
}

// IEEE754 based fast approximation of exp(x) with lowest possible max error below exact value
static inline double FastExp32LowerError(const double exponent)
{
    const int32_t correction = -1;
    const int32_t biasfactor = 1072693248;
    const double expfactor = 1048576 / M_LN2;
    FastExpUnion_t approx = {0};
    return (approx.n.i = expfactor * exponent + (biasfactor - correction), approx.Value);
}

// IEEE754 based fast approximation of exp(x) with equal max error on both upper and lower side
static inline double FastExp32TightError(const double exponent)
{
    const int32_t correction = 45799;
    const int32_t biasfactor = 1072693248;
    const double expfactor = 1048576 / M_LN2;
    FastExpUnion_t approx = {0};
    return (approx.n.i = expfactor * exponent + (biasfactor - correction), approx.Value);
}

