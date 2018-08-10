//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	SimStates.h        		    //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Simulation States           //
//////////////////////////////////////////

#pragma once
#include "Framework/Math/Types/Vector.h"
#include "Framework/Basic/BaseTypes/BaseTypes.h"
#include "Framework/Basic/FileIO/FileIO.h"

struct sim_context_t;

typedef vector3_t tracker_t;

typedef struct { int32_t Count; byte_t* Start, * End; } lat_state_t;

typedef struct { int32_t Count; tracker_t* Start, * End; } trc_state_t;

typedef struct { int64_t CycleCnt, StepCnt, RejectCnt, BlockCnt, OnUnstCnt, ToUnstCnt; } cnt_col_t;

typedef struct { int32_t Count; cnt_col_t * Start, * End; } cnt_state_t;

typedef struct { double SimTime; int64_t RunTime; } tim_state_t;

typedef struct { int32_t Count; int32_t * Start, * End; } trc_idx_t;

typedef struct
{
    buffer_t    Buffer;
    tim_state_t Timers;
    lat_state_t Lattice;
    cnt_state_t Counters;
    trc_state_t GlobalTrackers;
    trc_state_t MobileTrackers;
    trc_state_t StaticTrackers;
    trc_idx_t   MobileTrackerIdx;
} mc_state_t;

//error_t ConstructSimulationState(sim_context_t* restrict simContext);

//error_t PrepareSimulationState(sim_context_t* restrict simContext);

//error_t WriteSimulationStateToFile(const sim_context_t* restrict simContext);

//error_t LoadSimulationStateFromFile(sim_context_t* restrict simContext);

//error_t GetSimulationStateEval(const sim_context_t* restrict simContext);