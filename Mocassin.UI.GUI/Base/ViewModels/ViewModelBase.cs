using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

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
        ///     Forwards a property set using a <see cref="PropertyInfo"/> and a target <see cref="object"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyInfo"></param>
        /// <param name="target"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        protected void SetProperty<T>(PropertyInfo propertyInfo, object target, T value, [CallerMemberName] string propertyName = null)
        {
            if (target == null) return;
            if (Equals((T) propertyInfo.GetValue(target), value)) return;

            propertyInfo.SetValue(target, value);
            OnPropertyChanged(propertyName);
        }

        /// <summary>
        ///     Synchronous execution of an <see cref="Action" /> on the dispatcher
        /// </summary>
        /// <param name="action"></param>
        public void ExecuteOnDispatcher(Action action)
        {
            if (!(Application.Current?.Dispatcher is Dispatcher dispatcher)) return;
            if (!dispatcher.CheckAccess())
            {
                dispatcher.Invoke(action);
                return;
            }

            action.Invoke();
        }

        /// <summary>
        ///     Synchronous execution of a <see cref="Func{TResult}" /> on the dispatcher
        /// </summary>
        /// <param name="function"></param>
        public TResult ExecuteOnDispatcher<TResult>(Func<TResult> function)
        {
            if (!(Application.Current?.Dispatcher is Dispatcher dispatcher)) return default;

            return dispatcher.CheckAccess()
                ? function.Invoke()
                : dispatcher.Invoke(function);
        }

        /// <summary>
        ///     Queues an <see cref="Action" /> for execution on the dispatcher. This method should be used when the dispatcher action requires awaiting
        /// </summary>
        /// <param name="action"></param>
        /// <param name="priority"></param>
        public Task ExecuteOnDispatcherAsync(Action action, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            if (!(Application.Current?.Dispatcher is Dispatcher dispatcher)) return default;
            return dispatcher.InvokeAsync(action, priority).Task;
        }

        /// <summary>
        ///     Queues an <see cref="Func{TResult}" /> for execution on the dispatcher. This method should be used when the dispatcher action requires awaiting
        /// </summary>
        /// <param name="function"></param>
        /// <param name="priority"></param>
        public Task<TResult> ExecuteOnDispatcherAsync<TResult>(Func<TResult> function, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            if (!(Application.Current?.Dispatcher is Dispatcher dispatcher)) return default;
            return dispatcher.InvokeAsync(function, priority).Task;
        }

        /// <summary>
        ///     Queues an <see cref="Action" /> for execution on the dispatcher. This method should be used when the dispatcher action is low priority fire-and-forget
        /// </summary>
        /// <param name="action"></param>
        /// <param name="priority"></param>
        public void SendToDispatcher(Action action, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            if (!(Application.Current?.Dispatcher is Dispatcher dispatcher)) return;
            dispatcher.InvokeAsync(action, priority);
        }

        /// <summary>
        ///     Attaches a single <see cref="Action"/> invoke to the next property change that fulfills the <see cref="Predicate{T}"/>
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="action"></param>
        /// <param name="predicate"></param>
        /// <param name="propertyName"></param>
        public void AttachToPropertyChange<TProperty>(Action action, Predicate<TProperty> predicate, string propertyName)
        {
            bool CheckProperty(string name)
            {
                if (propertyName != name) return false;
                var value = (TProperty) GetType().GetProperty(propertyName)?.GetValue(this);
                return predicate(value);
            }

            void InvokeAction(object sender, PropertyChangedEventArgs e)
            {
                if (!CheckProperty(e.PropertyName)) return;
                action();
                PropertyChanged -= InvokeAction;
            }

            if (CheckProperty(propertyName))
            {
                action();
                return;
            }

            PropertyChanged += InvokeAction;
        }
    }
}