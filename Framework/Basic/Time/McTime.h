//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	McTime.h        		    //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Functions for UTC time      //
//////////////////////////////////////////

#pragma once

#include <string.h>
#include <time.h>
#include "Framework/Basic/FileIO/FileIO.h"
#include "Framework/Basic/BaseTypes/BaseTypes.h"
#include "Framework/Errors/McErrors.h"

// Defines the format for ISO8601 UTC time (YYYY-MM-DDTHH:MM:SS+HH:MM) format string
#define TIME_ISO8601_FORMATSTR "%Y-%m-%dT%H:%M:%S+00:00"

// Defines the number of byte/char required to print the ISO8601 UTC time string
#define TIME_ISO8601_BYTECOUNT sizeof("YYYY-MM-DDTHH:MM:SS+HH:MM")

// Get the current time info as local time or GMT if specified
static inline error_t GetCurrentTimeInfo(struct tm *restrict timeInfo, bool_t asGMT)
{
    time_t raw = time(NULL);
    return  (asGMT) ? gmtime_s(timeInfo, &raw) : localtime_s(timeInfo, &raw);
}

// Get the current time as a string with the provided format and writes it to the passed buffer (Default if format is NULL). Return error code on failure!
static inline error_t GetFormatedTimeStamp(const char *format, char *buffer, size_t maxBytes)
{
    struct tm tmInfo;
    error_t error = GetCurrentTimeInfo(&tmInfo, false);
    return_if(error, error);
    error = strftime(buffer, maxBytes, format ? format : "%c", &tmInfo) != 0 ? ERR_OK : ERR_UNKNOWN;
    return error;
}

// Converts the passed time epoch into an ISO8601 UTC time stamp using the passed buffer
static inline error_t TimeToTimeStampISO8601UTC(char *restrict buffer, const time_t *restrict time)
{
    struct tm timeInfo;
    error_t error = gmtime_s(&timeInfo, time);
    return_if(error, error);
    error = strftime(buffer, TIME_ISO8601_BYTECOUNT, TIME_ISO8601_FORMATSTR, &timeInfo) != 0 ? ERR_OK : ERR_UNKNOWN;
    return error;
}

// Get the current time as UTC with an ISO8601 format or an error if the creation fails
static inline error_t GetCurrentTimeStampISO8601UTC(char *restrict buffer)
{
    time_t raw = time(NULL);
    return TimeToTimeStampISO8601UTC(buffer, &raw);
}

// Converts seconds to an ISO8601 time period "PxxDTxxHxxMxxS"
static inline error_t SecondsToISO8601TimeSpan(char *restrict buffer, const int64_t totalSeconds)
{
    const char format[] = "P" FORMAT_I64(02) "DT" FORMAT_I64(02) "H" FORMAT_I64(02) "M" FORMAT_I64(02) "S";
    let days = totalSeconds / (3600 * 24);
    let hours = (totalSeconds / (3600 * 24)) % 3600;
    let minutes = (totalSeconds % 3600) / 60;
    let seconds = totalSeconds % 60;
    return sprintf(buffer, format, days, hours, minutes, seconds) > 0 ? ERR_OK : ERR_STREAM;
}