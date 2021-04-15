//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	JobDbModel.h        		    //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Db model data types         //
//////////////////////////////////////////

#pragma once
#include "Libraries/Framework/Errors/McErrors.h"
#include "Libraries/Framework/Basic/BaseTypes.h"
#include "Libraries/Framework/Math/Vectors.h"
#include "Libraries/Framework/Basic/Buffers.h"
#include "Libraries/Simulator/Logic/Helper/Constants.h"

/* General definitions */

// Union type for encoding state occupations with 64 bit integers (8 Particles max)
typedef union OccupationCode64 { int64_t Value; byte_t ParticleIds[8]; } OccupationCode64_t;

// Type for 1D index mappings
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(int32_t, IdMappingSpan) IdMappingSpan_t;

// Type for defining a range of unit cells (Supports 16 bit alignment)
// Layout@ggc_x86_64 => 16@[4,4,4,4]
typedef Vector4_t InteractionRange_t;

// Type for passing custom routine request and affiliated data
// Layout@ggc_x86_64 => 32@[16,16]
typedef struct RoutineData
{
    // The 16 bytes of the routine UUID/GUID
    byte_t      Guid[16];

    //  The custom routine parameter data span
    Buffer_t    ParamData;

} RoutineData_t;

/* Structure model */

// Type for pair interaction definitions
// Layout@ggc_x86_64 => 20@[16,4]
typedef struct PairInteraction
{
    // The relative 4D vector that points to the target
    Vector4_t   RelativeVector;

    // The energy table Id that the pair belongs to
    int32_t     EnergyTableId;

} PairInteraction_t;

// Type for cluster interaction definitions
// Layout@ggc_x86_64 => 36@[8x4,4]
typedef struct ClusterInteraction
{
    // The pair interaction ids that from the cluster
    int32_t     PairInteractionIds[8];

    // The energy table id that the cluster belongs to
    int32_t     EnergyTableId;

} ClusterInteraction_t;

// Span type for pair definitions
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(PairInteraction_t, PairInteractions) PairInteractions_t;

// Span type for cluster definitions
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(ClusterInteraction_t, ClusterInteractions) ClusterInteractions_t;

// Type for full environment definitions
// Layout@ggc_x86_64 => 176@[4,{4},8,16,16,64,64]
typedef struct EnvironmentDefinition
{
    // The object id of the environment. Is equal to the position id
    int32_t                     PositionId;

    // Padding
    uint32_t                    Padding;

    // The particle mask of center positions that should be put into the selection pool
    Bitmask_t                   SelectionParticleMask;

    // The pair interaction collection
    PairInteractions_t          PairInteractions;

    // The cluster interaction collection
    ClusterInteractions_t       ClusterInteractions;

    // Position particle id byte buffer, encodes possible particles
    // First one that is an invalid index terminates the set
    byte_t                      PositionParticleIds[PARTICLE_IDLIMIT];

    // Update particle id buffer, encodes energy update particles
    // First one that is an invalid index terminates the set
    byte_t                      UpdateParticleIds[PARTICLE_IDLIMIT];

} EnvironmentDefinition_t;

// Span of environment definitions
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(EnvironmentDefinition_t, EnvironmentDefinitions) EnvironmentDefinitions_t;

// Type for the unit cell vector collection
// Layout@ggc_x86_64 => 72@[3x24]
typedef struct UnitCellVectors
{
    // The cartesian unit cell vector for direction A in [Ang]
    Vector3_t   A;

    // The cartesian unit cell vector for direction B in [Ang]
    Vector3_t   B;

    // The cartesian unit cell vector for direction C in [Ang]
    Vector3_t   C;

} UnitCellVectors_t;

// Type for the structure meta data that contains non essential structure information
// Layout@ggc_x86_64 => 608@[64x8,8,72]
typedef struct StructureMetaData
{
    // The charge values of the particles in units of [C]
    double              ParticleCharges[PARTICLE_IDLIMIT];

    // The normalized electric field vector in cartesian coordinates
    Vector3_t           NormElectricFieldVector;

    // The unit cell vector collection in units of [Ang]
    UnitCellVectors_t   CellVectors;

} StructureMetaData_t;

