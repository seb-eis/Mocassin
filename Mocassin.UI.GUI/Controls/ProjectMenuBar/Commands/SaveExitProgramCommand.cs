using System;
using System.Windows;
using Mocassin.UI.Base.Commands;
using Mocassin.UI.GUI.Base.DataContext;

namespace Mocassin.UI.GUI.Controls.ProjectMenuBar.Commands
{
    /// <summary>
    ///     <see cref="Command" /> implementation to safely exit the program with check for potential unsaved changes
    /// </summary>
    public class SaveExitProgramCommand : ParameterlessCommand
    {
        /// <summary>
        ///     Get the <see cref="IMocassinProjectControl" /> the command targets
        /// </summary>
        private readonly IMocassinProjectControl projectControl;

        /// <inheritdoc />
        public SaveExitProgramCommand(IMocassinProjectControl projectControl)
        {
            this.projectControl = projectControl ?? throw new ArgumentNullException(nameof(projectControl));
        }

        /// <inheritdoc />
        public override void Execute()
        {
            projectControl.ProjectManagerViewModel.CloseProjectLibraryCommand.Execute(null);
            if (projectControl.OpenProjectLibrary == null) Application.Current.Shutdown();
        }

        /// <inheritdoc />
        public override bool CanExecute()
        {
            return base.CanExecute() && projectControl.ProjectManagerViewModel != null;
        }
    }
}