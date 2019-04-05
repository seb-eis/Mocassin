using System.Linq;
using System.Windows.Controls;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.ViewModels;
using Mocassin.UI.GUI.Base.ViewModels.JsonBrowser;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.ProjectLibrary;

namespace Mocassin.UI.GUI.Controls.ProjectBrowser.SubControls.LibraryBrowser
{
    /// <summary>
    ///     The <see cref="ViewModelBase" /> for <see cref="ProjectLibraryBrowserView" />
    /// </summary>
    public class ProjectLibraryBrowserViewModel : PrimaryControlViewModel
    {
        /// <summary>
        ///     The <see cref="JsonBrowserViewModel" /> backing field
        /// </summary>
        private JsonBrowserViewModel jsonBrowserViewModel;

        /// <summary>
        ///     Get the <see cref="JsonBrowserViewModel" /> that provides the <see cref="IMocassinProjectLibrary" /> data as a JSON
        ///     tree
        /// </summary>
        public JsonBrowserViewModel JsonBrowserViewModel
        {
            get => jsonBrowserViewModel;
            private set => SetProperty(ref jsonBrowserViewModel, value);
        }

        /// <inheritdoc />
        public ProjectLibraryBrowserViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            JsonBrowserViewModel = new JsonBrowserViewModel();
        }

        /// <inheritdoc />
        protected override void OnProjectLibraryChangedInternal(IMocassinProjectLibrary newProjectLibrary)
        {
            SendToDispatcher(RebuildProjectTreeView);
        }

        /// <inheritdoc />
        protected override void OnProjectContentChangedInternal()
        {
            SendToDispatcher(RebuildProjectTreeView);
        }

        /// <summary>
        ///     Rebuilds the <see cref="TreeViewItem" /> collection of the project graph and displays it in the JSON browser
        /// </summary>
        private void RebuildProjectTreeView()
        {
            if (ProjectControl.ProjectGraphs == null)
            {
                JsonBrowserViewModel.SetRootViewToNoContent();
                return;
            }

            JsonBrowserViewModel.SetActiveTreeView(ProjectControl.ProjectGraphs, "Project Graphs");
        }
    }
}