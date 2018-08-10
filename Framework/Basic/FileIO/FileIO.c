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
        return MC_FILE_ERROR;
    }
    if ((fileSize = ftell(fileStream)) < 0)
    {
        return MC_FILE_ERROR;
    }
    rewind(fileStream);
    return fileSize;
}

bool_t CheckFileExistance(const char* restrict fileName)
{
    return access(fileName, F_OK) != -1;
}

error_t WriteBufferToStream(file_t* restrict fileStream, const buffer_t* restrict buffer)
{
    if (fileStream == NULL)
    {
        return MC_STREAM_ERROR;
    }

    size_t bufferSize = GetBufferSize(buffer);
    if (fwrite(buffer->Start, sizeof(byte_t), bufferSize, fileStream) != bufferSize)
    {
        return MC_STREAM_ERROR;
    }
    return MC_NO_ERROR;
}

error_t WriteBufferToFile(const char* restrict fileName, const char* restrict fileMode, const buffer_t* restrict buffer)
{
    file_t* fileStream = NULL;
    int32_t result; 

    if (strcmp(fileMode, "wb") != 0 && strcmp(fileMode, "ab") != 0)
    {
        return MC_FILE_MODE_ERROR;
    }
    if ((fileStream = fopen(fileName, fileMode)) == NULL)
    {
        return MC_STREAM_ERROR;
    }
    result = WriteBufferToStream(fileStream, buffer);
    fclose(fileStream);
    return result;
}

error_t LoadBufferFromStream(file_t* restrict fileStream, buffer_t* restrict buffer)
{
    if (fileStream == NULL)
    {
        return MC_STREAM_ERROR;
    }
    fread(buffer->Start, 1, GetBufferSize(buffer), fileStream);
    return MC_NO_ERROR;
}

error_t LoadBufferFromFile(const char* restrict fileName, buffer_t* restrict outBuffer)
{
    file_t* fileStream;
    int64_t bufferSize;
    int32_t writeResult;

    if ((fileStream = fopen(fileName, "rb")) == NULL || (bufferSize = GetFileSize(fileStream)) < 0)
    {
        return MC_STREAM_ERROR;
    }

    if (AllocateBufferChecked(bufferSize, 1, outBuffer) != MC_NO_ERROR)
    {     
        fclose(fileStream);
        return MC_MEM_ALLOCATION_ERROR;
    }

    writeResult = LoadBufferFromStream(fileStream, outBuffer);
    fclose(fileStream);
    return writeResult;
}

error_t WriteBufferHexToStream(file_t* restrict fileStream, const buffer_t* restrict buffer, size_t bytesPerLine)
{
    if (fileStream == NULL)
    {
        return MC_STREAM_ERROR;
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
    return MC_NO_ERROR;
}

error_t WriteBlockHexToStream(file_t* restrict fileStream, const memblock_array_t* restrict blockArray, size_t blocksPerLine)
{
    if (fileStream == NULL)
    {
        return MC_STREAM_ERROR;
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
    return MC_NO_ERROR;
}