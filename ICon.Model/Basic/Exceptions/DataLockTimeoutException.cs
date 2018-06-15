using System;
using System.Collections.Generic;
using System.Text;

using ICon.Framework.Exceptions;

namespace ICon.Model.Basic
{
    /// <summary>
    /// Exception thrown if a data locker cannot get a valid read or write lock within its specified await period
    /// </summary>
    public class DataLockTimeoutException : IConCustomException
    {
        /// <summary>
        /// The data object that was used during the invalid access
        /// </summary>
        public Object DataObject { get; }

        /// <summary>
        /// Creates new data access exception
        /// </summary>
        /// <param name="index"></param>
        public DataLockTimeoutException(String message, Object dataObject) : base(message)
        {
            DataObject = dataObject;
        }

        /// <summary>
        /// Creates info string from exception, overrides obejcts ToString()
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return $"Locking attempt was aborted due to a timeout{Environment.NewLine}Details:{Environment.NewLine}{Message}";
        }
    }
}