// Type for the structure model
// Layout@ggc_x86_64 => 48@[4,4,8,16,16]
typedef struct StructureModel
{
    // The number of required static trackers per cell
    int32_t                         StaticTrackersPerCellCount;

    // The number of required global trackers
    int32_t                         GlobalTrackerCount;

    // The pointer to the structure meta data
    StructureMetaData_t*            MetaData;

    // The interaction range cube for MMC
    InteractionRange_t              InteractionRange;

    // The environment definition collection
    EnvironmentDefinitions_t        EnvironmentDefinitions;
    
} StructureModel_t;

/* Energy model */

// Type for 2d rectangular energy tables
// Layout@ggc_x86_64 => 24@[8,8,8]
typedef Array_t(double, 2, EnergyTable) EnergyTable_t;

// Type for lists of occupation codes
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(OccupationCode64_t, OccupationCodes64) OccupationCodes64_t;

// Type for pair tables to store energies of pair interactions
// Layout@ggc_x86_64 => 32@[24,4,{4}]
typedef struct PairTable
{
    // The 2D energy table of the pair table
    // Access by [ParticleId,ParticleId]
    EnergyTable_t   EnergyTable;

    // The object id of the pair table
    int32_t         ObjectId;

    // Padding, used for temporary flags during initialization
    uint32_t        Padding;

} PairTable_t;

// Type for cluster tables to store energies of pair interactions
// Layout@ggc_x86_64 => 112@[16,24,4,64,{4}]
typedef struct ClusterTable
{
    // The occupation code collection in order of [OccCodeId]
    OccupationCodes64_t      OccupationCodes;

    // The energy table of the cluster.
    // Access by [TableId,OccCodId]
    EnergyTable_t           EnergyTable;

    // The object id
    int32_t                 ObjectId;

    // The particle table mapping. Assigns each particle id its valid sub table in the cluster table
    // Access by [ParticleId]
    byte_t                  ParticleTableMapping[PARTICLE_IDLIMIT];

    // Padding, used for temporary flags during initialization
    uint32_t                Padding;
    
} ClusterTable_t;

// Span type for pair tables
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(PairTable_t, PairTables) PairTables_t;

// Span type for cluster tables
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(ClusterTable_t, ClusterTables) ClusterTables_t;

// Type for the double 2D rectangular energy defect access [positionId][particleId]
// Layout@ggc_x86_64 => 24@[8,8,8]
typedef Array_t(double, 2, DefectBackground) DefectBackground_t;

// Type for the energy model
// Layout@ggc_x86_64 => 48@[16,16,16]
typedef struct EnergyModel
{
    // The collection of pair energy tables.
    // Access by [TableId] of a pair interaction
    PairTables_t        PairTables;

    // The collection of cluster energy tables
    // Access by [TableId] of a cluster interaction
    ClusterTables_t     ClusterTables;

    //  The defect energy background 2D array
    // Access by [PositionId][ParticleId]
    DefectBackground_t  DefectBackground;

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
typedef Span_t(Vector3_t, MoveSequence) MoveSequence_t;

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
    MoveSequence_t  MovementSequence;
    
} JumpDirection_t;

// Span type for jump directions
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(JumpDirection_t, JumpDirections) JumpDirections_t;

// Type for a transition jump rule
// Layout@ggc_x86_64 => 56@[8,8,8,8,8,8,8]
typedef struct JumpRule
{
    // The occupation code for the start state
    OccupationCode64_t   StateCode0;

    // The occupation code for the transition state
    OccupationCode64_t   StateCode1;

    // The occupation code for the final state
    OccupationCode64_t   StateCode2;

    // The attempt frequency factor that describes the fraction of the frequency modulus that is applied
    double               FrequencyFactor;

    // The electric field rule factor that encodes direction
    double               ElectricFieldFactor;

    // The virtual path jump energy correction value that corrects the biased S2 calculation. Is NaN if no universally valid values exists for the transition
    double               StaticVirtualJumpEnergyCorrection;

    // The tracker order code that encodes the tracker reordering
    byte_t               TrackerOrderCode[JUMPS_JUMPLENGTH_MAX];
    
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
    uint32_t             Padding;

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
    JumpCountTable_t        JumpCountMappingTable;

    // The jump assign table, assigns each [PositionId,ParticleId,RelativeJumpId] the affiliated jump direction id
    JumpMappingTable_t      JumpDirectionMappingTable;

    // The static tracker assign table, assigns each [PositionId,ParticleId] a static tracker index offset
    TrackerMappingTable_t   StaticTrackerMappingTable;

    // The global tracker assign table, assigns each [JumpCollectionId,ParticleId] a global tracker index
    TrackerMappingTable_t   GlobalTrackerMappingTable;

} TransitionModel_t;

