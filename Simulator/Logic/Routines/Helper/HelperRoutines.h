
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
#include "xmmintrin.h"

#define get_compare(lhs,rhs) ((lhs)==(rhs)) ? 0 : ((lhs)<(rhs)) ? -1 : 1;

static inline void SetCodeByteAt(OccCode_t* restrict code, const int32_t id, const byte_t value)
{
    marshalAs(byte_t, code)[id] = value;
}

static inline byte_t GetCodeByteAt(OccCode_t* restrict code, const int32_t id)
{
    return marshalAs(byte_t, code)[id];
}

static inline Vector4_t AddAndTrimVector4(Vector4_t* restrict lhs, const Vector4_t* restrict rhs, const Vector4_t* sizes)
{
    Vector4_t result = AddVector4(lhs, rhs);
    PeriodicTrimVector4(&result, sizes);
    return result;
}

static inline EnvironmentState_t* GetEnvByVector4(const Vector4_t* restrict vec, const EnvironmentLattice* restrict lattice)
{
    return &array_Get(*lattice, vec->a, vec->b, vec->c, vec->d);
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

static inline int32_t FindMaxJumpDirectionCount(const JumpCountTable_t* restrict jumpCountTable)
{
    int32_t max = 0;
    cpp_foreach(count, *jumpCountTable)
    {
        max = getMaxOfTwo(*count, max);
    }
    return max;
}

static inline double GetNextCompareDouble(__SCONTEXT_PAR)
{
    return Pcg32NextDouble(&SCONTEXT->RandomNumberGenerator);
}

static inline int32_t MakeNextCeiledRnd(__SCONTEXT_PAR, const int32_t upperLimit)
{
    return (int32_t) Pcg32NextCeiled(&SCONTEXT->RandomNumberGenerator, upperLimit);
}

static inline EnvironmentState_t* ResolvePairDefTargetEnvironment(__SCONTEXT_PAR, const PairDefinition_t* restrict pairDef, const EnvironmentState_t* startEnv)
{
    Vector4_t target = AddVector4(&startEnv->PositionVector, &pairDef->RelativeVector);
    PeriodicTrimVector4(&target, getLatticeSizeVector(SCONTEXT));
    return getEnvironmentStateByVector4(SCONTEXT, &target);
}

static inline byte_t FindLastEnvParId(EnvironmentDefinition_t* restrict envDef)
{
    for(byte_t i = 0;; i++)
    {
        if(envDef->UpdateParticleIds[i] == 0)
        {
            return envDef->UpdateParticleIds[i-1];
        }
    }
}

static inline bool_t JobInfoHasFlgs(__SCONTEXT_PAR, const Bitmask_t flgs)
{
    return flagsAreTrue(getJobInformation(SCONTEXT)->JobFlags, flgs);
}

static inline bool_t JobHeaderHasFlgs(__SCONTEXT_PAR, const Bitmask_t flgs)
{
    return flagsAreTrue(getJobHeaderFlagsMmc(SCONTEXT), flgs);
}

static inline bool_t MainStateHasFlags(__SCONTEXT_PAR, const int32_t flgs)
{
    return flagsAreTrue(getMainStateHeader(SCONTEXT)->Data->Flags, flgs);
}

static inline int32_t GetTotalPosCount(__SCONTEXT_PAR)
{
    Vector4_t* sizes = getLatticeSizeVector(SCONTEXT);
    return sizes->a * sizes->b * sizes->c * sizes->d;
}

static inline byte_t GetMaxParId(__SCONTEXT_PAR)
{
    int32_t dimensions[2];
    GetArrayDimensions((VoidArray_t*)getJumpDirectionsPerPositionTable(SCONTEXT), &dimensions[0]);
    return (byte_t) dimensions[0];
}

static inline const int32_t GetNumberOfUnitCells(__SCONTEXT_PAR)
{
    const Vector4_t* sizes = getLatticeSizeVector(SCONTEXT);
    return sizes->a * sizes->b * sizes->c;
}
