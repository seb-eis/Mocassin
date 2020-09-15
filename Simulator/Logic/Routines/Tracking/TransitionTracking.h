//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	TransitionTracking.h        //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   MC transition tracking      //
//////////////////////////////////////////

#pragma once

#include <stdint.h>
#include "Simulator/Data/SimContext/ContextAccess.h"

// Type for int64_t spans
// Layout@ggc_x86_64 => 16@[8,8]
typedef Span_t(int64_t, Int64Span) Int64Span_t;

// Type for the dynamic jump histogram header that stores the access meta data of a dynamic jump histogram
// Layout@ggc_x86_64 => 56@[8,8,8,8,8,8,8]
typedef struct DynamicJumpHistogramHeader
{
    // The minimal energy value of the histogram
    double              MinValue;

    // The maximum energy value of the histogram
    double              MaxValue;

    // The energy stepping value of the the count buffer
    double              Stepping;

    // The inverse energy stepping value of the the count buffer
    double              SteppingInverse;

    // The counter for occurred cases above the max energy value
    int64_t             OverflowCount;

    // The counter for occurred cases below the min energy value
    int64_t             UnderflowCount;

    // The number of entries in the attached count buffer
    int64_t             EntryCount;

} DynamicJumpHistogramHeader_t;

// Type for the dynamic jump histogram (Variable buffer size energy histogram)
// Layout@ggc_x86_64 => 24@[8,16]
typedef struct DynamicJumpHistogram
{
    // The pointer to the histogram header
    DynamicJumpHistogramHeader_t*   Header;

    // The span access struct for the counter values
    Int64Span_t                     Counters;

} DynamicJumpHistogram_t;

// Resets a dynamic jump histogram by setting all values to zero except for the size information
static inline void ResetDynamicJumpHistogramToEmptyState(DynamicJumpHistogram_t*restrict jumpHistogram)
{
    memset(jumpHistogram->Header, 0, (void*) jumpHistogram->Counters.End - (void*) jumpHistogram->Header);
    jumpHistogram->Header->EntryCount = span_Length(jumpHistogram->Counters);
}

// Creates a new dynamic jump histogram with the requested counter number
static inline DynamicJumpHistogram_t ctor_DynamicJumpHistogram(const int32_t numOfCounters)
{
    let bufferSize = numOfCounters * sizeof(int64_t);
    let headerSize = sizeof(DynamicJumpHistogramHeader_t);
    var ptr = malloc(bufferSize + headerSize);
    var result = (DynamicJumpHistogram_t){.Header = ptr,.Counters = (Int64Span_t){.Begin = ptr + headerSize, .End = ptr + bufferSize + headerSize}};
    ResetDynamicJumpHistogramToEmptyState(&result);
    return result;
}

// Creates a new dynamic jump histogram by reinterpreting the passed buffer point (Buffer is copied and not freed)
static inline DynamicJumpHistogram_t ctor_DynamicJumpHistogram_FromBuffer(const void* buffer)
{
    if (buffer == NULL) return (DynamicJumpHistogram_t) {0,{0,0}};
    let entryCount = ((DynamicJumpHistogramHeader_t*) buffer)->EntryCount;
    var histogram = ctor_DynamicJumpHistogram(entryCount);
    memcpy(&histogram.Header, buffer, sizeof(DynamicJumpHistogramHeader_t) + span_ByteCount(histogram.Counters));
    return histogram;
}

// Updates the tracking system on the simulation state after a successful KMC transition with the current data
void AdvanceKmcTransitionTrackingSystem(SCONTEXT_PARAMETER);

// Updates the jump histogram data after a KMC jump attempt without advancing to the next state
void AddCurrentKmcTransitionDataToHistograms(SCONTEXT_PARAMETER);

// Initializes the jump statistics system on the passed simulation context (Has en effect only in KMC runs)
error_t InitJumpStatisticsTrackingSystem(SCONTEXT_PARAMETER);

// Synchronizes the mobile tracker mapping of the main simulation state to the current values in the simulation lattice
error_t SyncMainStateTrackerMappingToSimulation(SCONTEXT_PARAMETER);

//  Adds an energy value to the passed fixed size jump histogram
void AddEnergyValueToJumpHistogram(JumpHistogram_t*restrict jumpHistogram, double value);

// Adds an energy value to the passed variable size dynamic jump histogram
void AddEnergyValueToDynamicJumpHistogram(DynamicJumpHistogram_t*restrict jumpHistogram, double value);

// Changes the sampling area settings of a dynamic jump histogram by definition of the min and max value (Implicit reset to empty state)
error_t ChangeDynamicJumpHistogramSamplingAreaByMinMax(DynamicJumpHistogram_t*restrict jumpHistogram, double minValue, double maxValue);

// Changes the sampling area settings of a dynamic jump histogram by definition of the center value and valueRange (Implicit reset to empty state)
error_t ChangeDynamicJumpHistogramSamplingAreaByRange(DynamicJumpHistogram_t*restrict jumpHistogram, double centerValue, double valueRange);

// Calculates the mean energy value of the passed dynamic jump histogram
double CalculateDynamicJumpHistogramMeanEnergy(const DynamicJumpHistogram_t*restrict jumpHistogram);

// Finds the energy value with the highest number of counts in the provided histogram
double FindDynamicJumpHistogramMaxValue(const DynamicJumpHistogram_t*restrict jumpHistogram);