namespace Mocassin.UI.Base.Commands
{
    /// <summary>
    ///     Abstract base class for implementations of parameterless <see cref="Command" /> objects
    /// </summary>
    public abstract class VoidParameterCommand : Command
    {
        /// <inheritdoc />
        public sealed override bool CanExecute(object parameter)
        {
            return CanExecute();
        }

        /// <summary>
        ///     Parameterless can execute check
        /// </summary>
        /// <returns></returns>
        public virtual bool CanExecute()
        {
            return true;
        }

        /// <inheritdoc />
        public sealed override void Execute(object parameter)
        {
            Execute();
        }

        /// <summary>
        ///     Parameterless execution of the command
        /// </summary>
        public abstract void Execute();
    }
}