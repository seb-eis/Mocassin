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

// Get the collection of resolvers for essential cmd arguments
static const CmdArgLookup_t* getEssentialCmdArgsResolverTable()
{
    static const CmdArgResolver_t resolvers[] =
            {
                    { "-dbPath",    (FValidator_t) ValidateStringNotNullOrEmpty,   (FCmdCallback_t) setDatabasePath },
                    { "-dbQuery",   (FValidator_t) ValidateDatabaseQueryString,   (FCmdCallback_t) setDatabaseLoadString }
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
                    { "-engPluginSymbol", (FValidator_t)  ValidateStringNotNullOrEmpty,(FCmdCallback_t) setEnergyPluginSymbol }
            };

    static const CmdArgLookup_t resolverTable =
            {
                    &resolvers[0],
                    &resolvers[sizeof(resolvers) / sizeof(CmdArgResolver_t)]
            };

    return &resolverTable;
}

// Searches for a command line argument in the passed resolver table and calls validator and callback if a handler is found
static error_t LookupAndResolveCmdArgument(__SCONTEXT_PAR, const CmdArgLookup_t* restrict resolverTable, const int32_t argId)
{
    break_on_debug(resolverTable != NULL);

    error_t error;
    char const * keyArgument = getCommandArgumentStringById(SCONTEXT, argId);
    char const * valArgument = getCommandArgumentStringById(SCONTEXT, argId + 1);

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

// Resolves the essential command line arguments and calls the affiliated callbacks
static error_t ResolveAndSetEssentialCmdArguments(__SCONTEXT_PAR)
{
    error_t error;

    const CmdArgLookup_t* resolverTable = getEssentialCmdArgsResolverTable();
    size_t unresolved = span_GetSize(*resolverTable);

    for (int32_t i = 1; i < getCommandArguments(SCONTEXT)->Count; i++)
    {
        error = LookupAndResolveCmdArgument(SCONTEXT, resolverTable, i);
        return_if(error == ERR_VALIDATION, error);

        if(error == ERR_OK)
        {
            --unresolved;
        }

        return_if(unresolved == 0, ERR_OK);
    }
    return ERR_CMDARGUMENT;
}

// Resolves all optional command line arguments and calls the affiliated callbacks
static error_t ResolveAndSetOptionalCmdArguments(__SCONTEXT_PAR)
{
    error_t error;

    const CmdArgLookup_t* resolverTable = getOptionalCmdArgsResolverTable();
    size_t unresolved = span_GetSize(*resolverTable);

    for (int32_t i = 1; i < getCommandArguments(SCONTEXT)->Count; i++)
    {
        error = LookupAndResolveCmdArgument(SCONTEXT, resolverTable, i);
        continue_if(error);
        return_if(--unresolved == 0, ERR_OK);
    }
    return ERR_OK;
}

void ResolveCommandLineArguments(__SCONTEXT_PAR, const int32_t argCount, char const * const * argValues)
{
    error_t error;

    setCommandArguments(SCONTEXT, argCount, argValues);
    setProgramRunPath(SCONTEXT, getCommandArgumentStringById(SCONTEXT, 0));

    error = ResolveAndSetEssentialCmdArguments(SCONTEXT);
    error_assert(error, "Failed to resolve essential command line arguments.");

    error = ResolveAndSetOptionalCmdArguments(SCONTEXT);
    error_assert(error, "Failed to resolve optional command line arguments.");
}