using System;
using System.Threading.Tasks;
using System.Windows;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Commands;
using Mocassin.UI.GUI.Logic.Validation;
using Mocassin.UI.Xml.Customization;
using Mocassin.UI.Xml.Jobs;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.Base.Content.Commands
{
    /// <summary>
    ///     The <see cref="AsyncProjectControlCommand" /> to add a new <see cref="ProjectJobTranslationGraph" /> to the selected
    ///     project
    /// </summary>
    public class AddNewJobTranslationCommand : AsyncProjectControlCommand
    {
        /// <summary>
        ///     Get the getter delegate for the <see cref="MocassinProjectGraph"/>
        /// </summary>
        private Func<MocassinProjectGraph> ProjectGetter { get; }

        /// <summary>
        ///     Get an <see cref="Action"/> to be executed on success
        /// </summary>
        private Action OnSuccessAction { get; }

        /// <inheritdoc />
        public AddNewJobTranslationCommand(IMocassinProjectControl projectControl, Func<MocassinProjectGraph> projectGetter, Action onSuccessAction)
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
        ///     Tries to create and add a new <see cref="ProjectJobTranslationGraph" /> to the passed
        ///     <see cref="MocassinProjectGraph" />
        /// </summary>
        /// <param name="projectGraph"></param>
        private void TryAddTranslation(MocassinProjectGraph projectGraph)
        {
            if (projectGraph == null) return;

            using (var validator = new ModelValidatorViewModel(projectGraph.ProjectModelGraph, ProjectControl))
            {
                var status = validator.TryCreateCustomization(out var customization);

                if (status != ModelValidationStatus.NoErrorsDetected)
                {
                    ShowErrorMessageBox(status);
                    return;
                }

                var newItem = CreateJobTranslation();
                ProjectControl.ExecuteOnDispatcher(() => projectGraph.ProjectJobTranslationGraphs.Add(newItem));
            }
            OnSuccessAction?.Invoke();
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
        ///     Creates a new <see cref="ProjectJobTranslationGraph" /> that is linked to the <see cref="MocassinProjectGraph" />
        ///     supplied by the internal getter
        /// </summary>
        /// <returns></returns>
        private ProjectJobTranslationGraph CreateJobTranslation()
        {
            return ProjectJobTranslationGraph.Create(ProjectGetter.Invoke());
        }
    }
}