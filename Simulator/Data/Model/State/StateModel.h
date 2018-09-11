//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	StateModel.h        		//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Simulation state model      //
//////////////////////////////////////////

#pragma once
#include "Framework/Math/Types/Vector.h"
#include "Framework/Basic/BaseTypes/BaseTypes.h"
#include "Framework/Basic/BaseTypes/Buffers.h"

// Type for 3d movement tracking without tracker id (Does currently not support 16 bit alignment!)
// Layout@ggc_x86_64 => 32@[24]
typedef vector3_t tracker_t;

// Type for the state header information
// Layout@ggc_x86_64 => 48@[8,8,4,4,4,4,4,4,4,4,4,{4}]
typedef struct
{
    int64_t Mcs;
    int64_t Cycles;
    int32_t Flags;
    int32_t MetaStartByte;
    int32_t LatticeStartByte;
    int32_t CountersStartByte;
    int32_t GlobalTrackerStartByte;
    int32_t MobileTrackerStartByte;
    int32_t StaticTrackerStartByte;
    int32_t MobileTrackerIdxStartByte;
    int32_t ProbabilityMapStartByte;

    int32_t Padding:32;

} hdr_info_t;

// Type for the state header data
// Layout@ggc_x86_64 => 8@[8]
typedef struct
{
    hdr_info_t* Data;

} hdr_state_t;

// Type for the linearized state lattice
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(byte_t) lat_state_t;

// Type for the linearized tracker state
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(tracker_t) trc_state_t;

// Type for the particle assigned cycle counter collections
// Layout@ggc_x86_64 => 48@[8,8,8,8,8,8]
typedef struct
{
    int64_t NumOfCyles;
    int64_t NumOfMcs;
    int64_t NumOfRejects;
    int64_t NumOfBlocks;
    int64_t NumOfUnstableStarts;
    int64_t NumOfUnstableEnds;
    
} cnt_col_t;

// Type for the list of cnt collections in the state
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(cnt_col_t) cnt_state_t;

// Type for the state meta information
// Layout@ggc_x86_64 => 56@[8,8,8,8,8,8,8]
typedef struct
{
    double  SimulatedTime;
    double  JumpNormalization;
    double  MaxJumpProbability;
    int64_t ProgramRunTime;
    int64_t CycleRate;
    int64_t SuccessRate;
    int64_t TimePerBlock;
    
} meta_info_t;

// Type for the state meta information data access
// Layout@ggc_x86_64 => 8@[8]
typedef struct
{
    meta_info_t* Data;
    
} mta_state_t;

// Type for the storage of the dynmaic tracker indexing
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(int32_t) idx_state_t;

// Type for the 2d rectangular probability count map
// Layout@ggc_x86_64 => 24@[8,8,4,4,4,4,4,4,4,4,4,{4}]
typedef Array_t(int64_t, 2) prb_state_t;

// Type for the simulation state
// Layout@ggc_x86_64 => 148@[16,8,8,16,16,16,16,16,16,24]
typedef struct
{
    buffer_t    Buffer;
    hdr_state_t Header;
    mta_state_t Meta;
    lat_state_t Lattice;
    cnt_state_t Counters;
    trc_state_t GlobalTrackers;
    trc_state_t MobileTrackers;
    trc_state_t StaticTrackers;
    idx_state_t MobileTrackerIndexing;
    prb_state_t ProbabilityTrackMap;

} mc_state_t;