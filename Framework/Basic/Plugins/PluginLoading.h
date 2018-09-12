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

#if defined(MC_USE_PLUGIN_SUPPORT)
    #if defined(_WIN32)
        #include <windows.h>
    #endif
        
    #if defined(linux)
        #include <dlfcn.h>
    #endif

    #if !defined(_DEBUG)
        #define IGNORE_INVALID_PLUGINS
    #endif
#else
    #define IGNORE_INVALID_PLUGINS
#endif

void* ImportFunction(const char* restrict libraryPath, const char* restrict exportName, error_t* restrict error);
