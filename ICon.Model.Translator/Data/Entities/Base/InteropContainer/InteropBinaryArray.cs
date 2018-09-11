using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.ComponentModel.DataAnnotations.Schema;

using ICon.Framework.Extensions;
using System.Collections;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Linearized binray interop array to provide multidimensional arrays of structs to unmanaged code
    /// </summary>
    public class InteropBinaryArray<T> : BlobEntityBase where T : struct
    {
        /// <summary>
        /// The value array of the matrix entity
        /// </summary>
        [NotMapped]
        public T[] Values { get; set; }

        /// <summary>
        /// Index skips to access the linear array by dimensions
        /// </summary>
        [NotMapped]
        public int[] IndexSkips { get; set; }

        /// <summary>
        /// The dimension of the matrix entity
        /// </summary>
        [NotMapped]
        public int Rank => (IndexSkips?.Length ?? -1) + 1;

        /// <summary>
        /// The total length of the matrix entries in all dimensions
        /// </summary>
        [NotMapped]
        public int Length => Values?.Length ?? 0;

        /// <summary>
        /// Get the value at the given indices. Throws if the number of indices does not match dimension
        /// </summary>
        /// <param name="indices"></param>
        /// <returns></returns>
        public T GetValue(params int[] indices)
        {
            return Values[IndexOf(indices)];
        }

        /// <summary>
        /// Set the value at the given indices. Throws if the number of indices does not match dimension
        /// </summary>
        /// <param name="value"></param>
        /// <param name="indices"></param>
        public void SetValue(T value, params int[] indices)
        {
            Values[IndexOf(indices)] = value;
        }


        /// <summary>
        /// Parses the binary header and data array and populates the matrix. Nulls out the binary info after completion
        /// </summary>
        public override void ChangeStateToObject(IMarshalProvider marshalProvider)
        {
            int itemSize = Marshal.SizeOf(default(T));
            int arraySize = (BinaryState.Length - HeaderSize) / itemSize;

            IndexSkips = new int[HeaderSize / sizeof(int) - 2];

            for (int i = 0; i < IndexSkips.Length; i++)
            {
                IndexSkips[i] = BitConverter.ToInt32(BinaryState, sizeof(int) * (2 + i));
            }

            Values = new T[arraySize].Populate(marshalProvider.BytesToManyStructures<T>(BinaryState, HeaderSize, BinaryState.Length));

            BinaryState = null;
        }

        /// <summary>
        /// Parses the object into its binary header and data array. Nulls out the data objects afterwards
        /// </summary>
        public override void ChangeStateToBinary(IMarshalProvider marshalProvider)
        {
            int itemSize = Marshal.SizeOf(default(T));
            BinaryState = new byte[HeaderSize + itemSize * Values.Length];

            Buffer.BlockCopy(BitConverter.GetBytes(Rank), 0, BinaryState, 0, sizeof(int));
            Buffer.BlockCopy(BitConverter.GetBytes(Length), 0, BinaryState, sizeof(int), sizeof(int));
            Buffer.BlockCopy(IndexSkips, 0, BinaryState, 2 * sizeof(int), IndexSkips.Length * sizeof(int));

            marshalProvider.ManyStructuresToBytes(BinaryState, HeaderSize, Values);

            Values = null;
            IndexSkips = new int[0];
        }

        /// <summary>
        /// Get the 1-dimensional index of the m-dimensional index access to the matrix
        /// </summary>
        /// <param name="indices"></param>
        /// <returns></returns>
        public int IndexOf(params int[] indices)
        {
            if (indices.Length != Rank)
            {
                throw new ArgumentException($"Cannot access matrix as dimension {indices.Length}");
            }

            int index = indices[indices.Length - 1];
            for (int i = 0; i < IndexSkips.Length; i++)
            {
                index += indices[i] * IndexSkips[i];
            }
            return index;
        }

        /// <summary>
        /// Get the n-dimensional indices that matches the provided linear index in the matrix
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int[] IndicesOf(int index)
        {
            var indices = new int[Rank];
            for (int i = 0; i < IndexSkips.Length; i++)
            {
                indices[i] = index / IndexSkips[i];
                index = index % IndexSkips[i];
            }
            indices[IndexSkips.Length] = index;
            return indices;
        }

        /// <summary>
        /// Get the number of bytes for the binary header
        /// </summary>
        /// <returns></returns>
        protected int GetHeaderSize()
        {
            return sizeof(int) * (2 + IndexSkips.Length);
        }

        /// <summary>
        /// Creates a new matrix entity from an array and linerizes the values within the array
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static InteropBinaryArray<T> FromArray(Array array)
        {
            if (!typeof(T).IsAssignableFrom(array.GetType().GetElementType()))
            {
                throw new ArgumentException($"Passed array elemnt type cannot be assigned to type {typeof(T)}", nameof(array));
            }

            var interopArray = new InteropBinaryArray<T>
            {
                IndexSkips = array.GetDimensionIndexSkips()
            };
            interopArray.HeaderSize = interopArray.GetHeaderSize();

            interopArray.Values = new T[array.Length];
            int index = 0;
            foreach (T item in array)
            {
                interopArray.Values[index++] = item;
            }
            return interopArray;
        }
    }
}
