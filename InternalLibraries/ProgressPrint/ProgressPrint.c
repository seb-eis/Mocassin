//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	DebugRoutines.h        		//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Debug routines              //
//////////////////////////////////////////

#include <math.h>

#include "Framework/Basic/BaseTypes/Buffers.h"
#include "Simulator/Logic/Routines/Helper/HelperRoutines.h"
#include "Simulator/Logic/Routines/Statistics/McStatistics.h"
#include "InternalLibraries/Interfaces/ProgressPrint.h"

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
#define MC_STATHEADER_FORMAT "\n== Particle statistics for (Id=%i, Charge=%+.2e [e], Count=%i) ==\n"

// Calculates an eta in [s] for the remaining program duration
static inline int64_t GetRemainingRunTimeEta(SCONTEXT_PARAM)
{
    let metaData = getMainStateMetaData(SCONTEXT);
    let counters = getMainCycleCounters(SCONTEXT);

    var result = ((double) (counters->TotalSimulationGoalMcsCount - counters->McsCount) / metaData->SuccessRate);
    return (isfinite(result)) ? (int64_t) result : 0;
}

// Get the total cycle count of a state counter collection
static inline int64_t  GetCounterCollectionCycleCount(const StateCounterCollection_t* restrict counters)
{
    return counters->McsCount + counters->RejectionCount
        + counters->UnstableEndCount + counters->UnstableStartCount
        + counters->SiteBlockingCount;
}

// Checks if a particle id is potentially marked as mobile in any environment definition
static inline bool_t ParticleIsMarkedAsMobile(SCONTEXT_PARAM, const byte_t particleId)
{
    cpp_foreach(jumpCollection, *getJumpCollections(SCONTEXT))
        return_if(flagsAreTrue(jumpCollection->MobileParticlesMask, 1 << particleId), true);

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

    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTF64_FORMAT), "Mobility => Field direction", "m^2 V^-1 s^-1",
            data->TotalMobility);

    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTVEC_FORMAT), "Mobility => Components", "m^2 V^-1 s^-1",
            data->MobilityVector.A, data->MobilityVector.B, data->MobilityVector.C);

    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTF64_FORMAT), "Movement => Diffusion coefficient", "m^2 s^-1",
            data->DiffusionCoefficient);

    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTVEC_FORMAT), "Movement => Ensemble R", "m^2",
            data->EnsembleMoveR1.A, data->EnsembleMoveR1.B, data->EnsembleMoveR1.C);

    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTVEC_FORMAT), "Movement => Ensemble R^2", "m",
            data->EnsembleMoveR2.A, data->EnsembleMoveR2.B, data->EnsembleMoveR2.C);

    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTVEC_FORMAT), "Movement => Mean <R>", "m",
            data->MeanMoveR1.A, data->MeanMoveR1.B, data->MeanMoveR1.C);

    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTVEC_FORMAT), "Movement => Mean <R^2>", "m^2",
            data->MeanMoveR2.A, data->MeanMoveR2.B, data->MeanMoveR2.C);

    fflush(fstream);
}

// Prints the particle information (optional only mobile skip) about the passed context to the passed stream
static void PrintParticleInformation(SCONTEXT_PARAM, FILE *fstream, const bool_t onlyMobiles)
{
    let meta = getDbStructureModelMetaData(SCONTEXT);
    for (byte_t i = 1; isfinite(meta->ParticleCharges[i]);++i)
    {
        continue_if(!ParticleIsMarkedAsMobile(SCONTEXT, i) && onlyMobiles);

        var particleStatistics = (ParticleStatistics_t) { .ParticleId = i, .ParticleCharge = meta->ParticleCharges[i] };
        PopulateParticleStatistics(SCONTEXT, &particleStatistics);
        PrintParticleStatistics(&particleStatistics, fstream);

        continue_if(JobInfoFlagsAreSet(SCONTEXT, INFO_FLG_MMC));

        var conductivityData = (ParticleMobilityData_t) { .ParticleStatistics = &particleStatistics };
        PopulateMobilityData(SCONTEXT, &conductivityData);
        PrintParticleMobility(&conductivityData, fstream);
    }
}

// Prints the basic run statistics information about the passed context to the passes stream
static void PrintRunStatisticsMetaInfo(SCONTEXT_PARAM, FILE* fstream)
{
    char runTimeBuffer[100];
    char etaBuffer[100];

    var jobInfo = getDbModelJobInfo(SCONTEXT);
    var metaData = getMainStateMetaData(SCONTEXT);
    var stateHeaderData = getMainStateHeader(SCONTEXT)->Data;
    var fluctuationBuffer = getLatticeEnergyBuffer(SCONTEXT);
    var cycleCounters = getMainCycleCounters(SCONTEXT);
    let remainingRunTimeEta = GetRemainingRunTimeEta(SCONTEXT);

    SecondsToISO8601TimeSpan(runTimeBuffer, (int64_t) metaData->ProgramRunTime);
    SecondsToISO8601TimeSpan(etaBuffer, remainingRunTimeEta);

    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTCOM_FORMAT, MC_OUTPRC_FORMAT), "Status => Completion", "",
            stateHeaderData->Mcs, cycleCounters->TotalSimulationGoalMcsCount, getPercent(stateHeaderData->Mcs, cycleCounters->TotalSimulationGoalMcsCount));

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

    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTF64_FORMAT), "Probability => Max value", "",
            metaData->RawMaxJumpProbability);

    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTF64_FORMAT), "Probability => Normalization", "",
            metaData->JumpNormalization);

    fprintf(fstream, "\n");
    fflush(fstream);

}

