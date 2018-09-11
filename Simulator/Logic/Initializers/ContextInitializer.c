//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	ContextInitializer.h   		//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Context initializer logic   //
//////////////////////////////////////////

#include <strings.h>
#include "Framework/Basic/BaseTypes/BaseTypes.h"
#include "Simulator/Data/Model/Database/DbModel.h"
#include "Simulator/Logic/Routines/Statistics/McStatistics.h"
#include "Simulator/Logic/Objects/JumpSelection.h"
#include "Simulator/Logic/Constants/Constants.h"
#include "Framework/Basic/FileIO/FileIO.h"
#include "Simulator/Logic/Validators/Validators.h"
#include "Simulator/Logic/Initializers/ContextInitializer.h"
#include "Framework/Basic/Plugins/PluginLoading.h"
#include "Simulator/Logic/Routines/Environment/EnvRoutines.h"
#include "Simulator/Logic/Routines/Main/MainRoutines.h"
#include "Simulator/Logic/Routines/Helper/HelperRoutines.h"
#include "Simulator/Data/Model/SimContext/ContextAccess.h"
#include "Framework/Basic/BaseTypes/Buffers.h"

static const cmdarg_lookup_t* getEssentialCmdArgsResolverTable()
{
    static const cmdarg_resolver_t resolvers[] =
    {
        { "-dbPath",    (f_validator_t) ValidateStringNotNullOrEmpty,   (f_cmdcallback_t) setDatabasePath },
        { "-dbQuery",   (f_validator_t) ValidateDatabaseQueryString,   (f_cmdcallback_t) setDatabaseLoadString }
    };

    static const cmdarg_lookup_t resolverTable = 
    {
        &resolvers[0],
        &resolvers[sizeof(resolvers) / sizeof(cmdarg_resolver_t)]
    };

    return &resolverTable;
}

static const cmdarg_lookup_t* getOptionalCmdArgsResolverTable()
{
    static const cmdarg_resolver_t resolvers[] =
    {
        { "-outPluginPath",   (f_validator_t)  ValidateIsValidFilePath,     (f_cmdcallback_t) setOutputPluginPath },
        { "-outPluginSymbol", (f_validator_t)  ValidateStringNotNullOrEmpty,(f_cmdcallback_t) setOutputPluginSymbol },
        { "-engPluginPath",   (f_validator_t)  ValidateIsValidFilePath,     (f_cmdcallback_t) setEnergyPluginPath },
        { "-engPluginSymbol", (f_validator_t)  ValidateStringNotNullOrEmpty,(f_cmdcallback_t) setEnergyPluginSymbol }
    };

    static const cmdarg_lookup_t resolverTable =
    {
        &resolvers[0],
        &resolvers[sizeof(resolvers) / sizeof(cmdarg_resolver_t)]
    };

    return &resolverTable;
}

