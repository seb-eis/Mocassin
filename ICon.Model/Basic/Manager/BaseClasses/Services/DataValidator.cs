using System;
using System.Collections.Generic;
using Mocassin.Framework.Operations;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Basic
{
    /// <summary>
    ///     Generic abstract base class for implementations of validators for a specific data type that need access to project
    ///     service, a settings object and data through a reader object
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    /// <typeparam name="TSetting"></typeparam>
    /// <typeparam name="TDataPort"></typeparam>
    public abstract class DataValidator<TObject, TSetting, TDataPort> : IDataValidator<TObject>
        where TDataPort : class, IModelDataPort
    {
        /// <summary>
        ///     Access to the project services
        /// </summary>
        protected IModelProject ModelProject { get; set; }

        /// <summary>
        ///     Access to the settings object for basic constraints
        /// </summary>
        protected TSetting Settings { get; set; }

        /// <summary>
        ///     Data reader to access the existing data for conflict evaluation
        /// </summary>
        protected IDataReader<TDataPort> DataReader { get; set; }

        /// <summary>
        ///     Creates new data validator that used the provided project services, settings object and data reader
        /// </summary>
        /// <param name="modelProject"></param>
        /// <param name="settings"></param>
        /// <param name="dataReader"></param>
        protected DataValidator(IModelProject modelProject, TSetting settings, IDataReader<TDataPort> dataReader)
        {
            ModelProject = modelProject ?? throw new ArgumentNullException(nameof(modelProject));
            Settings = settings;
            DataReader = dataReader ?? throw new ArgumentNullException(nameof(dataReader));
        }

        /// <inheritdoc />
        public abstract IValidationReport Validate(TObject obj);

        /// <summary>
        ///     Performs a content equality check for model parameters that implement the equatable interface and adds a general
        ///     result to the validation report
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="report"></param>
        protected void AddGenericContentEqualityValidation<T1>(T1 first, T1 second, ValidationReport report) where T1 : IModelParameter
        {
            if (!first.Equals(second)) 
                return;

            var detail = $"The current model parameter of type {first.GetParameterName()} is equal to the provided replacement";
            report.AddWarning(ModelMessageSource.CreateParameterIdenticalWarning(this, detail));
        }

        /// <summary>
        ///     Validates that the model object is unique in terms of existing data through the IEquatable interface implementation
        ///     of the model object and adds the results to the validation report
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="obj"></param>
        /// <param name="existingData"></param>
        /// <param name="report"></param>
        protected void AddGenericObjectDuplicateValidation<T1>(T1 obj, IEnumerable<T1> existingData, ValidationReport report)
            where T1 : IModelObject, IEquatable<T1>
        {
            foreach (var item in existingData)
            {
                if (!item.Equals(obj)) 
                    continue;

                var detail = $"The provided {obj} is a duplicate to the existing object ({item.Index}) {item}";
                report.AddWarning(ModelMessageSource.CreateModelDuplicateWarning(this, detail));
            }
        }
    }
}