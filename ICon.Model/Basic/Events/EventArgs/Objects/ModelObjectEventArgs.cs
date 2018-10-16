using System;

namespace ICon.Model.Basic
{
    /// <summary>
    ///     Abstract base class for model object event args
    /// </summary>
    public abstract class ModelObjectEventArgs : EventArgs
    {
        /// <summary>
        ///     Creates new model object event argument of the specified type
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="modelObject"></param>
        /// <returns></returns>
        public static ModelObjectEventArgs<T1> Create<T1>(T1 modelObject)
            where T1 : IModelObject
        {
            return new ModelObjectEventArgs<T1>(modelObject);
        }
    }

    /// <summary>
    ///     Event arguments for data object events that inform about changes on a specific model object
    /// </summary>
    public class ModelObjectEventArgs<T1> : ModelObjectEventArgs, IModelObjectEventArgs<T1>
        where T1 : IModelObject
    {
        /// <inheritdoc />
        public T1 ModelObject { get; }

        /// <summary>
        ///     Create new model object event arguments with the provided model object interface
        /// </summary>
        /// <param name="modelObject"></param>
        public ModelObjectEventArgs(T1 modelObject)
        {
            ModelObject = modelObject;
        }
    }
}