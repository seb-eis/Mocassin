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

typedef int64_t OccCode_t;

// Type for index redirection lists
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(int32_t, IdRedirection) IdRedirection_t;

// Type for tracking movement (Supports 16 bit alignment)
// Layout@ggc_x86_64 => 32@[8,8,8,4,{4}]
typedef struct MoveVector
{
    Vector3_t   Vector;
    int32_t     TrackerId;

    int32_t     Padding:32;

} MoveVector_t;

// Type for defining a range of unit cells (Supports 16 bit alignment)
// Layout@ggc_x86_64 => 16@[16]
typedef struct InteractionRange
{ 
    Vector4_t   Vector;

} InteractionRange_t;

// Type for blob loading from the database
// Layout@ggc_x86_64 => 24@[8,4,4,4,{4}]
typedef struct DbBlob
{
    void*       Buffer;
    int32_t     Key;
    int32_t     HeaderSize;
    int32_t     BlobSize;
    int32_t     Padding:32;

} DbBlob_t;

/* Structure model */

// Type for pair interaction definitions (Supports 16 bit alignment)
// Layout@ggc_x86_64 => 32@[20,4,{8}]
typedef struct PairDefinition
{
    Vector4_t   RelativeVector;
    int32_t     TableId;

    int64_t     Padding:64;

} PairDefinition_t;

// Type for cluster interaction definitions
// Layout@ggc_x86_64 => 40@[8x4,4,{4}]
typedef struct ClusterDefinition
{ 
    int32_t     EnvironmentPairIds[8];
    int32_t     TableId;

    int32_t     Padding:32;

} ClusterDefinition_t;

// List type for pair definitions
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(PairDefinition_t, PairDefinitions) PairDefinitions_t;

// List type for cluster definitions
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(ClusterDefinition_t, ClusterDefinitions) ClusterDefinitions_t;

// Type for full environment definitions
// Layout@ggc_x86_64 => 168@[4,{4},16,16,64,64]
typedef struct EnvironmentDefinition
{
    int32_t                 ObjId;

    int32_t                 Padding:32;

    PairDefinitions_t       PairDefinitions;
    ClusterDefinitions_t    ClusterDefinitions;
    byte_t                  PositionParticleIds[64];
    byte_t                  UpdateParticleIds[64];

} EnvironmentDefinition_t;

// Type for lists of environment definitions
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(EnvironmentDefinition_t, EnvironmentDefinitions_t) EnvironmentDefinitions_t;

// Type for the structure model
// Layout@ggc_x86_64 => 56@[4,4,16,16,16]
typedef struct StructureModel
{
    int32_t                     NumOfTrackersPerCell;
    int32_t                     NumOfGlobalTrackers;
    InteractionRange_t          InteractionRange;
    IdRedirection_t             PositionIdToCellTrackerOffset;
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
    EnergyTable_t   EnergyTable;
    int32_t         ObjectId;

    int32_t         Padding:32;

} PairTable_t;

// Type for cluster tables to store energies of pair interactions
// Layout@ggc_x86_64 => 112@[16,24,4,64,{4}]
typedef struct ClusterTable
{
    OccCodes_t      OccupationCodes;
    EnergyTable_t   EnergyTable;
    int32_t         ObjectId;
    byte_t          ParticleToTableId[64];

    int32_t         Padding:32;
    
} ClusterTable_t;

// List type for pair table access
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(PairTable_t, PairTables) PairTables_t;

// List type for cluster table access
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(ClusterTable_t, ClusterTables) ClusterTables_t;

// Type for the energy model
// Layout@ggc_x86_64 => 32@[16,16]
typedef struct EnergyModel
{
    PairTables_t        PairTables;
    ClusterTables_t     ClusterTables;
    
} EnergyModel_t;

/* Transition model */

// Type for 2d rectangular jump count tables
// Layout@ggc_x86_64 => 24@[8,8,8]
typedef Array_t(int32_t, 2, JumpCountTable) JumpCountTable_t;

// Type for 3d jump id assignment tables
// Layout@ggc_x86_64 => 24@[8,8,8]
typedef Array_t(int32_t, 3, JumpAssignTable) JumpAssignTable_t;

// Type for jump sequence lists
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(Vector4_t, JumpSequence) JumpSequence_t;

// Type for jump links
// Layout@ggc_x86_64 => 8@[4,4]
typedef struct JumpLink
{
    int32_t     PathId;
    int32_t     LinkId;
    
} JumpLink_t;

// Type for jump link lists
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(JumpLink_t, JumpLinks) JumpLinks_t;

