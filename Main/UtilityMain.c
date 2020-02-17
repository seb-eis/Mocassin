//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	UtilityMain.c          		//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Utility entry point         //
//////////////////////////////////////////

#include "Utility/JumpHistogram/JumpHistogramPrint.h"

int _main(int argc, char const * const *argv);

#if defined(WIN32)
#include <wchar.h>
// Windows unicode entry point that gets the arguments string trough the windows API
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

// Defines the span for multiple named utility callback functions
typedef Span_t(NamedCmdFunction_t, UtilityCallbacks) UtilityCallbacks_t;

UtilityCallbacks_t UtilityCmd_GetCallbackCollection()
{
    static NamedCmdFunction_t collection[] =
    {
            (NamedCmdFunction_t) {.Name = "-print-jump-histograms", .Callback = UtilityCmd_PrintJumpHistogram}
    };
    return (UtilityCallbacks_t) span_CArrayToSpan(collection);
}

int _main(int argc, char const * const *argv)
{
    error_assert(argc >= 2 ? ERR_OK : ERR_ARGUMENT, "Invalid number of arguments, no command defined!");
    let callbackName = argv[1];

    cpp_foreach(item, UtilityCmd_GetCallbackCollection())
    {
        if (strcmp(callbackName, item->Name) != 0) continue;
        item->Callback(argc, argv);
        return 0;
    }

    error_assert(ERR_ARGUMENT, "No matching command was found!");
}