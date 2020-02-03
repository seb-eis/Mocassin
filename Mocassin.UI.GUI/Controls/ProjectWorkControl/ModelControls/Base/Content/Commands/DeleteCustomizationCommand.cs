using System;
using System.Threading.Tasks;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Commands;
using Mocassin.UI.Xml.Customization;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.Base.Content.Commands
{
    /// <summary>
    ///     The <see cref="AsyncProjectControlCommand{T}" /> to remove a <see cref="ProjectCustomizationTemplate" /> from a
    ///     <see cref="MocassinProject" />
    /// </summary>
    public class DeleteCustomizationCommand : AsyncProjectControlCommand<ProjectCustomizationTemplate>
    {
        /// <summary>
        ///     Additional <see cref="Action" /> to be invoked on successful removal
        /// </summary>
        private Action OnRemovalAction { get; }

        /// <inheritdoc />
        public DeleteCustomizationCommand(IMocassinProjectControl projectControl, Action onRemovalAction = null)
            : base(projectControl)
        {
            OnRemovalAction = onRemovalAction;
        }

        /// <inheritdoc />
        public override bool CanExecuteInternal(ProjectCustomizationTemplate parameter)
        {
            if (parameter?.Parent == null) return false;
            return base.CanExecuteInternal(parameter) && parameter.Parent.CustomizationTemplates.Contains(parameter);
        }

        /// <inheritdoc />
        public override async Task ExecuteAsync(ProjectCustomizationTemplate parameter)
        {
            var isRemoved = false;
            await Task.Run(() =>
            {
                ProjectControl.ExecuteOnAppThread(() =>
                {
                    isRemoved = parameter.Parent.CustomizationTemplates.Remove(parameter);
                });
            });
            if (isRemoved) OnRemovalAction?.Invoke();
        }
    }
}