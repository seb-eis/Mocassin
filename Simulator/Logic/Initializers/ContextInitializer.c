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
    static cmdarg_resolver_t resolvers[] =
    {
        { "-dbPath", (f_validator_t) ValidateStringNotNullOrEmpty, (f_cmdcallback_t) Set_DatabasePath }
    };
    static cmdarg_lookup_t resolverTable = 
    { 
        (int32_t) (sizeof(resolvers) / sizeof(cmdarg_resolver_t)),
        &resolvers[0],
        &resolvers[sizeof(resolvers) / sizeof(cmdarg_resolver_t)]
    };
    return &resolverTable;
}

static const cmdarg_lookup_t* Get_OptionalCmdArgsResolverTable()
{
    static cmdarg_resolver_t resolvers[] =
    {
        { "-outPluginPath",   (f_validator_t)  ValidateIsValidFilePath,     (f_cmdcallback_t) Set_OutputPluginPath },
        { "-outPluginSymbol", (f_validator_t)  ValidateStringNotNullOrEmpty,(f_cmdcallback_t) Set_OutputPluginSymbol },
        { "-engPluginPath",   (f_validator_t)  ValidateIsValidFilePath,     (f_cmdcallback_t) Set_EnergyPluginPath },
        { "-engPluginSymbol", (f_validator_t)  ValidateStringNotNullOrEmpty,(f_cmdcallback_t) Set_EnergyPluginSymbol }
    };
    static cmdarg_lookup_t resolverTable =
    {
        (int32_t) (sizeof(resolvers) / sizeof(cmdarg_resolver_t)),
        &resolvers[0],
        &resolvers[sizeof(resolvers) / sizeof(cmdarg_resolver_t)]
    };
    return &resolverTable;
}

static error_t LookupAndResolveCmdArgument(__SCONTEXT_PAR, const cmdarg_lookup_t* restrict resolverTable, const int32_t argId)
{
    char const * keyArgument = Get_CommandArgumentStringById(SCONTEXT, argId);
    char const * valArgument = Get_CommandArgumentStringById(SCONTEXT, argId + 1);

    if (ValidateCmdKeyArgumentFormat(keyArgument) != ERR_OK)
    {
        return ERR_CMDARGUMENT;
    }

    FOR_EACH(cmdarg_resolver_t, argResolver, *resolverTable)
    {   
        if (strcmp(keyArgument, argResolver->KeyArgument) == 0)
        {
            if(argResolver->ValueValidator(valArgument) != ERR_OK)
            {
                return ERR_VALIDATION;
            }
            argResolver->ValueCallback(SCONTEXT, valArgument);
            return ERR_OK;
        }
    }
    return ERR_CMDARGUMENT;
}

static error_t ResolveAndSetEssentialCmdArguments(__SCONTEXT_PAR)
{
    const cmdarg_lookup_t* resolverTable = Get_EssentialCmdArgsResolverTable();
    int32_t unresolved = resolverTable->Count;
    error_t error = ERR_OK;

    for (int32_t i = 1; i < Get_CommandArguments(SCONTEXT)->Count; i++)
    {
        if ((error = LookupAndResolveCmdArgument(SCONTEXT, resolverTable, i)) == ERR_OK)
        {
            if((--unresolved) == 0)
            {
                return ERR_OK;
            }
        }
        if (error == ERR_VALIDATION)
        {
            return error;
        }
    }
    return ERR_CMDARGUMENT;
}

static error_t ResolveAndSetOptionalCmdArguments(__SCONTEXT_PAR)
{
    const cmdarg_lookup_t* resolverTable = Get_OptionalCmdArgsResolverTable();
    int32_t unresolved = resolverTable->Count;

    for (int32_t i = 1; i < Get_CommandArguments(SCONTEXT)->Count; i++)
    {
        if (LookupAndResolveCmdArgument(SCONTEXT, resolverTable, i) == ERR_OK)
        {
            if((--unresolved) == 0)
            {
                return ERR_OK;
            }
        }
    }
    return ERR_OK;
}

