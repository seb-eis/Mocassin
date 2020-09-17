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
#include "SimulationContextInitialization.h"
#include "Libraries/Simulator/Logic/Routines/Statistics/McStatistics.h"
#include "Libraries/Simulator/Logic/JumpSelection/JumpSelection.h"
#include "Libraries/Framework/Basic/DlLoading/DlLoading.h"
#include "Libraries/Simulator/Logic/Routines/Environment/EnvRoutines.h"
#include "Libraries/Simulator/Logic/Routines/Main/MainRoutines.h"
#include "Libraries/Simulator/Logic/Routines/Helper/HelperRoutines.h"
#include "Libraries/Simulator/Logic/Initializers/JumpStatusInit/JumpStatusInititialization.h"
#include "Libraries/Simulator/Logic/Routines/Tracking/TransitionTracking.h"

// Allocates the environment energy and cluster buffers with the required sizes
static void AllocateEnvironmentBuffers(EnvironmentState_t *restrict env, EnvironmentDefinition_t *restrict envDef)
{
    let environmentMaxParticleId = GetEnvironmentMaxParticleId(envDef);
    let clusterStatesSize = span_Length(envDef->ClusterInteractions);
    let energyStatesSize = (environmentMaxParticleId == PARTICLE_NULL) ? 0 : environmentMaxParticleId + 1;

    env->EnergyStates = span_New(env->EnergyStates, energyStatesSize);
    env->ClusterStates = span_New(env->ClusterStates, clusterStatesSize);
}

// Allocates the the environment lattice and affiliated buffers ands sets the affiliated model pointers
static void AllocateEnvironmentLattice(SCONTEXT_PARAMETER)
{
    let sizes = getLatticeSizeVector(simContext);
    var lattice = getEnvironmentLattice(simContext);
    *lattice = array_New(*lattice, vecCoorSet4(*sizes));

    for (int32_t i = 0; i < lattice->Header->Size;)
    {
        for (int32_t j = 0; j < sizes->D; ++j)
        {
            let envModel = getEnvironmentModelAt(simContext, j);
            var envState = getEnvironmentStateAt(simContext, i);
            AllocateEnvironmentBuffers(envState, envModel);

            // Premature ID assignment required for further allocation/construction routines
            envState->EnvironmentDefinition = envModel;
            i++;
        }
    }
}

// Allocates the lattice energy buffer by the passed mmc job header
static void AllocateEnergyFluctuationAbortBuffer(Flp64Buffer_t *restrict bufferAccess, MmcHeader_t *restrict header)
{
    Buffer_t tmp = span_New(tmp, header->AbortSampleLength * sizeof(double));
    *bufferAccess = (Flp64Buffer_t)
    {
        .Begin = (void*) tmp.Begin,
        .End = (void*) tmp.Begin,
        .CapacityEnd = (void*) tmp.End,
        .LastSum = INFINITY,
        .CurrentSum = INFINITY
    };
}

// Allocates the abort condition buffers if they are required
static void AllocateAbortConditionBuffers(SCONTEXT_PARAMETER)
{
    return_if(!JobInfoFlagsAreSet(simContext, INFO_FLG_MMC));

    let jobInfo = getDbModelJobInfo(simContext);
    var energyBuffer = getLatticeEnergyBuffer(simContext);

    AllocateEnergyFluctuationAbortBuffer(energyBuffer, jobInfo->JobHeader);
}

// Constructs the dynamic simulation model
static void ConstructSimulationModel(SCONTEXT_PARAMETER)
{
    AllocateEnvironmentLattice(simContext);
    AllocateAbortConditionBuffers(simContext);
}

// Constructs the selection pool index redirection that redirects jump counts to selection pool id
static error_t ConstructSelectionPoolIndexRedirection(SCONTEXT_PARAMETER)
{
    let transitionModel = getDbTransitionModel(simContext);
    let maxPoolCount = 1 + FindMaxJumpDirectionCount(&transitionModel->JumpCountMappingTable);
    var poolMapping = getDirectionPoolMapping(simContext);
    var selectionPool = getJumpSelectionPool(simContext);
    int32_t poolIndex = 1;

    *poolMapping = span_New(*poolMapping, maxPoolCount);
    cpp_foreach(dirCount, transitionModel->JumpCountMappingTable)
    {
        if ((*dirCount > JPOOL_DIRCOUNT_PASSIVE) && (getDirectionPoolIdByJumpCount(simContext, *dirCount) == 0))
        {
            setDirectionPoolIdByJumpCount(simContext, *dirCount, poolIndex);
            poolIndex++;
        }
    }
    selectionPool->DirectionPoolCount = poolIndex;
    return ERR_OK;
}

