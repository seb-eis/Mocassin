using System;
using System.Collections.Generic;

namespace Mocassin.Mathematics.Codes
{
    /// <summary>
    ///     Represents a index code value that contains 8 byte entries
    /// </summary>
    public readonly struct ByteCode64
    {
        /// <summary>
        ///     The encoded value
        /// </summary>
        private readonly long encoded;

        /// <summary>
        ///     Creates a ne <see cref="ByteCode64" /> from a long value
        /// </summary>
        /// <param name="encoded"></param>
        public ByteCode64(long encoded)
        {
            this.encoded = encoded;
        }

        /// <summary>
        ///     Get the data as a long value
        /// </summary>
        /// <returns></returns>
        public long AsLong()
        {
            return encoded;
        }

        /// <summary>
        ///     Decodes the value into a byte array
        /// </summary>
        /// <returns></returns>
        public byte[] Decode()
        {
            return BitConverter.GetBytes(encoded);
        }

        /// <summary>
        ///     Access the code entries by index
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public byte this[int i]
        {
            get
            {
                if (i > 7) throw new IndexOutOfRangeException("Code has only 8 entries");
                return (byte) ((encoded >> (8 * i)) & 0xFF);
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"Code: {encoded} [{this[0]}-{this[1]}-{this[2]}-{this[3]}-{this[4]}-{this[5]}-{this[6]}-{this[7]}]";
        }

        /// <summary>
        ///     Create a <see cref="ByteCode64" /> from a byte array with 8 entries
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static ByteCode64 FromBytes(byte[] bytes)
        {
            if (bytes.Length != 8) throw new InvalidOperationException("Array does not contain 8 entries.");
            return new ByteCode64(BitConverter.ToInt64(bytes, 0));
        }

        /// <summary>
        ///     Creates a new <see cref="ByteCode64" /> from a sequence of indices and populates the trailing values with the
        ///     provided data
        /// </summary>
        /// <param name="values"></param>
        /// <param name="trailingValues"></param>
        /// <returns></returns>
        public static ByteCode64 FromIndices(IEnumerable<int> values, byte trailingValues = 0)
        {
            var buffer = new byte[8];
            var index = -1;
            foreach (var item in values)
            {
                if (item < 0 || item > byte.MaxValue) throw new InvalidOperationException("Cannot safely encode negative or our of range indices.");
                buffer[++index] = (byte) item;
            }

            if (trailingValues != 0)
                while (++index < buffer.Length)
                    buffer[index] = trailingValues;
            var code = BitConverter.ToInt64(buffer, 0);
            return new ByteCode64(code);
        }
    }
}