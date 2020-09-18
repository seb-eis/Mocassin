using System;
using System.Threading.Tasks;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Commands;
using Mocassin.UI.Xml.Jobs;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.Base.Content.Commands
{
    /// <summary>
    ///     The <see cref="AsyncProjectControlCommand{T}" /> to remove a <see cref="ProjectJobSetTemplate" /> from a
    ///     <see cref="MocassinProject" />
    /// </summary>
    public class DeleteJobSetTemplateCommand : AsyncProjectControlCommand<ProjectJobSetTemplate>
    {
        /// <summary>
        ///     Additional <see cref="Action" /> to be invoked on successful removal
        /// </summary>
        private Action OnRemovalAction { get; }

        /// <inheritdoc />
        public DeleteJobSetTemplateCommand(IProjectAppControl projectControl, Action onRemovalAction = null)
            : base(projectControl)
        {
            OnRemovalAction = onRemovalAction;
        }

        /// <inheritdoc />
        public override bool CanExecuteInternal(ProjectJobSetTemplate parameter)
        {
            if (parameter?.Parent == null) return false;
            return base.CanExecuteInternal(parameter) && parameter.Parent.JobSetTemplates.Contains(parameter);
        }

        /// <inheritdoc />
        public override async Task ExecuteAsync(ProjectJobSetTemplate parameter)
        {
            var isRemoved = false;
            await Task.Run(() => { ProjectControl.ExecuteOnAppThread(() => { isRemoved = parameter.Parent.JobSetTemplates.Remove(parameter); }); });
            if (isRemoved) OnRemovalAction?.Invoke();
        }
    }
}