// Construct the selection pool direction buffers
static error_t ConstructSelectionPoolDirectionBuffers(SCONTEXT_PARAMETER)
{
    let poolCount = getJumpSelectionPool(simContext)->DirectionPoolCount;
    let poolSize = getNumberOfSelectables(simContext);
    let poolMapping = getDirectionPoolMapping(simContext);
    var directionPools = getDirectionPools(simContext);

    *directionPools = span_New(*directionPools, poolCount);
    cpp_foreach(dirPool, *directionPools)
        dirPool->EnvironmentPool = list_New(dirPool->EnvironmentPool, poolSize);

    int32_t jumpCount = 0;
    cpp_foreach(id, *poolMapping)
    {
        if (*id > 0) span_Get(*directionPools, *id).DirectionCount = jumpCount;
        jumpCount++;
    }
    return ERR_OK;
}

// Construct the jump selection pool on the simulation context
static void ConstructJumpSelectionPool(SCONTEXT_PARAMETER)
{
    var error = ConstructSelectionPoolIndexRedirection(simContext);
    assert_success(error, "Failed to construct selection pool indexing information.");

    error = ConstructSelectionPoolDirectionBuffers(simContext);
    assert_success(error, "Failed to construct selection pool direction buffers.");
}

// Get the number of bytes the state header requires
static inline int32_t GetStateHeaderDataSize(SCONTEXT_PARAMETER)
{
    return (int32_t) sizeof(StateHeaderData_t);
}

// Configure the state header access address and return the number of used buffer bytes
static int32_t ConfigStateHeaderAccess(SCONTEXT_PARAMETER)
{
    var stateHeader = getMainStateHeader(simContext);
    stateHeader->Data = getMainStateBufferAddress(simContext, 0);
    return GetStateHeaderDataSize(simContext);
}

// Get the number of bytes the state meta data requires
static inline int32_t GetStateMetaDataSize(SCONTEXT_PARAMETER)
{
    return (int32_t) sizeof(StateMetaData_t);
}

// Configure the state meta access address and return the new number of used buffer bytes
static int32_t ConfigStateMetaAccess(SCONTEXT_PARAMETER, const int32_t usedBufferBytes)
{
    let cfgBufferBytes = GetStateMetaDataSize(simContext);
    var headerData = getMainStateHeader(simContext)->Data;
    var metaInfo = getMainStateMetaInfo(simContext);

    headerData->MetaStartByte = usedBufferBytes;
    metaInfo->Data = getMainStateBufferAddress(simContext, usedBufferBytes);

    return usedBufferBytes + cfgBufferBytes;
}

// Get the number of bytes the state lattice data requires
static inline int32_t GetStateLatticeDataSize(SCONTEXT_PARAMETER)
{
    let latticeModel = getDbLatticeModel(simContext);
    return latticeModel->Lattice.Header->Size;
}

// Configure the state lattice access address and return the new number of used buffer bytes
static int32_t ConfigStateLatticeAccess(SCONTEXT_PARAMETER, const int32_t usedBufferBytes)
{
    var configObject = getMainStateLattice(simContext);
    let cfgBufferBytes = GetStateLatticeDataSize(simContext);
    
    getMainStateHeader(simContext)->Data->LatticeStartByte = usedBufferBytes;
    configObject->Begin = getMainStateBufferAddress(simContext, usedBufferBytes);
    configObject->End = configObject->Begin + cfgBufferBytes;

    return usedBufferBytes + cfgBufferBytes;
}

// Get the number of bytes the state counters data requires
static inline int32_t GetStateCountersDataSize(SCONTEXT_PARAMETER)
{
    return sizeof(StateCounterCollection_t) * (int32_t) (GetMaxParticleId(simContext) + 1);
}

// Configure the state counter access address and return the new number of used buffer bytes
static int32_t ConfigStateCountersAccess(SCONTEXT_PARAMETER, const int32_t usedBufferBytes)
{
    var configObject = getMainStateCounters(simContext);
    let cfgBufferBytes = GetStateCountersDataSize(simContext);
    
    getMainStateHeader(simContext)->Data->CountersStartByte = usedBufferBytes;
    configObject->Begin = getMainStateBufferAddress(simContext, usedBufferBytes);
    configObject->End = configObject->Begin + (cfgBufferBytes / sizeof(StateCounterCollection_t));

    return usedBufferBytes + cfgBufferBytes;
}

// Get the number of bytes the state global tracker data requires
static inline int32_t GetStateGlobalTrackerDataSize(SCONTEXT_PARAMETER)
{
    if (JobInfoFlagsAreSet(simContext, INFO_FLG_KMC))
        return getDbStructureModel(simContext)->GlobalTrackerCount * sizeof(Tracker_t);

    return 0;
}

// Configure the state global tracking access address and return the new number of used buffer bytes
static int32_t ConfigStateGlobalTrackerAccess(SCONTEXT_PARAMETER, const int32_t usedBufferBytes)
{
    let cfgBufferBytes = GetStateGlobalTrackerDataSize(simContext);
    getMainStateHeader(simContext)->Data->GlobalTrackerStartByte = JobInfoFlagsAreSet(simContext, INFO_FLG_KMC) ? usedBufferBytes : -1;

    return_if(cfgBufferBytes == 0, usedBufferBytes);

    var configObject = getGlobalMovementTrackers(simContext);

    configObject->Begin = getMainStateBufferAddress(simContext, usedBufferBytes);
    configObject->End = configObject->Begin + getDbStructureModel(simContext)->GlobalTrackerCount;

    return usedBufferBytes + cfgBufferBytes;
}