void ResolveCommandLineArguments(__SCONTEXT_PAR, const int32_t argCount, char const * const * argValues)
{
    error_t error = ERR_OK;
    Set_CommandArguments(SCONTEXT, argCount, argValues);
    Set_ProgramRunPath(SCONTEXT, Get_CommandArgumentStringById(SCONTEXT, 0));

    if ((error = ResolveAndSetEssentialCmdArguments(SCONTEXT)) != ERR_OK)
    {
        MC_ERROREXIT(error, "Failed to resolve essential command line arguments.");
    }
    if ((error = ResolveAndSetOptionalCmdArguments(SCONTEXT)) != ERR_OK)
    {
        MC_ERROREXIT(error, "Failed to resolve optional command line arguments.")
    }
}

static error_t ConstructEngStateBuffer(eng_states_t *restrict bufferAccess, const byte_t count)
{
    buffer_t tmp = AllocateBufferUnchecked(count, sizeof(double));
    *bufferAccess = BUFFER_TO_ARRAY_WCOUNT(tmp, count, eng_states_t);
    return tmp.Start ? ERR_OK : ERR_MEMALLOCATION;
}

static error_t ConstructEnvLinkBuffer(env_links_t *restrict bufferAccess, const int32_t count)
{
    buffer_t tmp = AllocateBufferUnchecked(count, sizeof(env_link_t));
    *bufferAccess = BUFFER_TO_ARRAY_WCOUNT(tmp, count, env_links_t);
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
    error_t error = ERR_OK;
    Set_BufferByteValues(env, sizeof(env_state_t), 0);

    error |= ConstructEngStateBuffer(&env->EnergyStates, FindLastEnvParId(envDef) + 1);
    error |= ConstructCluStateBuffer(&env->ClusterStates, envDef->CluDefs.Count);

    return ERR_OK;
}

static void ConstructEnvironmentLattice(__SCONTEXT_PAR)
{
    error_t error = ERR_OK;
    blob_t tmpBlob;

    if ((error = AllocateMdaChecked(4, sizeof(env_state_t), (int32_t*) Get_LatticeSizeVector(SCONTEXT), &tmpBlob)) != ERR_OK)
    {
        MC_ERROREXIT(error, "Failed to construct the environment lattice buffer.");
    }

    Set_EnvironmentLattice(SCONTEXT, CAST_OBJECT(env_lattice_t, tmpBlob));

    for (int32_t i = 0; i < Get_EnvironmentLattice(SCONTEXT)->Header->Size; i++)
    {
        if ((error = ConstructEnvironmentBuffers(Get_EnvironmentStateById(SCONTEXT, i), Get_EnvironmentModelById(SCONTEXT, i))) != ERR_OK)
        {
            MC_ERROREXIT(error, "Failed to construct environment state buffers.");
        }
    }
}

static error_t ConstructLatticeEnergyBuffer(flp_buffer_t* restrict bufferAccess, mmc_header_t* restrict header)
{
    buffer_t tmp = AllocateBufferUnchecked(header->AbortSeqLen, sizeof(double));
    *bufferAccess = (flp_buffer_t) { header->AbortSeqLen, 0.0, (void*) tmp.Start, (void*) tmp.End, (void*) tmp.End };
    return tmp.Start ? ERR_OK : ERR_MEMALLOCATION;
}

static void ConstructAbortConditionBuffers(__SCONTEXT_PAR)
{
    error_t error = ERR_OK;
    if (JobInfoHasFlgs(SCONTEXT, FLG_MMC))
    {
        if ((error = ConstructLatticeEnergyBuffer(Get_LatticeEnergyBuffer(SCONTEXT), Get_JobInformation(SCONTEXT)->JobHeader)) != ERR_OK)
        {
            MC_ERROREXIT(error, "Failed to construct lattice energy buffer.")
        }
    }
}

static void ConstructSimulationModel(__SCONTEXT_PAR)
{
    ConstructEnvironmentLattice(SCONTEXT);
    ConstructAbortConditionBuffers(SCONTEXT);
}

