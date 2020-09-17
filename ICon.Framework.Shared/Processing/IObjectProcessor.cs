namespace Mocassin.Framework.Processing
{
    /// <summary>
    ///     Represents an handler for objects that processes passed values and returns the result if the object could be
    ///     processed
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface IObjectProcessor<out TResult>
    {
        /// <summary>
        ///     Checks if the handler can process the provided set of objects
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        bool CanProcess(object obj, params object[] args);

        /// <summary>
        ///     Handles the passed set of objects, behavior on failure is specified by the implementation
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        TResult Process(object obj, params object[] args);
    }
}