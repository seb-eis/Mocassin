namespace ICon.Model.Basic
{
    /// <summary>
    /// Model parameter event arguments interface that allows the covariant usage of the event arguments with non-interface types
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    interface IModelParameterEventArgs<T1> where T1 : IModelParameter
    {
        /// <summary>
        /// Access to the parameter that was changed
        /// </summary>
        T1 ModelParameter { get; }
    }
}
