using System;
using System.Collections.Generic;
using System.Text;

using ICon.Framework.Operations;
using ICon.Model.Basic;

namespace ICon.Model.ProjectServices
{
    public interface IValidationServiceProvider
    {
        /// <summary>
        /// Validates if a model object does not violate basic restrictions or has conflicts with existing data
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="TPort"></typeparam>
        /// <param name="obj"></param>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        IValidationReport ValidateObject<T1, TPort>(T1 obj, IDataReader<TPort> dataReader) where T1 : IModelObject where TPort : class, IModelDataPort;

        /// <summary>
        /// Validates if a model parameter does not violate basic restrictions or has conflicts with existing data
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="TPort"></typeparam>
        /// <param name="obj"></param>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        IValidationReport ValidateParameter<T1, TPort>(T1 obj, IDataReader<TPort> dataReader) where T1 : IModelParameter where TPort : class, IModelDataPort;
    }
}
