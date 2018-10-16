namespace ICon.Model.Basic
{
    /// <summary>
    ///     Model object event arguments interface that allows the covariant usage of the event arguments with non-interface
    ///     types
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public interface IModelObjectEventArgs<out T1> where T1 : IModelObject
    {
        /// <summary>
        ///     Interface access to the object that was changed or added
        /// </summary>
        T1 ModelObject { get; }
    }
}