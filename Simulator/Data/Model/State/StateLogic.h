//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	StateModel.h        		//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Simulation state logic      //
//////////////////////////////////////////

#pragma once
#include "Framework/Basic/FileIO/FileIO.h"
#include "Simulator/Data/Model/State/StateModel.h"
#include "Simulator/Data/Model/SimContext/SimContext.h"

#define MC_RUN_STATE_FILE "./run.state"
#define MC_PRE_STATE_FILE "./pre.state"
#define MC_STD_LOCAL_FILE "./"
#define MC_RUN_FINISH_TAG "[STATUS][COMPLETED]"
#define MC_RUN_RUNERR_TAG "[STATUS][____ERROR]"
#define MC_RUN_ACTIVE_TAG "[STATUS][__RUNNING]"
#define MC_BIN_WRITE_MODE "wb"

error_t ConstructSimulationState(__SCONTEXT_PAR);

error_t PrepareSimulationState(__SCONTEXT_PAR);

error_t WriteSimulationStateToFile(const __SCONTEXT_PAR);

error_t LoadSimulationStateFromFile(__SCONTEXT_PAR);

error_t GetSimulationStateEval(const __SCONTEXT_PAR);

error_t DumpStateStatsToStream(__SCONTEXT_PAR, file_t* restrict stream);

error_t DumpStateStatsToFile(__SCONTEXT_PAR, const char* restrict filePath);