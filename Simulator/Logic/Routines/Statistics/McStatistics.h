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
static inline double GetCurrentTimeStepPerJumpAttempt(SCONTEXT_PARAMETER)
{
    let factors = getPhysicalFactors(simContext);
    let header = getDbModelJobHeaderAsKMC(simContext);
    let pool = getJumpSelectionPool(simContext);
    let dofFactor = (getDbModelJobInfo(simContext)->JobFlags & INFO_FLG_DUALDOF) ? 2.0 : 1.0;

    return dofFactor * factors->TotalJumpNormalization / (header->AttemptFrequencyModulus * (double) pool->SelectableJumpCount);
}

// Calculates the basic normalization factor from the max jump probability
static inline double GetBasicJumpNormalization(SCONTEXT_PARAMETER)
{
    let metaInfo = getMainStateMetaInfo(simContext);
    return 1.0 / metaInfo->Data->RawMaxJumpProbability;
}

// Calculates the total jump normalization from basic and dynamic normalization values
static inline double GetTotalJumpNormalization(SCONTEXT_PARAMETER)
{
    let header = getDbModelJobHeaderAsKMC(simContext);
    let basicNormalization = GetBasicJumpNormalization(simContext);

    return (isfinite(basicNormalization))
        ? basicNormalization * header->FixedNormalizationFactor
        : header->FixedNormalizationFactor;
}

// Calculates the energy conversion factor from [eV] to [kT]
static inline double GetEnergyFactorEvToKt(SCONTEXT_PARAMETER)
{
    let jobInfo = getDbModelJobInfo(simContext);
    return 1.0 / (jobInfo->Temperature * NATCONST_BLOTZMANN);
}

// Calculates the current electric field influence from active rule and direction
static inline double GetCurrentElectricFieldJumpInfluence(SCONTEXT_PARAMETER)
{
    let direction = getActiveJumpDirection(simContext);
    let rule = getActiveJumpRule(simContext);

    // Note: The direction factor is calculated in C# as a coulomb energy (field * charge * distance)
    //       giving E(coulomb) = E_f * q * d => Value has to be negated to describe the gain in energy!
    return -(direction->ElectricFieldFactor * rule->ElectricFieldFactor);
}

// Calculates the probability pre factor using the current cycle state
static inline double GetCurrentProbabilityPreFactor(SCONTEXT_PARAMETER)
{
    let factors = getPhysicalFactors(simContext);

    #if defined (OPT_PRECHECK_FREQUENCY)
    return factors->TotalJumpNormalization;
    #else
    let jumpRule = getActiveJumpRule(simContext);
    return factors->TotalJumpNormalization * jumpRule->FrequencyFactor;
    #endif
}

// Updates the time stepping per jump to the current value
static inline void UpdateTimeStepPerJumpToCurrent(SCONTEXT_PARAMETER)
{
    var factors = getPhysicalFactors(simContext);
    factors->TimeStepPerJumpAttempt = GetCurrentTimeStepPerJumpAttempt(simContext);
}

// Updates the total jump normalization and affiliated step timing to the current value using the current max probability
static inline void UpdateTotalKmcJumpNormalization(SCONTEXT_PARAMETER)
{
    var factors = getPhysicalFactors(simContext);
    var metaData = getMainStateMetaData(simContext);
    factors->TotalJumpNormalization = GetTotalJumpNormalization(simContext);
    metaData->JumpNormalization = factors->TotalJumpNormalization;

    UpdateTimeStepPerJumpToCurrent(simContext);
}

// Advances the simulated time span by the current time step per jump
static inline void AdvanceSimulatedTimeByCurrentStep(SCONTEXT_PARAMETER)
{
    let factors = getPhysicalFactors(simContext);
    var metaData = getMainStateMetaData(simContext);
    metaData->SimulatedTime += factors->TimeStepPerJumpAttempt;
}

// Calculates the default status of the cycle counters and sets them on the passed counter state
error_t SetCycleCounterStateToDefault(SCONTEXT_PARAMETER, CycleCounterState_t *counters);

// Calculates the physical simulation factors and sets them on the passed factor collection
error_t SetPhysicalSimulationFactorsToDefault(SCONTEXT_PARAMETER, PhysicalInfo_t *factors);

// Get the linear (fractional) or square displacement (cartesian) vector of the mobile tracker ensemble of the passed particle
Vector3_t CalculateMobileTrackerEnsembleShift(SCONTEXT_PARAMETER, byte_t particleId, bool_t isSquared);

// Get the linear displacement vector of the static tracker ensemble of the passed particle
Vector3_t CalculateStaticTrackerEnsembleShift(SCONTEXT_PARAMETER, byte_t particleId);

// Get the linear displacement vector of the global tracker ensemble of the passed particle and jump collection
Vector3_t GetGlobalTrackerEnsembleShift(SCONTEXT_PARAMETER, JumpCollection_t *jumpCollection, byte_t particleId);

// Calculates a mobility vector in [m^2/(V s)] using the provided mean displacement and normalized field vector
Vector3_t CalculateMobilityVector(SCONTEXT_PARAMETER, const Vector3_t *displacement, const Vector3_t *normFieldVector);

// Calculates the conductivity components (x,y,z) for the passed particle id in [S/m] from diffusion components and particle density
// using the fluctuation dissipation theorem
Vector3_t CalculateNernstEinsteinConductivity(SCONTEXT_PARAMETER, const Vector3_t *diffusionVector, byte_t particleId, double particleDensity);

// Calculates the total mobility in [m^2/(V s)] using the provided mean displacement and normalized field vector
double CalculateFieldProjectedMobility(SCONTEXT_PARAMETER, const Vector3_t *displacement, const Vector3_t *normFieldVector);

// Calculates the particle density in [1/m^3] for the passed particle id
double CalculateParticleDensity(SCONTEXT_PARAMETER, byte_t particleId);

// Calculates the volume of the simulation super-cell in [m^3]
double CalculateSuperCellVolume(SCONTEXT_PARAMETER);

// Calculates a conductivity value in [S/m] from the passed mobility [m^2 / (V s)], charge [e] and particle density [1/(m^3)]
double CalculateTotalConductivity(double mobility, double charge, double particleDensity);

// Calculates and adds the mobility result data that results from the passed data info and context using the mobile tracking system
void PopulateMobilityData(SCONTEXT_PARAMETER, ParticleMobilityData_t *restrict data);

// Calculates and adds the particle statistics using the provided
void PopulateParticleStatistics(SCONTEXT_PARAMETER, ParticleStatistics_t *restrict statistics);