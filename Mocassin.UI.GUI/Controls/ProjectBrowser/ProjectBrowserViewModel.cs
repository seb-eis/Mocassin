using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.ViewModels.Tabs;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectBrowser.SubControls.GraphBrowser;
using Mocassin.UI.GUI.Controls.ProjectBrowser.SubControls.LibraryBrowser;

namespace Mocassin.UI.GUI.Controls.ProjectBrowser
{
    /// <summary>
    ///     The <see cref="PrimaryControlViewModel" /> that controls project browser tabs
    /// </summary>
    public class ProjectBrowserViewModel : PrimaryControlViewModel
    {
        /// <summary>
        ///     Get the <see cref="ProjectLibraryBrowserViewModel" /> that controls model data browsing
        /// </summary>
        private ProjectLibraryBrowserViewModel ProjectLibraryBrowserViewModel { get; }

        /// <summary>
        ///     Get the <see cref="UserControlTabControlViewModel" /> that controls the browser tabs
        /// </summary>
        public UserControlTabControlViewModel TabControlViewModel { get; }

        /// <inheritdoc />
        public ProjectBrowserViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            ProjectLibraryBrowserViewModel = new ProjectLibraryBrowserViewModel(projectControl);
            TabControlViewModel = new UserControlTabControlViewModel();
            TabControlViewModel.AddNonClosableTab("Project Library", ProjectLibraryBrowserViewModel, new ProjectLibraryBrowserView());
            TabControlViewModel.SetActiveTabByIndex(0);
        }
    }
}