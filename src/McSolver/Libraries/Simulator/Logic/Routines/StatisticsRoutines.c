//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	McStatistics.c   		    //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   MC parameter calculations   //
//////////////////////////////////////////

#include <math.h>
#include "Libraries/Simulator/Logic/Helper/Constants.h"
#include "StatisticsRoutines.h"
#include "Libraries/Simulator/Logic/Routines/HelperRoutines.h"

static inline Vector3_t TransformFractionalToCartesian(Vector3_t *vector, const UnitCellVectors_t* cellVectors)
{
    Vector3_t result;
    result.A = vector->A * cellVectors->A.A + vector->B * cellVectors->B.A  + vector->C * cellVectors->C.A;
    result.B = vector->A * cellVectors->A.B + vector->B * cellVectors->B.B  + vector->C * cellVectors->C.B;
    result.C = vector->A * cellVectors->A.C + vector->B * cellVectors->B.C  + vector->C * cellVectors->C.C;
    return result;
}

error_t SetCycleCounterStateToDefault(SCONTEXT_PARAMETER, CycleCounterState_t *counters)
{
    return_if (counters == NULL, ERR_NULLPOINTER);
    nullStructContent(*counters);

    let jobInfo = getDbModelJobInfo(simContext);
    let mobileCount = getNumberOfMobiles(simContext);
    let kmcHeader = JobInfoFlagsAreSet(simContext, INFO_FLG_KMC) ? getDbModelJobHeaderAsKMC(simContext) : NULL;
    return_if(mobileCount == 0, ERR_NOMOBILES);

    counters->PrerunGoalMcs = (kmcHeader != NULL) ? kmcHeader->PreRunMcsp * mobileCount : 0;
    counters->TotalSimulationGoalMcsCount = counters->PrerunGoalMcs + jobInfo->TargetMcsp * mobileCount;
    return_if(counters->TotalSimulationGoalMcsCount == 0, ERR_DATACONSISTENCY);

    let rem = counters->TotalSimulationGoalMcsCount % CYCLE_BLOCKCOUNT;
    if (rem != 0) counters->TotalSimulationGoalMcsCount += CYCLE_BLOCKCOUNT - rem;
    counters->McsCountPerExecutionPhase = counters->TotalSimulationGoalMcsCount / CYCLE_BLOCKCOUNT;

    counters->CycleCountPerExecutionLoop = counters->McsCountPerExecutionPhase * CYCLE_BLOCKSIZE_MUL;
    counters->CycleCountPerExecutionLoop = getMinOfTwo(counters->CycleCountPerExecutionLoop, CYCLE_BLOCKSIZE_MAX);
    counters->CycleCountPerExecutionLoop = getMinOfTwo(counters->CycleCountPerExecutionLoop, CYCLE_BLOCKSIZE_MIN);

    return ERR_OK;
}

error_t SetPhysicalSimulationFactorsToDefault(SCONTEXT_PARAMETER, PhysicalInfo_t *factors)
{
    return_if (factors == NULL, ERR_NULLPOINTER);

    factors->EnergyFactorEvToKt = GetEnergyFactorEvToKt(simContext);
    factors->EnergyFactorKtToEv = 1.0 / factors->EnergyFactorEvToKt;

    return_if (!JobInfoFlagsAreSet(simContext, INFO_FLG_KMC), ERR_OK);

    let jobHeader = getDbModelJobHeaderAsKMC(simContext);
    if (isfinite(GetBasicJumpNormalization(simContext)))
        factors->TotalJumpNormalization = GetTotalJumpNormalization(simContext);
    else
        factors->TotalJumpNormalization = jobHeader->FixedNormalizationFactor;

    factors->TimeStepPerJumpAttempt = GetCurrentTimeStepPerJumpAttempt(simContext);
    return ERR_OK;
}

