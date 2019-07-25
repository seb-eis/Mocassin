using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Mocassin.Tools.UAccess.Readers.Base
{
    /// <summary>
    ///     Provides generic read only reference access to structure contents of a byte buffer
    /// </summary>
    public class BinaryStructureReader
    {
        /// <summary>
        ///     Get the byte array buffer that is accessed
        /// </summary>
        private byte[] Binary { get; }

        /// <summary>
        ///     Get the length of the accessed binary content
        /// </summary>
        public int BinaryLength => Binary.Length;

        /// <summary>
        ///     Creates a new <see cref="BinaryStructureReader" /> for the provided byte array
        /// </summary>
        /// <param name="binary"></param>
        public BinaryStructureReader(byte[] binary)
        {
            Binary = binary ?? throw new ArgumentNullException(nameof(binary));
            if (binary.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(binary));
        }

        /// <summary>
        ///     Gets a reference to a value of specified type from the internal buffer starting at the specified byte start index
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public ref T ReadAs<T>(int startIndex) where T : struct
        {
            if (startIndex < 0 || startIndex + Marshal.SizeOf<T>() > Binary.Length)
                throw new InvalidOperationException("Access to buffer is out of range.");

            unsafe
            {
                fixed (void* ptr = &Binary[startIndex])
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
            if (startIndex < 0 || startIndex + length > Binary.Length)
                throw new InvalidOperationException("Access to buffer is out of range.");

            unsafe
            {
                fixed (void* ptr = &Binary[startIndex])
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
        public ReadOnlySpan<T> ReadAreaAs<T>(int startIndex, int endIndex) where T : struct
        {
            return ReadLengthAs<T>(startIndex, endIndex - startIndex);
        }
    }
}