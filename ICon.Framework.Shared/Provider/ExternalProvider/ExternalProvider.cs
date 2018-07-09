using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

namespace ICon.Framework.Provider
{
    /// <summary>
    /// Provider class for provider systems defined in external DLLs that handles assembly loading and provider delegate creation
    /// </summary>
    /// <typeparam name="TOut"></typeparam>
    /// <typeparam name="TIn"></typeparam>
    /// <remarks> No void specific implementation exists, use null refernce as input for this case </remarks>
    public class ExternalProvider<TOut, TIn> : IExternalProvider<TOut, TIn>
    {
        /// <summary>
        /// Boolean flag if the provider system is loaded and can be used
        /// </summary>
        public bool IsLoaded { get; protected set; }

        /// <summary>
        /// The class instance of the provider
        /// </summary>
        protected object ProviderInstance { get; set; }

        /// <summary>
        /// The provider delegate used for object retrival
        /// </summary>
        public Func<TIn, TOut> ProviderDelegate { get; protected set; }

        /// <summary>
        /// Defines the dll load information by dll path, provider class and method name on the provider
        /// </summary>
        public ExternalLoadInfo LoadInfo { get; set; }

        /// <summary>
        /// Non generic interface access to the provider delegate
        /// </summary>
        Delegate IExternalProvider.ProviderDelegate => ProviderDelegate;

        /// <summary>
        /// Accepts an input object and retrives the output object affiliated with it from the external provider
        /// </summary>
        /// <param name="inputObject"></param>
        /// <returns></returns>
        public TOut GetValue(TIn inputObject)
        {
            if (!IsLoaded)
            {
                if (!TryLoadProvider(out var exception))
                {
                    throw new InvalidOperationException("Dynamic loading of provider failed with an exception", exception);
                }
            }
            return ProviderDelegate(inputObject);
        }

        /// <summary>
        /// Non generic get value overload for the non generic external provider interface
        /// </summary>
        /// <param name="inputObject"></param>
        /// <returns></returns>
        object IExternalProvider.GetValue(object inputObject)
        {
            return GetValue((TIn)inputObject);
        }

        /// <summary>
        /// Tries to load the provider into the assembly and create a provider instance with the passed construction arguments.
        /// Returns false on fail and provides catched exceptions
        /// </summary>
        /// <param name="exceptions"></param>
        /// <param name="constArgs"></param>
        /// <returns></returns>
        public bool TryLoadProvider(out Exception exception, params object[] constArgs)
        {
            exception = null;
            if (!IsLoaded)
            {
                try
                {
                    var assembly = Assembly.LoadFrom(LoadInfo.AssemblyPath);
                    ProviderInstance = Activator.CreateInstance(assembly.GetType(LoadInfo.FullClassName), constArgs);
                    var method = ProviderInstance.GetType().GetMethod(LoadInfo.MethodName, new Type[] { typeof(TIn) });
                    ProviderDelegate = (Func<TIn, TOut>)method.CreateDelegate(typeof(Func<TIn, TOut>), ProviderInstance);
                    IsLoaded = true;
                }
                catch (Exception excep)
                {
                    exception = excep;
                    IsLoaded = false;
                }
            }
            return IsLoaded;
        }
    }
}
