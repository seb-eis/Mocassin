using System;
using System.Threading.Tasks;

namespace Mocassin.UI.Base.Commands
{
    /// <summary>
    ///     Adapter class that wraps <see cref="Delegate" /> objects into a <see cref="AsyncVoidParameterCommand" />
    /// </summary>
    public sealed class AsyncRelayCommand : AsyncVoidParameterCommand
    {
        /// <summary>
        ///     The <see cref="Delegate" /> to call on checking if the command can be executed
        /// </summary>
        private readonly Func<bool> canExecuteFunc;

        /// <summary>
        ///     The <see cref="Delegate" /> to call on command execution
        /// </summary>
        private readonly Func<Task> executeFunc;

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
        public override bool CanExecuteInternal() => canExecuteFunc?.Invoke() ?? base.CanExecuteInternal();

        /// <inheritdoc />
        public override Task ExecuteAsync() => executeFunc.Invoke();

        /// <summary>
        ///     Creates an <see cref="AsyncRelayCommand" /> from a set of delegates
        /// </summary>
        /// <param name="action"></param>
        /// <param name="canExecuteFunc"></param>
        /// <returns></returns>
        public static AsyncRelayCommand Create(Action action, Func<bool> canExecuteFunc = null)
        {
            return new AsyncRelayCommand(() => Task.Run(action), canExecuteFunc);
        }

        /// <summary>
        ///     Creates an <see cref="AsyncRelayCommand{T}" /> from a set of delegates
        /// </summary>
        /// <param name="action"></param>
        /// <param name="canExecuteFunc"></param>
        /// <returns></returns>
        public static AsyncRelayCommand<T> Create<T>(Action<T> action, Func<T, bool> canExecuteFunc = null)
        {
            return new AsyncRelayCommand<T>(x => Task.Run(() => action.Invoke(x)), canExecuteFunc);
        }
    }

    /// <summary>
    ///     Adapter class that wraps <see cref="Delegate" /> objects into a <see cref="AsyncCommand{T}" /> />
    /// </summary>
    public sealed class AsyncRelayCommand<T> : AsyncCommand<T>
    {
        /// <summary>
        ///     The <see cref="Delegate" /> to call on checking if the command can be executed
        /// </summary>
        private readonly Func<T, bool> canExecuteFunc;

        /// <summary>
        ///     The <see cref="Delegate" /> to call on command execution
        /// </summary>
        private readonly Func<T, Task> executeFunc;

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
        public override bool CanExecuteInternal(T parameter) => canExecuteFunc?.Invoke(parameter) ?? base.CanExecuteInternal(parameter);

        /// <inheritdoc />
        public override Task ExecuteAsync(T parameter) => executeFunc.Invoke(parameter);
    }
}