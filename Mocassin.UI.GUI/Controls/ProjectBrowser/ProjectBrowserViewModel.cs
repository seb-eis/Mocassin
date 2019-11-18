using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.ViewModels.Tabs;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectBrowser.SubControls.GraphBrowser;
using Mocassin.UI.GUI.Controls.ProjectBrowser.SubControls.LibraryBrowser;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.GUI.Controls.ProjectBrowser
{
    /// <summary>
    ///     The <see cref="PrimaryControlViewModel" /> that controls project browser tabs
    /// </summary>
    public class ProjectBrowserViewModel : PrimaryControlViewModel
    {
        /// <summary>
        ///     Get the <see cref="ProjectGraphBrowserViewModel"/> used by the tab system
        /// </summary>
        private ProjectGraphBrowserViewModel GraphBrowserViewModel { get; }

        /// <summary>
        ///     Get the <see cref="ProjectLibraryBrowserViewModel"/> used by the tab system
        /// </summary>
        private ProjectLibraryBrowserViewModel LibraryBrowserViewModel { get; }

        /// <summary>
        ///     Get the <see cref="UserControlTabControlViewModel" /> that controls the browser tabs
        /// </summary>
        public UserControlTabControlViewModel TabControlViewModel { get; }

        /// <inheritdoc />
        public ProjectBrowserViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            GraphBrowserViewModel = new ProjectGraphBrowserViewModel(projectControl);
            LibraryBrowserViewModel = new ProjectLibraryBrowserViewModel(projectControl);
            TabControlViewModel = new UserControlTabControlViewModel();
            TabControlViewModel.AddNonClosableTab("Solution Explorer", GraphBrowserViewModel, new ProjectGraphBrowserView());
            TabControlViewModel.AddNonClosableTab("Data Viewers", LibraryBrowserViewModel, new ProjectLibraryBrowserView());
            TabControlViewModel.SetActiveTabByIndex(0);
        }

        /// <summary>
        ///     Retrieves the currently user selected <see cref="MocassinProjectGraph"/>
        /// </summary>
        /// <returns></returns>
        public MocassinProjectGraph GetWorkProject()
        {
            return GraphBrowserViewModel.SelectedProject;
        }
    }
}