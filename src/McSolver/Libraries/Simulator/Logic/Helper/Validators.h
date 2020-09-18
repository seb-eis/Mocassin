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
#include "Libraries/Framework/Errors/McErrors.h"
#include "Libraries/Framework/Basic/BaseTypes.h"

// Empty validation that is always true
static inline error_t ValidateAsTrue(void const *value)
{
    return ERR_OK;
}

// Validates the cmd argument formatting
error_t ValidateCmdKeyArgumentFormat(char const * value);

// Validates that is string is not null or empty
error_t ValidateStringNotNullOrEmpty(char const * value);

// Validates that a string is a valid file path
error_t ValidateIsValidFilePath(char const * value);

// Validates that a string is a valid directory path
error_t ValidateIsDiretoryPath(char const * value);

// Validates the database query string
error_t ValidateDatabaseQueryString(char const* value);

// Validates that the provided string can be parsed to a finite and positive FLP64
error_t ValidateIsPositiveDoubleString(char const* value);