using System;
using System.Threading.Tasks;
using System.Windows;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Commands;
using Mocassin.UI.GUI.Logic.Validation;
using Mocassin.UI.Xml.Customization;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.Base.Content.Commands
{
    /// <summary>
    ///     The <see cref="AsyncProjectControlCommand" /> to add a new <see cref="ProjectCustomizationGraph" /> to the selected
    ///     project
    /// </summary>
    public class AddNewCustomizationCommand : AsyncProjectControlCommand
    {
        private Func<MocassinProjectGraph> ProjectGetter { get; }

        /// <inheritdoc />
        public AddNewCustomizationCommand(IMocassinProjectControl projectControl, Func<MocassinProjectGraph> projectGetter)
            : base(projectControl)
        {
            ProjectGetter = projectGetter ?? throw new ArgumentNullException(nameof(projectGetter));
        }

        /// <inheritdoc />
        public override Task ExecuteAsync(object parameter)
        {
            return Task.Run(() => TryAddCustomization(ProjectGetter()));
        }

        /// <inheritdoc />
        protected override bool CanExecuteInternal()
        {
            return ProjectGetter() != null;
        }

        /// <summary>
        ///     Tries to create and add a new <see cref="ProjectCustomizationGraph" /> to the passed
        ///     <see cref="MocassinProjectGraph" />
        /// </summary>
        /// <param name="projectGraph"></param>
        private void TryAddCustomization(MocassinProjectGraph projectGraph)
        {
            if (projectGraph == null) return;
            var validator = new ModelValidatorViewModel(projectGraph.ProjectModelGraph, ProjectControl);
            var status = validator.TryCreateCustomization(out var customization);

            if (status != ModelValidationStatus.NoErrorsDetected)
            {
                ShowErrorMessageBox(status);
                return;
            }

            projectGraph.ProjectCustomizationGraphs.Add(customization);
        }

        /// <summary>
        ///     Shows an <see cref="MessageBox" /> that contains the status error information if the command execution fails
        /// </summary>
        /// <param name="validationStatus"></param>
        private void ShowErrorMessageBox(ModelValidationStatus validationStatus)
        {
            var message = $"Model of project [{ProjectGetter()?.ProjectName}] contains errors." +
                          " Please resolve all model errors before attempting to create customization information!";
            var caption = $"Model error - [{validationStatus.ToString()}]";
            MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}