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

    static char defaultMessage[] = "No detail on error code available";
    static struct { int32_t ErrCode; char* Message; } errTable[] =
    {
            { ERR_OK,               "FATAL FAILURE. Runtime error exit triggered without an error flag.\n\t(Expected reason: Implementation error)" },
            { ERR_STREAM,           "File stream operation failed.\n\t(Expected reason: Storage system is full)" },
            { ERR_BLOCKDUMP,        "Memory block dump operation failed.\n\t(Expected reason: Implementation error)" },
            { ERR_BUFFERCAST,       "Incompatible buffer cast operation detected.\n\t(Expected reason: Implementation error)" },
            { ERR_BUFFEROVERFLOW,   "Buffer overflow detected.\n\t(Expected reason: Implementation error)" },
            { ERR_FILE,             "File path is missing or invalid.\n\t(Expected reason: File is missing or has wrong name)" },
            { ERR_FILEMODE,         "Invalid file mode passed to I/O.\n\t(Expected reason: Implementation error)" },
            { ERR_DATABASE,         "Database access failure.\n\t(Expected reason: Corrupted or invalid database)" },
            { ERR_UNKNOWN,          "Unspecified error.\n\t(Expected reason: Implementation error)" },
            { ERR_MEMALLOCATION,    "Dynamic memory allocation returned NULL.\n\t(Expected reason: Process is out of memory)" },
            { ERR_DATACONSISTENCY,  "Data is inconsistent.\n\t(Expected reason: Corrupted or invalid input data)" },
            { ERR_HASHPROTECTION,   "Invalid file/state/database checksum.\n\t(Expected reason: Unsupported manual file manipulation)" },
            { ERR_LIBRARYLOADING,   "Plugin library loading failed.\n\t(Expected reason: The plugin library path is invalid)" },
            { ERR_FUNCTIONIMPORT,   "Plugin function lookup failed.\n\t(Expected reason: The export function name is invalid)" },
            { ERR_CMDARGUMENT,      "Command Argument missing or invalid\n\t(Expected reason: Database path or database load instruction is missing or not defined)" },
            { ERR_VALIDATION,       "Validation failure.\n\t(Expected reason: Invalid cmd argument string)" },
            { ERR_NOTIMPLEMENTED,   "Function is not implemented.\n\t(Expected reason: Currently not supported feature)" },
            { ERR_NULLPOINTER,      "Function argument is null.\n\t(Expected reason: Implementation error/corrupted model data)" },
            { ERR_ARGUMENT,         "Function argument is invalid.\n\t(Expected reason: Implementation error)" },
            { ERR_DEBUGASSERT,      "Debug assertion failed." }
    };

    c_foreach(error, errTable)
    {
        if (error->ErrCode == errCode)
        {
            return error->Message;
        }
    }

    return defaultMessage;
}

// Awaits an error response
static void AwaitErrorResponse(error_t error)
{
    while (true)
    {
        fprintf(stdout, "Error %x, continue execution ? [y/n]", error);

        int value = getchar();
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
}

void ErrorToStdout(int32_t errCode, const char *errFunc, int32_t errLine, const char *errMsg)
{
    fprintf(stdout, ERROR_FORMAT, errCode, errFunc, errLine, ErrorCodeToString(errCode), errMsg);
    fflush(stdout);
    AwaitErrorResponse(errCode);
    return;
}

void OnErrorExit(int32_t errCode, const char* errFunc, int32_t errLine, const char* errMsg)
{
    FILE* fileStream = fopen(STDERR_PATH, "w");
    fprintf(fileStream, ERROR_FORMAT, errCode, errFunc, errLine, ErrorCodeToString(errCode), errMsg);
    fclose(fileStream);
    exit(errCode);
}

void OnErrorExitWithMemDump(int32_t errCode, const char* errFunc, int32_t errLine, const char* errMsg, uint8_t* memStart, uint8_t* memEnd)
{
    FILE* fileStream = fopen(STDERR_PATH, "w");
    fprintf(fileStream, ERROR_FORMAT_WDUMP, errCode, errFunc, errLine, ErrorCodeToString(errCode), errMsg);

    Buffer_t buffer = {memStart, memEnd};
    WriteBufferHexToStream(fileStream, &buffer, 24);
    fclose(fileStream);
    exit(errCode);
}