
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
#include "Simulator/Data/Model/SimContext/ContextAccess.h"
#include "Simulator/Logic/Constants/Constants.h"
#include "Framework/Errors/McErrors.h"
#include "Framework/Basic/BaseTypes/BaseTypes.h"
#include "Framework/Math/Types/Vector.h"

#define get_compare(lhs,rhs) ((lhs)==(rhs)) ? 0 : ((lhs)<(rhs)) ? -1 : 1;

static inline void SetCodeByteAt(occode_t* restrict code, const byte_t id, const byte_t value)
{
    marshalAs(byte_t, code)[id] = value;
}

static inline byte_t GetCodeByteAt(occode_t* restrict code, const byte_t id)
{
    return marshalAs(byte_t, code)[id];
}

static inline void AddToLhsVector4(vector4_t* restrict lhs, const vector4_t* restrict rhs)
{
    lhs->a += rhs->a;
    lhs->b += rhs->b;
    lhs->c += rhs->c;
    lhs->d += rhs->d;
}

static inline void AddToLhsAndTrimVector4(vector4_t* restrict lhs, const vector4_t* restrict rhs, const vector4_t* sizes)
{
    AddToLhsVector4(lhs, rhs);
    PeriodicTrimVector4(lhs, sizes);
}

static inline env_state_t* GetEnvByVector4(const vector4_t* restrict vec, const env_lattice_t* restrict latt)
{
    return MDA_GET_4(*latt, vec->a, vec->b, vec->c, vec->d);
}

static inline double GetEnergyConvValue(const double temp)
{
    return 1.0 / (temp * NATCONST_BLOTZMANN);
}

static inline void ConvEnergyPhysToBoltz(double* restrict value, const double convValue)
{
    *value *= convValue;
}

static inline void ConvEnergyBoltzToPhys(double* restrict value, const double convValue)
{
    *value /= convValue;
}

static inline int32_t FindMaxJumpDirectionCount(const jump_counts_t* restrict jumpCountTable)
{
    int32_t max = 0;
    FOR_EACH(int32_t, count, *jumpCountTable)
    {
        max = (max < *count) ? *count : max;
    }
    return max;
}

static inline int32_t GetNextCompareDouble(__SCONTEXT_PAR)
{
    return Pcg32NextDouble(&SCONTEXT->RandomNumberGenerator);
}

static inline int32_t GetNextCeiledRnd(__SCONTEXT_PAR, const int32_t upperLimit)
{
    return (int32_t) (Pcg32Next(&SCONTEXT->RandomNumberGenerator) % upperLimit);
}

static inline env_state_t* ResolvePairDefTargetEnvironment(__SCONTEXT_PAR, const pair_def_t* restrict pairDef, const env_state_t* startEnv)
{
    vector4_t target = AddVector4(&startEnv->PositionVector, &pairDef->RelativeVector);
    PeriodicTrimVector4(&target, getLatticeSizeVector(SCONTEXT));
    return getEnvironmentStateByVector4(SCONTEXT, &target);
}

static inline byte_t FindLastEnvParId(env_def_t* restrict envDef)
{
    for(byte_t i = 0;; i++)
    {
        if(envDef->UpdateParticleIds[i] == 0)
        {
            return envDef->UpdateParticleIds[i-1];
        }
    }
}

static inline bool_t JobInfoHasFlgs(__SCONTEXT_PAR, const bitmask_t flgs)
{
    return flagsAreTrue(getJobInformation(SCONTEXT)->JobFlags, flgs);
}

static inline bool_t JobHeaderHasFlgs(__SCONTEXT_PAR, const bitmask_t flgs)
{
    return flagsAreTrue(getJobHeaderFlagsMmc(SCONTEXT), flgs);
}

static inline bool_t MainStateHasFlags(__SCONTEXT_PAR, const int32_t flgs)
{
    return flagsAreTrue(getMainStateHeader(SCONTEXT)->Data->Flags, flgs);
}

static inline int32_t GetTotalPosCount(__SCONTEXT_PAR)
{
    vector4_t* sizes = getLatticeSizeVector(SCONTEXT);
    return sizes->a * sizes->b * sizes->c * sizes->d;
}

static inline byte_t GetMaxParId(__SCONTEXT_PAR)
{
    int32_t dimensions[2];
    GetMdaDimensions((int32_t*)getJumpDirectionsPerPositionTable(SCONTEXT)->Header, &dimensions[0]);
    return dimensions[0];
}

static inline const int32_t GetNumberOfUnitCells(__SCONTEXT_PAR)
{
    const vector4_t* sizes = getLatticeSizeVector(SCONTEXT);
    return sizes->a * sizes->b * sizes->c;
}
