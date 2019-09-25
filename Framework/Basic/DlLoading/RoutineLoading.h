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
#include "Framework/Basic/BaseTypes/Buffers.h"

#if defined(_WIN32)
#define MOCEXT_EXTROUTINE_LIBNAME ".mocext.dll"
#elif defined(linux)
#define MOCEXT_EXTROUTINE_LIBNAME ".mocext.so"
#endif

// Tries to find an extension library in the provided search path with the given routine UUID and returns the entry point function pointer
FMocExtEntry_t MocExt_TryFindExtensionRoutine(const moc_uuid_t* routineUuid, const char* searchPath);

// Tries to load the provided path as an extension library with the given routine UUID and returns the entry point function pointer if successful
FMocExtEntry_t MocExt_TryLoadExtensionRoutine(const moc_uuid_t* routineUuid, const char* libraryPath);