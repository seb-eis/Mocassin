using System;
using Mocassin.UI.Base.Commands;

namespace Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.ProjectManager.Commands
{
    /// <summary>
    ///     The <see cref="Command{T}" /> to open a project library
    /// </summary>
    public class OpenProjectLibraryCommand : Command<string>
    {
        /// <summary>
        ///     The affiliated <see cref="ProjectManagerViewModel" />
        /// </summary>
        private readonly ProjectManagerViewModel projectManagerViewModel;

        /// <summary>
        ///     Creates new <see cref="OpenProjectLibraryCommand" /> that targets the passed <see cref="ProjectManagerViewModel" />
        /// </summary>
        /// <param name="projectManagerViewModel"></param>
        public OpenProjectLibraryCommand(ProjectManagerViewModel projectManagerViewModel)
        {
            this.projectManagerViewModel = projectManagerViewModel ?? throw new ArgumentNullException(nameof(projectManagerViewModel));
        }

        /// <inheritdoc />
        public override void Execute(string parameter)
        {
            projectManagerViewModel.LoadActiveProjectLibrary(parameter);
        }
    }
}