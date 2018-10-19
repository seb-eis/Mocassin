using System;
using System.Threading;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Carries marshal target info. Size info of struct and a pointer to unmanaged memory of correct size
    /// </summary>
    public class MarshalTarget
    {
        /// <summary>
        ///     The lock object to mark that the target is in use
        /// </summary>
        public object LockObj { get; } = new object();

        public MarshalTarget(IntPtr pointer, int typeSize)
        {
            Pointer = pointer;
            TypeSize = typeSize;
        }

        /// <summary>
        ///     Pointer to unmanaged memory
        /// </summary>
        public IntPtr Pointer { get; }

        /// <summary>
        ///     Size of the unmanaged memory
        /// </summary>
        public int TypeSize { get; }

        /// <summary>
        ///     Tries to get an exclusive lock on the target
        /// </summary>
        /// <returns></returns>
        public LockedMarshalTarget GetLocked()
        {
            return Monitor.TryEnter(LockObj) 
                ? new LockedMarshalTarget(this)
                : null;
        }

        /// <summary>
        ///     Unlocks the target
        /// </summary>
        public void Unlock()
        {
            Monitor.Exit(LockObj);
        }
    }

    /// <summary>
    ///     Locked marshal target that unlocks on dispose
    /// </summary>
    public class LockedMarshalTarget : IDisposable
    {
        public LockedMarshalTarget(MarshalTarget target)
        {
            Target = target ?? throw new ArgumentNullException(nameof(target));
        }

        /// <summary>
        ///     The locked marshal target
        /// </summary>
        private MarshalTarget Target { get; }

        /// <summary>
        ///     Get the unmanaged memory pointer
        /// </summary>
        public IntPtr Pointer => Target.Pointer;

        /// <summary>
        ///     Get the size of the structure
        /// </summary>
        public int TypeSize => Target.TypeSize;

        /// <inheritdoc />
        public void Dispose()
        {
            Target.Unlock();
        }
    }
}