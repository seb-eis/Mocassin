using System;
using Mocassin.UI.Base.Commands;
using Mocassin.UI.GUI.Base.DataContext;

namespace Mocassin.UI.GUI.Controls.Base.Commands
{
    /// <summary>
    ///     Base class for <see cref="AsyncCommand" /> implementations that target the main
    ///     <see cref="IProjectAppControl" />
    ///     that ensures that potential context change events are triggered before checking execution
    /// </summary>
    public abstract class AsyncProjectControlCommand : AsyncCommand
    {
        /// <summary>
        ///     Get the access to the <see cref="IProjectAppControl" /> main project control
        /// </summary>
        protected IProjectAppControl ProjectControl { get; }

        /// <summary>
        ///     Creates new <see cref="ProjectControlCommand" /> that targets the passed <see cref="IProjectAppControl" />
        /// </summary>
        /// <param name="projectControl"></param>
        protected AsyncProjectControlCommand(IProjectAppControl projectControl)
        {
            ProjectControl = projectControl ?? throw new ArgumentNullException(nameof(projectControl));
        }

        /// <inheritdoc />
        protected sealed override bool CanExecuteInternal(object parameter)
        {
            return CanExecuteInternal();
        }

        /// <summary>
        ///     Parameterless internal version of <see cref="CanExecuteInternal(object)" />
        /// </summary>
        /// <returns></returns>
        protected virtual bool CanExecuteInternal()
        {
            return true;
        }
    }

    /// <summary>
    ///     Base class for <see cref="Command{T}" /> implementations that target the main
    ///     <see cref="IProjectAppControl" />
    ///     that ensures that potential context change events are triggered before checking execution
    /// </summary>
    public abstract class AsyncProjectControlCommand<T> : AsyncCommand<T>
    {
        /// <summary>
        ///     Get the access to the <see cref="IProjectAppControl" /> main project control
        /// </summary>
        protected IProjectAppControl ProjectControl { get; }

        /// <summary>
        ///     Creates new <see cref="ProjectControlCommand{T}" /> that targets the passed <see cref="IProjectAppControl" />
        /// </summary>
        /// <param name="projectControl"></param>
        protected AsyncProjectControlCommand(IProjectAppControl projectControl)
        {
            ProjectControl = projectControl ?? throw new ArgumentNullException(nameof(projectControl));
        }

        /// <inheritdoc />
        public override bool CanExecuteInternal(T parameter)
        {
            return true;
        }
    }
}