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
#include "Framework/Basic/FileIO/FileIO.h"

#if !defined(MC_USE_PLUGIN_SUPPORT)
    void* LibraryLoadingImportFunction(const char* restrict libraryPath, const char* restrict exportName, error_t* restrict error)
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

        void* LibraryLoadingImportFunction(const char* restrict libraryPath, const char* restrict exportName, error_t* restrict error)
        {
            HMODULE module;
            void* function = NULL;

            if ((module = LibraryLoadingGetLibraryHandle(libraryPath)) == NULL)
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

        static wchar_t * GetUnicodeLibraryName(const char* restrict libraryName)
        {
            wchar_t * libraryName16;
            var error = Win32ConvertUtf8ToUtf16(libraryName, &libraryName16);
            return_if(error <= 0, NULL);
            return libraryName16;
        }

        bool_t LibraryLoadingUnloadLibrary(const char* restrict libraryName)
        {
            var libraryName16 = GetUnicodeLibraryName(libraryName);

            HMODULE module = GetModuleHandleW(libraryName16);
            free(libraryName16);

            return_if(module == NULL, false);
            return (bool_t) FreeLibrary(module);
        }

        LIBHANDLE LibraryLoadingGetLibraryHandle(const char *restrict libraryPath)
        {
            var libraryName16 = GetUnicodeLibraryName(libraryPath);
            HMODULE module;
            if ((module = GetModuleHandleW(libraryName16)) == NULL)
                module = LoadLibraryW(libraryName16);
            free(libraryName16);
            return module;
        }

    #endif
    #if defined(linux) || defined(__INTEL_COMPILER)
        static void LogOsApiError(FILE* restrict stream, const char* restrict message)
        {
            fprintf(stream, "LINUX API ERROR:\t%s\n API CALL DETAIL:\t%s\n", dlerror(), message);
            fflush(stream);
        }

        void* LibraryLoadingImportFunction(const char* restrict libraryPath, const char* restrict exportName, error_t* restrict error)
        {
            void* dlHandle, *function;
            
            if ((dlHandle = LibraryLoadingGetLibraryHandle(libraryPath)) == NULL)
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

        LIBHANDLE LibraryLoadingGetLibraryHandle(const char *restrict libraryPath)
        {
            void* dlHandle;
            if ((dlHandle = dlopen(libraryPath, RTLD_NOLOAD)) == NULL)
                dlHandle = dlopen(libraryPath, RTLD_LAZY);

            return dlHandle;
        }

        bool_t LibraryLoadingUnloadLibrary(const char* restrict libraryName)
        {
	        void* dlHandle = LibraryLoadingGetLibraryHandle(libraryName);
            return_if(dlHandle == NULL, false);
            return (bool_t) dlclose(dlHandle);
        }
    #endif
#endif