void PrintFullSimulationStatistics(SCONTEXT_PARAM, FILE *fstream, const bool_t onlyMobiles)
{
    fprintf(fstream, "\n\nn== Simulation statistics status ==\n\n");
    PrintRunStatisticsMetaInfo(SCONTEXT, fstream);
    PrintParticleInformation(SCONTEXT, fstream, onlyMobiles);
    fflush(fstream);
}

//  Prints the simulation copyright information
static void PrintCopyrightInfo(file_t* fstream)
{
    fprintf(fstream, "Authors       - Sebastian Eisele [1], John Arnold [2]\n");
    fprintf(fstream, "Copyright     - [1] Helmholtz Institute Muenster, HIMS, IEK-12 Juelich Research Center, Germany\n");
    fprintf(fstream, "              - [2] Institute Of Physical Chemistry, RWTH Aachen University, Germany\n");
    fprintf(fstream, "SQLite C/C++  - https://www.sqlite.org/copyright.html\n\n");
    fprintf(fstream, "Note          - Software is in beta state and provided as is. Please report errors to the contacts!\n");
    fprintf(fstream, "Contacts      - s.eisele@fz-juelich.de, arnold@pc.rwth-aachen.de\n");
    fflush(fstream);
}

// Prints the general job information to a stream
static void PrintGeneralJobInfo(SCONTEXT_PARAM, file_t *fstream)
{
    char buffer[100];
    let jobInfo = getDbModelJobInfo(SCONTEXT);
    let cmdArgs = getCommandArguments(SCONTEXT);
    let executionPath = cmdArgs->Values[0];

    let jobType = JobInfoFlagsAreSet(SCONTEXT, INFO_FLG_MMC) ? "METROPOLIS" : "KINETIC";
    let runType = StateFlagsAreSet(SCONTEXT, STATE_FLG_PRERUN) ? "PRERUN" : "MAIN";
    let stateLoaded = StateFlagsAreSet(SCONTEXT, STATE_FLG_FIRSTCYCLE) ? "FALSE" : "TRUE";

    GetCurrentTimeStampISO8601UTC(buffer);
    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTSTR_FORMAT), "Job => Start time", "ISO8601", buffer);

    SecondsToISO8601TimeSpan(buffer, jobInfo->TimeLimit);
    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTSTR_FORMAT), "Job => Runtime limit", "ISO8601", buffer);

    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTSTR_FORMAT), "Job => Simulation type", "", jobType);

    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTSTR_FORMAT), "Job => Current status", "", runType);

    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTSTR_FORMAT), "Job => State loaded", "", stateLoaded);

    fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTCMD_FORMAT), "CMD => Execution path", "", executionPath);

    fflush(fstream);
    for (int32_t i = 1; i < cmdArgs->Count; i=i+2)
    {
        fprintf(fstream, MC_DEFAULT_FORMAT(MC_OUTCMD_FORMAT, MC_OUTCMD_FORMAT), "CMD => Argument ", "",
                cmdArgs->Values[i], cmdArgs->Values[i+1]);
    }

    fprintf(fstream, "\n");
    fflush(fstream);
}

void PrintJobStartInfo(SCONTEXT_PARAM, file_t *fstream)
{

    fprintf(fstream, "=== MOCASSIN SIMULATION START NOTIFICATION ===\n\n");

    fprintf(fstream, "(C11) MOCASSIN SIMULATOR for HPC\n\n");
    PrintCopyrightInfo(fstream);

    fprintf(fstream, "=== JOB INFORMATION ===\n\n");
    PrintGeneralJobInfo(SCONTEXT, fstream);

    fprintf(fstream, "=== MOCASSIN SIMULATION START STATE STATUS ===\n\n");
    PrintFullSimulationStatistics(SCONTEXT, fstream, false);
    fprintf(fstream, "==============================================\n\n");
    fflush(fstream);
}

void PrintContextResetNotice(SCONTEXT_PARAM, file_t *fstream)
{
    fprintf(fstream, "\n\n=== MOCASSIN PRE-RUN COMPLETION NOTIFICATION ===\n\n");
    PrintFullSimulationStatistics(SCONTEXT, fstream, true);
    fflush(fstream);
}

void PrintFinishNotice(SCONTEXT_PARAM, file_t* fstream)
{
    char buffer[100];
    let waitTime = 10;
    let flags = getMainStateHeader(SCONTEXT)->Data->Flags;
    SecondsToISO8601TimeSpan(buffer, (int64_t) getMainStateMetaData(SCONTEXT)->ProgramRunTime);

    PrintFullSimulationStatistics(SCONTEXT, stdout, true);
    fprintf(stdout, "Main routine reached end @ %s  (ERR=0x%08x, FLAGS=0x%08x)\n", buffer, SIMERROR, flags);
    fprintf(stdout, "Auto termination in %i seconds...", waitTime);
    fflush(stdout);
    sleep(waitTime);
}