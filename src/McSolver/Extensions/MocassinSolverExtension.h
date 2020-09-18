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

// The automated extension loading system searches for

#define MOCASSIN_EXTENSION_GET_ENTRY_FUNC MocassinGetExtensionRoutineIdentification
#define MOCASSIN_EXTENSION_GET_ENTRY_NAME "MocassinGetExtensionRoutineIdentification"

#define MOCASSIN_EXTENSION_GET_UUID_FUNC MocassinGetExtensionRoutineEntryFunction
#define MOCASSIN_EXTENSION_GET_UUID_NAME "MocassinGetExtensionRoutineEntryFunction"

// Type for storage of an int32,int16,int16
// Layout@ggc_x86_64 => 16@[4,2,2,8]
typedef struct MocsimUuid
{
    uint32_t A;

    uint16_t B;

    uint16_t C;

    uint8_t  D[8];

} MocsimUuid_t;

// Defines the type for a mocassin routine function (Routine should do its own error handling)
typedef void (*FMocassinRoutine_t)(void* context);

// Get the pointer to the start of the 16 byte routine identification guid/uuid
const MocsimUuid_t* MOCASSIN_EXTENSION_GET_ENTRY_FUNC();

// Get the pointer to the routine execution entry function that will accept an initialized simulation context
FMocassinRoutine_t MOCASSIN_EXTENSION_GET_UUID_FUNC();