static error_t LookupAndResolveCmdArgument(__SCONTEXT_PAR, const cmdarg_lookup_t* restrict resolverTable, const int32_t argId)
{
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

static error_t ResolveAndSetEssentialCmdArguments(__SCONTEXT_PAR)
{
    error_t error;

    const cmdarg_lookup_t* resolverTable = getEssentialCmdArgsResolverTable();
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

static error_t ResolveAndSetOptionalCmdArguments(__SCONTEXT_PAR)
{
    error_t error;

    const cmdarg_lookup_t* resolverTable = getOptionalCmdArgsResolverTable();
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

static error_t ConstructEngStateBuffer(eng_states_t *restrict bufferAccess, const size_t count)
{
    buffer_t tmp = new_Span(tmp, count*sizeof(double));
    *bufferAccess = (eng_states_t) span_AsVoid(tmp);
    return tmp.Begin ? ERR_OK : ERR_MEMALLOCATION;
}

static error_t ConstructCluStateBuffer(clu_states_t *restrict bufferAccess, const size_t count)
{
    buffer_t tmp = new_Span(tmp, count*sizeof(clu_state_t));
    *bufferAccess = (clu_states_t) span_AsVoid(tmp);
    return tmp.Begin ? ERR_OK : ERR_MEMALLOCATION;
}

static error_t ConstructEnvironmentBuffers(env_state_t *restrict env, env_def_t *restrict envDef)
{
    error_t error;;

    setBufferByteValues(env, sizeof(env_state_t), 0);

    error = ConstructEngStateBuffer(&env->EnergyStates, FindLastEnvParId(envDef) + 1);
    return_if(error, error);

    error = ConstructCluStateBuffer(&env->ClusterStates, span_GetSize(envDef->ClusterDefinitions));
    return_if(error, error);

    return error;
}

static void ConstructEnvironmentLattice(__SCONTEXT_PAR)
{
    error_t error;
    vector4_t* sizes = getLatticeSizeVector(SCONTEXT);
    env_lattice_t lattice = new_Array(lattice, sizes->a, sizes->b, sizes->c, sizes->d);

    setEnvironmentLattice(SCONTEXT, lattice);

    for (int32_t i = 0; i < getEnvironmentLattice(SCONTEXT)->Header->Size; i++)
    {
        error = ConstructEnvironmentBuffers(getEnvironmentStateById(SCONTEXT, i), getEnvironmentModelById(SCONTEXT, i));
        error_assert(error, "Failed to construct environment state buffers.");
    }
}

static error_t ConstructLatticeEnergyBuffer(flp_buffer_t* restrict bufferAccess, mmc_header_t* restrict header)
{
    buffer_t tmp = new_Span(tmp, header->AbortSequenceLength * sizeof(double));
    *bufferAccess = (flp_buffer_t) { (void*) tmp.Begin, (void*) tmp.Begin, (void*) tmp.End, 0.0 };
    return tmp.Begin ? ERR_OK : ERR_MEMALLOCATION;
}

static void ConstructAbortConditionBuffers(__SCONTEXT_PAR)
{
    error_t error;
    if (JobInfoHasFlgs(SCONTEXT, FLG_MMC))
    {
        error = ConstructLatticeEnergyBuffer(getLatticeEnergyBuffer(SCONTEXT), getJobInformation(SCONTEXT)->JobHeader);
        error_assert(error, "Failed to construct lattice energy buffer.");
    }
}

static void ConstructSimulationModel(__SCONTEXT_PAR)
{
    ConstructEnvironmentLattice(SCONTEXT);
    ConstructAbortConditionBuffers(SCONTEXT);
}

static error_t ConstructSelectionPoolIndexRedirection(__SCONTEXT_PAR)
{
    error_t error;

    buffer_t tmpBuffer;
    int32_t poolCount = 1 + FindMaxJumpDirectionCount(&getTransitionModel(SCONTEXT)->JumpCountTable);
    int32_t poolIndex = 1;

    error = ctor_Buffer(tmpBuffer, poolCount * sizeof(int32_t));
    return_if(error, error);

    setBufferByteValues(tmpBuffer.Begin, span_GetSize(tmpBuffer), 0);
    setDirectionPoolIndexing(SCONTEXT, (id_redirect_t) span_AsVoid(tmpBuffer));

    cpp_foreach(dirCount, getTransitionModel(SCONTEXT)->JumpCountTable)
    {
        if ((*dirCount != 0) && (getDirectionPoolIdByJumpCount(SCONTEXT, *dirCount) != 0))
        {
            setDirectionPoolIdByJumpCount(SCONTEXT, *dirCount, poolIndex);
            poolIndex++;
        }
    }

    return ERR_OK;
}

static error_t ConstructSelectionPoolDirectionBuffers(__SCONTEXT_PAR)
{
    error_t error;

    buffer_t tmpBuffer;
    size_t poolCount = span_GetSize(*getDirectionPools(SCONTEXT));
    int32_t poolSize = getLatticeInformation(SCONTEXT)->NumOfSelectables;

    error = ctor_Buffer(tmpBuffer, poolCount * sizeof(dir_pool_t));
    return_if(error, error);
    
    setDirectionPools(SCONTEXT, (dir_pools_t) span_AsVoid(tmpBuffer));

    cpp_foreach(dirPool, *getDirectionPools(SCONTEXT))
    {
        error = ctor_Buffer(tmpBuffer, poolSize * sizeof(env_pool_t));
        return_if(error, error);

        dirPool->EnvironmentPool = (env_pool_t) span_AsList(tmpBuffer);
    }

    return ERR_OK;
}

static void ConstructJumpSelectionPool(__SCONTEXT_PAR)
{
    error_t error;

    error = ConstructSelectionPoolIndexRedirection(SCONTEXT);
    error_assert(error, "Failed to construct selection pool indexing information.");

    error = ConstructSelectionPoolDirectionBuffers(SCONTEXT);
    error_assert(error, "Failed to construct selection pool direction buffers.");
}

static int32_t ConfigStateHeaderAccess(__SCONTEXT_PAR)
{
    getMainStateHeader(SCONTEXT)->Data = getMainStateBufferAddress(SCONTEXT, 0);
    return sizeof(hdr_info_t);
}

static int32_t ConfigStateMetaAccess(__SCONTEXT_PAR, const int32_t usedBufferBytes)
{
    int32_t cfgBufferBytes = sizeof(meta_info_t);

    getMainStateHeader(SCONTEXT)->Data->MetaStartByte = usedBufferBytes;   
    getMainStateMetaInfo(SCONTEXT)->Data = getMainStateBufferAddress(SCONTEXT, usedBufferBytes);

    return usedBufferBytes + cfgBufferBytes;
}

static int32_t ConfigStateLatticeAccess(__SCONTEXT_PAR, const int32_t usedBufferBytes)
{
    lat_state_t* configObject = getMainStateLattice(SCONTEXT);
    int32_t cfgBufferBytes = getLatticeInformation(SCONTEXT)->Lattice.Header->Size;
    
    getMainStateHeader(SCONTEXT)->Data->LatticeStartByte = usedBufferBytes;
    configObject->Begin = getMainStateBufferAddress(SCONTEXT, usedBufferBytes);
    configObject->End = configObject->Begin + usedBufferBytes;

    return usedBufferBytes + cfgBufferBytes;
}

static int32_t ConfigStateCountersAccess(__SCONTEXT_PAR, const int32_t usedBufferBytes)
{
    cnt_state_t* configObject = getMainStateCounters(SCONTEXT);
    int32_t cfgBufferBytes = sizeof(cnt_col_t) * (int32_t) (GetMaxParId(SCONTEXT) + 1);
    
    getMainStateHeader(SCONTEXT)->Data->CountersStartByte = usedBufferBytes;
    configObject->Begin = getMainStateBufferAddress(SCONTEXT, usedBufferBytes);
    configObject->End = configObject->Begin + (usedBufferBytes / sizeof(cnt_col_t));

    return usedBufferBytes + cfgBufferBytes;
}

static int32_t ConfigStateAbstractTrackerAccess(__SCONTEXT_PAR, const int32_t usedBufferBytes)
{
    int32_t cfgBufferBytes = 0;
    getMainStateHeader(SCONTEXT)->Data->GlobalTrackerStartByte = JobHeaderHasFlgs(SCONTEXT, FLG_KMC) ? usedBufferBytes : -1;

    if (JobHeaderHasFlgs(SCONTEXT, FLG_KMC))
    {
        trc_state_t* configObject = getAbstractMovementTrackers(SCONTEXT);

        configObject->Begin = getMainStateBufferAddress(SCONTEXT, usedBufferBytes);
        configObject->End = configObject->Begin + getStructureModel(SCONTEXT)->NumOfGlobalTrackers;

        cfgBufferBytes = span_GetSize(*configObject) * sizeof(tracker_t);
    }

    return usedBufferBytes + cfgBufferBytes;
}

static int32_t ConfigStateMobileTrackerAccess(__SCONTEXT_PAR, const int32_t usedBufferBytes)
{
    int32_t cfgBufferBytes = 0;
    getMainStateHeader(SCONTEXT)->Data->MobileTrackerStartByte = JobHeaderHasFlgs(SCONTEXT, FLG_KMC) ? usedBufferBytes : -1;

    if (JobHeaderHasFlgs(SCONTEXT, FLG_KMC))
    {
        trc_state_t* configObject = getMobileMovementTrackers(SCONTEXT);

        configObject->Begin = getMainStateBufferAddress(SCONTEXT, usedBufferBytes);
        configObject->End = configObject->Begin + getLatticeInformation(SCONTEXT)->NumOfMobiles;

        cfgBufferBytes = span_GetSize(*configObject) * sizeof(tracker_t);
    }

    return usedBufferBytes + cfgBufferBytes;
}

static int32_t ConfigStateStaticTrackerAccess(__SCONTEXT_PAR, const int32_t usedBufferBytes)
{
    int32_t cfgBufferBytes = 0;
    getMainStateHeader(SCONTEXT)->Data->StaticTrackerStartByte = JobHeaderHasFlgs(SCONTEXT, FLG_KMC) ? usedBufferBytes : -1;

    if (JobHeaderHasFlgs(SCONTEXT, FLG_KMC))
    {
        trc_state_t* configObject = getStaticMovementTrackers(SCONTEXT);

        configObject->Begin = getMainStateBufferAddress(SCONTEXT, usedBufferBytes);
        configObject->End = configObject->Begin + (getStructureModel(SCONTEXT)->NumOfTrackersPerCell * GetNumberOfUnitCells(SCONTEXT));

        cfgBufferBytes = span_GetSize(*configObject)* sizeof(tracker_t);
    }

    return usedBufferBytes + cfgBufferBytes;
}

static int32_t ConfigStateMobileTrcIdxAccess(__SCONTEXT_PAR, const int32_t usedBufferBytes)
{
    int32_t cfgBufferBytes = 0;
    getMainStateHeader(SCONTEXT)->Data->MobileTrackerIdxStartByte = JobHeaderHasFlgs(SCONTEXT, FLG_KMC) ? usedBufferBytes : -1;

    if (JobHeaderHasFlgs(SCONTEXT, FLG_KMC))
    {
        idx_state_t* configObject = getMobileTrackerIndexing(SCONTEXT);

        configObject->Begin = getMainStateBufferAddress(SCONTEXT, usedBufferBytes);
        configObject->End = configObject->Begin + getLatticeInformation(SCONTEXT)->NumOfMobiles;

        cfgBufferBytes = span_GetSize(*configObject) * sizeof(int32_t);
    }

    return usedBufferBytes + cfgBufferBytes;
}

static int32_t ConfigStateJumpProbabilityMapAccess(__SCONTEXT_PAR, const int32_t usedBufferBytes)
{
    int32_t cfgBufferBytes = 0;
    getMainStateHeader(SCONTEXT)->Data->ProbabilityMapStartByte = JobHeaderHasFlgs(SCONTEXT, FLG_KMC) ? usedBufferBytes : -1;

    if (JobHeaderHasFlgs(SCONTEXT, FLG_KMC))
    {
        prb_state_t* configObject = getJumpProbabilityMap(SCONTEXT);

        configObject->Begin = getMainStateBufferAddress(SCONTEXT, usedBufferBytes);
        configObject->End = configObject->Begin + getLatticeInformation(SCONTEXT)->NumOfMobiles;

        cfgBufferBytes = span_GetSize(*configObject) * sizeof(int32_t);
    }

    return usedBufferBytes + cfgBufferBytes;
}

static error_t ConstructMainStateBufferAccessors(__SCONTEXT_PAR)
{
    int32_t usedBufferBytes = 0;

    usedBufferBytes = ConfigStateHeaderAccess(SCONTEXT);
    usedBufferBytes = ConfigStateMetaAccess(SCONTEXT, usedBufferBytes);
    usedBufferBytes = ConfigStateLatticeAccess(SCONTEXT, usedBufferBytes);
    usedBufferBytes = ConfigStateCountersAccess(SCONTEXT, usedBufferBytes);
    usedBufferBytes = ConfigStateAbstractTrackerAccess(SCONTEXT, usedBufferBytes);
    usedBufferBytes = ConfigStateMobileTrackerAccess(SCONTEXT, usedBufferBytes);
    usedBufferBytes = ConfigStateStaticTrackerAccess(SCONTEXT, usedBufferBytes);
    usedBufferBytes = ConfigStateMobileTrcIdxAccess(SCONTEXT, usedBufferBytes);
    usedBufferBytes = ConfigStateJumpProbabilityMapAccess(SCONTEXT, usedBufferBytes);

    return (usedBufferBytes == span_GetSize(*getMainStateBuffer(SCONTEXT)));
}

static void ConstructMainState(__SCONTEXT_PAR)
{
    error_t error;

    setBufferByteValues(getSimulationState(SCONTEXT), sizeof(mc_state_t), 0);

    size_t stateSize =(size_t)getJobInformation(SCONTEXT)->StateSize;

    error = ctor_Buffer(*getMainStateBuffer(SCONTEXT), stateSize);
    error_assert(error, "Failed to construct main state.");

    setBufferByteValues(getMainStateBuffer(SCONTEXT)->Begin, stateSize, 0);

    error = ConstructMainStateBufferAccessors(SCONTEXT);
    error_assert(error, "Failed to construct main state buffer accessor system.");
}

void ConstructSimulationContext(__SCONTEXT_PAR)
{
    ConstructSimulationModel(SCONTEXT);
    ConstructMainState(SCONTEXT);
    ConstructJumpSelectionPool(SCONTEXT);
}

static error_t TryLoadOuputPlugin(__SCONTEXT_PAR)
{
    error_t error = ERR_OK;
    file_info_t* fileInfo = getFileInformation(SCONTEXT);

    if ((fileInfo->OutputPluginPath) == NULL || (fileInfo->OutputPluginSymbol == NULL))
    {
        return ERR_USEDEFAULT;
    }

    if ((getPluginCollection(SCONTEXT)->OnDataOutput = ImportFunction(fileInfo->OutputPluginPath, fileInfo->OutputPluginSymbol, &error)) == NULL)
    {
        #if defined(IGNORE_INVALID_PLUGINS)
            fprintf(stdout, "[IGNORE_INVALID_PLUGINS] Error during output plugin loading. Using default settings.\n");
            return ERR_USEDEFAULT;
        #else
            runtime_assertion(false, error, "Cannot load requested ouput plugin.");
        #endif
    }

    return ERR_USEDEFAULT;
}

static error_t TryLoadEnergyPlugin(__SCONTEXT_PAR)
{
    error_t error = ERR_OK;
    file_info_t* fileInfo = getFileInformation(SCONTEXT);

    if ((fileInfo->EnergyPluginPath) == NULL || (fileInfo->EnergyPluginSymbol == NULL))
    {
        return ERR_USEDEFAULT;
    }

    if ((getPluginCollection(SCONTEXT)->OnSetJumpProbabilities = ImportFunction(fileInfo->EnergyPluginPath, fileInfo->EnergyPluginSymbol, &error)) == NULL)
    {
        #if defined(IGNORE_INVALID_PLUGINS)
            fprintf(stdout, "[IGNORE_INVALID_PLUGINS] Error during energy plugin loading. Using default settings.\n");
            return ERR_USEDEFAULT;
        #else
            runtime_assertion(false, error, "Cannot load requested energy plugin.");
        #endif
    }

    return ERR_USEDEFAULT;
}

static inline void SetEnergyPluginFunctionToDefault(__SCONTEXT_PAR)
{
    if (JobHeaderHasFlgs(SCONTEXT, FLG_KMC))
    {
        getPluginCollection(SCONTEXT)->OnSetJumpProbabilities = (f_plugin_t) SetKmcJumpProbsDefault;
    }
    else
    {
        getPluginCollection(SCONTEXT)->OnSetJumpProbabilities = (f_plugin_t) SetMmcJumpProbsDefault;
    }
}

static inline void SetOutputPluginFunctionToDefault(__SCONTEXT_PAR)
{
    getPluginCollection(SCONTEXT)->OnDataOutput = NULL;
}

static void PopulatePluginDelegateFunctions(__SCONTEXT_PAR)
{
    if (TryLoadOuputPlugin(SCONTEXT) == ERR_USEDEFAULT)
    {
        SetOutputPluginFunctionToDefault(SCONTEXT);
    }

    if (TryLoadEnergyPlugin(SCONTEXT) == ERR_USEDEFAULT)
    {
        SetEnergyPluginFunctionToDefault(SCONTEXT);
    }
}

static error_t TryLoadStateFromFile(__SCONTEXT_PAR, char const * restrict filePath)
{
    return_if(!IsAccessibleFile(filePath), ERR_USEDEFAULT);
    return LoadBufferFromFile(filePath, getMainStateBuffer(SCONTEXT));
}

static error_t DropCreateStateFile(__SCONTEXT_PAR, char const * restrict filePath)
{
    return WriteBufferToFile(filePath, FMODE_BINARY_W, getMainStateBuffer(SCONTEXT));
}

static error_t TryLoadSimulationState(__SCONTEXT_PAR)
{
    error_t error;

    if ((error = TryLoadStateFromFile(SCONTEXT, FILE_MAINSTATE)) == ERR_OK)
    {
        EnsureFileIsDeleted(FILE_PRERSTATE);
        return error;
    }

    return TryLoadStateFromFile(SCONTEXT, FILE_PRERSTATE);
}

static error_t SyncMainStateToDatabaseModel(__SCONTEXT_PAR)
{
    lattice_t * dbLattice = getDatabaseModelLattice(SCONTEXT);
    lat_state_t * stLattice = getMainStateLattice(SCONTEXT);

    size_t latticeSize = span_GetSize(*stLattice);

    return_if(latticeSize != dbLattice->Header->Size, ERR_DATACONSISTENCY);

    CopyBuffer(dbLattice->Begin, stLattice->Begin, latticeSize);
    return ERR_OK;
}

static error_t SyncDynamicEnvironmentsWithState(__SCONTEXT_PAR)
{
    env_lattice_t* envLattice = getEnvironmentLattice(SCONTEXT);
    lat_state_t* stLattice = getMainStateLattice(SCONTEXT);

    size_t latticeSize = span_GetSize(*stLattice);

    return_if(envLattice->Header->Size != latticeSize, ERR_DATACONSISTENCY);

    for (int32_t i = 0; i < latticeSize; i++)
    {
        SetEnvStateStatusToDefault(SCONTEXT, i, getStateLatticeEntryById(SCONTEXT, i));
    }

    return ERR_OK;
}

static error_t SyncDynamicModelToMainState(__SCONTEXT_PAR)
{
    // Potentially incomplete sync. review during testing
    error_t error = SyncDynamicEnvironmentsWithState(SCONTEXT);
    return error;
}

static void PopulateSimulationState(__SCONTEXT_PAR)
{
    error_t error;

    if ((error = TryLoadSimulationState(SCONTEXT)) == ERR_USEDEFAULT)
    {
        error = SyncMainStateToDatabaseModel(SCONTEXT);
        error_assert(error, "Data structure synchronization failure (static model ==> state).");

        error = DropCreateStateFile(SCONTEXT, FILE_PRERSTATE);
        error_assert(error, "Could not create initial state file.");

        return;
    }

    error_assert(error, "A state file exists but failed to load.");
}

static void PopulateDynamicSimulationModel(__SCONTEXT_PAR)
{
    error_t error;

    error = SyncDynamicModelToMainState(SCONTEXT);
    error_assert(error, "Data structure synchronization failed (state ==> dynamic model).");

    error = CalcPhysicalSimulationFactors(SCONTEXT, getPhysicalFactors(SCONTEXT));
    error_assert(error, "Failed to calculate default physical factors.");
}

static error_t SyncCycleCountersWithStateStatus(__SCONTEXT_PAR)
{
    cycle_cnt_t* counters = getMainCycleCounters(SCONTEXT);
    hdr_info_t* stHeader = getMainStateHeader(SCONTEXT)->Data;
    
    counters->Cycles = stHeader->Cycles;
    counters->Mcs = stHeader->Mcs;

    return (counters->Mcs < counters->TotalGoalMcs) ? ERR_OK : ERR_DATACONSISTENCY;
}

static void SyncSimulationCycleStateWithModel(__SCONTEXT_PAR)
{
    error_t error;

    error = CalcCycleCounterDefaultStatus(SCONTEXT, getMainCycleCounters(SCONTEXT));
    error_assert(error, "Failed to set default main counter status.");

    error = SyncCycleCountersWithStateStatus(SCONTEXT);
    error_assert(error, "Failed to synchronize data structure (state ==> cycle counters).");
}

static void SyncSelectionPoolWithDynamicModel(__SCONTEXT_PAR)
{
    error_t error;

    for (int32_t i = 0; i < getEnvironmentLattice(SCONTEXT)->Header->Size; i++)
    {
        error = HandleEnvStatePoolRegistration(SCONTEXT, i);
        error_assert(error, "Could not register environment on the jump selection pool.");
    }
}

void PopulateSimulationContext(__SCONTEXT_PAR)
{
    PopulatePluginDelegateFunctions(SCONTEXT);
    PopulateSimulationState(SCONTEXT);
    PopulateDynamicSimulationModel(SCONTEXT);
    SyncSimulationCycleStateWithModel(SCONTEXT);
    SyncSelectionPoolWithDynamicModel(SCONTEXT);
}

void PrepareContextForSimulation(__SCONTEXT_PAR)
{
    ConstructSimulationContext(SCONTEXT);
    PopulateSimulationContext(SCONTEXT);

    BuildEnvironmentLinkingSystem(SCONTEXT);
    SyncEnvironmentEnergyStatus(SCONTEXT);
}
