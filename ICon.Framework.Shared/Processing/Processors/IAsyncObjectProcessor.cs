using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ICon.Framework.Processing
{
    /// <summary>
    /// Represents a handler for objects that processes passed values in a new task and returns the processing task if the object could be processed
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface IAsyncObjectProcessor<TResult> : IObjectProcessor<Task<TResult>>
    {
    }
}
