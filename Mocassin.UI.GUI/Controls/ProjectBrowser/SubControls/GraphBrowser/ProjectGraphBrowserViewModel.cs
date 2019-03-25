using System.Collections.Generic;
using System.Collections.ObjectModel;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.ProjectLibrary;

namespace Mocassin.UI.GUI.Controls.ProjectBrowser.SubControls.GraphBrowser
{
    /// <summary>
    ///     The <see cref="PrimaryControlViewModel" /> for <see cref="ProjectGraphBrowserView" />
    /// </summary>
    public class ProjectGraphBrowserViewModel : PrimaryControlViewModel
    {
        private object selectedObjectGraph;
        private ObservableCollection<MocassinProjectGraph> projectGraphs;

        /// <summary>
        ///     Get the <see cref="IEnumerable{T}" /> of <see cref="MocassinProjectGraph" /> for the browser
        /// </summary>
        public ObservableCollection<MocassinProjectGraph> ProjectGraphs
        {
            get => projectGraphs;
            private set => SetProperty(ref projectGraphs, value);
        }

        /// <summary>
        ///     Get or set the selected <see cref="object" />
        /// </summary>
        public object SelectedObjectGraph
        {
            get => selectedObjectGraph;
            set => SetProperty(ref selectedObjectGraph, value);
        }

        /// <inheritdoc />
        public ProjectGraphBrowserViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
        }

        /// <inheritdoc />
        protected override void OnProjectLibraryChangedInternal(IMocassinProjectLibrary newProjectLibrary)
        {
            ProjectGraphs = ProjectControl.ProjectGraphs;
        }
    }
}