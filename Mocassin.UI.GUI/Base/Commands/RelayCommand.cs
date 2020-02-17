using System;

namespace Mocassin.UI.Base.Commands
{
    /// <summary>
    ///     Adapter base class to wrap <see cref="Delegate" /> objects into a <see cref="ParameterlessCommand" />
    /// </summary>
    public sealed class RelayCommand : ParameterlessCommand
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
        public override bool CanExecute()
        {
            return canExecuteFunction?.Invoke() ?? base.CanExecute();
        }

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
        ///     Wraps a <see cref="Command{T}" /> provider and a parameter provider <see cref="Func{TResult}" /> into a parameterless <see cref="RelayCommand" />
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
            bool LocalCanExecute() { var command = commandProvider(); return command != null && command.CanExecute(paramProvider());}

            return new RelayCommand(LocalExecute, LocalCanExecute);
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
        public override bool CanExecute(T parameter)
        {
            return canExecuteFunction?.Invoke(parameter) ?? base.CanExecute(parameter);
        }

        /// <inheritdoc />
        public override void Execute(T parameter)
        {
            action.Invoke(parameter);
        }
    }
}