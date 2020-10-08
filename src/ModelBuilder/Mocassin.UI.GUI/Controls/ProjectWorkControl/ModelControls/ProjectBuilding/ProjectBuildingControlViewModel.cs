using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.ProjectBuilding.DataControl;
using Mocassin.UI.Data.Main;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.ProjectBuilding
{
    /// <summary>
    ///     The <see cref="ProjectGraphControlViewModel" /> for <see cref="ProjectBuildingControlView" /> that controls project
    ///     to simulation database conversion
    /// </summary>
    public class ProjectBuildingControlViewModel : ProjectGraphControlViewModel
    {
        /// <summary>
        ///     Get the <see cref="ProjectBuildGraphControlViewModel" /> that controls the build instruction definition
        /// </summary>
        public ProjectBuildGraphControlViewModel BuildGraphControlViewModel { get; }

        /// <summary>
        ///     Get the <see cref="LocalProjectDeployControlViewModel" /> that controls local deployment of a simulation job
        ///     database
        /// </summary>
        public LocalProjectDeployControlViewModel LocalDeployControlViewModel { get; }

        /// <inheritdoc />
        public ProjectBuildingControlViewModel(IProjectAppControl projectControl)
            : base(projectControl)
        {
            BuildGraphControlViewModel = new ProjectBuildGraphControlViewModel(projectControl);
            LocalDeployControlViewModel =
                new LocalProjectDeployControlViewModel(projectControl, BuildGraphControlViewModel.ProjectBuildCollectionViewModel);
        }

        /// <inheritdoc />
        public override void ChangeContentSource(MocassinProject contentSource)
        {
            ContentSource = contentSource;
            BuildGraphControlViewModel.ChangeContentSource(contentSource);
            LocalDeployControlViewModel.ChangeContentSource(contentSource);
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            BuildGraphControlViewModel.Dispose();
            LocalDeployControlViewModel.Dispose();
            base.Dispose();
        }
    }
}