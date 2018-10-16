using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ICon.Framework.Operations;
using ICon.Framework.Processing;
using ICon.Framework.Reflection;
using ICon.Model.ProjectServices;

namespace ICon.Model.Basic
{
    /// <summary>
    ///     Abstract base class for all implementations of services that perform validations for a specific data port based
    ///     upon a synchronous data processing pipeline
    /// </summary>
    /// <typeparam name="TPort"></typeparam>
    public abstract class ValidationService<TPort> : IValidationService<TPort> 
        where TPort : class, IModelDataPort
    {
        /// <inheritdoc />
        public Type DataPortType { get; } = typeof(TPort);

        /// <summary>
        ///     Link to parent project services
        /// </summary>
        protected IProjectServices ProjectServices { get; set; }

        /// <summary>
        ///     The synchronous validation pipeline for model parameters that cannot
        /// </summary>
        protected BreakPipeline<IValidationReport> ParameterPipeline { get; set; }

        /// <summary>
        ///     The synchronous validation pipeline for model objects that have potential conflicts with existing data
        /// </summary>
        protected BreakPipeline<IValidationReport> ObjectPipeline { get; set; }

        /// <summary>
        ///     Creates new validation service, initializes the validation pipeline with the handlers defined in the implementing
        ///     class
        /// </summary>
        protected ValidationService(IProjectServices projectServices)
        {
            ParameterPipeline = new BreakPipeline<IValidationReport>(MakeCannotValidateProcessor(), MakeParameterValidationProcessors());
            ObjectPipeline = new BreakPipeline<IValidationReport>(MakeCannotValidateProcessor(), MakeObjectValidationProcessors());
            ProjectServices = projectServices ?? throw new ArgumentNullException(nameof(projectServices));
        }

        /// <inheritdoc />
        public IValidationReport ValidateObject<TObject>(TObject obj, IDataReader<TPort> dataReader)
            where TObject : IModelObject
        {
            if (dataReader == null) 
                throw new ArgumentNullException(nameof(dataReader));

            if (obj == null) 
                throw new ArgumentNullException(nameof(obj));

            if (obj.IsDeprecated) 
                throw new ArgumentException("Model object passed to validation is deprecated", nameof(obj));

            return ObjectPipeline.Process(obj, dataReader);
        }

        /// <inheritdoc />
        public IValidationReport ValidateParameter<TParameter>(TParameter obj, IDataReader<TPort> dataReader)
            where TParameter : IModelParameter
        {
            return ParameterPipeline.Process(obj, dataReader);
        }

        /// <summary>
        ///     Get the handler for cases where the processing pipeline is passed a model object it cannot handle
        /// </summary>
        /// <returns></returns>
        protected virtual IObjectProcessor<IValidationReport> MakeCannotValidateProcessor()
        {
            return ObjectProcessorFactory.Create<object, IValidationReport>(obj =>
                throw new InvalidOperationException("Invalid object was passed to validation pipeline"));
        }

        /// <summary>
        ///     Validates that the original model parameter object is not equal in content to the provided model parameter
        ///     interface
        /// </summary>
        /// <param name="original"></param>
        /// <param name="replacement"></param>
        /// <param name="report"></param>
        protected void ValidateModelParameterContentEquality<TParameter>(TParameter original, TParameter replacement,
            ValidationReport report) where TParameter : IModelParameter
        {
            if (!original.Equals(replacement)) 
                return;

            var detail = $"The current model parameter of type {original.GetParameterName()} is equal to the provided replacement";
            report.AddWarning(ModelMessageSource.CreateParameterIdenticalWarning(this, detail));
        }

        /// <summary>
        ///     Validates that the model object is unique in terms of existing data through the IEquatable interface implementation
        ///     of the model object
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="obj"></param>
        /// <param name="existingData"></param>
        /// <param name="report"></param>
        protected void ValidateModelObjectUniqueness<TObject>(TObject obj, IEnumerable<TObject> existingData, ValidationReport report)
            where TObject : IModelObject, IEquatable<TObject>
        {
            foreach (var item in existingData)
            {
                if (!item.Equals(obj)) 
                    continue;

                var detail =
                    $"The provided {obj.GetObjectName()} is a duplicate to the existing model object with index ({item.Index})";
                report.AddWarning(ModelMessageSource.CreateModelDuplicateWarning(this, detail));
            }
        }

        /// <summary>
        ///     Get the list of validation handlers for model objects defined by the implementing validation service
        /// </summary>
        /// <returns></returns>
        protected virtual List<IObjectProcessor<IValidationReport>> MakeObjectValidationProcessors()
        {
            return MakeValidationProcessors(ValidationType.Object).ToList();
        }

        /// <summary>
        ///     Get the list of validation handlers for model parameters defined by the implementing validation service
        /// </summary>
        /// <returns></returns>
        protected virtual List<IObjectProcessor<IValidationReport>> MakeParameterValidationProcessors()
        {
            return MakeValidationProcessors(ValidationType.Parameter).ToList();
        }

        /// <summary>
        ///     Searches the validation service for all non-public members marked as validation functions of the specified type and
        ///     creates a processor sequence
        /// </summary>
        /// <param name="validationType"></param>
        /// <returns></returns>
        protected IEnumerable<IObjectProcessor<IValidationReport>> MakeValidationProcessors(ValidationType validationType)
        {
            bool SearchMethod(MethodInfo methodInfo)
            {
                if (methodInfo.GetCustomAttribute(typeof(ValidationOperationAttribute)) is ValidationOperationAttribute attribute)
                    return attribute.ValidationType == validationType;

                return false;
            }

            return new ObjectProcessorCreator().CreateProcessors<IValidationReport>(this, SearchMethod,
                BindingFlags.Instance | BindingFlags.NonPublic);
        }
    }
}