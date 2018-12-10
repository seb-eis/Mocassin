//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	DbModel.h        		    //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Db model data types         //
//////////////////////////////////////////

#pragma once
#include "Framework/Errors/McErrors.h"
#include "Framework/Basic/BaseTypes/BaseTypes.h"
#include "Framework/Math/Types/Vector.h"
#include "Framework/Basic/BaseTypes/Buffers.h"

/* General */

// Type for encoding state occupations
typedef int64_t OccCode_t;

// Type for index redirection lists
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(int32_t, IdRedirection) IdRedirection_t;

// Type for tracking movement (Supports 16 bit alignment)
// Layout@ggc_x86_64 => 32@[8,8,8,8]
typedef Vector3_t MoveVector_t;

// Type for defining a range of unit cells (Supports 16 bit alignment)
// Layout@ggc_x86_64 => 16@[16]
typedef Vector4_t InteractionRange_t;

// Type for blob loading from the database
// Layout@ggc_x86_64 => 24@[8,4,4,4,{4}]
typedef struct DbBlob
{
    // Buffer pointer
    void*       Buffer;

    // The db context key of the object
    int32_t     Key;

    // The number of bytes in the header
    int32_t     HeaderSize;

    // The size of the blob
    int32_t     BlobSize;

    // Padding integer
    int32_t     Padding:32;

} DbBlob_t;

/* Structure model */

// Type for pair interaction definitions (Supports 16 bit alignment)
// Layout@ggc_x86_64 => 32@[20,4,{8}]
typedef struct PairDefinition
{
    // The relative 4D vector that points to the target
    Vector4_t   RelativeVector;

    // The energy table Id that the pair belongs to
    int32_t     EnergyTableId;

    // Padding
    int64_t     Padding:64;

} PairDefinition_t;

// Type for cluster interaction definitions
// Layout@ggc_x86_64 => 40@[8x4,4,{4}]
typedef struct ClusterDefinition
{
    // The pair interaction ids that from the cluster
    int32_t     EnvironmentPairIds[8];

    // The energy table id that the cluster belongs to
    int32_t     EnergyTableId;

    // Padding
    int32_t     Padding:32;

} ClusterDefinition_t;

// Span type for pair definitions
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(PairDefinition_t, PairDefinitions) PairDefinitions_t;

// Span type for cluster definitions
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(ClusterDefinition_t, ClusterDefinitions) ClusterDefinitions_t;

// Type for full environment definitions
// Layout@ggc_x86_64 => 168@[4,{4},16,16,64,64]
typedef struct EnvironmentDefinition
{
    // The object id of the environment. Is equal to the position id
    int32_t                 ObjectId;

    // Padding
    int32_t                 Padding:32;

    // The pair definition collection
    PairDefinitions_t       PairDefinitions;

    // Teh cluster defintion collection
    ClusterDefinitions_t    ClusterDefinitions;

    // Position particle id byte buffer, encodes possible particles
    // First one that is an invalid index terminates the set
    byte_t                  PositionParticleIds[64];

    // Update particle id buffer, encodes energy update particles
    // First one that is an invalid index terminates the set
    byte_t                  UpdateParticleIds[64];

} EnvironmentDefinition_t;

// Span of environment definitions
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(EnvironmentDefinition_t, EnvironmentDefinitions_t) EnvironmentDefinitions_t;

// Type for the structure model
// Layout@ggc_x86_64 => 40@[4,4,16,16]
typedef struct StructureModel
{
    // The number of required static trackers per cell
    int32_t                     NumOfTrackersPerCell;

    // The number of required global trackers
    int32_t                     NumOfGlobalTrackers;

    // The interaction range cube for MMC
    InteractionRange_t          InteractionRange;

    // The environment definition collection
    EnvironmentDefinitions_t    EnvironmentDefinitions;
    
} StructureModel_t;

/* Energy model */

// Type for 2d rectangular energy tables
// Layout@ggc_x86_64 => 24@[8,8,8]
typedef Array_t(double, 2, EnergyTable) EnergyTable_t;

// Type for lists of occupation codes
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(OccCode_t, OccCodes) OccCodes_t;

// Type for pair tables to store energies of pair interactions
// Layout@ggc_x86_64 => 32@[24,4,{4}]
typedef struct PairTable
{
    // The 2D energy table of the pair table
    // Access by [ParticleId,ParticleId]
    EnergyTable_t   EnergyTable;

    // The object id of the pair table
    int32_t         ObjectId;

    // Padding
    int32_t         Padding:32;

} PairTable_t;

