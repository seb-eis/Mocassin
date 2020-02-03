using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Mocassin.Framework.Provider
{
    /// <inheritdoc />
    [DataContract]
    public class ExternalLoadInfo : IExternalLoadInfo
    {
        /// <inheritdoc />
        public bool IsUndefined => (AssemblyPath == null) | (FullClassName == null) | (MethodName == null);

        /// <inheritdoc />
        [DataMember]
        public string AssemblyPath { get; set; }

        /// <inheritdoc />
        [DataMember]
        public string FullClassName { get; set; }

        /// <inheritdoc />
        [DataMember]
        public string MethodName { get; set; }

        /// <summary>
        ///     Default construct an empty load info
        /// </summary>
        public ExternalLoadInfo()
        {
        }

        /// <summary>
        ///     Construct load information from an load information interface
        /// </summary>
        /// <param name="loadInfo"></param>
        public ExternalLoadInfo(IExternalLoadInfo loadInfo)
        {
            if (loadInfo == null)
                return;

            AssemblyPath = loadInfo.AssemblyPath;
            FullClassName = loadInfo.FullClassName;
            MethodName = loadInfo.MethodName;
        }

        /// <inheritdoc />
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
                    exception = new InvalidOperationException(
                        "The defined provider class does not have a default public constructor or is a value type");
                    return false;
                }

                var method = classType.GetMethod(MethodName, new[] {inputType});
                if (method == null)
                {
                    exception = new InvalidOperationException("The method name with the provided parameter type does not exist");
                    return false;
                }

                if (method.ReturnType.IsAssignableFrom(outputType))
                    return true;

                exception = new InvalidOperationException("The output argument of the provider function has the wrong type");
                return false;
            }
            catch (Exception caughtException)
            {
                Console.WriteLine(caughtException);
                exception = caughtException;
                return false;
            }
        }

        /// <summary>
        ///     Returns a string representing the load information
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"[AssemblyPath = {AssemblyPath}, ClassName = {FullClassName}, MethodName = {MethodName}]";
        }
    }
}