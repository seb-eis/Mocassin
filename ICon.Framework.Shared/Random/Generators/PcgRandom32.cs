using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

// *Really* minimal PCG32 code / (c) 2014 M.E. O'Neill / pcg-random.org
// Licensed under Apache License 2.0 (NO WARRANTY, etc. see website)


namespace ICon.Framework.Random
{
    /// <summary>
    /// Implementation of the PCG familiy 32 bit minimal random number generator
    /// </summary>
    [DataContract(Name ="PcgRandom32")]
    public sealed class PcgRandom32
    {
        /// <summary>
        /// The seed state of the generator
        /// </summary>
        public ulong SeedState { get; private set; }

        /// <summary>
        /// The increase value of the generator
        /// </summary>
        public ulong Increment { get; private set; }

        /// <summary>
        /// The current state of the random number generator
        /// </summary>
        public ulong State { get; private set; }

        /// <summary>
        /// Create new random number generator
        /// </summary>
        /// <param name="increase"></param>
        /// <param name="state"></param>
        public PcgRandom32(ulong increase, ulong state)
        {
            SeedState = state;
            Increment = increase | 1;
            State = state;
        }

        /// <summary>
        /// Default initialized random generator
        /// </summary>
        public PcgRandom32() : this(0x853c49e6748fea9bUL | 1, 0xda3e39cb94b95bdbUL)
        {
        }

        /// <summary>
        /// Get the next random 32 bit unsigned integer
        /// </summary>
        /// <returns></returns>
        public uint Next()
        {
            unchecked
            {
                ulong oldState = State;
                State = oldState * 6364136223846793005UL + Increment;
                uint xorShifted = (uint)(((oldState >> 18) ^ oldState) >> 27);
                int rot = (int)(oldState >> 59);
                return ((xorShifted >> rot) | (xorShifted << ((int)((0x80000000U - (uint)rot) & 31U))));
            }
        }

        /// <summary>
        /// Get the next random non negative 32 bit integer
        /// </summary>
        /// <returns></returns>
        public int NextInt()
        {
            unchecked
            {
                return (int)(Next() & (uint.MaxValue >> 1));
            }
        }
    }
}
