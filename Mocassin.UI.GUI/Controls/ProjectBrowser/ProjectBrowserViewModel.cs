using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.ViewModels.Tabs;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectBrowser.SubControls.ComponentBrowser;
using Mocassin.UI.GUI.Controls.ProjectBrowser.SubControls.DataBrowser;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.GUI.Controls.ProjectBrowser
{
    /// <summary>
    ///     The <see cref="PrimaryControlViewModel" /> that controls project browser tabs
    /// </summary>
    public class ProjectBrowserViewModel : PrimaryControlViewModel
    {
        /// <summary>
        ///     Get the <see cref="ProjectComponentBrowserViewModel" /> used by the tab system
        /// </summary>
        private ProjectComponentBrowserViewModel ComponentBrowserViewModel { get; }

        /// <summary>
        ///     Get the <see cref="ProjectDataBrowserViewModel" /> used by the tab system
        /// </summary>
        private ProjectDataBrowserViewModel DataBrowserViewModel { get; }

        /// <summary>
        ///     Get the <see cref="ControlTabHostViewModel" /> that controls the browser tabs
        /// </summary>
        public ControlTabHostViewModel TabHostViewModel { get; }

        /// <inheritdoc />
        public ProjectBrowserViewModel(IProjectAppControl projectControl)
            : base(projectControl)
        {
            ComponentBrowserViewModel = new ProjectComponentBrowserViewModel(projectControl);
            DataBrowserViewModel = new ProjectDataBrowserViewModel(projectControl);
            TabHostViewModel = new ControlTabHostViewModel();
            TabHostViewModel.AddStaticTab("Solution Explorer", ComponentBrowserViewModel, new ProjectComponentBrowserView());
            TabHostViewModel.AddStaticTab("Data Viewers", DataBrowserViewModel, new ProjectDataBrowserView());
            TabHostViewModel.SetActiveTabByIndex(0);
        }

        /// <summary>
        ///     Retrieves the currently user selected <see cref="MocassinProject" />
        /// </summary>
        /// <returns></returns>
        public MocassinProject GetActiveWorkProject()
        {
            return ComponentBrowserViewModel.ActiveProject;
        }
    }
}