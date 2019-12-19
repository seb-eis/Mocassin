using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
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
        ///     Get or set a <see cref="Dictionary{TKey,TValue}"/> for property backups
        /// </summary>
        private Dictionary<string, object> PropertyBackups { get; set; }

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
        ///     Sets a property value and invokes the changed event if the value was actually changed
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
        ///     Sets a property value. Executes the provided callback and raises the change event only if the value was actually changed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="backingField"></param>
        /// <param name="value"></param>
        /// <param name="onChangeAction"></param>
        /// <param name="propertyName"></param>
        protected void SetProperty<T>(ref T backingField, T value, Action onChangeAction, [CallerMemberName] string propertyName = null)
        {
            if (Equals(backingField, value)) return;
            backingField = value;
            onChangeAction.Invoke();
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
        ///     Get a property backup value by property name or a default value if the backup does not exist
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected T GetPropertyBackup<T>([CallerMemberName] string propertyName = null)
        {
            if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));
            if (PropertyBackups == null || !PropertyBackups.TryGetValue(propertyName, out var propertyValue)) return default;
            return (T) propertyValue;
        }

        /// <summary>
        ///     Sets a property backup value by property name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        protected void SetPropertyBackup<T>(T value, [CallerMemberName] string propertyName = null)
        {
            if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));
            PropertyBackups ??= new Dictionary<string, object>();
            PropertyBackups[propertyName] = value;
        }

        /// <summary>
        ///     Synchronous execution of an <see cref="Action" /> on the UI thread
        /// </summary>
        /// <param name="action"></param>
        public void ExecuteOnAppThread(Action action)
        {
            if (!(Application.Current?.Dispatcher is { } dispatcher)) return;
            if (!dispatcher.CheckAccess())
            {
                dispatcher.Invoke(action);
                return;
            }

            action.Invoke();
        }

        /// <summary>
        ///     Synchronous execution of a <see cref="Func{TResult}" /> on the UI thread
        /// </summary>
        /// <param name="function"></param>
        public TResult ExecuteOnAppThread<TResult>(Func<TResult> function)
        {
            if (!(Application.Current?.Dispatcher is { } dispatcher)) return default;

            return dispatcher.CheckAccess()
                ? function.Invoke()
                : dispatcher.Invoke(function);
        }

        /// <summary>
        ///     Async executes an <see cref="Action" /> on the UI thread. This method should be used when the UI action requires awaiting
        /// </summary>
        /// <param name="action"></param>
        /// <param name="priority"></param>
        public Task ExecuteOnAppThreadAsync(Action action, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            return !(Application.Current?.Dispatcher is { } dispatcher) ? default : dispatcher.InvokeAsync(action, priority).Task;
        }

        /// <summary>
        ///     Async executes a <see cref="Func{TResult}" /> on the UI thread. This method should be used when the UI action requires awaiting
        /// </summary>
        /// <param name="function"></param>
        /// <param name="priority"></param>
        public Task<TResult> ExecuteOnAppThreadAsync<TResult>(Func<TResult> function, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            return !(Application.Current?.Dispatcher is { } dispatcher) ? default : dispatcher.InvokeAsync(function, priority).Task;
        }

        /// <summary>
        ///     Queues an <see cref="Action" /> for execution on the UI thread. This method should be used when the UI action is low priority fire-and-forget
        /// </summary>
        /// <param name="action"></param>
        /// <param name="priority"></param>
        public void QueueOnAppDispatcher(Action action, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            if (!(Application.Current?.Dispatcher is { } dispatcher)) return;
            dispatcher.InvokeAsync(action, priority);
        }

        /// <summary>
        ///     Attaches a single <see cref="Action"/> invoke to the next property change that fulfills the <see cref="Predicate{T}"/>. The optional flag controls if an already matched predicate causes immediate execution
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="action"></param>
        /// <param name="predicate"></param>
        /// <param name="propertyName"></param>
        /// <param name="executeIfAlreadyTrue"></param>
        public void AttachToPropertyChange<TProperty>(Action action, Predicate<TProperty> predicate, string propertyName, bool executeIfAlreadyTrue = true)
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

            if (executeIfAlreadyTrue && CheckProperty(propertyName))
            {
                action();
                return;
            }

            PropertyChanged += InvokeAction;
        }

        /// <summary>
        ///     Attaches a single <see cref="Action"/> invoke to the next property change. The optional flag controls if an already matched predicate causes immediate execution
        /// </summary>
        /// <param name="action"></param>
        /// <param name="propertyName"></param>
        /// <param name="executeIfAlreadyTrue"></param>
        public void AttachToPropertyChange(Action action, string propertyName, bool executeIfAlreadyTrue = true)
        {
            AttachToPropertyChange<object>(action, x => true, propertyName, executeIfAlreadyTrue);
        }

        /// <summary>
        ///     Performs a delayed execution of the passed <see cref="Action"/> if the property with the provided name does not change within the delay
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="action"></param>
        /// <param name="propertyName"></param>
        /// <param name="onAppThread"></param>
        protected async Task ExecuteIfPropertyUnchanged(Action action, TimeSpan delay, string propertyName, bool onAppThread = false)
        {
            var abortExecution = false;
            AttachToPropertyChange(() => abortExecution = true, propertyName, false);
            await Task.Run(() => Thread.Sleep(delay));
            if (abortExecution) return;
            if (onAppThread) ExecuteOnAppThread(action);
            else action();
        }
    }
}