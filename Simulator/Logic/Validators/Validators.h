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

static inline error_t ValidateAlwaysTrue(void const * value)
{
    return 0;
}

error_t ValidateCmdKeyArgumentFormat(char const * value);

error_t ValidateStringNotNullOrEmpty(char const * value);

error_t ValidateIsValidFilePath(char const * value);