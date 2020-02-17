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

// Internal main function that requires argv to be in utf8 encoding
int _main(int argc, char const * const *argv);

#if defined(WIN32)
#include <wchar.h>
// Windows unicode entry point that gets the arguments string as UTF16
int wmain(int argc, wchar_t const* const* argv)
{
    char* utf8Argv[argc];
    for (var i = 0; i < argc; ++i)
    {
        let error = Win32ConvertUtf16ToUtf8(argv[i], &utf8Argv[i]) <= 0 ? ERR_VALIDATION : ERR_OK;
        error_assert(error, "Failure on converting UTF16 argument set to UTF8.");
    }
    return _main(argc, (const char *const *) utf8Argv);
}
#else
// Normal entry point for OS with native utf8
int main(int argc, char const * const *argv)
{
    return _main(argc, argv);
}
#endif

int _main(int argc, char const * const *argv)
{
    // General preparations for routine execution
    var SCONTEXT = ctor_SimulationContext();
    ResolveCommandLineArguments(&SCONTEXT, argc, argv);
    JobLoader_LoadDatabaseModelToContext(&SCONTEXT);
    PrepareForMainRoutine(&SCONTEXT);

    // Load and jump into a custom extension routine if valid data exists
    var routine = MocExt_TryFindExtensionRoutine(getCustomRoutineUuid(&SCONTEXT), getFileInformation(&SCONTEXT)->ExtensionLookupPath);
    if (routine != NULL)
    {
        ProgressPrint_OnSimulationStart(&SCONTEXT, stdout);
        fprintf(stdout, "\nINFO  => Regular progress prints may be suppressed in custom routines.\n");
        fflush(stdout);
        return (routine(&SCONTEXT), 0);
    }

    // Jump into the usual KMC/MMM system if no extension routine data exist
    StartMainSimulationRoutine(&SCONTEXT);
    ProgressPrint_OnSimulationFinish(&SCONTEXT, stdout);

    #if defined(MC_AWAIT_TERMINATION_OK)
    getchar();
    #endif
    return 0;
}

