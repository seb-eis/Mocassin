using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Mocassin.Tools.UAccess.Readers
{
    /// <summary>
    ///     Provides generic read only reference access to structure contents of a byte buffer
    /// </summary>
    public class BinaryStructureReader : IDisposable
    {
        /// <summary>
        ///     Get the <see cref="GCHandle" /> that pins the <see cref="Bytes" />
        /// </summary>
        private GCHandle bytesGcHandle;

        /// <summary>
        ///     Get the byte array buffer that is accessed
        /// </summary>
        private byte[] Bytes { get; set; }

        /// <summary>
        ///     Get the length of the accessed binary content
        /// </summary>
        public int ByteCount => Bytes.Length;

        /// <summary>
        ///     Creates a new <see cref="BinaryStructureReader" /> for the provided byte array. Array is pinned till the reader is
        ///     disposed
        /// </summary>
        /// <param name="bytes"></param>
        public BinaryStructureReader(byte[] bytes)
        {
            Bytes = bytes ?? throw new ArgumentNullException(nameof(bytes));
            if (bytes.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(bytes));
            bytesGcHandle = GCHandle.Alloc(Bytes, GCHandleType.Pinned);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            bytesGcHandle.Free();
            Bytes = null;
        }

        /// <summary>
        ///     Gets a reference to a value of specified type from the internal buffer starting at the specified byte start index
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public ref T ReadAs<T>(int startIndex) where T : struct
        {
            if (startIndex < 0 || startIndex + Marshal.SizeOf<T>() > Bytes.Length)
                throw new InvalidOperationException("Access to buffer is out of range.");

            unsafe
            {
                fixed (void* ptr = &Bytes[startIndex])
                {
                    return ref Unsafe.AsRef<T>(ptr);
                }
            }
        }

        /// <summary>
        ///     Gets a <see cref="ReadOnlySpan{T}" /> to a value area of the internal buffer starting at the specified index
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public ReadOnlySpan<T> ReadLengthAs<T>(int startIndex, int length) where T : struct
        {
            if (startIndex < 0 || startIndex + length > Bytes.Length)
                throw new InvalidOperationException("Access to buffer is out of range.");

            unsafe
            {
                fixed (void* ptr = &Bytes[startIndex])
                {
                    return new ReadOnlySpan<T>(ptr, length / Marshal.SizeOf<T>());
                }
            }
        }

        /// <summary>
        ///     Gets a <see cref="ReadOnlySpan{T}" /> to a value area of the internal buffer defined by the passed start and end
        ///     index
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public ReadOnlySpan<T> ReadAreaAs<T>(int startIndex, int endIndex) where T : struct => ReadLengthAs<T>(startIndex, endIndex - startIndex);
    }
}