// Type for movement sequence lists
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(MoveVector_t, MoveSequence) MoveSequence_t;

// Type for jump direction definitions
// Layout@ggc_x86_64 => 88@[4,4,4,4,8,16,16,16,16]
typedef struct JumpDirection
{
    int32_t         ObjectId;
    int32_t         PositionId;
    int32_t         CollectionId;
    int32_t         JumpLength;
    double          FieldProjectionFactor;
    JumpSequence_t  JumpSequence;
    JumpLinks_t     JumpLinkSequence;
    MoveSequence_t  LocalMoveSequence;
    MoveSequence_t  GlobalMoveSequence;
    
} JumpDirection_t;

// Type for jump direction lists
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(JumpDirection_t, JumpDirections) JumpDirections_t;

// Type for a transition jump rule
// Layout@ggc_x86_64 => 48@[8,8,8,8,8,8]
typedef struct JumpRule
{
    OccCode_t   StateCode0;
    OccCode_t   StateCode1;
    OccCode_t   StateCode2;
    double      FrequencyFactor;
    double      FieldFactor;
    byte_t      TrackerOrderCode[8];
    
} JumpRule_t;

// Type for jump rule lists
// Layout@ggc_x86_64 => 16@[8,8}]
typedef Span_t(JumpRule_t, JumpRules) JumpRules_t;

// Type for jump collections
// Layout@ggc_x86_64 => 48@[8,16,16,4,{4}]
typedef struct JumpCollection
{
    Bitmask_t           ParticleMask;
    JumpDirections_t    JumpDirections;
    JumpRules_t         JumpRules;
    int32_t             ObjectId;

    int32_t             Padding:32;
} JumpCollection_t;

// Type for jump collection lists
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(JumpCollection_t, JumpCollections) JumpCollections_t;

// Type for the transition model
// Layout@ggc_x86_64 => 80@[16,16,24,24]
typedef struct TransitionModel
{
    JumpCollections_t   JumpCollections;
    JumpDirections_t    JumpDirections;
    JumpCountTable_t    JumpCountTable;
    JumpAssignTable_t   JumpAssignTable;

} TransitionModel_t;

/* Job model */

// Type for the mmc job header
// Layout@ggc_x86_64 => 32@[8,8,4,4,4,{4}]
typedef struct MmcHeader
{
    Bitmask_t   JobFlags;
    double      AbortTolerance;
    int32_t     AbortSequenceLength;
    int32_t     AbortSampleLength;
    int32_t     AbortSampleInterval;

    int32_t     Padding:32;
    
} MmcHeader_t;

// Type for the kmc job header
// Layout@ggc_x86_64 => 40@[8,8,8,8,4,{4}]
typedef struct KmcHeader
{
    Bitmask_t   JobFlags;
    double      FieldMagnitude;
    double      BaseFrequency;
    double      FixedNormFactor;
    int32_t     NumOfDynamicTrackers;

    int32_t     Padding:32;

} KmcHeader_t;

// Type for the job info
// Layout@ggc_x86_64 => 72@[8,8,8,8,8,8,8,8,4,{4}]
typedef struct JobInfo
{
    Bitmask_t   JobFlags;
    Bitmask_t   StatusFlags;
    int64_t     StateSize;
    int64_t     TargetMcsp;
    int64_t     TimeLimit;
    double      Temperature;
    double      MinimalSuccessRate;
    void *      JobHeader;
    int32_t     ObjectId;

    int32_t     Padding:32;
    
} JobInfo_t;

/* Lattice model */

// Type for the byte based 4d rectangular lattice access
// Layout@ggc_x86_64 => 24@[8,8,8]
typedef Array_t(byte_t, 4, Lattice) Lattice_t;

// Type for the double 5D rectangular energy background access
typedef Array_t(double, 5, EnergyBackground) EnergyBackground_t;

// Type for the lattice information (Supports 16 bit alignment)
// Layout@ggc_x86_64 => 80@[16,4,4,24,24,{8}]
typedef struct LatticeInfo
{
    Vector4_t           SizeVector;
    int32_t             NumOfMobiles;
    int32_t             NumOfSelectables;
    Lattice_t           Lattice;
    EnergyBackground_t  EnergyBackground;

    int64_t             Padding:64;

} LatticeInfo_t;

/* Database model */

// Type for the database model context
// Layout@ggc_x86_64 => 320@[80,72,56,32,80]
typedef struct DbModel
{
    LatticeInfo_t       LattInfo;
    JobInfo_t           JobInfo;
    StructureModel_t    Structure;
    EnergyModel_t       Energy;
    TransitionModel_t   Transition;

} DbModel_t;