/* Job model */

// Type for the mmc job header
// Layout@ggc_x86_64 => 32@[8,8,4,4,4,{4}]
typedef struct MmcHeader
{
    // Bitmask for MMC specific job flags
    Bitmask_t   JobFlags;

    // The relative energy abort tolerance
    double      AbortTolerance;

    // The sequence length of the abort test
    int32_t     AbortSequenceLength;

    // The sample length of the abort test
    int32_t     AbortSampleLength;

    // The sample interval fro the abort test
    int32_t     AbortSampleInterval;

    // Padding
    uint32_t     Padding;
    
} MmcHeader_t;

// Type for the kmc job header
// Layout@ggc_x86_64 => 40@[8,8,8,8,4,{4}]
typedef struct KmcHeader
{
    // Bitmask for KMC specific job flags
    Bitmask_t   JobFlags;

    // The electric field modulus
    double      ElectricFieldModulus;

    // The main attempt frequency modulus
    double      AttemptFrequencyModulus;

    // Fixed norm factor for the simulation
    double      FixedNormalizationFactor;

    // Pre-run mcsp before the main simulation
    int32_t     PreRunMcsp;

    // Padding
    uint32_t     Padding;

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

    // The random number generator start state
    uint64_t    RngStartState;

    // The random number generator increase value
    uint64_t    RngIncValue;

    // The simulation temperature in kelvin
    double      Temperature;

    // The lower limit of success rate before the simulation terminates
    double      MinimalSuccessRate;

    // Pointer to the job header
    void *      JobHeader;

    // The object id (Is the job id)
    int32_t     ObjectId;

    // Padding
    uint32_t     Padding;
    
} JobInfo_t;

// Type for the job model
// Layout@ggc_x86_64 => 72@[4,4,4,4,4,4,8,8,32]
typedef struct JobModel
{
    // The objects database context key
    int32_t         ContextId;

    // The simulation package context id
    int32_t         PackageId;

    // The lattice model context id
    int32_t         LatticeModelId;

    // The structure model context id
    int32_t         StructureModelId;

    // The energy model context id
    int32_t         EnergyModelId;

    // The transition model context id
    int32_t         TransitionModelId;

    // The job info object
    JobInfo_t       JobInfo;

    // The job header pointer
    void*           JobHeader;

    // Additional routine data for custom non-standard routines
    RoutineData_t   RoutineData;

} JobModel_t;

/* Lattice model */

// Type for the byte based 4d rectangular lattice access
// Layout@ggc_x86_64 => 24@[8,8,8]
typedef Array_t(byte_t, 4, Lattice) Lattice_t;

// Type for the double 5D rectangular energy background access [a][b][c][positionId][particleId]
// Layout@ggc_x86_64 => 24@[8,8,8]
typedef Array_t(double, 5, EnergyBackground) EnergyBackground_t;

// Type for the lattice meta information
// Layout@ggc_x86_64 => 24@[16,4,4]
typedef struct LatticeInfo
{
    // The size information for the lattice as a 4d vector
    Vector4_t           SizeVector;

    // The number of mobiles in the lattice
    int32_t             MobileParticleCount;

    // The number of select-able particles in the lattice
    int32_t             SelectParticleCount;

} LatticeInfo_t;

// Type for the lattice model (Supports 16 bit alignment)
// Layout@ggc_x86_64 => 80@[24,24,24,{8}]
typedef struct LatticeModel
{
    // The lattice info. Contains lattice model meta data
    LatticeInfo_t       LatticeInfo;

    // The 4D simulation lattice start state
    // Access by [A,B,C,D]
    Lattice_t           Lattice;

    // The 5D energy background
    // Access by [A,B,C,D,ParticleId]
    EnergyBackground_t  EnergyBackground;

    // Padding
    uint64_t             Padding;

} LatticeModel_t;

/* Database model */

// Type for the database model context
// Layout@ggc_x86_64 => 336@[80,72,56,48,80]
typedef struct JobDbModel
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

} JobDbModel_t;