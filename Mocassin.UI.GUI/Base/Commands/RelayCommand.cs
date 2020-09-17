using System;

namespace Mocassin.UI.Base.Commands
{
    /// <summary>
    ///     Adapter base class to wrap <see cref="Delegate" /> objects into a <see cref="VoidParameterCommand" />
    /// </summary>
    public sealed class RelayCommand : VoidParameterCommand
    {
        /// <summary>
        ///     The <see cref="Action" /> to call on command execution
        /// </summary>
        private readonly Action action;

        /// <summary>
        ///     The <see cref="Func{TResult}" /> object to call to check for can execute
        /// </summary>
        private readonly Func<bool> canExecuteFunction;

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
        /// <param name="canExecuteFunction"></param>
        public RelayCommand(Action execute, Func<bool> canExecuteFunction)
        {
            action = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecuteFunction = canExecuteFunction;
        }

        /// <inheritdoc />
        public override bool CanExecute() => canExecuteFunction?.Invoke() ?? base.CanExecute();

        /// <inheritdoc />
        public override void Execute()
        {
            action.Invoke();
        }

        /// <summary>
        ///     Wraps a <see cref="Command{T}" /> with a fixed parameter into a parameterless <see cref="RelayCommand" />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static RelayCommand WrapCommand<T>(Command<T> command, T parameter)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            return new RelayCommand(() => command.Execute(parameter), () => command.CanExecute(parameter));
        }

        /// <summary>
        ///     Wraps a <see cref="Command{T}" /> and a parameter provider <see cref="Func{TResult}" /> into a parameterless
        ///     <see cref="RelayCommand" />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"></param>
        /// <param name="paramProvider"></param>
        /// <returns></returns>
        public static RelayCommand WrapCommand<T>(Command<T> command, Func<T> paramProvider)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            if (paramProvider == null) throw new ArgumentNullException(nameof(paramProvider));

            return new RelayCommand(() => command.Execute(paramProvider()), () => command.CanExecute(paramProvider()));
        }

        /// <summary>
        ///     Wraps a <see cref="Command{T}" /> provider and a parameter provider <see cref="Func{TResult}" /> into a
        ///     parameterless <see cref="RelayCommand" />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandProvider"></param>
        /// <param name="paramProvider"></param>
        /// <returns></returns>
        public static RelayCommand WrapCommand<T>(Func<Command<T>> commandProvider, Func<T> paramProvider)
        {
            if (commandProvider == null) throw new ArgumentNullException(nameof(commandProvider));
            if (paramProvider == null) throw new ArgumentNullException(nameof(paramProvider));

            void LocalExecute() => commandProvider()?.Execute(paramProvider());

            bool LocalCanExecute()
            {
                var command = commandProvider();
                return command != null && command.CanExecute(paramProvider());
            }

            return new RelayCommand(LocalExecute, LocalCanExecute);
        }

        /// <summary>
        ///     Creates a new <see cref="RelayCommand" /> that executes an <see cref="Action" />
        /// </summary>
        /// <param name="executeAction"></param>
        /// <returns></returns>
        public static RelayCommand Create(Action executeAction)
        {
            if (executeAction is null) throw new ArgumentNullException(nameof(executeAction));
            return new RelayCommand(executeAction);
        }

        /// <summary>
        ///     Creates a new <see cref="RelayCommand" /> that executes an <see cref="Action" /> with a can execute check
        /// </summary>
        /// <param name="executeAction"></param>
        /// <param name="canExecuteFunc"></param>
        /// <returns></returns>
        public static RelayCommand Create(Action executeAction, Func<bool> canExecuteFunc)
        {
            if (executeAction is null) throw new ArgumentNullException(nameof(executeAction));
            if (canExecuteFunc is null) throw new ArgumentNullException(nameof(canExecuteFunc));
            return new RelayCommand(executeAction, canExecuteFunc);
        }

        /// <summary>
        ///     Creates a new <see cref="RelayCommand{T}" /> that executes an <see cref="Action{T}" />
        /// </summary>
        /// <param name="executeAction"></param>
        /// <returns></returns>
        public static RelayCommand<T> Create<T>(Action<T> executeAction)
        {
            if (executeAction is null) throw new ArgumentNullException(nameof(executeAction));
            return new RelayCommand<T>(executeAction);
        }

        /// <summary>
        ///     Creates a new <see cref="RelayCommand{T}" /> that executes an <see cref="Action" />
        /// </summary>
        /// <param name="executeAction"></param>
        /// <param name="canExecuteFunc"></param>
        /// <returns></returns>
        public static RelayCommand<T> Create<T>(Action<T> executeAction, Func<T, bool> canExecuteFunc)
        {
            if (executeAction is null) throw new ArgumentNullException(nameof(executeAction));
            if (canExecuteFunc is null) throw new ArgumentNullException(nameof(canExecuteFunc));
            return new RelayCommand<T>(executeAction, canExecuteFunc);
        }
    }

    /// <summary>
    ///     Adapter base class to wrap <see cref="Delegate" /> objects into a <see cref="Command{T}" />
    /// </summary>
    public sealed class RelayCommand<T> : Command<T>
    {
        /// <summary>
        ///     The <see cref="Func{TResult}" /> to call on execution
        /// </summary>
        private readonly Action<T> action;

        /// <summary>
        ///     The <see cref="Func{TResult}" /> to
        /// </summary>
        private readonly Func<T, bool> canExecuteFunction;

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
        public RelayCommand(Action<T> execute, Func<T, bool> canExecuteFunction)
        {
            action = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecuteFunction = canExecuteFunction;
        }

        /// <inheritdoc />
        public override bool CanExecute(T parameter) => canExecuteFunction?.Invoke(parameter) ?? base.CanExecute(parameter);

        /// <inheritdoc />
        public override void Execute(T parameter)
        {
            action.Invoke(parameter);
        }
    }
}