Vector3_t CalculateMobileTrackerEnsembleShift(SCONTEXT_PARAMETER, byte_t particleId, bool_t isSquared)
{
    Vector3_t result = {.A = 0, .B = 0, .C = 0};
    let meta = getDbStructureModelMetaData(simContext);

    cpp_foreach(envState, *getEnvironmentLattice(simContext))
    {
        continue_if((envState->ParticleId != particleId) || (envState->MobileTrackerId <= INVALID_INDEX));

        var tracker = *getMobileTrackerAt(simContext, envState->MobileTrackerId);
        if (isSquared)
        {
            tracker = TransformFractionalToCartesian(&tracker, &meta->CellVectors);
            vector3VectorOp(tracker, tracker, *=);
        }
        vector3VectorOp(result, tracker, +=);
    }

    return result;
}

Vector3_t CalculateStaticTrackerEnsembleShift(SCONTEXT_PARAMETER, byte_t particleId)
{
    Vector3_t result = {.A = 0, .B = 0, .C = 0};

    cpp_foreach(envState, *getEnvironmentLattice(simContext))
    {
        continue_if(envState->MobileTrackerId <= INVALID_INDEX);

        let tracker = *getStaticMovementTrackerAt(simContext, &envState->LatticeVector, particleId);
        vector3VectorOp(result, tracker, +=);
    }

    return result;
}

Vector3_t GetGlobalTrackerEnsembleShift(SCONTEXT_PARAMETER, JumpCollection_t *jumpCollection, byte_t particleId)
{
    return *getGlobalMovementTrackerAt(simContext, jumpCollection->ObjectId, particleId);
}

// Get a mobility vector component in [m^2 / (V s)]
static double GetMobilityVectorComponent(SCONTEXT_PARAMETER, const double displacement, const double normField)
{
    if (fabs(normField) < 0.0001) return 0;

    let simulatedTime = getMainStateMetaData(simContext)->SimulatedTime;
    let fieldModulus = getDbModelJobHeaderAsKMC(simContext)->ElectricFieldModulus;
    let component = displacement / (normField * fieldModulus * simulatedTime);
    return component;
}

double CalculateFieldProjectedMobility(SCONTEXT_PARAMETER, const Vector3_t *displacement, const Vector3_t *normFieldVector)
{
    let time = getMainStateMetaData(simContext)->SimulatedTime;
    let fieldModulus = getDbModelJobHeaderAsKMC(simContext)->ElectricFieldModulus;
    let displacementFieldProduct = CalcVector3DotProduct(displacement, normFieldVector);

    let result = displacementFieldProduct / (time * fieldModulus);
    return result;
}

Vector3_t CalculateMobilityVector(SCONTEXT_PARAMETER, const Vector3_t *displacement, const Vector3_t *normFieldVector)
{
    Vector3_t result;
    result.A = GetMobilityVectorComponent(simContext, displacement->A, normFieldVector->A);
    result.B = GetMobilityVectorComponent(simContext, displacement->B, normFieldVector->B);
    result.C = GetMobilityVectorComponent(simContext, displacement->C, normFieldVector->C);
    return result;
}

double CalculateParticleDensity(SCONTEXT_PARAMETER, byte_t particleId)
{
    let volume = CalculateSuperCellVolume(simContext);
    let particleCount = (double) GetParticleCountInLattice(simContext, particleId);
    return particleCount / volume;
}

double CalculateSuperCellVolume(SCONTEXT_PARAMETER)
{
    let cellVectors = &getDbStructureModelMetaData(simContext)->CellVectors;
    let spatProduct = CalcVector3SpatProduct(&cellVectors->A, &cellVectors->B, &cellVectors->C);
    let cellCount = (double) GetUnitCellCount(simContext);
    let volume = fabs(spatProduct) * CONV_VOLUME_ANG_TO_M * cellCount;
    return volume;
}

double CalculateTotalConductivity(const double mobility, const double charge, const double particleDensity)
{
    return mobility * charge * particleDensity * NATCONST_ELMCHARGE;
}

