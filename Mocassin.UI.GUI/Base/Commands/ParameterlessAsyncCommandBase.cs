using System.Threading.Tasks;

namespace Mocassin.UI.Base.Commands
{
    /// <summary>
    ///     Abstract base class for <see cref="AsyncCommandBase" /> implementations without a call parameter
    /// </summary>
    public abstract class ParameterlessAsyncCommandBase : AsyncCommandBase
    {
        /// <inheritdoc />
        protected sealed override bool CanExecuteInternal(object parameter)
        {
            return CanExecuteInternal();
        }

        /// <summary>
        ///     Parameterless can execute check
        /// </summary>
        /// <returns></returns>
        public virtual bool CanExecuteInternal()
        {
            return true;
        }

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