using System;
using System.Collections.Generic;
using Mocassin.UI.Base.Commands;
using Mocassin.UI.GUI.Base.DataContext;

namespace Mocassin.UI.GUI.Controls.Base.Commands
{
    /// <summary>
    ///     Base class for <see cref="CommandDictionary{TKey,TCommand}" /> implementations that redirect <see cref="Type" />
    ///     information to <see cref="ProjectControlCommand" /> instances
    /// </summary>
    public abstract class ProjectControlCommandDictionary : CommandDictionary<Type, ProjectControlCommand>
    {
        /// <summary>
        ///     Get the <see cref="IProjectAppControl" /> the dictionary targets
        /// </summary>
        protected IProjectAppControl ProjectControl { get; }

        /// <summary>
        ///     Creates new <see cref="ProjectControlCommandDictionary" /> with the passed sequence of
        ///     <see cref="ProjectControlCommand" /> instances
        /// </summary>
        /// <param name="projectControl"></param>
        /// <param name="commands"></param>
        protected ProjectControlCommandDictionary(IProjectAppControl projectControl, IEnumerable<ProjectControlCommand> commands)
            : this(projectControl)
        {
            AddCommands(commands);
        }

        /// <inheritdoc />
        protected ProjectControlCommandDictionary(IProjectAppControl projectControl)
        {
            ProjectControl = projectControl ?? throw new ArgumentNullException(nameof(projectControl));
        }

        /// <summary>
        ///     Add a <see cref="ProjectControlCommand" /> to the <see cref="ProjectControlCommandDictionary" />
        /// </summary>
        /// <param name="command"></param>
        public void AddCommand(ProjectControlCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            if (ContainsKey(command.GetType())) throw new InvalidOperationException("Command type already defined");

            Add(new KeyValuePair<Type, ProjectControlCommand>(command.GetType(), command));
        }

        /// <summary>
        ///     Adds a sequence of <see cref="ProjectControlCommand" /> instances to the
        ///     <see cref="ProjectControlCommandDictionary" />
        /// </summary>
        /// <param name="commands"></param>
        public void AddCommands(IEnumerable<ProjectControlCommand> commands)
        {
            foreach (var projectControlCommand in commands) AddCommand(projectControlCommand);
        }
    }
}