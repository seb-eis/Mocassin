
//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	DefaultOutput.h       		//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Default runtime output      //
//////////////////////////////////////////

#pragma once

#include "Framework/Errors/McErrors.h"
#include "Framework/Basic/FileIO/FileIO.h"
#include "Framework/Basic/Time/McTime.h"
#include "Simulator/Data/SimContext/ContextAccess.h"

// Defines the default getter function for context out calls
typedef error_t (*FContextOutputGetter_t) (__SCONTEXT_PAR, char** outString);

// Defines the type for simulation tag based simulation data outputs that carries a string getter and affiliated tag
typedef struct {FContextOutputGetter_t Getter; const char* Tag;} ContextTagOutput_t;

// Defines the span type for the set of output calls with affiliated tags
typedef List_t(ContextTagOutput_t, ContextTagOutputList) ContextTagOutputList_t;

// Calls the passed output list content with the passed simulation context to write data to the passed stream
error_t InvokeContextTagOutput(FILE *restrict fstream, const ContextTagOutputList_t *restrict callList, __SCONTEXT_PAR);