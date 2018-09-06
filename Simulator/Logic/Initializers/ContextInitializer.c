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
#include "Simulator/Logic/Routines/McStatistics.h"
#include "Simulator/Logic/Objects/JumpSelection.h"
#include "Simulator/Logic/Constants/Constants.h"
#include "Framework/Basic/FileIO/FileIO.h"
#include "Simulator/Logic/Validators/Validators.h"
#include "Simulator/Logic/Initializers/ContextInitializer.h"
#include "Framework/Basic/Plugins/PluginLoading.h"
#include "Simulator/Logic/Routines/EnvRoutines.h"
#include "Simulator/Logic/Routines/MainRoutines.h"
#include "Simulator/Logic/Routines/HelperRoutines.h"
#include "Simulator/Data/Model/SimContext/ContextAccess.h"

static const cmdarg_lookup_t* Get_EssentialCmdArgsResolverTable()
{
    static const cmdarg_resolver_t resolvers[] =
    {
        { "-dbPath",    (f_validator_t) ValidateStringNotNullOrEmpty,   (f_cmdcallback_t) Set_DatabasePath },
        { "-dbQuery",   (f_validator_t) ValidateDatabaseQueryString,   (f_cmdcallback_t) Set_DatabaseLoadString }
    };

    static const cmdarg_lookup_t resolverTable = 
    { 
        (int32_t) (sizeof(resolvers) / sizeof(cmdarg_resolver_t)),
        &resolvers[0],
        &resolvers[sizeof(resolvers) / sizeof(cmdarg_resolver_t)]
    };

    return &resolverTable;
}

static const cmdarg_lookup_t* Get_OptionalCmdArgsResolverTable()
{
    static const cmdarg_resolver_t resolvers[] =
    {
        { "-outPluginPath",   (f_validator_t)  ValidateIsValidFilePath,     (f_cmdcallback_t) Set_OutputPluginPath },
        { "-outPluginSymbol", (f_validator_t)  ValidateStringNotNullOrEmpty,(f_cmdcallback_t) Set_OutputPluginSymbol },
        { "-engPluginPath",   (f_validator_t)  ValidateIsValidFilePath,     (f_cmdcallback_t) Set_EnergyPluginPath },
        { "-engPluginSymbol", (f_validator_t)  ValidateStringNotNullOrEmpty,(f_cmdcallback_t) Set_EnergyPluginSymbol }
    };

    static const cmdarg_lookup_t resolverTable =
    {
        (int32_t) (sizeof(resolvers) / sizeof(cmdarg_resolver_t)),
        &resolvers[0],
        &resolvers[sizeof(resolvers) / sizeof(cmdarg_resolver_t)]
    };

    return &resolverTable;
}

static error_t LookupAndResolveCmdArgument(__SCONTEXT_PAR, const cmdarg_lookup_t* restrict resolverTable, const int32_t argId)
{
    error_t error;
    char const * keyArgument = Get_CommandArgumentStringById(SCONTEXT, argId);
    char const * valArgument = Get_CommandArgumentStringById(SCONTEXT, argId + 1);

    error = ValidateCmdKeyArgumentFormat(keyArgument);
    return_if(error, ERR_CONTINUE);

    FOR_EACH(const cmdarg_resolver_t, argResolver, *resolverTable)
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

    const cmdarg_lookup_t* resolverTable = Get_EssentialCmdArgsResolverTable();
    int32_t unresolved = resolverTable->Count;

    for (int32_t i = 1; i < Get_CommandArguments(SCONTEXT)->Count; i++)
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

    const cmdarg_lookup_t* resolverTable = Get_OptionalCmdArgsResolverTable();
    int32_t unresolved = resolverTable->Count;

    for (int32_t i = 1; i < Get_CommandArguments(SCONTEXT)->Count; i++)
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

    Set_CommandArguments(SCONTEXT, argCount, argValues);
    Set_ProgramRunPath(SCONTEXT, Get_CommandArgumentStringById(SCONTEXT, 0));

    error = ResolveAndSetEssentialCmdArguments(SCONTEXT);
    ASSERT_ERROR(error, "Failed to resolve essential command line arguments.");

    error = ResolveAndSetOptionalCmdArguments(SCONTEXT);
    ASSERT_ERROR(error, "Failed to resolve optional command line arguments.");
}

