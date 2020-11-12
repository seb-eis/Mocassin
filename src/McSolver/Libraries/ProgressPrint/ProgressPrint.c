//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	FullProgressPint.h        	//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Full print routines         //
//////////////////////////////////////////

#include <math.h>

#include "Libraries/Framework/Basic/Buffers.h"
#include "Libraries/Simulator/Logic/Routines/HelperRoutines.h"
#include "Libraries/Simulator/Logic/Routines/StatisticsRoutines.h"
#include "ProgressPrint.h"

#define MC_OUTTAG_FORMAT "%-35s"
#define MC_OUTPRC_FORMAT "%+.3e%%"
#define MC_OUTF64_FORMAT "%+.12e"
#define MC_OUTSTR_FORMAT "%-20s"
#define MC_OUTCMD_FORMAT "%s"
#define MC_OUTCOM_FORMAT FORMAT_I64(-9)"/"FORMAT_I64(10)
#define MC_OUTI32_FORMAT FORMAT_I32(-20)
#define MC_OUTI64_FORMAT FORMAT_I64(-20)
#define MC_OUTU64_FORMAT FORMAT_U64(-20)
#define MC_OUTVEC_FORMAT MC_OUTF64_FORMAT" "MC_OUTF64_FORMAT" "MC_OUTF64_FORMAT
#define MC_UNIT_FORMAT "[%-13s]"
#define MC_DEFAULT_FORMAT(VALUEFORMAT,...) MC_OUTTAG_FORMAT ": " MC_UNIT_FORMAT " " VALUEFORMAT " " __VA_ARGS__ "\n"
#define MC_STATHEADER_FORMAT "<PARTICLE_DUMP ID=%i CHARGE=%+.2e COUNT=%i)>\n"

// Calculates an eta in [s] for the remaining program duration
static inline int64_t GetRemainingRunTimeEta(SCONTEXT_PARAMETER)
{
    let metaData = getMainStateMetaData(simContext);
    let counters = getMainCycleCounters(simContext);

    var result = ((double) (counters->TotalSimulationGoalMcsCount - counters->McsCount) / metaData->SuccessRate);
    return (isfinite(result)) ? (int64_t) result : 0;
}

// Get the total cycle count of a state counter collection
static inline int64_t  GetCounterCollectionCycleCount(const StateCounterCollection_t* restrict counters)
{
    return counters->McsCount + counters->RejectionCount + counters->SkipCount
           + counters->UnstableEndCount + counters->UnstableStartCount
           + counters->SiteBlockingCount;
}

// Checks if a particle id is potentially marked as mobile in any environment definition
static inline bool_t ParticleIsMarkedAsMobile(SCONTEXT_PARAMETER, const byte_t particleId)
{
    let jumpCountTable = getJumpCountMapping(simContext);
    int32_t dimensions[] = {0, 0};
    GetArrayDimensions((VoidArray_t*) jumpCountTable, dimensions);
    for (int32_t posId = 0; posId < dimensions[0]; ++posId)
    {
        let jumpCount = array_Get(*jumpCountTable, posId, particleId);
        if (jumpCount != JPOOL_DIRCOUNT_STATIC) return true;
    }

    return false;
}

// Prints the passed particle statistics data to the stream
static void PrintParticleStatistics(const ParticleStatistics_t* restrict statistics, file_t*restrict fstream)
{
    let counters = statistics->CounterCollection;
    let totalCycles = GetCounterCollectionCycleCount(statistics->CounterCollection);

    fprintf(fstream, MC_STATHEADER_FORMAT,
            statistics->ParticleId, statistics->ParticleCharge, statistics->ParticleCount);

    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTF64_FORMAT), "Particle density", "m^-3",
            statistics->ParticleDensity);

    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTI64_FORMAT, MC_OUTPRC_FORMAT), "Cycles => Total", "",
            totalCycles, getPercent(totalCycles, totalCycles));

    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTI64_FORMAT, MC_OUTPRC_FORMAT), "Cycles => Success", "",
            counters->McsCount, getPercent(counters->McsCount, totalCycles));

    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTI64_FORMAT, MC_OUTPRC_FORMAT), "Cycles => Skipped", "",
            counters->SkipCount, getPercent(counters->SkipCount, totalCycles));

    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTI64_FORMAT, MC_OUTPRC_FORMAT), "Cycles => Rejected", "",
            counters->RejectionCount, getPercent(counters->RejectionCount, totalCycles));

    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTI64_FORMAT, MC_OUTPRC_FORMAT), "Cycles => Site blocked", "",
            counters->SiteBlockingCount, getPercent(counters->SiteBlockingCount, totalCycles));

    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTI64_FORMAT, MC_OUTPRC_FORMAT), "Cycles => Start unstable", "",
            counters->UnstableStartCount, getPercent(counters->UnstableStartCount, totalCycles));

    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTI64_FORMAT, MC_OUTPRC_FORMAT), "Cycles => End unstable", "",
            counters->UnstableEndCount, getPercent(counters->UnstableEndCount, totalCycles));

    fflush(fstream);
}

