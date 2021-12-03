//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	SimContext.h        		//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Sim context (Full access)   //
//////////////////////////////////////////

#pragma once
#include "Libraries/Framework/Errors/McErrors.h"
#include "Libraries/Framework/Basic/BaseTypes.h"
#include "Libraries/Framework/Basic/Buffers.h"
#include "Libraries/Framework/Math/PcgRandom.h"
#include "Libraries/Simulator/Data/Jobs/JobDbModel.h"
#include "Libraries/Simulator/Data/State/SimulationState.h"

// Marks the "no result yet" cycle outcome case
#define MC_UNFINISHED_CYCLE     0

// Marks the "statistically accepted" cycle outcome case
#define MC_ACCEPTED_CYCLE       1

// Marks the "statistically rejected" cycle outcome case
#define MC_REJECTED_CYCLE       2

// Marks the "site blocked" cycle outcome case
#define MC_BLOCKED_CYCLE        3

// Marks the "start state is unstable" cycle outcome case
#define MC_STARTUNSTABLE_CYCLE  4

// Marks the "end state is unstable" cycle outcome case
#define MC_ENDUNSTABLE_CYCLE    5

// Marks the "skipped due to jump frequency" cycle outcome case
#define MC_SKIPPED_CYCLE        6

// Array type for 3D pair energy delta tables [Original][New][Partner]
// Layout@ggc_x86_64 => 24@[8,8,8]
typedef Array_t(double, 3, PairDeltaTable) PairDeltaTable_t;

// Span type for 3D pair energy delta table sets [TableId]
// Layout@ggc_x86_64 => 24@[8,8,8]
typedef Span_t(PairDeltaTable_t, PairDeltaTables) PairDeltaTables_t;

// Type for cluster links
// Layout@ggc_x86_64 => 2@[1,1]
typedef struct ClusterLink
{
    // The id of the cluster. 256 clusters per environment are supported
    byte_t ClusterId;

    // The id of the code byte that has to be changed
    byte_t CodeByteId;
    
} ClusterLink_t;

// Type for cluster link lists
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(ClusterLink_t, ClusterLinks) ClusterLinks_t;

// Type for an environment link
// Layout@ggc_x86_64 => 24@[4,4,16]
typedef struct EnvironmentLink
{
    // The linear id of the target environment
    int32_t         TargetEnvironmentId;

    // The target pair id in the environment
    int32_t         TargetPairId;

    // The collection of affiliated cluster links
    ClusterLinks_t  ClusterLinks;
    
} EnvironmentLink_t;

// Type for env link list that supports push back operation
// Layout@ggc_x86_64 => 24@[8,8,8]
typedef List_t(EnvironmentLink_t, EnvironmentLinks) EnvironmentLinks_t;

// Type for cluster states and affiliated backups
// Layout@ggc_x86_64 => 24@[4,4,8,8]
typedef struct ClusterState
{
    // The current code id of the cluster
    int32_t                 CodeId;

    // The current code id backup
    int32_t                 CodeIdBackup;

    // The current occupation code
    OccupationCode64_t      OccupationCode;

    // The current occupation code backup
    OccupationCode64_t      OccupationCodeBackup;
    
} ClusterState_t;

// Type for lists of cluster states
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(ClusterState_t, ClusterStates) ClusterStates_t;

// Type for lists of energy states
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(double, EnergyStates) EnergyStates_t;

// Type for a full environment state definition (Supports 16 bit alignment)
// Layout@ggc_x86_64 => 96@[16,1,1,1,1,4,4,4,16,16,24,8]
typedef struct EnvironmentState
{
    // Absolute 4D position vector of the environment in the lattice
    Vector4_t                   LatticeVector;

    // Current id of the environment in the jump path
    byte_t                      PathId;

    // Boolean flag if the environment center is mobile
    bool_t                      IsMobile;

    // Boolean flag if the environment center is stable
    bool_t                      IsStable;

    // Current occupation particle id
    byte_t                      ParticleId;

    // Current direction pool id the environment is registered in
    int32_t                     PoolId;

    // Current relative position id in the affiliated direction pool environment list
    int32_t                     PoolPositionId;

    // Current mobile tracker id of the environment
    int32_t                     MobileTrackerId;

    // Current energy states of the environment
    EnergyStates_t              EnergyStates;

    // Current cluster states of the environment
    ClusterStates_t             ClusterStates;

    // Set of registered links to other environments
    EnvironmentLinks_t          EnvironmentLinks;

    // Pointer to the affiliated environment definition
    EnvironmentDefinition_t*    EnvironmentDefinition;

} EnvironmentState_t;

