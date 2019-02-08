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
#include "Simulator/Logic/Initializers/ContextInit/ContextInit.h"
#include "Framework/Basic/BaseTypes/BaseTypes.h"
#include "Simulator/Data/Database/DbModel.h"
#include "Simulator/Logic/Routines/Statistics/McStatistics.h"
#include "Simulator/Logic/Objects/JumpSelection.h"
#include "Simulator/Logic/Constants/Constants.h"
#include "Framework/Basic/FileIO/FileIO.h"
#include "Framework/Basic/Plugins/PluginLoading.h"
#include "Simulator/Logic/Routines/Environment/EnvRoutines.h"
#include "Simulator/Logic/Routines/Main/MainRoutines.h"
#include "Simulator/Logic/Routines/Helper/HelperRoutines.h"
#include "Simulator/Data/SimContext/ContextAccess.h"
#include "Framework/Basic/BaseTypes/Buffers.h"
#include "Simulator/Logic/Initializers/JumpStatusInit/JumpStatusInit.h"
#include "Simulator/Logic/Initializers/CmdArgResolver/CmdArgumentResolver.h"
#include "Simulator/Logic/Routines/Tracking/TransitionTracking.h"

// Allocates the environment energy and cluster buffers with the required sizes
static void AllocateEnvironmentBuffers(EnvironmentState_t *restrict env, EnvironmentDefinition_t *restrict envDef)
{
    byte_t environmentMaxParticleId = GetEnvironmentMaxParticleId(envDef);
    size_t clusterStatesSize = span_GetSize(envDef->ClusterDefinitions);
    size_t energyStatesSize = (environmentMaxParticleId == PARTICLE_NULL) ? 0 : environmentMaxParticleId + 1;

    env->EnergyStates = new_Span(env->EnergyStates, energyStatesSize);
    env->ClusterStates = new_Span(env->ClusterStates, clusterStatesSize);
}

// Allocates the the environment lattice and affiliated buffers ands sets the affiliated model pointers
static void AllocateEnvironmentLattice(__SCONTEXT_PAR)
{
    Vector4_t* sizes = getLatticeSizeVector(SCONTEXT);

    EnvironmentLattice_t lattice = new_Array(lattice, vecCoorSet4(*sizes));
    setEnvironmentLattice(SCONTEXT, lattice);

    for (int32_t i = 0; i < lattice.Header->Size;)
    {
        for (int j = 0; j < sizes->D; ++j)
        {
            EnvironmentState_t * envState = getEnvironmentStateAt(SCONTEXT, i);
            EnvironmentDefinition_t * envModel = getEnvironmentModelAt(SCONTEXT, j);
            AllocateEnvironmentBuffers(envState, envModel);
            envState->EnvironmentDefinition = envModel;
            envState->EnvironmentId = i++;
        }
    }
}

// Allocates the lattice energy buffer by the passed mmc job header
static void AllocateLatticeEnergyBuffer(Flp64Buffer_t *restrict bufferAccess, MmcHeader_t *restrict header)
{
    Buffer_t tmp = new_Span(tmp, header->AbortSequenceLength * sizeof(double));
    *bufferAccess = (Flp64Buffer_t)
        { .Begin = (void*) tmp.Begin,
          .End = (void*) tmp.Begin,
          .CapacityEnd = (void*) tmp.End,
          .LastAverage = 0.0
        };
}

// Allocates the abort condition buffers if they are required
static void AllocateAbortConditionBuffers(__SCONTEXT_PAR)
{
    if (JobInfoFlagsAreSet(SCONTEXT, INFO_FLG_MMC))
    {
        AllocateLatticeEnergyBuffer(getLatticeEnergyBuffer(SCONTEXT), getDbModelJobInfo(SCONTEXT)->JobHeader);
    }
}

// Constructs the dynamic simulation model
static void ConstructSimulationModel(__SCONTEXT_PAR)
{
    AllocateEnvironmentLattice(SCONTEXT);
    AllocateAbortConditionBuffers(SCONTEXT);
}

