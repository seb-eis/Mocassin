//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	RoutineLoading.h   	        //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Routine library loader      //
//////////////////////////////////////////

#pragma once

#include "Extensions/Interface/MocsimExtesionTypes.h"
#include "Framework/Basic/DlLoading/DlLoading.h"
#include "Framework/Basic/Macros/Macros.h"
#include "Framework/Basic/Buffers/Buffers.h"

#if defined(_WIN32)
#define MOCEXT_EXTROUTINE_LIBNAME ".mocext.dll"
#elif defined(linux)
#define MOCEXT_EXTROUTINE_LIBNAME ".mocext.so"
#endif

// Tries to find an extension library in the provided search path with the given routine UUID and returns the entry point function pointer
FMocassinRoutine_t TryFindMocassinExtensionRoutine(const mocuuid_t* routineUuid, const char* searchPath);

// Tries to load the provided path as an extension library with the given routine UUID and returns the entry point function pointer if successful
FMocassinRoutine_t TryLoadMocassinExtensionRoutine(const mocuuid_t* routineUuid, const char* libraryPath);