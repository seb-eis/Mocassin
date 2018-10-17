using System;
using System.Collections.Generic;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;

namespace Mocassin.Model.ModelProject
{
    /// <inheritdoc />
    public class ValidationServiceProvider : IValidationServiceProvider
    {
        /// <summary>
        ///     List of all registered validation services
        /// </summary>
        protected List<IValidationService> ValidationServices { get; set; }

        /// <summary>
        ///     Creates new validation service provider with empty service list
        /// </summary>
        public ValidationServiceProvider()
        {
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
    }
}