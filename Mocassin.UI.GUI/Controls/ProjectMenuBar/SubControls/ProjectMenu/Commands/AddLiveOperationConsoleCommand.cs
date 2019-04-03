using System.Linq;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Commands;
using Mocassin.UI.GUI.Controls.ProjectConsole.SubControls.OperationConsole;
using Mocassin.UI.GUI.Logic.Validation;

namespace Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.ProjectMenu.Commands
{
    /// <summary>
    ///     The <see cref="ProjectControlCommand"/> to add live validation console tabs
    /// </summary>
    public class AddLiveOperationConsoleCommand : ProjectControlCommand
    {
        /// <inheritdoc />
        public AddLiveOperationConsoleCommand(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
        }

        /// <inheritdoc />
        public override bool CanExecuteInternal()
        {
            return ProjectControl.ProjectGraphs?.FirstOrDefault() != null;
        }

        /// <inheritdoc />
        public override void Execute()
        {
            var validator = CreateValidator();
            HookConsoleTab(validator);
            validator.StartContinuousValidation();
        }

        private void HookConsoleTab(ModelLiveValidator validator)
        {
            var viewModel = new OperationReportConsoleViewModel(ProjectControl);
            var view = new OperationReportConsoleView();
            viewModel.ChangeReportSubscription(validator.ReportsChangeNotification, validator.ModelGraph.Key);
            ProjectControl.ProjectConsoleTabControlViewModel.TabControlViewModel
                .AddCloseableTab("Reports", viewModel,view);
        }

        private ModelLiveValidator CreateValidator()
        {
            return new ModelLiveValidator(ProjectControl.ProjectGraphs?.FirstOrDefault()?.ProjectModelGraph, ProjectControl);
        }
    }
}