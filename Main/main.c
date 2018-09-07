#include <stdio.h>
#include <stdlib.h>
#include <time.h>
#include <math.h>
#include "Simulator/Data/Model/Database/DbModelLoad.h"
#include "Framework/Math/Random/PcgRandom.h"
#include "Framework/Math/Types/Vector.h"
#include "Framework/Basic/BaseTypes/BaseTypes.h"
#include "Framework/Basic/FileIO/FileIO.h"
#include "Framework/Errors/McErrors.h"
#include "Simulator/Logic/Objects/JumpSelection.h"
#include "Simulator/Logic/Routines/Helper/HelperRoutines.h"
#include "Simulator/Logic/Routines/Main/MainRoutines.h"
#include "Simulator/Logic/Validators/Validators.h"
#include "Simulator/Data/Model/SimContext/ContextAccess.h"
#include "Simulator/Logic/Initializers/ContextInitializer.h"

#if !defined(MC_TESTBUILD)

    int main(int argc, char const * const *argv)
    {   
        int32_t cellsize = 10*10*10;
        int32_t mask = 0;
        for (int32_t i = 0; i < 20; i++)
        {
            mask |= Pcg32GlobalNext() % cellsize;
        }
        for (int32_t i = 0; i < 20; i++)
        {
            int32_t searchValue = Pcg32GlobalNext() % cellsize;
            bool_t isThere = (mask & searchValue) == searchValue;
            printf("Cell %i is %i\n", searchValue, isThere);
        }
        mask = 0;
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