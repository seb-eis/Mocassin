using System;
using System.Threading.Tasks;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Commands;
using Mocassin.UI.Xml.Customization;
using Mocassin.UI.Xml.Jobs;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.Base.Content.Commands
{
    /// <summary>
    ///     The <see cref="AsyncProjectControlCommand" /> to duplicate a <see cref="ProjectJobTranslationGraph" /> and add the
    ///     copy to the selected
    ///     project
    /// </summary>
    public class DuplicateCustomizationCommand : AsyncProjectControlCommand<ProjectCustomizationGraph>
    {
        /// <summary>
        ///     Get the getter delegate for the <see cref="MocassinProjectGraph" />
        /// </summary>
        private Func<MocassinProjectGraph> ProjectGetter { get; }

        /// <summary>
        ///     Get an <see cref="Action" /> to be executed on success
        /// </summary>
        private Action OnSuccessAction { get; }

        /// <inheritdoc />
        public DuplicateCustomizationCommand(IMocassinProjectControl projectControl, Func<MocassinProjectGraph> projectGetter, Action onSuccessAction)
            : base(projectControl)
        {
            ProjectGetter = projectGetter ?? throw new ArgumentNullException(nameof(projectGetter));
            OnSuccessAction = onSuccessAction;
        }

        /// <inheritdoc />
        public override Task ExecuteAsync(ProjectCustomizationGraph parameter)
        {
            return AsyncExecuteWithoutProjectChangeDetection(() => TryAddDuplicate(ProjectGetter(), parameter));
        }

        /// <inheritdoc />
        public override bool CanExecuteInternal(ProjectCustomizationGraph parameter)
        {
            return ProjectGetter() != null && parameter != null;
        }

        /// <summary>
        ///     Tries to create and add a duplicate <see cref="ProjectCustomizationGraph" /> to the passed
        ///     <see cref="MocassinProjectGraph" />
        /// </summary>
        /// <param name="projectGraph"></param>
        /// <param name="source"></param>
        private void TryAddDuplicate(MocassinProjectGraph projectGraph, ProjectCustomizationGraph source)
        {
            if (projectGraph == null) return;

            projectGraph.ProjectCustomizationGraphs.Add(source.Duplicate());
            OnSuccessAction?.Invoke();
        }
    }
}