// Get the number of bytes the state mobile tracker data requires
static inline int32_t GetStateMobileTrackerDataSize(SCONTEXT_PARAMETER)
{
    if (JobInfoFlagsAreSet(simContext, INFO_FLG_KMC))
        return getNumberOfMobiles(simContext) * sizeof(Tracker_t);

    return 0;
}

// Configure the state mobile tracking access address and return the new number of used buffer bytes
static int32_t ConfigStateMobileTrackerAccess(SCONTEXT_PARAMETER, const int32_t usedBufferBytes)
{
    let cfgBufferBytes = GetStateMobileTrackerDataSize(simContext);
    getMainStateHeader(simContext)->Data->MobileTrackerStartByte = JobInfoFlagsAreSet(simContext, INFO_FLG_KMC) ? usedBufferBytes : -1;

    return_if (cfgBufferBytes == 0, usedBufferBytes);

    var configObject = getMobileMovementTrackers(simContext);

    configObject->Begin = getMainStateBufferAddress(simContext, usedBufferBytes);
    configObject->End = configObject->Begin + getNumberOfMobiles(simContext);

    return usedBufferBytes + cfgBufferBytes;
}

// Get the number of bytes the state static tracker data requires
static inline int32_t GetStateStaticTrackerDataSize(SCONTEXT_PARAMETER)
{
    if (JobInfoFlagsAreSet(simContext, INFO_FLG_KMC))
        return getDbStructureModel(simContext)->StaticTrackersPerCellCount * GetUnitCellCount(simContext) * sizeof(Tracker_t);

    return 0;
}

// Configure the state static tracking access address and return the new number of used buffer bytes
static int32_t ConfigStateStaticTrackerAccess(SCONTEXT_PARAMETER, const int32_t usedBufferBytes)
{
    let cfgBufferBytes = GetStateStaticTrackerDataSize(simContext);
    getMainStateHeader(simContext)->Data->StaticTrackerStartByte = JobInfoFlagsAreSet(simContext, INFO_FLG_KMC) ? usedBufferBytes : -1;

    return_if(cfgBufferBytes == 0, usedBufferBytes);

    var configObject = getStaticMovementTrackers(simContext);

    configObject->Begin = getMainStateBufferAddress(simContext, usedBufferBytes);
    configObject->End = configObject->Begin + (getDbStructureModel(simContext)->StaticTrackersPerCellCount * GetUnitCellCount(simContext));

    return usedBufferBytes + cfgBufferBytes;
}

// Get the number of bytes the state mobile tracker mapping data requires
static inline int32_t GetStateMobileTrackerMappingDataSize(SCONTEXT_PARAMETER)
{
    let mobileCount = getNumberOfMobiles(simContext);
    return (JobInfoFlagsAreSet(simContext, INFO_FLG_KMC))
        ? mobileCount * sizeof(int32_t)
        : 0;
}

// Configure the state mobile tracking mapping access address and return the new number of used buffer bytes
static int32_t ConfigStateMobileTrackerMappingAccess(SCONTEXT_PARAMETER, const int32_t usedBufferBytes)
{
    let cfgBufferBytes = GetStateMobileTrackerMappingDataSize(simContext);
    var headerData = getMainStateHeader(simContext)->Data;

    headerData->MobileTrackerIdxStartByte = JobInfoFlagsAreSet(simContext, INFO_FLG_KMC) ? usedBufferBytes : -1;
    return_if(cfgBufferBytes == 0, usedBufferBytes);

    var configObject = getMobileTrackerMapping(simContext);
    let mobileCount = getNumberOfMobiles(simContext);

    configObject->Begin = getMainStateBufferAddress(simContext, usedBufferBytes);
    configObject->End = configObject->Begin + mobileCount;

    return usedBufferBytes + cfgBufferBytes;
}

// Get the number of bytes the state jump statistics data requires
static inline int32_t GetStateJumpStatisticsDataSize(SCONTEXT_PARAMETER)
{
    return_if(JobInfoFlagsAreSet(simContext, INFO_FLG_NOJUMPLOGGING), 0);
    let structureModel = getDbStructureModel(simContext);
    return (JobInfoFlagsAreSet(simContext, INFO_FLG_KMC))
        ? structureModel->GlobalTrackerCount * sizeof(JumpStatistic_t)
        : 0;
}

