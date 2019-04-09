using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mocassin.Framework.Events;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.ProjectLibrary;

namespace Mocassin.UI.GUI.Logic.Updating
{
    /// <summary>
    ///     The <see cref="PrimaryControlViewModel"/> that continuously checks the active project library for content changes
    /// </summary>
    public sealed class ProjectContentChangeTriggerViewModel : PrimaryControlViewModel
    {
        /// <summary>
        ///     Get the <see cref="ReactiveEvent{TSubject}" /> for the <see cref="CheckTriggerNotification" />
        /// </summary>
        private ReactiveEvent<Unit> CheckTriggeredEvent { get; }

        /// <summary>
        ///     Get the <see cref="CancellationTokenSource" /> to cancel the trigger <see cref="Task" />
        /// </summary>
        private CancellationTokenSource CancellationSource { get; }

        /// <summary>
        ///     Get the <see cref="IObservable{T}" /> that informs when the change check is triggered
        /// </summary>
        public IObservable<Unit> CheckTriggerNotification => CheckTriggeredEvent.AsObservable();

        /// <summary>
        ///     Get the running trigger <see cref="Task" />
        /// </summary>
        public Task TriggerTask { get; }

        /// <summary>
        ///     Get or set the check interval <see cref="TimeSpan" /> (defaults to 1s)
        /// </summary>
        public TimeSpan CheckInterval { get; set; } = TimeSpan.FromSeconds(1);

        /// <inheritdoc />
        public ProjectContentChangeTriggerViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            CheckTriggeredEvent = new ReactiveEvent<Unit>();
            CancellationSource = new CancellationTokenSource();
            TriggerTask = Task.Run(() => RunTriggerLoop(CancellationSource.Token));
            CheckTriggerNotification.Subscribe(x => RunChangeCheck());
        }

        /// <summary>
        ///     Invokes the changes check on the currently open <see cref="IMocassinProjectLibrary" />
        /// </summary>
        private void RunChangeCheck()
        {
            if (ProjectControl.OpenProjectLibrary?.IsDisposed != true)
            {
                ProjectControl.OpenProjectLibrary?.CheckForContentChange();
            }
        }

        /// <summary>
        ///     Runs the trigger loop that calls the change check event in a given interval
        /// </summary>
        /// <param name="cancellationToken"></param>
        private void RunTriggerLoop(CancellationToken cancellationToken)
        {
            var isStop = false;
            CheckTriggerNotification.Subscribe(x => { }, e => { }, () => { isStop = true; });
            while (!isStop && !cancellationToken.IsCancellationRequested)
            {
                Thread.Sleep(CheckInterval);
                CheckTriggeredEvent.OnNextAsync(Unit.Default);
            }
        }

        /// <inheritdoc />
        public override async void Dispose()
        {
            CheckTriggeredEvent.OnCompleted();
            await TriggerTask;
            base.Dispose();
        }
    }
}