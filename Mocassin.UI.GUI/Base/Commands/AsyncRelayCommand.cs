using System;
using System.Threading.Tasks;

namespace Mocassin.UI.Base.Commands
{
    /// <summary>
    ///     Adapter class that wraps <see cref="Delegate" /> objects into a <see cref="ParameterlessAsyncCommand" />
    /// </summary>
    public sealed class AsyncRelayCommand : ParameterlessAsyncCommand
    {
        /// <summary>
        ///     The <see cref="Delegate" /> to call on command execution
        /// </summary>
        private readonly Func<Task> executeFunc;

        /// <summary>
        ///     The <see cref="Delegate" /> to call on checking if the command can be executed
        /// </summary>
        private readonly Func<bool> canExecuteFunc;

        /// <summary>
        ///     Creates new <see cref="AsyncRelayCommand" /> using the provided <see cref="Func{TResult}" />
        /// </summary>
        /// <param name="execute"></param>
        public AsyncRelayCommand(Func<Task> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        ///     Creates new <see cref="AsyncRelayCommand" /> using the provided <see cref="Func{TResult}" /> and execution check
        /// </summary>
        /// <param name="execute"></param>
        /// <param name="canExecuteFunc"></param>
        public AsyncRelayCommand(Func<Task> execute, Func<bool> canExecuteFunc)
        {
            executeFunc = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecuteFunc = canExecuteFunc;
        }

        /// <inheritdoc />
        public override bool CanExecuteInternal()
        {
            return canExecuteFunc?.Invoke() ?? base.CanExecuteInternal();
        }

        /// <inheritdoc />
        public override Task ExecuteAsync()
        {
            return executeFunc.Invoke();
        }
    }

    /// <summary>
    ///     Adapter class that wraps <see cref="Delegate" /> objects into a <see cref="AsyncCommand{T}" /> />
    /// </summary>
    public sealed class AsyncRelayCommand<T> : AsyncCommand<T>
    {
        /// <summary>
        ///     The <see cref="Delegate" /> to call on command execution
        /// </summary>
        private readonly Func<T, Task> executeFunc;

        /// <summary>
        ///     The <see cref="Delegate" /> to call on checking if the command can be executed
        /// </summary>
        private readonly Func<T, bool> canExecuteFunc;

        /// <summary>
        ///     Create new <see cref="AsyncCommand{T}" /> using the provided execution <see cref="Delegate" />
        /// </summary>
        /// <param name="execute"></param>
        public AsyncRelayCommand(Func<T, Task> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        ///     Create new <see cref="AsyncRelayCommand{T}" /> using the provided execution and check <see cref="Delegate" />
        /// </summary>
        /// <param name="execute"></param>
        /// <param name="canExecuteFunc"></param>
        public AsyncRelayCommand(Func<T, Task> execute, Func<T, bool> canExecuteFunc)
        {
            executeFunc = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecuteFunc = canExecuteFunc;
        }

        /// <inheritdoc />
        public override bool CanExecuteInternal(T parameter)
        {
            return canExecuteFunc?.Invoke(parameter) ?? base.CanExecuteInternal(parameter);
        }

        /// <inheritdoc />
        public override Task ExecuteAsync(T parameter)
        {
            return executeFunc.Invoke(parameter);
        }
    }
}