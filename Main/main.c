#include "InternalLibraries/Interfaces/ProgressPrint.h"
#include "InternalLibraries/Interfaces/JobLoader.h"
#include "Simulator/Data/SimContext/ContextAccess.h"
#include "Simulator/Logic/Routines/Main/MainRoutines.h"
#include "Simulator/Logic/Initializers/CmdArgResolver/CmdArgumentResolver.h"

#define MC_AWAIT_TERMINATION_OK

int main(int argc, char const * const *argv)
{
    var SCONTEXT = ctor_SimulationContext();

    ResolveCommandLineArguments(&SCONTEXT, argc, argv);

    LoadSimulationDbModelToContext(&SCONTEXT);

    PrepareForMainRoutine(&SCONTEXT);

    StartMainSimulationRoutine(&SCONTEXT);

    PrintFinishNotice(&SCONTEXT, stdout);

    #if defined(MC_AWAIT_TERMINATION_OK)
        getchar();
    #endif
}