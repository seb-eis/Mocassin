namespace Mocassin.UI.GUI.Controls.Base.Interfaces
{
    /// <summary>
    ///     Provides a generic interface for sub UI elements that supply selection dependent content of a managing view model
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IContentSupplier<T>
    {
        /// <summary>
        ///     Get the currently set content source
        /// </summary>
        T ContentSource { get; }

        /// <summary>
        ///     Notify the receiver that a selection has been changed to the passed object with type <see cref="T" />
        /// </summary>
        /// <param name="contentSource"></param>
        void ChangeContentSource(T contentSource);
    }
}