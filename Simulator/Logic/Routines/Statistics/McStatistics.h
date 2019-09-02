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

// Defines the particle statistics data type that contains all information for a simulation particle
typedef struct ParticleStatistics
{
    // The particle id the calculation is for
    byte_t                      ParticleId;

    // The particle charge value in units of [C]
    double                      ParticleCharge;

    // The particle count in the lattice
    int32_t                     ParticleCount;

    // The particle density value in [1/m^3]
    double                      ParticleDensity;

    // The super-cell volume in [1/m^3]
    double                      SuperCellVolume;

    // Pointer to the affiliated counter collection
    StateCounterCollection_t*   CounterCollection;

} ParticleStatistics_t;

// Defines the data type that carries all missing information to calculate a conductivity value from the simulation data
typedef struct ParticleMobilityData
{
    // The particle statistics used for the calculation
    ParticleStatistics_t* ParticleStatistics;

    // The total mobility in [m^2/(V s)]
    double      TotalMobility;

    // The total conductivity in [S/m]
    double      TotalConductivity;

    // The normalized conductivity per positive charge number [S/(m*z)]
    double      TotalConductivityPerCharge;

    //   The actual migration rate in [Hz]
    double      MigrationRate;

    // The total diffusion coefficient components in [m^2/s] in (x,y,z) direction
    Vector3_t   DiffusionCoefficient;

    // The conductivity components in [S/m] as calculated using the Stokes-Einstein relation (Fluctuation dissipation theorem)
    Vector3_t   NernstEinsteinConductivity;

    // The mobility vector in [m^2/(V s)]
    Vector3_t   MobilityVector;

    // The ensemble movement vector in [m]
    Vector3_t   EnsembleMoveR1;

    // The ensembles squared movement vector in [m^2]
    Vector3_t   EnsembleMoveR2;

    // The mean movement vector in [m]
    Vector3_t   MeanMoveR1;

    // The mean square movement vector in [m^2]
    Vector3_t   MeanMoveR2;

} ParticleMobilityData_t;

// Calculates the current time step per jump attempt
static inline double GetCurrentTimeStepPerJumpAttempt(SCONTEXT_PARAM)
{
    let factors = getPhysicalFactors(SCONTEXT);
    let header = getDbModelJobHeaderAsKMC(SCONTEXT);
    let pool = getJumpSelectionPool(SCONTEXT);
    let dofFactor = (getDbModelJobInfo(SCONTEXT)->JobFlags & INFO_FLG_DUALDOF) ? 2.0 : 1.0;

    return dofFactor * factors->TotalJumpNormalization / (header->AttemptFrequencyModulus * (double) pool->SelectableJumpCount);
}

// Calculates the basic normalization factor from the max jump probability
static inline double GetBasicJumpNormalization(SCONTEXT_PARAM)
{
    let metaInfo = getMainStateMetaInfo(SCONTEXT);
    return 1.0 / metaInfo->Data->RawMaxJumpProbability;
}

// Calculates the total jump normalization from basic and dynamic normalization values
static inline double GetTotalJumpNormalization(SCONTEXT_PARAM)
{
    let header = getDbModelJobHeaderAsKMC(SCONTEXT);
    let basicNormalization = GetBasicJumpNormalization(SCONTEXT);

    return (isfinite(basicNormalization))
        ? basicNormalization * header->FixedNormalizationFactor
        : header->FixedNormalizationFactor;
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

    #if defined (OPT_PRECHECK_FREQUENCY)
    return factors->TotalJumpNormalization;
    #else
    let jumpRule = getActiveJumpRule(SCONTEXT);
    return factors->TotalJumpNormalization * jumpRule->FrequencyFactor;
    #endif
}

// Updates the time stepping per jump to the current value
static inline void UpdateTimeStepPerJumpToCurrent(SCONTEXT_PARAM)
{
    var factors = getPhysicalFactors(SCONTEXT);
    factors->TimeStepPerJumpAttempt = GetCurrentTimeStepPerJumpAttempt(SCONTEXT);
}

// Updates the total jump normalization and affiliated step timing to the current value using the current max probability
static inline void UpdateTotalJumpNormalization(SCONTEXT_PARAM)
{
    var factors = getPhysicalFactors(SCONTEXT);
    var metaData = getMainStateMetaData(SCONTEXT);
    factors->TotalJumpNormalization = GetTotalJumpNormalization(SCONTEXT);
    metaData->JumpNormalization = factors->TotalJumpNormalization;

    UpdateTimeStepPerJumpToCurrent(SCONTEXT);
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

// Get the linear or square displacement vector of the mobile tracker ensemble of the passed particle
Vector3_t CalculateMobileTrackerEnsembleShift(SCONTEXT_PARAM, byte_t particleId, bool_t isSquared);

// Get the linear displacement vector of the static tracker ensemble of the passed particle
Vector3_t CalculateStaticTrackerEnsembleShift(SCONTEXT_PARAM, byte_t particleId);

// Get the linear displacement vector of the global tracker ensemble of the passed particle and jump collection
Vector3_t GetGlobalTrackerEnsembleShift(SCONTEXT_PARAM, JumpCollection_t *jumpCollection, byte_t particleId);

// Calculates a mobility vector in [m^2/(V s)] using the provided mean displacement and normalized field vector
Vector3_t CalculateMobilityVector(SCONTEXT_PARAM, const Vector3_t *displacement, const Vector3_t *normFieldVector);

// Calculates the conductivity components (x,y,z) for the passed particle id in [S/m] from diffusion components and particle density
// using the fluctuation dissipation theorem
Vector3_t CalculateNernstEinsteinConductivity(SCONTEXT_PARAM, const Vector3_t *diffusionVector, byte_t particleId, double particleDensity);

// Calculates the total mobility in [m^2/(V s)] using the provided mean displacement and normalized field vector
double CalculateFieldProjectedMobility(SCONTEXT_PARAM, const Vector3_t *displacement, const Vector3_t *normFieldVector);

// Calculates the particle density in [1/m^3] for the passed particle id
double CalculateParticleDensity(SCONTEXT_PARAM, byte_t particleId);

// Calculates the volume of the simulation super-cell in [m^3]
double CalculateSuperCellVolume(SCONTEXT_PARAM);

// Calculates a conductivity value in [S/m] from the passed mobility [m^2 / (V s)], charge [e] and particle density [1/(m^3)]
double CalculateTotalConductivity(double mobility, double charge, double particleDensity);

// Calculates and adds the mobility result data that results from the passed data info and context using the mobile tracking system
void PopulateMobilityData(SCONTEXT_PARAM, ParticleMobilityData_t *restrict data);

// Calculates and adds the particle statistics using the provided
void PopulateParticleStatistics(SCONTEXT_PARAM, ParticleStatistics_t *restrict statistics);