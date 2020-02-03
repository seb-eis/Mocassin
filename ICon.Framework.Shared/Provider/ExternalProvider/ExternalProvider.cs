using System;
using System.Reflection;

namespace Mocassin.Framework.Provider
{
    /// <summary>
    ///     Provider class for provider systems defined in external DLLs that handles assembly loading and provider delegate
    ///     creation
    /// </summary>
    /// <typeparam name="TOut"></typeparam>
    /// <typeparam name="TIn"></typeparam>
    /// <remarks> No void specific implementation exists, use null reference as input for this case </remarks>
    public class ExternalProvider<TOut, TIn> : IExternalProvider<TOut, TIn>
    {
        public bool IsLoaded { get; protected set; }

        /// <summary>
        ///     The class instance of the provider
        /// </summary>
        protected object ProviderInstance { get; set; }

        /// <inheritdoc />
        public Func<TIn, TOut> ProviderDelegate { get; protected set; }

        /// <summary>
        ///     Defines the dll load information by dll path, provider class and method name on the provider
        /// </summary>
        public ExternalLoadInfo LoadInfo { get; set; }

        /// <inheritdoc />
        Delegate IExternalProvider.ProviderDelegate => ProviderDelegate;

        /// <inheritdoc />
        public TOut GetValue(TIn inputObject)
        {
            if (IsLoaded)
                return ProviderDelegate(inputObject);

            if (!TryLoadProvider(out var exception))
                throw new InvalidOperationException("Dynamic loading of provider failed with an exception", exception);

            return ProviderDelegate(inputObject);
        }

        /// <inheritdoc />
        object IExternalProvider.GetValue(object inputObject)
        {
            return GetValue((TIn) inputObject);
        }

        /// <inheritdoc />
        public bool TryLoadProvider(out Exception exception, params object[] constArgs)
        {
            exception = null;
            if (IsLoaded)
                return IsLoaded;

            try
            {
                var assembly = Assembly.LoadFrom(LoadInfo.AssemblyPath);
                ProviderInstance = Activator.CreateInstance(assembly.GetType(LoadInfo.FullClassName), constArgs);
                var method = ProviderInstance
                    .GetType()
                    .GetMethod(LoadInfo.MethodName, new[] {typeof(TIn)});

                ProviderDelegate = (Func<TIn, TOut>) method?.CreateDelegate(typeof(Func<TIn, TOut>), ProviderInstance)
                                   ?? throw new InvalidOperationException("Method lookup returned null");

                IsLoaded = true;
            }
            catch (Exception localException)
            {
                Console.WriteLine(localException);
                exception = localException;
                IsLoaded = false;
            }

            return IsLoaded;
        }
    }
}