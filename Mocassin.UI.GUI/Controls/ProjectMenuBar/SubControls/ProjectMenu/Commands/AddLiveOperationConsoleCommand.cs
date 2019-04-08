using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Commands;
using Mocassin.UI.GUI.Controls.ProjectConsole.SubControls.OperationConsole;

namespace Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.ProjectMenu.Commands
{
    /// <summary>
    ///     The <see cref="ProjectControlCommand" /> to add live validation console tabs
    /// </summary>
    public class AddLiveOperationConsoleCommand : ProjectControlCommand
    {
        /// <inheritdoc />
        public AddLiveOperationConsoleCommand(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
        }

        /// <inheritdoc />
        public override void Execute()
        {
            var view = new OperationReportConsoleView();
            var viewModel = new OperationReportConsoleViewModel(ProjectControl);
            ProjectControl.ProjectConsoleTabControlViewModel.TabControlViewModel
                .AddCloseableTab("Model Validation", viewModel, view);

            ProjectControl.ProjectConsoleTabControlViewModel.TabControlViewModel.SetActiveTabByIndex(-1);
        }
    }
}