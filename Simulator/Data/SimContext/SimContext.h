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
    OccCode_t   OccupationCode;
    OccCode_t   OccupationCodeBackup;
    
} ClusterState_t;

// Type for lists of cluster states
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(ClusterState_t, ClusterStates) ClusterStates_t;

// Type for lists of energy states
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(double, EnergyStates) EnergyStates_t;

// Type for a full environment state definition (Supports 16 bit alignment)
// Layout@ggc_x86_64 => 96@[1,1,1,1,4,4,4,16,16,16,24,8]
typedef struct EnvironmentState
{
    bool_t                      IsMobile;
    byte_t                      IsStable;
    byte_t                      ParticleId;
    byte_t                      PathId;
    int32_t                     EnvironmentId;
    int32_t                     PoolId;
    int32_t                     PoolPositionId;
    Vector4_t                   PositionVector;
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
    int32_t EnvironmentId;
    int32_t JumpId;
    int32_t RelativeId;
    int32_t OffsetId;
    
} JumpSelectionInfo_t;

// Type for the transition energy information
// Layout@ggc_x86_64 => 56@[8,8,8,8,8,8,8]
typedef struct JumpEnergyInfo
{
    double Energy0;
    double Energy1;
    double Energy2;
    double FieldInfluence;
    double ConformationDelta;
    double Probability0to2;
    double Probability2to0;
    
} JumpEnergyInfo_t;

// Type for the internal simulation cycle counters
// Layout@ggc_x86_64 => 48@[8,8,8,8,8,8]
typedef struct CycleCounterState
{
    int64_t Cycles;
    int64_t Mcs;
    int64_t CyclesPerBlock;
    int64_t McsPerBlock;
    int64_t StepGoalMcs;
    int64_t TotalGoalMcs;
    
} CycleCounterState_t;

// Type for the path energy backups
// Layout@ggc_x86_64 => 64@[8x8]
typedef struct EnvironmentBackup
{
    double PathEnergies[8];
    
} EnvironmentBackup_t;

// Type for the cycle state storage
// Layout@ggc_x86_64 => 208@[48,8,16,64,8,8,8,8,8,8,8,8]
typedef struct CycleState
{
    CycleCounterState_t         MainCounters;
    OccCode_t                   ActiveStateCode;
    JumpSelectionInfo_t         ActiveSelectionInfo;
    JumpEnergyInfo_t            ActiveEnergyInfo;
    EnvironmentBackup_t         ActiveEnvironmentBackup;
    JumpDirection_t*            ActiveJumpDirection;
    JumpCollection_t*           ActiveJumpCollection;
    JumpRule_t*                 ActiveJumpRule;
    StateCounterCollection_t*   ActiveCounterCollection;
    EnvironmentState_t*         ActivePathEnvironments[8];
    EnvironmentState_t*         WorkEnvironment;
    ClusterState_t*             WorkCluster;
    PairTable_t*                WorkPairTable;
    ClusterTable_t*             WorkClusterTable;

} CycleState_t;

// Type for the environment pool access
// Layout@ggc_x86_64 => 24@[8,8,8]
typedef List_t(int32_t, EnvironmentPool) EnvironmentPool_t;

// Type for the direction pools
// Layout@ggc_x86_64 => 40@[24,4,4,4,{4}]
typedef struct DirectionPool
{
    EnvironmentPool_t   EnvironmentPool;
    int32_t             NumOfPositions;
    int32_t             NumOfDirections;
    int32_t             NumOfJumps;

    int32_t             Padding:32;
    
} DirectionPool_t;

// Type for lists of direction pools
// Layout@ggc_x86_64 => 40@[24,4,4,4,{4}]
typedef Span_t(DirectionPool_t, DirectionPools) DirectionPools_t;

// Type for the jump selection pool
// Layout@ggc_x86_64 => 64@[4,4,16,40]
typedef struct JumpSelectionPool
{
    int32_t             SelectableJumpCount;
    int32_t             DirectionPoolCount;
    IdRedirection_t     DirectionPoolMapping;
    DirectionPools_t    DirectionPools;
    
} JumpSelectionPool_t;

// Type for the program run information
// Layout@ggc_x86_64 => 16@[8,8]
typedef struct SimulationRunInfo
{
    int64_t StartClock;
    int64_t LastClock;

} SimulationRunInfo_t;

// Type for physical simulation values
// Layout@ggc_x86_64 => 24@[8,8,8]
typedef struct PhysicalInfo
{
    double EnergyConversionFactor;
    double TotalNormalizationFactor;
    double CurrentTimeStepping;
    
} PhysicalInfo_t;

// Type for the file string information
// Layout@ggc_x86_64 => 56@[8,8,8,8,8,8,8]
typedef struct FileInfo
{
    char const* DbQueryString;
    char const* ExecutionPath;
    char const* DatabasePath;
    char const* OutputPluginPath;
    char const* OutputPluginSymbol;
    char const* EnergyPluginPath;
    char const* EnergyPluginSymbol;
    
} FileInfo_t;

// Type for floating point buffers with storage of last average
// Layout@ggc_x86_64 => 32@[8,8,8,8]
typedef struct Flp64Buffer
{
    double* Begin;
    double* End;
    double* CapacityEnd;
    double  LastAverage;
    
} Flp64Buffer_t;

// Type for jump links
// Layout@ggc_x86_64 => 8@[4,4]
typedef struct JumpLink
{
    // The path id te jump link is valid for
    int32_t     PathId;

    // The id of the corresponding environment link
    int32_t     LinkId;

} JumpLink_t;

// Type for jump link lists
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(JumpLink_t, JumpLinks) JumpLinks_t;

// Type for the jump status that holds the runtime information of a single jump
// Layout@ggc_x86_64 => 16@[16]
typedef struct JumpStatus
{
    // The jump links of the jump status
    JumpLinks_t JumpLinks;

} JumpStatus_t;

// Type for a 4D array of jump status objects access by [A,B,C,JumpDirId]
// Layout@ggc_x86_64 => 24@[8,8,8]
typedef Array_t(JumpStatus_t, 4, JumpStatusArray) JumpStatusArray_t;

// Type for the simulation dynamic model
// Layout@ggc_x86_64 => 168@[56,24,32,16,24,16]
typedef struct DynamicModel
{
    // The simulation file information
    FileInfo_t          FileInfo;

    // The simulation physical factor collection
    PhysicalInfo_t      PhysicalFactors;

    // The lattice energy buffer
    Flp64Buffer_t       LatticeEnergyBuffer;

    // The simulation runtime information
    SimulationRunInfo_t RuntimeInfo;

    // The simulation environment lattice
    EnvironmentLattice_t  EnvironmentLattice;

    // The jump status array
    JumpStatusArray_t   JumpStatusArray;

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