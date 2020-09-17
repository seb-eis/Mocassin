//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	RoutineLoading.c   	        //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Routine library loader      //
//////////////////////////////////////////


#include "RoutineLoading.h"
#include "FileIO.h"

FMocassinRoutine_t TryFindMocassinExtensionRoutine(const MocsimUuid_t* routineUuid, const char* searchPath)
{
    MocsimUuid_t nullUuid = {.A = 0, .C = 0, .B = 0, .D = {0, 0, 0, 0, 0, 0, 0, 0}};
    return_if(CompareMocuuid(&nullUuid, routineUuid) == 0, NULL);
    return_if(searchPath == NULL || !IsAccessibleDirectory(searchPath), NULL);

    StringList_t libList;
    FMocassinRoutine_t entryFunc = NULL;
    var error = ListAllFilesByPattern(searchPath, MOCASSIN_EXTENSION_FILEENDING, true, &libList);
    if (error == ERR_OK)
    {
        cpp_foreach(item, libList)
        {
            entryFunc = TryLoadMocassinExtensionRoutine(routineUuid, *item);
            break_if(entryFunc != NULL);
        }
    }

    cpp_foreach(item, libList) free(*item);
    list_Delete(libList);
    return entryFunc;
}

FMocassinRoutine_t TryLoadMocassinExtensionRoutine(const MocsimUuid_t* routineUuid, const char* libraryPath)
{
    error_t error = ERR_OK;
    MocsimUuid_t* (*uuidGetter)(void) = LibraryLoadingImportFunction(libraryPath, MOCASSIN_EXTENSION_GET_ENTRY_NAME, &error);
    return_if(uuidGetter == NULL || error != ERR_OK, (LibraryLoadingUnloadLibrary(libraryPath), NULL));

    let libUuid = uuidGetter();
    return_if(CompareMocuuid(routineUuid, libUuid) != 0 || error != ERR_OK, (LibraryLoadingUnloadLibrary(libraryPath), NULL));

    FMocassinRoutine_t (*entryGetter)(void) = LibraryLoadingImportFunction(libraryPath, MOCASSIN_EXTENSION_GET_UUID_NAME,
                                                                           &error);
    return_if(entryGetter == NULL || error != ERR_OK, (LibraryLoadingUnloadLibrary(libraryPath), NULL));

    return entryGetter();
}