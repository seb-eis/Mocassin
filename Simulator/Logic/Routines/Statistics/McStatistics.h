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

// Calculates the current time step per jump attempt
static inline double GetCurrentTimeStepPerJumpAttempt(SCONTEXT_PARAM)
{
    let factors = getPhysicalFactors(SCONTEXT);
    let header = getDbModelJobHeaderAsKMC(SCONTEXT);
    let pool = getJumpSelectionPool(SCONTEXT);

    return factors->TotalJumpNormalization / (header->AttemptFrequencyModulus * (double) pool->SelectableJumpCount);
}

// Calculates the basic normalization factor from the max jump probability
static inline double GetBasicJumpNormalization(SCONTEXT_PARAM)
{
    let metaInfo = getMainStateMetaInfo(SCONTEXT);
    return 1.0 / metaInfo->Data->MaxJumpProbability;
}

// Calculates the total jump normalization from basic and dynamic normalization values
static inline double GetTotalJumpNormalization(SCONTEXT_PARAM)
{
    let header = getDbModelJobHeaderAsKMC(SCONTEXT);
    let basicNormalization = GetBasicJumpNormalization(SCONTEXT);

    return basicNormalization * header->FixedNormalizationFactor;
}

// Calculates the energy conversion factor from [eV] to [kT]
static inline double GetEnergyFactorEvToKt(SCONTEXT_PARAM)
{
    let jobInfo = getDbModelJobInfo(SCONTEXT);
    return 1.0 / (jobInfo->Temperature * NATCONST_BLOTZMANN);
}

// Calculates the current electric field influence from active rule and direction
static inline double GetCurrentElectricFieldJumpInfluence(SCONTEXT_PARAM)
{
    let direction = getActiveJumpDirection(SCONTEXT);
    let rule = getActiveJumpRule(SCONTEXT);

    // Note: The direction factor is calculated in C# as a coulomb energy (field * charge * distance)
    //       giving E(coulomb) = E_f * q * d => Value has to be negated to describe the gain in energy!
    return -(direction->ElectricFieldFactor * rule->ElectricFieldFactor);
}

// Calculates the probability pre factor using the current cycle state
static inline double GetCurrentProbabilityPreFactor(SCONTEXT_PARAM)
{
    let factors = getPhysicalFactors(SCONTEXT);
    let jumpRule = getActiveJumpRule(SCONTEXT);

    return factors->TotalJumpNormalization * jumpRule->FrequencyFactor;
}

// Updates the time stepping per jump to the current value
static inline void UpdateTimeStepPerJumpToCurrent(SCONTEXT_PARAM)
{
    var factors = getPhysicalFactors(SCONTEXT);
    factors->TimeStepPerJumpAttempt = GetCurrentTimeStepPerJumpAttempt(SCONTEXT);
}

// Advances the simulated time span by the current time step per jump
static inline void AdvanceSimulatedTimeByCurrentStep(SCONTEXT_PARAM)
{
    let factors = getPhysicalFactors(SCONTEXT);
    var metaData = getMainStateMetaData(SCONTEXT);
    metaData->SimulatedTime += factors->TimeStepPerJumpAttempt;
}

// Calculates the default status of the cycle counters and sets them on the passed counter state
error_t SetCycleCounterStateToDefault(SCONTEXT_PARAM, CycleCounterState_t *counters);

// Calculates the physical simulation factors and sets them on the passed factor collection
error_t SetPhysicalSimulationFactorsToDefault(SCONTEXT_PARAM, PhysicalInfo_t *factors);