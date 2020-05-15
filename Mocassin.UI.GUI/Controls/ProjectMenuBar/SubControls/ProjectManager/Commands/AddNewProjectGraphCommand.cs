using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Commands;

namespace Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.ProjectManager.Commands
{
    /// <summary>
    ///     The <see cref="ProjectControlCommand" /> implementation to add a new project graph to the active project
    /// </summary>
    public class AddNewProjectGraphCommand : ProjectControlCommand
    {
        /// <inheritdoc />
        public AddNewProjectGraphCommand(IProjectAppControl projectControl)
            : base(projectControl)
        {
        }

        /// <inheritdoc />
        public override bool CanExecuteInternal()
        {
            return ProjectControl?.OpenProjectLibrary != null && ProjectControl.ProjectManagerViewModel != null;
        }

        /// <inheritdoc />
        public override void Execute()
        {
            ProjectControl.ProjectManagerViewModel.AddNewProjectGraphToProject(ProjectControl.OpenProjectLibrary);
        }
    }
}