static error_t ConstructEngStateBuffer(eng_states_t *restrict bufferAccess, const byte_t count)
{
    buffer_t tmp = AllocateBufferUnchecked(count, sizeof(double));
    *bufferAccess = BUFFER_TO_ARRAY_WCOUNT(tmp, count, eng_states_t);
    return tmp.Start ? ERR_OK : ERR_MEMALLOCATION;
}

static error_t ConstructCluStateBuffer(clu_states_t *restrict bufferAccess, const byte_t count)
{
    buffer_t tmp = AllocateBufferUnchecked(count, sizeof(clu_state_t));
    *bufferAccess = BUFFER_TO_ARRAY_WCOUNT(tmp, count, clu_states_t);
    return tmp.Start ? ERR_OK : ERR_MEMALLOCATION;
}

static error_t ConstructEnvironmentBuffers(env_state_t *restrict env, env_def_t *restrict envDef)
{
    error_t error;;

    Set_BufferByteValues(env, sizeof(env_state_t), 0);

    error = ConstructEngStateBuffer(&env->EnergyStates, FindLastEnvParId(envDef) + 1);
    return_if(error, error);

    error = ConstructCluStateBuffer(&env->ClusterStates, envDef->ClusterDefinitions.Count);
    return_if(error, error);

    return error;
}

static void ConstructEnvironmentLattice(__SCONTEXT_PAR)
{
    error_t error;
    blob_t tmpBlob;

    error = AllocateMdaChecked(4, sizeof(env_state_t), (int32_t*) Get_LatticeSizeVector(SCONTEXT), &tmpBlob);
    ASSERT_ERROR(error, "Failed to construct the environment lattice buffer.");

    Set_EnvironmentLattice(SCONTEXT, CAST_OBJECT(env_lattice_t, tmpBlob));

    for (int32_t i = 0; i < Get_EnvironmentLattice(SCONTEXT)->Header->Size; i++)
    {
        error = ConstructEnvironmentBuffers(Get_EnvironmentStateById(SCONTEXT, i), Get_EnvironmentModelById(SCONTEXT, i));
        ASSERT_ERROR(error, "Failed to construct environment state buffers.");
    }
}

static error_t ConstructLatticeEnergyBuffer(flp_buffer_t* restrict bufferAccess, mmc_header_t* restrict header)
{
    buffer_t tmp = AllocateBufferUnchecked(header->AbortSequenceLength, sizeof(double));
    *bufferAccess = (flp_buffer_t) { header->AbortSequenceLength, 0.0, (void*) tmp.Start, (void*) tmp.End, (void*) tmp.End };
    return tmp.Start ? ERR_OK : ERR_MEMALLOCATION;
}

