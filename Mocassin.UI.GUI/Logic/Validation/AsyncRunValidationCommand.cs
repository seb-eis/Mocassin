using System;
using System.Threading.Tasks;
using Mocassin.UI.Base.Commands;

namespace Mocassin.UI.GUI.Logic.Validation
{
    /// <summary>
    ///     The <see cref="AsyncCommand" /> to run a <see cref="ModelValidatorViewModel" /> validation cycle
    /// </summary>
    public class AsyncRunValidationCommand : AsyncCommand
    {
        private Task currentExecutionTask;
        private readonly object lockObject = new object();

        /// <summary>
        ///     Get the <see cref="ModelValidatorViewModel" />  that the command targets
        /// </summary>
        private ModelValidatorViewModel ValidatorViewModel { get; }

        /// <summary>
        ///     Get the current execution <see cref="Task"/>
        /// </summary>
        private Task CurrentExecutionTask
        {
            get 
            {
                lock (lockObject)
                {
                    return currentExecutionTask;
                }
            }
            set
            {
                lock (lockObject)
                {
                    currentExecutionTask = value;
                }
            }

        }

        /// <inheritdoc />
        public AsyncRunValidationCommand(ModelValidatorViewModel validatorViewModel)
        {
            ValidatorViewModel = validatorViewModel ?? throw new ArgumentNullException(nameof(validatorViewModel));
        }

        /// <inheritdoc />
        public override Task ExecuteAsync(object parameter)
        {
            return Task.Run(() =>
            {
                WhenCurrentExecutionFinished().Wait();
                CurrentExecutionTask = Task.Run(ValidatorViewModel.RunValidation);
            });

        }

        /// <summary>
        ///     Get a <see cref="Task"/> that completes when the last execution has finished
        /// </summary>
        /// <returns></returns>
        public Task WhenCurrentExecutionFinished()
        {
            return CurrentExecutionTask == null || !CurrentExecutionTask.IsCompleted
            ? Task.CompletedTask
            : Task.WhenAll(CurrentExecutionTask);
        }
    }
}