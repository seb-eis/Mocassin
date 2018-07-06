using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Simulations
{
    /// <summary>
    /// Represents a string based provider assembly load info that defines DLL load information to use outside provider systems
    /// </summary>
    public interface IProviderAssemblyLoadInfo
    {
        /// <summary>
        /// The path to the DLL that should be loaded for provision
        /// </summary>
        string DllFilePath { get; }

        /// <summary>
        /// The name of the provider class that will be created for provision
        /// </summary>
        string ProviderClassName { get; }

        /// <summary>
        /// The name of the method on the provider that creates the required information
        /// </summary>
        string ProviderMethodName { get; }
    }
}
