using System;
using System.Collections.Generic;
using Mocassin.UI.GUI.Base.DataContext;

namespace Mocassin.UI.GUI.Controls.Base.Commands
{
    /// <summary>
    ///     Lazy relay for <see cref="ProjectControlCommand" /> instances that creates missing instances automatically on
    ///     first request
    /// </summary>
    public class LazyProjectCommandRelay : ProjectControlCommandDictionary
    {
        /// <inheritdoc />
        public LazyProjectCommandRelay(IProjectAppControl projectControl)
            : base(projectControl)
        {
        }

        /// <inheritdoc />
        public override bool CanExecute(Type parameter)
        {
            return GetCommand(parameter).CanExecute();
        }

        /// <inheritdoc />
        public override void Execute(Type parameter)
        {
            GetCommand(parameter).Execute();
        }

        /// <summary>
        ///     Gets the <see cref="ProjectControlCommand" /> of the specified <see cref="Type" />. If it doesn't exist, the system
        ///     will try to create a new instance
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ProjectControlCommand GetCommand(Type type)
        {
            if (TryGetValue(type, out var command)) return command;

            command = (ProjectControlCommand) Activator.CreateInstance(type, ProjectControl);
            AddCommand(command);

            return command;
        }
    }
}