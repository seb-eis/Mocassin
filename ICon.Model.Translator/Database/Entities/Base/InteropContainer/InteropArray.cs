using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.InteropServices;
using Mocassin.Framework.Extensions;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Provides a wrapper system for <see cref="Array"/> objects to convert from and into the binary layout used by the C simulator
    /// </summary>
    public abstract class InteropArray<T> : BlobEntityBase where T : struct
    {
        /// <summary>
        ///     Get the wrapped <see cref="Array"/>
        /// </summary>
        public Array InternalArray { get; private set; }

        /// <summary>
        ///     Index skips to access the linear array by dimensions
        /// </summary>
        [NotMapped]
        public int[] IndexSkips { get; set; }

        /// <summary>
        ///     The rank of the matrix entity
        /// </summary>
        [NotMapped]
        public int Rank { get; }

        /// <summary>
        ///     The total length of the matrix entries in all dimensions
        /// </summary>
        [NotMapped]
        public int Length { get; }

        /// <summary>
        ///     Creates new interop binary array from an array object
        /// </summary>
        /// <param name="array"></param>
        protected InteropArray(Array array)
        {
            InternalArray = array ?? throw new ArgumentNullException(nameof(array));
            Length = array.Length;
            Rank = array.Rank;
            Initialize(array);
        }

        /// <summary>
        ///     Get the value at the given indices. Throws if the number of indices does not match dimension (Slow, not intended for frequent usage)
        /// </summary>
        /// <param name="indices"></param>
        /// <returns></returns>
        public T GetValue(params int[] indices)
        {
            return (T) InternalArray.GetValue(indices);
        }

        /// <summary>
        ///     Set the value at the given indices. Throws if the number of indices does not match dimension (Slow, not intended for frequent usage)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="indices"></param>
        public void SetValue(T value, params int[] indices)
        {
            InternalArray.SetValue(value, indices);
        }

        /// <inheritdoc />
        public override void ChangeStateToObject(IMarshalService marshalService)
        {
            if (BinaryState != null)
            {
                HeaderByteCount = (ReadRankFromBinaryState() + 1) * sizeof(int);

                IndexSkips = new int[HeaderByteCount / sizeof(int) - 2];

                for (var i = 0; i < IndexSkips.Length; i++)
                    IndexSkips[i] = BitConverter.ToInt32(BinaryState, sizeof(int) * (2 + i));

                InternalArray = Array.CreateInstance(typeof(T), GetDimensions());
                Buffer.BlockCopy(BinaryState, HeaderByteCount, InternalArray, 0, BinaryState.Length - HeaderByteCount);
            }

            BinaryState = null;
        }

        /// <inheritdoc />
        public override void ChangeStateToBinary(IMarshalService marshalService)
        {
            if (InternalArray != null)
            {
                var itemSize = Marshal.SizeOf(default(T));
                BinaryState = new byte[HeaderByteCount + itemSize * Length];

                Buffer.BlockCopy(BitConverter.GetBytes(Rank), 0, BinaryState, 0, sizeof(int));
                Buffer.BlockCopy(BitConverter.GetBytes(Length), 0, BinaryState, sizeof(int), sizeof(int));
                Buffer.BlockCopy(IndexSkips, 0, BinaryState, 2 * sizeof(int), IndexSkips.Length * sizeof(int));
                Buffer.BlockCopy(InternalArray, 0, BinaryState, HeaderByteCount, BinaryState.Length - HeaderByteCount);
            }

            InternalArray = null;
            IndexSkips = null;
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

            return indices[indices.Length - 1] + IndexSkips.Select((t, i) => indices[i] * t).Sum();
        }

        /// <summary>
        ///     Get the n-dimensional indices that matches the provided linear index in the matrix
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int[] IndicesOf(int index)
        {
            var indices = new int[Rank];
            for (var i = 0; i < IndexSkips.Length; i++)
            {
                indices[i] = index / IndexSkips[i];
                index %= IndexSkips[i];
            }

            indices[IndexSkips.Length] = index;
            return indices;
        }

        /// <summary>
        ///     Get the dimensions of the interop array
        /// </summary>
        /// <returns></returns>
        public int[] GetDimensions()
        {
            var result = IndicesOf(Length - 1);
            for (var i = 0; i < result.Length; i++)
                result[i]++;

            return result;
        }

        /// <summary>
        ///     Get the number of bytes for the binary header
        /// </summary>
        /// <returns></returns>
        protected int CalculateHeaderSize()
        {
            return sizeof(int) * (2 + IndexSkips.Length);
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
            IndexSkips = array.GetDimensionIndexSkips();
            HeaderByteCount = CalculateHeaderSize();
        }
    }
}