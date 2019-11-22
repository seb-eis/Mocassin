using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public MessageConsoleViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            PushMessageCollectionViewModel = new ObservableCollectionViewModel<PushMessage>(100);
            SubscribeToMessageSystem(projectControl.PushMessageSystem);
        }

        /// <inheritdoc />
        public ObservableCollection<PushMessage> ObservableItems => PushMessageCollectionViewModel.ObservableItems;

        /// <inheritdoc />
        public void InsertCollectionItem(int index, PushMessage value)
        {
            PushMessageCollectionViewModel.InsertCollectionItem(index, value);
        }

        /// <inheritdoc />
        public void AddCollectionItem(PushMessage value)
        {
            PushMessageCollectionViewModel.AddCollectionItem(value);
        }

        /// <inheritdoc />
        public void AddCollectionItems(IEnumerable<PushMessage> values)
        {
            PushMessageCollectionViewModel.AddCollectionItems(values);
        }

        /// <inheritdoc />
        public void RemoveCollectionItem(PushMessage value)
        {
            PushMessageCollectionViewModel.RemoveCollectionItem(value);
        }

        /// <inheritdoc />
        public bool CollectionContains(PushMessage value)
        {
            return PushMessageCollectionViewModel.CollectionContains(value);
        }

        /// <inheritdoc />
        public void MoveCollectionItem(int oldIndex, int newIndex)
        {
            PushMessageCollectionViewModel.MoveCollectionItem(oldIndex, newIndex);
        }

        /// <inheritdoc />
        public void ClearCollection()
        {
            PushMessageCollectionViewModel.ClearCollection();
        }

        /// <summary>
        ///     Creates basic subscriptions to the passed <see cref="IPushMessageSystem" /> notifications
        /// </summary>
        /// <param name="messageSystem"></param>
        private void SubscribeToMessageSystem(IPushMessageSystem messageSystem)
        {
            messageSystem?.AnyMessageNotification.Subscribe(AddCollectionItem);
        }
    }
}