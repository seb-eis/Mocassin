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
#define MC_EXTROUTINE_FILEMASK "mocext.*.dll"
#elif defined(linux)
#define MC_EXTROUTINE_FILEMASK "mocext.*.so"
#endif

#define MOCEXT_IDENTIFICATION_FUNC MocExtRoutine_GetUUID
#define MOCEXT_IDENTIFICATION_FUNCNAME "MocExtRoutine_GetUUID"

#define MOCEXT_ENTRYPOINTGET_FUNC MocExtRoutine_GetEntryPoint
#define MOCEXT_ENTRYPOINTGET_FUNCNAME "MocExtRoutine_GetEntryPoint"

// Type for storage of an int32,int16,int16
// Layout@ggc_x86_64 => 16@[4,2,2,8]
typedef struct moc_uuid
{
    int32_t A;

    int16_t B;

    int16_t C;

    int8_t  D[8];

} moc_uuid_t;

// Defines the type for an extension routine (Routine should do its own error handling)
typedef void (*FMocExtEntry_t)(void* context);
