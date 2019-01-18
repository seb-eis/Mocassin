
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
#include "Simulator/Data/SimContext/ContextAccess.h"
#include "Simulator/Logic/Constants/Constants.h"
#include "Framework/Errors/McErrors.h"
#include "Framework/Basic/BaseTypes/BaseTypes.h"
#include "Framework/Math/Types/Vector.h"
#include "xmmintrin.h"

// Set a code byte at the provided index to the provided value
static inline void SetCodeByteAt(OccCode_t* restrict code, const int32_t id, const byte_t value)
{
    accessPtrAs(byte_t, code)[id] = value;
}

// Get the code byte value at the provided index
static inline byte_t GetCodeByteAt(OccCode_t* restrict code, const int32_t id)
{
    return accessPtrAs(byte_t, code)[id];
}

// Adds two 4D vectors and trims the result into the unit cell
static inline Vector4_t AddAndTrimVector4(const Vector4_t* restrict lhs, const Vector4_t* restrict rhs, const Vector4_t* sizes)
{
    Vector4_t result = AddVector4(lhs, rhs);
    PeriodicTrimVector4(&result, sizes);
    return result;
}

// Get the conversion factor for [eV] to [kT] by the passed temperature value
static inline double GetEnergyConvValue(const double temp)
{
    return 1.0 / (temp * NATCONST_BLOTZMANN);
}

// Converts [eV] energy to [kT] energy by a conversion factor
static inline void ConvEnergyPhysToBoltz(double* restrict value, const double convValue)
{
    *value *= convValue;
}

// Converts [kT] energy to [eV] energy by a conversion factor
static inline void ConvEnergyBoltzToPhys(double* restrict value, const double convValue)
{
    *value /= convValue;
}

// Finds the max jump direction count in the jump count table
static inline int32_t FindMaxJumpDirectionCount(const JumpCountTable_t* restrict jumpCountTable)
{
    int32_t max = 0;
    cpp_foreach(count, *jumpCountTable)
    {
        max = getMaxOfTwo(*count, max);
    }
    return max;
}

// Get the next compare double between [0,1] from the RNG
static inline double GetNextRandomDouble(__SCONTEXT_PAR)
{
    return Pcg32NextDouble(&SCONTEXT->Rng);
}

// Get a ceiled random number from the main RNG
static inline int32_t GetNextCeiledRandom(__SCONTEXT_PAR, const int32_t upperLimit)
{
    return (int32_t) Pcg32NextCeiled(&SCONTEXT->Rng, upperLimit);
}

// Resolves the passed pair definition and start environment to the target environment state
static inline EnvironmentState_t* GetPairDefinitionTargetEnvironment(__SCONTEXT_PAR, const PairDefinition_t *restrict pairDef, const EnvironmentState_t *startEnv)
{
    Vector4_t target = AddAndTrimVector4(&startEnv->PositionVector, &pairDef->RelativeVector, getLatticeSizeVector(SCONTEXT));
    return getEnvironmentStateByVector4(SCONTEXT, &target);
}

// Get the index of the first update particle that is null
static inline byte_t GetIndexOfFirstNullUpdateParticle(EnvironmentDefinition_t *restrict envDef)
{
    for(byte_t i = 0;; i++)
    {
        if(envDef->UpdateParticleIds[i] == PARTICLE_NULL)
        {
            return envDef->UpdateParticleIds[i-1];
        }
    }
}

// Check if the job info has the passed flags set to true
static inline bool_t JobInfoFlagsAreSet(__SCONTEXT_PAR, const Bitmask_t flgs)
{
    return flagsAreTrue(getDbModelJobInfo(SCONTEXT)->JobFlags, flgs);
}

// Check if the job header has the passed flags set to true
static inline bool_t JobHeaderFlagsAreSet(__SCONTEXT_PAR, const Bitmask_t flgs)
{
    return flagsAreTrue(getJobHeaderFlagsMmc(SCONTEXT), flgs);
}

// Check if the main state has the passed flags set
static inline bool_t StateFlagsAreSet(__SCONTEXT_PAR, const int32_t flgs)
{
    return flagsAreTrue(getMainStateHeader(SCONTEXT)->Data->Flags, flgs);
}

// Get the total position count of the lattice
static inline int32_t GetLatticePositionCount(__SCONTEXT_PAR)
{
    Vector4_t* sizes = getLatticeSizeVector(SCONTEXT);
    return sizes->A * sizes->B * sizes->C * sizes->D;
}

// Get tha maximum particle id defined in the simulation
static inline byte_t GetMaxParticleId(__SCONTEXT_PAR)
{
    int32_t dimensions[2];
    GetArrayDimensions((VoidArray_t*) getJumpCountMapping(SCONTEXT), &dimensions[0]);
    return (byte_t) dimensions[0];
}

// Get the number of unit cells in the lattice
static inline const int32_t GetUnitCellCount(__SCONTEXT_PAR)
{
    const Vector4_t* sizes = getLatticeSizeVector(SCONTEXT);
    return sizes->A * sizes->B * sizes->C;
}

// Get the distance between two points on a 1D axis that has a periodic boundary and limited size
static inline int32_t GetPeriodicPointDistance(const int32_t pointA, const int32_t pointB, const int32_t axisSize)
{
    int32_t distance = abs(pointA - pointB);
    return (distance < (axisSize / 2)) ? distance : abs(distance - axisSize);
}

// Get a boolean value indicating if the two passed 4D positions are in interaction range
static inline bool_t PositionAreInInteractionRange(__SCONTEXT_PAR, const Vector4_t* vector0, const Vector4_t* vector1)
{
    InteractionRange_t* range = &getDbStructureModel(SCONTEXT)->InteractionRange;
    Vector4_t* latticeSizes = getLatticeSizeVector(SCONTEXT);

    if (GetPeriodicPointDistance(vector0->A, vector1->A, latticeSizes->A) <= range->A)
        return true;

    if (GetPeriodicPointDistance(vector0->B, vector1->B, latticeSizes->B) <= range->B)
        return true;

    if (GetPeriodicPointDistance(vector0->C, vector1->C, latticeSizes->C) <= range->C)
        return true;

    return false;
}

// Count the number of particles in the simulation lattice that have the provided particle id
static inline int32_t GetParticleCountInLattice(__SCONTEXT_PAR, const byte_t particleId)
{
    int32_t result = 0;
    cpp_foreach(envState, *getEnvironmentLattice(SCONTEXT))
    {
        if (envState->ParticleId == particleId)
            result++;
    }

    return result;
}