// Configure the state jump probability tracking access address and return the new number of used buffer bytes
static int32_t ConfigStateJumpStatisticsAccess(SCONTEXT_PARAMETER, const int32_t usedBufferBytes)
{
    let cfgBufferBytes = GetStateJumpStatisticsDataSize(simContext);
    var headerData = getMainStateHeader(simContext)->Data;

    headerData->JumpStatisticsStartByte = (JobInfoFlagsAreSet(simContext, INFO_FLG_KMC) && !JobInfoFlagsAreSet(simContext, INFO_FLG_NOJUMPLOGGING)) ? usedBufferBytes : -1;
    return_if(cfgBufferBytes == 0, usedBufferBytes);

    var configObject = getJumpStatistics(simContext);
    let structureModel = getDbStructureModel(simContext);

    configObject->Begin = getMainStateBufferAddress(simContext, usedBufferBytes);
    configObject->End = configObject->Begin + structureModel->GlobalTrackerCount;
    return usedBufferBytes + cfgBufferBytes;
}

// Construct the main state buffer accessor system
static error_t ConstructMainStateBufferAccessors(SCONTEXT_PARAMETER)
{
    int32_t usedBufferBytes = 0;
    let stateBuffer = getMainStateBuffer(simContext);

    usedBufferBytes = ConfigStateHeaderAccess(simContext);
    usedBufferBytes = ConfigStateMetaAccess(simContext, usedBufferBytes);
    usedBufferBytes = ConfigStateLatticeAccess(simContext, usedBufferBytes);
    usedBufferBytes = ConfigStateCountersAccess(simContext, usedBufferBytes);
    usedBufferBytes = ConfigStateGlobalTrackerAccess(simContext, usedBufferBytes);
    usedBufferBytes = ConfigStateMobileTrackerAccess(simContext, usedBufferBytes);
    usedBufferBytes = ConfigStateStaticTrackerAccess(simContext, usedBufferBytes);
    usedBufferBytes = ConfigStateMobileTrackerMappingAccess(simContext, usedBufferBytes);
    usedBufferBytes = ConfigStateJumpStatisticsAccess(simContext, usedBufferBytes);

    return (usedBufferBytes == span_Length(*stateBuffer)) ? ERR_OK : ERR_DATACONSISTENCY;
}

// Calculates the required size in bytes for the main simulation state buffer
static int32_t CalculateMainStateBufferSize(SCONTEXT_PARAMETER)
{
    int32_t size = 0;

    size += GetStateHeaderDataSize(simContext);
    size += GetStateMetaDataSize(simContext);
    size += GetStateLatticeDataSize(simContext);
    size += GetStateCountersDataSize(simContext);
    size += GetStateGlobalTrackerDataSize(simContext);
    size += GetStateMobileTrackerDataSize(simContext);
    size += GetStateStaticTrackerDataSize(simContext);
    size += GetStateMobileTrackerMappingDataSize(simContext);
    size += GetStateJumpStatisticsDataSize(simContext);

    return size;
}

// Construct the simulation main state on the simulation context
static void ConstructMainState(SCONTEXT_PARAMETER)
{
    var stateBuffer = getMainStateBuffer(simContext);
    var state = getSimulationState(simContext);
    var jobInfo = getDbModelJobInfo(simContext);

    memset(state, 0, sizeof(SimulationState_t));
    jobInfo->StateSize = CalculateMainStateBufferSize(simContext);
    *stateBuffer = span_New(*stateBuffer, jobInfo->StateSize);

    var error = ConstructMainStateBufferAccessors(simContext);
    assert_success(error, "Failed to construct main state buffer accessor system.");
}

//  Sets values on the context that are the result of user set flags
static void SetFlagDependentValuesOnContext(SCONTEXT_PARAMETER)
{
    simContext->IsJumpLoggingDisabled = JobInfoFlagsAreSet(simContext, INFO_FLG_NOJUMPLOGGING);
    simContext->IsExpApproximationActive = JobInfoFlagsAreSet(simContext, INFO_FLG_USEFASTEXP);
}

// Construct the components of the simulation context
void ConstructSimulationContext(SCONTEXT_PARAMETER)
{
    SetFlagDependentValuesOnContext(simContext);
    ConstructSimulationModel(simContext);
    ConstructMainState(simContext);
    ConstructJumpSelectionPool(simContext);
}

// Tries to load the output plugin if it is defined on the simulation context and set it on the plugin collection
static error_t TryLoadOutputPlugin(SCONTEXT_PARAMETER)
{
    error_t error = ERR_OK;
    let fileInfo = getFileInformation(simContext);
    var plugins = getPluginCollection(simContext);

    return_if((fileInfo->OutputPluginPath) == NULL || (fileInfo->OutputPluginSymbol == NULL), ERR_USEDEFAULT);
    if ((plugins->OnDataOutput = LibraryLoadingImportFunction(fileInfo->OutputPluginPath, fileInfo->OutputPluginSymbol,
                                                              &error)) == NULL)
    {
        #if defined(IGNORE_INVALID_PLUGINS)
            fprintf(stdout, "[IGNORE_INVALID_PLUGINS] Error during output plugin loading. Using default settings.\n");
            return ERR_USEDEFAULT;
        #else
            assert_true(false, error, "Cannot load requested ouput plugin.");
        #endif
    }

    return ERR_USEDEFAULT;
}

