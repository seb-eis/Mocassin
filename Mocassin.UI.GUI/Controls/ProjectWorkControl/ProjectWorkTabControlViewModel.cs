using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.ViewModels.Tabs;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.WelcomeControl;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl
{
    /// <summary>
    ///     The <see cref="PrimaryControlViewModel" /> for the <see cref="UserControlTabControlView" /> of work tabs
    /// </summary>
    public class ProjectWorkTabControlViewModel : PrimaryControlViewModel
    {
        /// <summary>
        ///     Get the <see cref="UserControlTabControlViewModel" /> that controls the project control tabs
        /// </summary>
        public UserControlTabControlViewModel TabControlViewModel { get; }

        /// <inheritdoc />
        public ProjectWorkTabControlViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            TabControlViewModel = new UserControlTabControlViewModel();
            TabControlViewModel.AddCloseableTab("Welcome", new WelcomeControlViewModel(projectControl), new WelcomeControlView());
            TabControlViewModel.SetActiveTabByIndex(0);
        }
    }
}