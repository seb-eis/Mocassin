using System;
using System.Threading.Tasks;
using System.Windows;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Commands;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.Reports;
using Mocassin.UI.Xml.Customization;
using Mocassin.UI.Xml.Helper.Migration;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.Base.Content.Commands
{
    /// <summary>
    ///     A <see cref="AsyncProjectControlCommand" /> implementation that enables partial recycling of deprecated
    ///     <see cref="ProjectCustomizationTemplate" /> instances
    /// </summary>
    public class MigrateCustomizationCommand : AsyncProjectControlCommand<ProjectCustomizationTemplate>
    {
        /// <summary>
        ///     Get the <see cref="Action{T}" /> that is called on success
        /// </summary>
        public Action<ProjectCustomizationTemplate> OnSuccessAction { get; }

        /// <inheritdoc />
        public MigrateCustomizationCommand(IProjectAppControl projectControl, Action<ProjectCustomizationTemplate> onSuccessAction = null)
            : base(projectControl)
        {
            OnSuccessAction = onSuccessAction;
        }

        /// <inheritdoc />
        public override Task ExecuteAsync(ProjectCustomizationTemplate parameter)
        {
            return Task.Run(async () => await AddNewAndMigrateSource(parameter));
        }

        /// <inheritdoc />
        public override bool CanExecuteInternal(ProjectCustomizationTemplate parameter)
        {
            return parameter?.Parent != null && base.CanExecuteInternal(parameter);
        }

        /// <summary>
        ///     Creates a new <see cref="ProjectCustomizationTemplate" />, adds it to its parent project and migrates matching data
        ///     from the source
        /// </summary>
        /// <param name="source"></param>
        private async Task AddNewAndMigrateSource(ProjectCustomizationTemplate source)
        {
            ProjectCustomizationTemplate target = null;
            var command = new AddNewCustomizationCommand(ProjectControl, () => source.Parent, x => target = x);
            await command.ExecuteAsync(null);
            Migrate(source, target);
            OnSuccessAction?.Invoke(target);
        }

        /// <summary>
        ///     Migrates the source <see cref="ProjectCustomizationTemplate" /> to the target
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        private void Migrate(ProjectCustomizationTemplate source, ProjectCustomizationTemplate target)
        {
            target.Name = $"{source.Name} (Migrated)";
            var promptResult = GetRedundantReportUserPromptResult(source);
            var isRedundantReportEnabled = promptResult == MessageBoxResult.Yes;
            var migrationTool = new ProjectCustomizationMigrationTool {IsRedundantReportEnabled = isRedundantReportEnabled};
            var report = migrationTool.Migrate(source, target);
            ProjectControl.ExecuteOnAppThread(() => OnMigrationReportCreated(report));
        }

        /// <summary>
        ///     Handles the provision of a generated <see cref="MigrationReport" /> to the user
        /// </summary>
        /// <param name="report"></param>
        private void OnMigrationReportCreated(MigrationReport report)
        {
            var viewModel = new MigrationReportViewModel {Report = report};
            var view = new MigrationReportView();
            var name = $"Migration Report [{report.MigrationSource}]";
            ProjectControl.ProjectWorkTabControlViewModel.TabHostViewModel.AddDynamicTab(name, viewModel, view);
        }

        /// <summary>
        ///     Prompt the user if redundant data should be reported and returns the result (Options: Yes, No)
        /// </summary>
        /// <returns></returns>
        private MessageBoxResult GetRedundantReportUserPromptResult(ProjectCustomizationTemplate source)
        {
            var caption = $"Migrating - {source.Name}";
            const string message = "Should redundant data be migrated and included in the report?";
            return MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Question);
        }
    }
}