static error_t ConstructSelectionPoolIndexRedirection(__SCONTEXT_PAR)
{
    buffer_t tmpBuffer;
    int32_t poolCount = 1 + FindMaxJumpDirectionCount(&Get_TransitionModel(SCONTEXT)->JumpCountTable);

    if (AllocateBufferChecked(poolCount, sizeof(int32_t), &tmpBuffer) != ERR_OK)
    {
        return ERR_MEMALLOCATION;
    }

    Set_BufferByteValues(tmpBuffer.Start, GetBufferSize(&tmpBuffer), 0);
    Set_DirectionPoolIndexing(SCONTEXT, BUFFER_TO_ARRAY_WCOUNT(tmpBuffer, poolCount, id_redirect_t));

    int32_t poolIndex = 1;
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
    buffer_t tmpBuffer;
    int32_t poolCount = Get_DirectionPools(SCONTEXT)->Count;
    int32_t poolSize = Get_LatticeInformation(SCONTEXT)->SelectableCount;

    if (AllocateBufferChecked(poolCount, sizeof(dir_pool_t), &tmpBuffer) != ERR_OK)
    {
        return ERR_MEMALLOCATION;
    }
    
    Set_DirectionPools(SCONTEXT, BUFFER_TO_ARRAY_WCOUNT(tmpBuffer, poolCount, dir_pools_t));

    FOR_EACH(dir_pool_t, dirPool, *Get_DirectionPools(SCONTEXT))
    {
        if (AllocateBufferChecked(poolSize, sizeof(env_pool_t), &tmpBuffer) != ERR_OK)
        {
            return ERR_MEMALLOCATION;
        }
        dirPool->EnvPool = BUFFER_TO_LIST(tmpBuffer, env_pool_t);
    }

    return ERR_OK;
}

static void ConstructJumpSelectionPool(__SCONTEXT_PAR)
{
    error_t error = ERR_OK;

    if ((error = ConstructSelectionPoolIndexRedirection(SCONTEXT)) != ERR_OK)
    {        
        MC_ERROREXIT(error, "Failed to construct selection pool indexing information.");
    }

    if ((error = ConstructSelectionPoolDirectionBuffers(SCONTEXT)) != ERR_OK)
    {
        MC_ERROREXIT(error, "Failed to construct selection pool direction buffers.");
    }
}

static size_t ConfigStateHeaderAccess(__SCONTEXT_PAR)
{
    Get_MainStateHeader(SCONTEXT)->Data = Get_MainStateBufferAddress(SCONTEXT, 0);
    return sizeof(hdr_info_t);
}

static size_t ConfigStateMetaAccess(__SCONTEXT_PAR, const int32_t usedBufferBytes)
{
    size_t cfgBufferBytes = sizeof(meta_info_t);

    Get_MainStateHeader(SCONTEXT)->Data->MetaByte = usedBufferBytes;   
    Get_MainStateMetaInfo(SCONTEXT)->Data = Get_MainStateBufferAddress(SCONTEXT, usedBufferBytes);

    return usedBufferBytes + cfgBufferBytes;
}

static size_t ConfigStateLatticeAccess(__SCONTEXT_PAR, const size_t usedBufferBytes)
{
    lat_state_t* configObject = Get_MainStateLattice(SCONTEXT);
    size_t cfgBufferBytes = Get_LatticeInformation(SCONTEXT)->Lattice.Header->Size;
    
    Get_MainStateHeader(SCONTEXT)->Data->LatticeByte = usedBufferBytes;
    configObject->Count = cfgBufferBytes;
    configObject->Start = Get_MainStateBufferAddress(SCONTEXT, usedBufferBytes);
    configObject->End = configObject->Start + configObject->Count;

    return usedBufferBytes + cfgBufferBytes;
}

static size_t ConfigStateCountersAccess(__SCONTEXT_PAR, const size_t usedBufferBytes)
{
    cnt_state_t* configObject = Get_MainStateCounters(SCONTEXT);
    size_t cfgBufferBytes = sizeof(cnt_col_t) * (size_t) (GetMaxParId(SCONTEXT) + 1);
    
    Get_MainStateHeader(SCONTEXT)->Data->CountersByte = usedBufferBytes;
    configObject->Count = cfgBufferBytes / sizeof(cnt_col_t);
    configObject->Start = Get_MainStateBufferAddress(SCONTEXT, usedBufferBytes);
    configObject->End = configObject->Start + configObject->Count;

    return usedBufferBytes + cfgBufferBytes;
}

