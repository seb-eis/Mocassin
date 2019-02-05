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
#include "Simulator/Data/Model/Database/DbModel.h"
#include "Simulator/Data/Model/State/StateModel.h"

// Type for cluster links
// Layout@ggc_x86_64 => 2@[1,1]
typedef struct ClusterLink
{
    byte_t ClusterId;
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
typedef Array_t(EnvironmentState_t, 4, EnvironmentLattice) EnvironmentLattice;

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
// Layout@ggc_x86_64 => 48@[8,8,8,8,8,8]
typedef struct JumpEnergyInfo
{
    double Energy0;
    double Energy1;
    double Energy2;
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
    int32_t             NumOfSelectableJumps;
    int32_t             NumOfDirectionPools;
    IdRedirection_t     NumOfDirectionsToPoolId;
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

// Type for the simulation dynamic model
// Layout@ggc_x86_64 => 152@[56,24,32,16,24]
typedef struct DynamicModel
{
    FileInfo_t          FileInfo;
    PhysicalInfo_t      PhysicalFactors;
    Flp64Buffer_t       LatticeEnergyBuffer;
    SimulationRunInfo_t RuntimeInfo;
    EnvironmentLattice  EnvironmentLattice;

} DynamicModel_t;

// Type for plugin function pointers
typedef void (*FPlugin_t)(void* restrict);

// Type for storing multiple plugin function pointers
// Layout@ggc_x86_64 => 16@[8,8]
typedef struct SimulationPlugins
{ 
    FPlugin_t OnDataOutput;
    FPlugin_t OnSetJumpProbabilities;
    
} SimulationPlugins_t;

// Type for storing the programs cmd arguments
// Layout@ggc_x86_64 => 16@[8,4,{4}]
typedef struct CmdArguments
{
    char const* const*  Values;
    int32_t             Count;

    int32_t             Padding:32;

} CmdArguments_t;

// Type for the full simulation context that provides access to all simulation data structures
// Layout@ggc_x86_64 => 32@[4,]
typedef struct SimulationContext
{
    SimulationState_t   MainState;
    CycleState_t        CycleState;
    DbModel_t           DbModel;
    DynamicModel_t      DynamicModel;
    JumpSelectionPool_t SelectionPool;
    Pcg32_t      RandomNumberGenerator;
    SimulationPlugins_t Plugins;
    CmdArguments_t      CommandArguments;
    error_t             ErrorCode;
    
} SimulationContext_t;