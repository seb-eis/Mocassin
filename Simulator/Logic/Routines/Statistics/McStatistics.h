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
static inline double CalcTimeStepPerJump(__SCONTEXT_PAR)
{
    return 1.0 / (getJobHeaderAsKmc(SCONTEXT)->AttemptFrequencyModulus
        * (double) getJumpSelectionPool(SCONTEXT)->NumOfSelectableJumps);
}

// Calculates the basic normalization factor from the max jump probability
static inline double CalcBasicJumpNormalization(__SCONTEXT_PAR)
{
    return 1.0 / getMainStateMetaInfo(SCONTEXT)->Data->MaxJumpProbability;
}

// Calculates the total jump normalization
static inline double CalcTotalJumpNormalization(__SCONTEXT_PAR)
{
    return CalcBasicJumpNormalization(SCONTEXT) * getJobHeaderAsKmc(SCONTEXT)->FixedNormFactor;
}

// Calculates the energy conversion factor
static inline double CalcEnergyConversionFactor(__SCONTEXT_PAR)
{
    return 1.0 / (getJobInformation(SCONTEXT)->Temperature * NATCONST_BLOTZMANN);
}

// Calculates the current electric field influence
static inline double CalcElectricFieldInfluence(__SCONTEXT_PAR)
{
    return getJobHeaderAsKmc(SCONTEXT)->ElectricFieldModulus
        * getActiveJumpDirection(SCONTEXT)->ElectricFieldFactor
        * getActiveJumpRule(SCONTEXT)->ElectricFieldFactor;
}

// Updates the time stepping per jump to the current value
static inline void UpdateTimeStepping(__SCONTEXT_PAR)
{
    getPhysicalFactors(SCONTEXT)->CurrentTimeStepping = CalcTimeStepPerJump(SCONTEXT);
}

// Advances the simulated time span by the current time step per jump
static inline void AdvanceSimulatedTime(__SCONTEXT_PAR)
{
    getMainStateMetaData(SCONTEXT)->SimulatedTime += getPhysicalFactors(SCONTEXT)->CurrentTimeStepping;
}

// Calculates the default status of the cycle counters and sets them on the passed counter state
error_t CalcCycleCounterDefaultStatus(__SCONTEXT_PAR, CycleCounterState_t* counters);

// Calculates the physical simulation factors and sets them on the passed factor collection
error_t CalcPhysicalSimulationFactors(__SCONTEXT_PAR, PhysicalInfo_t* factors);