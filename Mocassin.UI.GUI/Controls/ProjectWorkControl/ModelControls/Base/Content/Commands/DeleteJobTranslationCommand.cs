﻿using System;
using System.Threading.Tasks;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Commands;
using Mocassin.UI.Xml.Jobs;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.Base.Content.Commands
{
    /// <summary>
    ///     The <see cref="AsyncProjectControlCommand{T}" /> to remove a <see cref="ProjectJobTranslationGraph" /> from a
    ///     <see cref="MocassinProjectGraph" />
    /// </summary>
    public class DeleteJobTranslationCommand : AsyncProjectControlCommand<ProjectJobTranslationGraph>
    {
        /// <summary>
        ///     The getter delegate for the target <see cref="MocassinProjectGraph" />
        /// </summary>
        private Func<MocassinProjectGraph> ProjectGetter { get; }

        /// <summary>
        ///     Additional <see cref="Action" /> to be invoked on successful removal
        /// </summary>
        private Action OnRemovalAction { get; }

        /// <inheritdoc />
        public DeleteJobTranslationCommand(IMocassinProjectControl projectControl, Func<MocassinProjectGraph> projectGetter,
            Action onRemovalAction)
            : base(projectControl)
        {
            ProjectGetter = projectGetter ?? throw new ArgumentNullException(nameof(projectGetter));
            OnRemovalAction = onRemovalAction;
        }

        /// <inheritdoc />
        public override bool CanExecuteInternal(ProjectJobTranslationGraph parameter)
        {
            if (parameter == null || ProjectGetter() == null) return false;
            return base.CanExecuteInternal(parameter) && ProjectGetter().ProjectJobTranslationGraphs.Contains(parameter);
        }

        /// <inheritdoc />
        public override async Task ExecuteAsync(ProjectJobTranslationGraph parameter)
        {
            var isRemoved = false;
            await Task.Run(() =>
            {
                isRemoved = ProjectGetter().ProjectJobTranslationGraphs.Remove(parameter);
            });
            if (isRemoved) OnRemovalAction?.Invoke();
        }
    }
}