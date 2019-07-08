#include "Utility/JumpHistogram/JumpHistogramPrint.h"

static void PrintJumpHistogramHeader(SimulationState_t* simulationState, file_t* fstream)
{
    let format = "%+10s-%i\t";
    let energyTag = "SampleEng";
    var index = 0;

    cpp_foreach(histogram, simulationState->JumpStatistics)
    {
        fprintf(fstream, format, energyTag, index);
        fprintf(fstream, format, "EEdgeCnt", index);
        fprintf(fstream, format, energyTag, index);
        fprintf(fstream, format, "PConfCnt", index);
        fprintf(fstream, format, energyTag, index);
        fprintf(fstream, format, "NConfCnt", index);
        fprintf(fstream, format, energyTag, index);
        fprintf(fstream, format, "TotalCnt", index++);
        fflush(fstream);
    }
    fprintf(fstream, "\n");
}

static void PrintJumpHistogramOverflows(SimulationState_t* simulationState, file_t* fstream)
{
    let tokFormat = "%-12s\t";
    let cntFormat = FORMAT_I64(12) "\t";
    let lowerToken = "Below_MinVal";
    let upperToken = "Above_MaxVal";

    cpp_foreach(histrogram, simulationState->JumpStatistics)
    {
        fprintf(fstream, tokFormat, lowerToken);
        fprintf(fstream, cntFormat, histrogram->EdgeEnergyHistogram.UnderflowCount);

        fprintf(fstream, tokFormat, lowerToken);
        fprintf(fstream, cntFormat, histrogram->PosConfEnergyHistogram.UnderflowCount);

        fprintf(fstream, tokFormat, lowerToken);
        fprintf(fstream, cntFormat, histrogram->NegConfEnergyHistogram.UnderflowCount);

        fprintf(fstream, tokFormat, lowerToken);
        fprintf(fstream, cntFormat, histrogram->TotalEnergyHistogram.UnderflowCount);
        fflush(fstream);
    }
    fprintf(fstream, "\n");

    cpp_foreach(histrogram, simulationState->JumpStatistics)
    {
        fprintf(fstream, tokFormat, upperToken);
        fprintf(fstream, cntFormat, histrogram->EdgeEnergyHistogram.OverflowCount);

        fprintf(fstream, tokFormat, upperToken);
        fprintf(fstream, cntFormat, histrogram->PosConfEnergyHistogram.OverflowCount);

        fprintf(fstream, tokFormat, upperToken);
        fprintf(fstream, cntFormat, histrogram->NegConfEnergyHistogram.OverflowCount);

        fprintf(fstream, tokFormat, upperToken);
        fprintf(fstream, cntFormat, histrogram->TotalEnergyHistogram.OverflowCount);
        fflush(fstream);
    }
    fprintf(fstream, "\n");
}

static void PrintJumpHistogramContent(SimulationState_t* simulationState, file_t* fstream)
{
    let flpFormat = "%+.5e\t";
    let cntFormat = FORMAT_I64(12) "\t";
    for (var i = 0; i < STATE_JUMPSTAT_SIZE; i++)
    {
        cpp_foreach(histrogram, simulationState->JumpStatistics)
        {
            fprintf(fstream, flpFormat, histrogram->EdgeEnergyHistogram.Stepping * (double) i);
            fprintf(fstream, cntFormat, histrogram->EdgeEnergyHistogram.CountBuffer[i]);

            fprintf(fstream, flpFormat, histrogram->PosConfEnergyHistogram.Stepping * (double) i);
            fprintf(fstream, cntFormat, histrogram->PosConfEnergyHistogram.CountBuffer[i]);

            fprintf(fstream, flpFormat, -1 * histrogram->NegConfEnergyHistogram.Stepping * (double) i);
            fprintf(fstream, cntFormat, histrogram->NegConfEnergyHistogram.CountBuffer[i]);

            fprintf(fstream, flpFormat, histrogram->TotalEnergyHistogram.Stepping * (double) i);
            fprintf(fstream, cntFormat, histrogram->TotalEnergyHistogram.CountBuffer[i]);
            fflush(fstream);
        }
        fprintf(fstream, "\n");
    }
}

error_t PrintJumpHistogramsToStream(SimulationState_t *simulationState, file_t *fstream)
{
    return_if(fstream == NULL || simulationState == NULL, ERR_NULLPOINTER);

    PrintJumpHistogramHeader(simulationState,fstream);
    PrintJumpHistogramOverflows(simulationState,fstream);
    PrintJumpHistogramContent(simulationState,fstream);
    return ERR_OK;
}

void PrintJumpHistogramsFromStateFile(char const *stateFileName, char const *outFileName)
{
    SimulationState_t simulationState;

    var error = LoadContextFreeSimulationStateFromFile(stateFileName, &simulationState);
    error_assert(error, "Could not load the requested file as a simulation state!");
    error_assert(span_Length(simulationState.Buffer) != 0 ? ERR_OK : ERR_FILE, "The loaded state is empty!");

    var fstream = outFileName != NULL ? fopen(outFileName, "w") : stdout;
    error = PrintJumpHistogramsToStream(&simulationState, fstream);
    if (fstream != stdout) fclose(fstream);

    error_assert(error, "Failed to write the data to the target file!");
}

void UtilityCmd_PrintJumpHistogram(int32_t argc, const char*const* argv)
{
    error_assert(argc >= 3 ? ERR_OK : ERR_ARGUMENT, "Invalid number of arguments");
    let sourceName = argv[2];
    error_assert(IsAccessibleFile(sourceName) ? ERR_OK : ERR_ARGUMENT, "Passed source file does not exist or cannot be accessed!");
    PrintJumpHistogramsFromStateFile(sourceName, NULL);
}