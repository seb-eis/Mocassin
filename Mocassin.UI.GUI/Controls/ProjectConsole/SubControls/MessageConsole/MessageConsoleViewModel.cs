﻿using System;
using System.Reactive;
using System.Collections.ObjectModel;
using Mocassin.Framework.Messaging;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.ViewModels.Collections;
using Mocassin.UI.GUI.Controls.Base;

namespace Mocassin.UI.GUI.Controls.ProjectConsole.SubControls.MessageConsole
{
    /// <summary>
    ///     The <see cref="PrimaryControlViewModel" /> for the <see cref="MessageConsoleView" /> that controls the display of
    ///     send <see cref="PushMessage" /> instances
    /// </summary>
    public class MessageConsoleViewModel : PrimaryControlViewModel, IObservableCollectionViewModel<PushMessage>
    {
        /// <summary>
        ///     Get the <see cref="IObservableCollectionViewModel{T}" /> that controls the visible <see cref="PushMessage" />
        ///     instances
        /// </summary>
        public ObservableCollectionViewModel<PushMessage> PushMessageCollectionViewModel { get; }

        /// <inheritdoc />
        public MessageConsoleViewModel(IMocassinProjectControl mainProjectControl)
            : base(mainProjectControl)
        {
            PushMessageCollectionViewModel = new ObservableCollectionViewModel<PushMessage>(100);
            SubscribeToMessageSystem(mainProjectControl.PushMessageSystem);
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
        public void RemoveCollectionItem(PushMessage value)
        {
            PushMessageCollectionViewModel.RemoveCollectionItem(value);
        }

        /// <inheritdoc />
        public bool CollectionContains(PushMessage value)
        {
            return PushMessageCollectionViewModel.CollectionContains(value);
        }

        /// <summary>
        ///     Creates basic subscriptions to the passed <see cref="IPushMessageSystem"/> notifications
        /// </summary>
        /// <param name="messageSystem"></param>
        private void SubscribeToMessageSystem(IPushMessageSystem messageSystem)
        {
            messageSystem?.AnyMessageNotification.Subscribe(AddCollectionItem);
        }
    }
}