// Type for cluster tables to store energies of pair interactions
// Layout@ggc_x86_64 => 112@[16,24,4,64,{4}]
typedef struct ClusterTable
{
    // The occupation code collection in order of [OccCodeId]
    OccCodes_t      OccupationCodes;

    // The energy table of the cluster.
    // Access by [TableId,OccCodId]
    EnergyTable_t   EnergyTable;

    // The object id
    int32_t         ObjectId;

    // The particle to table id mapping.
    // Access by [ParticleId]
    byte_t          ParticleToTableId[64];

    // Padding
    int32_t         Padding:32;
    
} ClusterTable_t;

// Span type for pair tables
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(PairTable_t, PairTables) PairTables_t;

// Span type for cluster tables
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(ClusterTable_t, ClusterTables) ClusterTables_t;

// Type for the energy model
// Layout@ggc_x86_64 => 32@[16,16]
typedef struct EnergyModel
{
    // The collection of pair energy tables.
    // Access by [TableId] of a pair interaction
    PairTables_t        PairTables;

    // The collection of cluster energy tables
    // Access by [TableId] of a cluster interaction
    ClusterTables_t     ClusterTables;
    
} EnergyModel_t;

/* Transition model */

// Type for 2d rectangular jump count tables
// Layout@ggc_x86_64 => 24@[8,8,8]
typedef Array_t(int32_t, 2, JumpCountTable) JumpCountTable_t;

// Type for 2d tracker index assignment by two separate index values
// Layout@gcc_x86_64 => 24[8,8,8]
typedef Array_t(int32_t, 2, TrackerMappingTable) TrackerMappingTable_t;

// Type for 3d jump id assignment tables
// Layout@ggc_x86_64 => 24@[8,8,8]
typedef Array_t(int32_t, 3, JumpMappingTable) JumpMappingTable_t;

// Span type for jump sequences
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(Vector4_t, JumpSequence) JumpSequence_t;

// Span type for movement sequences
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(MoveVector_t, MoveSequence) MoveSequence_t;

// Type for jump direction definitions
// Layout@ggc_x86_64 => 56@[4,4,4,4,8,16,16]
typedef struct JumpDirection
{
    // The object id
    int32_t         ObjectId;

    // The position id the jump is valid fro
    int32_t         PositionId;

    // The jump collection id
    int32_t         JumpCollectionId;

    // The number of path entries of the jump
    int32_t         JumpLength;

    // The electric field influence factor
    double          ElectricFieldFactor;

    // The jump sequence for position lookup
    JumpSequence_t  JumpSequence;

    // The movement sequence for tracking
    MoveSequence_t  LocalMoveSequence;
    
} JumpDirection_t;

// Span type for jump directions
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(JumpDirection_t, JumpDirections) JumpDirections_t;

// Type for a transition jump rule
// Layout@ggc_x86_64 => 48@[8,8,8,8,8,8]
typedef struct JumpRule
{
    // The occupation code for the start state
    OccCode_t   StateCode0;

    // The occupation code for the transition state
    OccCode_t   StateCode1;

    // The occupation code for the final state
    OccCode_t   StateCode2;

    // The attempt frequency factor
    double      FrequencyFactor;

    // The electric field rule factor that encodes direction
    double      ElectricFieldFactor;

    // The tracker order code that encodes the tracker reordering
    byte_t      TrackerOrderCode[8];
    
} JumpRule_t;

// Span type for jump rules
// Layout@ggc_x86_64 => 16@[8,8}]
typedef Span_t(JumpRule_t, JumpRules) JumpRules_t;

// Type for jump collections
// Layout@ggc_x86_64 => 48@[8,16,16,4,{4}]
typedef struct JumpCollection
{
    // The particle masks that defines all mobile particles of the collection
    Bitmask_t           MobileParticlesMask;

    // The collection of affiliated jump directions
    JumpDirections_t    JumpDirections;

    // The collection of existing jump rules
    JumpRules_t         JumpRules;

    // The object id
    int32_t             ObjectId;

    // Padding
    int32_t             Padding:32;

} JumpCollection_t;

// Type for jump collection lists
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(JumpCollection_t, JumpCollections) JumpCollections_t;

// Type for the transition model
// Layout@ggc_x86_64 => 128@[16,16,24,24,24,24]
typedef struct TransitionModel
{
    // The set of jump collections that exist
    JumpCollections_t       JumpCollections;

    // The set of jump directions that exist in order of jump index and grouped by the jump collection
    JumpDirections_t        JumpDirections;

    // The jump count table, assigns each [PositionId,ParticleId] the number of selectable jumps
    JumpCountTable_t        JumpCountTable;

    // The jump assign table, assigns each [PositionId,ParticleId,RelativeJumpId] the affiliated jump direction id
    JumpMappingTable_t       JumpAssignTable;

    // The static tracker assign table, assigns each [PositionId,ParticleId] a static tracker index offset
    TrackerMappingTable_t    StaticTrackerAssignTable;

    // The global tracker assign table, assigns each [JumpCollectionId,ParticleId] a global tracker index
    TrackerMappingTable_t    GlobalTrackerAssignTable;

} TransitionModel_t;

