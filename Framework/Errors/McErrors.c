//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	Errors.h         		    //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Error codes + messages      //
//////////////////////////////////////////

#include "Framework/Errors/McErrors.h"
#include "Framework/Basic/FileIO/FileIO.h"

const char* ErrorCodeToString(error_t errCode)
{
    // Redirect all non-critical errors to FATAL FAILURE since they should never cause an error string lookup
    errCode = (errCode <= 0) ? 0 : errCode;

    static var defaultMessage = "No detail on error code available";
    static struct { int32_t ErrCode; char* Message; } errTable[] =
    {
            { ERR_OK,               "FATAL FAILURE. Runtime error exit triggered without an error flag.\n\t\t(Expected reason: Implementation error)" },
            { ERR_STREAM,           "File stream operation failed.\n\t\t(Expected reason: Storage system is full)" },
            { ERR_BLOCKDUMP,        "Memory block dump operation failed.\n\t\t(Expected reason: Implementation error)" },
            { ERR_BUFFERCAST,       "Incompatible buffer cast operation detected.\n\t\t(Expected reason: Implementation error)" },
            { ERR_BUFFEROVERFLOW,   "Buffer overflow detected.\n\t\t(Expected reason: Implementation error)" },
            { ERR_FILE,             "File path is missing or invalid.\n\t\t(Expected reason: File is missing or has wrong name)" },
            { ERR_FILEMODE,         "Invalid file mode passed to I/O.\n\t\t(Expected reason: Implementation error)" },
            { ERR_DATABASE,         "Database access failure.\n\t\t(Expected reason: Corrupted or invalid database)" },
            { ERR_UNKNOWN,          "Unspecified error.\n\t\t(Expected reason: Implementation error)" },
            { ERR_MEMALLOCATION,    "Dynamic memory allocation returned NULL.\n\t\t(Expected reason: Process is out of memory)" },
            { ERR_DATACONSISTENCY,  "Data is inconsistent.\n\t\t(Expected reason: Corrupted or invalid input data)" },
            { ERR_HASHPROTECTION,   "Invalid file/state/database checksum.\n\t(Expected reason: Unsupported manual file manipulation)" },
            { ERR_LIBRARYLOADING,   "Plugin library loading failed.\n\t\t(Expected reason: The plugin library path is invalid)" },
            { ERR_FUNCTIONIMPORT,   "Plugin function lookup failed.\n\t\t(Expected reason: The export function name is invalid)" },
            { ERR_CMDARGUMENT,      "Command Argument missing or invalid\n\t\t(Expected reason: Database path or database load instruction is missing or not defined)" },
            { ERR_VALIDATION,       "Validation failure.\n\t\t(Expected reason: Invalid cmd argument string)" },
            { ERR_NOTIMPLEMENTED,   "Function is not implemented.\n\t\t(Expected reason: Currently not supported feature)" },
            { ERR_NULLPOINTER,      "Function argument is null.\n\t\t(Expected reason: Implementation error/corrupted model data)" },
            { ERR_ARGUMENT,         "Function argument is invalid.\n\t\t(Expected reason: Implementation error)" },
            { ERR_DEBUGASSERT,      "Debug assertion failed." },
            { ERR_ALREADYCOMPLETED, "Done MCS is greater that target MCS count.\n\t\t(Expected reason: Simulation is already done!)"}
    };

    c_foreach(error, errTable)
        return_if (error->ErrCode == errCode, error->Message);

    return defaultMessage;
}

// Awaits an error response
static void AwaitErrorResponse(error_t error)
{
    #if defined(MC_AWAIT_TERMINATION_OK)
    while (true)
    {
        fprintf(stderr, "Error %x, continue execution? [y/n]", error);

        let value = getchar();
        ClearStdintBuffer();
        switch(value)
        {
            case 'y': return;
            case 'Y': return;
            case 'n': exit(error);
            case 'N': exit(error);
            default: continue;
        }
    }
    #endif
}

void ErrorToStdout(int32_t errCode, const char *errFunc, int32_t errLine, const char *errMsg)
{
    fprintf(stderr, ERROR_FORMAT, errCode, errFunc, errLine, ErrorCodeToString(errCode), errMsg);
    fflush(stderr);
    AwaitErrorResponse(errCode);
}

void OnErrorExit(int32_t errCode, const char* errFunc, int32_t errLine, const char* errMsg)
{
    var fileStream = fopen(STDERR_PATH, "w");
    fprintf(fileStream, ERROR_FORMAT, errCode, errFunc, errLine, ErrorCodeToString(errCode), errMsg);
    fclose(fileStream);
    exit(errCode);
}

void OnErrorExitWithMemDump(int32_t errCode, const char* errFunc, int32_t errLine, const char* errMsg, uint8_t* memStart, uint8_t* memEnd)
{
    var fileStream = fopen(STDERR_PATH, "w");
    fprintf(fileStream, ERROR_FORMAT_WDUMP, errCode, errFunc, errLine, ErrorCodeToString(errCode), errMsg);

    let buffer = (Buffer_t) {.Begin = memStart, .End = memEnd};
    WriteBufferHexToStream(fileStream, &buffer, 24);
    fclose(fileStream);
    exit(errCode);
}