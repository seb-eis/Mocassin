//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	FullProgressPint.h        	//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Minimal print routines      //
//////////////////////////////////////////

#include "Simulator/Logic/Routines/Helper/HelperRoutines.h"
#include "Simulator/Logic/Routines/Statistics/McStatistics.h"
#include "InternalLibraries/Interfaces/ProgressPrint.h"

void PrintFullSimulationStatistics(SCONTEXT_PARAM, file_t *fstream, bool_t onlyMobiles)
{
    let counters = getMainCycleCounters(SCONTEXT);
    fprintf(fstream, "P " FORMAT_I64(012) "/" FORMAT_I64(012)"\n", counters->McsCount, counters->TotalSimulationGoalMcsCount);
    fflush(fstream);
}

void PrintJobStartInfo(SCONTEXT_PARAM, file_t *fstream)
{
    fprintf(fstream, "Job started!\n");
    fflush(fstream);
}

void PrintContextResetNotice(SCONTEXT_PARAM, file_t *fstream)
{
    fprintf(fstream, "\nPre-run completed!\n");
    fflush(fstream);
}

void PrintFinishNotice(SCONTEXT_PARAM, file_t* fstream)
{
    let meta = getDbStructureModelMetaData(SCONTEXT);
    let stateMeta = getMainStateMetaData(SCONTEXT);
    let energyBuffer = getLatticeEnergyBuffer(SCONTEXT);

    fprintf(fstream, "\n");
    fprintf(fstream, "Engergy => %.10e %s %.10e %s %.10e %s\n",
            stateMeta->LatticeEnergy,  "eV", energyBuffer->CurrentSum, "eV", energyBuffer->LastSum, "eV");
    fprintf(fstream, "Time    => %.10e %s %.10e %s\n",
            stateMeta->SimulatedTime, "s ", stateMeta->ProgramRunTime, "s ");
    fprintf(fstream, "Rates   => %.10e %s %.10e %s\n",
            stateMeta->CycleRate, "Hz", stateMeta->SuccessRate, "Hz");

    for (byte_t i = 1; isfinite(meta->ParticleCharges[i]);++i)
    {
        var statisticsData = (ParticleStatistics_t) { .ParticleId = i, .ParticleCharge = meta->ParticleCharges[i] };
        PopulateParticleStatistics(SCONTEXT, &statisticsData);

        continue_if(JobInfoFlagsAreSet(SCONTEXT, INFO_FLG_MMC));

        var mobilityData = (ParticleMobilityData_t) { .ParticleStatistics = &statisticsData };
        PopulateMobilityData(SCONTEXT, &mobilityData);


        let moveR1 = mobilityData.EnsembleMoveR1;
        let moveR2 = mobilityData.EnsembleMoveR2;
        let counters = statisticsData.CounterCollection;

        fprintf(fstream, "\n");
        fprintf(fstream, "P (%i) Counters     => " FORMAT_I64(012)" S "FORMAT_I64(012)" R "FORMAT_I64(012)" B "FORMAT_I64(012)" UE "FORMAT_I64(012)" US\n", i,
                counters->McsCount, counters->RejectionCount, counters->SiteBlockingCount, counters->UnstableEndCount, counters->UnstableStartCount);
        fprintf(fstream, "P (%i) Mobility     => %.10e %s %.10e %s %.10e %s\n", i,
                mobilityData.DiffusionCoefficient, "m^2 s^-1     ",
                mobilityData.TotalMobility,        "m^2 V^-1 s^-1",
                mobilityData.TotalConductivity,    "S m^-1");

        fprintf(fstream, "P (%i) Ensemble R   => %.10e %.10e %.10e %s\n", i,
                moveR1.A, moveR1.B, moveR1.C, "m");
        fprintf(fstream, "P (%i) Ensemble R^2 => %.10e %.10e %.10e %s\n", i,
                moveR2.A, moveR2.B, moveR2.C, "m");
    }
    fprintf(fstream, "\nJob done!\n");
    fflush(fstream);
}