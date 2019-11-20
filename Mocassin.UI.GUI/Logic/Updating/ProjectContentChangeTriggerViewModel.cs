using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using Mocassin.Framework.Events;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.ProjectLibrary;

namespace Mocassin.UI.GUI.Logic.Updating
{
    /// <summary>
    ///     The <see cref="PrimaryControlViewModel" /> that checks the active project library for content changes
    /// </summary>
    public sealed class ProjectContentChangeTriggerViewModel : PrimaryControlViewModel
    {
        private bool isChecking;

        /// <summary>
        ///     Get the <see cref="Queue{T}"/> that contains conflicting task
        /// </summary>
        private Queue<Task> ConflictingTasks { get; }

        /// <summary>
        ///     Get the <see cref="ReactiveEvent{TSubject}" /> for the <see cref="CheckTriggerNotification" />
        /// </summary>
        private ReactiveEvent<Unit> CheckTriggeredEvent { get; }

        /// <summary>
        ///     Get the <see cref="CancellationTokenSource" /> to cancel the trigger <see cref="Task" />
        /// </summary>
        private CancellationTokenSource CancellationSource { get; set; }

        /// <summary>
        ///     Get the <see cref="IObservable{T}" /> that informs when the change check is triggered
        /// </summary>
        public IObservable<Unit> CheckTriggerNotification => CheckTriggeredEvent.AsObservable();

        /// <summary>
        ///     Get the running trigger <see cref="Task" />
        /// </summary>
        public Task TriggerTask { get; private set; }

        /// <summary>
        ///     Get or set the check interval <see cref="TimeSpan" /> (defaults to 1s)
        /// </summary>
        public TimeSpan CheckInterval { get; set; } = TimeSpan.FromSeconds(1);

        /// <summary>
        ///     Get a boolean flag if the system is currently checking for changes
        /// </summary>
        public bool IsChecking
        {
            get => isChecking;
            private set => SetProperty(ref isChecking, value);
        }

        /// <inheritdoc />
        public ProjectContentChangeTriggerViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            CheckTriggeredEvent = new ReactiveEvent<Unit>();
            ConflictingTasks = new Queue<Task>(10);
        }

        /// <summary>
        ///     Starts the trigger service if not already running
        /// </summary>
        public void Start()
        {
            if (TriggerTask != null && !TriggerTask.IsCompleted) return;
            CancellationSource = new CancellationTokenSource();
            CheckTriggerNotification.Subscribe(x => RunChangeCheck(), () => CancellationSource.Cancel());
            TriggerTask = Task.Run(() => RunTriggerLoop(CancellationSource.Token));
        }

        /// <summary>
        ///     Stops the trigger service
        /// </summary>
        public void Stop()
        {
            CheckTriggeredEvent.OnCompleted();
            TriggerTask.Wait();
            CancellationSource = null;
            TriggerTask = null;
        }

        /// <summary>
        ///     Attaches an <see cref=" Action"/> and ensures that the change checking system is not running during execution
        /// </summary>
        /// <param name="action"></param>
        /// <param name="onDispatcher"></param>
        /// <returns></returns>
        public Task AttachConflictingAction(Action action, bool onDispatcher = false)
        {
            var task = new Task(onDispatcher ? () => ExecuteOnAppThread(action) : action);

            void EnqueueAndStart()
            {
                ConflictingTasks.Enqueue(task);
                task.Start();
            }

            AttachToPropertyChange<bool>(EnqueueAndStart, x => !x, nameof(IsChecking));
            return task;
        }

        /// <summary>
        ///     Invokes the changes check on the currently open <see cref="IMocassinProjectLibrary" />
        /// </summary>
        private void RunChangeCheck()
        {
            if (ProjectControl.OpenProjectLibrary?.IsDisposed == true || IsChecking) return;
            IsChecking = true;
            try
            {
                AwaitConflictingTasks();
                ProjectControl.OpenProjectLibrary?.CheckForModelChanges();
            }
            catch (Exception e)
            {
                SendCallErrorMessage(new InvalidOperationException($"Unexpected error in change detection system: {e.Message}"));
                IsChecking = false;
            }
            IsChecking = false;
        }

        /// <summary>
        ///     Awaits for completion of all conflicting tasks in the queue
        /// </summary>
        private void AwaitConflictingTasks(int maxSeconds = 20)
        {
            while (ConflictingTasks.Count != 0)
            {
                ConflictingTasks.Dequeue().Wait(TimeSpan.FromSeconds(maxSeconds));
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

            Task<Unit> triggerEventTask = null;
            while (!isStop && !cancellationToken.IsCancellationRequested)
            {
                Thread.Sleep(CheckInterval);
                if (!(triggerEventTask ?? Task.CompletedTask).IsCompleted) continue;
                triggerEventTask = CheckTriggeredEvent.OnNextAsync(Unit.Default);
            }
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            Stop();
            base.Dispose();
        }
    }
}