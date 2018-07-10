using System;

namespace ICon.Framework.Provider
{
    /// <summary>
    /// Represents a load information for externally defined data provider functions
    /// </summary>
    public interface IExternalLoadInfo
    {
        /// <summary>
        /// The path of the assembly that should be loaded
        /// </summary>
        string AssemblyPath { get; }

        /// <summary>
        /// The full name of the class the function is defined in
        /// </summary>
        string FullClassName { get; }

        /// <summary>
        /// The name of the method used for data provision
        /// </summary>
        string MethodName { get; }

        /// <summary>
        /// Check if the load info actually describes a valid method with the described input type and output type. Provides caught exceptions
        /// if the method returns false
        /// </summary>
        /// <param name="inputType"></param>
        /// <param name="outputType"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        bool IsValidProviderFor(Type inputType, Type outputType, out Exception exception);
    }
}