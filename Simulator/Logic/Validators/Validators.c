//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	Validators.c   		        //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Data/cmd args validators    //
//////////////////////////////////////////

#include <strings.h>
#include "Framework/Basic/FileIO/FileIO.h"
#include "Simulator/Logic/Validators/Validators.h"

error_t ValidateCmdKeyArgumentFormat(char const * value)
{
    if (ValidateStringNotNullOrEmpty(value) != ERR_OK)
    {
        return ERR_VALIDATION;
    }
    return (value[0] == '-') ? ERR_OK : ERR_VALIDATION;
}

error_t ValidateStringNotNullOrEmpty(char const * value)
{
    if (value == NULL)
    {
        return ERR_VALIDATION;
    }
    if (strcmp(value, "") == 0)
    {
        return ERR_VALIDATION;
    }
    return ERR_OK;
}

error_t ValidateIsValidFilePath(char const * value)
{
    if (ValidateStringNotNullOrEmpty(value) != ERR_OK)
    {
        return ERR_VALIDATION;
    }
    if (!IsAccessibleFile(value))
    {
        return ERR_VALIDATION;
    }
    return ERR_OK;
}

// Validates the database string format to be #JobId
error_t ValidateDatabaseQueryString(char const* value)
{
    if (ValidateStringNotNullOrEmpty(value) != ERR_OK)
    {
        return ERR_VALIDATION;
    }

    int32_t jobId;

    if (sscanf(value, "%i", &jobId) != 1)
    {
        return ERR_VALIDATION;
    }

    if (jobId >= 0)
    {
        return ERR_OK;
    }

    return ERR_VALIDATION;
}
