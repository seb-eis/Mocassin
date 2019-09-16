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
#include "Simulator/Logic/JumpSelection/JumpSelection.h"
#include "Simulator/Logic/Constants/Constants.h"
#include "Framework/Basic/FileIO/FileIO.h"
#include "Framework/Basic/DlLoading/DlLoading.h"
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
    let environmentMaxParticleId = GetEnvironmentMaxParticleId(envDef);
    let clusterStatesSize = span_Length(envDef->ClusterInteractions);
    let energyStatesSize = (environmentMaxParticleId == PARTICLE_NULL) ? 0 : environmentMaxParticleId + 1;

    env->EnergyStates = new_Span(env->EnergyStates, energyStatesSize);
    env->ClusterStates = new_Span(env->ClusterStates, clusterStatesSize);
}

// Allocates the the environment lattice and affiliated buffers ands sets the affiliated model pointers
static void AllocateEnvironmentLattice(SCONTEXT_PARAM)
{
    let sizes = getLatticeSizeVector(SCONTEXT);
    var lattice = getEnvironmentLattice(SCONTEXT);
    *lattice = new_Array(*lattice, vecCoorSet4(*sizes));

    for (int32_t i = 0; i < lattice->Header->Size;)
    {
        for (int32_t j = 0; j < sizes->D; ++j)
        {
            let envModel = getEnvironmentModelAt(SCONTEXT, j);
            var envState = getEnvironmentStateAt(SCONTEXT, i);
            AllocateEnvironmentBuffers(envState, envModel);

            // Premature ID assignment required for further allocation/construction routines
            envState->EnvironmentDefinition = envModel;
            envState->EnvironmentId = i++;
        }
    }
}

