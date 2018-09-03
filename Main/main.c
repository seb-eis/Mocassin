#include <stdio.h>
#include <stdlib.h>
#include <time.h>
#include <math.h>
#include "Simulator/Data/Model/DbModel/DbModelLoad.h"
#include "Framework/Math/Random/PcgRandom.h"
#include "Framework/Math/Types/Vector.h"
#include "Framework/Basic/BaseTypes/BaseTypes.h"
#include "Framework/Basic/FileIO/FileIO.h"
#include "Framework/Errors/McErrors.h"
#include "Simulator/Logic/Objects/JumpSelection.h"
#include "Simulator/Logic/Routines/HelperRoutines.h"
#include "Simulator/Logic/Routines/MainRoutines.h"
#include "Simulator/Logic/Validators/Validators.h"
#include "Simulator/Data/Model/SimContext/ContextAccess.h"
#include "Simulator/Logic/Initializers/ContextInitializer.h"

#if !defined(MC_TESTBUILD)

    int main(int argc, char const * const *argv)
    {   
        sim_context_t* SCONTEXT = malloc(sizeof(sim_context_t));
        
        char const * values[] = { "./", "-dbPath", "./Main/Simulator.c", "-dbQuery", "0.0.192" };
        int32_t count = sizeof(values) / sizeof(char*);

        ResolveCommandLineArguments(SCONTEXT, count, &values[0]);
        
        return (0);
    }

#else

    int main(int argc, char const * const *argv)
    {
        sim_context_t SCONTEXT;

        ResolveCommandLineArguments(&SCONTEXT, argc, argv);

        LoadSimulationModelFromDatabase(&SCONTEXT);

        PrepareContextForSimulation(&SCONTEXT);

        PrepareForMainRoutine(&SCONTEXT);

        StartMainRoutine(&SCONTEXT);
    }

#endif