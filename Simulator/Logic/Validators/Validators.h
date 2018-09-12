//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	Validators.h   		        //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Data/cmd args validators    //
//////////////////////////////////////////

#pragma once
#include "Framework/Errors/McErrors.h"
#include "Framework/Basic/BaseTypes/BaseTypes.h"

static inline error_t ValidateNothing(void const * value)
{
    return ERR_OK;
}

error_t ValidateCmdKeyArgumentFormat(char const * value);

error_t ValidateStringNotNullOrEmpty(char const * value);

error_t ValidateIsValidFilePath(char const * value);

error_t ValidateDatabaseQueryString(char const* value);