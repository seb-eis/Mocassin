using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Mocassin.UI.Xml.Base
{
    /// <summary>
    ///     Base class for all objects that provide basic <see cref="INotifyPropertyChanged" /> functionality
    /// </summary>
    public class PropertyChangeNotifier : INotifyPropertyChanged
    {
        /// <summary>
        ///     Property changed event (Dummy)
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Sets a property value and invokes the changed event
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="backingField"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        protected void SetProperty<T>(ref T backingField, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(backingField, value)) return;

            backingField = value;
            OnPropertyChanged(propertyName);
        }

        /// <summary>
        ///     Notify about a property change
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}