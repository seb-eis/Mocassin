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
static void TerminateOnSetBuildCallFlag(SCONTEXT_PARAM)
{
    let args = getCommandArguments(SCONTEXT);
    for (int32_t i = 1; i < args->Count; i++)
    {
        if (strcmp("--bcall", args->Values[i]) != 0) continue;
        fprintf(stdout, "Build call flag detected, terminating now!");
        exit(ERR_OK);
    }
}

// Tries to redirect the stdout and stderr stream to file streams (stdout is selectable, stderr defaults to stderr.log)
static void setStdoutRedirection(SCONTEXT_PARAM, const char* stdoutFile)
{
    var fileInfo = getFileInformation(SCONTEXT);
    char* tmp = NULL;
    char* tmp1 = NULL;

    var error = ConcatStrings(fileInfo->IODirectoryPath, "/", &tmp);
    error_assert(error, "Stream redirection of stdout failed on target building");
    error = ConcatStrings(tmp, stdoutFile, &tmp1);
    error_assert(error, "Stream redirection of stdout failed on target building");
    error = freopen(tmp1, "a", stdout) != NULL ? ERR_OK : ERR_STREAM;
    error_assert(error, "Stream redirection of stdout to a file returned a null stream");

    free(tmp1);

    error = ConcatStrings(tmp, "stderr.log", &tmp1);
    error_assert(error, "Stream redirection of stderr failed on target building");
    error = freopen(tmp1, "a", stderr) != NULL ? ERR_OK : ERR_STREAM;
    error_assert(error, "Stream redirection of stderr to a file returned a null stream");

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
        { "-fexp",            (FValidator_t)  ValidateAsTrue,              (FCmdCallback_t) setEnableFastExpMode},
    };

    static const CmdArgLookup_t resolverTable =
        {
            &resolvers[0],
            &resolvers[sizeof(resolvers) / sizeof(CmdArgResolver_t)]
        };

    return &resolverTable;
}

// Searches for a command line argument in the passed resolver table and calls validator and callback if a handler is found
static error_t LookupAndResolveCmdArgument(SCONTEXT_PARAM, const CmdArgLookup_t* restrict resolverTable, const int32_t argId)
{
    error_t error;
    let keyArgument = getCommandArgumentStringAt(SCONTEXT, argId);
    let valArgument = getCommandArgumentStringAt(SCONTEXT, argId + 1);

    error = ValidateCmdKeyArgumentFormat(keyArgument);
    return_if(error, ERR_CONTINUE);

    cpp_foreach(argResolver, *resolverTable)
    {
        if (strcmp(keyArgument, argResolver->KeyArgument) == 0)
        {
            error = argResolver->ValueValidator(valArgument);
            return_if(error, error);

            argResolver->ValueCallback(SCONTEXT, valArgument);
            return ERR_OK;
        }
    }
    
    return ERR_CMDARGUMENT;
}

// Resolves the essential command line arguments and using the affiliated callback table
static error_t ResolveAndSetEssentialCmdArguments(SCONTEXT_PARAM)
{
    error_t error;
    let resolverTable = getEssentialCmdArgsResolverTable();
    var unresolved = span_Length(*resolverTable);

    TerminateOnSetBuildCallFlag(SCONTEXT);

    for (int32_t i = 1; i < getCommandArguments(SCONTEXT)->Count; i++)
    {
        error = LookupAndResolveCmdArgument(SCONTEXT, resolverTable, i);
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
static error_t ResolveAndSetOptionalCmdArguments(SCONTEXT_PARAM)
{
    error_t error;
    let resolverTable = getOptionalCmdArgsResolverTable();
    var unresolved = span_Length(*resolverTable);

    for (int32_t i = 1; i < getCommandArguments(SCONTEXT)->Count; i++)
    {
        error = LookupAndResolveCmdArgument(SCONTEXT, resolverTable, i);
        continue_if(error);

        return_if(--unresolved == 0, ERR_OK);
    }

    return ERR_OK;
}

static error_t BuildAndSetFileTargets(SCONTEXT_PARAM)
{
    error_t error;
    var fileInfo = getFileInformation(SCONTEXT);
    char* tmp = NULL;

    error = ConcatStrings(fileInfo->IODirectoryPath, "/" FILE_MAINSTATE, &tmp);
    return_if(error, error);
    fileInfo->MainStateFile = tmp;

    error = ConcatStrings(fileInfo->IODirectoryPath, "/" FILE_PRERSTATE, &tmp);
    return_if(error, error);
    fileInfo->PrerunStateFile = tmp;

    return ERR_OK;
}

void ResolveCommandLineArguments(SCONTEXT_PARAM, const int32_t argCount, char const * const * argValues)
{
    error_t error;

    setCommandArguments(SCONTEXT, argCount, argValues);
    setProgramRunPath(SCONTEXT, getCommandArgumentStringAt(SCONTEXT, 0));

    error = ResolveAndSetEssentialCmdArguments(SCONTEXT);
    error_assert(error, "Failed to resolve essential command line arguments.");

    error = ResolveAndSetOptionalCmdArguments(SCONTEXT);
    error_assert(error, "Failed to resolve optional command line arguments.");

    error = BuildAndSetFileTargets(SCONTEXT);
    error_assert(error, "Failed to build required file targets.");
}