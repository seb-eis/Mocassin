//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	McStatistics.h   		    //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   MC parameter calculations   //
//////////////////////////////////////////

#pragma once
#include "Framework/Errors/McErrors.h"
#include "Simulator/Logic/Constants/Constants.h"
#include "Framework/Basic/BaseTypes/BaseTypes.h"
#include "Simulator/Data/SimContext/ContextAccess.h"
#include "Simulator/Data/SimContext/SimContext.h"

// Calculates the current time step per jump
static inline double CalcTimeStepPerJump(SCONTEXT_PARAM)
{
    return getPhysicalFactors(SCONTEXT)->TotalNormalizationFactor
        / (getDbModelJobHeaderAsKMC(SCONTEXT)->AttemptFrequencyModulus
        * (double) getJumpSelectionPool(SCONTEXT)->SelectableJumpCount);
}

// Calculates the basic normalization factor from the max jump probability
static inline double CalcBasicJumpNormalization(SCONTEXT_PARAM)
{
    return 1.0 / getMainStateMetaInfo(SCONTEXT)->Data->MaxJumpProbability;
}

// Calculates the total jump normalization from basic, dynamic and attempt freqeuncy modulus
static inline double CalcTotalJumpNormalization(SCONTEXT_PARAM)
{
    return CalcBasicJumpNormalization(SCONTEXT)
    * getDbModelJobHeaderAsKMC(SCONTEXT)->FixedNormFactor;
}

// Calculates the energy conversion factor
static inline double CalcEnergyConversionFactor(SCONTEXT_PARAM)
{
    return 1.0 / (getDbModelJobInfo(SCONTEXT)->Temperature * NATCONST_BLOTZMANN);
}

// Calculates the current electric field influence
static inline double CalcElectricFieldInfluence(SCONTEXT_PARAM)
{
    return getDbModelJobHeaderAsKMC(SCONTEXT)->ElectricFieldModulus
        * getActiveJumpDirection(SCONTEXT)->ElectricFieldFactor
        * getActiveJumpRule(SCONTEXT)->ElectricFieldFactor;
}

// Calculates the probability pre factor using the current cycle state
static inline double CalcCurrentProbabilityPreFactor(SCONTEXT_PARAM)
{
    return getDbModelJobHeaderAsKMC(SCONTEXT)->AttemptFrequencyModulus
    * getDbModelJobHeaderAsKMC(SCONTEXT)->FixedNormFactor;
}

// Updates the time stepping per jump to the current value
static inline void UpdateTimeStepping(SCONTEXT_PARAM)
{
    getPhysicalFactors(SCONTEXT)->CurrentTimeStepping = CalcTimeStepPerJump(SCONTEXT);
}

// Advances the simulated time span by the current time step per jump
static inline void AdvanceSimulatedTime(SCONTEXT_PARAM)
{
    getMainStateMetaData(SCONTEXT)->SimulatedTime += getPhysicalFactors(SCONTEXT)->CurrentTimeStepping;
}

// Calculates the default status of the cycle counters and sets them on the passed counter state
error_t SetCycleCounterToDefaultStatus(SCONTEXT_PARAM, CycleCounterState_t *counters);

// Calculates the physical simulation factors and sets them on the passed factor collection
error_t SetPhysicalSimulationFactorsToDefault(SCONTEXT_PARAM, PhysicalInfo_t *factors);