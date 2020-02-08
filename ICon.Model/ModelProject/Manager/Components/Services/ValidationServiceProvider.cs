using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;

namespace Mocassin.Model.ModelProject
{
    /// <inheritdoc />
    public class ValidationServiceProvider : IValidationServiceProvider
    {
        /// <summary>
        ///     Get the <see cref="Dictionary{TKey,TValue}" /> for caching of validation <see cref="Func{TResult}" /> delegates
        /// </summary>
        private Dictionary<Type, Func<object, IValidationReport>> ValidationDelegateDictionary { get; }

        /// <summary>
        ///     Get the <see cref="IModelProject" /> the <see cref="ValidationServiceProvider" /> is affiliated with
        /// </summary>
        private IModelProject ModelProject { get; }

        /// <summary>
        ///     List of all registered validation services
        /// </summary>
        protected List<IValidationService> ValidationServices { get; set; }

        /// <summary>
        ///     Creates new <see cref="ValidationServiceProvider" /> that uses the passed <see cref="IModelProject" />
        /// </summary>
        /// <param name="modelProject"></param>
        public ValidationServiceProvider(IModelProject modelProject)
        {
            ValidationDelegateDictionary = new Dictionary<Type, Func<object, IValidationReport>>();
            ModelProject = modelProject ?? throw new ArgumentNullException(nameof(modelProject));
            ValidationServices = new List<IValidationService>();
        }

        /// <inheritdoc />
        public IValidationReport ValidateObject<T1, TPort>(T1 obj, IDataReader<TPort> dataReader)
            where T1 : IModelObject where TPort : class, IModelDataPort
        {
            foreach (var item in ValidationServices)
            {
                if (item is IValidationService<TPort> service)
                    return service.ValidateObject(obj, dataReader);
            }

            throw new InvalidOperationException("No matching validation service found");
        }

        /// <inheritdoc />
        public IValidationReport ValidateParameter<T1, TPort>(T1 obj, IDataReader<TPort> dataReader)
            where T1 : IModelParameter where TPort : class, IModelDataPort
        {
            foreach (var item in ValidationServices)
            {
                if (item is IValidationService<TPort> service)
                    return service.ValidateParameter(obj, dataReader);
            }

            throw new InvalidOperationException("No matching validation service found");
        }

        /// <inheritdoc />
        public IValidationReport ValidateObject<T>(T modelObject) where T : IModelObject
        {
            if (typeof(T) == typeof(IModelObject))
                throw new InvalidOperationException("Unspecified interface lookup is not supported");

            return Validate(modelObject);
        }

        /// <inheritdoc />
        public IValidationReport ValidateParameter<T>(T modelParameter) where T : IModelParameter
        {
            if (typeof(T) == typeof(IModelParameter))
                throw new InvalidOperationException("Unspecified interface lookup is not supported");

            return Validate(modelParameter);
        }

        /// <summary>
        ///     Registers a new service to the service provider (Overwrites existing service of same type)
        /// </summary>
        /// <param name="service"></param>
        public void RegisterService(IValidationService service)
        {
            for (var i = 0; i < ValidationServices.Count; i++)
            {
                if (ValidationServices[i].DataPortType != service.DataPortType)
                    continue;

                ValidationServices[i] = service;
                return;
            }

            ValidationServices.Add(service);
        }

        /// <summary>
        ///     Validates the passed object of type <see cref="T" /> if a matching <see cref="IValidationService" /> is available
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected IValidationReport Validate<T>(T obj)
        {
            return GetValidationDelegate(obj)(obj);
        }

        /// <summary>
        ///     Get a generic validation <see cref="Func{T1, TResult}" /> delegate for the passed object of type <see cref="T" />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected Func<object, IValidationReport> GetValidationDelegate<T>(T obj)
        {
            if (ValidationDelegateDictionary.TryGetValue(typeof(T), out var func))
                return func;

            var reader = GetMatchingDataReader(obj)
                         ?? throw new InvalidOperationException($"[{obj.GetType()}] has no matching model manager");

            var service = ValidationServices.FirstOrDefault(x => x.CanValidate(obj, reader))
                          ?? throw new InvalidOperationException($"[{obj.GetType()}] has no matching validation service");

            func = a =>
            {
                service.TryValidate(a, reader, out var report);
                return report;
            };

            ValidationDelegateDictionary[typeof(T)] = func;
            return func;
        }

        /// <summary>
        ///     Get the correct <see cref="IDataReader{TPort}" /> of the <see cref="IModelManager" /> that manages the provided
        ///     object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected IDataReader<IModelDataPort> GetMatchingDataReader(object obj)
        {
            var results = from modelManager in ModelProject.GetAllManagers()
                where modelManager.InputPort.GetSupportedModelTypes().Any(x => x.IsInstanceOfType(obj))
                select modelManager.InputPort.GetDataReader();

            return results.FirstOrDefault();
        }
    }
}