// Prints the passed particle mobility data to the stream
static void PrintParticleMobility(const ParticleMobilityData_t* restrict data, file_t*restrict fstream)
{
    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTF64_FORMAT), "Conductivity => Field direction", "S m^-1",
            data->TotalConductivity);

    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTF64_FORMAT), "Conductivity => Normalized (z=+1)", "S m^-1",
            data->TotalConductivityPerCharge);

    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTVEC_FORMAT), "Conductivity => FDT components", "S m^-1",
            data->NernstEinsteinConductivity.A, data->NernstEinsteinConductivity.B, data->NernstEinsteinConductivity.C);

    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTF64_FORMAT), "Mobility => Average migration rate", "Hz",
            data->MigrationRate);

    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTF64_FORMAT), "Mobility => Field direction", "m^2 V^-1 s^-1",
            data->TotalMobility);

    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTVEC_FORMAT), "Mobility => Components", "m^2 V^-1 s^-1",
            data->MobilityVector.A, data->MobilityVector.B, data->MobilityVector.C);

    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTVEC_FORMAT), "Movement => Diffusion coefficient", "m^2 s^-1",
            data->DiffusionCoefficient.A, data->DiffusionCoefficient.B, data->DiffusionCoefficient.C);

    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTVEC_FORMAT), "Movement => Ensemble R", "m^2",
            data->EnsembleMoveR1.A, data->EnsembleMoveR1.B, data->EnsembleMoveR1.C);

    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTVEC_FORMAT), "Movement => Ensemble R^2", "m",
            data->EnsembleMoveR2.A, data->EnsembleMoveR2.B, data->EnsembleMoveR2.C);

    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTVEC_FORMAT), "Movement => Mean <R>", "m",
            data->MeanMoveR1.A, data->MeanMoveR1.B, data->MeanMoveR1.C);

    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTVEC_FORMAT), "Movement => Mean <R^2>", "m^2",
            data->MeanMoveR2.A, data->MeanMoveR2.B, data->MeanMoveR2.C);
    fprintf(fstream, "\n");
    fflush(fstream);
}

// Prints the particle information (optional only mobile skip) about the passed context to the passed stream
static void PrintParticleInformation(SCONTEXT_PARAMETER, FILE *fstream, const bool_t onlyMobiles)
{
    let meta = getDbStructureModelMetaData(simContext);
    for (byte_t i = 1; isfinite(meta->ParticleCharges[i]);++i)
    {
        continue_if(!ParticleIsMarkedAsMobile(simContext, i) && onlyMobiles);

        var particleStatistics = (ParticleStatistics_t) { .ParticleId = i, .ParticleCharge = meta->ParticleCharges[i] };
        PopulateParticleStatistics(simContext, &particleStatistics);
        PrintParticleStatistics(&particleStatistics, fstream);

        continue_if(JobInfoFlagsAreSet(simContext, INFO_FLG_MMC));

        var conductivityData = (ParticleMobilityData_t) { .ParticleStatistics = &particleStatistics };
        PopulateMobilityData(simContext, &conductivityData);
        PrintParticleMobility(&conductivityData, fstream);
    }
}

