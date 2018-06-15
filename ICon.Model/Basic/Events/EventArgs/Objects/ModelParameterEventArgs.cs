using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Basic
{
    /// <summary>
    /// Abstract base class for model parameter event args
    /// </summary>
    public abstract class ModelParameterEventArgs : EventArgs
    {
        /// <summary>
        /// Factory method to create new parameter events of the specfified type
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="modelParameter"></param>
        /// <returns></returns>
        public static ModelParameterEventArgs<T1> Create<T1>(T1 modelParameter) where T1 : IModelParameter
        {
            return new ModelParameterEventArgs<T1>(modelParameter);
        }
    }

    /// <summary>
    /// Event arguments for data events that inform about changes on a specfifc model parameters
    /// </summary>
    public class ModelParameterEventArgs<T1> : ModelParameterEventArgs, IModelParameterEventArgs<T1> where T1 : IModelParameter
    {
        /// <summary>
        /// Interface access to the parameter that was changed
        /// </summary>
        public T1 ModelParameter { get; }

        /// <summary>
        /// Create new model object event arguments with the provided model parameter interface
        /// </summary>
        /// <param name="modelParameter"></param>
        public ModelParameterEventArgs(T1 modelParameter)
        {
            ModelParameter = modelParameter;
        }
    }
}