// Tries to load the energy plugin if it is defined on the simulation context and set it on the plugin collection
static error_t TryLoadEnergyPlugin(SCONTEXT_PARAMETER)
{
    error_t error = ERR_OK;
    let fileInfo = getFileInformation(simContext);
    var plugins = getPluginCollection(simContext);

    return_if((fileInfo->EnergyPluginPath) == NULL || (fileInfo->EnergyPluginSymbol == NULL), ERR_USEDEFAULT);
    if ((plugins->OnSetTransitionStateEnergy = LibraryLoadingImportFunction(fileInfo->EnergyPluginPath,
                                                                            fileInfo->EnergyPluginSymbol, &error)) == NULL)
    {
        #if defined(IGNORE_INVALID_PLUGINS)
            fprintf(stdout, "[IGNORE_INVALID_PLUGINS] Error during energy plugin loading. Using default settings.\n");
            return ERR_USEDEFAULT;
        #else
            assert_true(false, error, "Cannot load requested energy plugin.");
        #endif
    }

    return ERR_USEDEFAULT;
}

// Sets the energy plugin function to the internal default (NULL)
static inline void SetEnergyPluginFunctionToDefault(SCONTEXT_PARAMETER)
{
    var plugins = getPluginCollection(simContext);
    plugins->OnSetTransitionStateEnergy = NULL;
}

// Set the output plugin function to the internal default (NULL)
static inline void SetOutputPluginFunctionToDefault(SCONTEXT_PARAMETER)
{
    var plugins = getPluginCollection(simContext);
    plugins->OnDataOutput = NULL;
}

// Populates the plugin delegates to either loadable plugins or the internal default
static void PopulatePluginDelegates(SCONTEXT_PARAMETER)
{
    if (TryLoadOutputPlugin(simContext) == ERR_USEDEFAULT)
        SetOutputPluginFunctionToDefault(simContext);

    if (TryLoadEnergyPlugin(simContext) == ERR_USEDEFAULT)
        SetEnergyPluginFunctionToDefault(simContext);
}

// Tries to load a simulation main state from the passed file path
static error_t TryLoadStateFromFile(SCONTEXT_PARAMETER, char const * restrict filePath)
{
    return_if(!IsAccessibleFile(filePath), ERR_USEDEFAULT);
    var stateBuffer = getMainStateBuffer(simContext);
    return LoadBufferFromFile(filePath, stateBuffer);
}

// Drop creates a state file by ensuring that the original is deleted
//static error_t DropCreateStateFile(SCONTEXT_PARAMETER, char const * restrict filePath)
//{
//    EnsureFileIsDeleted(filePath);
//    return WriteBufferToFile(filePath, FMODE_BINARY_W, getMainStateBuffer(simContext));
//}

// Tries to load the simulation state from the typical possible save locations
static error_t TryLoadSimulationState(SCONTEXT_PARAMETER)
{
    error_t error;

    if ((error = TryLoadStateFromFile(simContext, getMainRunStateFile(simContext))) == ERR_OK)
        return error;

    error = TryLoadStateFromFile(simContext, getPreRunStateFile(simContext));
    return error;
}

// Copies the database random number generator seed information to the main simulation state
static error_t CopyDbRngInfoToMainState(SCONTEXT_PARAMETER)
{
    let jobInfo = getDbModelJobInfo(simContext);
    var metaData = getMainStateMetaData(simContext);

    metaData->RngState = jobInfo->RngStartState;
    metaData->RngIncrease = jobInfo->RngIncValue;
    return ((metaData->RngIncrease & 1) != 0) ? ERR_OK : ERR_DATACONSISTENCY;
}

// Copies the database lattice information to the simulation main state
static error_t CopyDbLatticeToMainState(SCONTEXT_PARAMETER)
{
    let dbLattice = getDbModelLattice(simContext);
    var stLattice = getMainStateLattice(simContext);
    let latticeSize = span_Length(*stLattice);

    return_if(latticeSize != dbLattice->Header->Size, ERR_DATACONSISTENCY);

    CopyBuffer(dbLattice->Begin, stLattice->Begin, latticeSize);
    return ERR_OK;
}

// Translates the db lattice data into a mobile tracker id mapping on the state
static error_t CopyDefaultMobileTrackersToMainState(SCONTEXT_PARAMETER)
{
    return_if(JobInfoFlagsAreSet(simContext, INFO_FLG_MMC), ERR_OK);

    let dbLattice = getDbModelLattice(simContext);
    var mapping = getMobileTrackerMapping(simContext);
    int32_t trackerId = 0;

    cpp_foreach(envState, *getEnvironmentLattice(simContext))
    {
        let envId = getEnvironmentStateIdByPointer(simContext, envState);
        let particleId = span_Get(*dbLattice, envId);
        let jumpCount = getJumpCountAt(simContext, envState->EnvironmentDefinition->PositionId, particleId);
        if ((jumpCount >= JPOOL_DIRCOUNT_PASSIVE) && (particleId != PARTICLE_VOID))
        {
            envState->MobileTrackerId = trackerId;
            span_Get(*mapping, trackerId) = getEnvironmentStateIdByPointer(simContext, envState);
            trackerId++;
        }
    }

    return (trackerId == getNumberOfMobiles(simContext)) ? ERR_OK : ERR_DATACONSISTENCY;
}

