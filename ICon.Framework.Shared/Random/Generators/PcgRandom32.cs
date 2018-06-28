using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

// *Really* minimal PCG32 code / (c) 2014 M.E. O'Neill / pcg-random.org
// Licensed under Apache License 2.0 (NO WARRANTY, etc. see website)


namespace ICon.Framework.Random
{
    /// <summary>
    /// Implementation of the PCG familiy 32 bit minimal random number generator
    /// </summary>
    [DataContract(Name ="PcgRandom32")]
    public sealed class PcgRandom32 : System.Random
    {
        /// <summary>
        /// The default state initializer value
        /// </summary>
        public const ulong DefaultState = 0xda3e39cb94b95bdbUL;

        /// <summary>
        /// The default state increment value
        /// </summary>
        public const ulong DefaultIncrement = 0x853c49e6748fea9bUL;

        /// <summary>
        /// The factor used to create a random double from a random uint
        /// </summary>
        public const double SampleStepping = 1.0 / uint.MaxValue;

        /// <summary>
        /// The seed state of the generator
        /// </summary>
        public readonly ulong SeedState;

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
        public PcgRandom32() : this(DefaultIncrement, DefaultState)
        {

        }

        /// <summary>
        /// Initializes the generator with a seed value
        /// </summary>
        /// <param name="Seed"></param>
        public PcgRandom32(int Seed)
        {
            SeedState = DefaultState ^ ((ulong)(Seed) + (ulong)(Seed) << 32);
            Increment = (DefaultIncrement ^ ((ulong)(Seed) + (ulong)(Seed) << 32)) | 1;
            State = SeedState;
        }

        /// <summary>
        /// Get the next random 32 bit unsigned integer
        /// </summary>
        /// <returns></returns>
        public uint NextUnsigned()
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
        public override int Next()
        {
            unchecked
            {
                return (int)(NextUnsigned() & (uint.MaxValue >> 1));
            }
        }

        /// <summary>
        /// Returns the current status of the generator as a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Pcg32 (State: {State}, Inc: {Increment}, SeedState: {SeedState})";
        }

        /// <summary>
        /// Get the next random number limited vy the passed upper bound. Returns 0 if upper bound is less or equal to zero
        /// </summary>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public override int Next(int maxValue)
        {
            unchecked
            {
                return (maxValue <= 0) ? 0 : Next() % maxValue;
            }
        }

        /// <summary>
        /// Returns the next random integer in between the passed boundaries that is lesser than max value and greater or equal to min value
        /// </summary>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public override int Next(int minValue, int maxValue)
        {
            return minValue + Next(maxValue - minValue);
        }

        /// <summary>
        /// Fills a buffer with random bytes
        /// </summary>
        /// <param name="buffer"></param>
        public override void NextBytes(byte[] buffer)
        {
            for (int i = -1; i < buffer.Length;)
            {
                var values = BitConverter.GetBytes(NextUnsigned());
                for (int j = 0; j < 4; j++)
                {
                    buffer[++i] = values[j];
                }
            }
        }

        /// <summary>
        /// Get the next double from the range of [0,1]
        /// </summary>
        /// <returns></returns>
        public override double NextDouble()
        {
            unchecked
            {
                return NextUnsigned() * SampleStepping;
            }
        }
    }
}
