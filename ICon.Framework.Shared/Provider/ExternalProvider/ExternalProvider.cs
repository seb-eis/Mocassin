using System;
using System.Collections.Generic;
using System.Text;
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
        public (string DllPath, string Class, string Method) LoadInfo { get; set; }

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
        /// Tries to load the provider into the assembly and create a provider instance with the passed construction arguments.
        /// Returns false on fail and provides catched exceptions
        /// </summary>
        /// <param name="exceptions"></param>
        /// <param name="constructionArgs"></param>
        /// <returns></returns>
        public bool TryLoadProvider(out Exception exception, params object[] constructionArgs)
        {
            exception = null;
            if (!IsLoaded)
            {
                try
                {
                    var assembly = Assembly.LoadFrom(LoadInfo.DllPath);
                    ProviderInstance = Activator.CreateInstance(assembly.GetType(LoadInfo.Class), constructionArgs);
                    var method = ProviderInstance.GetType().GetMethod(LoadInfo.Method, new Type[] { typeof(TIn) });
                    ProviderDelegate = (Func<TIn, TOut>)method.CreateDelegate(typeof(Func<TIn, TOut>), ProviderInstance);
                    if (ProviderDelegate == null)
                    {
                        throw new InvalidOperationException("Could not create delegate instance");
                    }
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
