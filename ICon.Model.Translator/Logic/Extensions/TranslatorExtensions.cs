using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace ICon.Model.Translator
{
    public static class TranslatorExtensions
    {
        public static void UnmanagedToByteArray<T>(this IEnumerable<T> values, byte[] target, int startByte) where T : struct
        {
            int itemSize = Marshal.SizeOf<T>();
            int index = 0;

            var ptr = Marshal.AllocHGlobal(itemSize);
            foreach (var item in values)
            {
                Marshal.StructureToPtr(item, ptr, true);
                Marshal.Copy(ptr, target, startByte + index * itemSize, itemSize);
                index++;
            }
            Marshal.FreeHGlobal(ptr);
        }

        public static IEnumerable<T> ByteArrayToUnmanaged<T>(this byte[] array, int startByte) where T : struct
        {
            int itemSize = Marshal.SizeOf<T>();
            int itemCount = (array.Length - startByte) / itemSize;

            var ptr = Marshal.AllocHGlobal(itemSize);
            for (int i = 0; i < itemCount; i++)
            {
                Marshal.Copy(array, startByte + i * itemSize, ptr, itemSize);
                yield return (T)Marshal.PtrToStructure(ptr, typeof(T));
            }

            Marshal.FreeHGlobal(ptr);
        }

        public static TUnmanaged ByteArrayToStruct<TUnmanaged>(this byte[] binary) where TUnmanaged : struct
        {
            int size = Marshal.SizeOf<TUnmanaged>();

            if (size != binary.Length)
            {
                throw new ArgumentException("Array has wrong size", nameof(binary));
            }

            var ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(binary, 0, ptr, size);
            var result = Marshal.PtrToStructure<TUnmanaged>(ptr);
            Marshal.FreeHGlobal(ptr);
            return result;
        }
    }
}