/* Job model */

// Type for the mmc job header
// Layout@ggc_x86_64 => 32@[8,8,4,4,4,{4}]
typedef struct MmcHeader
{
    // Bitmask fro MMC specific job flags
    Bitmask_t   JobFlags;

    // The relative energy abort tolerance
    double      AbortTolerance;

    // The sequence length of the abort test
    int32_t     AbortSequenceLength;

    // The sample length of the abort test
    int32_t     AbortSampleLength;

    // The smaple interval fro the abort test
    int32_t     AbortSampleInterval;

    // Padding
    int32_t     Padding:32;
    
} MmcHeader_t;

// Type for the kmc job header
// Layout@ggc_x86_64 => 32@[8,8,8,8]
typedef struct KmcHeader
{
    // Bitmask for KMC specific job flags
    Bitmask_t   JobFlags;

    // The electric field modulus
    double      ElectricFieldModulus;

    // The main attempt frequency modulus
    double      AttemptFrequencyModulus;

    // Fixed norm factor for the simulation
    double      FixedNormFactor;

} KmcHeader_t;

// Type for the job info
// Layout@ggc_x86_64 => 88@[8,8,8,8,8,8,8,8,8,8,4,{4}]
typedef struct JobInfo
{
    // Bitmask for the general job flags
    Bitmask_t   JobFlags;

    // Bitmask for the general status flags
    Bitmask_t   StatusFlags;

    // The number of bytes in the main state
    int64_t     StateSize;

    // The target MCSP of the simulation
    int64_t     TargetMcsp;

    // The save run time limit in seconds
    int64_t     TimeLimit;

    // The random number generator state seed
    uint64_t    RngStateSeed;

    // The random number generator increase seed
    uint64_t    RngIncSeed;

    // The simulation temperature in kelvin
    double      Temperature;

    // The lower limit of success rate before the simulation terminates
    double      MinimalSuccessRate;

    // Pointer to the job header
    void *      JobHeader;

    // The object id (Is the job id)
    int32_t     ObjectId;

    // Padding
    int32_t     Padding:32;
    
} JobInfo_t;

// Type for the job model
// Layout@ggc_x86_64 => 40@[4,4,4,4,4,4,8,8]
typedef struct JobModel
{
    // The objects database context key
    int32_t     ContextId;

    // The simulation package context id
    int32_t     PackageId;

    // The lattice model context id
    int32_t     LatticeModelId;

    // The structure model context id
    int32_t     StructureModelId;

    // The energy model context id
    int32_t     EnergyModelId;

    // The transition model context id
    int32_t     TransitionModelId;

    // The job info object
    JobInfo_t   JobInfo;

    // The job header pointer
    void*       JobHeader;

} JobModel_t;

/* Lattice model */

// Type for the byte based 4d rectangular lattice access
// Layout@ggc_x86_64 => 24@[8,8,8]
typedef Array_t(byte_t, 4, Lattice) Lattice_t;

// Type for the double 5D rectangular energy background access
typedef Array_t(double, 5, EnergyBackground) EnergyBackground_t;

// Type for the lattice model (Supports 16 bit alignment)
// Layout@ggc_x86_64 => 80@[16,4,4,24,24,{8}]
typedef struct LatticeModel
{
    // The size information for the lattice as a 4d vector
    Vector4_t           SizeVector;

    // The number of mobiles in the lattice
    int32_t             NumOfMobiles;

    // The number of selectables in the lattice
    int32_t             NumOfSelectables;

    // The 4D simulation lattice start state
    // Access by [A,B,C,D]
    Lattice_t           Lattice;

    // The 5D energy background
    // Access by [A,B,C,D,ParticleId]
    EnergyBackground_t  EnergyBackground;

    // Padding
    int64_t             Padding:64;

} LatticeModel_t;

/* Database model */

// Type for the database model context
// Layout@ggc_x86_64 => 320@[80,72,56,32,80]
typedef struct DbModel
{
    // The lattice model
    LatticeModel_t      LatticeModel;

    // The job model
    JobModel_t          JobModel;

    // The structure model
    StructureModel_t    StructureModel;

    // The energy model
    EnergyModel_t       EnergyModel;

    // The transition model
    TransitionModel_t   TransitionModel;

} DbModel_t;