static void ConstructAbortConditionBuffers(__SCONTEXT_PAR)
{
    error_t error = ERR_OK;
    if (JobInfoHasFlgs(SCONTEXT, FLG_MMC))
    {
        error = ConstructLatticeEnergyBuffer(Get_LatticeEnergyBuffer(SCONTEXT), Get_JobInformation(SCONTEXT)->JobHeader);
        ASSERT_ERROR(error, "Failed to construct lattice energy buffer.");
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
    int32_t poolCount = 1 + FindMaxJumpDirectionCount(&Get_TransitionModel(SCONTEXT)->JumpCountTable);
    int32_t poolIndex = 1;

    error = AllocateBufferChecked(poolCount, sizeof(int32_t), &tmpBuffer);
    return_if(error, error);

    Set_BufferByteValues(tmpBuffer.Start, GetBufferSize(&tmpBuffer), 0);
    Set_DirectionPoolIndexing(SCONTEXT, BUFFER_TO_ARRAY_WCOUNT(tmpBuffer, poolCount, id_redirect_t));

    FOR_EACH(int32_t, dirCount, Get_TransitionModel(SCONTEXT)->JumpCountTable)
    {
        if ((*dirCount != 0) && (Get_DirectionPoolIdByJumpCount(SCONTEXT, *dirCount) != 0))
        {
            Set_DirectionPoolIdByJumpCount(SCONTEXT, *dirCount, poolIndex);
            poolIndex++;
        }
    }

    Get_DirectionPools(SCONTEXT)->Count = poolIndex;  
    return ERR_OK;
}

static error_t ConstructSelectionPoolDirectionBuffers(__SCONTEXT_PAR)
{
    error_t error;

    buffer_t tmpBuffer;
    int32_t poolCount = Get_DirectionPools(SCONTEXT)->Count;
    int32_t poolSize = Get_LatticeInformation(SCONTEXT)->NumOfSelectables;

    error = AllocateBufferChecked(poolCount, sizeof(dir_pool_t), &tmpBuffer);
    return_if(error, error);
    
    Set_DirectionPools(SCONTEXT, BUFFER_TO_ARRAY_WCOUNT(tmpBuffer, poolCount, dir_pools_t));

    FOR_EACH(dir_pool_t, dirPool, *Get_DirectionPools(SCONTEXT))
    {
        error = AllocateBufferChecked(poolSize, sizeof(env_pool_t), &tmpBuffer);
        return_if(error, error);

        dirPool->EnvironmentPool = BUFFER_TO_LIST(tmpBuffer, env_pool_t);
    }

    return ERR_OK;
}

static void ConstructJumpSelectionPool(__SCONTEXT_PAR)
{
    error_t error;

    error = ConstructSelectionPoolIndexRedirection(SCONTEXT);
    ASSERT_ERROR(error, "Failed to construct selection pool indexing information.");

    error = ConstructSelectionPoolDirectionBuffers(SCONTEXT);
    ASSERT_ERROR(error, "Failed to construct selection pool direction buffers.");
}

static size_t ConfigStateHeaderAccess(__SCONTEXT_PAR)
{
    Get_MainStateHeader(SCONTEXT)->Data = Get_MainStateBufferAddress(SCONTEXT, 0);
    return sizeof(hdr_info_t);
}

static size_t ConfigStateMetaAccess(__SCONTEXT_PAR, const int32_t usedBufferBytes)
{
    size_t cfgBufferBytes = sizeof(meta_info_t);

    Get_MainStateHeader(SCONTEXT)->Data->MetaStartByte = usedBufferBytes;   
    Get_MainStateMetaInfo(SCONTEXT)->Data = Get_MainStateBufferAddress(SCONTEXT, usedBufferBytes);

    return usedBufferBytes + cfgBufferBytes;
}

static size_t ConfigStateLatticeAccess(__SCONTEXT_PAR, const size_t usedBufferBytes)
{
    lat_state_t* configObject = Get_MainStateLattice(SCONTEXT);
    size_t cfgBufferBytes = Get_LatticeInformation(SCONTEXT)->Lattice.Header->Size;
    
    Get_MainStateHeader(SCONTEXT)->Data->LatticeStartByte = usedBufferBytes;
    configObject->Count = cfgBufferBytes;
    configObject->Start = Get_MainStateBufferAddress(SCONTEXT, usedBufferBytes);
    configObject->End = configObject->Start + configObject->Count;

    return usedBufferBytes + cfgBufferBytes;
}

static size_t ConfigStateCountersAccess(__SCONTEXT_PAR, const size_t usedBufferBytes)
{
    cnt_state_t* configObject = Get_MainStateCounters(SCONTEXT);
    size_t cfgBufferBytes = sizeof(cnt_col_t) * (size_t) (GetMaxParId(SCONTEXT) + 1);
    
    Get_MainStateHeader(SCONTEXT)->Data->CountersStartByte = usedBufferBytes;
    configObject->Count = cfgBufferBytes / sizeof(cnt_col_t);
    configObject->Start = Get_MainStateBufferAddress(SCONTEXT, usedBufferBytes);
    configObject->End = configObject->Start + configObject->Count;

    return usedBufferBytes + cfgBufferBytes;
}

static size_t ConfigStateAbstractTrackerAccess(__SCONTEXT_PAR, const size_t usedBufferBytes)
{
    size_t cfgBufferBytes = 0;
    Get_MainStateHeader(SCONTEXT)->Data->GlobalTrackerStartByte = JobHeaderHasFlgs(SCONTEXT, FLG_KMC) ? usedBufferBytes : -1;

    if (JobHeaderHasFlgs(SCONTEXT, FLG_KMC))
    {
        trc_state_t* configObject = Get_AbstractMovementTrackers(SCONTEXT);

        configObject->Count = Get_StructureModel(SCONTEXT)->NumOfGlobalTrackers;
        configObject->Start = Get_MainStateBufferAddress(SCONTEXT, usedBufferBytes);
        configObject->End = configObject->Start + configObject->Count;
        cfgBufferBytes = configObject->Count * sizeof(tracker_t);
    }

    return usedBufferBytes + cfgBufferBytes;
}

static size_t ConfigStateMobileTrackerAccess(__SCONTEXT_PAR, const size_t usedBufferBytes)
{
    size_t cfgBufferBytes = 0;
    Get_MainStateHeader(SCONTEXT)->Data->MobileTrackerStartByte = JobHeaderHasFlgs(SCONTEXT, FLG_KMC) ? usedBufferBytes : -1;

    if (JobHeaderHasFlgs(SCONTEXT, FLG_KMC))
    {
        trc_state_t* configObject = Get_MobileMovementTrackers(SCONTEXT);

        configObject->Count = Get_LatticeInformation(SCONTEXT)->NumOfMobiles;
        configObject->Start = Get_MainStateBufferAddress(SCONTEXT, usedBufferBytes);
        configObject->End = configObject->Start + configObject->Count;
        cfgBufferBytes = configObject->Count * sizeof(tracker_t);
    }

    return usedBufferBytes + cfgBufferBytes;
}

static size_t ConfigStateStaticTrackerAccess(__SCONTEXT_PAR, const size_t usedBufferBytes)
{
    size_t cfgBufferBytes = 0;
    Get_MainStateHeader(SCONTEXT)->Data->StaticTrackerStartByte = JobHeaderHasFlgs(SCONTEXT, FLG_KMC) ? usedBufferBytes : -1;

    if (JobHeaderHasFlgs(SCONTEXT, FLG_KMC))
    {
        trc_state_t* configObject = Get_StaticMovementTrackers(SCONTEXT);

        configObject->Count = Get_StructureModel(SCONTEXT)->NumOfTrackersPerCell * GetNumberOfUnitCells(SCONTEXT);
        configObject->Start = Get_MainStateBufferAddress(SCONTEXT, usedBufferBytes);
        configObject->End = configObject->Start + configObject->Count;
        cfgBufferBytes = configObject->Count * sizeof(tracker_t);
    }

    return usedBufferBytes + cfgBufferBytes;
}

static size_t ConfigStateMobileTrcIdxAccess(__SCONTEXT_PAR, const size_t usedBufferBytes)
{
    size_t cfgBufferBytes = 0;
    Get_MainStateHeader(SCONTEXT)->Data->MobileTrackerIdxStartByte = JobHeaderHasFlgs(SCONTEXT, FLG_KMC) ? usedBufferBytes : -1;

    if (JobHeaderHasFlgs(SCONTEXT, FLG_KMC))
    {
        idx_state_t* configObject = Get_MobileTrackerIndexing(SCONTEXT);

        configObject->Count = Get_LatticeInformation(SCONTEXT)->NumOfMobiles;
        cfgBufferBytes = configObject->Count * sizeof(int32_t);
        configObject->Start = Get_MainStateBufferAddress(SCONTEXT, usedBufferBytes);
        configObject->End = configObject->Start + configObject->Count;
    }

    return usedBufferBytes + cfgBufferBytes;
}

static size_t ConfigStateJumpProbabilityMapAccess(__SCONTEXT_PAR, const size_t usedBufferBytes)
{
    size_t cfgBufferBytes = 0;
    Get_MainStateHeader(SCONTEXT)->Data->ProbabilityMapStartByte = JobHeaderHasFlgs(SCONTEXT, FLG_KMC) ? usedBufferBytes : -1;

    if (JobHeaderHasFlgs(SCONTEXT, FLG_KMC))
    {
        prb_state_t* configObject = Get_JumpProbabilityMap(SCONTEXT);

        configObject->Count = Get_LatticeInformation(SCONTEXT)->NumOfMobiles;
        cfgBufferBytes = configObject->Count * sizeof(int32_t);
        configObject->Start = Get_MainStateBufferAddress(SCONTEXT, usedBufferBytes);
        configObject->End = configObject->Start + configObject->Count;
    }

    return usedBufferBytes + cfgBufferBytes;
}

static error_t ConstructMainStateBufferAccessors(__SCONTEXT_PAR)
{
    size_t usedBufferBytes = 0;

    usedBufferBytes = ConfigStateHeaderAccess(SCONTEXT);
    usedBufferBytes = ConfigStateMetaAccess(SCONTEXT, usedBufferBytes);
    usedBufferBytes = ConfigStateLatticeAccess(SCONTEXT, usedBufferBytes);
    usedBufferBytes = ConfigStateCountersAccess(SCONTEXT, usedBufferBytes);
    usedBufferBytes = ConfigStateAbstractTrackerAccess(SCONTEXT, usedBufferBytes);
    usedBufferBytes = ConfigStateMobileTrackerAccess(SCONTEXT, usedBufferBytes);
    usedBufferBytes = ConfigStateStaticTrackerAccess(SCONTEXT, usedBufferBytes);
    usedBufferBytes = ConfigStateMobileTrcIdxAccess(SCONTEXT, usedBufferBytes);
    usedBufferBytes = ConfigStateJumpProbabilityMapAccess(SCONTEXT, usedBufferBytes);

    return (usedBufferBytes == GetBufferSize(Get_MainStateBuffer(SCONTEXT)));
}

static void ConstructMainState(__SCONTEXT_PAR)
{
    error_t error = ERR_OK;

    Set_BufferByteValues(Get_SimulationState(SCONTEXT), sizeof(mc_state_t), 0);

    error = AllocateBufferChecked(Get_JobInformation(SCONTEXT)->StateSize, 1, Get_MainStateBuffer(SCONTEXT));
    ASSERT_ERROR(error, "Failed to construct main state.");

    Set_BufferByteValues(Get_MainStateBuffer(SCONTEXT)->Start, Get_JobInformation(SCONTEXT)->StateSize, 0);

    error = ConstructMainStateBufferAccessors(SCONTEXT);
    ASSERT_ERROR(error, "Failed to construct main state buffer accessor system.");
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
    file_info_t* fileInfo = Get_FileInformation(SCONTEXT);

    if ((fileInfo->OutputPluginPath) == NULL || (fileInfo->OutputPluginSymbol == NULL))
    {
        return ERR_USEDEFAULT;
    }

    if ((Get_PluginCollection(SCONTEXT)->OnDataOutput = ImportFunction(fileInfo->OutputPluginPath, fileInfo->OutputPluginSymbol, &error)) == NULL)
    {
        #if defined(IGNORE_INVALID_PLUGINS)
            fprintf(stdout, "[IGNORE_INVALID_PLUGINS] Error during output plugin loading. Using default settings.\n");
            return ERR_USEDEFAULT;
        #else
            RUNTIME_ASSERT(false, error, "Cannot load requested ouput plugin.");
        #endif
    }

    return ERR_USEDEFAULT;
}

static error_t TryLoadEnergyPlugin(__SCONTEXT_PAR)
{
    error_t error = ERR_OK;
    file_info_t* fileInfo = Get_FileInformation(SCONTEXT);

    if ((fileInfo->EnergyPluginPath) == NULL || (fileInfo->EnergyPluginSymbol == NULL))
    {
        return ERR_USEDEFAULT;
    }

    if ((Get_PluginCollection(SCONTEXT)->OnSetJumpProbabilities = ImportFunction(fileInfo->EnergyPluginPath, fileInfo->EnergyPluginSymbol, &error)) == NULL)
    {
        #if defined(IGNORE_INVALID_PLUGINS)
            fprintf(stdout, "[IGNORE_INVALID_PLUGINS] Error during energy plugin loading. Using default settings.\n");
            return ERR_USEDEFAULT;
        #else
            RUNTIME_ASSERT(false, error, "Cannot load requested energy plugin.");
        #endif
    }

    return ERR_USEDEFAULT;
}

static inline void SetEnergyPluginFunctionToDefault(__SCONTEXT_PAR)
{
    if (JobHeaderHasFlgs(SCONTEXT, FLG_KMC))
    {
        Get_PluginCollection(SCONTEXT)->OnSetJumpProbabilities = (f_plugin_t) SetKmcJumpProbsDefault;
    }
    else
    {
        Get_PluginCollection(SCONTEXT)->OnSetJumpProbabilities = (f_plugin_t) SetMmcJumpProbsDefault;
    }
}

static inline void SetOutputPluginFunctionToDefault(__SCONTEXT_PAR)
{
    Get_PluginCollection(SCONTEXT)->OnDataOutput = NULL;
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
    return LoadBufferFromFile(filePath, Get_MainStateBuffer(SCONTEXT));
}

static error_t DropCreateStateFile(__SCONTEXT_PAR, char const * restrict filePath)
{
    return WriteBufferToFile(filePath, FMODE_BINARY_W, Get_MainStateBuffer(SCONTEXT));
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
    lattice_t * dbLattice = Get_DatabaseModelLattice(SCONTEXT);
    lat_state_t * stLattice = Get_MainStateLattice(SCONTEXT);

    return_if(stLattice->Count != dbLattice->Header->Size, ERR_DATACONSISTENCY);

    CopyBuffer(dbLattice->Start, stLattice->Start, stLattice->Count);
    return ERR_OK;
}

static error_t SyncDynamicEnvironmentsWithState(__SCONTEXT_PAR)
{
    env_lattice_t* envLattice = Get_EnvironmentLattice(SCONTEXT);
    lat_state_t* stLattice = Get_MainStateLattice(SCONTEXT);

    return_if(envLattice->Header->Size != stLattice->Count, ERR_DATACONSISTENCY);

    for (int32_t i = 0; i < stLattice->Count; i++)
    {
        SetEnvStateStatusToDefault(SCONTEXT, i, Get_StateLatticeEntryById(SCONTEXT, i));
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
        ASSERT_ERROR(error, "Data structure synchronization failure (static model ==> state).");

        error = DropCreateStateFile(SCONTEXT, FILE_PRERSTATE);
        ASSERT_ERROR(error, "Could not create initial state file.");

        return;
    }

    ASSERT_ERROR(error, "A state file exists but failed to load.");
}

static void PopulateDynamicSimulationModel(__SCONTEXT_PAR)
{
    error_t error;

    error = SyncDynamicModelToMainState(SCONTEXT);
    ASSERT_ERROR(error, "Data structure synchronization failed (state ==> dynamic model).");

    error = CalcPhysicalSimulationFactors(SCONTEXT, Get_PhysicalFactors(SCONTEXT));
    ASSERT_ERROR(error, "Failed to calculate default physical factors.");
}

static error_t SyncCycleCountersWithStateStatus(__SCONTEXT_PAR)
{
    cycle_cnt_t* counters = Get_MainCycleCounters(SCONTEXT);
    hdr_info_t* stHeader = Get_MainStateHeader(SCONTEXT)->Data;
    
    counters->Cycles = stHeader->Cycles;
    counters->Mcs = stHeader->Mcs;

    return (counters->Mcs < counters->TotalGoalMcs) ? ERR_OK : ERR_DATACONSISTENCY;
}

static void SyncSimulationCycleStateWithModel(__SCONTEXT_PAR)
{
    error_t error;

    error = CalcCycleCounterDefaultStatus(SCONTEXT, Get_MainCycleCounters(SCONTEXT));
    ASSERT_ERROR(error, "Failed to set default main counter status.");

    error = SyncCycleCountersWithStateStatus(SCONTEXT);
    ASSERT_ERROR(error, "Failed to synchronize data structure (state ==> cycle counters).");
}

static void SyncSelectionPoolWithDynamicModel(__SCONTEXT_PAR)
{
    error_t error;

    for (int32_t i = 0; i < Get_EnvironmentLattice(SCONTEXT)->Header->Size; i++)
    {
        error = HandleEnvStatePoolRegistration(SCONTEXT, i);
        ASSERT_ERROR(error, "Could not register environment on the jump selection pool.");
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