// Prints the basic run statistics information about the passed context to the passes stream
static void PrintRunStatisticsMetaInfo(SCONTEXT_PARAMETER, FILE* fstream)
{
    char runTimeBuffer[100];
    char etaBuffer[100];

    var jobInfo = getDbModelJobInfo(simContext);
    var metaData = getMainStateMetaData(simContext);
    var stateHeaderData = getMainStateHeader(simContext)->Data;
    var fluctuationBuffer = getLatticeEnergyBuffer(simContext);
    var cycleCounters = getMainCycleCounters(simContext);
    let remainingRunTimeEta = GetRemainingRunTimeEta(simContext);

    SecondsToIso8601FormattedTimePeriod(runTimeBuffer, (int64_t) metaData->ProgramRunTime);
    SecondsToIso8601FormattedTimePeriod(etaBuffer, remainingRunTimeEta);

    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTCOM_FORMAT, MC_OUTPRC_FORMAT), "Status => Completion", "",
            stateHeaderData->Mcs, cycleCounters->TotalSimulationGoalMcsCount,
            getPercent(stateHeaderData->Mcs, cycleCounters->TotalSimulationGoalMcsCount));

    let preRunMcs = getMinOfTwo(cycleCounters->McsCount, cycleCounters->PrerunGoalMcs);
    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTCOM_FORMAT, MC_OUTPRC_FORMAT), "Status => Pre run completion", "",
            preRunMcs, cycleCounters->PrerunGoalMcs, getPercent(preRunMcs, cycleCounters->PrerunGoalMcs));

    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTI64_FORMAT, MC_OUTPRC_FORMAT), "Cycles => Total", "",
            stateHeaderData->Cycles, getPercent(stateHeaderData->Cycles, stateHeaderData->Cycles));

    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTI64_FORMAT, MC_OUTPRC_FORMAT), "Cycles => Success", "",
            stateHeaderData->Mcs, getPercent(stateHeaderData->Mcs, stateHeaderData->Cycles));

    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTSTR_FORMAT, MC_OUTPRC_FORMAT), "Time => Program running", "ISO8601",
            runTimeBuffer, getPercent(metaData->ProgramRunTime, jobInfo->TimeLimit));

    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTSTR_FORMAT, MC_OUTPRC_FORMAT), "Time => Completion ETA", "ISO8601",
            etaBuffer, getPercent(remainingRunTimeEta, jobInfo->TimeLimit));

    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTF64_FORMAT), "Time => Simulated", "s",
            metaData->SimulatedTime);

    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTF64_FORMAT), "Time => Per cycle", "s",
            1.0 / metaData->CycleRate);

    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTF64_FORMAT), "Time => Per block", "s",
            metaData->TimePerBlock);

    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTF64_FORMAT), "Rate => Cycle", "Hz",
            metaData->CycleRate);

    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTF64_FORMAT), "Rate => Mcs", "Hz",
            metaData->SuccessRate);

    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTF64_FORMAT), "Energy => Lattice", "eV",
            metaData->LatticeEnergy);

    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTF64_FORMAT, MC_OUTPRC_FORMAT), "Energy => Abort fluctuation", "eV",
            fluctuationBuffer->LastSum, getPercent(fluctuationBuffer->LastSum, metaData->LatticeEnergy));

    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTF64_FORMAT), "Probability => Norm. Factor", "",
            metaData->JumpNormalization);

    fprintf(fstream, "\n");
    fflush(fstream);

}

void PrintMocassinSimulationBlockInfo(SCONTEXT_PARAMETER, FILE *fstream, bool_t onlyMobiles)
{
    fprintf(fstream, "<STATISTICS_DUMP>\n");
    PrintRunStatisticsMetaInfo(simContext, fstream);
    PrintParticleInformation(simContext, fstream, onlyMobiles);
    fflush(fstream);
}

//  Prints the simulation copyright and basic data information
static void PrintCopyrightInfo(file_t* fstream)
{
    fprintf(fstream, "Authors       - Sebastian Eisele [1,2]\n");
    fprintf(fstream, "Copyright     - [1] Helmholtz Institute Muenster, HIMS, IEK-12 Juelich Research Center, Germany\n");
    fprintf(fstream, "              - [2] Institute Of Physical Chemistry, RWTH Aachen University, Germany\n");
    fprintf(fstream, "SQLite C/C++  - https://www.sqlite.org/copyright.html\n\n");
    fprintf(fstream, "Notes         - Software is provided as is with no warranty. Please report errors to the contacts!\n");
    fprintf(fstream, "              - Metric space of output is euclidean!\n");
    fprintf(fstream, "Contacts      - s.eisele@fz-juelich.de\n");
    fflush(fstream);
}

