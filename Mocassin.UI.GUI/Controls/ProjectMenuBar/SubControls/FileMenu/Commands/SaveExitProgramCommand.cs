using System.Windows;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Commands;

namespace Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.FileMenu.Commands
{
    /// <summary>
    ///     <see cref="ProjectControlCommand" /> implementation to safely exit the program with check for potential unsaved
    ///     changes
    /// </summary>
    public class SaveExitProgramCommand : ProjectControlCommand
    {
        /// <inheritdoc />
        public SaveExitProgramCommand(IProjectAppControl projectControl)
            : base(projectControl)
        {
        }

        /// <inheritdoc />
        public override void Execute()
        {
            ProjectControl.ProjectManagerViewModel.CloseProjectLibraryCommand.Execute(null);
            if (ProjectControl.OpenProjectLibrary != null) return;

            ProjectControl.DisposeServices();
            Application.Current.Shutdown();
        }

        /// <inheritdoc />
        public override bool CanExecuteInternal() => ProjectControl.ProjectManagerViewModel != null;
    }
}