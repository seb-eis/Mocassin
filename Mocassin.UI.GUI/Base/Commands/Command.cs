using System;
using System.Windows.Input;

namespace Mocassin.UI.Base.Commands
{
    /// <summary>
    ///     Abstract base class for untyped <see cref="ICommand" /> implementations
    /// </summary>
    public abstract class Command : ICommand
    {
        /// <summary>
        ///     Get a boolean value if the command can be executed
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public virtual bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        ///     Execute the command with the passed parameter
        /// </summary>
        /// <param name="parameter"></param>
        public abstract void Execute(object parameter);

        /// <summary>
        ///     The <see cref="EventHandler" /> for change of the can execute status
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        /// <summary>
        ///     Notifies all subscribes that the can execution status has changed
        /// </summary>
        protected void NotifyCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
            //CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    ///     Abstract base class for typed <see cref="ICommand" /> implementations
    /// </summary>
    public abstract class Command<T> : Command
    {
        /// <inheritdoc />
        public sealed override bool CanExecute(object parameter)
        {
            return CanExecute((T) parameter);
        }

        /// <summary>
        ///     Checks if the command can be executed
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public virtual bool CanExecute(T parameter)
        {
            return true;
        }


        /// <inheritdoc />
        public sealed override void Execute(object parameter)
        {
            Execute((T) parameter);
        }

        /// <summary>
        ///     Execute the command with the passed parameter of type <see cref="T" />
        /// </summary>
        /// <param name="parameter"></param>
        public abstract void Execute(T parameter);
    }
}