﻿using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Commands;

namespace Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.ProjectManager.Commands
{
    /// <summary>
    ///     The <see cref="ProjectControlCommand" /> to save all pending changes to the current project library
    /// </summary>
    public class SaveProjectLibraryChangesCommand : ProjectControlCommand
    {
        /// <inheritdoc />
        public SaveProjectLibraryChangesCommand(IProjectAppControl projectControl)
            : base(projectControl)
        {
        }

        /// <inheritdoc />
        public override bool CanExecuteInternal() =>
            ProjectControl.ProjectManagerViewModel != null
            && ProjectControl.OpenProjectLibrary != null;

        /// <inheritdoc />
        public override void Execute()
        {
            ProjectControl.ProjectManagerViewModel.SaveActiveProjectLibraryChanges();
        }
    }
}