// Get the diffusion coefficient component vector from mean square displacement and time information
static inline Vector3_t CalculateDiffusionCoefficient(const Vector3_t* vectorR2, const double time)
{
    var factor = 0.5 / time;
    return ScalarMultiplyVector3(vectorR2, factor);
}

// Calculates the average actual migration rate per particle in [Hz]
static inline double CalculateAverageMigrationRate(double simulatedTime, int64_t steps, int64_t particleCount)
{
    return (double) steps / (simulatedTime * (double) particleCount);
}

Vector3_t CalculateNernstEinsteinConductivity(SCONTEXT_PARAMETER, const Vector3_t *diffusionVector, const byte_t particleId, const double particleDensity)
{
    let charge = getDbStructureModelMetaData(simContext)->ParticleCharges[particleId] * NATCONST_ELMCHARGE;
    let tempFactor = getDbModelJobInfo(simContext)->Temperature * NATCONST_BLOTZMANN * NATCONST_ELMCHARGE;
    let factor = charge * charge * particleDensity / tempFactor;

    let result = ScalarMultiplyVector3(diffusionVector, factor);
    return result;
}

void PopulateMobilityData(SCONTEXT_PARAMETER, ParticleMobilityData_t *restrict data)
{
    let meta = getDbStructureModelMetaData(simContext);
    let id = data->ParticleStatistics->ParticleId;
    let count = data->ParticleStatistics->ParticleCount;
    let density = data->ParticleStatistics->ParticleDensity;
    let simulatedTime = getMainStateMetaData(simContext)->SimulatedTime;

    data->EnsembleMoveR1 = CalculateMobileTrackerEnsembleShift(simContext, id, false);
    data->EnsembleMoveR2 = CalculateMobileTrackerEnsembleShift(simContext, id, true);
    vector3ScalarOp(data->EnsembleMoveR1, CONV_LENGTH_ANG_TO_M, *=);
    vector3ScalarOp(data->EnsembleMoveR2, CONV_LENGTH_ANG_TO_M*CONV_LENGTH_ANG_TO_M, *=);

    data->EnsembleMoveR1 = TransformFractionalToCartesian(&data->EnsembleMoveR1, &meta->CellVectors);

    data->MeanMoveR1 = data->EnsembleMoveR1;
    data->MeanMoveR2 = data->EnsembleMoveR2;
    vector3ScalarOp(data->MeanMoveR1, count, /=);
    vector3ScalarOp(data->MeanMoveR2, count, /=);

    data->MigrationRate = CalculateAverageMigrationRate(simulatedTime, data->ParticleStatistics->CounterCollection->McsCount, count);
    data->DiffusionCoefficient = CalculateDiffusionCoefficient(&data->MeanMoveR2, simulatedTime);
    data->MobilityVector = CalculateMobilityVector(simContext, &data->MeanMoveR1, &meta->NormElectricFieldVector);
    data->TotalMobility = CalculateFieldProjectedMobility(simContext, &data->MeanMoveR1, &meta->NormElectricFieldVector);
    data->NernstEinsteinConductivity = CalculateNernstEinsteinConductivity(simContext, &data->DiffusionCoefficient, id, density);

    data->TotalConductivity = CalculateTotalConductivity(data->TotalMobility, meta->ParticleCharges[id], density);
    data->TotalConductivityPerCharge = CalculateTotalConductivity(data->TotalMobility, 1, density);
}

void PopulateParticleStatistics(SCONTEXT_PARAMETER, ParticleStatistics_t *restrict statistics)
{
    statistics->SuperCellVolume = CalculateSuperCellVolume(simContext);
    statistics->ParticleCount = GetParticleCountInLattice(simContext, statistics->ParticleId);
    statistics->ParticleDensity = (double) statistics->ParticleCount / statistics->SuperCellVolume;
    statistics->CounterCollection = getMainStateCounterAt(simContext, statistics->ParticleId);
}