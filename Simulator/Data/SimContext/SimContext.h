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
#include "Framework/Errors/McErrors.h"
#include "Framework/Basic/BaseTypes/BaseTypes.h"
#include "Framework/Basic/BaseTypes/Buffers.h"
#include "Framework/Math/Random/PcgRandom.h"
#include "Simulator/Data/Database/DbModel.h"
#include "Simulator/Data/State/StateModel.h"

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
    int32_t         EnvironmentId;
    int32_t         PairId;
    ClusterLinks_t  ClusterLinks;
    
} EnvironmentLink_t;

// Type for env link list that supports push back operation
// Layout@ggc_x86_64 => 24@[8,8,8]
typedef List_t(EnvironmentLink_t, EnvironmentLinks) EnvironmentLinks_t;

// Type for cluster states and affiliated backups
// Layout@ggc_x86_64 => 24@[4,4,8,8]
typedef struct ClusterState
{
    int32_t     CodeId;
    int32_t     CodeIdBackup;
    OccupationCode64_t   OccupationCode;
    OccupationCode64_t   OccupationCodeBackup;
    
} ClusterState_t;

// Type for lists of cluster states
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(ClusterState_t, ClusterStates) ClusterStates_t;

// Type for lists of energy states
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(double, EnergyStates) EnergyStates_t;

// Type for a full environment state definition (Does not support 16 bit alignment)
// Layout@ggc_x86_64 => 100@[1,1,1,1,4,4,4,16,4,,16,16,24,8]
typedef struct EnvironmentState
{
    bool_t                      IsMobile;
    bool_t                      IsStable;
    byte_t                      ParticleId;
    byte_t                      PathId;
    int32_t                     EnvironmentId;
    int32_t                     PoolId;
    int32_t                     PoolPositionId;
    Vector4_t                   PositionVector;
    int32_t                     MobileTrackerId;
    EnergyStates_t              EnergyStates;
    ClusterStates_t             ClusterStates;
    EnvironmentLinks_t          EnvironmentLinks;
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

    // The selected jump direction id
    int32_t JumpId;

    // The selected relative jump id
    int32_t RelativeId;

    // The selected offset id for an MMC transition
    int32_t OffsetId;
    
} JumpSelectionInfo_t;

// Type for the transition energy information
// Layout@ggc_x86_64 => 72@[8,8,8,8,8,8,8,8,8]
typedef struct JumpEnergyInfo
{
    double Energy0;
    double Energy1;
    double Energy2;
    double FieldInfluence;
    double ConformationDelta;
    double Energy0To2;
    double Energy2To0;
    double Probability0to2;
    double Probability2to0;
    
} JumpEnergyInfo_t;

// Type for the internal simulation cycle counters
// Layout@ggc_x86_64 => 48@[8,8,8,8,8,8]
typedef struct CycleCounterState
{
    // The total number of cycles
    int64_t CurrentCycles;

    // The total successful steps
    int64_t CurrentMcs;

    // The cycles per execution loop
    int64_t CyclesPerExecutionLoop;

    // The goal mcs per execution phase
    int64_t McsPerExecutionPhase;

    // The next total mcs an execution phase has to reach before entering the next write phase
    int64_t NextExecutionPhaseGoalMcs;

    // The simulation abort mcs count
    int64_t TotalSimulationGoalMcs;
    
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
// Layout@ggc_x86_64 => 48@[16]
typedef struct JumpStatus
{
    // The jump links of the jump status
    JumpLinks_t JumpLinks;

} JumpStatus_t;

// Type for a 4D array of jump status objects access by [A,B,C,JumpDirId]
// Layout@ggc_x86_64 => 24@[8,8,8]
typedef Array_t(JumpStatus_t, 4, JumpStatusArray) JumpStatusArray_t;

// Type for the cycle state storage. Contains all information manipulated and buffered during simulation cycles
// Layout@ggc_x86_64 => 248@[48,8,16,96,8,8,8,8,8,8,8,8,8]
typedef struct CycleState
{
    // The main counter state. Controls the cycle loop settings
    CycleCounterState_t         MainCounters;

    // The current active state code that describes a transition start state
    OccupationCode64_t            ActiveStateCode;

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

    // The pointer to the current pair table
    PairTable_t*                WorkPairTable;

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
// Layout@ggc_x86_64 => 56@[8,8,8,8,8,8,8]
typedef struct FileInfo
{
    // The database query string for data loading
    char const* JobDbQuery;

    // The program execution path
    char const* ExecutionPath;

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
// Layout@ggc_x86_64 => 168@[56,24,32,16,24,16]
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

} DynamicModel_t;

// Type for plugin function pointers
typedef void (*FPlugin_t)(void* restrict);

// Type for storing multiple plugin function pointers
// Layout@ggc_x86_64 => 16@[8,8]
typedef struct SimulationPlugins
{
    // The callback plugin function on data outputs
    FPlugin_t OnDataOutput;

    // The callback plugin function on set jump probabilities
    FPlugin_t OnSetJumpProbabilities;
    
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

// Type for the full simulation context that provides access to all simulation data structures
// Layout@ggc_x86_64 => 32@[4,]
typedef struct SimulationContext
{
    // The main simulation state. Stores the result collections
    SimulationState_t   MainState;

    // The dynamic simulation cycle state. Stores the current cycle information
    CycleState_t        CycleState;

    // The simulation database model. Loaded from the managed model system
    DbModel_t           DbModel;

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
    
} SimulationContext_t;

// Construct a new raw simulation context struct
static inline SimulationContext_t ctor_SimulationContext()
{
    SimulationContext_t context;
    memset(&context, 0, sizeof(SimulationContext_t));
    return context;
}