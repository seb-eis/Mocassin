using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Mocassin.UI.GUI.Base.ViewModels
{
    /// <summary>
    ///     Base class for implementations of view models that support the <see cref="INotifyPropertyChanged" /> interface
    /// </summary>
    public abstract class ViewModel : INotifyPropertyChanged
    {
        /// <summary>
        ///     The <see cref="PropertyChangedEventHandler" /> that informs about changed properties
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Function to call on changed properties
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

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
        ///     Ensures that the provided <see cref="Action" /> is executed on the dispatcher thread
        /// </summary>
        /// <param name="action"></param>
        protected void InvokeOnDispatcher(Action action)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(action);
                return;
            }
            action.Invoke();
        }

        /// <summary>
        ///     Ensures that the provided <see cref="Func{TResult}" /> is executed on the dispatcher thread
        /// </summary>
        /// <param name="function"></param>
        protected TResult InvokeOnDispatcher<TResult>(Func<TResult> function)
        {
            return !Application.Current.Dispatcher.CheckAccess() ? Application.Current.Dispatcher.Invoke(function) : function.Invoke();
        }
    }
}