static size_t ConfigStateAbstractTrackerAccess(__SCONTEXT_PAR, const size_t usedBufferBytes)
{
    size_t cfgBufferBytes = 0;
    Get_MainStateHeader(SCONTEXT)->Data->GlobalTrcByte = JobHeaderHasFlgs(SCONTEXT, FLG_KMC) ? usedBufferBytes : -1;

    if (JobHeaderHasFlgs(SCONTEXT, FLG_KMC))
    {
        trc_state_t* configObject = Get_AbstractMovementTrackers(SCONTEXT);

        configObject->Count = Get_StructureModel(SCONTEXT)->GloTrcCount;
        configObject->Start = Get_MainStateBufferAddress(SCONTEXT, usedBufferBytes);
        configObject->End = configObject->Start + configObject->Count;
        cfgBufferBytes = configObject->Count * sizeof(tracker_t);
    }

    return usedBufferBytes + cfgBufferBytes;
}

static size_t ConfigStateMobileTrackerAccess(__SCONTEXT_PAR, const size_t usedBufferBytes)
{
    size_t cfgBufferBytes = 0;
    Get_MainStateHeader(SCONTEXT)->Data->MobileTrcByte = JobHeaderHasFlgs(SCONTEXT, FLG_KMC) ? usedBufferBytes : -1;

    if (JobHeaderHasFlgs(SCONTEXT, FLG_KMC))
    {
        trc_state_t* configObject = Get_MobileMovementTrackers(SCONTEXT);

        configObject->Count = Get_LatticeInformation(SCONTEXT)->MobilesCount;
        configObject->Start = Get_MainStateBufferAddress(SCONTEXT, usedBufferBytes);
        configObject->End = configObject->Start + configObject->Count;
        cfgBufferBytes = configObject->Count * sizeof(tracker_t);
    }

    return usedBufferBytes + cfgBufferBytes;
}

static size_t ConfigStateStaticTrackerAccess(__SCONTEXT_PAR, const size_t usedBufferBytes)
{
    size_t cfgBufferBytes = 0;
    Get_MainStateHeader(SCONTEXT)->Data->StaticTrcByte = JobHeaderHasFlgs(SCONTEXT, FLG_KMC) ? usedBufferBytes : -1;

    if (JobHeaderHasFlgs(SCONTEXT, FLG_KMC))
    {
        trc_state_t* configObject = Get_StaticMovementTrackers(SCONTEXT);

        configObject->Count = Get_StructureModel(SCONTEXT)->CellTrcCount * GetNumberOfUnitCells(SCONTEXT);
        configObject->Start = Get_MainStateBufferAddress(SCONTEXT, usedBufferBytes);
        configObject->End = configObject->Start + configObject->Count;
        cfgBufferBytes = configObject->Count * sizeof(tracker_t);
    }

    return usedBufferBytes + cfgBufferBytes;
}

static size_t ConfigStateMobileTrcIdxAccess(__SCONTEXT_PAR, const size_t usedBufferBytes)
{
    size_t cfgBufferBytes = 0;
    Get_MainStateHeader(SCONTEXT)->Data->MobileTrcIdxByte = JobHeaderHasFlgs(SCONTEXT, FLG_KMC) ? usedBufferBytes : -1;

    if (JobHeaderHasFlgs(SCONTEXT, FLG_KMC))
    {
        idx_state_t* configObject = Get_MobileTrackerIndexing(SCONTEXT);

        configObject->Count = Get_LatticeInformation(SCONTEXT)->MobilesCount;
        cfgBufferBytes = configObject->Count * sizeof(int32_t);
        configObject->Start = Get_MainStateBufferAddress(SCONTEXT, usedBufferBytes);
        configObject->End = configObject->Start + configObject->Count;
    }

    return usedBufferBytes + cfgBufferBytes;
}

