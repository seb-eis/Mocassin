namespace Mocassin.UI.GUI.Controls.Base.Interfaces
{
    /// <summary>
    ///     Provides an interface to be informed about user induced changes in selected items of UI elements
    /// </summary>
    public interface IReactToSelectionChanges
    {
        /// <summary>
        ///     Notify the receiver that a selection has been changed to the passed object
        /// </summary>
        /// <param name="newObj"></param>
        void NotifyThatSelectionChanged(object newObj);
    }

    /// <summary>
    ///     Provides an interface to be informed about user induced changes in selected items of type <see cref="T"/> of UI elements
    /// </summary>
    public interface IReactToSelectionChanges<in T> : IReactToSelectionChanges
    {
        /// <summary>
        ///     Notifies the receiver that a selection has changed to the object of type <see cref="T"/>
        /// </summary>
        /// <param name="newObj"></param>
        void NotifyThatSelectionChanged(T newObj);
    }
}