using System;
using System.Collections.Generic;
using System.Text;
using ICon.Framework.Messaging;

namespace ICon.Framework.Provider
{
    /// <summary>
    /// Non generic external provider that
    /// </summary>
    public interface IExternalProvider
    {
        /// <summary>
        /// Get the delegate that the provider uses for object provision
        /// </summary>
        Delegate ProviderDelegate { get; }

        /// <summary>
        /// Boolean flag that indicates if the provider system is loaded and ready for use
        /// </summary>
        bool IsLoaded { get; }

        /// <summary>
        /// Tries to load the provider system into the assembly and create in instance with the passed construction arguments.
        /// Returns false if loading fails and sets catched exception to out parameter
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="constArgs"></param>
        /// <returns></returns>
        bool TryLoadProvider(out Exception exception, params object[] constArgs);

        /// <summary>
        /// Calls the provider with an input object to get an output object
        /// </summary>
        /// <param name="inputObject"></param>
        /// <returns></returns>
        object GetValue(object inputObject);
    }

    /// <summary>
    /// Represents an external provider for data that takes an input object and returns the matching result
    /// </summary>
    /// <typeparam name="TOut"></typeparam>
    /// <typeparam name="TIn"></typeparam>
    public interface IExternalProvider<out TOut, TIn> : IExternalProvider
    {
        /// <summary>
        /// Get the delegate that the provider uses for object provision
        /// </summary>
        new Func<TIn, TOut> ProviderDelegate { get; }

        /// <summary>
        /// Calls the provider with an input object to get an output object
        /// </summary>
        /// <param name="inputObject"></param>
        /// <returns></returns>
        TOut GetValue(TIn inputObject);
    }
}
