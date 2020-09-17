using System.Threading.Tasks;

namespace Mocassin.Framework.Processing
{
    /// <summary>
    ///     Represents a handler for objects that processes passed values in a new task and returns the processing task if the
    ///     object could be processed
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface IAsyncObjectProcessor<TResult> : IObjectProcessor<Task<TResult>>
    {
    }
}