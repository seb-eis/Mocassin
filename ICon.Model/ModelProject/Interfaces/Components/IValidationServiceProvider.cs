using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;

namespace Mocassin.Model.ModelProject
{
    /// <summary>
    ///     Represents a service provider for validations that supports object and parameter validations
    /// </summary>
    public interface IValidationServiceProvider
    {
        /// <summary>
        ///     Validates the passed <see cref="IModelObject" /> in the context of the provided <see cref="IDataReader{TPort}" />
        ///     and generates the affiliated <see cref="IValidationReport" />
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="TPort"></typeparam>
        /// <param name="obj"></param>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        IValidationReport ValidateObject<T1, TPort>(T1 obj, IDataReader<TPort> dataReader)
            where T1 : IModelObject
            where TPort : class, IModelDataPort;

        /// <summary>
        ///     Validates the passed <see cref="IModelParameter" /> in the context of the provided
        ///     <see cref="IDataReader{TPort}" /> and generates the affiliated <see cref="IValidationReport" />
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="TPort"></typeparam>
        /// <param name="obj"></param>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        IValidationReport ValidateParameter<T1, TPort>(T1 obj, IDataReader<TPort> dataReader)
            where T1 : IModelParameter
            where TPort : class, IModelDataPort;

        /// <summary>
        ///     Validates the passed <see cref="IModelObject" /> and generates the affiliated <see cref="IValidationReport" />
        /// </summary>
        /// <param name="modelObject"></param>
        /// <returns></returns>
        /// <remarks> Required <see cref="IDataReader{TPort}" /> has to be looked up internally </remarks>
        IValidationReport ValidateObject<T>(T modelObject) where T : IModelObject;

        /// <summary>
        ///     Validates the passed <see cref="IModelParameter" /> and generates the affiliated <see cref="IValidationReport" />
        /// </summary>
        /// <param name="modelParameter"></param>
        /// <returns></returns>
        /// <remarks> Required <see cref="IDataReader{TPort}" /> has to be looked up internally </remarks>
        IValidationReport ValidateParameter<T>(T modelParameter) where T : IModelParameter;
    }
}