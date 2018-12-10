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

#define PCG32_STATE_INT	0x853c49e6748fea9bULL
#define PCG32_INC_INT 	0xda3e39cb94b95bdbULL

// Typedef of the pcg32 random number generator
typedef struct Pcg32 { uint64_t State;  uint64_t Inc; } Pcg32_t;

// Get next random unsigned integer from the passed pcg32 rng
static inline uint32_t Pcg32Next(Pcg32_t* restrict rng)
{
	uint64_t oldstate = rng->State;
	rng->State = oldstate * 6364136223846793005ULL + rng->Inc;
	uint32_t xorshifted = ((oldstate >> 18u) ^ oldstate) >> 27u;
	uint32_t rot = oldstate >> 59u;
	return (xorshifted >> rot) | (xorshifted << ((-rot) & 31));
}

static inline uint32_t Pcg32NextCeiled(Pcg32_t* restrict rng, uint32_t ceil)
{
    uint32_t threshold = -ceil % ceil;
    for(;;)
    {
        uint32_t rnv = Pcg32Next(rng);
        if(rnv >= threshold) return rnv % ceil;
    }
}

// Get next random double from range [0.0,1.0] using the passed pcg32 rng
static inline double Pcg32NextDouble(Pcg32_t* restrict rng)
{
	return ((double)Pcg32Next(rng) / (double)UINT32_MAX);
}

// Seed the rng
static inline Pcg32_t* Pcg32Seed(Pcg32_t* restrict rng, uint64_t state, uint64_t inc)
{
	rng->State = 0U;
	rng->Inc = (inc << 1u) | 1u;
	Pcg32Next(rng);
	rng->State += state;
	Pcg32Next(rng);
    return rng;
}

// Global pcg32 random number generator state
static Pcg32_t pcg32Global = { PCG32_STATE_INT, PCG32_INC_INT };

// Advance global pcg32 state and get the next random uint32_t value
static inline uint32_t Pcg32GlobalNext()
{
	return Pcg32Next(&pcg32Global);
}

// Advance global pcg32 state and get the next random uint32_t value divided by UINT32_MAX that gives a double in range [0.0,1.0] with 1 / UINT32_MAX stepping
static inline double Pcg32GlobalNextDouble()
{
	return ((double)Pcg32GlobalNext() / (double)UINT32_MAX);
}

#define new_Pcg32(PCG32, STATE, INC) *Pcg32Seed(&(PCG32), (STATE), (INC))