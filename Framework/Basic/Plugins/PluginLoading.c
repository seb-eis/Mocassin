//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	PluginLoading.h    		    //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   General plugin loading      //
//////////////////////////////////////////

#include "Framework/Basic/Plugins/PluginLoading.h"

#if !defined(MC_USE_PLUGIN_SUPPORT)
    void* ImportFunction(const char* restrict libraryPath, const char* restrict exportName, error_t* restrict error)
    {
        return NULL;
    }
#else
    #if defined(_WIN32)
        void LogApiError_WIN32(FILE* restrict stream, const char* restrict message)
        {
            fprintf(stream, "WIN32 API ERROR:\t0x%lx\n API CALL DETAIL:\t%s\n", GetLastError(), message);
        }

        void* ImportFunction(const char* restrict libraryPath, const char* restrict exportName, error_t* restrict error)
        {
            HMODULE module;
            void* function = NULL;

            if ((module = LoadLibrary(libraryPath)) == NULL)
            {
                LogApiError_WIN32(stderr, "Call to 'LoadLibrary(<FILENAME>)' returned NULL. Could not load library.");
                *error = ERR_LIBRARYLOADING;
                return NULL;
            }

            if ((function = GetProcAddress(module, exportName)) == NULL)
            {
                LogApiError_WIN32(stderr, "Call to 'GetProcAddress(<HMODULE>, <FUNCNAME>)' returned NULL. Could not load function.");
                *error = ERR_FUNCTIONIMPORT;
                return NULL;
            }
            return function;
        }
    #endif

    #if defined(linux)
        void LogApiError_LINUX(FILE* restrict stream, const char* restrict message)
        {
            fprintf(stream, "LINUX API ERROR:\t%s\n API CALL DETAIL:\t%s\n", dlerror(), message);
        }

        void* ImportFunction(const char* restrict libraryPath, const char* restrict exportName, error_t* restrict error)
        {
            void* dlHandle, function;
            
            if ((dlHandle = dlopen(libraryPath, RTLD_LAZY)) == NULL)
            {
                LogApiError_LINUX(stderr, "Call to 'dlopen(<FILENAME>, <FLAGS>)' returned NULL. Could not load library.");
                *error = ERR_LIBRARYLOADING;
                return NULL;
            }

            if ((function = dlsym(dlHandle, exportName)) == NULL)
            {
                LogApiError_LINUX(stderr, "Call to 'dlsym(<HANDLE>, <SYMBOL>)' returned NULL. Could not load function.");
                *error = ERR_FUNCTIONIMPORT;
                return NULL;
            }
            return function;
        }
    #endif
#endif