// Prints the general job information to a stream
static void PrintGeneralJobInfo(SCONTEXT_PARAMETER, file_t *fstream)
{
    char buffer[100], extName[9];
    let jobInfo = getDbModelJobInfo(simContext);
    let cmdArgs = getCommandArguments(simContext);
    let executionPath = cmdArgs->Values[0];

    let jobType = JobInfoFlagsAreSet(simContext, INFO_FLG_MMC) ? "METROPOLIS" : "KINETIC";
    let routineUuid = getCustomRoutineUuid(simContext);
    let runType = StateFlagsAreSet(simContext, STATE_FLG_PRERUN) ? "PRERUN" : "MAIN";
    let stateLoaded = StateFlagsAreSet(simContext, STATE_FLG_FIRSTCYCLE) ? "FALSE" : "TRUE";
    GetCurrentIso8601UtcTimeStamp(buffer);
    fprintf(fstream, "<JOB_INFORMATION>\n");
    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTSTR_FORMAT), "Job => Start time", "ISO8601", buffer);

    SecondsToIso8601FormattedTimePeriod(buffer, jobInfo->TimeLimit);
    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTSTR_FORMAT), "Job => Runtime limit", "ISO8601", buffer);

    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTSTR_FORMAT), "Job => Simulation type", "", jobType);

    memset(buffer,0, sizeof(buffer));
    memcpy(extName,routineUuid->D,8);
    extName[8]=0;

    sprintf(buffer, "[%08x-%04x-%04x-%02x%02x-%02x%02x%02x%02x%02x%02x] (%s)", routineUuid->A, routineUuid->B, routineUuid->C,
            routineUuid->D[0], routineUuid->D[1], routineUuid->D[2], routineUuid->D[3],
            routineUuid->D[4], routineUuid->D[5], routineUuid->D[6], routineUuid->D[7], extName);
    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTSTR_FORMAT), "Job => Extension info", "", buffer);

    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTSTR_FORMAT), "Job => Current status", "", runType);

    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTSTR_FORMAT), "Job => State loaded", "", stateLoaded);

    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTCMD_FORMAT), "CMD => Execution path", "", executionPath);

    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTCMD_FORMAT), "CMD => Main state dump", "", getMainRunStateFile(simContext));

    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTCMD_FORMAT), "CMD => Pre-Run state dump", "", getPreRunStateFile(simContext));

    fflush(fstream);
    for (int32_t i = 1; i < cmdArgs->Count; i=i+2)
    {
        fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTCMD_FORMAT, MC_OUTCMD_FORMAT), "CMD => Argument", "",
                cmdArgs->Values[i], cmdArgs->Values[i+1]);
    }
    fprintf(fstream, "\n");
    fflush(fstream);
}

void PrintMocassinSimulationStartInfo(SCONTEXT_PARAMETER, file_t *fstream)
{
    fprintf(fstream, "\n(C11) MOCASSIN SIMULATOR for HPC\n");
    PrintCopyrightInfo(fstream);

    fprintf(fstream, "\n");
    PrintGeneralJobInfo(simContext, fstream);

    PrintMocassinSimulationBlockInfo(simContext, fstream, false);
    fflush(fstream);
}

void PrintMocassinSimulationContextResetInfo(SCONTEXT_PARAMETER, file_t *fstream)
{
    fprintf(fstream, "\n<END_OF_PRE_RUN>\n");
    PrintMocassinSimulationBlockInfo(simContext, fstream, true);
    fflush(fstream);
}

//  Prints all set state flags of the context as a readable string to the passed file-stream
static void PrintStatusFlagCollection(SCONTEXT_PARAMETER, file_t* fstream)
{
    debug_assert(fstream == NULL && simContext != NULL);

    let flags = getMainStateHeader(simContext)->Data->Flags;
    fprintf(fstream, "Abort-flags: ");

    if (flagsAreTrue(flags, STATE_FLG_COMPLETED)) fprintf(fstream, "ABORT_REASON_COMPLETED ");
    if (flagsAreTrue(flags, STATE_FLG_TIMEOUT)) fprintf(fstream, "ABORT_REASON_TIMEOUT ");
    if (flagsAreTrue(flags, STATE_FLG_CONDABORT)) fprintf(fstream, "ABORT_REASON_CONDITION ");
    if (flagsAreTrue(flags, STATE_FLG_RATEABORT)) fprintf(fstream, "ABORT_REASON_SUCCESSRATE ");
    if (flagsAreTrue(flags, STATE_FLG_ENERGYABORT)) fprintf(fstream, "ABORT_REASON_LATTICEENERGY ");
    fprintf(fstream, "\n");
    fflush(fstream);
}

void PrintMocassinSimulationFinishInfo(SCONTEXT_PARAMETER, file_t *fstream)
{
    char buffer[100];
    let waitTime = 1;
    let flags = getMainStateHeader(simContext)->Data->Flags;
    SecondsToIso8601FormattedTimePeriod(buffer, (int64_t) getMainStateMetaData(simContext)->ProgramRunTime);

    PrintMocassinSimulationBlockInfo(simContext, fstream, true);
    fprintf(fstream, "Main routine reached end @ %s  (ERR_CODE=0x%08x, STATE_FLAGS=" FORMAT_I64() ")\n", buffer, SIMERROR, flags);
    PrintStatusFlagCollection(simContext, fstream);
    fprintf(fstream, "Auto termination in %i seconds...", waitTime);
    fflush(fstream);
    sleep(waitTime);
}