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
#include <errno.h>
#include "Libraries/Framework/Basic/FileIO/FileIO.h"
#include "Validators.h"

error_t ValidateCmdKeyArgumentFormat(char const * value)
{
    return_if (ValidateStringNotNullOrEmpty(value) != ERR_OK, ERR_VALIDATION);
    return (value[0] == '-') ? ERR_OK : ERR_VALIDATION;
}

error_t ValidateStringNotNullOrEmpty(char const * value)
{
    return_if(value == NULL, ERR_VALIDATION);
    return_if(strcmp(value, "") == 0, ERR_VALIDATION);
    return ERR_OK;
}

error_t ValidateIsValidFilePath(char const * value)
{
    return_if(ValidateStringNotNullOrEmpty(value) != ERR_OK, ERR_VALIDATION);
    return_if (!IsAccessibleFile(value), ERR_VALIDATION);
    return ERR_OK;
}

error_t ValidateIsDiretoryPath(char const * value)
{
    return_if(ValidateStringNotNullOrEmpty(value) != ERR_OK, ERR_VALIDATION);
    return_if (!IsAccessibleDirectory(value), ERR_VALIDATION);
    return ERR_OK;
}

// Validates the database string format to be #JobId
error_t ValidateDatabaseQueryString(char const* value)
{
    return_if (ValidateStringNotNullOrEmpty(value) != ERR_OK, ERR_VALIDATION);

    int32_t jobId;
    return_if (sscanf(value, "%i", &jobId) != 1, ERR_VALIDATION);
    return  (jobId >= 0) ? ERR_OK : ERR_VALIDATION;
}

error_t ValidateIsPositiveDoubleString(char const* value)
{
    var flpValue = strtod(value, NULL);
    if (errno == ERANGE || flpValue <= 0.0) return ERR_VALIDATION;
    return ERR_OK;
}
