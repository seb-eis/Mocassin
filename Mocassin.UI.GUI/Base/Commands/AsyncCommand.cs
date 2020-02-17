using System.Threading.Tasks;

namespace Mocassin.UI.Base.Commands
{
    /// <summary>
    ///     Abstract base class for async <see cref="Command" /> implementations
    /// </summary>
    public abstract class AsyncCommand : Command
    {
        /// <summary>
        ///     Backing field for the is executing status
        /// </summary>
        private bool isExecuting;

        /// <summary>
        ///     Get or set a boolean flag if the command is currently executing
        /// </summary>
        protected bool IsExecuting
        {
            get => isExecuting;
            private set
            {
                if (value == isExecuting) return;
                isExecuting = value;
                NotifyCanExecuteChanged();
            }
        }

        /// <inheritdoc />
        protected AsyncCommand()
        {
            isExecuting = false;
        }

        /// <inheritdoc />
        public sealed override bool CanExecute(object parameter)
        {
            return !IsExecuting && CanExecuteInternal(parameter);
        }

        /// <summary>
        ///     Internal can execute check that extends the check for current execution
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        protected virtual bool CanExecuteInternal(object parameter)
        {
            return true;
        }

        /// <inheritdoc />
        public sealed override async void Execute(object parameter)
        {
            IsExecuting = true;
            try
            {
                await ExecuteAsync(parameter);
            }
            finally
            {
                IsExecuting = false;
            }
        }

        /// <summary>
        ///     Execute the command async
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public abstract Task ExecuteAsync(object parameter);
    }

    /// <summary>
    ///     Abstract base class for async <see cref="Command{T}" /> implementations
    /// </summary>
    public abstract class AsyncCommand<T> : AsyncCommand
    {
        /// <inheritdoc />
        protected sealed override bool CanExecuteInternal(object parameter)
        {
            return CanExecuteInternal((T) parameter);
        }

        /// <summary>
        ///     Check if the command can be executed with the passed parameter
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public virtual bool CanExecuteInternal(T parameter)
        {
            return true;
        }

        /// <inheritdoc />
        public sealed override async Task ExecuteAsync(object parameter)
        {
            await ExecuteAsync((T) parameter);
        }

        /// <summary>
        ///     Execute the command async with the passed parameter
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public abstract Task ExecuteAsync(T parameter);
    }
}