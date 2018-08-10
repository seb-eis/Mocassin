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
#include <stdint.h>
#include <limits.h>

// *Really* minimal PCG32 code / (c) 2014 M.E. O'Neill / pcg-random.org
// Licensed under Apache License 2.0 (NO WARRANTY, etc. see website)

#define PCG32_INITIALIZER   { 0x853c49e6748fea9bULL, 0xda3e39cb94b95bdbULL }

// TYpedef of the pcg32 random number generator
typedef struct { uint64_t state;  uint64_t inc; } pcg32_random_t;

// Global pcg32 random number generator state
static pcg32_random_t pcg32_global = PCG32_INITIALIZER;

// Get next random unsigned integer from the passed pcg32 rng
static inline uint32_t Pcg32Next(pcg32_random_t* restrict rng)
{
	uint64_t oldstate = rng->state;
	rng->state = oldstate * 6364136223846793005ULL + (rng->inc | 1);
	uint32_t xorshifted = ((oldstate >> 18u) ^ oldstate) >> 27u;
	uint32_t rot = oldstate >> 59u;
	return (xorshifted >> rot) | (xorshifted << ((-rot) & 31));
}

// Get next random double from range [0.0,1.0] using the passed pcg32 rng
static inline double Pcg32NextDouble(pcg32_random_t* restrict rng)
{
	return ((double)Pcg32Next(rng) / (double)UINT32_MAX);
}

// Advance global pcg32 state and get the next random uint32_t value
static inline uint32_t Pcg32GlobalNext()
{
	return Pcg32Next(&pcg32_global);
}

// Advance global pcg32 state and get the next random uint32_t value divided by UINT32_MAX that gives a double in range [0.0,1.0] with 1 / UINT32_MAX stepping
static inline double Pcg32GlobalNextDouble()
{
	return ((double)Pcg32GlobalNext() / (double)UINT32_MAX);
}