static size_t ConfigStateJumpProbabilityMapAccess(__SCONTEXT_PAR, const size_t usedBufferBytes)
{
    size_t cfgBufferBytes = 0;
    Get_MainStateHeader(SCONTEXT)->Data->ProbStatMapByte = JobHeaderHasFlgs(SCONTEXT, FLG_KMC) ? usedBufferBytes : -1;

    if (JobHeaderHasFlgs(SCONTEXT, FLG_KMC))
    {
        prb_state_t* configObject = Get_JumpProbabilityMap(SCONTEXT);

        configObject->Count = Get_LatticeInformation(SCONTEXT)->MobilesCount;
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

    return (usedBufferBytes == GetBufferSize(RefStateBuffer(SCONTEXT)));
}

static void ConstructMainState(__SCONTEXT_PAR)
{
    error_t error = ERR_OK;

    Set_BufferByteValues(Get_SimulationState(SCONTEXT), sizeof(mc_state_t), 0);

    if ((error = AllocateBufferChecked(Get_JobInformation(SCONTEXT)->StateSize, 1, Get_MainStateBuffer(SCONTEXT))) != ERR_OK)
    {
        MC_ERROREXIT(error, "Failed to construct main state.");
    }

    Set_BufferByteValues(Get_MainStateBuffer(SCONTEXT)->Start, Get_JobInformation(SCONTEXT)->StateSize, 0);

    if ((error = ConstructMainStateBufferAccessors(SCONTEXT)) != ERR_OK)
    {
        MC_ERROREXIT(error, "Failed to construct main state buffer accessor system.");
    }
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

    if ((Get_PluginCollection(SCONTEXT)->OnDataOut = ImportFunction(fileInfo->OutputPluginPath, fileInfo->OutputPluginSymbol, &error)) == NULL)
    {
        #ifdef IGNORE_INVALID_PLUGINS
            fprintf(stdout, "[IGNORE_INVALID_PLUGINS] Error during output plugin loading. Using default settings.\n");
            return ERR_USEDEFAULT;
        #else
            MC_ERROREXIT(error, "Cannot load requested ouput plugin.");
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

    if ((Get_PluginCollection(SCONTEXT)->OnSetJumpProbs = ImportFunction(fileInfo->EnergyPluginPath, fileInfo->EnergyPluginSymbol, &error)) == NULL)
    {
        #ifdef IGNORE_INVALID_PLUGINS
            fprintf(stdout, "[IGNORE_INVALID_PLUGINS] Error during energy plugin loading. Using default settings.\n");
            return ERR_USEDEFAULT;
        #else
            MC_ERROREXIT(error, "Cannot load requested energy plugin.");
        #endif
    }

    return ERR_USEDEFAULT;
}

static inline void SetEnergyPluginFunctionToDefault(__SCONTEXT_PAR)
{
    if (JobHeaderHasFlgs(SCONTEXT, FLG_KMC))
    {
        Get_PluginCollection(SCONTEXT)->OnSetJumpProbs = (f_plugin_t) SetKmcJumpProbsDefault;
    }
    else
    {
        Get_PluginCollection(SCONTEXT)->OnSetJumpProbs = (f_plugin_t) SetMmcJumpProbsDefault;
    }
}

static inline void SetOutputPluginFunctionToDefault(__SCONTEXT_PAR)
{
    Get_PluginCollection(SCONTEXT)->OnDataOut = NULL;
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
    if (!IsAccessibleFile(filePath))
    {
        return ERR_USEDEFAULT;
    }
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

    if ((error = TryLoadStateFromFile(SCONTEXT, FILE_PRERSTATE)) == ERR_OK)
    {
        return error;
    }

    return error;
}

static error_t SyncMainStateToDatabaseModel(__SCONTEXT_PAR)
{
    lattice_t * dbLattice = Get_DatabaseModelLattice(SCONTEXT);
    lat_state_t * stLattice = Get_MainStateLattice(SCONTEXT);

    if (stLattice->Count != dbLattice->Header->Size)
    {
        return ERR_DATACONSISTENCY;
    }

    CopyBuffer(dbLattice->Start, stLattice->Start, stLattice->Count);
    return ERR_OK;
}

static error_t SyncDynamicEnvironmentsWithState(__SCONTEXT_PAR)
{
    env_lattice_t* envLattice = Get_EnvironmentLattice(SCONTEXT);
    lat_state_t* stLattice = Get_MainStateLattice(SCONTEXT);

    if (envLattice->Header->Size != stLattice->Count)
    {
        return ERR_DATACONSISTENCY;
    }

    for (int32_t i = 0; i < stLattice->Count; i++)
    {
        env_state_t* envState = Get_EnvironmentStateById(SCONTEXT, i);
        envState->ParId = Get_StateLatticeEntryById(SCONTEXT, i);
        envState->EnvId = i;
    }
}

static error_t SyncDynamicModelToMainState(__SCONTEXT_PAR)
{
    error_t error;

    if ((error = SyncEnvironmentsWithState(SCONTEXT)) != ERR_OK)
    {

    }

    return ERR_OK;
}

static void PopulateSimulationState(__SCONTEXT_PAR)
{
    error_t error;

    if ((error = TryLoadSimulationState(SCONTEXT)) == ERR_USEDEFAULT)
    {
        if((error = SyncMainStateToDatabaseModel(SCONTEXT)) != ERR_OK )
        {
            MC_ERROREXIT(error, "Data structure synchronization failure (static model ==> state).")
        }

        if ((error = DropCreateStateFile(SCONTEXT, FILE_PRERSTATE)) != ERR_OK)
        {
            MC_ERROREXIT(error, "Could not create initial state file.");
        }
        return;
    }

    if (error != ERR_OK)
    {
        MC_ERROREXIT(error, "Failure during loading of existing state file.");
    }
}

static void PopulateDynamicSimulationModel(__SCONTEXT_PAR)
{
    error_t error = ERR_OK;

    if ((error = SyncDynamicModelToMainState(SCONTEXT)) != ERR_OK)
    {
        MC_ERROREXIT(error, "Data structure synchronization failed (state ==> dynamic model).");
    }

    if ((error = CalcPhysicalSimulationFactors(SCONTEXT, Get_PhysicalFactors(SCONTEXT))) != ERR_OK)
    {
        MC_ERROREXIT(error, "Failed to calculate default physical factors.");
    }
}

static error_t SyncCycleCountersWithStateStatus(__SCONTEXT_PAR)
{
    cycle_cnt_t* counters = Get_MainCycleCounters(SCONTEXT);
    hdr_info_t* stHeader = Get_MainStateHeader(SCONTEXT)->Data;
    
    counters->CurCycles = stHeader->Cycles;
    counters->CurMcs = stHeader->Mcs;

    return (counters->CurMcs < counters->TotTargetMcs) ? ERR_OK : ERR_DATACONSISTENCY;
}

static void SyncSimulationCycleStateWithModel(__SCONTEXT_PAR)
{
    error_t error;

    if ((error = CalcCycleCounterDefaultStatus(SCONTEXT, Get_MainCycleCounters(SCONTEXT))) != ERR_OK)
    {
        MC_ERROREXIT(error, "Failed to set default main counter status.");
    }

    if ((error = SyncCycleCountersWithStateStatus(SCONTEXT)) != ERR_OK)
    {
        MC_ERROREXIT(error, "Failed to synchronize data structure (state ==> cycle counters).");
    }
}

static void SyncSelectionPoolWithDynamicModel(__SCONTEXT_PAR)
{
    error_t error = ERR_OK;

    for (int32_t i = 0; i < Get_EnvironmentLattice(SCONTEXT)->Header->Size; i++)
    {
        if ((error = HandleEnvStatePoolRegistration(SCONTEXT, i)) != ERR_OK)
        {
            MC_ERROREXIT(error, "Could not register environment on the jump selection pool.");
        }
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

void PrepareContextForSimulation(__SCONTEXT_PAR, const int32_t argCount, char const * const * argValues)
{
    ResolveCommandLineArguments(SCONTEXT, argCount, argValues);
    ConstructSimulationContext(SCONTEXT);
    PopulateSimulationContext(SCONTEXT);
}
