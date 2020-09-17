//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	EnvRoutines.h        		//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Set of helper routines      //
//////////////////////////////////////////

#pragma once
#include "Libraries/Simulator/Data/SimContext/SimulationContextAccess.h"
#include "Libraries/Simulator/Logic/Helper/Constants.h"
#include "Libraries/Framework/Errors/McErrors.h"
#include "Libraries/Framework/Basic/BaseTypes.h"
#include "Libraries/Framework/Math/Vectors.h"
#include "xmmintrin.h"

// Set a code byte at the provided index to the provided value
static inline void SetOccupationCodeByteAt(OccupationCode64_t* restrict code, const int32_t id, const byte_t value)
{
    code->ParticleIds[id] = value;
}

// Get the code byte value at the provided index
static inline byte_t GetOccupationCodeByteAt(OccupationCode64_t* restrict code, const int32_t id)
{
    return code->ParticleIds[id];
}

// Adds two 4D vectors and trims the result into the unit cell
static inline Vector4_t AddAndTrimVector4(const Vector4_t* restrict lhs, const Vector4_t* restrict rhs, const Vector4_t*restrict sizes)
{
    var result = AddVector4(lhs, rhs);
    PeriodicTrimVector4(&result, sizes);
    return result;
}

// Finds the max jump direction count in the jump count table
static inline int32_t FindMaxJumpDirectionCount(const JumpCountTable_t* restrict jumpCountTable)
{
    int32_t max = 0;
    cpp_foreach(count, *jumpCountTable) max = getMaxOfTwo(*count, max);
    return max;
}

// Resolves the passed pair definition and start environment to the target environment state
static inline EnvironmentState_t* GetPairDefinitionTargetEnvironment(SCONTEXT_PARAMETER, const PairInteraction_t *restrict pairDef, const EnvironmentState_t *startEnv)
{
    let target = AddAndTrimVector4(&startEnv->LatticeVector, &pairDef->RelativeVector, getLatticeSizeVector(simContext));
    return getEnvironmentStateByVector4(simContext, &target);
}

// Get the highest index within the update particle set of an environment definition
static inline byte_t GetMaxParticleUpdateId(EnvironmentDefinition_t *restrict envDef)
{
    for(byte_t i = 0;; i++)
        if(envDef->UpdateParticleIds[i] == PARTICLE_NULL)
            return (i == 0) ? (byte_t) PARTICLE_NULL : envDef->UpdateParticleIds[i-1];
}

// Get the highest index of a particle occupation on the passed environment model
static inline byte_t GetEnvironmentMaxParticleId(EnvironmentDefinition_t *restrict envDef)
{
    return_if(envDef->PositionParticleIds[0] == PARTICLE_NULL, PARTICLE_NULL);

    byte_t maxId = 0;
    for(byte_t i = 0;; i++)
    {
        if(envDef->PositionParticleIds[i] != PARTICLE_NULL)
            maxId = getMaxOfTwo(maxId, envDef->PositionParticleIds[i]);
        else
            return maxId;
    }
}

// Check if the job info has the passed flags set to true
static inline bool_t JobInfoFlagsAreSet(SCONTEXT_PARAMETER, const Bitmask_t flgs)
{
    return flagsAreTrue(getDbModelJobInfo(simContext)->JobFlags, flgs);
}

// Check if the job header has the passed flags set to true
static inline bool_t JobHeaderFlagsAreSet(SCONTEXT_PARAMETER, const Bitmask_t flgs)
{
    return flagsAreTrue(getJobHeaderFlagsMmc(simContext), flgs);
}

// Check if the main state has the passed flags set
static inline bool_t StateFlagsAreSet(SCONTEXT_PARAMETER, const int32_t flgs)
{
    return flagsAreTrue(getMainStateHeader(simContext)->Data->Flags, flgs);
}

// Get the total position count of the lattice
static inline int32_t GetLatticePositionCount(SCONTEXT_PARAMETER)
{
    let sizes = getLatticeSizeVector(simContext);
    return sizes->A * sizes->B * sizes->C * sizes->D;
}

// Get tha maximum particle id defined in the simulation
static inline byte_t GetMaxParticleId(SCONTEXT_PARAMETER)
{
    int32_t dimensions[2];
    GetArrayDimensions((VoidArray_t*) getJumpCountMapping(simContext), &dimensions[0]);
    return (byte_t) dimensions[1] - 1;
}

// Get the number of unit cells in the lattice
static inline int32_t GetUnitCellCount(SCONTEXT_PARAMETER)
{
    let sizes = getLatticeSizeVector(simContext);
    return sizes->A * sizes->B * sizes->C;
}

// Get the distance between two points on a 1D axis that has a periodic boundary and limited size
static inline int32_t GetPeriodicPointDistance(const int32_t pointA, const int32_t pointB, const int32_t axisSize)
{
    let distance = abs(pointA - pointB);
    return (distance < (axisSize / 2)) ? distance : abs(distance - axisSize);
}

// Get a boolean value indicating if the two passed 4D positions are in interaction range
static inline bool_t PositionAreInInteractionRange(SCONTEXT_PARAMETER, const Vector4_t* vector0, const Vector4_t* vector1)
{
    // To be in interaction range all distances (x,y,z) on their affiliated periodic 1D axis have to be in range
    let interactionRange = &getDbStructureModel(simContext)->InteractionRange;
    let latticeSizes = getLatticeSizeVector(simContext);

    var result = GetPeriodicPointDistance(vector0->A, vector1->A, latticeSizes->A) > interactionRange->A;
    return_if(result, false);
    result = GetPeriodicPointDistance(vector0->B, vector1->B, latticeSizes->B) > interactionRange->B;
    return_if(result, false);
    return GetPeriodicPointDistance(vector0->C, vector1->C, latticeSizes->C) <= interactionRange->C;
}

// Count the number of particles in the simulation lattice that have the provided particle id
static inline int32_t GetParticleCountInLattice(SCONTEXT_PARAMETER, const byte_t particleId)
{
    int32_t result = 0;
    cpp_foreach(envState, *getEnvironmentLattice(simContext))
        if (envState->ParticleId == particleId)
            result++;

    return result;
}

// Get the currently valid energy value of the passed environment state in units of [kT]
static inline double GetEnvironmentStateEnergy(const EnvironmentState_t* restrict envState)
{
    return span_Get(envState->EnergyStates, envState->ParticleId);
}

// Get the next compare double between [0,1] from the RNG
static inline double GetNextRandomDoubleFromContextRng(SCONTEXT_PARAMETER)
{
    return Pcg32NextRandomDouble(getMainRng(simContext));
}

// Get a ceiled random number from the main RNG
static inline int32_t GetNextCeiledRandomFromContextRng(SCONTEXT_PARAMETER, const int32_t upperLimit)
{
    return Pcg32NextCeiledRandom(getMainRng(simContext), upperLimit);
}

// Checks if the passed pair table is constant and has always the same energy value independent of context
bool_t CheckPairEnergyTableIsConstant(SCONTEXT_PARAMETER, const PairTable_t *restrict table);

// Checks if the passed cluster table is constant and has always the same energy value independent of context
bool_t CheckClusterEnergyTableIsConstant(SCONTEXT_PARAMETER, const ClusterTable_t *restrict table);

// Checks if the pair interaction of a environment definition is link irrelevant (Is const value and has not cluster dependencies)
bool_t CheckPairInteractionIsLinkIrrelevantByIndex(SCONTEXT_PARAMETER, const EnvironmentDefinition_t *restrict environment, int32_t pairId);