// Constructs the selection pool index redirection that redirects jump counts to selection pool id
static error_t ConstructSelectionPoolIndexRedirection(__SCONTEXT_PAR)
{
    int32_t maxPoolCount = 1 + FindMaxJumpDirectionCount(&getDbTransitionModel(SCONTEXT)->JumpCountMappingTable);
    int32_t poolIndex = 1;

    IdRedirection_t poolMapping = new_Span(poolMapping, maxPoolCount);
    setDirectionPoolMapping(SCONTEXT, poolMapping);

    cpp_foreach(dirCount, getDbTransitionModel(SCONTEXT)->JumpCountMappingTable)
    {
        if ((*dirCount > JPOOL_DIRCOUNT_PASSIVE) && (getDirectionPoolIdByJumpCount(SCONTEXT, *dirCount) == 0))
        {
            setDirectionPoolIdByJumpCount(SCONTEXT, *dirCount, poolIndex);
            poolIndex++;
        }
    }

    getJumpSelectionPool(SCONTEXT)->DirectionPoolCount = poolIndex;
    return ERR_OK;
}

// Construct the selection pool direction buffers
static error_t ConstructSelectionPoolDirectionBuffers(__SCONTEXT_PAR)
{
    int32_t poolCount = getJumpSelectionPool(SCONTEXT)->DirectionPoolCount;
    int32_t poolSize = getNumberOfSelectables(SCONTEXT);

    DirectionPools_t directionPools = new_Span(directionPools, poolCount);
    setDirectionPools(SCONTEXT, directionPools);

    cpp_foreach(dirPool, directionPools)
    {
        EnvironmentPool_t envPool = new_List(envPool, poolSize);
        dirPool->EnvironmentPool = envPool;
    }

    int32_t jumpCount = 0;
    cpp_foreach(id, *getDirectionPoolMapping(SCONTEXT))
    {
        if (*id > 0) span_Get(directionPools, *id).NumOfDirections = jumpCount;
        jumpCount++;
    }

    return ERR_OK;
}

// Construct the jump selection pool on the simulation context
static void ConstructJumpSelectionPool(__SCONTEXT_PAR)
{
    error_t error;

    error = ConstructSelectionPoolIndexRedirection(SCONTEXT);
    error_assert(error, "Failed to construct selection pool indexing information.");

    error = ConstructSelectionPoolDirectionBuffers(SCONTEXT);
    error_assert(error, "Failed to construct selection pool direction buffers.");
}

// Get the number of bytes the state header requires
static inline int32_t GetStateHeaderDataSize(__SCONTEXT_PAR)
{
    return (int32_t) sizeof(StateHeaderData_t);
}

// Configure the state header access address and return the number of used buffer bytes
static int32_t ConfigStateHeaderAccess(__SCONTEXT_PAR)
{
    getMainStateHeader(SCONTEXT)->Data = getMainStateBufferAddress(SCONTEXT, 0);
    return GetStateHeaderDataSize(SCONTEXT);
}

// Get the number of bytes the state meta data requires
static inline int32_t GetStateMetaDataSize(__SCONTEXT_PAR)
{
    return (int32_t) sizeof(StateMetaData_t);
}

// Configure the state meta access address and return the new number of used buffer bytes
static int32_t ConfigStateMetaAccess(__SCONTEXT_PAR, const int32_t usedBufferBytes)
{
    int32_t cfgBufferBytes = GetStateMetaDataSize(SCONTEXT);

    getMainStateHeader(SCONTEXT)->Data->MetaStartByte = usedBufferBytes;   
    getMainStateMetaInfo(SCONTEXT)->Data = getMainStateBufferAddress(SCONTEXT, usedBufferBytes);

    return usedBufferBytes + cfgBufferBytes;
}

// Get the number of bytes the state lattice data requires
static inline int32_t GetStateLatticeDataSize(__SCONTEXT_PAR)
{
    return getDbLatticeModel(SCONTEXT)->Lattice.Header->Size;
}

