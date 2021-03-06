﻿using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Commands;

namespace Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.ProjectManager.Commands
{
    /// <summary>
    ///     The <see cref="ProjectControlCommand{T}" /> to create a new project library
    /// </summary>
    public class CreateProjectLibraryCommand : ProjectControlCommand<string>
    {
        /// <inheritdoc />
        public CreateProjectLibraryCommand(IProjectAppControl projectControl)
            : base(projectControl)
        {
        }

        /// <inheritdoc />
        public override bool CanExecuteInternal(string parameter) => ProjectControl.ProjectManagerViewModel != null;

        /// <inheritdoc />
        public override void Execute(string parameter)
        {
            ProjectControl.ProjectManagerViewModel.CreateNewActiveProjectLibrary(parameter);
        }
    }
}