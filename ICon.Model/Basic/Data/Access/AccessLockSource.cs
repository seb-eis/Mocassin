using System;
using System.Threading;

namespace Mocassin.Model.Basic
{
    /// <summary>
    ///     Access lock source for model data that tries to access the data a specified number of times within certain
    ///     intervals
    ///     before throwing an exception
    /// </summary>
    public class AccessLockSource
    {
        /// <summary>
        ///     The number of attempts for locking
        /// </summary>
        public int Attempts { get; set; }

        /// <summary>
        ///     The interval in between attempts
        /// </summary>
        public TimeSpan Interval { get; set; }

        /// <summary>
        ///     Creates new access lock source from interval length and attempt counter
        /// </summary>
        /// <param name="attempts"></param>
        /// <param name="interval"></param>
        public AccessLockSource(int attempts, TimeSpan interval)
        {
            Attempts = attempts;
            Interval = interval;
        }

        /// <summary>
        ///     Tries to get a read only lock on the model data object until internal timeout occurs
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public IDisposable TryGetReadAccess(ModelData data)
        {
            for (var i = 0; i < Attempts; i++)
            {
                if (data.TryGetReadingLock(out var locker))
                    return locker;

                Thread.Sleep(Interval);
            }

            throw new DataAccessTimeoutException(
                $"Could not establish valid reading lock with ({Attempts} attempts, interval length ({Interval})", data);
        }

        /// <summary>
        ///     Tries to get a full access lock on the model data object until internal timeout occurs
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public IDisposable TryGetFullAccess(ModelData data)
        {
            for (var i = 0; i < Attempts; i++)
            {
                if (data.TryGetFullLock(out var locker))
                    return locker;

                Thread.Sleep(Interval);
            }

            throw new DataAccessTimeoutException(
                $"Could not establish valid writing lock with ({Attempts} attempts, interval length ({Interval})", data);
        }
    }
}