//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	main.c          		    //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Main simulation entry point //
//////////////////////////////////////////

#include "Framework/Basic/DlLoading/RoutineLoading.h"
#include "InternalLibraries/Interfaces/ProgressPrint.h"
#include "InternalLibraries/Interfaces/JobLoader.h"
#include "Simulator/Data/SimContext/ContextAccess.h"
#include "Simulator/Logic/Routines/Main/MainRoutines.h"
#include "Simulator/Logic/Initializers/CmdArgResolver/CmdArgumentResolver.h"

#define MC_AWAIT_TERMINATION_OK

int main(int argc, char const * const *argv)
{
    // General preparations for routine execution
    var SCONTEXT = ctor_SimulationContext();
    ResolveCommandLineArguments(&SCONTEXT, argc, argv);
    JobLoader_LoadDatabaseModelToContext(&SCONTEXT);
    PrepareForMainRoutine(&SCONTEXT);

    // Load and jump into a custom extension routine if valid data exists
    var extensionRoutine = MocExt_TryFindExtensionRoutine(getCustomRoutineUuid(&SCONTEXT), NULL);
    if (extensionRoutine != NULL)
    {
        ProgressPrint_OnSimulationStart(&SCONTEXT, stdout);
        return (extensionRoutine(&SCONTEXT), 0);
    }

    // Jump into the usual KMC/MMc system if no extension routine data exist
    StartMainSimulationRoutine(&SCONTEXT);
    ProgressPrint_OnSimulationFinish(&SCONTEXT, stdout);

    #if defined(MC_AWAIT_TERMINATION_OK)
        getchar();
    #endif
    return 0;
}