// Constructs a pair delta table from the passed pair table. Access is [OrgPartner][NewPartner][CenterId]
static error_t ConstructPairDeltaTable(const PairTable_t* restrict pairTable, PairDeltaTable_t* restrict target)
{
    // Note: The function abuses the fact that stable pair tables have to have the same dimensions for both center & partner

    int32_t dimensions[2];
    GetArrayDimensions((VoidArray_t*) &pairTable->EnergyTable, dimensions);

    let maxCenterId = dimensions[0];
    let maxPartnerId = dimensions[1];
    PairDeltaTable_t deltaTable = array_New(deltaTable, maxPartnerId, maxPartnerId, maxCenterId);

    for (int32_t orgPartnerId = 0; orgPartnerId < maxPartnerId; orgPartnerId++)
    {
        for (int32_t newPartnerId = 0; newPartnerId < maxPartnerId; newPartnerId++)
        {
            if (orgPartnerId == newPartnerId) continue;
            for (int32_t centerId = 0; centerId < maxCenterId; centerId++)
            {
                let oldEnergy = array_Get(pairTable->EnergyTable, centerId, orgPartnerId);
                let newEnergy = array_Get(pairTable->EnergyTable, centerId, newPartnerId);
                let deltaEnergy = newEnergy - oldEnergy;
                array_Get(deltaTable, orgPartnerId, newPartnerId, centerId) = deltaEnergy;
            }
        }
    }

    *target = deltaTable;
    return ERR_OK;
}

// Generates and sets the set of 3D pair delta tables on the passed context
static error_t GenerateAndSetPairDeltaTables(SCONTEXT_PARAMETER)
{
    error_t error = ERR_OK;
    let pairTables = getPairEnergyTables(simContext);
    let tableCount = span_Length(*pairTables);
    PairDeltaTables_t deltaTables = span_New(deltaTables, tableCount);

    for (int32_t i = 0; i < tableCount; i++)
    {
        let pairTable = &span_Get(*pairTables, i);
        let deltaTable = &span_Get(deltaTables, i);
        error = ConstructPairDeltaTable(pairTable, deltaTable);
        return_if(error, error);
    }

    getDynamicModel(simContext)->PairDeltaTables = deltaTables;
    return ERR_OK;
}

// Sets all default flags on a new state when none could be loaded from file
static void SetMainStateFlagsToStartConditions(SCONTEXT_PARAMETER)
{
    setMainStateFlags(simContext, STATE_FLG_FIRSTCYCLE);

    if (JobInfoFlagsAreSet(simContext, INFO_FLG_USEPRERUN))
        setMainStateFlags(simContext, STATE_FLG_PRERUN);
}


// Synchronizes the main state to the database model by overwriting existing information in the state
static error_t SyncMainStateToDatabaseModel(SCONTEXT_PARAMETER)
{
    var error = CopyDbLatticeToMainState(simContext);
    return_if(error, error);

    error = CopyDbRngInfoToMainState(simContext);
    return_if(error, error);

    error = CopyDefaultMobileTrackersToMainState(simContext);
    return_if(error, error);

    error = InitJumpStatisticsTrackingSystem(simContext);
    return_if(error,error);

    SetMainStateFlagsToStartConditions(simContext);

    return error;
}

// Synchronizes the dynamic environment lattice with the main simulation state
static error_t SyncDynamicEnvironmentsWithState(SCONTEXT_PARAMETER)
{
    let envLattice = getEnvironmentLattice(simContext);
    let stLattice = getMainStateLattice(simContext);
    let latticeSize = span_Length(*stLattice);

    return_if(envLattice->Header->Size != latticeSize, ERR_DATACONSISTENCY);

    for (int32_t i = 0; i < latticeSize; i++)
        SetEnvironmentStateToDefault(simContext, i, getStateLatticeEntryAt(simContext, i));

    return ERR_OK;
}

// Synchronizes the mobile tracker information of the dynamic lattice with the mapping data from the main state
static error_t SyncMobileTrackersWithState(SCONTEXT_PARAMETER)
{
    return_if(JobInfoFlagsAreSet(simContext, INFO_FLG_MMC), ERR_OK);

    int32_t trackerId = 0;
    cpp_foreach(environmentId, getSimulationState(simContext)->MobileTrackerMapping)
    {
        var envState = getEnvironmentStateAt(simContext, *environmentId);
        envState->MobileTrackerId = trackerId++;
    }

    return (trackerId == getNumberOfMobiles(simContext)) ? ERR_OK : ERR_DATACONSISTENCY;
}

