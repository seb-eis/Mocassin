using System;
using System.Threading.Tasks;
using System.Windows;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Commands;
using Mocassin.UI.GUI.Logic.Validation;
using Mocassin.UI.Xml.Jobs;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.Base.Content.Commands
{
    /// <summary>
    ///     The <see cref="AsyncProjectControlCommand" /> to add a new <see cref="ProjectJobSetTemplate" /> to the
    ///     selected
    ///     project
    /// </summary>
    public class AddNewJobSetTemplateCommand : AsyncProjectControlCommand
    {
        /// <summary>
        ///     Get the getter delegate for the <see cref="MocassinProject" />
        /// </summary>
        private Func<MocassinProject> ProjectGetter { get; }

        /// <summary>
        ///     Get an <see cref="Action" /> to be executed on success
        /// </summary>
        private Action<ProjectJobSetTemplate> OnSuccessAction { get; }

        /// <inheritdoc />
        public AddNewJobSetTemplateCommand(IProjectAppControl projectControl, Func<MocassinProject> projectGetter,
            Action<ProjectJobSetTemplate> onSuccessAction = null)
            : base(projectControl)
        {
            ProjectGetter = projectGetter ?? throw new ArgumentNullException(nameof(projectGetter));
            OnSuccessAction = onSuccessAction;
        }

        /// <inheritdoc />
        public override Task ExecuteAsync(object parameter)
        {
            return Task.Run(() => TryAddTranslation(ProjectGetter()));
        }

        /// <inheritdoc />
        protected override bool CanExecuteInternal()
        {
            return ProjectGetter() != null;
        }

        /// <summary>
        ///     Tries to create and add a new <see cref="ProjectJobSetTemplate" /> to the passed
        ///     <see cref="MocassinProject" />
        /// </summary>
        /// <param name="project"></param>
        private void TryAddTranslation(MocassinProject project)
        {
            if (project == null) return;

            using var validator = new ModelValidatorViewModel(project.ProjectModelData, ProjectControl);
            var status = validator.TryCreateCustomization(out _);

            if (status != ModelValidationStatus.NoErrors)
            {
                ShowErrorMessageBox(status);
                return;
            }

            var newItem = CreateJobTranslation();
            ProjectControl.ExecuteOnAppThread(() => project.JobSetTemplates.Add(newItem));
            OnSuccessAction?.Invoke(newItem);
        }

        /// <summary>
        ///     Shows an <see cref="MessageBox" /> that contains the status error information if the command execution fails
        /// </summary>
        /// <param name="validationStatus"></param>
        private void ShowErrorMessageBox(ModelValidationStatus validationStatus)
        {
            var message = $"Model of project [{ProjectGetter()?.ProjectName}] contains errors." +
                          " Please resolve all model errors before attempting to create job translation instructions!";
            var caption = $"Model error - [{validationStatus.ToString()}]";
            MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        ///     Creates a new <see cref="ProjectJobSetTemplate" /> that is linked to the <see cref="MocassinProject" />
        ///     supplied by the internal getter
        /// </summary>
        /// <returns></returns>
        private ProjectJobSetTemplate CreateJobTranslation()
        {
            return ProjectJobSetTemplate.Create(ProjectGetter.Invoke());
        }
    }
}