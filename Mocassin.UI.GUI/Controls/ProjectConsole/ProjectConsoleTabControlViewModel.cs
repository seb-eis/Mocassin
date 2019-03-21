using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.ViewModels;
using Mocassin.UI.GUI.Base.ViewModels.Tabs;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectConsole.SubControls.MessageConsole;

namespace Mocassin.UI.GUI.Controls.ProjectConsole
{
    /// <summary>
    ///     TThe <see cref="ViewModelBase" /> for the main project console
    /// </summary>
    public class ProjectConsoleTabControlViewModel : PrimaryControlViewModel
    {
        /// <summary>
        ///     Get the <see cref="UserControlTabControlViewModel" /> that controls the additional tabs of the console control
        /// </summary>
        public UserControlTabControlViewModel TabControlViewModel { get; }

        /// <summary>
        ///     Get the <see cref="SubControls.MessageConsole.MessageConsoleViewModel" /> that controls the display of string
        ///     messages
        /// </summary>
        public MessageConsoleViewModel MessageConsoleViewModel { get; }

        /// <inheritdoc />
        public ProjectConsoleTabControlViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            TabControlViewModel = new UserControlTabControlViewModel();
            MessageConsoleViewModel = new MessageConsoleViewModel(projectControl);
            TabControlViewModel.AddNonClosableTab("Notifications", MessageConsoleViewModel, new MessageConsoleView());
            TabControlViewModel.SetActiveTabByIndex(0);
        }
    }
}