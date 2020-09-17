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
static int InternalMain(int argc, char const * const *argv);

#if defined(WIN32)
#include <wchar.h>
// Windows unicode entry point that gets the arguments string as UTF16
int wmain(int argc, wchar_t const* const* argv)
{
    char* utf8Argv[argc];
    for (var i = 0; i < argc; ++i)
    {
        let error = Win32ConvertUtf16ToUtf8(argv[i], &utf8Argv[i]) < 0 ? ERR_VALIDATION : ERR_OK;
        assert_success(error, "Failure on converting UTF16 argument set to UTF8.");
    }
    return InternalMain(argc, (const char *const *) utf8Argv);
}
#else
// Normal entry point for OS with native utf8
int main(int argc, char const * const *argv)
{
    return InternalMain(argc, argv);
}
#endif

static int InternalMain(int argc, char const * const *argv)
{
    // General preparations for routine execution
    var simContext = ctor_SimulationContext();
    ResolveMocassinCommandLineArguments(&simContext, argc, argv);
    LoadMocassinSimulationDatabaseModelToContext(&simContext);
    PrepareSimulationContextForMainRoutine(&simContext);

    // Load and jump into a custom extension routine if valid data exists
    var routine = TryFindMocassinExtensionRoutine(getCustomRoutineUuid(&simContext),
                                                  getFileInformation(&simContext)->ExtensionLookupPath);
    if (routine != NULL)
    {
        fprintf(stdout, "[Init-Info]: Entering specified custom routine.\n");
        PrintMocassinSimulationStartInfo(&simContext, stdout);
        fflush(stdout);
        return (routine(&simContext), 0);
    }

    // Jump into the usual KMC/MMM system if no extension routine data exist
    StartMainSimulationRoutine(&simContext);
    PrintMocassinSimulationFinishInfo(&simContext, stdout);

    #if defined(MC_AWAIT_TERMINATION_OK)
    getchar();
    #endif
    return 0;
}

