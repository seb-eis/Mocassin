using System;
using System.Threading.Tasks;
using Mocassin.UI.Base.Commands;
using Mocassin.UI.GUI.Base.DataContext;

namespace Mocassin.UI.GUI.Controls.Base.Commands
{
    /// <summary>
    ///     Base class for <see cref="AsyncCommand"/> implementations that target the main <see cref="IMocassinProjectControl"/>
    ///     that ensures that potential context change events are triggered before checking execution
    /// </summary>
    public abstract class AsyncProjectControlCommand : AsyncCommand
    {
        /// <summary>
        ///     Get the access to the <see cref="IMocassinProjectControl"/> main project control
        /// </summary>
        protected IMocassinProjectControl ProjectControl { get; }

        /// <summary>
        ///     Creates new <see cref="ProjectControlCommand"/> that targets the passed <see cref="IMocassinProjectControl"/>
        /// </summary>
        /// <param name="projectControl"></param>
        protected AsyncProjectControlCommand(IMocassinProjectControl projectControl)
        {
            ProjectControl = projectControl ?? throw new ArgumentNullException(nameof(projectControl));
        }

        /// <inheritdoc />
        protected sealed override bool CanExecuteInternal(object parameter)
        {
            ProjectControl.OpenProjectLibrary?.CheckForModelChanges();
            return CanExecuteInternal();
        }

        /// <summary>
        ///     Parameterless internal version of <see cref="CanExecuteInternal(object)"/>
        /// </summary>
        /// <returns></returns>
        protected virtual bool CanExecuteInternal()
        {
            return true;
        }

        /// <summary>
        ///     Async save execution of the passed <see cref="Action"/> as a change check conflicting action
        /// </summary>
        /// <param name="action"></param>
        /// <param name="onDispatcher"></param>
        /// <returns></returns>
        protected Task AsyncExecuteWithoutProjectChangeDetection(Action action, bool onDispatcher = false)
        {
            return ProjectControl.AsyncExecuteChangeCheckConflictAction(action, onDispatcher);
        }
    }

    /// <summary>
    ///     Base class for <see cref="Command{T}"/> implementations that target the main <see cref="IMocassinProjectControl"/>
    ///     that ensures that potential context change events are triggered before checking execution
    /// </summary>
    public abstract class AsyncProjectControlCommand<T> : AsyncCommand<T>
    {
        /// <summary>
        ///     Get the access to the <see cref="IMocassinProjectControl"/> main project control
        /// </summary>
        protected IMocassinProjectControl ProjectControl { get; }

        /// <summary>
        ///     Creates new <see cref="ProjectControlCommand{T}"/> that targets the passed <see cref="IMocassinProjectControl"/>
        /// </summary>
        /// <param name="projectControl"></param>
        protected AsyncProjectControlCommand(IMocassinProjectControl projectControl)
        {
            ProjectControl = projectControl ?? throw new ArgumentNullException(nameof(projectControl));
        }

        /// <inheritdoc />
        public override bool CanExecuteInternal(T parameter)
        {
            ProjectControl.OpenProjectLibrary?.CheckForModelChanges();
            return true;
        }

        /// <summary>
        ///     Async save execution of the passed <see cref="Action"/> as a change check conflicting action
        /// </summary>
        /// <param name="action"></param>
        /// <param name="onDispatcher"></param>
        /// <returns></returns>
        protected Task AsyncExecuteWithoutProjectChangeDetection(Action action, bool onDispatcher = false)
        {
            return ProjectControl.AsyncExecuteChangeCheckConflictAction(action, onDispatcher);
        }
    }
}