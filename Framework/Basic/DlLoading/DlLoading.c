//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	PluginLoading.h    		    //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   General plugin loading      //
//////////////////////////////////////////

#include "Framework/Basic/DlLoading/DlLoading.h"

#if !defined(MC_USE_PLUGIN_SUPPORT)
    void* ImportFunction(const char* restrict libraryPath, const char* restrict exportName, error_t* restrict error)
    {
        return NULL;
    }
#else
    #if defined(_WIN32)
        static void LogOsApiError(FILE* restrict stream, const char* restrict message)
        {
            fprintf(stream, "WIN32 API ERROR:\t0x%lx\n API CALL DETAIL:\t%s\n", GetLastError(), message);
            fflush(stream);
        }

        void* ImportFunction(const char* restrict libraryPath, const char* restrict exportName, error_t* restrict error)
        {
            HMODULE module;
            void* function = NULL;

            if ((module = GetLibraryHandle(libraryPath)) == NULL)
            {
                LogOsApiError(stderr, "Call to 'LoadLibrary(<FILENAME>)' returned NULL. Could not load library.");
                *error = ERR_LIBRARYLOADING;
                return NULL;
            }

            if ((function = GetProcAddress(module, exportName)) == NULL)
            {
                LogOsApiError(stderr, "Call to 'GetProcAddress(<HMODULE>, <FUNCNAME>)' returned NULL. Could not load function.");
                *error = ERR_FUNCTIONIMPORT;
                return NULL;
            }
            return function;
        }

        bool_t UnloadDynamicLibrary(const char* restrict libraryName)
        {
            HMODULE module = GetModuleHandleA(libraryName);
            return_if(module == NULL, false);
            return (bool_t) FreeLibrary(module);
        }

        LIBHANDLE GetLibraryHandle(const char *restrict libraryPath)
        {
            HMODULE module;
            if ((module = GetModuleHandleA(libraryPath)) == NULL)
                module = LoadLibraryA(libraryPath);

            return module;
        }

    #endif

    #if defined(linux)
        static void LogOsApiError(FILE* restrict stream, const char* restrict message)
        {
            fprintf(stream, "LINUX API ERROR:\t%s\n API CALL DETAIL:\t%s\n", dlerror(), message);
            fflush(stream);
        }

        void* ImportFunction(const char* restrict libraryPath, const char* restrict exportName, error_t* restrict error)
        {
            void* dlHandle, function;
            
            if ((dlHandle = GetLibraryHandle(libraryPath)) == NULL)
            {
                LogOsApiError(stderr, "Call to 'dlopen(<FILENAME>, <FLAGS>)' returned NULL. Could not load library.");
                *error = ERR_LIBRARYLOADING;
                return NULL;
            }

            if ((function = dlsym(dlHandle, exportName)) == NULL)
            {
                LogOsApiError(stderr, "Call to 'dlsym(<HANDLE>, <SYMBOL>)' returned NULL. Could not load function.");
                *error = ERR_FUNCTIONIMPORT;
                return NULL;
            }
            return function;
        }

        LIBHANDLE GetLibraryHandle(const char *restrict libraryPath)
        {
            void* dlHandle;
            if ((dlHandle) = dlopen(libraryPath, RTLD_NOLOAD) == NULL)
                dlHandle =dlopen(libraryPath, RTLD_LAZY);

            return dlHandle;
        }

        bool_t UnloadDynamicLibrary(const char* restrict libraryName)
        {
            dlclose()
        }
    #endif
#endif