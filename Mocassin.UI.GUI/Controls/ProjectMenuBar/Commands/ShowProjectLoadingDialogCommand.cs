﻿using Mocassin.UI.Base.Commands;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Commands;
using Mocassin.UI.GUI.Controls.Base.IO;

namespace Mocassin.UI.GUI.Controls.ProjectMenuBar.Commands
{
    /// <summary>
    ///     The <see cref="ProjectControlCommand" /> to show a project loading dialog
    /// </summary>
    public class ShowProjectLoadingDialogCommand : ProjectControlCommand
    {
        /// <summary>
        ///     The <see cref="UserFileSelectionSource" /> that is used by the command
        /// </summary>
        private readonly UserFileSelectionSource userFileSelectionSource;

        /// <inheritdoc />
        public ShowProjectLoadingDialogCommand(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            userFileSelectionSource = UserFileSelectionSource.CreateForProjectFiles();
        }

        /// <inheritdoc />
        public override void Execute()
        {
            if (userFileSelectionSource.TryGetFileSelection(out var selected, true))
                ProjectControl.ProjectManagerViewModel.OpenProjectLibraryCommand.Execute(selected);
        }

        /// <inheritdoc />
        public override bool CanExecuteInternal()
        {
            return ProjectControl.ProjectManagerViewModel != null;
        }
    }
}