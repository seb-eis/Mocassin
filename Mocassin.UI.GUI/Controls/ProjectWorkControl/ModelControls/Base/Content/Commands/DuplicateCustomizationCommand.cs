using System;
using System.Threading.Tasks;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Commands;
using Mocassin.UI.Xml.Customization;
using Mocassin.UI.Xml.Jobs;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.Base.Content.Commands
{
    /// <summary>
    ///     The <see cref="AsyncProjectControlCommand" /> to duplicate a <see cref="ProjectJobSetTemplate" /> and add the
    ///     copy to the selected
    ///     project
    /// </summary>
    public class DuplicateCustomizationCommand : AsyncProjectControlCommand<ProjectCustomizationTemplate>
    {
        /// <summary>
        ///     Get an <see cref="Action" /> to be executed on success
        /// </summary>
        private Action<ProjectCustomizationTemplate> OnSuccessAction { get; }

        /// <inheritdoc />
        public DuplicateCustomizationCommand(IProjectAppControl projectControl, Action<ProjectCustomizationTemplate> onSuccessAction = null)
            : base(projectControl)
        {
            OnSuccessAction = onSuccessAction;
        }

        /// <inheritdoc />
        public override Task ExecuteAsync(ProjectCustomizationTemplate parameter)
        {
            return Task.Run(() => AddDuplicate(parameter));
        }

        /// <inheritdoc />
        public override bool CanExecuteInternal(ProjectCustomizationTemplate parameter) => parameter?.Parent != null && base.CanExecuteInternal(parameter);

        /// <summary>
        ///     Creates and adds a duplicate of a <see cref="ProjectCustomizationTemplate" /> to its parent project
        /// </summary>
        /// <param name="source"></param>
        private void AddDuplicate(ProjectCustomizationTemplate source)
        {
            var duplicate = source.Duplicate();
            ProjectControl.ExecuteOnAppThread(() => source.Parent.CustomizationTemplates.Add(duplicate));
            OnSuccessAction?.Invoke(duplicate);
        }
    }
}