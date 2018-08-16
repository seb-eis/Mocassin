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

void OnErrorExit(int32_t errCode, const char* errFile, int32_t errLine, const char* errMsg)
{
    FILE* fileStream = fopen(STDERR_PATH, "w");
    fprintf(fileStream, "MC Runtime Error: 0x%08x\n File: %s\n Line: %d\n Info: %s", errCode, errFile, errLine, errMsg);
    fclose(fileStream);
    exit(errCode);
}

void OnErrorExitWithMemDump(int32_t errCode, const char* errFile, int32_t errLine, const char* errMsg, uint8_t* memStart, uint8_t* memEnd)
{
    FILE* fileStream = fopen(STDERR_PATH, "w");
    fprintf(fileStream, "MC Runtime Error: 0x%08x\n File: %s\n Line: %d\n Info: %s\n Buffer:\n\n", errCode, errFile, errLine, errMsg);

    buffer_t buffer = {memStart, memEnd};
    WriteBufferHexToStream(fileStream, &buffer, 24);
    fclose(fileStream);
    exit(errCode);
}