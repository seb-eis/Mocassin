using System.Threading.Tasks;

namespace Mocassin.UI.Base.Commands
{
    /// <summary>
    ///     Abstract base class for <see cref="AsyncCommand" /> implementations without a call parameter
    /// </summary>
    public abstract class AsyncVoidParameterCommand : AsyncCommand
    {
        /// <inheritdoc />
        protected sealed override bool CanExecuteInternal(object parameter) => CanExecuteInternal();

        /// <summary>
        ///     Parameterless can execute check
        /// </summary>
        /// <returns></returns>
        public virtual bool CanExecuteInternal() => true;

        /// <inheritdoc />
        public sealed override async Task ExecuteAsync(object parameter)
        {
            await ExecuteAsync();
        }

        /// <summary>
        ///     Parameterless async execution
        /// </summary>
        /// <returns></returns>
        public abstract Task ExecuteAsync();
    }
}