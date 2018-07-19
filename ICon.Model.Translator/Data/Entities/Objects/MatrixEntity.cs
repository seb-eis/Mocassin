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
    /// Linearized matrix entity for struct types that supports n dimensional matrix definitions and storage as data + header blob in the database
    /// </summary>
    public class MatrixEntity<T> : BlobEntity where T : struct
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
        public int Dimension => IndexSkips == null ? -1 : IndexSkips.Length + 1;

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
        public override void BinaryToData()
        {
            int itemSize = Marshal.SizeOf(default(T));
            IndexSkips = new int[HeaderBinary.Length / sizeof(int) - 1];

            for (int i = 0; i < IndexSkips.Length; i++)
            {
                IndexSkips[i] = BitConverter.ToInt32(HeaderBinary, 4 * (1 + i));
            }

            Values = new T[DataBinary.Length / itemSize];
            var ptr = Marshal.AllocHGlobal(itemSize);
            for (int i = 0; i < Values.Length; i++)
            {
                Marshal.Copy(DataBinary, i * itemSize, ptr, itemSize);
                Values[i] = (T)Marshal.PtrToStructure(ptr, typeof(T));
            }
            Marshal.FreeHGlobal(ptr);
            NullBinaries();
        }

        /// <summary>
        /// Parses the object into its binary header and data array. Nulls out the data objects afterwards
        /// </summary>
        public override void DataToBinary()
        {
            HeaderBinary = new byte[(1 + IndexSkips.Length) * sizeof(int)];
            Buffer.BlockCopy(BitConverter.GetBytes(Dimension), 0, HeaderBinary, 0, sizeof(int));
            Buffer.BlockCopy(IndexSkips, 0, HeaderBinary, sizeof(int), IndexSkips.Length * sizeof(int));

            int size = Marshal.SizeOf(default(T));
            DataBinary = new byte[size * Values.Length];
            var ptr = Marshal.AllocHGlobal(size);
            for (int i = 0; i < Values.Length; i++)
            {
                Marshal.StructureToPtr(Values[i], ptr, true);
                Marshal.Copy(ptr, DataBinary, i * size, size);
            }
            Marshal.FreeHGlobal(ptr);

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
            if (indices.Length != Dimension)
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
            var indices = new int[Dimension];
            for (int i = 0; i < IndexSkips.Length; i++)
            {
                indices[i] = index / IndexSkips[i];
                index = index % IndexSkips[i];
            }
            indices[IndexSkips.Length] = index;
            return indices;
        }

        /// <summary>
        /// Creates a new matrix entity from an array and linerizes the values within the array
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static MatrixEntity<T> FromArray(Array array)
        {
            if (!typeof(T).IsAssignableFrom(array.GetType().GetElementType()))
            {
                throw new ArgumentException($"Passed array elemnt type cannot be assigned to type {typeof(T)}", nameof(array));
            }

            var entity = new MatrixEntity<T>
            {
                IndexSkips = array.GetDimensionIndexSkips()
            };
            entity.Values = new T[array.Length];
            int index = 0;
            foreach (T item in array)
            {
                entity.Values[index++] = item;
            }
            return entity;
        }
    }
}
