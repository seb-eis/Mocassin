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
#include "Simulator/Logic/JumpSelection/JumpSelection.h"
#include "Simulator/Logic/Routines/Helper/HelperRoutines.h"
#include "Simulator/Logic/Routines/Main/MainRoutines.h"
#include "Simulator/Logic/Validators/Validators.h"
#include "Simulator/Data/SimContext/ContextAccess.h"
#include "Simulator/Logic/Initializers/ContextInit/ContextInit.h"
#include "Framework/Basic/BaseTypes/Buffers.h"
#include "Simulator/Logic/Initializers/CmdArgResolver/CmdArgumentResolver.h"
#include "Simulator/Logic/Routines/PrintOut/PrintRoutines.h"
#include "UnitTesting/MinimalUnitTest.h"
#include "UnitTesting/UnitTests.h"

#define MC_AWAIT_TERMINATION_OK

int main(int argc, char const * const *argv)
{
    var SCONTEXT = ctor_SimulationContext();

    ResolveCommandLineArguments(&SCONTEXT, argc, argv);

    LoadSimulationModelFromDatabase(&SCONTEXT);

    PrepareForMainRoutine(&SCONTEXT);

    StartMainSimulationRoutine(&SCONTEXT);

    PrintFinishNotice(&SCONTEXT, stdout);

    #if defined(MC_AWAIT_TERMINATION_OK)
        getchar();
    #endif
}