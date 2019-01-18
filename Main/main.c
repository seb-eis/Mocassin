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
#include "Framework/Basic/Macros/BinarySearch.h"
#include "Framework/Basic/Macros/Macros.h"
#include <immintrin.h>

#if defined(MC_TESTBUILD)
    int main(int argc, char const * const *argv)
    {
        getchar();
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

        return (0);
    }

#endif