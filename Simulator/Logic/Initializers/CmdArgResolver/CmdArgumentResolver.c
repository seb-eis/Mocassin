//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	CmdArgumentResolver.h		//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Resolver for cmd arguments  //
//////////////////////////////////////////

#include <string.h>
#include "Simulator/Logic/Initializers/CmdArgResolver/CmdArgumentResolver.h"
#include "Simulator/Logic/Validators/Validators.h"

// Checks if the set of command arguments contains the build call flag and terminates the execution if true
static void TerminateOnSetBuildCallFlag(SCONTEXT_PARAMETER)
{
    let args = getCommandArguments(simContext);
    for (int32_t i = 1; i < args->Count; i++)
    {
        if (strcmp("--bcall", args->Values[i]) != 0) continue;
        fprintf(stdout, "Build call flag detected, terminating now!\n");
        exit(ERR_OK);
    }
}

// Wrapper for freopen() to correctly use utf8 encoded file names on both Win32 and linux
static struct _iobuf* freopen_utf8(const char* filename, const char* mode, file_t* file)
{
#if defined(WIN32)
    wchar_t * filename16, * mode16;
    var error = Win32ConvertUtf8ToUtf16(filename, &filename16);
    return_if(error <= 0, NULL);
    error = Win32ConvertUtf8ToUtf16(mode, &mode16);
    return_if(error <= 0, NULL);
    var iobuf = _wfreopen(filename16, mode16, file);
    return free(filename16), free(mode16), iobuf;
#else
    return freopen(filename, mode, file);
#endif
}

// Tries to redirect the stdout and stderr stream to file streams (stdout is selectable, stderr defaults to stderr.log)
static void setStdoutRedirection(SCONTEXT_PARAMETER, const char* stdoutFile)
{
    var fileInfo = getFileInformation(simContext);
    char* tmp = NULL;
    char* tmp1 = NULL;

    var error = ConcatStrings(fileInfo->IODirectoryPath, "/", &tmp);
    assert_success(error, "Stream redirection of stdout failed on target building");
    error = ConcatStrings(tmp, stdoutFile, &tmp1);
    assert_success(error, "Stream redirection of stdout failed on target building");
    error = freopen_utf8(tmp1, "a", stdout) != NULL ? ERR_OK : ERR_STREAM;
    assert_success(error, "Stream redirection of stdout to a file returned a null stream");

    free(tmp1);

    error = ConcatStrings(tmp, "stderr.log", &tmp1);
    assert_success(error, "Stream redirection of stderr failed on target building");
    error = freopen_utf8(tmp1, "a", stderr) != NULL ? ERR_OK : ERR_STREAM;
    assert_success(error, "Stream redirection of stderr to a file returned a null stream");

    free(tmp);
    free(tmp1);
}

// Get the collection of resolvers for essential cmd arguments
static const CmdArgLookup_t* getEssentialCmdArgsResolverTable()
{
    static const CmdArgResolver_t resolvers[] =
    {
        { "-dbPath",  (FValidator_t) ValidateIsValidFilePath,     (FCmdCallback_t) setDatabasePath },
        { "-jobId",   (FValidator_t) ValidateDatabaseQueryString, (FCmdCallback_t) setDatabaseLoadString },
        { "-ioPath",  (FValidator_t) ValidateIsDiretoryPath,      (FCmdCallback_t) setIODirectoryPath},
    };

    static const CmdArgLookup_t resolverTable =
    {
            &resolvers[0],
            &resolvers[sizeof(resolvers) / sizeof(CmdArgResolver_t)]
    };

    return &resolverTable;
}

