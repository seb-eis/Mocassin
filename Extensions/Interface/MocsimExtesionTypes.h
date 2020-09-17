//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	MocsimRoutineExtension.h   	//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Extension type header       //
//////////////////////////////////////////

#pragma once
#include <stdint.h>

#if defined(_WIN32)
#define MOCEXTENSION_LIBRARY_FILEMASK "mocext.*.dll"
#elif defined(linux)
#define MOCEXTENSION_LIBRARY_FILEMASK "mocext.*.so"
#endif

#define MOCEXTENSION_GET_UUID_FUNC MocassinExtensionGetRoutineIdentification
#define MOCEXTENSION_GET_UUID_FUNC_NAME "MocassinExtensionGetRoutineIdentification"

#define MOCEXTENSION_GET_ROUTINE_FUNC MocassinExtensionGetRoutineEntry
#define MOCEXTENSION_GET_ROUTINE_FUNC_NAME "MocassinExtensionGetRoutineEntry"

// Type for storage of an int32,int16,int16
// Layout@ggc_x86_64 => 16@[4,2,2,8]
typedef struct mocuuid
{
    uint32_t A;

    uint16_t B;

    uint16_t C;

    uint8_t  D[8];

} mocuuid_t;

// Defines the type for a mocassin routine function (Routine should do its own error handling)
typedef void (*FMocassinRoutine_t)(void* context);
