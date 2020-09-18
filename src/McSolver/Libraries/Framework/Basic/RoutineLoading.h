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

#include "Extensions/MocassinSolverExtension.h"
#include "DlLoading.h"
#include "Macros.h"
#include "Libraries/Framework/Basic/Buffers.h"

#if defined(_WIN32)
#define MOCASSIN_EXTENSION_FILEENDING ".mocext.dll"
#elif defined(linux)
#define MOCASSIN_EXTENSION_FILEENDING ".mocext.so"
#endif

// Tries to find an extension library in the provided search path with the given routine UUID and returns the entry point function pointer
FMocassinRoutine_t TryFindMocassinExtensionRoutine(const MocsimUuid_t* routineUuid, const char* searchPath);

// Tries to load the provided path as an extension library with the given routine UUID and returns the entry point function pointer if successful
FMocassinRoutine_t TryLoadMocassinExtensionRoutine(const MocsimUuid_t* routineUuid, const char* libraryPath);