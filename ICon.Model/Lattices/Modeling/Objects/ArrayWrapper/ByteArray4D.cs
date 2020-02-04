using System;

namespace Mocassin.Model.Lattices
{
    /// <summary>
    ///     Simple wrapper around a real 4D byte[,,,] object to implement <see cref="IByteArray4D"/>
    /// </summary>
    public class ByteArray4D : IByteArray4D
    {
        /// <summary>
        ///     The actual data byte[,,,]
        /// </summary>
        public byte[,,,] Data { get; }

        /// <inheritdoc />
        public byte this[int a, int b, int c, int d]
        {
            get => Data[a, b, c, d];
            set => Data[a,b,c,d] = value;
        }

        /// <summary>
        ///     Creates new <see cref="ByteArray4D"/> wrapper for the passed byte[,,,]
        /// </summary>
        /// <param name="data"></param>
        public ByteArray4D(byte[,,,] data)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
        }

        /// <inheritdoc />
        public int GetLength(int dimension)
        {
            return Data.GetLength(dimension);
        }

        /// <inheritdoc />
        public byte[,,,] AsArray()
        {
            return Data;
        }
    }
}