using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mocassin.Framework.Extensions;

namespace Mocassin.Framework.Messaging
{
    /// <summary>
    /// Exception message class that carries error information including the thrown exceptions (Only to be used when program errors occure)
    /// </summary>
    public class ErrorMessage : PushMessage
    {
        /// <summary>
        /// The caught exception
        /// </summary>
        public Exception Exception { get; set; }

        /// <inheritdoc />
        public override IEnumerable<string> DetailSequence => EnumerateException();

        /// <inheritdoc />
        public ErrorMessage(object sender, string shortInfo) : base(sender, shortInfo)
        {
        }

        /// <summary>
        ///     Enumerates the contents of the set <see cref="Exception"/> into a sequence of <see cref="string"/> values
        /// </summary>
        /// <returns></returns>
        private IEnumerable<string> EnumerateException()
        {
            if (Exception == null) yield break;
            yield return Exception.ToString();

            var innerException = Exception.InnerException;
            while (innerException != null)
            {
                yield return innerException.ToString();
                innerException = innerException.InnerException;
            }
        }
    }
}