// Synchronizes the dynamic model to the main simulation state
static error_t SyncDynamicModelToMainState(SCONTEXT_PARAMETER)
{
    // ToDo: Potentially incomplete sync. review during testing
    var error = SyncDynamicEnvironmentsWithState(simContext);
    return_if(error, error);

    error = SyncMobileTrackersWithState(simContext);
    return error;
}

// Populates the constructed simulation state with the required run information
static void PopulateSimulationState(SCONTEXT_PARAMETER)
{
    error_t error;

    if ((error = TryLoadSimulationState(simContext)) == ERR_USEDEFAULT)
    {
        error = SyncMainStateToDatabaseModel(simContext);
        assert_success(error, "Data structure synchronization failure (static model ==> state).");

        return;
    }

    assert_success(error, "A state file exists but failed to load.");
}

// Populates the constructed dynamic simulation model with the required run information
static void PopulateDynamicSimulationModel(SCONTEXT_PARAMETER)
{
    var error = SyncDynamicModelToMainState(simContext);
    assert_success(error, "Data structure synchronization failed (state ==> dynamic model).");

    #if defined(OPT_USE_3D_PAIRTABLES)
    error = GenerateAndSetPairDeltaTables(simContext);
    assert_success(error, "Error on generation of pair delta tables.");
    #endif
}

// Synchronizes the cycle counters of the dynamic state with the info from the main simulation state
static error_t SyncMainCycleCountersWithStateStatus(SCONTEXT_PARAMETER)
{
    var counters = getMainCycleCounters(simContext);
    var stateHeader = getMainStateHeader(simContext)->Data;
    
    counters->CycleCount = stateHeader->Cycles;
    counters->McsCount = stateHeader->Mcs;
    return ERR_OK;
}

// Synchronizes the simulation cycle state to the main simulation state
static void SyncSimulationCycleStateWithModel(SCONTEXT_PARAMETER)
{
    var mainCounters = getMainCycleCounters(simContext);

    var error = SetCycleCounterStateToDefault(simContext, mainCounters);
    assert_success(error, "Failed to initialize the default counter state.");

    error = SyncMainCycleCountersWithStateStatus(simContext);
    assert_success(error, "Failed to synchronize data structure (state ==> cycle counters).");
}

// Synchronizes the selection pool with the prepared dynamic simulation model
static void SyncSelectionPoolWithDynamicModel(SCONTEXT_PARAMETER)
{
    let lattice = getEnvironmentLattice(simContext);

    for (int32_t i = 0; i < lattice->Header->Size; i++)
    {
        var error = RegisterEnvironmentStateInTransitionPool(simContext, i);
        assert_success(error, "Could not register environment on the jump selection pool.");
    }

    var error = getJumpSelectionPool(simContext)->SelectableJumpCount == 0 ? ERR_DATACONSISTENCY : ERR_OK;
    assert_success(error, "Model synchronization yielded an empty transition pool. Are you missing a doping?");
}

// Converts all energy values in pair, cluster tables, and delta tables (if enabled) & others from [eV] to units of [kT]
static error_t ConvertEnergyTablesToInternalUnits(SCONTEXT_PARAMETER)
{
    let pairTables = getPairEnergyTables(simContext);
    let clusterTables = getClusterEnergyTables(simContext);
    let defectBackground = getDefectBackground(simContext);
    let latticeBackground = getLatticeEnergyBackground(simContext);
    let factor = getPhysicalFactors(simContext)->EnergyFactorEvToKt;
    return_if(!isfinite(factor) || (factor <  0), ERR_DATACONSISTENCY);

    cpp_foreach(table, *pairTables)
        cpp_foreach(value, table->EnergyTable)
            *value *= factor;

    cpp_foreach(table, *clusterTables)
        cpp_foreach(value, table->EnergyTable)
            *value *= factor;

    cpp_foreach(value, *defectBackground)
        *value *= factor;

    cpp_foreach(value, *latticeBackground)
        *value *= factor;

    #if defined(OPT_USE_3D_PAIRTABLES)
    let deltaTables = getPairDeltaTables(simContext);
    cpp_foreach(table, *deltaTables)
        cpp_foreach(value, *table)
            *value *= factor;
    #endif

    return ERR_OK;
}

// Corrects all loaded electric field mapping factors from [eV * m/V] to units of [kT]
static error_t ConvertElectricFieldFactorsToInternalUnits(SCONTEXT_PARAMETER)
{
    return_if(JobInfoFlagsAreSet(simContext, INFO_FLG_MMC), ERR_OK);

    let jumpDirections = getJumpDirections(simContext);
    let physicalFactors = getPhysicalFactors(simContext);
    let jobHeader = getDbModelJobHeaderAsKMC(simContext);

    // Note: Correction by 0.5 as only half of the potential energy can actually affect the migration barrier!
    let factor = 0.5 * physicalFactors->EnergyFactorEvToKt * jobHeader->ElectricFieldModulus;

    return_if(!isfinite(factor) || (factor <  0), ERR_DATACONSISTENCY);

    cpp_foreach(direction, *jumpDirections)
        direction->ElectricFieldFactor *= factor;

    return ERR_OK;
}

