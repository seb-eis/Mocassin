using System;
using Mocassin.Framework.Exceptions;

namespace Mocassin.Model.Basic
{
    /// <summary>
    ///     Exception thrown if a data lock source cannot get a valid read or write lock within its specified await period
    /// </summary>
    public class DataAccessTimeoutException : MocassinException
    {
        /// <summary>
        ///     The data object that was used during the invalid access
        /// </summary>
        public object DataObject { get; }

        /// <summary>
        ///     Creates new data access exception
        /// </summary>
        /// <param name="message"></param>
        /// <param name="dataObject"></param>
        public DataAccessTimeoutException(string message, object dataObject)
            : base(message)
        {
            DataObject = dataObject;
        }

        /// <inheritdoc />
        public override string ToString() => $"Locking attempt was aborted due to a timeout{Environment.NewLine}Details:{Environment.NewLine}{Message}";
    }
}