// Configure the state lattice access address and return the new number of used buffer bytes
static int32_t ConfigStateLatticeAccess(__SCONTEXT_PAR, const int32_t usedBufferBytes)
{
    LatticeState_t* configObject = getMainStateLattice(SCONTEXT);
    int32_t cfgBufferBytes = GetStateLatticeDataSize(SCONTEXT);
    
    getMainStateHeader(SCONTEXT)->Data->LatticeStartByte = usedBufferBytes;
    configObject->Begin = getMainStateBufferAddress(SCONTEXT, usedBufferBytes);
    configObject->End = configObject->Begin + cfgBufferBytes;

    return usedBufferBytes + cfgBufferBytes;
}

// Get the number of bytes the state counters data requires
static inline int32_t GetStateCountersDataSize(__SCONTEXT_PAR)
{
    return sizeof(StateCounterCollection_t) * (int32_t) (GetMaxParticleId(SCONTEXT) + 1);
}

// Configure the state counter access address and return the new number of used buffer bytes
static int32_t ConfigStateCountersAccess(__SCONTEXT_PAR, const int32_t usedBufferBytes)
{
    CountersState_t* configObject = getMainStateCounters(SCONTEXT);
    int32_t cfgBufferBytes = GetStateCountersDataSize(SCONTEXT);
    
    getMainStateHeader(SCONTEXT)->Data->CountersStartByte = usedBufferBytes;
    configObject->Begin = getMainStateBufferAddress(SCONTEXT, usedBufferBytes);
    configObject->End = configObject->Begin + (cfgBufferBytes / sizeof(StateCounterCollection_t));

    return usedBufferBytes + cfgBufferBytes;
}

// Get the number of bytes the state global tracker data requires
static inline int32_t GetStateGlobalTrackerDataSize(__SCONTEXT_PAR)
{
    if (JobInfoFlagsAreSet(SCONTEXT, INFO_FLG_KMC))
    {
        return getDbStructureModel(SCONTEXT)->NumOfGlobalTrackers * sizeof(Tracker_t);
    }
    return 0;
}

// Configure the state global tracking access address and return the new number of used buffer bytes
static int32_t ConfigStateGlobalTrackerAccess(__SCONTEXT_PAR, const int32_t usedBufferBytes)
{
    int32_t cfgBufferBytes = GetStateGlobalTrackerDataSize(SCONTEXT);
    getMainStateHeader(SCONTEXT)->Data->GlobalTrackerStartByte = JobInfoFlagsAreSet(SCONTEXT, INFO_FLG_KMC) ? usedBufferBytes : -1;

    if (JobHeaderFlagsAreSet(SCONTEXT, INFO_FLG_KMC))
    {
        TrackersState_t* configObject = getGlobalMovementTrackers(SCONTEXT);

        configObject->Begin = getMainStateBufferAddress(SCONTEXT, usedBufferBytes);
        configObject->End = configObject->Begin + getDbStructureModel(SCONTEXT)->NumOfGlobalTrackers;
    }

    return usedBufferBytes + cfgBufferBytes;
}

// Get the number of bytes the state mobile tracker data requires
static inline int32_t GetStateMobileTrackerDataSize(__SCONTEXT_PAR)
{
    if (JobInfoFlagsAreSet(SCONTEXT, INFO_FLG_KMC))
    {
        return getNumberOfMobiles(SCONTEXT) * sizeof(Tracker_t);
    }
    return 0;
}

// Configure the state mobile tracking access address and return the new number of used buffer bytes
static int32_t ConfigStateMobileTrackerAccess(__SCONTEXT_PAR, const int32_t usedBufferBytes)
{
    int32_t cfgBufferBytes = GetStateMobileTrackerDataSize(SCONTEXT);
    getMainStateHeader(SCONTEXT)->Data->MobileTrackerStartByte = JobInfoFlagsAreSet(SCONTEXT, INFO_FLG_KMC) ? usedBufferBytes : -1;

    if (JobInfoFlagsAreSet(SCONTEXT, INFO_FLG_KMC))
    {
        TrackersState_t* configObject = getMobileMovementTrackers(SCONTEXT);

        configObject->Begin = getMainStateBufferAddress(SCONTEXT, usedBufferBytes);
        configObject->End = configObject->Begin + getNumberOfMobiles(SCONTEXT);
    }

    return usedBufferBytes + cfgBufferBytes;
}