// Synchronizes the physical simulation with the loaded db data and makes required data corrections to the input data
static void SyncPhysicalParametersAndEnergyTables(SCONTEXT_PARAMETER)
{
    var physicalFactors = getPhysicalFactors(simContext);

    var error = SetPhysicalSimulationFactorsToDefault(simContext, physicalFactors);
    assert_success(error, "Failed to calculate default physical factors.");

    error = ConvertEnergyTablesToInternalUnits(simContext);
    assert_success(error, "Failed to convert energy tables to internal units");

    error = ConvertElectricFieldFactorsToInternalUnits(simContext);
    assert_success(error, "Failed to convert field influence data to internal units");
}

// Initializes the random number generator from the main state seed information
static void PopulateRngFromMainState(SCONTEXT_PARAMETER)
{
    var rng = getMainRng(simContext);
    let metaData = getMainStateMetaData(simContext);

    *rng = (Pcg32_t) { .State = metaData->RngState, .Inc = metaData->RngIncrease };
}

// Resets the status of a single jump histogram to start conditions without touching the set limits
static void ResetJumpHistogramToNull(JumpHistogram_t*restrict histogram)
{
    histogram->OverflowCount = 0;
    histogram->UnderflowCount = 0;
    nullStructContent(histogram->CountBuffer);
}

// Resets all jump histogram buffers to zero values
static error_t ResetJumpStatisticsToNull(SCONTEXT_PARAMETER)
{
    let jumpStatistics = getJumpStatistics(simContext);
    cpp_foreach(statistic, *jumpStatistics)
    {
        ResetJumpHistogramToNull(&statistic->EdgeEnergyHistogram);
        ResetJumpHistogramToNull(&statistic->NegConfEnergyHistogram);
        ResetJumpHistogramToNull(&statistic->PosConfEnergyHistogram);
        ResetJumpHistogramToNull(&statistic->TotalEnergyHistogram);
    }
    return ERR_OK;
}

// Resets all state counter collections to zero values
static error_t ResetStateCounterCollectionsToNull(SCONTEXT_PARAMETER)
{
    // Change the main counters to compensate for overflow during pre-run phase
    var mainCounters = getMainCycleCounters(simContext);
    mainCounters->TotalSimulationGoalMcsCount += mainCounters->McsCount - mainCounters->PrerunGoalMcs;

    var counters = getMainStateCounters(simContext);
    cpp_foreach(counter, *counters)
        nullStructContent(*counter);

    return ERR_OK;
}

// Resets all state meta data to null that is affiliated with physical properties
static error_t ResetStateMetaDataToNull(SCONTEXT_PARAMETER)
{
    var metaData = getMainStateMetaData(simContext);
    metaData->SimulatedTime = 0;

    return ERR_OK;
}


// Resets the KMC tracking system to zero
static error_t ResetTrackingSystemToNull(SCONTEXT_PARAMETER)
{
    var mobileTrackers = getMobileMovementTrackers(simContext);
    var staticTrackers = getStaticMovementTrackers(simContext);
    var globalTrackers = getGlobalMovementTrackers(simContext);

    memset(mobileTrackers->Begin, 0, span_ByteCount(*mobileTrackers));
    memset(staticTrackers->Begin, 0, span_ByteCount(*staticTrackers));
    memset(globalTrackers->Begin, 0, span_ByteCount(*globalTrackers));
    return ERR_OK;
}

error_t ResetContextAfterKmcPreRun(SCONTEXT_PARAMETER)
{
    var error = ResetJumpStatisticsToNull(simContext);
    return_if(error, error);

    error = ResetStateCounterCollectionsToNull(simContext);
    return_if(error, error);

    error = ResetStateMetaDataToNull(simContext);
    return_if(error, error);

    error = ResetTrackingSystemToNull(simContext);
    return error;
}

// Populates a freshly constructed simulation context with the required runtime information
static void PopulateSimulationContext(SCONTEXT_PARAMETER)
{
    PopulatePluginDelegates(simContext);
    PopulateSimulationState(simContext);
    PopulateDynamicSimulationModel(simContext);
    SyncSimulationCycleStateWithModel(simContext);
    SyncSelectionPoolWithDynamicModel(simContext);
    SyncPhysicalParametersAndEnergyTables(simContext);
    PopulateRngFromMainState(simContext);
}

void InitializeContextForSimulation(SCONTEXT_PARAMETER)
{
    ConstructSimulationContext(simContext);
    PopulateSimulationContext(simContext);

    InitializeEnvironmentLinkingSystem(simContext);
    BuildJumpStatusCollection(simContext);
    ResynchronizeEnvironmentEnergyStatus(simContext);
}
