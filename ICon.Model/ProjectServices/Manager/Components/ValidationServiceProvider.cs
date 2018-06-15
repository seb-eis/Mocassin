using System;
using System.Collections.Generic;
using System.Text;

using ICon.Framework.Operations;
using ICon.Model.Basic;

namespace ICon.Model.ProjectServices
{
    /// <summary>
    /// Provider for multiple validation services for model objects and parameters
    /// </summary>
    public class ValidationServiceProvider : IValidationServiceProvider
    {
        /// <summary>
        /// List of all registered validation services
        /// </summary>
        protected List<IValidationService> ValidationServices { get; set; }

        /// <summary>
        /// Creates new validation service provider with empty service list
        /// </summary>
        public ValidationServiceProvider()
        {
            ValidationServices = new List<IValidationService>();
        }

        /// <summary>
        /// Forwards object validation request to the correct registered validation service (Throws exception if a matching service is not registered)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="TPort"></typeparam>
        /// <param name="obj"></param>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        public IValidationReport ValidateObject<T1, TPort>(T1 obj, IDataReader<TPort> dataReader) where T1 : IModelObject where TPort : class, IModelDataPort
        {
            foreach (var item in ValidationServices)
            {
                if (item is IValidationService<TPort> service)
                {
                    return service.ValidateObject(obj, dataReader);
                }
            }
            throw new InvalidOperationException("No matching validation service found");
        }

        /// <summary>
        /// Forwards parameter validation request to the correct registered validation service (Throws exception if a matching service is not registered)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="TPort"></typeparam>
        /// <param name="obj"></param>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        public IValidationReport ValidateParameter<T1, TPort>(T1 obj, IDataReader<TPort> dataReader) where T1 : IModelParameter where TPort : class, IModelDataPort
        {
            foreach (var item in ValidationServices)
            {
                if (item is IValidationService<TPort> service)
                {
                    return service.ValidateParameter(obj, dataReader);
                }
            }
            throw new InvalidOperationException("No matching validation service found");
        }

        /// <summary>
        /// Registers a new service to the service provider (Overwrites exististing service of same type)
        /// </summary>
        /// <param name="service"></param>
        public void RegisterService(IValidationService service)
        {
            for (Int32 i = 0; i < ValidationServices.Count; i++)
            {
                if (ValidationServices[i].DataPortType == service.DataPortType)
                {
                    ValidationServices[i] = service;
                    return;
                }
            }
            ValidationServices.Add(service);
        }
    }
}
