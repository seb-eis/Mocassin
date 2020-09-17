using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Provides thread safe access to unmanaged memory for marshal operations of structs
    /// </summary>
    public class MarshalMemoryProvider : IDisposable
    {
        /// <summary>
        ///     The lock object to mark that the target is in use
        /// </summary>
        public object LockObj { get; } = new object();

        /// <summary>
        ///     Pointer to unmanaged memory
        /// </summary>
        public IntPtr Pointer { get; }

        /// <summary>
        ///     Size of the unmanaged memory
        /// </summary>
        public int TypeSize { get; }

        /// <summary>
        ///     Creates new marshal memory provider
        /// </summary>
        /// <param name="pointer"></param>
        /// <param name="typeSize"></param>
        public MarshalMemoryProvider(IntPtr pointer, int typeSize)
        {
            Pointer = pointer;
            TypeSize = typeSize;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Marshal.FreeHGlobal(Pointer);
        }

        /// <summary>
        ///     Tries to get an exclusive lock on the target
        /// </summary>
        /// <returns></returns>
        public LockedMarshalMemory GetLocked() =>
            Monitor.TryEnter(LockObj)
                ? new LockedMarshalMemory(this)
                : null;

        /// <summary>
        ///     Unlocks the target
        /// </summary>
        public void Unlock()
        {
            Monitor.Exit(LockObj);
        }
    }

    /// <summary>
    ///     Locked marshal memory that encapsulates a marshal memory provider and unlocks it on dispose
    /// </summary>
    public class LockedMarshalMemory : IDisposable
    {
        /// <summary>
        ///     The locked marshal memory provider
        /// </summary>
        private MarshalMemoryProvider MemoryProvider { get; }

        /// <summary>
        ///     Get the unmanaged memory pointer
        /// </summary>
        public IntPtr Pointer => MemoryProvider.Pointer;

        /// <summary>
        ///     Get the size of the structure
        /// </summary>
        public int TypeSize => MemoryProvider.TypeSize;

        /// <summary>
        ///     Creates a <see cref="LockedMarshalMemory" /> using a <see cref="MarshalMemoryProvider" />
        /// </summary>
        /// <param name="memoryProvider"></param>
        public LockedMarshalMemory(MarshalMemoryProvider memoryProvider)
        {
            MemoryProvider = memoryProvider ?? throw new ArgumentNullException(nameof(memoryProvider));
        }

        /// <inheritdoc />
        public void Dispose()
        {
            MemoryProvider.Unlock();
        }
    }
}