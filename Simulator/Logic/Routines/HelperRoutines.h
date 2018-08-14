
//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	EnvRoutines.h        		//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Inlinable helper routines   //
//////////////////////////////////////////

#pragma once
#include "Framework/Errors/McErrors.h"
#include "Framework/Basic/BaseTypes/BaseTypes.h"
#include "Framework/Math/Types/Vector.h"

#define MC_CONST_BLOTZMANN_ELV 8.6173303e-05
#define MC_CONST_JUMPTRACK_MIN 1.0e-05
#define MC_CONST_JUMPTRACK_MAX 1.0e+00
#define MC_CONST_JUMPLIMIT_MIN 0.0e+00
#define MC_CONST_JUMPLIMIT_MAX 1.0e+00

#define FLAG_IS_SET(__VALUE, __FLAG) ((__VALUE) & (__FLAG)) == 0

#define FLAG_NOT_SET(__VALUE, __FLAG) ((__VALUE) & (__FLAG)) != 0

#define SET_FLAG(__VALUE, __FLAG) (__VALUE) |= (__VALUE)

#define UNSET_FLAG(__VALUE, __FLAG) (__VALUE) -= ((__VALUE) & (__FLAG))

static inline void AddToLhsVector4(vector4_t* restrict lhs, const vector4_t* restrict rhs)
{
    lhs->a += rhs->a;
    lhs->b += rhs->b;
    lhs->c += rhs->c;
    lhs->d += rhs->d;
}

static inline void TrimVector4ToCell(vector4_t* restrict vector, const vector4_t* restrict sizes)
{
    while(vector->a >= sizes->a) vector->a -= sizes->a;
    while(vector->b >= sizes->b) vector->b -= sizes->b;
    while(vector->c >= sizes->c) vector->c -= sizes->c;
}

static inline void AddToLhsAndTrimVector4(vector4_t* restrict lhs, const vector4_t* restrict rhs, const vector4_t* sizes)
{
    AddToLhsVector4(lhs, rhs);
    TrimVector4ToCell(lhs, sizes);
}

static inline env_state_t* GetEnvByVector4(const vector4_t* restrict vec, const env_lattice_t* restrict latt)
{
    return MDA_GET_4(*latt, vec->a, vec->b, vec->c, vec->d);
}

static inline double GetEnergyConvValue(const double temp)
{
    return 1.0 / (temp * MC_CONST_BLOTZMANN_ELV);
}

static inline void ConvEnergyPhysToBoltz(double* restrict value, const double convValue)
{
    *value *= convValue;
}

static inline void ConvEnergyBoltzToPhys(double* restrict value, const double convValue)
{
    *value /= convValue;
}