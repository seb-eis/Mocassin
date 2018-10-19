using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Extension methods for the translator and data context related functionality
    /// </summary>
    public static class TranslatorExtensions
    {
        /// <summary>
        ///     Marshals a sequence of structs into the provided buffer byte array starting at the given index
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <param name="target"></param>
        /// <param name="startByte"></param>
        public static void UnmanagedToByteArray<T>(this IEnumerable<T> values, byte[] target, int startByte) where T : struct
        {
            var itemSize = Marshal.SizeOf<T>();
            var index = 0;

            var ptr = Marshal.AllocHGlobal(itemSize);
            foreach (var item in values)
            {
                Marshal.StructureToPtr(item, ptr, true);
                Marshal.Copy(ptr, target, startByte + index * itemSize, itemSize);
                index++;
            }

            Marshal.FreeHGlobal(ptr);
        }

        /// <summary>
        ///     Marshals a binary array into a sequence of structs beginning at the provided index
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="startByte"></param>
        /// <returns></returns>
        public static IEnumerable<T> ByteArrayToUnmanaged<T>(this byte[] array, int startByte) where T : struct
        {
            var itemSize = Marshal.SizeOf<T>();
            var itemCount = (array.Length - startByte) / itemSize;

            var ptr = Marshal.AllocHGlobal(itemSize);
            for (var i = 0; i < itemCount; i++)
            {
                Marshal.Copy(array, startByte + i * itemSize, ptr, itemSize);
                yield return (T) Marshal.PtrToStructure(ptr, typeof(T));
            }

            Marshal.FreeHGlobal(ptr);
        }

        /// <summary>
        ///     Marshals a byte array into a single struct
        /// </summary>
        /// <typeparam name="TUnmanaged"></typeparam>
        /// <param name="binary"></param>
        /// <returns></returns>
        public static TUnmanaged ByteArrayToStruct<TUnmanaged>(this byte[] binary) where TUnmanaged : struct
        {
            var size = Marshal.SizeOf<TUnmanaged>();

            if (size != binary.Length) 
                throw new ArgumentException("Array has wrong size", nameof(binary));

            var ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(binary, 0, ptr, size);
            var result = Marshal.PtrToStructure<TUnmanaged>(ptr);
            Marshal.FreeHGlobal(ptr);
            return result;
        }
    }
}