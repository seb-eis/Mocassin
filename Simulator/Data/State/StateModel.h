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
#include "Simulator/Logic/Constants/Constants.h"

// Type for 3d movement tracking without tracker id (Does currently not support 16 bit alignment!)
// Layout@ggc_x86_64 => 32@[24]
typedef Vector3_t Tracker_t;

// Type for the state header information
// Layout@ggc_x86_64 => 48@[8,8,4,4,4,4,4,4,4,4,4,{4}]
typedef struct StateHeaderData
{
    // The number of successful steps
    int64_t Mcs;

    // The number of simulation cycles
    int64_t Cycles;

    // The simulation runtime flags
    int32_t Flags;

    // The start byte number of the state meta data
    int32_t MetaStartByte;

    // The start byte number of the state lattice data
    int32_t LatticeStartByte;

    // The start byte number of the state counter data
    int32_t CountersStartByte;

    // The start byte number of the state global tracker data
    int32_t GlobalTrackerStartByte;

    // The start byte number of the state mobile tracker data
    int32_t MobileTrackerStartByte;

    // The start byte number of the state static tracker data
    int32_t StaticTrackerStartByte;

    // The start byte number of the state mobile tracker indexing data
    int32_t MobileTrackerIdxStartByte;

    // The start byte number of the state jump statistics data
    int32_t JumpStatisticsStartByte;

    // Explicit Padding
    int32_t Padding:32;

} StateHeaderData_t;

// Type for the state header data
// Layout@ggc_x86_64 => 8@[8]
typedef struct StateHeader
{
    // The state header data pointer
    StateHeaderData_t* Data;

} StateHeader_t;

// Type for the linearized state lattice
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(byte_t, LatticeState) LatticeState_t;

// Type for the linearized tracker state
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(Tracker_t, TrackerState) TrackersState_t;

// Type for the particle assigned cycle counter collections
// Layout@ggc_x86_64 => 48@[8,8,8,8,8,8]
typedef struct StateCounterCollection
{
    // Counter for simulation cycles
    int64_t CycleCount;

    // Counter for successful cycles
    int64_t McsCount;

    // Counter for rejected cycles
    int64_t RejectionCount;

    // Counter for site blocking cycles
    int64_t SiteBlockingCount;

    // Counter for unstable start cycles
    int64_t UnstableStartCount;

    // Counter for unstable end cycles
    int64_t UnstableEndCount;
    
} StateCounterCollection_t;

// Type for the list of cnt collections in the state
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(StateCounterCollection_t, CountersState) CountersState_t;

// Type for the state meta information
// Layout@ggc_x86_64 => 72@[8,8,8,8,8,8,8,8,8,8]
typedef struct StateMetaData
{
    // The simulated time span of the system [seconds]
    double      SimulatedTime;

    // The current jump normalization value
    double      JumpNormalization;

    // The highest jump probability that has occurred
    double      MaxJumpProbability;

    // The last calculated lattice energy value in [eV]
    double      LatticeEnergy;

    // The runtime of the program in [seconds]
    int64_t     ProgramRunTime;

    // The cycle rate of the simulation in [Hz]
    int64_t     CycleRate;

    // The success rate of the simulation in [Hz]
    int64_t     SuccessRate;

    // The number of seconds for a block completion
    int64_t     TimePerBlock;

    // The random number generator state value
    uint64_t    RngState;

    // The random number generator increase value
    uint64_t    RngIncrease;
    
} StateMetaData_t;

// Type for the state meta information data access
// Layout@ggc_x86_64 => 8@[8]
typedef struct StateMetaInfo
{
    // Pointer to the state meta data
    StateMetaData_t* Data;
    
} StateMetaInfo_t;

// Type for the jump histogram type that stores a energy value occurrence statistic
// Layout@ggc_x86_64 => 32+1000*8@[8,8,8,8,1000*8]
typedef struct JumpHistogram
{
    // The minimal energy value of the histogram
    double MinValue;

    // The maximum energy value of the histogram
    double MaxValue;

    // The energy stepping value of the the count buffer
    double Stepping;

    // The counter for occurred cases above the max energy value
    int64_t OverflowCount;

    // The counter for occurred cases below the min energy value
    int64_t UnderflowCount;

    // The histogram buffer to count the number of specific occurrences
    int64_t CountBuffer[STATE_JUMPSTAT_SIZE];

} JumpHistogram_t;

// Type to track the jump statistics of a particle index and jump collection combination
// Layout@ggc_x86_64 => 4x@{?,?,?,?}
typedef struct JumpStatistic
{
    // Histogram for edge energy occurrences
    JumpHistogram_t EdgeEnergyHistogram;

    // Histogram for positive conformation energy occurrences
    JumpHistogram_t PosConfEnergyHistogram;

    // Histogram for negative conformation energy occurrences
    JumpHistogram_t NegConfEnergyHistogram;

    // Histogram for total energy occurrences
    JumpHistogram_t TotalEnergyHistogram;

} JumpStatistic_t;

// Type for the storage of multiple jump statistics of the global trackers
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(JumpStatistic_t, JumpStatisticsState) JumpStatisticsState_t;

// Type for the storage of the dynamic tracker mapping
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(int32_t, IndexingState) MobileTrackerMapping_t;

// Type for the simulation state
// Layout@ggc_x86_64 => 148@[16,8,8,16,16,16,16,16,16,24]
typedef struct SimulationState
{
    // Access span to the full simulation state buffer
    Buffer_t                Buffer;

    // The simulation state header access
    StateHeader_t           Header;

    // The simulation state meta data access
    StateMetaInfo_t         Meta;

    // The simulation state lattice data access
    LatticeState_t          Lattice;

    // The simulation state counter data access
    CountersState_t         Counters;

    // The simulation state global tracker data access
    TrackersState_t         GlobalTrackers;

    // The simulation state mobile tracker data access
    TrackersState_t         MobileTrackers;

    // The simulation state static tracker data access
    TrackersState_t         StaticTrackers;

    // The simulation state mobile tracker mapping data access
    MobileTrackerMapping_t  MobileTrackerMapping;

    // The simulation state jump statistics data access
    JumpStatisticsState_t   JumpStatistics;

} SimulationState_t;