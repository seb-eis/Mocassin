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

// Optimizes the linking process to ignore immobile positions (Major impact)
#define OPT_LINK_ONLY_MOBILES

// Optimizes the pair table system to use 1x 3D lookup instead of 2x 2D lookups per delta value (Minor impact)
#define OPT_USE_3D_PAIRTABLES

/* State buffer constants and default values */

#define STATE_JUMPSTAT_SIZE 1000
#define STATE_JUMPSTAT_EMIN 0
#define STATE_JUMPSTAT_EMAX 10.0

/* Job info flag values */

#define INFO_FLG_KMC        1       // Flag that marks a job as MMC
#define INFO_FLG_MMC        1 << 1  // Flag that marks a job as KMC
#define INFO_FLG_USEPRERUN  1 << 2  // Flag that marks a job to contain pre-run
#define INFO_FLG_SKIPSAVE   1 << 3  // Flag that marks a job as unsaved with no state saving
#define INFO_FLG_DUALDOF    1 << 4  // Flag that marks a job as non-optimized with twice the actually existing degrees of freedom

/* Main state flag values */

#define STATE_FLG_PRERUN        1
#define STATE_FLG_CONTINUE      1 << 1
#define STATE_FLG_COMPLETED     1 << 2
#define STATE_FLG_TIMEOUT       1 << 3
#define STATE_FLG_SIMABORT      1 << 4
#define STATE_FLG_CONDABORT     1 << 5
#define STATE_FLG_RATEABORT     1 << 6
#define STATE_FLG_FIRSTCYCLE    1 << 7
#define STATE_FLG_INITIALIZED   1 << 8
#define STATE_FLG_SIMERROR      1 << 9
#define STATE_FLG_PRERUN_RESET  1 << 10
#define STATE_FLG_ENERGYABORT   1 << 11

/* Monte Carlo constants */

#define MC_CONST_JUMPLIMIT_MIN 0.0e+00
#define MC_CONST_JUMPLIMIT_MAX 1.0e+00

/* Physical constants */

#define NATCONST_BLOTZMANN  8.6173303e-05
#define NATCONST_ELMCHARGE  1.6021766208e-19

/* Conversion factors */
#define CONV_LENGTH_ANG_TO_M 1.0e-10
#define CONV_LENGTH_CM_TO_M 1.0e-2
#define CONV_VOLUME_ANG_TO_M 1.0e-30
#define CONV_VOLUME_CM_TO_M 1.0e-6

/* Run/Cycle constants */

#define CYCLE_BLOCKCOUNT    100LL
#define CYCLE_BLOCKSIZE_MIN 100000
#define CYCLE_BLOCKSIZE_MAX 10000000
#define CYCLE_BLOCKSIZE_MUL 100

/* Jump constants */

#define JUMPS_JUMPLENGTH_MIN 2
#define JUMPS_JUMPLENGTH_MAX 8
#define JUMPS_JUMPLINK_LIMIT (JUMPS_JUMPLENGTH_MAX * (JUMPS_JUMPLENGTH_MAX - 1))

/* Jump pool constants */

#define JPOOL_DIRCOUNT_STATIC   -1
#define JPOOL_DIRCOUNT_PASSIVE   0
#define JPOOL_NOT_SELECTABLE    -1

/* Cluster defines */
#define CLUSTER_MAXLINK_COUNT   256

/* Particle/position/index constants */

#define PARTICLE_VOID       0
#define PARTICLE_NULL       255
#define PARTICLE_IDLIMIT    64
#define POSITION_NULL       -1
#define INVALID_INDEX       -1
#define INVALID_COUNT       -1

/* File and writemode definitions */

#define FILE_MAINSTATE   "run.mcs"
#define FILE_PRERSTATE   "prerun.mcs"
#define FILE_STDOUTLOG   "stdout.log"
#define FILE_STDERRLOG   "stderr.log"
#define FMODE_BINARY_R   "rb"
#define FMODE_BINARY_W   "wb"
#define FMODE_NORMAL_R   "r"
#define FMODE_NORMAL_W   "w"
#define FMODE_NORMAL_A   "a"

/* Tags/values for log files */

#define TAG_SIMULATING   "[-  RUNNING  -]"
#define TAG_COMPLETION   "[- COMPLETED -]"
#define TAG_ERRORABORT   "[-   ERROR   -]"