// Allocates the lattice energy buffer by the passed mmc job header
static void AllocateEnergyFluctuationAbortBuffer(Flp64Buffer_t *restrict bufferAccess, MmcHeader_t *restrict header)
{
    Buffer_t tmp = new_Span(tmp, header->AbortSequenceLength * sizeof(double));
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
static void AllocateAbortConditionBuffers(SCONTEXT_PARAM)
{
    return_if(!JobInfoFlagsAreSet(SCONTEXT, INFO_FLG_MMC));

    let jobInfo = getDbModelJobInfo(SCONTEXT);
    var energyBuffer = getLatticeEnergyBuffer(SCONTEXT);

    AllocateEnergyFluctuationAbortBuffer(energyBuffer, jobInfo->JobHeader);
}

// Constructs the dynamic simulation model
static void ConstructSimulationModel(SCONTEXT_PARAM)
{
    AllocateEnvironmentLattice(SCONTEXT);
    AllocateAbortConditionBuffers(SCONTEXT);
}

// Constructs the selection pool index redirection that redirects jump counts to selection pool id
static error_t ConstructSelectionPoolIndexRedirection(SCONTEXT_PARAM)
{
    let transitionModel = getDbTransitionModel(SCONTEXT);
    let maxPoolCount = 1 + FindMaxJumpDirectionCount(&transitionModel->JumpCountMappingTable);
    var poolMapping = getDirectionPoolMapping(SCONTEXT);
    var selectionPool = getJumpSelectionPool(SCONTEXT);
    int32_t poolIndex = 1;

    *poolMapping = new_Span(*poolMapping, maxPoolCount);
    cpp_foreach(dirCount, transitionModel->JumpCountMappingTable)
    {
        if ((*dirCount > JPOOL_DIRCOUNT_PASSIVE) && (getDirectionPoolIdByJumpCount(SCONTEXT, *dirCount) == 0))
        {
            setDirectionPoolIdByJumpCount(SCONTEXT, *dirCount, poolIndex);
            poolIndex++;
        }
    }
    selectionPool->DirectionPoolCount = poolIndex;
    return ERR_OK;
}

// Construct the selection pool direction buffers
static error_t ConstructSelectionPoolDirectionBuffers(SCONTEXT_PARAM)
{
    let poolCount = getJumpSelectionPool(SCONTEXT)->DirectionPoolCount;
    let poolSize = getNumberOfSelectables(SCONTEXT);
    let poolMapping = getDirectionPoolMapping(SCONTEXT);
    var directionPools = getDirectionPools(SCONTEXT);

    *directionPools = new_Span(*directionPools, poolCount);
    cpp_foreach(dirPool, *directionPools)
        dirPool->EnvironmentPool = new_List(dirPool->EnvironmentPool, poolSize);

    int32_t jumpCount = 0;
    cpp_foreach(id, *poolMapping)
    {
        if (*id > 0) span_Get(*directionPools, *id).DirectionCount = jumpCount;
        jumpCount++;
    }
    return ERR_OK;
}

// Construct the jump selection pool on the simulation context
static void ConstructJumpSelectionPool(SCONTEXT_PARAM)
{
    var error = ConstructSelectionPoolIndexRedirection(SCONTEXT);
    error_assert(error, "Failed to construct selection pool indexing information.");

    error = ConstructSelectionPoolDirectionBuffers(SCONTEXT);
    error_assert(error, "Failed to construct selection pool direction buffers.");
}

// Get the number of bytes the state header requires
static inline int32_t GetStateHeaderDataSize(SCONTEXT_PARAM)
{
    return (int32_t) sizeof(StateHeaderData_t);
}

// Configure the state header access address and return the number of used buffer bytes
static int32_t ConfigStateHeaderAccess(SCONTEXT_PARAM)
{
    var stateHeader = getMainStateHeader(SCONTEXT);
    stateHeader->Data = getMainStateBufferAddress(SCONTEXT, 0);
    return GetStateHeaderDataSize(SCONTEXT);
}

// Get the number of bytes the state meta data requires
static inline int32_t GetStateMetaDataSize(SCONTEXT_PARAM)
{
    return (int32_t) sizeof(StateMetaData_t);
}

// Configure the state meta access address and return the new number of used buffer bytes
static int32_t ConfigStateMetaAccess(SCONTEXT_PARAM, const int32_t usedBufferBytes)
{
    let cfgBufferBytes = GetStateMetaDataSize(SCONTEXT);
    var headerData = getMainStateHeader(SCONTEXT)->Data;
    var metaInfo = getMainStateMetaInfo(SCONTEXT);

    headerData->MetaStartByte = usedBufferBytes;
    metaInfo->Data = getMainStateBufferAddress(SCONTEXT, usedBufferBytes);

    return usedBufferBytes + cfgBufferBytes;
}

// Get the number of bytes the state lattice data requires
static inline int32_t GetStateLatticeDataSize(SCONTEXT_PARAM)
{
    let latticeModel = getDbLatticeModel(SCONTEXT);
    return latticeModel->Lattice.Header->Size;
}

// Configure the state lattice access address and return the new number of used buffer bytes
static int32_t ConfigStateLatticeAccess(SCONTEXT_PARAM, const int32_t usedBufferBytes)
{
    var configObject = getMainStateLattice(SCONTEXT);
    let cfgBufferBytes = GetStateLatticeDataSize(SCONTEXT);
    
    getMainStateHeader(SCONTEXT)->Data->LatticeStartByte = usedBufferBytes;
    configObject->Begin = getMainStateBufferAddress(SCONTEXT, usedBufferBytes);
    configObject->End = configObject->Begin + cfgBufferBytes;

    return usedBufferBytes + cfgBufferBytes;
}

// Get the number of bytes the state counters data requires
static inline int32_t GetStateCountersDataSize(SCONTEXT_PARAM)
{
    return sizeof(StateCounterCollection_t) * (int32_t) (GetMaxParticleId(SCONTEXT) + 1);
}

// Configure the state counter access address and return the new number of used buffer bytes
static int32_t ConfigStateCountersAccess(SCONTEXT_PARAM, const int32_t usedBufferBytes)
{
    var configObject = getMainStateCounters(SCONTEXT);
    let cfgBufferBytes = GetStateCountersDataSize(SCONTEXT);
    
    getMainStateHeader(SCONTEXT)->Data->CountersStartByte = usedBufferBytes;
    configObject->Begin = getMainStateBufferAddress(SCONTEXT, usedBufferBytes);
    configObject->End = configObject->Begin + (cfgBufferBytes / sizeof(StateCounterCollection_t));

    return usedBufferBytes + cfgBufferBytes;
}

// Get the number of bytes the state global tracker data requires
static inline int32_t GetStateGlobalTrackerDataSize(SCONTEXT_PARAM)
{
    if (JobInfoFlagsAreSet(SCONTEXT, INFO_FLG_KMC))
        return getDbStructureModel(SCONTEXT)->GlobalTrackerCount * sizeof(Tracker_t);

    return 0;
}

// Configure the state global tracking access address and return the new number of used buffer bytes
static int32_t ConfigStateGlobalTrackerAccess(SCONTEXT_PARAM, const int32_t usedBufferBytes)
{
    let cfgBufferBytes = GetStateGlobalTrackerDataSize(SCONTEXT);
    getMainStateHeader(SCONTEXT)->Data->GlobalTrackerStartByte = JobInfoFlagsAreSet(SCONTEXT, INFO_FLG_KMC) ? usedBufferBytes : -1;

    return_if(cfgBufferBytes == 0, usedBufferBytes);

    var configObject = getGlobalMovementTrackers(SCONTEXT);

    configObject->Begin = getMainStateBufferAddress(SCONTEXT, usedBufferBytes);
    configObject->End = configObject->Begin + getDbStructureModel(SCONTEXT)->GlobalTrackerCount;

    return usedBufferBytes + cfgBufferBytes;
}

// Get the number of bytes the state mobile tracker data requires
static inline int32_t GetStateMobileTrackerDataSize(SCONTEXT_PARAM)
{
    if (JobInfoFlagsAreSet(SCONTEXT, INFO_FLG_KMC))
        return getNumberOfMobiles(SCONTEXT) * sizeof(Tracker_t);

    return 0;
}

// Configure the state mobile tracking access address and return the new number of used buffer bytes
static int32_t ConfigStateMobileTrackerAccess(SCONTEXT_PARAM, const int32_t usedBufferBytes)
{
    let cfgBufferBytes = GetStateMobileTrackerDataSize(SCONTEXT);
    getMainStateHeader(SCONTEXT)->Data->MobileTrackerStartByte = JobInfoFlagsAreSet(SCONTEXT, INFO_FLG_KMC) ? usedBufferBytes : -1;

    return_if (cfgBufferBytes == 0, usedBufferBytes);

    var configObject = getMobileMovementTrackers(SCONTEXT);

    configObject->Begin = getMainStateBufferAddress(SCONTEXT, usedBufferBytes);
    configObject->End = configObject->Begin + getNumberOfMobiles(SCONTEXT);

    return usedBufferBytes + cfgBufferBytes;
}

// Get the number of bytes the state static tracker data requires
static inline int32_t GetStateStaticTrackerDataSize(SCONTEXT_PARAM)
{
    if (JobInfoFlagsAreSet(SCONTEXT, INFO_FLG_KMC))
        return getDbStructureModel(SCONTEXT)->StaticTrackersPerCellCount * GetUnitCellCount(SCONTEXT) * sizeof(Tracker_t);

    return 0;
}

// Configure the state static tracking access address and return the new number of used buffer bytes
static int32_t ConfigStateStaticTrackerAccess(SCONTEXT_PARAM, const int32_t usedBufferBytes)
{
    let cfgBufferBytes = GetStateStaticTrackerDataSize(SCONTEXT);
    getMainStateHeader(SCONTEXT)->Data->StaticTrackerStartByte = JobInfoFlagsAreSet(SCONTEXT, INFO_FLG_KMC) ? usedBufferBytes : -1;

    return_if(cfgBufferBytes == 0, usedBufferBytes);

    var configObject = getStaticMovementTrackers(SCONTEXT);

    configObject->Begin = getMainStateBufferAddress(SCONTEXT, usedBufferBytes);
    configObject->End = configObject->Begin + (getDbStructureModel(SCONTEXT)->StaticTrackersPerCellCount * GetUnitCellCount(SCONTEXT));

    return usedBufferBytes + cfgBufferBytes;
}

// Get the number of bytes the state mobile tracker mapping data requires
static inline int32_t GetStateMobileTrackerMappingDataSize(SCONTEXT_PARAM)
{
    let mobileCount = getNumberOfMobiles(SCONTEXT);
    return (JobInfoFlagsAreSet(SCONTEXT, INFO_FLG_KMC))
        ? mobileCount * sizeof(int32_t)
        : 0;
}

// Configure the state mobile tracking mapping access address and return the new number of used buffer bytes
static int32_t ConfigStateMobileTrackerMappingAccess(SCONTEXT_PARAM, const int32_t usedBufferBytes)
{
    let cfgBufferBytes = GetStateMobileTrackerMappingDataSize(SCONTEXT);
    var headerData = getMainStateHeader(SCONTEXT)->Data;

    headerData->MobileTrackerIdxStartByte = JobInfoFlagsAreSet(SCONTEXT, INFO_FLG_KMC) ? usedBufferBytes : -1;
    return_if(cfgBufferBytes == 0, usedBufferBytes);

    var configObject = getMobileTrackerMapping(SCONTEXT);
    let mobileCount = getNumberOfMobiles(SCONTEXT);

    configObject->Begin = getMainStateBufferAddress(SCONTEXT, usedBufferBytes);
    configObject->End = configObject->Begin + mobileCount;

    return usedBufferBytes + cfgBufferBytes;
}

// Get the number of bytes the state jump statistics data requires
static inline int32_t GetStateJumpStatisticsDataSize(SCONTEXT_PARAM)
{
    let structureModel = getDbStructureModel(SCONTEXT);
    return (JobInfoFlagsAreSet(SCONTEXT, INFO_FLG_KMC))
        ? structureModel->GlobalTrackerCount * sizeof(JumpStatistic_t)
        : 0;
}

// Configure the state jump probability tracking access address and return the new number of used buffer bytes
static int32_t ConfigStateJumpStatisticsAccess(SCONTEXT_PARAM, const int32_t usedBufferBytes)
{
    let cfgBufferBytes = GetStateJumpStatisticsDataSize(SCONTEXT);
    var headerData = getMainStateHeader(SCONTEXT)->Data;

    headerData->JumpStatisticsStartByte = JobInfoFlagsAreSet(SCONTEXT, INFO_FLG_KMC) ? usedBufferBytes : -1;
    return_if(cfgBufferBytes == 0, usedBufferBytes);

    var configObject = getJumpStatistics(SCONTEXT);
    let structureModel = getDbStructureModel(SCONTEXT);

    configObject->Begin = getMainStateBufferAddress(SCONTEXT, usedBufferBytes);
    configObject->End = configObject->Begin + structureModel->GlobalTrackerCount;
    return usedBufferBytes + cfgBufferBytes;
}

// Construct the main state buffer accessor system
static error_t ConstructMainStateBufferAccessors(SCONTEXT_PARAM)
{
    int32_t usedBufferBytes = 0;
    let stateBuffer = getMainStateBuffer(SCONTEXT);

    usedBufferBytes = ConfigStateHeaderAccess(SCONTEXT);
    usedBufferBytes = ConfigStateMetaAccess(SCONTEXT, usedBufferBytes);
    usedBufferBytes = ConfigStateLatticeAccess(SCONTEXT, usedBufferBytes);
    usedBufferBytes = ConfigStateCountersAccess(SCONTEXT, usedBufferBytes);
    usedBufferBytes = ConfigStateGlobalTrackerAccess(SCONTEXT, usedBufferBytes);
    usedBufferBytes = ConfigStateMobileTrackerAccess(SCONTEXT, usedBufferBytes);
    usedBufferBytes = ConfigStateStaticTrackerAccess(SCONTEXT, usedBufferBytes);
    usedBufferBytes = ConfigStateMobileTrackerMappingAccess(SCONTEXT, usedBufferBytes);
    usedBufferBytes = ConfigStateJumpStatisticsAccess(SCONTEXT, usedBufferBytes);

    return (usedBufferBytes == span_Length(*stateBuffer)) ? ERR_OK : ERR_DATACONSISTENCY;
}

// Calculates the required size in bytes for the main simulation state buffer
static int32_t CalculateMainStateBufferSize(SCONTEXT_PARAM)
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
static void ConstructMainState(SCONTEXT_PARAM)
{
    var stateBuffer = getMainStateBuffer(SCONTEXT);
    var state = getSimulationState(SCONTEXT);
    var jobInfo = getDbModelJobInfo(SCONTEXT);

    memset(state, 0, sizeof(SimulationState_t));
    jobInfo->StateSize = CalculateMainStateBufferSize(SCONTEXT);
    *stateBuffer = new_Span(*stateBuffer, jobInfo->StateSize);

    var error = ConstructMainStateBufferAccessors(SCONTEXT);
    error_assert(error, "Failed to construct main state buffer accessor system.");
}

// Construct the components of the simulation context
void ConstructSimulationContext(SCONTEXT_PARAM)
{
    ConstructSimulationModel(SCONTEXT);
    ConstructMainState(SCONTEXT);
    ConstructJumpSelectionPool(SCONTEXT);
}

// Tries to load the output plugin if it is defined on the simulation context and set it on the plugin collection
static error_t TryLoadOutputPlugin(SCONTEXT_PARAM)
{
    error_t error = ERR_OK;
    let fileInfo = getFileInformation(SCONTEXT);
    var plugins = getPluginCollection(SCONTEXT);

    return_if((fileInfo->OutputPluginPath) == NULL || (fileInfo->OutputPluginSymbol == NULL), ERR_USEDEFAULT);
    if ((plugins->OnDataOutput = ImportFunction(fileInfo->OutputPluginPath, fileInfo->OutputPluginSymbol, &error)) == NULL)
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
static error_t TryLoadEnergyPlugin(SCONTEXT_PARAM)
{
    error_t error = ERR_OK;
    let fileInfo = getFileInformation(SCONTEXT);
    var plugins = getPluginCollection(SCONTEXT);

    return_if((fileInfo->EnergyPluginPath) == NULL || (fileInfo->EnergyPluginSymbol == NULL), ERR_USEDEFAULT);
    if ((plugins->OnSetTransitionStateEnergy = ImportFunction(fileInfo->EnergyPluginPath, fileInfo->EnergyPluginSymbol, &error)) == NULL)
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

// Sets the energy plugin function to the internal default (NULL)
static inline void SetEnergyPluginFunctionToDefault(SCONTEXT_PARAM)
{
    var plugins = getPluginCollection(SCONTEXT);
    plugins->OnSetTransitionStateEnergy = NULL;
}

// Set the output plugin function to the internal default (NULL)
static inline void SetOutputPluginFunctionToDefault(SCONTEXT_PARAM)
{
    var plugins = getPluginCollection(SCONTEXT);
    plugins->OnDataOutput = NULL;
}

// Populates the plugin delegates to either loadable plugins or the internal default
static void PopulatePluginDelegates(SCONTEXT_PARAM)
{
    if (TryLoadOutputPlugin(SCONTEXT) == ERR_USEDEFAULT)
        SetOutputPluginFunctionToDefault(SCONTEXT);

    if (TryLoadEnergyPlugin(SCONTEXT) == ERR_USEDEFAULT)
        SetEnergyPluginFunctionToDefault(SCONTEXT);
}

// Tries to load a simulation main state from the passed file path
static error_t TryLoadStateFromFile(SCONTEXT_PARAM, char const * restrict filePath)
{
    return_if(!IsAccessibleFile(filePath), ERR_USEDEFAULT);
    var stateBuffer = getMainStateBuffer(SCONTEXT);
    return LoadBufferFromFile(filePath, stateBuffer);
}

// Drop creates a state file by ensuring that the original is deleted
//static error_t DropCreateStateFile(SCONTEXT_PARAM, char const * restrict filePath)
//{
//    EnsureFileIsDeleted(filePath);
//    return WriteBufferToFile(filePath, FMODE_BINARY_W, getMainStateBuffer(SCONTEXT));
//}

// Tries to load the simulation state from the typical possible save locations
static error_t TryLoadSimulationState(SCONTEXT_PARAM)
{
    error_t error;

    if ((error = TryLoadStateFromFile(SCONTEXT, getMainRunStateFile(SCONTEXT))) == ERR_OK)
        return error;

    error = TryLoadStateFromFile(SCONTEXT, getPreRunStateFile(SCONTEXT));
    return error;
}

// Copies the database random number generator seed information to the main simulation state
static error_t CopyDbRngInfoToMainState(SCONTEXT_PARAM)
{
    let jobInfo = getDbModelJobInfo(SCONTEXT);
    var metaData = getMainStateMetaData(SCONTEXT);

    metaData->RngState = jobInfo->RngStartState;
    metaData->RngIncrease = jobInfo->RngIncValue;
    return ((metaData->RngIncrease & 1) != 0) ? ERR_OK : ERR_DATACONSISTENCY;
}

// Copies the database lattice information to the simulation main state
static error_t CopyDbLatticeToMainState(SCONTEXT_PARAM)
{
    let dbLattice = getDbModelLattice(SCONTEXT);
    var stLattice = getMainStateLattice(SCONTEXT);
    let latticeSize = span_Length(*stLattice);

    return_if(latticeSize != dbLattice->Header->Size, ERR_DATACONSISTENCY);

    CopyBuffer(dbLattice->Begin, stLattice->Begin, latticeSize);
    return ERR_OK;
}

// Translates the db lattice data into a mobile tracker id mapping on the state
static error_t CopyDefaultMobileTrackersToMainState(SCONTEXT_PARAM)
{
    return_if(JobInfoFlagsAreSet(SCONTEXT, INFO_FLG_MMC), ERR_OK);

    let dbLattice = getDbModelLattice(SCONTEXT);
    var mapping = getMobileTrackerMapping(SCONTEXT);
    int32_t trackerId = 0;

    cpp_foreach(envState, *getEnvironmentLattice(SCONTEXT))
    {
        let particleId = span_Get(*dbLattice, envState->EnvironmentId);
        let jumpCount = getJumpCountAt(SCONTEXT, envState->EnvironmentDefinition->ObjectId, particleId);
        if ((jumpCount >= JPOOL_DIRCOUNT_PASSIVE) && (particleId != PARTICLE_VOID))
        {
            envState->MobileTrackerId = trackerId;
            span_Get(*mapping, trackerId) = envState->EnvironmentId;
            trackerId++;
        }
    }

    return (trackerId == getNumberOfMobiles(SCONTEXT)) ? ERR_OK : ERR_DATACONSISTENCY;
}

// Constructs a pair delta table from the passed pair table. Access is [OrgPartner][NewPartner][CenterId]
static error_t ConstructPairDeltaTable(const PairTable_t* restrict pairTable, PairDeltaTable_t* restrict target)
{
    // Note: The function abuses the fact that stable pair tables have to have the same dimensions for both center & partner

    int32_t dimensions[2];
    GetArrayDimensions((VoidArray_t*) &pairTable->EnergyTable, dimensions);

    let maxCenterId = dimensions[0];
    let maxPartnerId = dimensions[1];
    PairDeltaTable_t deltaTable = new_Array(deltaTable, maxPartnerId, maxPartnerId, maxCenterId);

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
static error_t GenerateAndSetPairDeltaTables(SCONTEXT_PARAM)
{
    error_t error = ERR_OK;
    let pairTables = getPairEnergyTables(SCONTEXT);
    let tableCount = span_Length(*pairTables);
    PairDeltaTables_t deltaTables = new_Span(deltaTables, tableCount);

    for (int32_t i = 0; i < tableCount; i++)
    {
        let pairTable = &span_Get(*pairTables, i);
        let deltaTable = &span_Get(deltaTables, i);
        error = ConstructPairDeltaTable(pairTable, deltaTable);
        return_if(error, error);
    }

    getDynamicModel(SCONTEXT)->PairDeltaTables = deltaTables;
    return ERR_OK;
}

// Sets all default flags on a new state when none could be loaded from file
static void SetMainStateFlagsToStartConditions(SCONTEXT_PARAM)
{
    setMainStateFlags(SCONTEXT, STATE_FLG_FIRSTCYCLE);

    if (JobInfoFlagsAreSet(SCONTEXT, INFO_FLG_USEPRERUN))
        setMainStateFlags(SCONTEXT, STATE_FLG_PRERUN);
}


// Synchronizes the main state to the database model by overwriting existing information in the state
static error_t SyncMainStateToDatabaseModel(SCONTEXT_PARAM)
{
    var error = CopyDbLatticeToMainState(SCONTEXT);
    return_if(error, error);

    error = CopyDbRngInfoToMainState(SCONTEXT);
    return_if(error, error);

    error = CopyDefaultMobileTrackersToMainState(SCONTEXT);
    return_if(error, error);

    error = InitJumpStatisticsTrackingSystem(SCONTEXT);
    return_if(error,error);

    SetMainStateFlagsToStartConditions(SCONTEXT);

    return error;
}

// Synchronizes the dynamic environment lattice with the main simulation state
static error_t SyncDynamicEnvironmentsWithState(SCONTEXT_PARAM)
{
    let envLattice = getEnvironmentLattice(SCONTEXT);
    let stLattice = getMainStateLattice(SCONTEXT);
    let latticeSize = span_Length(*stLattice);

    return_if(envLattice->Header->Size != latticeSize, ERR_DATACONSISTENCY);

    for (int32_t i = 0; i < latticeSize; i++)
        SetEnvironmentStateToDefault(SCONTEXT, i, getStateLatticeEntryAt(SCONTEXT, i));

    return ERR_OK;
}

// Synchronizes the mobile tracker information of the dynamic lattice with the mapping data from the main state
static error_t SyncMobileTrackersWithState(SCONTEXT_PARAM)
{
    return_if(JobInfoFlagsAreSet(SCONTEXT, INFO_FLG_MMC), ERR_OK);

    int32_t trackerId = 0;
    cpp_foreach(environmentId, getSimulationState(SCONTEXT)->MobileTrackerMapping)
    {
        var envState = getEnvironmentStateAt(SCONTEXT, *environmentId);
        envState->MobileTrackerId = trackerId++;
    }

    return (trackerId == getNumberOfMobiles(SCONTEXT)) ? ERR_OK : ERR_DATACONSISTENCY;
}

// Synchronizes the dynamic model to the main simulation state
static error_t SyncDynamicModelToMainState(SCONTEXT_PARAM)
{
    // ToDo: Potentially incomplete sync. review during testing
    var error = SyncDynamicEnvironmentsWithState(SCONTEXT);
    return_if(error, error);

    error = SyncMobileTrackersWithState(SCONTEXT);
    return error;
}

// Populates the constructed simulation state with the required run information
static void PopulateSimulationState(SCONTEXT_PARAM)
{
    error_t error;

    if ((error = TryLoadSimulationState(SCONTEXT)) == ERR_USEDEFAULT)
    {
        error = SyncMainStateToDatabaseModel(SCONTEXT);
        error_assert(error, "Data structure synchronization failure (static model ==> state).");

        return;
    }

    error_assert(error, "A state file exists but failed to load.");
}

// Populates the constructed dynamic simulation model with the required run information
static void PopulateDynamicSimulationModel(SCONTEXT_PARAM)
{
    var error = SyncDynamicModelToMainState(SCONTEXT);
    error_assert(error, "Data structure synchronization failed (state ==> dynamic model).");

    #if defined(OPT_USE_3D_PAIRTABLES)
    error = GenerateAndSetPairDeltaTables(SCONTEXT);
    error_assert(error, "Error on generation of pair delta tables.");
    #endif
}

// Synchronizes the cycle counters of the dynamic state with the info from the main simulation state
static error_t SyncMainCycleCountersWithStateStatus(SCONTEXT_PARAM)
{
    var counters = getMainCycleCounters(SCONTEXT);
    var stateHeader = getMainStateHeader(SCONTEXT)->Data;
    
    counters->CycleCount = stateHeader->Cycles;
    counters->McsCount = stateHeader->Mcs;

    return (counters->McsCount < counters->TotalSimulationGoalMcsCount) ? ERR_OK : ERR_ALREADYCOMPLETED;
}

// Synchronizes the simulation cycle state to the main simulation state
static void SyncSimulationCycleStateWithModel(SCONTEXT_PARAM)
{
    var mainCounters = getMainCycleCounters(SCONTEXT);

    var error = SetCycleCounterStateToDefault(SCONTEXT, mainCounters);
    error_assert(error, "Failed to set default main counter status.");

    error = SyncMainCycleCountersWithStateStatus(SCONTEXT);
    error_assert(error, "Failed to synchronize data structure (state ==> cycle counters).");
}

// Synchronizes the selection pool with the prepared dynamic simulation model
static void SyncSelectionPoolWithDynamicModel(SCONTEXT_PARAM)
{
    let lattice = getEnvironmentLattice(SCONTEXT);

    for (int32_t i = 0; i < lattice->Header->Size; i++)
    {
        var error = HandleEnvStatePoolRegistration(SCONTEXT, i);
        error_assert(error, "Could not register environment on the jump selection pool.");
    }

    var error = getJumpSelectionPool(SCONTEXT)->SelectableJumpCount == 0 ? ERR_DATACONSISTENCY : ERR_OK;
    error_assert(error, "Model synchronization yielded an empty transition pool. Are you missing a doping?");
}

// Converts all energy values in pair, cluster tables, and delta tables (if enabled) & others from [eV] to units of [kT]
static error_t ConvertEnergyTablesToInternalUnits(SCONTEXT_PARAM)
{
    let pairTables = getPairEnergyTables(SCONTEXT);
    let clusterTables = getClusterEnergyTables(SCONTEXT);
    let defectBackground = getDefectBackground(SCONTEXT);
    let latticeBackground = getLatticeEnergyBackground(SCONTEXT);
    let factor = getPhysicalFactors(SCONTEXT)->EnergyFactorEvToKt;
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
    let deltaTables = getPairDeltaTables(SCONTEXT);
    cpp_foreach(table, *deltaTables)
        cpp_foreach(value, *table)
            *value *= factor;
    #endif

    return ERR_OK;
}

// Corrects all loaded electric field mapping factors from [eV * m/V] to units of [kT]
static error_t ConvertElectricFieldFactorsToInternalUnits(SCONTEXT_PARAM)
{
    return_if(JobInfoFlagsAreSet(SCONTEXT, INFO_FLG_MMC), ERR_OK);

    let jumpDirections = getJumpDirections(SCONTEXT);
    let physicalFactors = getPhysicalFactors(SCONTEXT);
    let jobHeader = getDbModelJobHeaderAsKMC(SCONTEXT);

    // Note: Correction by 0.5 as only half of the potential energy can actually affect the migration barrier!
    let factor = 0.5 * physicalFactors->EnergyFactorEvToKt * jobHeader->ElectricFieldModulus;

    return_if(!isfinite(factor) || (factor <  0), ERR_DATACONSISTENCY);

    cpp_foreach(direction, *jumpDirections)
        direction->ElectricFieldFactor *= factor;

    return ERR_OK;
}

// Synchronizes the physical simulation with the loaded db data and makes required data corrections to the input data
static void SyncPhysicalParametersAndEnergyTables(SCONTEXT_PARAM)
{
    var physicalFactors = getPhysicalFactors(SCONTEXT);

    var error = SetPhysicalSimulationFactorsToDefault(SCONTEXT, physicalFactors);
    error_assert(error, "Failed to calculate default physical factors.");

    error = ConvertEnergyTablesToInternalUnits(SCONTEXT);
    error_assert(error, "Failed to convert energy tables to internal units");

    error = ConvertElectricFieldFactorsToInternalUnits(SCONTEXT);
    error_assert(error, "Failed to convert field influence data to internal units");
}

// Initializes the random number generator from the main state seed information
static void PopulateRngFromMainState(SCONTEXT_PARAM)
{
    var rng = getMainRng(SCONTEXT);
    let metaData = getMainStateMetaData(SCONTEXT);

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
static error_t ResetJumpStatisticsToNull(SCONTEXT_PARAM)
{
    let jumpStatistics = getJumpStatistics(SCONTEXT);
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
static error_t ResetStateCounterCollectionsToNull(SCONTEXT_PARAM)
{
    // Change the main counters to compensate for overflow during pre-run phase
    var mainCounters = getMainCycleCounters(SCONTEXT);
    mainCounters->TotalSimulationGoalMcsCount += mainCounters->McsCount - mainCounters->PrerunGoalMcs;

    var counters = getMainStateCounters(SCONTEXT);
    cpp_foreach(counter, *counters)
        nullStructContent(*counter);

    return ERR_OK;
}

// Resets all state meta data to null that is affiliated with physical properties
static error_t ResetStateMetaDataToNull(SCONTEXT_PARAM)
{
    var metaData = getMainStateMetaData(SCONTEXT);
    metaData->SimulatedTime = 0;

    return ERR_OK;
}


// Resets the KMC tracking system to zero
static error_t ResetTrackingSystemToNull(SCONTEXT_PARAM)
{
    var mobileTrackers = getMobileMovementTrackers(SCONTEXT);
    var staticTrackers = getStaticMovementTrackers(SCONTEXT);
    var globalTrackers = getGlobalMovementTrackers(SCONTEXT);

    memset(mobileTrackers->Begin, 0, span_ByteCount(*mobileTrackers));
    memset(staticTrackers->Begin, 0, span_ByteCount(*staticTrackers));
    memset(globalTrackers->Begin, 0, span_ByteCount(*globalTrackers));
    return ERR_OK;
}

error_t KMC_ResetContextAfterPreRun(SCONTEXT_PARAM)
{
    var error = ResetJumpStatisticsToNull(SCONTEXT);
    return_if(error, error);

    error = ResetStateCounterCollectionsToNull(SCONTEXT);
    return_if(error, error);

    error = ResetStateMetaDataToNull(SCONTEXT);
    return_if(error, error);

    error = ResetTrackingSystemToNull(SCONTEXT);
    return error;
}

// Populates a freshly constructed simulation context with the required runtime information
static void PopulateSimulationContext(SCONTEXT_PARAM)
{
    PopulatePluginDelegates(SCONTEXT);
    PopulateSimulationState(SCONTEXT);
    PopulateDynamicSimulationModel(SCONTEXT);
    SyncSimulationCycleStateWithModel(SCONTEXT);
    SyncSelectionPoolWithDynamicModel(SCONTEXT);
    SyncPhysicalParametersAndEnergyTables(SCONTEXT);
    PopulateRngFromMainState(SCONTEXT);
}

void PrepareContextForSimulation(SCONTEXT_PARAM)
{
    ConstructSimulationContext(SCONTEXT);
    PopulateSimulationContext(SCONTEXT);

    BuildEnvironmentLinkingSystem(SCONTEXT);
    BuildJumpStatusCollection(SCONTEXT);
    ResynchronizeEnvironmentEnergyStatus(SCONTEXT);
}
