using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.InteropServices;
using Mocassin.Framework.Extensions;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Linearized binary-convertible interop array to provide multidimensional arrays of structs to unmanaged code
    /// </summary>
    public class InteropArray<T> : BlobEntityBase where T : struct
    {
        /// <summary>
        ///     Creates new empty interop binary array
        /// </summary>
        public InteropArray()
        {
        }

        /// <summary>
        ///     Creates new interop binary array from an array object
        /// </summary>
        /// <param name="array"></param>
        public InteropArray(Array array)
        {
            PopulateFrom(array);
        }

        /// <summary>
        ///     The value array of the matrix entity
        /// </summary>
        [NotMapped]
        public T[] Values { get; set; }

        /// <summary>
        ///     Index skips to access the linear array by dimensions
        /// </summary>
        [NotMapped]
        public int[] IndexSkips { get; set; }

        /// <summary>
        ///     The rank of the matrix entity
        /// </summary>
        [NotMapped]
        public int Rank => (IndexSkips?.Length ?? -1) + 1;

        /// <summary>
        ///     The total length of the matrix entries in all dimensions
        /// </summary>
        [NotMapped]
        public int Length => Values?.Length ?? 0;

        /// <summary>
        ///     Get the value at the given indices. Throws if the number of indices does not match dimension
        /// </summary>
        /// <param name="indices"></param>
        /// <returns></returns>
        public T GetValue(params int[] indices)
        {
            return Values[IndexOf(indices)];
        }

        /// <summary>
        ///     Set the value at the given indices. Throws if the number of indices does not match dimension
        /// </summary>
        /// <param name="value"></param>
        /// <param name="indices"></param>
        public void SetValue(T value, params int[] indices)
        {
            Values[IndexOf(indices)] = value;
        }

        /// <inheritdoc />
        public override void ChangeStateToObject(IMarshalService marshalService)
        {
            if (BinaryState != null)
            {
                HeaderByteCount = (GetRankFromBinaryState() + 1) * sizeof(int);
                var itemSize = Marshal.SizeOf(default(T));
                var arraySize = (BinaryState.Length - HeaderByteCount) / itemSize;

                IndexSkips = new int[HeaderByteCount / sizeof(int) - 2];

                for (var i = 0; i < IndexSkips.Length; i++)
                    IndexSkips[i] = BitConverter.ToInt32(BinaryState, sizeof(int) * (2 + i));

                Values = new T[arraySize].Populate(marshalService.GetStructures<T>(BinaryState, HeaderByteCount, BinaryState.Length));
            }

            BinaryState = null;
        }

        /// <inheritdoc />
        public override void ChangeStateToBinary(IMarshalService marshalService)
        {
            if (Values != null)
            {
                var itemSize = Marshal.SizeOf(default(T));
                BinaryState = new byte[HeaderByteCount + itemSize * Length];

                Buffer.BlockCopy(BitConverter.GetBytes(Rank), 0, BinaryState, 0, sizeof(int));
                Buffer.BlockCopy(BitConverter.GetBytes(Length), 0, BinaryState, sizeof(int), sizeof(int));
                Buffer.BlockCopy(IndexSkips, 0, BinaryState, 2 * sizeof(int), IndexSkips.Length * sizeof(int));

                marshalService.GetBytes(BinaryState, HeaderByteCount, Values);
            }

            Values = null;
            IndexSkips = null;
        }

        /// <summary>
        ///     reads the rank of the array from the binary state
        /// </summary>
        /// <returns></returns>
        public int GetRankFromBinaryState()
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
                index = index % IndexSkips[i];
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
        protected int GetHeaderSize()
        {
            return sizeof(int) * (2 + IndexSkips.Length);
        }

        /// <summary>
        ///     Populates the interop binary array from an arbitrary array with correct element type
        /// </summary>
        /// <param name="array"></param>
        /// <exception cref="ArgumentException">
        ///     If the element type of the array cannot be assigned to the type of the interop
        ///     array
        /// </exception>
        public void PopulateFrom(Array array)
        {
            if (!typeof(T).IsAssignableFrom(array.GetType().GetElementType()))
                throw new ArgumentException($"Passed array element type cannot be assigned to type {typeof(T)}", nameof(array));

            IndexSkips = array.GetDimensionIndexSkips();
            HeaderByteCount = GetHeaderSize();

            Values = new T[array.Length];
            Buffer.BlockCopy(array, 0, Values, 0, array.Length * Marshal.SizeOf(default(T)));
        }

        /// <summary>
        ///     Creates a new interop array from an arbitrary array and flattens out the values within the array into a linear list
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static TArray FromArray<TArray>(Array array) where TArray : InteropArray<T>, new()
        {
            var result = new TArray();
            result.PopulateFrom(array);
            return result;
        }
    }
}