using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;
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
    public class MessageConsoleViewModel : PrimaryControlViewModel, IObservableCollectionViewModel<PushMessage>
    {
        /// <summary>
        ///     The <see cref="SelectedMessage" /> backing field
        /// </summary>
        private PushMessage selectedMessage;

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

        /// <inheritdoc />
        public MessageConsoleViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            PushMessageCollectionViewModel = new ObservableCollectionViewModel<PushMessage>(100);
            SubscribeToMessageSystem(projectControl.PushMessageSystem);
        }

        /// <inheritdoc />
        public void InsertItem(int index, PushMessage value)
        {
            PushMessageCollectionViewModel.InsertItem(index, value);
        }

        /// <inheritdoc />
        public void AddItem(PushMessage value)
        {
            PushMessageCollectionViewModel.AddItem(value);
        }

        /// <inheritdoc />
        public void AddItems(IEnumerable<PushMessage> values)
        {
            PushMessageCollectionViewModel.AddItems(values);
        }

        /// <inheritdoc />
        public void RemoveItem(PushMessage value)
        {
            PushMessageCollectionViewModel.RemoveItem(value);
        }

        /// <inheritdoc />
        public bool Contains(PushMessage value)
        {
            return PushMessageCollectionViewModel.Contains(value);
        }

        /// <inheritdoc />
        public void MoveItem(int oldIndex, int newIndex)
        {
            PushMessageCollectionViewModel.MoveItem(oldIndex, newIndex);
        }

        /// <inheritdoc />
        public void Clear()
        {
            PushMessageCollectionViewModel.Clear();
        }

        /// <summary>
        ///     Creates basic subscriptions to the passed <see cref="IPushMessageSystem" /> notifications
        /// </summary>
        /// <param name="messageSystem"></param>
        private void SubscribeToMessageSystem(IPushMessageSystem messageSystem)
        {
            messageSystem?.AnyMessageNotification.Subscribe(x => QueueOnAppDispatcher(() => AddItem(x), DispatcherPriority.Background));
        }
    }
}