// Type for the 4d rectangular environment state lattice access
// Layout@ggc_x86_64 => 24@[8,8,8]
typedef Array_t(EnvironmentState_t, 4, EnvironmentLattice) EnvironmentLattice_t;

// Type for the jump selection index information
// Layout@ggc_x86_64 => 16@[4,4,4,4]
typedef struct JumpSelectionInfo
{
    // The selected environment id
    int32_t EnvironmentId;

    // The selected relative jump id within the selected environment
    int32_t RelativeJumpId;

    // The global jump id of the selection
    int32_t GlobalJumpId;

    // The selected offset source environment id (MMC only)
    int32_t MmcOffsetSourceId;
    
} JumpSelectionInfo_t;

// Type for the transition energy information
// Layout@ggc_x86_64 => 104@[8,8,8,8,8,8,8,8,8,8,8,8,8]
typedef struct JumpEnergyInfo
{
    // The S0 energy in units of [kT]
    double S0Energy;

    // The S1 energy in units of [kT]
    double S1Energy;

    // The S2 energy in units of [kT]
    double S2Energy;

    // The S1 energy in units of [kT] without the S0 and S2 influence
    double RawS1Energy;

    // The electric field influence energy in units of [kT]
    double ElectricFieldEnergy;

    // The conformation delta energy in units of [kT]
    double ConformationDeltaEnergy;

    // The state energy change from S0 to S2 energy in units of [kT] (This is a KMC only field)
    double S0toS2EnergyBarrierWithoutField;

    // The state energy change from S2 to S0 energy in units of [kT] (This is a KMC only field)
    double S2toS0EnergyBarrierWithoutField;

    // The state change energy barrier to change from S2 to S0 energy in units of [kT] (total barrier for KMC and MMC)
    double S2toS0EnergyBarrier;

    // The state change energy barrier to change from S0 to S2 energy in units of [kT] (total barrier for KMC and MMC)
    double S0toS2EnergyBarrier;

    // The non-normalized state change probability from S0 to S2
    double RawS0toS2TransitionProbability;

    // The non-normalized state change probability from S2 to S0
    double RawS2toS0TransitionProbability;

    // The normalized compare state change probability from S0 to S2
    double NormalizedS0toS2TransitionProbability;
    
} JumpEnergyInfo_t;

// Type for the internal simulation cycle counters
// Layout@ggc_x86_64 => 56@[8,8,8,8,8,8,8]
typedef struct CycleCounterState
{
    // The total number of cycles
    int64_t CycleCount;

    // The total successful steps
    int64_t McsCount;

    // The cycles per execution loop
    int64_t CycleCountPerExecutionLoop;

    // The goal mcs per execution phase
    int64_t McsCountPerExecutionPhase;

    // The next total mcs an execution phase has to reach before entering the next write phase
    int64_t NextExecutionPhaseGoalMcsCount;

    // The simulation abort mcs count
    int64_t TotalSimulationGoalMcsCount;

    // The pre run mcs of the target mcs of the simulation
    int64_t PrerunGoalMcs;
    
} CycleCounterState_t;

// Type for the path related backups during a cycle
// Layout@ggc_x86_64 => 96@[8x8,8x4]
typedef struct EnvironmentBackup
{
    // Backups for the energy values of the path
    double  PathEnergies[JUMPS_JUMPLENGTH_MAX];

    // Backups for the mobile tracker mapping of the path
    int32_t PathMobileMappings[JUMPS_JUMPLENGTH_MAX];

} EnvironmentBackup_t;

// Type for jump links
// Layout@ggc_x86_64 => 8@[4,4]
typedef struct JumpLink
{
    // The path id of the sender that contains the affiliated environment link
    int32_t     SenderPathId;

    // The relative id in the collection of the corresponding link list
    int32_t     LinkId;

} JumpLink_t;

// Type for jump link lists
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(JumpLink_t, JumpLinks) JumpLinks_t;

// Type for the jump status that holds the jump link information of a single KMC jump
// Layout@ggc_x86_64 => 80@[16]
typedef struct JumpStatus
{
    // The jump links of the jump status
    JumpLinks_t JumpLinks;

} JumpStatus_t;

// Type for a 4D array of jump status objects access by [A,B,C,JumpDirId]
// Layout@ggc_x86_64 => 24@[8,8,8]
typedef Array_t(JumpStatus_t, 4, JumpStatusArray) JumpStatusArray_t;

