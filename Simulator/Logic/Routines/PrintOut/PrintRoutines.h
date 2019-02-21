//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	DebugRoutines.h        		//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Debug routines              //
//////////////////////////////////////////

#pragma once
#include "Framework/Errors/McErrors.h"
#include "Simulator/Data/SimContext/ContextAccess.h"

#define MC_OUTTAG_FORMAT "%-35s"
#define MC_OUTPRC_FORMAT "%+.3e%%"
#define MC_OUTF64_FORMAT "%+.12e"
#define MC_OUTSTR_FORMAT "%-20s"
#define MC_OUTCMD_FORMAT "%s"
#define MC_OUTCOM_FORMAT FORMAT_I64(-9)"/"FORMAT_I64(10)
#define MC_OUTI32_FORMAT FORMAT_I32(-20)
#define MC_OUTI64_FORMAT FORMAT_I64(-20)
#define MC_OUTU64_FORMAT FORMAT_U64(-20)
#define MC_OUTVEC_FORMAT MC_OUTF64_FORMAT" "MC_OUTF64_FORMAT" "MC_OUTF64_FORMAT
#define MC_UNIT_FORMAT "[%-13s]"
#define MC_DEFAULT_FORMAT(VALUEFORMAT,...) MC_OUTTAG_FORMAT ": " MC_UNIT_FORMAT " " VALUEFORMAT " " __VA_ARGS__ "\n"
#define MC_STATHEADER_FORMAT "\n== Particle statistics for (Id=%i, Charge=%+.2e [e], Count=%i) ==\n"

// Prints the run statistics to a stream
void PrintFullSimulationStatistics(SCONTEXT_PARAM, file_t *fstream);

// Prints the start information of the simulation
void PrintJobStartInfo(SCONTEXT_PARAM, file_t *fstream);

// Prints the pre run context rest notification
void PrintContextResetNotice(SCONTEXT_PARAM, file_t *fstream);

// Prints the simulation finsihe notice
void PrintFinishNotice(SCONTEXT_PARAM, file_t* fstream);
