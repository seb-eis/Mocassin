//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	RoutineLoading.c   	        //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Routine library loader      //
//////////////////////////////////////////


#include "Framework/Basic/DlLoading/RoutineLoading.h"
#include "Framework/Basic/FileIO/FileIO.h"

FMocExtEntry_t MocExt_TryFindExtensionRoutine(const moc_uuid_t* routineUuid, const char* searchPath)
{
    return_if(searchPath == NULL || !IsAccessibleDirectory(searchPath), NULL);
    StringList_t libList;
    FMocExtEntry_t entryFunc = NULL;

    if (ListAllFilesByPattern(searchPath, MOCEXT_EXTROUTINE_LIBNAME, true, &libList) == ERR_OK)
    {
        cpp_foreach(item, libList)
        {
            entryFunc = MocExt_TryLoadExtensionRoutine(routineUuid, *item);
            break_if(entryFunc != NULL);
        }
    }

    cpp_foreach(item, libList) free(*item);
    list_Delete(libList);
    return entryFunc;
}

FMocExtEntry_t MocExt_TryLoadExtensionRoutine(const moc_uuid_t* routineUuid, const char* libraryPath)
{
    error_t error = ERR_OK;
    moc_uuid_t* (*uuidGetter)(void) = DlLoading_ImportFunction(libraryPath, MOCEXT_IDENTIFICATION_FUNCNAME, &error);
    return_if(uuidGetter == NULL || error != ERR_OK, (DlLoading_UnloadDynamicLibrary(libraryPath), NULL));

    let libUuid = uuidGetter();
    return_if(CompareUUID(routineUuid, libUuid) != 0 || error != ERR_OK, (DlLoading_UnloadDynamicLibrary(libraryPath), NULL));

    FMocExtEntry_t (*entryGetter)(void) = DlLoading_ImportFunction(libraryPath, MOCEXT_ENTRYPOINTGET_FUNCNAME, &error);
    return_if(entryGetter == NULL || error != ERR_OK, (DlLoading_UnloadDynamicLibrary(libraryPath), NULL));

    return entryGetter();
}