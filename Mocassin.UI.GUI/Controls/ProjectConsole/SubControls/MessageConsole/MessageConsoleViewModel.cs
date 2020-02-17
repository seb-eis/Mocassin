using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using Mocassin.Framework.Exceptions;
using Mocassin.Framework.Messaging;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.ViewModels.Collections;
using Mocassin.UI.GUI.Controls.Base.ViewModels;

namespace Mocassin.UI.GUI.Controls.ProjectConsole.SubControls.MessageConsole
{
    /// <summary>
    ///     The <see cref="PrimaryControlViewModel" /> for the <see cref="MessageConsoleView" /> that controls the display of
    ///     send <see cref="PushMessage" /> instances
    /// </summary>
    public sealed class MessageConsoleViewModel : PrimaryControlViewModel, IObservableCollectionViewModel<PushMessage>
    {
        private PushMessage selectedMessage;
        private DispatcherPriority priority = DispatcherPriority.Background;

        /// <summary>
        ///     Get or set the <see cref="ConsoleOutputSpy"/> of the message system
        /// </summary>
        private ConsoleOutputSpy ConsoleSpy { get; } = new ConsoleOutputSpy();

        /// <summary>
        ///     Get or set the currently selected <see cref="PushMessage" />
        /// </summary>
        public PushMessage SelectedMessage
        {
            get => selectedMessage;
            set => SetProperty(ref selectedMessage, value);
        }

        /// <summary>
        ///     Get the <see cref="IObservableCollectionViewModel{T}" /> that controls the visible <see cref="PushMessage" />
        ///     instances
        /// </summary>
        public ObservableCollectionViewModel<PushMessage> PushMessageCollectionViewModel { get; }

        /// <inheritdoc />
        public ObservableCollection<PushMessage> ObservableItems => PushMessageCollectionViewModel.ObservableItems;

        /// <summary>
        ///     Get or set the <see cref="DispatcherPriority"/> of the console messages (Default is <see cref="DispatcherPriority.Background"/>)
        /// </summary>
        public DispatcherPriority Priority 
        { 
            get => priority; 
            set => SetProperty(ref priority, value);
        }

        /// <inheritdoc />
        public MessageConsoleViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            PushMessageCollectionViewModel = new ObservableCollectionViewModel<PushMessage>(100);
            SubscribeToMessageSystem(projectControl.PushMessageSystem);
            SpyConsoleOutput();
        }

        /// <inheritdoc />
        public void InsertItem(int index, PushMessage value)
        {
            QueueOnAppDispatcher(() => PushMessageCollectionViewModel.InsertItem(index, value), Priority);
        }

        /// <inheritdoc />
        public void AddItem(PushMessage value)
        {
            QueueOnAppDispatcher(() => PushMessageCollectionViewModel.AddItem(value), Priority);
        }

        /// <inheritdoc />
        public void AddItems(IEnumerable<PushMessage> values)
        {
            QueueOnAppDispatcher(() => PushMessageCollectionViewModel.AddItems(values), Priority);
        }

        /// <inheritdoc />
        public void RemoveItem(PushMessage value)
        {
            QueueOnAppDispatcher(() => PushMessageCollectionViewModel.RemoveItem(value), Priority);
        }

        /// <inheritdoc />
        public bool Contains(PushMessage value)
        {
            return PushMessageCollectionViewModel.Contains(value);
        }

        /// <inheritdoc />
        public void MoveItem(int oldIndex, int newIndex)
        {
            QueueOnAppDispatcher(() => PushMessageCollectionViewModel.MoveItem(oldIndex, newIndex), Priority);
        }

        /// <inheritdoc />
        public void Clear()
        {
            ExecuteOnAppThread(() => PushMessageCollectionViewModel.Clear());
        }

        /// <summary>
        ///     Creates basic subscriptions to the passed <see cref="IPushMessageSystem" /> notifications
        /// </summary>
        /// <param name="messageSystem"></param>
        private void SubscribeToMessageSystem(IPushMessageSystem messageSystem)
        {
            messageSystem?.AnyMessageNotification.Subscribe(AddItem);
        }

        /// <summary>
        ///     Begin to spy the default Console output
        /// </summary>
        private void SpyConsoleOutput()
        {
            ConsoleSpy.StringWriteNotifications.Subscribe(x => PushInfoMessage(x));
            ConsoleSpy.ErrorWriteNotifications.Subscribe(x => PushErrorMessage(x));
            ConsoleSpy.Attach();
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            ConsoleSpy.Dispose();
            base.Dispose();
        }
    }
}