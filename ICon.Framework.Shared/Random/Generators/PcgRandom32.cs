using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

// This implementation is based upon:

// *Really* minimal PCG32 code / (c) 2014 M.E. O'Neill / pcg-random.org
// Licensed under Apache License 2.0 (NO WARRANTY, etc. see website)


namespace ICon.Framework.Random
{
    /// <summary>
    /// Implementation of the PCG familiy 32 bit minimal random number generator
    /// </summary>
    [DataContract(Name = "PcgRandom32")]
    public sealed class PcgRandom32 : System.Random
    {
        /// <summary>
        /// The global pcg 32 random number generator
        /// </summary>
        public static readonly PcgRandom32 Global = new PcgRandom32();

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
        /// Default initialized random generator using the c# crypto provider
        /// </summary>
        public PcgRandom32()
        {
            var provider = new System.Security.Cryptography.RNGCryptoServiceProvider();
            var buffer = new byte[16];
            provider.GetBytes(buffer);
            Seed(BitConverter.ToUInt64(buffer, 0), BitConverter.ToUInt64(buffer, 8));
        }

        /// <summary>
        /// Initializes the generator with a single integer seed value (Inherited from system random)
        /// </summary>
        /// <param name="seed"></param>
        public PcgRandom32(int seed)
        {
            Increment = DefaultIncrement ^ (((ulong)seed << 32) + (ulong)seed) | 1;
            State += (ulong)(seed ^ NextUnsigned()) + (ulong)((seed ^ NextUnsigned()) << 32);
            Seed(Increment, State);
        }

        /// <summary>
        /// Seed the pcg by state and increment value
        /// </summary>
        /// <param name="state"></param>
        /// <param name="increase"></param>
        public PcgRandom32(ulong state, ulong increment)
        {
            Seed(state, increment);
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

        /// <summary>
        /// Seeds the random number genartor from state and increment
        /// </summary>
        /// <param name="state"></param>
        /// <param name="increment"></param>
        private void Seed(ulong state, ulong increment)
        {
            Increment = (increment << 1) | 1;
            Next();
            State += state;
            Next();
        }
    }
}
