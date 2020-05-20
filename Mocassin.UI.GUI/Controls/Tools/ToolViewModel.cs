using System;
using Mocassin.Framework.Events;
using Mocassin.UI.GUI.Base.ViewModels;
using Mocassin.UI.GUI.Base.ViewModels.Collections;

namespace Mocassin.UI.GUI.Controls.Tools
{
    /// <summary>
    ///     Extended <see cref="ViewModelBase"/> for tool controls
    /// </summary>
    public class ToolViewModel : ViewModelBase
    {
        /// <summary>
        ///     Get the <see cref="ObservableCollectionViewModel{T}"/> that manages the messages
        /// </summary>
        public ObservableCollectionViewModel<Tuple<DateTime, string>> MessagesViewModel { get; }

        /// <inheritdoc />
        public ToolViewModel()
        {
            MessagesViewModel = new ObservableCollectionViewModel<Tuple<DateTime, string>>(1000);
        }

        /// <summary>
        ///     Logs a message <see cref="string"/>
        /// </summary>
        /// <param name="value"></param>
        public void LogMessage(string value)
        {
            MessagesViewModel.AddItem(Tuple.Create(DateTime.Now, value));
        }
    }
}