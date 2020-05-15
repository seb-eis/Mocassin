using System;
using System.Windows.Input;

namespace Mocassin.UI.Base.Commands
{
    /// <summary>
    ///     Provides a <see cref="ICommand"/> interface that relays calls to lazy initialized command instances
    /// </summary>
    public class LazyCommandRelay : CommandDictionary<Type, ICommand>
    {
        /// <inheritdoc />
        public override bool CanExecute(Type parameter)
        {
            return GetCommand(parameter).CanExecute(null);
        }

        /// <inheritdoc />
        public override void Execute(Type parameter)
        {
            GetCommand(parameter).Execute(null);
        }

        /// <summary>
        ///     Finds the <see cref="ICommand" /> of the specified <see cref="Type" /> or creates a new one if required
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ICommand GetCommand(Type type)
        {
            if (TryGetValue(type, out var command)) return command;

            command = Activator.CreateInstance(type) as ICommand;
            if (command == null) throw new InvalidOperationException($"The type {type} does not implement ICommand.");
            Add(type, command);
            return command;
        }
    }
}