using System;
using System.Collections.Generic;
using ICon.Framework.Operations;
using ICon.Model.ProjectServices;

namespace ICon.Model.Basic
{
    /// <summary>
    /// GEneric abstract base class for implementations of validators for a specific data type that need access to project service, a settings object and data through a reader object
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    /// <typeparam name="TSetting"></typeparam>
    /// <typeparam name="TDataPort"></typeparam>
    public abstract class DataValidator<TObject, TSetting, TDataPort> : IDataValidator<TObject> where TDataPort : class, IModelDataPort
    {
        /// <summary>
        /// Access to the project services
        /// </summary>
        protected IProjectServices ProjectServices { get; set; }

        /// <summary>
        /// Access to the settings object for basic constraints
        /// </summary>
        protected TSetting Settings { get; set; }

        /// <summary>
        /// Data reader to access the existing data for conflict evaluation
        /// </summary>
        protected IDataReader<TDataPort> DataReader { get; set; }

        /// <summary>
        /// Creates new data validator that used the provided project services, settings object and data reader
        /// </summary>
        /// <param name="projectServices"></param>
        /// <param name="settings"></param>
        /// <param name="dataReader"></param>
        protected DataValidator(IProjectServices projectServices, TSetting settings, IDataReader<TDataPort> dataReader)
        {
            ProjectServices = projectServices ?? throw new ArgumentNullException(nameof(projectServices));
            Settings = settings;
            DataReader = dataReader ?? throw new ArgumentNullException(nameof(dataReader));
        }

        /// <summary>
        /// Performs the object validation and creates a validation report that contains the validation result
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public abstract IValidationReport Validate(TObject obj);

        /// <summary>
        /// Performs a content equality check for model parameters that implement the equatable interface and adds a general result to the validation report
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="report"></param>
        protected void AddGenericContentEqualityValidation<T1>(T1 first, T1 second, ValidationReport report) where T1 : IModelParameter
        {
            if (first.Equals(second))
            {
                var detail = $"The current model parameter of type {first.GetParameterName()} is equal to the provided replacement";
                report.AddWarning(ModelMessages.CreateParameterIdenticalWarning(this, detail));
            }
        }

        /// <summary>
        /// Validates that the model object is unique in terms of existing data through the IEquatable interface implementation of the model oject and adds the results to the validation report
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="obj"></param>
        /// <param name="existingData"></param>
        /// <param name="report"></param>
        protected void AddGenericObjectDuplicateValidation<T1>(T1 obj, IEnumerable<T1> existingData, ValidationReport report) where T1 : IModelObject, IEquatable<T1>
        {
            foreach (var item in existingData)
            {
                if (item.Equals(obj))
                {
                    var detail = $"The provided {obj.GetObjectName()} is a duplicate to the existing model object with index ({item.Index})";
                    report.AddWarning(ModelMessages.CreateModelDuplicateWarning(this, detail));
                }
            }
        }
    }
}
