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

cerror_t GetFileSize(file_t* restrict fileStream)
{
    int64_t fileSize;
    if (fileStream == NULL || fseek(fileStream, 0L, SEEK_END) != 0)
    {
        return ERR_FILE;
    }
    if ((fileSize = ftell(fileStream)) < 0)
    {
        return ERR_FILE;
    }
    rewind(fileStream);
    return fileSize;
}

bool_t IsAccessibleFile(const char* restrict fileName)
{
    return access(fileName, F_OK) != -1;
}

error_t WriteBufferToStream(file_t* restrict fileStream, const buffer_t* restrict buffer)
{
    if (fileStream == NULL)
    {
        return ERR_STREAM;
    }

    size_t bufferSize = GetBufferSize(buffer);
    if (fwrite(buffer->Start, sizeof(byte_t), bufferSize, fileStream) != bufferSize)
    {
        return ERR_STREAM;
    }
    return ERR_OK;
}

error_t WriteBufferToFile(const char* restrict fileName, const char* restrict fileMode, const buffer_t* restrict buffer)
{
    file_t* fileStream = NULL;
    int32_t result; 

    if (strcmp(fileMode, "wb") != 0 && strcmp(fileMode, "ab") != 0)
    {
        return ERR_FILEMODE;
    }
    if ((fileStream = fopen(fileName, fileMode)) == NULL)
    {
        return ERR_STREAM;
    }
    result = WriteBufferToStream(fileStream, buffer);
    fclose(fileStream);
    return result;
}

error_t ConcatStrings(const char* lhs, const char* rhs, char** result)
{
    error_t error = 0;
    int32_t bufferSize = strlen(lhs) + strlen(rhs) + 1;
    *result = malloc(bufferSize);

    if(*result == NULL)
    {
        return ERR_MEMALLOCATION;
    }

    error |= strcpy_s(*result, bufferSize, lhs);
    error |= strcat_s(*result, bufferSize, rhs);
    return errno;
}

error_t SaveWriteBufferToFile(const char* restrict fileName, const char* restrict fileMode, const buffer_t* restrict buffer)
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
    {
        if(tmpName != NULL)
        {
            error = remove(tmpName);
        }
    }

    free(tmpName);
    return error;
}

error_t LoadBufferFromStream(file_t* restrict fileStream, buffer_t* restrict buffer)
{
    if (fileStream == NULL)
    {
        return ERR_STREAM;
    }
    fread(buffer->Start, 1, GetBufferSize(buffer), fileStream);
    return ERR_OK;
}

error_t LoadBufferFromFile(const char* restrict fileName, buffer_t* restrict outBuffer)
{
    file_t* fileStream;
    int64_t bufferSize;
    int32_t writeResult;

    if ((fileStream = fopen(fileName, "rb")) == NULL || (bufferSize = GetFileSize(fileStream)) < 0)
    {
        return ERR_STREAM;
    }

    if (AllocateBufferChecked(bufferSize, 1, outBuffer) != ERR_OK)
    {     
        fclose(fileStream);
        return ERR_MEMALLOCATION;
    }

    writeResult = LoadBufferFromStream(fileStream, outBuffer);
    fclose(fileStream);
    return writeResult;
}

error_t WriteBufferHexToStream(file_t* restrict fileStream, const buffer_t* restrict buffer, size_t bytesPerLine)
{
    if (fileStream == NULL)
    {
        return ERR_STREAM;
    }

    size_t lineCnt = 0;
    for (byte_t* it = buffer->Start; it < buffer->End; it++)
    {
        fprintf(fileStream, "%02x ", *it);
        if  (++lineCnt == bytesPerLine)
        {
            fprintf(fileStream, "\n");
            lineCnt = 0;
        }
    }
    return ERR_OK;
}

error_t WriteBlockHexToStream(file_t* restrict fileStream, const memblock_array_t* restrict blockArray, size_t blocksPerLine)
{
    if (fileStream == NULL)
    {
        return ERR_STREAM;
    }

    size_t lineCnt = 0;
    for (memblock_t* it = blockArray->Start; it < blockArray->End; it++)
    {
        fprintf(fileStream, "%08x ", *it);
        if  (++lineCnt == blocksPerLine)
        {
            fprintf(fileStream, "\n");
            lineCnt = 0;
        }
    }
    return ERR_OK;
}

bool_t EnsureFileIsDeleted(char const * restrict filePath)
{
    if (IsAccessibleFile(filePath))
    {
        return remove(filePath) == 0;
    }
    return false;
}