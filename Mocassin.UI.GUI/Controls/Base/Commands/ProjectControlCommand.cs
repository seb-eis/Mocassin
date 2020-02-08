using System;
using Mocassin.UI.Base.Commands;
using Mocassin.UI.GUI.Base.DataContext;

namespace Mocassin.UI.GUI.Controls.Base.Commands
{
    /// <summary>
    ///     Base class for <see cref="VoidParameterCommand" /> implementations that target the main
    ///     <see cref="IMocassinProjectControl" />
    ///     that ensures that potential context change events are triggered before checking execution
    /// </summary>
    public abstract class ProjectControlCommand : VoidParameterCommand
    {
        /// <summary>
        ///     Get the access to the <see cref="IMocassinProjectControl" /> main project control
        /// </summary>
        protected IMocassinProjectControl ProjectControl { get; }

        /// <summary>
        ///     Creates new <see cref="ProjectControlCommand" /> that targets the passed <see cref="IMocassinProjectControl" />
        /// </summary>
        /// <param name="projectControl"></param>
        protected ProjectControlCommand(IMocassinProjectControl projectControl)
        {
            ProjectControl = projectControl ?? throw new ArgumentNullException(nameof(projectControl));
        }

        /// <inheritdoc />
        public sealed override bool CanExecute()
        {
            ProjectControl.OpenProjectLibrary?.CheckForModelChanges();
            return base.CanExecute() && CanExecuteInternal();
        }

        /// <summary>
        ///     Internal implementation of can execute check
        /// </summary>
        /// <returns></returns>
        public virtual bool CanExecuteInternal()
        {
            return true;
        }
    }

    /// <summary>
    ///     Base class for <see cref="Command{T}" /> implementations that target the main
    ///     <see cref="IMocassinProjectControl" />
    ///     that ensures that potential context change events are triggered before checking execution
    /// </summary>
    public abstract class ProjectControlCommand<T> : Command<T>
    {
        /// <summary>
        ///     Get the access to the <see cref="IMocassinProjectControl" /> main project control
        /// </summary>
        protected IMocassinProjectControl ProjectControl { get; }

        /// <summary>
        ///     Creates new <see cref="ProjectControlCommand{T}" /> that targets the passed <see cref="IMocassinProjectControl" />
        /// </summary>
        /// <param name="projectControl"></param>
        protected ProjectControlCommand(IMocassinProjectControl projectControl)
        {
            ProjectControl = projectControl ?? throw new ArgumentNullException(nameof(projectControl));
        }

        /// <inheritdoc />
        public sealed override bool CanExecute(T parameter)
        {
            ProjectControl.OpenProjectLibrary?.CheckForModelChanges();
            return base.CanExecute(parameter) && CanExecuteInternal(parameter);
        }

        /// <summary>
        ///     Internal implementation of can execute check
        /// </summary>
        /// <returns></returns>
        public virtual bool CanExecuteInternal(T parameter)
        {
            return true;
        }
    }
}