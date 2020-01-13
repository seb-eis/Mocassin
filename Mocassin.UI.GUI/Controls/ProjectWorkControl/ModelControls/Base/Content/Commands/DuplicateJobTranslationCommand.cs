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
    ///     The <see cref="AsyncProjectControlCommand" /> to duplicate a <see cref="ProjectJobTranslationGraph" /> and add the copy to the selected
    ///     project
    /// </summary>
    public class DuplicateJobTranslationCommand : AsyncProjectControlCommand<ProjectJobTranslationGraph>
    {
        /// <summary>
        ///     Get an <see cref="Action"/> to be executed on success
        /// </summary>
        private Action<ProjectJobTranslationGraph> OnSuccessAction { get; }

        /// <inheritdoc />
        public DuplicateJobTranslationCommand(IMocassinProjectControl projectControl, Action<ProjectJobTranslationGraph> onSuccessAction = null)
            : base(projectControl)
        {
            OnSuccessAction = onSuccessAction;
        }

        /// <inheritdoc />
        public override Task ExecuteAsync(ProjectJobTranslationGraph parameter)
        {
            return Task.Run(() => AddDuplicate(parameter));
        }

        /// <inheritdoc />
        public override bool CanExecuteInternal(ProjectJobTranslationGraph parameter)
        {
            return parameter?.Parent != null && base.CanExecuteInternal(parameter);
        }

        /// <summary>
        ///     Creates and adds  duplicate of a <see cref="ProjectJobTranslationGraph" /> to its parent project
        /// </summary>
        /// <param name="source"></param>
        private void AddDuplicate(ProjectJobTranslationGraph source)
        {
            var duplicate = source.Duplicate();
            ProjectControl.ExecuteOnAppThread(() => source.Parent.ProjectJobTranslationGraphs.Add(duplicate));
            OnSuccessAction?.Invoke(duplicate);
        }
    }
}