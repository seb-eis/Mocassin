//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	FileIO.c        		    //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Basic file IO functions     //
//////////////////////////////////////////

#include "Framework/Basic/FileIO/FileIO.h"
#include <dirent.h>

cerror_t CalculateFileSize(file_t *restrict fileStream)
{
    int64_t fileSize;
    if (fileStream == NULL || fseek(fileStream, 0L, SEEK_END) != 0)
        return ERR_FILE;

    if ((fileSize = ftell(fileStream)) < 0)
        return ERR_FILE;

    rewind(fileStream);
    return fileSize;
}

bool_t IsAccessibleFile(const char* restrict fileName)
{
    return access(fileName, F_OK) == 0 && !IsAccessibleDirectory(fileName);
}

bool_t IsAccessibleDirectory(const char* restrict dirName)
{
    var dir = opendir(dirName);
    if (dir)
    {
        closedir(dir);
        return true;
    }

    return false;
}

error_t WriteBufferToStream(file_t* restrict fileStream, const Buffer_t* restrict buffer)
{
    return_if(fileStream == NULL, ERR_STREAM);
    let bufferSize = span_Length(*buffer);

    if (fwrite(buffer->Begin, sizeof(byte_t), bufferSize, fileStream) != bufferSize)
        return ERR_STREAM;

    return ERR_OK;
}

error_t WriteBufferToFile(const char* restrict fileName, const char* restrict fileMode, const Buffer_t* restrict buffer)
{
    file_t* fileStream = NULL;
    error_t result;

    if (strcmp(fileMode, "wb") != 0 && strcmp(fileMode, "ab") != 0)
        return ERR_FILEMODE;

    if ((fileStream = fopen(fileName, fileMode)) == NULL)
        return ERR_STREAM;

    result = WriteBufferToStream(fileStream, buffer);
    fclose(fileStream);
    return result;
}

error_t ConcatStrings(const char* lhs, const char* rhs, char** result)
{
    error_t error = 0;
    let bufferSize = strlen(lhs) + strlen(rhs) + 1;
    *result = malloc(bufferSize);

    return_if(*result == NULL, ERR_MEMALLOCATION);

    error |= (strncpy(*result, lhs, bufferSize) != NULL) ? ERR_OK : ERR_BUFFEROVERFLOW;
    error |= (strncat(*result, rhs, bufferSize) != NULL) ? ERR_OK : ERR_BUFFEROVERFLOW;
    return error;
}

error_t SaveWriteBufferToFile(const char* restrict fileName, const char* restrict fileMode, const Buffer_t* restrict buffer)
{
    error_t error = 0;
    char* tmpName = NULL;

    if(IsAccessibleFile(fileName))
    {
        if((error = ConcatStrings(fileName, ".backup", &tmpName)) != ERR_OK)
        {
            free(tmpName);
            return error;
        }
        if((error = rename(fileName, tmpName)) != ERR_OK)
        {
            free(tmpName);
            return error;
        }
    }

    if((error = WriteBufferToFile(fileName, fileMode, buffer)) == ERR_OK)
        if(tmpName != NULL)
            error = remove(tmpName);

    free(tmpName);
    return error;
}

error_t LoadBufferFromStream(file_t* restrict fileStream, Buffer_t* restrict buffer)
{
    return_if(fileStream == NULL, ERR_STREAM);
    fread(buffer->Begin, 1, span_Length(*buffer), fileStream);
    return ERR_OK;
}

error_t LoadBufferFromFile(const char* restrict fileName, Buffer_t* restrict outBuffer)
{
    file_t* fileStream;
    int64_t bufferSize;
    error_t error;

    if ((fileStream = fopen(fileName, "rb")) == NULL || (bufferSize = CalculateFileSize(fileStream)) < 0)
        return ERR_STREAM;

    if (span_Length(*outBuffer) != (size_t)bufferSize)
        *outBuffer = new_Span(*outBuffer, (size_t)bufferSize);

    error = LoadBufferFromStream(fileStream, outBuffer);
    fclose(fileStream);
    return error;
}

error_t WriteBufferHexToStream(file_t* restrict fileStream, const Buffer_t* restrict buffer, size_t bytesPerLine)
{
    return_if (fileStream == NULL, ERR_STREAM);

    size_t lineCount = 0;
    cpp_foreach(it, *buffer)
    {
        fprintf(fileStream, "%02x ", *it);
        if  (++lineCount == bytesPerLine)
        {
            fprintf(fileStream, "\n");
            lineCount = 0;
        }
    }
    return ERR_OK;
}

bool_t EnsureFileIsDeleted(char const * restrict filePath)
{
    if (IsAccessibleFile(filePath))
        return remove(filePath) == 0;

    return false;
}

void ClearStdintBuffer()
{
    int32_t c;
    while ((c = getchar()) != '\n' && c != EOF) {};
}

static error_t SaveAddStringEntryToList(StringList_t*restrict list, char * value, bool_t createCopy)
{
    var insertValue = createCopy ? strdup(value) : value;

    if (!list_IsFull(*list))
    {
        list_PushBack(*list, insertValue);
        return ERR_OK;
    }

    var count = list_Capacity(*list);
    StringList_t newList = new_List(newList, 2 * count);
    memcpy(newList.Begin, list->Begin, count * sizeof(char*));
    newList.End += count;
    list_PushBack(newList, insertValue);
    delete_List(*list);
    *list = newList;
    return ERR_OK;
}

error_t ListAllFilesByPattern(const char* root, const char* pattern, bool_t includeSubdirs, StringList_t*restrict outList)
{
    *outList = new_List(*outList, 10);

    char buffer[260];
    DIR * directory = opendir(root);
    struct dirent* direntry;
    return_if(directory == NULL, ERR_FILE);

    var error = ERR_OK;
    for (;(direntry = readdir(directory)) != NULL;)
    {
        continue_if(strcmp(direntry->d_name, ".") == 0 || strcmp(direntry->d_name, "..") == 0);

        sprintf(buffer, "%s\\%s", root, direntry->d_name);
        if (IsAccessibleFile(buffer) && pattern != NULL && strstr(buffer, pattern) != NULL)
        {
            SaveAddStringEntryToList(outList, buffer, true);
            memset(buffer, 0, sizeof(buffer));
            continue;
        }
        if (includeSubdirs && IsAccessibleDirectory(buffer))
        {
            StringList_t subList;
            error = ListAllFilesByPattern(buffer, pattern, includeSubdirs, &subList);
            cpp_foreach(item, subList) SaveAddStringEntryToList(outList, *item, false);
            delete_List(subList);
        }
        memset(buffer, 0, sizeof(buffer));
    }

    closedir(directory);
    return error;
}