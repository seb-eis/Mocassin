using System.Linq;
using Mocassin.UI.Base.Commands;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.ProjectBuilding.DataControl
{
    /// <summary>
    ///     The <see cref="ProjectGraphControlViewModel" /> for <see cref="ProjectBuildGraphControlView" /> that controls
    ///     creation of <see cref="MocassinProjectBuildGraph" /> instances
    /// </summary>
    public class ProjectBuildGraphControlViewModel : ProjectGraphControlViewModel
    {
        /// <summary>
        ///     The <see cref="CollectionControlViewModel{T}" /> for the set of <see cref="MocassinProjectBuildGraph" /> instances
        /// </summary>
        public CollectionControlViewModel<MocassinProjectBuildGraph> ProjectBuildCollectionViewModel { get; }

        public RelayCommand AutoAssignBuildModelContextCommand { get; }

        /// <inheritdoc />
        public ProjectBuildGraphControlViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            ProjectBuildCollectionViewModel = new CollectionControlViewModel<MocassinProjectBuildGraph>();
            AutoAssignBuildModelContextCommand = MakeAutoAssignBuildModelContextCommand();
        }

        /// <inheritdoc />
        public override void ChangeContentSource(MocassinProjectGraph contentSource)
        {
            ContentSource = contentSource;
            ProjectBuildCollectionViewModel.SetCollection(ContentSource?.ProjectBuildGraphs);
        }

        public RelayCommand MakeAutoAssignBuildModelContextCommand()
        {
            void Execute()
            {
                ContentSource.ProjectBuildGraphs.Last().Parent = ContentSource;
                ContentSource.ProjectBuildGraphs.Last().ProjectModelGraph = ContentSource?.ProjectModelGraph;
            }

            bool CanExecute()
            {
                return ContentSource?.ProjectBuildGraphs?.LastOrDefault() != null;
            }

            return new RelayCommand(Execute, CanExecute);
        }
    }
}