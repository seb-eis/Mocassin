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
        "FATAL FAILURE. Runtime error exit triggered without an error flag. (Expected reason: Implementation error)",
        "File stream operation failed                                       (Expected reason: Storage system is full)",
        "Memory block dump operation failed                                 (Expected reason: Implementation error)",
        "Incompatible buffer cast operation detected                        (Expected reason: Implementation error)",
        "Buffer overflow detected                                           (Expected reason: Implementation error)",
        "File path is missing or invalid                                    (Expected reason: File is missing or has wrong name)",
        "Invalid file mode passed to I/O                                    (Expected reason: Implementation error)",
        "Database access failure                                            (Expected reason: Corrupted or invalid database)",
        "Unspecified error                                                  (Expected reason: Implementation error)",
        "Dynamic memory allocation returned NULL                            (Expected reason: Process is out of memory)",
        "Data is inconsistent                                               (Expected reason: Corrupted or invalid input data)",
        "Invalid file/state/database checksum                               (Expected reason: Unsupported manual file manipulation)",
        "Plugin loading failed                                              (Expected reason: The plugin library path is invalid)",
        "Plugin function lookup failed                                      (Expected reason: The export function name is invalid)"
    };

    return (errCode > (sizeof(errTable) / sizeof(char*))) ? "[???]" : errTable[errCode];
}

void OnErrorExit(int32_t errCode, const char* errFile, int32_t errLine, const char* errMsg)
{
    FILE* fileStream = fopen(STDERR_PATH, "w");
    fprintf(fileStream, "MC Runtime Error: 0x%08x\n File: %s\n Line: %d\n Type: %s\n Info: %s", errCode, errFile, errLine, ConvErrorToString(errCode), errMsg);
    fclose(fileStream);
    exit(errCode);
}

void OnErrorExitWithMemDump(int32_t errCode, const char* errFile, int32_t errLine, const char* errMsg, uint8_t* memStart, uint8_t* memEnd)
{
    FILE* fileStream = fopen(STDERR_PATH, "w");
    fprintf(fileStream, "MC Runtime Error: 0x%08x\n File: %s\n Line: %d\n Type: %s\n Info: %s\n Buffer:\n\n", errCode, errFile, errLine, ConvErrorToString(errCode), errMsg);

    buffer_t buffer = {memStart, memEnd};
    WriteBufferHexToStream(fileStream, &buffer, 24);
    fclose(fileStream);
    exit(errCode);
}