using Mocassin.UI.Base.ViewModels;
using Mocassin.UI.GUI.Base.ViewModels.Collections;

namespace Mocassin.UI.GUI.Controls.ProjectConsole.SubControls.MessageConsole
{
    /// <summary>
    ///     The <see cref="ViewModel" /> implementation for the main message display console
    /// </summary>
    public class MessageConsoleViewModel : ObservableCollectionViewModel<string>
    {
        /// <summary>
        ///     The <see cref="FontSize" /> backing field
        /// </summary>
        private int fontSize;

        /// <summary>
        ///     Get or set the font size of the message console
        /// </summary>
        public int FontSize
        {
            get => fontSize;
            set => SetProperty(ref fontSize, value);
        }

        /// <summary>
        ///     Creates a new <see cref="MessageConsoleViewModel" /> with default settings
        /// </summary>
        public MessageConsoleViewModel()
        {
            FontSize = 14;
        }
    }
}