// Type for the cycle state storage. Contains all information manipulated and buffered during simulation cycles
// Layout@ggc_x86_64 => 248@[48,8,16,104,8,8,8,8,8,8,8,8,8]
typedef struct CycleState
{
    // The main counter state. Controls the cycle loop settings
    CycleCounterState_t         MainCounters;

    // The current active state code that describes a transition start state
    OccupationCode64_t          ActiveStateCode;

    // The jump selection that contains the transition selection information
    JumpSelectionInfo_t         ActiveSelectionInfo;

    // Teh jump energy info that stores energy and probability information on the current transition
    JumpEnergyInfo_t            ActiveEnergyInfo;

    // The environment backup that stores rollback information about the path during the cycle
    EnvironmentBackup_t         ActiveEnvironmentBackup;

    // The pointer to the currently active jump direction
    JumpDirection_t*            ActiveJumpDirection;

    // The pointer to the currently active jump collection
    JumpCollection_t*           ActiveJumpCollection;

    // The pointer to the currently active jump rule
    JumpRule_t*                 ActiveJumpRule;

    // The pointer to the currently active counter collection
    StateCounterCollection_t*   ActiveCounterCollection;

    // The environment path array that buffers the current transition path
    EnvironmentState_t*         ActivePathEnvironments[JUMPS_JUMPLENGTH_MAX];

    // The active jump status
    JumpStatus_t*               ActiveJumpStatus;

    // The pointer to the current work environment
    EnvironmentState_t*         WorkEnvironment;

    // The pointer to the current work cluster
    ClusterState_t*             WorkCluster;

    #if defined(OPT_USE_3D_PAIRTABLES)
    // The pointer to the current pair delta table
    PairDeltaTable_t*           WorkPairTable;
    #else
    // The pointer to the current pair table
    PairTable_t*                WorkPairTable;
    #endif

    // The pointer to the current work cluster table
    ClusterTable_t*             WorkClusterTable;

} CycleState_t;

// Type for the environment pool access
// Layout@ggc_x86_64 => 24@[8,8,8]
typedef List_t(int32_t, EnvironmentPool) EnvironmentPool_t;

// Type for the direction pools
// Layout@ggc_x86_64 => 40@[24,4,4,4,{4}]
typedef struct DirectionPool
{
    // The environment pool of the direction pool. Contains affiliated [environmentId]
    EnvironmentPool_t   EnvironmentPool;

    // The current position count of the pool
    int32_t             PositionCount;

    // The direction count of the pool
    int32_t             DirectionCount;

    // The current selectable jump count of the pool
    int32_t             JumpCount;

    // Padding integer
    int32_t             Padding:32;
    
} DirectionPool_t;

// Type for lists of direction pools
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(DirectionPool_t, DirectionPools) DirectionPools_t;

// Type for the jump selection pool
// Layout@ggc_x86_64 => 64@[4,4,16,40]
typedef struct JumpSelectionPool
{
    // The number of selectable jumps in the pool
    int32_t             SelectableJumpCount;

    // The number of directions pools in the selection pool
    int32_t             DirectionPoolCount;

    // The direction count to pool index mapping
    IdMappingSpan_t     DirectionPoolMapping;

    // The span of direction pools
    DirectionPools_t    DirectionPools;
    
} JumpSelectionPool_t;

// Type for the program run information
// Layout@ggc_x86_64 => 16@[8,8]
typedef struct SimulationRunInfo
{
    // The clock value at simulation start
    int64_t MainRoutineStartClock;

    // The last clock value taken
    int64_t PreviousBlockFinishClock;

} SimulationRunInfo_t;

// Type for physical simulation values
// Layout@ggc_x86_64 => 24@[8,8,8,8]
typedef struct PhysicalInfo
{
    // The energy conversion factor from [eV] to [kT]
    double EnergyFactorEvToKt;

    // The energy conversion factor from [kT] to [eV]
    double EnergyFactorKtToEv;

    // The total jump normalization factor
    double TotalJumpNormalization;

    // The current time stepping in [s]
    double TimeStepPerJumpAttempt;
    
} PhysicalInfo_t;

// Type for the file string information
// Layout@ggc_x86_64 => 88@[8,8,8,8,8,8,8,8,8,8,8]
typedef struct FileInfo
{
    // The database query string for data loading
    char const* JobDbQuery;

    // The executable path
    char const* ExecutablePath;

    // The full path to the used prerun state file
    char const* PrerunStateFile;

    // The full path to the used main run state file
    char const* MainStateFile;

    // The path to the used IO directory
    char const* IODirectoryPath;

    // The job database path
    char const* JobDbPath;

    // The output plugin path
    char const* OutputPluginPath;

    // The output plugin search symbol
    char const* OutputPluginSymbol;

    // The energy plugin path
    char const* EnergyPluginPath;

    // The energy plugin search symbol
    char const* EnergyPluginSymbol;

    // The path where the system should look for extension routines
    char const* ExtensionLookupPath;
    
} FileInfo_t;

