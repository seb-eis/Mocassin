using System;
using Mocassin.UI.Base.Commands;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.IO;
using Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.ProjectManager;

namespace Mocassin.UI.GUI.Controls.ProjectMenuBar.Commands
{
    /// <summary>
    ///     The <see cref="ParameterlessCommand" /> to show a project loading dialog
    /// </summary>
    public class ShowProjectLoadingDialogCommand : ParameterlessCommand
    {
        /// <summary>
        ///     The <see cref="IMocassinProjectControl" /> that is targeted by the command
        /// </summary>
        private readonly IMocassinProjectControl projectControl;

        /// <summary>
        ///     The <see cref="UserFileSelectionSource" /> that is used by the command
        /// </summary>
        private readonly UserFileSelectionSource userFileSelectionSource;

        /// <summary>
        ///     Creates new <see cref="ShowProjectLoadingDialogCommand" /> that targets the passed <see cref="IMocassinProjectControl"/>
        /// </summary>
        /// <param name="projectControl"></param>
        public ShowProjectLoadingDialogCommand(IMocassinProjectControl projectControl)
        {
            this.projectControl = projectControl ?? throw new ArgumentNullException(nameof(projectControl));
            userFileSelectionSource = UserFileSelectionSource.CreateForProjectFiles();
        }

        /// <inheritdoc />
        public override void Execute()
        {
            if (userFileSelectionSource.TryGetFileSelection(out var selected, true))
                projectControl.ProjectManagerViewModel.OpenProjectLibraryCommand.Execute(selected);
        }

        /// <inheritdoc />
        public override bool CanExecute()
        {
            return base.CanExecute() && projectControl.ProjectManagerViewModel != null;
        }
    }
}