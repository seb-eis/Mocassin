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
    static char* errTable[] =
    {
        "FATAL FAILURE. Runtime error exit triggered without an error flag.",
        "File stream operation failed",
        "Memory block dump operation failed",
        "Incompatible buffer cast operation (Potential overflow)",
        "Buffer overflow",
        "File path is missing or invalid",
        "Invalid file mode passed function",
        "Database access failure",
        "Unspecified error",
        "Memory allocation error",
        "Data is inconsistent",
        "Invalid checksum"
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