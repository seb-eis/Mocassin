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

void ProgressPrint_OnBlockFinish(SCONTEXT_PARAMETER, file_t *fstream, bool_t onlyMobiles)
{
    fprintf(fstream, ".");
    fflush(fstream);
}

void ProgressPrint_OnSimulationStart(SCONTEXT_PARAMETER, file_t *fstream)
{
    fprintf(fstream, "[START]");
    fflush(fstream);
}

void ProgressPrint_OnContextReset(SCONTEXT_PARAMETER, file_t *fstream)
{
    fprintf(fstream, "[MAIN-RUN]");
    fflush(fstream);
}

void ProgressPrint_OnSimulationFinish(SCONTEXT_PARAMETER, file_t *fstream)
{
    let meta = getDbStructureModelMetaData(simContext);
    let stateMeta = getMainStateMetaData(simContext);
    let energyBuffer = getLatticeEnergyBuffer(simContext);

    fprintf(fstream, "[DONE]:\n");
    fprintf(fstream, "LatticeEnergy:%+.10e[eV]|EnergyBufferSum:%+.10e[eV]|LastBufferSum:%+.10e[eV]\n",
            stateMeta->LatticeEnergy,  energyBuffer->CurrentSum, energyBuffer->LastSum);
    fprintf(fstream, "SimulatedTime:%+.10e[s]|ProgramRunTime:%+.10e[s]\n",
            stateMeta->SimulatedTime, stateMeta->ProgramRunTime);
    fprintf(fstream, "CycleRate:%+.10e[Hz]|McsRate:%+.10e[Hz]\n",
            stateMeta->CycleRate, stateMeta->SuccessRate);

    for (byte_t i = 1; isfinite(meta->ParticleCharges[i]);++i)
    {
        var statisticsData = (ParticleStatistics_t) { .ParticleId = i, .ParticleCharge = meta->ParticleCharges[i] };
        PopulateParticleStatistics(simContext, &statisticsData);

        continue_if(JobInfoFlagsAreSet(simContext, INFO_FLG_MMC));

        var mobilityData = (ParticleMobilityData_t) { .ParticleStatistics = &statisticsData };
        PopulateMobilityData(simContext, &mobilityData);


        let moveR1 = mobilityData.EnsembleMoveR1;
        let moveR2 = mobilityData.EnsembleMoveR2;
        let dCoef = mobilityData.DiffusionCoefficient;
        let counters = statisticsData.CounterCollection;

        fprintf(fstream, "P:%i:Counters(Success:"FORMAT_I64(012)"|Reject:"FORMAT_I64(012)"|Block:"FORMAT_I64(012)"|EndUnstable:"FORMAT_I64(012)"|StartUnstable:"FORMAT_I64(012)")\n", i,
                counters->McsCount, counters->RejectionCount, counters->SiteBlockingCount, counters->UnstableEndCount, counters->UnstableStartCount);
        fprintf(fstream, "P:%i:Mobility:%+.10e[m^2 V^-1 s^-1]\n", i, mobilityData.TotalMobility);
        fprintf(fstream, "P:%i:Conductivity:%+.10e[S m^-1]\n", i, mobilityData.TotalConductivity);
        fprintf(fstream, "P:%i:DiffusionC(x:%.10e|y:%+.10e|z:%+.10e)[m^2 s^-1]\n", i, dCoef.A, dCoef.B, dCoef.C);
        fprintf(fstream, "P:%i:EnsembleR1(x:%+.10e|y:%+.10e|z:%+.10e)[m]\n", i, moveR1.A, moveR1.B, moveR1.C);
        fprintf(fstream, "P:%i:EnsembleR2(x:%+.10e|y:%+.10e|z:%+.10e)[m]\n", i, moveR2.A, moveR2.B, moveR2.C);
    }
    fflush(fstream);
}