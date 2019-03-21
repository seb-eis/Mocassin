using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace Mocassin.UI.GUI.Base.ViewModels
{
    /// <summary>
    ///     Base class for implementations of view models that support the <see cref="INotifyPropertyChanged" /> interface
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
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
        protected void ExecuteOnDispatcher(Action action)
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
        protected TResult ExecuteOnDispatcher<TResult>(Func<TResult> function)
        {
            return !Application.Current.Dispatcher.CheckAccess()
                ? Application.Current.Dispatcher.Invoke(function)
                : function.Invoke();
        }

        /// <summary>
        ///     Queues the provided <see cref="Action" /> for invocation on the dispatcher thread
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        protected async Task ExecuteOnDispatcherAsync(Action action)
        {
            await Task.Run(() => ExecuteOnDispatcher(action));
        }

        /// <summary>
        ///     Queues the provided <see cref="Func{TResult}" /> for invocation on the dispatcher thread and
        /// </summary>
        /// <param name="function"></param>
        /// <returns></returns>
        protected async Task<TResult> ExecuteOnDispatcherAsync<TResult>(Func<TResult> function)
        {
            return await Task.Run(() => ExecuteOnDispatcher(function));
        }

        /// <summary>
        ///     Queues an <see cref="Action" /> for execution on the dispatcher. This method should be used when awaiting the
        ///     operation is not required and it does not matter when the execution takes place
        /// </summary>
        /// <param name="action"></param>
        protected void SendToDispatcher(Action action)
        {
            Task.Run(() => ExecuteOnDispatcher(action));
        } 
    }
}