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

typedef vector3_t tracker_t;

typedef struct
{
    int64_t Mcs;
    int32_t Flags;
    int32_t MetaByte, LatticeByte, CountersByte, GlobalTrcByte, MobileTrcByte, StaticTrcByte, MobileTrcIdxByte, ProbStatMapByte;
} hdr_info_t;

typedef struct { hdr_info_t* Data; } hdr_state_t;

typedef struct { int32_t Count; byte_t* Start, * End; } lat_state_t;

typedef struct { int32_t Count; tracker_t* Start, * End; } trc_state_t;

typedef struct { int64_t CycleCnt, StepCnt, RejectCnt, BlockCnt, OnUnstCnt, ToUnstCnt; } cnt_col_t;

typedef struct { int32_t Count; cnt_col_t * Start, * End; } cnt_state_t;

typedef struct { double SimTime, JmpNorm; int64_t RunTime, CyleRate, SuccessRate, TimePerBlock; } meta_info_t;

typedef struct { meta_info_t* Data; } mta_state_t;

typedef struct { int32_t Count; int32_t * Start, * End; } idx_state_t;

typedef struct { int32_t Count; int64_t * Start, * End; } prb_state_t;

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
    idx_state_t MobileTrackerIdx;
    prb_state_t ProbStatMap;
} mc_state_t;