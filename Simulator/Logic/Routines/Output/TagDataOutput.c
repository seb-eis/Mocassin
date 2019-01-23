
//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	TagDataOutput.h       		//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Default runtime output      //
//////////////////////////////////////////

#include "Simulator/Logic/Routines/Output/TagDataOutput.h"

// Prints the passed value and tag to the passed stream and returns a stream error if failed
static error_t TryPrintWithTag(FILE*restrict fstream, const char* restrict tag, const char*restrict value)
{
    error_t error;
    error = fprintf(fstream, "[%-12s]\t", tag);
    return_if(error < 0, ERR_STREAM);
    error = fprintf(fstream, "%s\n", value);
    return_if(error < 0, ERR_STREAM);
    return ERR_OK;
}

error_t InvokeContextTagOutput(FILE *restrict fstream, const ContextTagOutputList_t *restrict callList, __SCONTEXT_PAR)
{
    return_if((fstream == NULL) || (SCONTEXT == NULL), ERR_NULLPOINTER);
    cpp_foreach(item, *callList)
    {
        char* str;
        error_t error = item->Getter(SCONTEXT, &str);
        return_if(error, error);
        error = TryPrintWithTag(fstream, item->Tag, str);
        free(str);
    }

    return fprintf(fstream, "\n") >= 0 ? ERR_OK : ERR_STREAM;
}