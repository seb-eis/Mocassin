#include <stdio.h>
#include <stdlib.h>
#include <stdint.h>
#include <time.h>
#include <math.h>
#include "Simulator/Data/Database/DbModelLoad.h"
#include "Framework/Math/Random/PcgRandom.h"
#include "Framework/Math/Types/Vector.h"
#include "Framework/Basic/BaseTypes/BaseTypes.h"
#include "Framework/Basic/FileIO/FileIO.h"
#include "Framework/Errors/McErrors.h"
#include "Simulator/Logic/Objects/JumpSelection.h"
#include "Simulator/Logic/Routines/Helper/HelperRoutines.h"
#include "Simulator/Logic/Routines/Main/MainRoutines.h"
#include "Simulator/Logic/Validators/Validators.h"
#include "Simulator/Data/SimContext/ContextAccess.h"
#include "Simulator/Logic/Initializers/ContextInit/ContextInit.h"
#include "Framework/Basic/BaseTypes/Buffers.h"
#include "Simulator/Logic/Initializers/CmdArgResolver/CmdArgumentResolver.h"

#if !defined(MC_TESTBUILD)

    int main(int argc, char const * const *argv)
    {
        Pcg32_t random = new_Pcg32(random, 856782567265295, 732567923986295);
        int32_t values[100];

        c_foreach(it, values)
        {
            *it = 0;
        }

        clock_t clock0 = clock();
        for (int i = 0; i < 1000000000; ++i)
        {
          values[Pcg32Next(&random)%99]++;
        }
        clock_t clock1 = clock();

        printf("Value: %llu Time: %li", random.State, clock1-clock0);

        return (0);
    }

#else

    int main(int argc, char const * const *argv)
    {
        SimulationContext_t SCONTEXT;

        ResolveCommandLineArguments(&SCONTEXT, argc, argv);

        LoadSimulationModelFromDatabase(&SCONTEXT);

        PrepareContextForSimulation(&SCONTEXT);

        PrepareForMainRoutine(&SCONTEXT);

        StartMainRoutine(&SCONTEXT);
    }

#endif