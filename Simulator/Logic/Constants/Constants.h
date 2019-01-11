//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	Constants.h   		        //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Global constants/defines    //
//////////////////////////////////////////

#pragma once

/* Optimization toggle defines */

#define OPT_LINK_ONLY_MOBILES

/* Flag values */

#define FLG_KMC             0x1
#define FLG_MMC             0x2
#define FLG_PRERUN          0x4
#define FLG_CONTINUE        0x8
#define FLG_COMPLETED       0x10
#define FLG_TIMEOUT         0x20
#define FLG_ABORTCONDITION  0x40
#define FLG_RATELIMIT       0x80
#define FLG_FIRSTCYCLE      0x100
#define FLG_INITIALIZED     0x20000000
#define FLG_ABORT           0x40000000
#define FLG_STATEERROR      0x80000000

/* Monte Carlo constants */

#define MC_CONST_JUMPTRACK_MIN 1.0e-05
#define MC_CONST_JUMPTRACK_MAX 1.0e+00
#define MC_CONST_JUMPLIMIT_MIN 0.0e+00
#define MC_CONST_JUMPLIMIT_MAX 1.0e+00

/* Physical constants */

#define NATCONST_BLOTZMANN  8.6173303e-05
#define NATCONST_ELMCHARGE  1.6021766208e-19

/* Tolerance constant */

#define CONST_JUMPTRACK_MIN 1.0e-05
#define CONST_JUMPTRACK_MAX 1.0e+00
#define CONST_JUMPLIMIT_MIN 0.0e+00
#define CONST_JUMPLIMIT_MAX 1.0e+00

/* Run/Cycle constants */

#define CYCLE_BLOCKCOUNT    100LL
#define CYCLE_BLOCKSIZE_MIN 1000
#define CYCLE_BLOCKSIZE_MAX 10000000

/* Jump constants */

#define JUMPS_JUMPLENGTH_MIN 2
#define JUMPS_JUMPLENGTH_MAX 8
#define JUMPS_JUMPLINK_LIMIT (JUMPS_JUMPLENGTH_MAX * (JUMPS_JUMPLENGTH_MAX - 1))

/* Jump pool constants */

#define JPOOL_DIRCOUNT_STATIC   -1
#define JPOOL_DIRCOUNT_PASSIVE   0
#define JPOOL_NOT_SELECTABLE    -1

/* Particle/position/index constants */

#define PARTICLE_VOID       0
#define PARTICLE_NULL       255
#define POSITION_NULL       -1
#define INVALID_INDEX       -1

/* File and writemode definitions */

#define FILE_MAINSTATE   "./run.mcs"
#define FILE_PRERSTATE   "./prerun.mcs"
#define FILE_STDOUTLOG   "./stdout.log"
#define FILE_STDERRLOG   "./stderr.log"
#define FMODE_BINARY_R   "rb"
#define FMODE_BINARY_W   "wb"
#define FMODE_NORMAL_R   "r"
#define FMODE_NORMAL_W   "w"
#define FMODE_NORMAL_A   "a"

/* Tags/values for log files */

#define TAG_SIMULATING   "[-  RUNNING  -]"
#define TAG_COMPLETION   "[- COMPLETED -]"
#define TAG_ERRORABORT   "[-   ERROR   -]"
