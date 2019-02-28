//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	PluginLoading.h    		    //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   General plugin loading      //
//////////////////////////////////////////

#pragma once
#include "Framework/Basic/BaseTypes/BaseTypes.h"
#include "Framework/Errors/McErrors.h"

#define MC_USE_PLUGIN_SUPPORT

#if defined(MC_USE_PLUGIN_SUPPORT)
    #if defined(_WIN32)
        #include <windows.h>
        #define LIBHANDLE HMODULE
    #endif
        
    #if defined(linux)
        #include <dlfcn.h>
        #define LIBHANDLE void*
    #endif

    #if defined(__INTEL_COMPILER)
        #include <dlfcn.h>
        #define LIBHANDLE void*
    #endif

    #if !defined(_DEBUG)
        #define IGNORE_INVALID_PLUGINS
    #endif
#else
    #define IGNORE_INVALID_PLUGINS
#endif

// Wrapper to load or get a dynamic library handle (Linux, Win32) that ensures that no more than one reference count exists
LIBHANDLE GetLibraryHandle(const char *restrict libraryPath);

// Wrapper for the import of a function from a C library with the provided file and export name (Linux, Win32)
void* ImportFunction(const char* restrict libraryPath, const char* restrict exportName, error_t* restrict error);

// Wrapper function to free a previously loaded library (Linux, Win32). Returns true on successful unloading
bool_t UnloadDynamicLibrary(const char* restrict libraryName);