// Get the collection of resolvers for non-essential cmd arguments
static const CmdArgLookup_t* getOptionalCmdArgsResolverTable()
{
    static const CmdArgResolver_t resolvers[] =
    {
        { "-outPluginPath",   (FValidator_t)  ValidateIsValidFilePath,     (FCmdCallback_t) setOutputPluginPath },
        { "-outPluginSymbol", (FValidator_t)  ValidateStringNotNullOrEmpty,(FCmdCallback_t) setOutputPluginSymbol },
        { "-engPluginPath",   (FValidator_t)  ValidateIsValidFilePath,     (FCmdCallback_t) setEnergyPluginPath },
        { "-engPluginSymbol", (FValidator_t)  ValidateStringNotNullOrEmpty,(FCmdCallback_t) setEnergyPluginSymbol },
        { "-stdout",          (FValidator_t)  ValidateStringNotNullOrEmpty,(FCmdCallback_t) setStdoutRedirection},
        { "-extDir",          (FValidator_t)  ValidateIsDiretoryPath,      (FCmdCallback_t) setExtensionLookupPath}
    };

    static const CmdArgLookup_t resolverTable =
        {
            &resolvers[0],
            &resolvers[sizeof(resolvers) / sizeof(CmdArgResolver_t)]
        };

    return &resolverTable;
}

// Searches for a command line argument in the passed resolver table and calls validator and callback if a handler is found
static error_t LookupAndResolveCmdArgument(SCONTEXT_PARAMETER, const CmdArgLookup_t* restrict resolverTable, const int32_t argId)
{
    error_t error;
    let keyArgument = getCommandArgumentStringAt(simContext, argId);
    let valArgument = getCommandArgumentStringAt(simContext, argId + 1);

    error = ValidateCmdKeyArgumentFormat(keyArgument);
    return_if(error, ERR_CONTINUE);

    cpp_foreach(argResolver, *resolverTable)
    {
        if (strcmp(keyArgument, argResolver->KeyArgument) == 0)
        {
            error = argResolver->ValueValidator(valArgument);
            return_if(error, error);

            argResolver->ValueCallback(simContext, valArgument);
            return ERR_OK;
        }
    }
    
    return ERR_CMDARGUMENT;
}

// Resolves the essential command line arguments and using the affiliated callback table
static error_t ResolveAndSetEssentialCmdArguments(SCONTEXT_PARAMETER)
{
    error_t error;
    let resolverTable = getEssentialCmdArgsResolverTable();
    var unresolved = span_Length(*resolverTable);

    TerminateOnSetBuildCallFlag(simContext);

    for (int32_t i = 1; i < getCommandArguments(simContext)->Count; i++)
    {
        error = LookupAndResolveCmdArgument(simContext, resolverTable, i);
        return_if(error == ERR_VALIDATION, error);

        if (error == ERR_OK)
        {
            --unresolved;
        }

        return_if(unresolved == 0, ERR_OK);
    }

    return ERR_CMDARGUMENT;
}

// Resolves all optional command line arguments and calls the affiliated callbacks
static error_t ResolveAndSetOptionalCmdArguments(SCONTEXT_PARAMETER)
{
    error_t error;
    let resolverTable = getOptionalCmdArgsResolverTable();
    var unresolved = span_Length(*resolverTable);

    for (int32_t i = 1; i < getCommandArguments(simContext)->Count; i++)
    {
        error = LookupAndResolveCmdArgument(simContext, resolverTable, i);
        continue_if(error);

        return_if(--unresolved == 0, ERR_OK);
    }

    return ERR_OK;
}

static error_t BuildAndSetFileTargets(SCONTEXT_PARAMETER)
{
    error_t error;
    var fileInfo = getFileInformation(simContext);
    char* tmp = NULL;

    error = ConcatStrings(fileInfo->IODirectoryPath, "/" FILE_MAINSTATE, &tmp);
    return_if(error, error);
    fileInfo->MainStateFile = tmp;

    error = ConcatStrings(fileInfo->IODirectoryPath, "/" FILE_PRERSTATE, &tmp);
    return_if(error, error);
    fileInfo->PrerunStateFile = tmp;

    return ERR_OK;
}

void ResolveCommandLineArguments(SCONTEXT_PARAMETER, const int32_t argCount, char const * const * argValues)
{
    error_t error;

    setCommandArguments(simContext, argCount, argValues);
    setProgramRunPath(simContext, getCommandArgumentStringAt(simContext, 0));

    error = ResolveAndSetEssentialCmdArguments(simContext);
    assert_success(error, "Failed to resolve essential command line arguments.");

    error = ResolveAndSetOptionalCmdArguments(simContext);
    assert_success(error, "Failed to resolve optional command line arguments.");

    error = BuildAndSetFileTargets(simContext);
    assert_success(error, "Failed to build required file targets.");
}