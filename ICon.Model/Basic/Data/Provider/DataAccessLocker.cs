using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ICon.Model.Basic
{
    /// <summary>
    /// Access locker for model data that tries to access the data a specified number of times with certain intervals before throwing an exception
    /// </summary>
    public class DataAccessLocker
    {
        /// <summary>
        /// The number of attempts for locking
        /// </summary>
        public Int32 Attempts { get; set; }

        /// <summary>
        /// The interval in between attempts
        /// </summary>
        public TimeSpan Interval { get; set; }

        /// <summary>
        /// Creates new access timer from interval length and attempt counter
        /// </summary>
        /// <param name="attempts"></param>
        /// <param name="interval"></param>
        public DataAccessLocker(Int32 attempts, TimeSpan interval)
        {
            Attempts = attempts;
            Interval = interval;
        }

        /// <summary>
        /// Tries to get a read only lock on the model data object until internal timeout occures
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public IDisposable TryGetReadOnlyLock(ModelData data)
        {
            IDisposable locker = null;
            for (Int32 i = 0; i < Attempts; i++)
            {
                if (data.TryGetReadingLock(out locker))
                {
                    return locker;
                }
                Thread.Sleep(Interval);
            }
            throw new DataLockTimeoutException($"Could not establish valid reading lock with ({Attempts.ToString()} attempts, interval length ({Interval.ToString()})", data);
        }

        /// <summary>
        /// Tries to get a full access lock on the model data object until internal timeout occures
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public IDisposable TryGetFullAccessLock(ModelData data)
        {
            IDisposable locker = null;
            for (Int32 i = 0; i < Attempts; i++)
            {
                if (data.TryGetFullLock(out locker))
                {
                    return locker;
                }
                Thread.Sleep(Interval);
            }
            throw new DataLockTimeoutException($"Could not establish valid writing lock with ({Attempts.ToString()} attempts, interval length ({Interval.ToString()})", data);
        }
    }
}