// Get the number of bytes the state static tracker data requires
static inline int32_t GetStateStaticTrackerDataSize(__SCONTEXT_PAR)
{
    if (JobInfoFlagsAreSet(SCONTEXT, INFO_FLG_KMC))
    {
        return getDbStructureModel(SCONTEXT)->NumOfTrackersPerCell * GetUnitCellCount(SCONTEXT) * sizeof(Tracker_t);
    }
    return 0;
}

// Configure the state static tracking access address and return the new number of used buffer bytes
static int32_t ConfigStateStaticTrackerAccess(__SCONTEXT_PAR, const int32_t usedBufferBytes)
{
    int32_t cfgBufferBytes = GetStateStaticTrackerDataSize(SCONTEXT);
    getMainStateHeader(SCONTEXT)->Data->StaticTrackerStartByte = JobInfoFlagsAreSet(SCONTEXT, INFO_FLG_KMC) ? usedBufferBytes : -1;

    if (JobInfoFlagsAreSet(SCONTEXT, INFO_FLG_KMC))
    {
        TrackersState_t* configObject = getStaticMovementTrackers(SCONTEXT);

        configObject->Begin = getMainStateBufferAddress(SCONTEXT, usedBufferBytes);
        configObject->End = configObject->Begin + (getDbStructureModel(SCONTEXT)->NumOfTrackersPerCell * GetUnitCellCount(SCONTEXT));
    }

    return usedBufferBytes + cfgBufferBytes;
}

// Get the number of bytes the state mobile tracker mapping data requires
static inline int32_t GetStateMobileTrackerMappingDataSize(__SCONTEXT_PAR)
{
    if (JobInfoFlagsAreSet(SCONTEXT, INFO_FLG_KMC))
    {
        return getNumberOfMobiles(SCONTEXT) * sizeof(int32_t);
    }
    return 0;
}

// Configure the state mobile tracking mapping access address and return the new number of used buffer bytes
static int32_t ConfigStateMobileTrackerMappingAccess(__SCONTEXT_PAR, const int32_t usedBufferBytes)
{
    int32_t cfgBufferBytes = GetStateMobileTrackerMappingDataSize(SCONTEXT);
    getMainStateHeader(SCONTEXT)->Data->MobileTrackerIdxStartByte = JobInfoFlagsAreSet(SCONTEXT, INFO_FLG_KMC) ? usedBufferBytes : -1;

    if (JobInfoFlagsAreSet(SCONTEXT, INFO_FLG_KMC))
    {
        MobileTrackerMapping_t* configObject = getMobileTrackerMapping(SCONTEXT);

        configObject->Begin = getMainStateBufferAddress(SCONTEXT, usedBufferBytes);
        configObject->End = configObject->Begin + getNumberOfMobiles(SCONTEXT);
    }

    return usedBufferBytes + cfgBufferBytes;
}

// Get the number of bytes the state jump statistics data requires
static inline int32_t GetStateJumpStatisticsDataSize(__SCONTEXT_PAR)
{
    if (JobInfoFlagsAreSet(SCONTEXT, INFO_FLG_KMC))
    {
        return getDbStructureModel(SCONTEXT)->NumOfGlobalTrackers * sizeof(JumpStatistic_t);
    }
    return 0;
}

// Configure the state jump probability tracking access address and return the new number of used buffer bytes
static int32_t ConfigStateJumpStatisticsAccess(__SCONTEXT_PAR, const int32_t usedBufferBytes)
{
    int32_t cfgBufferBytes = GetStateJumpStatisticsDataSize(SCONTEXT);
    getMainStateHeader(SCONTEXT)->Data->JumpStatisticsStartByte = JobInfoFlagsAreSet(SCONTEXT, INFO_FLG_KMC) ? usedBufferBytes : -1;

    if (JobInfoFlagsAreSet(SCONTEXT, INFO_FLG_KMC))
    {
        JumpStatisticsState_t* configObject = getJumpStatistics(SCONTEXT);

        configObject->Begin = getMainStateBufferAddress(SCONTEXT, usedBufferBytes);
        configObject->End = configObject->Begin + getDbStructureModel(SCONTEXT)->NumOfGlobalTrackers;
    }

    return usedBufferBytes + cfgBufferBytes;
}

