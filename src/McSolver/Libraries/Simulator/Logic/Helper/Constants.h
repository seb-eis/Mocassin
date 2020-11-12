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
#include "float.h"

/* Optimization toggle defines */

// Optimizes the linking process to ignore immobile positions (Major perf. impact, lattice de-synchronizes)
#define OPT_LINK_ONLY_MOBILES

// Optimizes the pair table system to use 1x 3D lookup instead of 2x 2D lookups per delta value (Minor perf. impact)
#define OPT_USE_3D_PAIRTABLES

// Optimizes the accept/reject system by using pre-rejection checks for frequency factors (Major perf. impact for multi-frequency simulations)
#define OPT_PRECHECK_FREQUENCY

// Set the upper threshold frequency factor [0;1]. The check will be skipped for values above
#define OPT_FRQPRECHECK_LIMIT (1.0 - DBL_EPSILON)

/* State buffer constants and default values */

#define STATE_JUMPSTAT_SIZE 1000
#define STATE_JUMPSTAT_EMIN 0
#define STATE_JUMPSTAT_EMAX 5.0

/* Job info flag values */

#define INFO_FLG_KMC                1ULL        // Flag that marks a job as KMC
#define INFO_FLG_MMC                (1ULL << 1U)   // Flag that marks a job as MMC
#define INFO_FLG_USEPRERUN          (1ULL << 2U)   // Flag that marks a job to contain pre-run
#define INFO_FLG_SKIPSAVE           (1ULL << 3U)   // Flag that marks a job as unsaved with no state saving
#define INFO_FLG_DUALDOF            (1ULL << 4U)   // Flag that marks a job as non-optimized with twice the actually existing degrees of freedom
#define INFO_FLG_NOJUMPLOGGING      (1ULL << 5U)   // Flag that marks a job as non histogram creating where the histograms will not be populated during simulation
#define INFO_FLG_USEFASTEXP         (1ULL << 6U)   // Flag that marks a job for fast exponential approximation usage

/* Main state flag values */

#define STATE_FLG_PRERUN        1ULL
#define STATE_FLG_CONTINUE      (1ULL << 1U)
#define STATE_FLG_COMPLETED     (1ULL << 2U)
#define STATE_FLG_TIMEOUT       (1ULL << 3U)
#define STATE_FLG_SIMABORT      (1ULL << 4U)
#define STATE_FLG_CONDABORT     (1ULL << 5U)
#define STATE_FLG_RATEABORT     (1ULL << 6U)
#define STATE_FLG_FIRSTCYCLE    (1ULL << 7U)
#define STATE_FLG_INITIALIZED   (1ULL << 8U)
#define STATE_FLG_SIMERROR      (1ULL << 9U)
#define STATE_FLG_PRERUN_RESET  (1ULL << 10U)
#define STATE_FLG_ENERGYABORT   (1ULL << 11U)

/* Monte Carlo constants */

#define MC_CONST_FLP_TOLERANCE 1.0e-10
#define MC_CONST_JUMPLIMIT_MIN 0.0e+00 - MC_CONST_FLP_TOLERANCE
#define MC_CONST_JUMPLIMIT_MAX 1.0e+00 + MC_CONST_FLP_TOLERANCE
#define MC_CONST_BACKJUMP_NULL 0.0
#define MC_CONST_BACKJUMP_INF  INFINITY

/* Physical constants */

#define NATCONST_BLOTZMANN  8.617333262145e-05
#define NATCONST_ELMCHARGE  1.602176634e-19

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
#define JUMPS_JUMPCORRECTION_NOTSTATIC NAN

/* Jump pool constants */

#define JPOOL_DIRCOUNT_STATIC   -1
#define JPOOL_DIRCOUNT_PASSIVE   0
#define JPOOL_NOT_SELECTABLE    -1

/* Cluster and energy defines */
#define CLUSTER_MAXLINK_COUNT           256
#define CLUSTER_MAXSIZE_LINEAR_SEARCH   8
#define ENERGY_FLG_CONST_TABLE          1U

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
