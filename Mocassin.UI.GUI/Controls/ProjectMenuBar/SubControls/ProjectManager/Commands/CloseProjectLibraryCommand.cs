using System;
using Mocassin.UI.Base.Commands;

namespace Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.ProjectManager.Commands
{
    /// <summary>
    ///     The <see cref="Command" /> to close a project library
    /// </summary>
    public class CloseProjectLibraryCommand : ParameterlessCommand
    {
        /// <summary>
        ///     The affiliated <see cref="ProjectManagerViewModel" />
        /// </summary>
        private readonly ProjectManagerViewModel projectManagerViewModel;

        /// <summary>
        ///     Creates new <see cref="CloseProjectLibraryCommand" /> that targets the passed <see cref="ProjectManagerViewModel" />
        /// </summary>
        /// <param name="projectManagerViewModel"></param>
        public CloseProjectLibraryCommand(ProjectManagerViewModel projectManagerViewModel)
        {
            this.projectManagerViewModel = projectManagerViewModel ?? throw new ArgumentNullException(nameof(projectManagerViewModel));
        }

        /// <inheritdoc />
        public override void Execute()
        {
            projectManagerViewModel.CloseActiveProjectLibrary();
        }
    }
}