// Construct the main state buffer accessor system
static error_t ConstructMainStateBufferAccessors(__SCONTEXT_PAR)
{
    int32_t usedBufferBytes = 0;

    usedBufferBytes = ConfigStateHeaderAccess(SCONTEXT);
    usedBufferBytes = ConfigStateMetaAccess(SCONTEXT, usedBufferBytes);
    usedBufferBytes = ConfigStateLatticeAccess(SCONTEXT, usedBufferBytes);
    usedBufferBytes = ConfigStateCountersAccess(SCONTEXT, usedBufferBytes);
    usedBufferBytes = ConfigStateGlobalTrackerAccess(SCONTEXT, usedBufferBytes);
    usedBufferBytes = ConfigStateMobileTrackerAccess(SCONTEXT, usedBufferBytes);
    usedBufferBytes = ConfigStateStaticTrackerAccess(SCONTEXT, usedBufferBytes);
    usedBufferBytes = ConfigStateMobileTrackerMappingAccess(SCONTEXT, usedBufferBytes);
    usedBufferBytes = ConfigStateJumpStatisticsAccess(SCONTEXT, usedBufferBytes);

    return (usedBufferBytes == span_GetSize(*getMainStateBuffer(SCONTEXT))) ? ERR_OK : ERR_DATACONSISTENCY;
}

// Calculates the required size in bytes for the main simulation state buffer
static int32_t CalculateMainStateBufferSize(__SCONTEXT_PAR)
{
    int32_t size = 0;

    size += GetStateHeaderDataSize(SCONTEXT);
    size += GetStateMetaDataSize(SCONTEXT);
    size += GetStateLatticeDataSize(SCONTEXT);
    size += GetStateCountersDataSize(SCONTEXT);
    size += GetStateGlobalTrackerDataSize(SCONTEXT);
    size += GetStateMobileTrackerDataSize(SCONTEXT);
    size += GetStateStaticTrackerDataSize(SCONTEXT);
    size += GetStateMobileTrackerMappingDataSize(SCONTEXT);
    size += GetStateJumpStatisticsDataSize(SCONTEXT);

    return size;
}

// Construct the simulation main state on the simulation context
static void ConstructMainState(__SCONTEXT_PAR)
{
    error_t error;

    memset(getSimulationState(SCONTEXT), 0, sizeof(SimulationState_t));

    getDbModelJobInfo(SCONTEXT)->StateSize = CalculateMainStateBufferSize(SCONTEXT);
    size_t stateSize = (size_t) getDbModelJobInfo(SCONTEXT)->StateSize;

    Buffer_t stateBuffer = new_Span(stateBuffer, stateSize);
    *getMainStateBuffer(SCONTEXT) = stateBuffer;

    error = ConstructMainStateBufferAccessors(SCONTEXT);
    error_assert(error, "Failed to construct main state buffer accessor system.");
}

// Construct the components of the simulation context
void ConstructSimulationContext(__SCONTEXT_PAR)
{
    ConstructSimulationModel(SCONTEXT);
    ConstructMainState(SCONTEXT);
    ConstructJumpSelectionPool(SCONTEXT);
}

