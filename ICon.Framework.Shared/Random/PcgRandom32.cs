using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

// This implementation is based upon:

// *Really* minimal PCG32 code / (c) 2014 M.E. O'Neill / pcg-random.org
// Licensed under Apache License 2.0 (NO WARRANTY, etc. see website)


namespace Mocassin.Framework.Random
{
    /// <summary>
    ///     Implementation of the PCG family 32 bit minimal random number generator
    /// </summary>
    public sealed class PcgRandom32 : System.Random
    {
        /// <summary>
        ///     The default state increment value
        /// </summary>
        public const ulong DefaultIncrement = 0x853c49e6748fea9bUL;

        /// <summary>
        ///     The factor used to create a random double from a random uint
        /// </summary>
        public const double DoubleStepping = 1.0 / uint.MaxValue;

        /// <summary>
        ///     The global pcg 32 random number generator
        /// </summary>
        public static readonly PcgRandom32 Global = new PcgRandom32();

        /// <summary>
        ///     The increase value of the generator
        /// </summary>
        public ulong Increment { get; private set; }

        /// <summary>
        ///     The current state of the random number generator
        /// </summary>
        public ulong State { get; private set; }

        /// <summary>
        ///     Default initialized random generator using the c# crypto provider
        /// </summary>
        public PcgRandom32()
        {
            var provider = new RNGCryptoServiceProvider();
            var buffer = new byte[16];
            provider.GetBytes(buffer);
            Seed(BitConverter.ToUInt64(buffer, 0), BitConverter.ToUInt64(buffer, 8));
        }

        /// <inheritdoc />
        public PcgRandom32(int seed)
        {
            Increment = (DefaultIncrement ^ (((ulong) seed << 32) + (ulong) seed)) | 1;
            State += (ulong) (seed ^ NextUnsigned()) + (ulong) ((seed ^ NextUnsigned()) << 32);
            Seed(Increment, State);
        }

        /// <summary>
        ///     Initializes the generator from a single string value
        /// </summary>
        /// <param name="seed"></param>
        public PcgRandom32(string seed)
            : this(seed.GetHashCode())
        {
        }

        /// <summary>
        ///     Seed the pcg by state and increment value
        /// </summary>
        /// <param name="state"></param>
        /// <param name="increment"></param>
        public PcgRandom32(ulong state, ulong increment)
        {
            Seed(state, increment);
        }

        /// <summary>
        ///     Get the next random 32 bit unsigned integer
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint NextUnsigned()
        {
            unchecked
            {
                var oldState = State;
                State = oldState * 6364136223846793005UL + Increment;
                var xorShifted = (uint) (((oldState >> 18) ^ oldState) >> 27);
                var rot = (int) (oldState >> 59);
                return (xorShifted >> rot) | (xorShifted << (int) ((0x80000000U - (uint) rot) & 31U));
            }
        }

        /// <inheritdoc />
        public override int Next()
        {
            unchecked
            {
                return (int) (NextUnsigned() & (uint.MaxValue >> 1));
            }
        }

        /// <summary>
        ///     Returns the current status of the generator as a string
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"Pcg32 (State: {State}, Increment: {Increment})";

        /// <inheritdoc />
        public override int Next(int maxValue) => maxValue <= 0 ? 0 : Next() % maxValue;

        /// <inheritdoc />
        public override int Next(int minValue, int maxValue) => minValue + Next(maxValue - minValue);

        /// <inheritdoc />
        public override void NextBytes(byte[] buffer)
        {
            for (var i = 0; i < buffer.Length;)
            {
                var random = NextUnsigned();
                for (var j = 0; j < 4; j++)
                {
                    if (i + 1 == buffer.Length) return;
                    buffer[i++] = (byte) ((random >> (j * 8)) & 0xFF);
                }
            }
        }

        /// <inheritdoc />
        public override double NextDouble() => NextUnsigned() * DoubleStepping;

        /// <summary>
        ///     Seeds the random number generator from state and increment
        /// </summary>
        /// <param name="state"></param>
        /// <param name="increment"></param>
        [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
        private void Seed(ulong state, ulong increment)
        {
            Increment = (increment << 1) | 1;
            Next();
            State += state;
            Next();
        }
    }
}