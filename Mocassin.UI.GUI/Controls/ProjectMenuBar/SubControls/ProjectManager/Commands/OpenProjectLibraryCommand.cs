using System.IO;
using Mocassin.UI.Base.Commands;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Commands;

namespace Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.ProjectManager.Commands
{
    /// <summary>
    ///     The <see cref="Command{T}" /> to open a project library
    /// </summary>
    public class OpenProjectLibraryCommand : ProjectControlCommand<string>
    {
        /// <inheritdoc />
        public OpenProjectLibraryCommand(IProjectAppControl projectControl)
            : base(projectControl)
        {
        }

        /// <inheritdoc />
        public override bool CanExecuteInternal(string parameter)
        {
            return File.Exists(parameter) && ProjectControl.ProjectManagerViewModel != null;
        }

        /// <inheritdoc />
        public override void Execute(string parameter)
        {
            ProjectControl.ProjectManagerViewModel.LoadActiveProjectLibrary(parameter);
        }
    }
}