// Tries to load the output plugin if it is defined on the simulation context and set it on the plugin collection
static error_t TryLoadOutputPlugin(__SCONTEXT_PAR)
{
    error_t error = ERR_OK;
    FileInfo_t* fileInfo = getFileInformation(SCONTEXT);

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

// Tries to load the energy plugin if it is defined on the simulation context and set it on the plugin collection
static error_t TryLoadEnergyPlugin(__SCONTEXT_PAR)
{
    error_t error = ERR_OK;
    FileInfo_t* fileInfo = getFileInformation(SCONTEXT);

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

// Sets the energy plugin function to the internal default function
static inline void SetEnergyPluginFunctionToDefault(__SCONTEXT_PAR)
{
    if (JobInfoFlagsAreSet(SCONTEXT, INFO_FLG_KMC))
    {
        getPluginCollection(SCONTEXT)->OnSetJumpProbabilities = (FPlugin_t) SetKmcJumpProbabilities;
    }
    else
    {
        getPluginCollection(SCONTEXT)->OnSetJumpProbabilities = (FPlugin_t) SetMmcJumpProbabilities;
    }
}

// Set the output plugin function to the internal default
static inline void SetOutputPluginFunctionToDefault(__SCONTEXT_PAR)
{
    getPluginCollection(SCONTEXT)->OnDataOutput = NULL;
}

// Populates the plugin delegates to either loadable plugins or the internal default
static void PopulatePluginDelegates(__SCONTEXT_PAR)
{
    if (TryLoadOutputPlugin(SCONTEXT) == ERR_USEDEFAULT)
    {
        SetOutputPluginFunctionToDefault(SCONTEXT);
    }

    if (TryLoadEnergyPlugin(SCONTEXT) == ERR_USEDEFAULT)
    {
        SetEnergyPluginFunctionToDefault(SCONTEXT);
    }
}

// Tries to load a simulation main state from the passed file path
static error_t TryLoadStateFromFile(__SCONTEXT_PAR, char const * restrict filePath)
{
    return_if(!IsAccessibleFile(filePath), ERR_USEDEFAULT);
    return LoadBufferFromFile(filePath, getMainStateBuffer(SCONTEXT));
}

// Drop creates a state file by ensuring that the original is deleted
static error_t DropCreateStateFile(__SCONTEXT_PAR, char const * restrict filePath)
{
    EnsureFileIsDeleted(filePath);
    return WriteBufferToFile(filePath, FMODE_BINARY_W, getMainStateBuffer(SCONTEXT));
}

// Tries to load the simulation state from the typical possible save locations
static error_t TryLoadSimulationState(__SCONTEXT_PAR)
{
    error_t error;

    if ((error = TryLoadStateFromFile(SCONTEXT, FILE_MAINSTATE)) == ERR_OK)
    {
        EnsureFileIsDeleted(FILE_PRERSTATE);
        return error;
    }

    error = TryLoadStateFromFile(SCONTEXT, FILE_PRERSTATE);
    return error;
}

// Copies the database random number generator seed information to the main simulation state
static error_t CopyDbRngInfoToMainState(__SCONTEXT_PAR)
{
    getMainStateMetaData(SCONTEXT)->RngState = getDbModelJobInfo(SCONTEXT)->RngStateSeed;
    getMainStateMetaData(SCONTEXT)->RngIncrease = getDbModelJobInfo(SCONTEXT)->RngIncSeed;
    return ((getMainStateMetaData(SCONTEXT)->RngIncrease & 1) != 0) ? ERR_OK : ERR_DATACONSISTENCY;
}

// Copies the database lattice information to the simulation main state
static error_t CopyDbLatticeToMainState(__SCONTEXT_PAR)
{
    Lattice_t * dbLattice = getDbModelLattice(SCONTEXT);
    LatticeState_t * stLattice = getMainStateLattice(SCONTEXT);
    size_t latticeSize = span_GetSize(*stLattice);

    return_if(latticeSize != dbLattice->Header->Size, ERR_DATACONSISTENCY);

    CopyBuffer(dbLattice->Begin, stLattice->Begin, latticeSize);
    return ERR_OK;
}

// Translates the db lattice data into a mobile tracker id mapping on the state
static error_t CopyDefaultMobileTrackersToMainState(__SCONTEXT_PAR)
{
    Lattice_t * dbLattice = getDbModelLattice(SCONTEXT);
    MobileTrackerMapping_t * mapping = getMobileTrackerMapping(SCONTEXT);
    int32_t trackerId = 0;

    cpp_foreach(envState, *getEnvironmentLattice(SCONTEXT))
    {
        byte_t particleId = span_Get(*dbLattice, envState->EnvironmentId);
        int32_t jumpCount = getJumpCountAt(SCONTEXT, envState->EnvironmentDefinition->ObjectId, particleId);
        if ((jumpCount >= JPOOL_DIRCOUNT_PASSIVE) && (particleId != PARTICLE_VOID))
        {
            envState->MobileTrackerId = trackerId;
            span_Get(*mapping, trackerId) = envState->EnvironmentId;
            trackerId++;
        }
    }

    return (trackerId == getNumberOfMobiles(SCONTEXT)) ? ERR_OK : ERR_DATACONSISTENCY;
}

// Sets all default flags on a new state when none could be loaded from file
static void SetMainStateFlagsToStartConditions(__SCONTEXT_PAR)
{
    setMainStateFlags(SCONTEXT, STATE_FLG_FIRSTCYCLE);
}


// Synchronizes the main state to the database model by overwriting existing information in the state
static error_t SyncMainStateToDatabaseModel(__SCONTEXT_PAR)
{
    error_t error;

    error = CopyDbLatticeToMainState(SCONTEXT);
    return_if(error, error);

    error = CopyDbRngInfoToMainState(SCONTEXT);
    return_if(error, error);

    error = CopyDefaultMobileTrackersToMainState(SCONTEXT);
    return_if(error, error);

    error = InitJumpStatisticsTrackingSystem(SCONTEXT);
    SetMainStateFlagsToStartConditions(SCONTEXT);

    return error;
}

// Synchronizes the dynamic environment lattice with the main simulation state
static error_t SyncDynamicEnvironmentsWithState(__SCONTEXT_PAR)
{
    EnvironmentLattice_t* envLattice = getEnvironmentLattice(SCONTEXT);
    LatticeState_t* stLattice = getMainStateLattice(SCONTEXT);
    size_t latticeSize = span_GetSize(*stLattice);

    return_if(envLattice->Header->Size != latticeSize, ERR_DATACONSISTENCY);

    for (int32_t i = 0; i < latticeSize; i++)
    {
        SetEnvStateStatusToDefault(SCONTEXT, i, getStateLatticeEntryAt(SCONTEXT, i));
    }

    return ERR_OK;
}

// Synchronizes the mobile tracker information of the dynamic lattice with the mapping data from the main state
static error_t SyncMobileTrackersWithState(__SCONTEXT_PAR)
{
    int32_t trackerId = 0;
    cpp_foreach(environmentId, getSimulationState(SCONTEXT)->MobileTrackerMapping)
    {
        EnvironmentState_t* envState = getEnvironmentStateAt(SCONTEXT, *environmentId);
        envState->MobileTrackerId = trackerId++;
    }

    return (trackerId == getNumberOfMobiles(SCONTEXT)) ? ERR_OK : ERR_DATACONSISTENCY;
}

// Synchronizes the dynamic model to the main simulation state
static error_t SyncDynamicModelToMainState(__SCONTEXT_PAR)
{
    // ToDo: Potentially incomplete sync. review during testing
    error_t error = SyncDynamicEnvironmentsWithState(SCONTEXT);
    return_if(error, error);

    error = SyncMobileTrackersWithState(SCONTEXT);
    return error;
}

// Populates the constructed simulation state with the required run information
static void PopulateSimulationState(__SCONTEXT_PAR)
{
    error_t error;

    // ToDo: Remove deletes after testing
    EnsureFileIsDeleted(FILE_PRERSTATE);
    EnsureFileIsDeleted(FILE_MAINSTATE);

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

// Populates the constructed dynamic simulation model with the required run information
static void PopulateDynamicSimulationModel(__SCONTEXT_PAR)
{
    error_t error;

    error = SyncDynamicModelToMainState(SCONTEXT);
    error_assert(error, "Data structure synchronization failed (state ==> dynamic model).");
}

// Synchronizes the cycle counters of the dynamic state with the info from the main simulation state
static error_t SyncCycleCountersWithStateStatus(__SCONTEXT_PAR)
{
    CycleCounterState_t* counters = getMainCycleCounters(SCONTEXT);
    StateHeaderData_t* stHeader = getMainStateHeader(SCONTEXT)->Data;
    
    counters->Cycles = stHeader->Cycles;
    counters->Mcs = stHeader->Mcs;

    return (counters->Mcs < counters->TotalGoalMcs) ? ERR_OK : ERR_DATACONSISTENCY;
}

// Synchronizes the simulation cycle state to the main simulation state
static void SyncSimulationCycleStateWithModel(__SCONTEXT_PAR)
{
    error_t error;

    error = CalcCycleCounterDefaultStatus(SCONTEXT, getMainCycleCounters(SCONTEXT));
    error_assert(error, "Failed to set default main counter status.");

    error = SyncCycleCountersWithStateStatus(SCONTEXT);
    error_assert(error, "Failed to synchronize data structure (state ==> cycle counters).");
}

// Synchronizes the selection pool with the prepared dynamic simulation model
static void SyncSelectionPoolWithDynamicModel(__SCONTEXT_PAR)
{
    error_t error;

    for (int32_t i = 0; i < getEnvironmentLattice(SCONTEXT)->Header->Size; i++)
    {
        error = HandleEnvStatePoolRegistration(SCONTEXT, i);
        error_assert(error, "Could not register environment on the jump selection pool.");
    }
}

// Converts all energy values in pair and cluster tables from [eV] to units of [kT]
static error_t ConvertEnergyTablesToInternalUnits(__SCONTEXT_PAR)
{
    double conversionFactor = getPhysicalFactors(SCONTEXT)->EnergyConversionFactor;
    return_if(!isfinite(conversionFactor) || (conversionFactor <  0), ERR_DATACONSISTENCY);

    cpp_foreach(table, *getPairEnergyTables(SCONTEXT))
        cpp_foreach(value, table->EnergyTable)
            *value *= conversionFactor;


    cpp_foreach(table, *getClusterEnergyTables(SCONTEXT))
        cpp_foreach(value, table->EnergyTable)
            *value *= conversionFactor;

    return ERR_OK;
}

// Synchronizes the physical simulation with the loaded db data and makes required data corrections to the input data
static void SyncPhysicalParametersAndEnergyTables(__SCONTEXT_PAR)
{
    error_t error;

    error = CalcPhysicalSimulationFactors(SCONTEXT, getPhysicalFactors(SCONTEXT));
    error_assert(error, "Failed to calculate default physical factors.");

    error = ConvertEnergyTablesToInternalUnits(SCONTEXT);
    error_assert(error, "Failed to convert energy tables to internal units");
}

// Initializes the random number generator from the main state seed information
static void PopulateRngFromMainState(__SCONTEXT_PAR)
{
    SCONTEXT->Rng = (Pcg32_t) { getMainStateMetaData(SCONTEXT)->RngState, getMainStateMetaData(SCONTEXT)->RngIncrease };
}

// Populates a freshly constructed simulation context with the required runtime information
static void PopulateSimulationContext(__SCONTEXT_PAR)
{
    PopulatePluginDelegates(SCONTEXT);
    PopulateSimulationState(SCONTEXT);
    PopulateDynamicSimulationModel(SCONTEXT);
    SyncSimulationCycleStateWithModel(SCONTEXT);
    SyncSelectionPoolWithDynamicModel(SCONTEXT);
    SyncPhysicalParametersAndEnergyTables(SCONTEXT);
    PopulateRngFromMainState(SCONTEXT);
}

void PrepareContextForSimulation(__SCONTEXT_PAR)
{
    ConstructSimulationContext(SCONTEXT);
    PopulateSimulationContext(SCONTEXT);

    BuildEnvironmentLinkingSystem(SCONTEXT);
    BuildJumpStatusCollection(SCONTEXT);
    SyncEnvironmentEnergyStatus(SCONTEXT);
}
