using System;

namespace Mocassin.UI.Base.Commands
{
    /// <summary>
    ///     Adapter base class to wrap <see cref="Delegate" /> objects into a <see cref="ParameterlessCommandBase" />
    /// </summary>
    public sealed class RelayCommand : ParameterlessCommandBase
    {
        /// <summary>
        ///     The <see cref="Action" /> to call on command execution
        /// </summary>
        private readonly Action _action;

        /// <summary>
        ///     The <see cref="Func{TResult}" /> object to call to check for can execute
        /// </summary>
        private readonly Func<bool> _canExecuteFunc;

        /// <summary>
        ///     Creates new <see cref="RelayCommand" /> using the passed <see cref="Action" />
        /// </summary>
        /// <param name="execute"></param>
        public RelayCommand(Action execute)
            : this(execute, null)
        {
        }

        /// <summary>
        ///     Creates new <see cref="RelayCommand" /> using the passed <see cref="Action" /> and <see cref="Func{TResult}" /> can
        ///     execute check
        /// </summary>
        /// <param name="execute"></param>
        public RelayCommand(Action execute, Func<bool> canExecuteFunc)
        {
            _action = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecuteFunc = canExecuteFunc;
        }

        /// <inheritdoc />
        public override bool CanExecute()
        {
            return _canExecuteFunc?.Invoke() ?? base.CanExecute();
        }

        /// <inheritdoc />
        public override void Execute()
        {
            _action.Invoke();
        }
    }

    /// <summary>
    ///     Adapter base class to wrap <see cref="Delegate" /> objects into a <see cref="CommandBase{T}" />
    /// </summary>
    public sealed class RelayCommand<T> : CommandBase<T>
    {
        /// <summary>
        ///     The <see cref="Func{TResult}" /> to call on execution
        /// </summary>
        private readonly Action<T> _action;

        /// <summary>
        ///     The <see cref="Func{TResult}" /> to
        /// </summary>
        private readonly Func<T, bool> _canExecuteFunc;

        /// <summary>
        ///     Creates new <see cref="RelayCommand{T}" /> using the passed <see cref="Action" />
        /// </summary>
        /// <param name="execute"></param>
        public RelayCommand(Action<T> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        ///     Creates new <see cref="RelayCommand{T}" /> using the passed <see cref="Action" /> and
        ///     <see cref="Func{TResult, T1}" /> can execute check
        /// </summary>
        public RelayCommand(Action<T> execute, Func<T, bool> canExecuteFunc)
        {
            _action = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecuteFunc = canExecuteFunc;
        }

        /// <inheritdoc />
        public override bool CanExecute(T parameter)
        {
            return _canExecuteFunc?.Invoke(parameter) ?? base.CanExecute(parameter);
        }

        /// <inheritdoc />
        public override void Execute(T parameter)
        {
            _action.Invoke(parameter);
        }
    }
}