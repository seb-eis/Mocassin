using System;
using ICon.Framework.Operations;

namespace ICon.Model.Basic
{
    /// <summary>
    ///     Represents a validation service for model parameters and objects
    /// </summary>
    public interface IValidationService
    {
        /// <summary>
        ///     Get the data port type this validation service uses for conflict determination with existing data
        /// </summary>
        /// <returns></returns>
        Type DataPortType { get; }
    }

    /// <summary>
    ///     Represents a validation service for a specific data port that handles model object validation requests
    /// </summary>
    /// <typeparam name="TDataPort"></typeparam>
    public interface IValidationService<in TDataPort> : IValidationService
        where TDataPort : class, IModelDataPort
    {
        /// <summary>
        ///     Validates model object in terms of conflicts with general limitations and potential conflicts with existing data
        ///     accessible by the data reader
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="obj"></param>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        IValidationReport ValidateObject<TObject>(TObject obj, IDataReader<TDataPort> dataReader) 
            where TObject : IModelObject;

        /// <summary>
        ///     Validates a model parameters in terms of conflicts with general limitations or dependent existing parameters
        /// </summary>
        /// <typeparam name="TParameter"></typeparam>
        /// <param name="obj"></param>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        IValidationReport ValidateParameter<TParameter>(TParameter obj, IDataReader<TDataPort> dataReader)
            where TParameter : IModelParameter;
    }
}