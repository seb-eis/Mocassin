using System;
using System.Threading.Tasks;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Commands;
using Mocassin.UI.Xml.Jobs;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.Base.Content.Commands
{
    /// <summary>
    ///     The <see cref="AsyncProjectControlCommand" /> to duplicate a <see cref="ProjectJobSetTemplate" /> and add the copy
    ///     to the selected
    ///     project
    /// </summary>
    public class DuplicateJobTranslationCommand : AsyncProjectControlCommand<ProjectJobSetTemplate>
    {
        /// <summary>
        ///     Get an <see cref="Action" /> to be executed on success
        /// </summary>
        private Action<ProjectJobSetTemplate> OnSuccessAction { get; }

        /// <inheritdoc />
        public DuplicateJobTranslationCommand(IMocassinProjectControl projectControl, Action<ProjectJobSetTemplate> onSuccessAction = null)
            : base(projectControl)
        {
            OnSuccessAction = onSuccessAction;
        }

        /// <inheritdoc />
        public override Task ExecuteAsync(ProjectJobSetTemplate parameter)
        {
            return Task.Run(() => AddDuplicate(parameter));
        }

        /// <inheritdoc />
        public override bool CanExecuteInternal(ProjectJobSetTemplate parameter)
        {
            return parameter?.Parent != null && base.CanExecuteInternal(parameter);
        }

        /// <summary>
        ///     Creates and adds  duplicate of a <see cref="ProjectJobSetTemplate" /> to its parent project
        /// </summary>
        /// <param name="source"></param>
        private void AddDuplicate(ProjectJobSetTemplate source)
        {
            var duplicate = source.Duplicate();
            ProjectControl.ExecuteOnAppThread(() => source.Parent.JobSetTemplates.Add(duplicate));
            OnSuccessAction?.Invoke(duplicate);
        }
    }
}