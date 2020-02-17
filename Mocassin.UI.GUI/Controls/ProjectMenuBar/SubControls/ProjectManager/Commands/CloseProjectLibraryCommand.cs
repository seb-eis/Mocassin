using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Commands;

namespace Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.ProjectManager.Commands
{
    /// <summary>
    ///     The <see cref="ProjectControlCommand" /> to close the active project library
    /// </summary>
    public class CloseProjectLibraryCommand : ProjectControlCommand
    {
        /// <inheritdoc />
        public CloseProjectLibraryCommand(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
        }

        /// <inheritdoc />
        public override void Execute()
        {
            ProjectControl.ProjectManagerViewModel.CloseActiveProjectLibrary();
        }

        /// <inheritdoc />
        public override bool CanExecuteInternal()
        {
            return ProjectControl.ProjectManagerViewModel != null;
        }
    }
}