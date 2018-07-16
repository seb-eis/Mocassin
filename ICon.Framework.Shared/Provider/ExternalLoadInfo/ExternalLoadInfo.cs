using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Reflection;

namespace ICon.Framework.Provider
{
    /// <summary>
    /// Defines a string based provider assembly load info that enables an outside dll to be used as a source for specific values
    /// </summary>
    [DataContract]
    public class ExternalLoadInfo : IExternalLoadInfo
    {
        /// <summary>
        /// The path to the DLL that should be loaded for provision
        /// </summary>
        [DataMember]
        public string AssemblyPath { get; set; }

        /// <summary>
        /// The name of the provider calss that will be created for provision
        /// </summary>
        [DataMember]
        public string FullClassName { get; set; }

        /// <summary>
        /// The name of the method on the provider that should be used to create a provder delegate
        /// </summary>
        [DataMember]
        public string MethodName { get; set; }

        /// <summary>
        /// Default construct an empty load info
        /// </summary>
        public ExternalLoadInfo()
        {
        }

        /// <summary>
        /// Construct load information from an load information interface
        /// </summary>
        /// <param name="loadInfo"></param>
        public ExternalLoadInfo(IExternalLoadInfo loadInfo)
        {
            if (loadInfo == null)
            {
                throw new ArgumentNullException(nameof(loadInfo));
            }
            AssemblyPath = loadInfo.AssemblyPath;
            FullClassName = loadInfo.FullClassName;
            MethodName = loadInfo.MethodName;
        }

        /// <summary>
        /// Checks if the external load info can be used to create an external provider that uses the specififed input and output type.
        /// Provides caught exceptions as an out parameter
        /// </summary>
        /// <param name="inputType"></param>
        /// <param name="outputType"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public bool IsValidProviderFor(Type inputType, Type outputType, out Exception exception)
        {
            exception = null;
            try
            {
                var assembly = Assembly.LoadFrom(AssemblyPath);
                var classType = assembly.GetType(FullClassName);
                if (classType == null)
                {
                    exception = new InvalidOperationException("The defined class name does not refer to a valid public type");
                    return false;
                }
                if (classType.IsValueType || classType.GetConstructor(Type.EmptyTypes) == null)
                {
                    exception = new InvalidOperationException("The defined provider class does not have a default public constructor or is a value type");
                    return false;                 
                }

                var method = classType.GetMethod(MethodName, new Type[] { inputType });
                if (method == null)
                {
                    exception = new InvalidOperationException("The method name with the provided parameter type does not exist");
                    return false;
                }
                if (!method.ReturnType.IsAssignableFrom(outputType))
                {
                    exception = new InvalidOperationException("The output argument of the provider function has the wrong type");
                    return false;
                }
                return true;
            }
            catch (Exception caught)
            {
                exception = caught;
                return false;
            }
        }

        /// <summary>
        /// Returns a string representing the load information
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"[AssemblyPath = {AssemblyPath}, ClassName = {FullClassName}, MethodName = {MethodName}]";
        }
    }
}