// Type for floating point buffers with storage for last average
// Layout@ggc_x86_64 => 32@[8,8,8,8]
typedef struct Flp64Buffer
{
    // Buffer start ptr. Values are buffered as [kT]
    double* Begin;

    // Buffer end ptr. Values are buffered as [kT]
    double* End;

    // Capacity end ptr. Values are buffered as [kT]
    double* CapacityEnd;

    // The last sum value in [eV]
    double  LastSum;

    // The current sum value in [eV]
    double  CurrentSum;
    
} Flp64Buffer_t;

// Type for the simulation dynamic model
// Layout@ggc_x86_64 => 208@[80,24,32,16,24,16,16]
typedef struct DynamicModel
{
    // The simulation file information
    FileInfo_t              FileInfo;

    // The simulation physical factor collection
    PhysicalInfo_t          PhysicalFactors;

    // The lattice energy buffer
    Flp64Buffer_t           LatticeEnergyBuffer;

    // The simulation runtime information
    SimulationRunInfo_t     RuntimeInfo;

    // The simulation environment lattice
    EnvironmentLattice_t    EnvironmentLattice;

    // The jump status array
    JumpStatusArray_t       JumpStatusArray;

    // The pair delta 3D table span. Access by [TableId][OriginalParticleId][NewParticleId][CenterParticleId]
    PairDeltaTables_t       PairDeltaTables;

} DynamicModel_t;

// Type for plugin function pointers
typedef void (*FPlugin_t)(void* restrict);

// Type for storing multiple plugin function pointers
// Layout@ggc_x86_64 => 16@[8,8]
typedef struct SimulationPlugins
{
    // The callback plugin function on data outputs
    FPlugin_t OnDataOutput;

    // The callback plugin function for setting the KMC transition energy value
    FPlugin_t OnSetTransitionStateEnergy;
    
} SimulationPlugins_t;

// Type for storing the programs cmd arguments
// Layout@ggc_x86_64 => 16@[8,4,{4}]
typedef struct CmdArguments
{
    // The command line arguments passed to the program
    char const* const*  Values;

    // The number of passed command line arguments
    int32_t             Count;

    // Padding integer
    int32_t             Padding:32;

} CmdArguments_t;

// Type for storing the program overwrites defined by CMD arguments
// Layout@ggc_x86_64 => 16@[8,4,{4}]
typedef struct CmdOverwrites
{
    //  An overwrite energy value in [eV] for the new upper limit of jump histograms
    double  JumpHistogramMaxValue;

} CmdOverwrites_t;

// Type for the full simulation context that provides access to all simulation data structures
// Layout@ggc_x86_64
typedef struct SimulationContext
{
    // The main simulation state. Stores the result collections
    SimulationState_t   MainState;

    // The dynamic simulation cycle state. Stores the current cycle information
    CycleState_t        CycleState;

    // The simulation database model. Loaded from the managed model system
    JobDbModel_t           DbModel;

    // The simulation dynamic model. Stores dynamically created simulation objects
    DynamicModel_t      DynamicModel;

    // The jump selection pool. Manages the statistical selection of jumps
    JumpSelectionPool_t SelectionPool;

    // The main random number generator
    Pcg32_t             Rng;

    // The simulation plugin collection. Stores the loaded plugin information
    SimulationPlugins_t Plugins;

    // The command line argument collection. Stores all passed command line information
    CmdArguments_t      CommandArguments;

    // Current main error code of the simulation
    error_t             ErrorCode;

    // Stores the last cycle outcome type (accepted, rejected, blocked, skipped, start unstable, end unstable)
    int32_t             CycleResult;

    // Stores the set CMD overwrites for the simulation
    CmdOverwrites_t     CmdOverwrites;

    // Marks if the simulation uses approximate EXP calculation
    bool_t              IsExpApproximationActive;

    //  Marks if the simulation does not log jump events into histograms
    bool_t              IsJumpLoggingDisabled;

} SimulationContext_t;

// Construct a new raw simulation context struct with relative path as IO and math.h exp as exp function
static inline SimulationContext_t ctor_SimulationContext()
{
    SimulationContext_t context;
    memset(&context, 0, sizeof(SimulationContext_t));
    context.DynamicModel.FileInfo.IODirectoryPath = ".";
    context.DynamicModel.FileInfo.ExtensionLookupPath = ".";
    context.CmdOverwrites.JumpHistogramMaxValue = NAN;
    return context;
}