using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.InteropServices;
using Mocassin.Framework.Extensions;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Provides a wrapper system for <see cref="Array" /> objects to convert from and into the binary layout used by the C
    ///     simulator
    /// </summary>
    public abstract class InteropArray<T> : BlobEntityBase where T : struct
    {
        /// <summary>
        ///     Get the byte array used to mark the four potentially required padding bytes
        /// </summary>
        private static byte[] PaddingMarkerBytes { get; } = {0xFF, 0xFF, 0xFF, 0xFF};

        /// <summary>
        ///     Get the wrapped <see cref="Array" />
        /// </summary>
        [NotMapped]
        public Array InternalArray { get; private set; }

        /// <summary>
        ///     Blocks lengths to access the linear array by dimensions
        /// </summary>
        [NotMapped]
        public int[] BlockLengths { get; set; }

        /// <summary>
        ///     The rank of the matrix entity
        /// </summary>
        [NotMapped]
        public int Rank => InternalArray?.Rank ?? 0;

        /// <summary>
        ///     The total length of the matrix entries in all dimensions
        /// </summary>
        [NotMapped]
        public int Length => InternalArray?.Length ?? 0;

        /// <summary>
        ///     Creates new interop binary array from an array object
        /// </summary>
        /// <param name="array"></param>
        protected InteropArray(Array array)
        {
            InternalArray = array ?? throw new ArgumentNullException(nameof(array));
            Initialize(array);
        }

        /// <summary>
        ///     Creates an empty <see cref="InteropArray{T}"/>
        /// </summary>
        protected InteropArray()
        {

        }

        /// <summary>
        ///     Get the value at the given indices. Throws if the number of indices does not match dimension (Slow, not intended
        ///     for frequent usage)
        /// </summary>
        /// <param name="indices"></param>
        /// <returns></returns>
        public T GetValue(params int[] indices)
        {
            return (T) InternalArray.GetValue(indices);
        }

        /// <summary>
        ///     Set the value at the given indices. Throws if the number of indices does not match dimension (Slow, not intended
        ///     for frequent usage)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="indices"></param>
        public void SetValue(T value, params int[] indices)
        {
            InternalArray.SetValue(value, indices);
        }

        /// <summary>
        ///     Get the header byte count with proper padding for 8 byte alignment
        /// </summary>
        /// <param name="rank"></param>
        /// <returns></returns>
        protected int GetHeaderByteCount(int rank)
        {
            var byteCount = (rank + 1) * sizeof(int);
            byteCount += byteCount % sizeof(long);
            return byteCount;
        }

        /// <summary>
        ///     Get a boolean flag if padding bytes are required to get 8 byte alignment for the provided rank
        /// </summary>
        /// <param name="rank"></param>
        /// <returns></returns>
        protected bool IsPaddingRequired(int rank)
        {
            return rank % 2 == 0;
        }

        /// <summary>
        ///  Get the number of index skip entries by the array rank
        /// </summary>
        /// <param name="rank"></param>
        /// <returns></returns>
        protected int GetItemBlocksCount(int rank)
        {
            return rank - 1;
        }

        /// <inheritdoc />
        public override void ChangeStateToObject(IMarshalService marshalService)
        {
            if (BinaryState != null)
            {
                var rank = ReadRankFromBinaryState();
                var blocksCount = GetItemBlocksCount(rank);

                HeaderByteCount = GetHeaderByteCount(rank);
                BlockLengths = new int[blocksCount];
                var dataLength = (BinaryState.Length - HeaderByteCount) / Marshal.SizeOf<T>();

                for (var i = 0; i < BlockLengths.Length; i++)
                    BlockLengths[i] = BitConverter.ToInt32(BinaryState, sizeof(int) * (2 + i));

                var dimensions = BlocksToDimensions(BlockLengths, dataLength);
                InternalArray = Array.CreateInstance(typeof(T), dimensions);
                Buffer.BlockCopy(BinaryState, HeaderByteCount, InternalArray, 0, BinaryState.Length - HeaderByteCount);
                Initialize(InternalArray);
            }

            BinaryState = null;
        }

        /// <summary>
        ///     Converts the blocks sizes and total item count into the dimensions of the rectangular array
        /// </summary>
        /// <param name="blocks"></param>
        /// <param name="dataLength"></param>
        /// <returns></returns>
        protected int[] BlocksToDimensions(int[] blocks, int dataLength)
        {
            var dimensions = new int[blocks.Length + 1];
            for (var i = 0; i < blocks.Length; i++)
            {
                dimensions[i] = dataLength / blocks[i];
                dataLength = blocks[i];
            }

            dimensions[BlockLengths.Length] = dataLength;
            return dimensions;
        }

        /// <inheritdoc />
        public override void ChangeStateToBinary(IMarshalService marshalService)
        {
            if (InternalArray != null)
            {
                var itemSize = Marshal.SizeOf(default(T));
                var binarySize = HeaderByteCount + itemSize * Length;
                BinaryState = new byte[binarySize];

                Buffer.BlockCopy(BitConverter.GetBytes(Rank), 0, BinaryState, 0, sizeof(int));
                Buffer.BlockCopy(BitConverter.GetBytes(Length), 0, BinaryState, sizeof(int), sizeof(int));
                Buffer.BlockCopy(BlockLengths, 0, BinaryState, 2 * sizeof(int), BlockLengths.Length * sizeof(int));
                if (IsPaddingRequired(Rank))
                    Buffer.BlockCopy(PaddingMarkerBytes, 0, BinaryState, HeaderByteCount - sizeof(int), sizeof(int));
                Buffer.BlockCopy(InternalArray, 0, BinaryState, HeaderByteCount, BinaryState.Length - HeaderByteCount);
            }

            InternalArray = null;
            BlockLengths = null;
        }

        /// <summary>
        ///     Reads the rank of the array from the binary state
        /// </summary>
        /// <returns></returns>
        public int ReadRankFromBinaryState()
        {
            if (BinaryState == null || BinaryState.Length == 0) return 0;
            return BitConverter.ToInt32(BinaryState, 0);
        }

        /// <summary>
        ///     Get the 1-dimensional index of the m-dimensional index access to the matrix
        /// </summary>
        /// <param name="indices"></param>
        /// <returns></returns>
        public int IndexOf(params int[] indices)
        {
            if (indices.Length != Rank)
                throw new ArgumentException($"Cannot access matrix as dimension {indices.Length}");

            return indices[indices.Length - 1] + BlockLengths.Select((t, i) => indices[i] * t).Sum();
        }

        /// <summary>
        ///     Get the n-dimensional indices that matches the provided linear index in the matrix
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int[] IndicesOf(int index)
        {
            var indices = new int[Rank];
            for (var i = 0; i < BlockLengths.Length; i++)
            {
                indices[i] = index / BlockLengths[i];
                index %= BlockLengths[i];
            }

            indices[BlockLengths.Length] = index;
            return indices;
        }

        /// <summary>
        ///     Get the dimensions of the interop array
        /// </summary>
        /// <returns></returns>
        public int[] GetDimensions()
        {
            return BlocksToDimensions(BlockLengths, Length);
        }

        /// <summary>
        ///     Initializes the interop binary array from an arbitrary array with correct element type
        /// </summary>
        /// <param name="array"></param>
        /// <exception cref="ArgumentException">
        ///     If the element type of the array cannot be assigned to the type of the interop array
        /// </exception>
        private void Initialize(Array array)
        {
            if (!typeof(T).IsAssignableFrom(array.GetType().GetElementType()))
                throw new ArgumentException($"Passed array element type cannot be assigned to type {typeof(T)}", nameof(array));
            BlockLengths = array.MakeBlockItemCounts();
            HeaderByteCount = GetHeaderByteCount(array.Rank);
        }
    }
}