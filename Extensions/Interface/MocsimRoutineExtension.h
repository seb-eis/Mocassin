//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	MocsimRoutineExtension.h   	//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Extension .dll/.so header   //
//////////////////////////////////////////

#pragma once
#include <stdint.h>

// Defines the type for an extension routine (Routine should do its own error handling)
typedef void (*FMocExtEntry_t)(void* context);

// Get the pointer to the start of the 16 byte routine identification guid/uuid string
const char* MocExtRoutine_GetUUID();

// Get the pointer to the routine execution entry function that will accept an initialized simulation context
FMocExtEntry_t MocExtRoutine_GetEntryPoint();

