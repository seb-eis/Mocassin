//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	Errors.h         		    //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Error codes + messages      //
//////////////////////////////////////////

#include "McErrors.h"
#include "Framework/Basic/FileIO/FileIO.h"

char* ConvErrorToString(error_t errCode)
{
    // Redirect all non-critical errors to FATAL FAILURE since they should never cause an error string lookup
    errCode = (errCode <= 0) ? 0 : errCode;

    static char* errTable[] =
    {
        "FATAL FAILURE. Runtime error exit triggered without an error flag.\n\t(Expected reason: Implementation error)",
        "File stream operation failed.\n\t(Expected reason: Storage system is full)",
        "Memory block dump operation failed.\n\t(Expected reason: Implementation error)",
        "Incompatible buffer cast operation detected.\n\t(Expected reason: Implementation error)",
        "Buffer overflow detected.\n\t(Expected reason: Implementation error)",
        "File path is missing or invalid.\n\t(Expected reason: File is missing or has wrong name)",
        "Invalid file mode passed to I/O.\n\t(Expected reason: Implementation error)",
        "Database access failure.\n\t(Expected reason: Corrupted or invalid database)",
        "Unspecified error.\n\t(Expected reason: Implementation error)",
        "Dynamic memory allocation returned NULL.\n\t(Expected reason: Process is out of memory)",
        "Data is inconsistent.\n\t(Expected reason: Corrupted or invalid input data)",
        "Invalid file/state/database checksum.\n\t(Expected reason: Unsupported manual file manipulation)",
        "Plugin loading failed.\n\t(Expected reason: The plugin library path is invalid)",
        "Plugin function lookup failed.\n\t(Expected reason: The export function name is invalid)",
        "Argument missing or invalid\n\t(Expected reason: Database filepath or database load instruction is missing or not defined)",
        "Validation failure.\n\t(Expected reason: Invalid cmd argument string)",
        "Function is not implemented.\n\t(Expected reason: Currently not supported feature)",
        "Function argument is null.\n\t(Expected reason: Implementation error/Corrupted model data)"
    };

    return (errCode > (sizeof(errTable) / sizeof(char*))) ? "[???]" : errTable[errCode];
}

void DisplayErrorAndAwait(int32_t errCode, const char* errFunc, int32_t errLine, const char* errMsg)
{
    fprintf(stdout, ERROR_FORMAT, errCode, errFunc, errLine, ConvErrorToString(errCode), errMsg);
    fprintf(stdout, "\nAwait...\n");
}

void OnErrorExit(int32_t errCode, const char* errFunc, int32_t errLine, const char* errMsg)
{
    FILE* fileStream = fopen(STDERR_PATH, "w");
    fprintf(fileStream, ERROR_FORMAT, errCode, errFunc, errLine, ConvErrorToString(errCode), errMsg);
    fclose(fileStream);
    exit(errCode);
}

void OnErrorExitWithMemDump(int32_t errCode, const char* errFunc, int32_t errLine, const char* errMsg, uint8_t* memStart, uint8_t* memEnd)
{
    FILE* fileStream = fopen(STDERR_PATH, "w");
    fprintf(fileStream, ERROR_FORMAT_WDUMP, errCode, errFunc, errLine, ConvErrorToString(errCode), errMsg);

    Buffer_t buffer = {memStart, memEnd};
    WriteBufferHexToStream(fileStream, &buffer, 24);
    fclose(fileStream);
    exit(errCode);
}