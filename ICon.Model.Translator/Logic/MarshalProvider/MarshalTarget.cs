using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Reactive.Disposables;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Carries marshal target info. Size info of struct and a pointer to unmanaged memory of correct size
    /// </summary>
    public class MarshalTarget
    {
        /// <summary>
        /// The lock object to mark that the target is in use
        /// </summary>
        public object LockObj { get; } = new object();

        public MarshalTarget(IntPtr pointer, int typeSize)
        {
            Pointer = pointer;
            TypeSize = typeSize;
        }

        /// <summary>
        /// Pointer to unmanaged memory
        /// </summary>
        public IntPtr Pointer { get; }

        /// <summary>
        ///  Size of the unmnagaed memory
        /// </summary>
        public int TypeSize { get; }

        /// <summary>
        /// Triesto get an exlusiv loc on the target
        /// </summary>
        /// <returns></returns>
        public LockedMarshalTarget GetLocked()
        {
            if (Monitor.TryEnter(LockObj))
            {
                return new LockedMarshalTarget(this);
            }
            return null;
        }

        /// <summary>
        /// Unlocks the target
        /// </summary>
        public void Unlock()
        {
            Monitor.Exit(LockObj);
        }
    }

    /// <summary>
    /// Locked marshal target that unlocks on dispose
    /// </summary>
    public class LockedMarshalTarget : IDisposable
    {
        public LockedMarshalTarget(MarshalTarget target)
        {
            Target = target ?? throw new ArgumentNullException(nameof(target));
        }

        /// <summary>
        /// The locked marshal target
        /// </summary>
        private MarshalTarget Target { get; }

        /// <summary>
        /// Get the unmanaged memory pointer
        /// </summary>
        public IntPtr Pointer => Target.Pointer;

        /// <summary>
        /// Get the size of the structure
        /// </summary>
        public int TypeSize => Target.TypeSize;

        /// <summary>
        /// Unlocks the wrapped masrhal target
        /// </summary>
        public void Dispose()
        {
            Target.Unlock();
        }
    }
}
