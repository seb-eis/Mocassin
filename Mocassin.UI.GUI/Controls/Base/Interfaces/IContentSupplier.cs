namespace Mocassin.UI.GUI.Controls.Base.Interfaces
{
    /// <summary>
    ///     Provides an interface for sub UI elements that supply selection dependent content of a managing view model
    /// </summary>
    public interface IContentSupplier
    {
        /// <summary>
        ///     Notify the receiver that a selection has been changed to the passed object
        /// </summary>
        /// <param name="contentSource"></param>
        void ChangeContentSource(object contentSource);
    }

    /// <summary>
    ///     Provides a typed interface for sub UI elements that supply selection dependent content of a managing view model
    /// </summary>
    public interface IContentSupplier<T> : IContentSupplier
    {
        /// <summary>
        ///     Get the currently content source of the content supplier
        /// </summary>
        T ContentSource { get; }

        /// <summary>
        ///     Notifies the receiver that a selection has changed to the object of type <see cref="T"/>
        /// </summary>
        /// <param name="contentSource"></param>
        void ChangeContentSource(T contentSource);
    }
}