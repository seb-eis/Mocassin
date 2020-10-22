//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	PcgRandom.h     		    //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   PCG32 RNG Implementation    //
//////////////////////////////////////////

#pragma once

#include "Libraries/Framework/Errors/McErrors.h"
#include "Libraries/Framework/Basic/TimeHelper.h"
#include <time.h>
#include <stdint.h>
#include <limits.h>

// *Really* minimal PCG32 code / (c) 2014 M.E. O'Neill / pcg-random.org
// Licensed under Apache License 2.0 (NO WARRANTY, etc. see website)

// Typedef of the pcg32 random number generator
typedef struct Pcg32 { uint64_t State;  uint64_t Inc; } Pcg32_t;

// Get next random unsigned integer from the passed pcg32 rng
static inline uint32_t Pcg32NextRandom(Pcg32_t* restrict rng)
{
	let oldstate = rng->State;
	rng->State = oldstate * 6364136223846793005ULL + rng->Inc;
	let xorshifted = (uint32_t) (((oldstate >> 18u) ^ oldstate) >> 27u);
	let rot = (uint32_t) (oldstate >> 59u);
	return (xorshifted >> rot) | (xorshifted << ((-rot) & 31u));
}

//  Get the next ceiled random number from [0...ceil) where the modulo bias is corrected
static inline uint32_t Pcg32NextCeiledRandom(Pcg32_t* restrict rng, uint32_t ceil)
{
    let threshold = -ceil % ceil;
    for(;;)
    {
        let rnv = Pcg32NextRandom(rng);
        if(rnv >= threshold) return rnv % ceil;
    }
}

// Get next random double from range [0.0,1.0) using the passed pcg32 rng
static inline double Pcg32NextRandomDoubleQuick(Pcg32_t* restrict rng)
{
	return ((double) Pcg32NextRandom(rng) / (double) (UINT32_MAX - 1));
}

// Get next random double from range [0.0,1.0) using the passed pcg32 rng
static inline double Pcg32NextRandomDouble(Pcg32_t* restrict rng)
{
    union { struct { uint32_t u0, u1; } u32; uint64_t u64; double f64; } random;
    random.u32.u0 = Pcg32NextRandom(rng);
    random.u32.u1 = Pcg32NextRandom(rng);
    random.u64 = 0x3FF0000000000000u | (random.u64 >> 12u);
    return random.f64 - 1.0;
}

// Seed the rng with state and increase value
static inline Pcg32_t* Pcg32SeedGenerator(Pcg32_t* restrict rng, uint64_t state, uint64_t inc)
{
	rng->State = 0U;
	rng->Inc = (inc << 1u) | 1u;
    Pcg32NextRandom(rng);
	rng->State += state;
    Pcg32NextRandom(rng);
    return rng;
}

// Get a hash value for the passed string
static inline uint64_t GetStringHash(const char* str)
{
	uint64_t hash = 5381ULL;
	int32_t c;

	while ((c = *str++))
		hash = ((hash << 5) + hash) + c;

	return hash;
}

// Build a new PCG32 that is initialized using the current time
static inline Pcg32_t ConstructTimeInitializedPcg32()
{
	Pcg32_t pcg;
	char timeStr[100];
	error_t error = GetFormatedTimeStamp("%Y-%m-%d-%H-%M-%S-STATE", timeStr, sizeof(timeStr));
	assert_success(error, "Failed to get system time string fro state");
	uint64_t state = GetStringHash(timeStr);

	error = GetFormatedTimeStamp("%Y-%m-%d-%H-%M-%S-INC", timeStr, sizeof(timeStr));
	assert_success(error, "Failed to get system time string for inc");
	uint64_t inc = GetStringHash(timeStr) | 1ULL;

	return *Pcg32SeedGenerator(&pcg, state, inc);
}