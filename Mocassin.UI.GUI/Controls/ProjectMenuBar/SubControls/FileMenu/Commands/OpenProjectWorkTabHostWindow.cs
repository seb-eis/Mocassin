using System;
using System.Collections.Generic;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.Objects;
using Mocassin.UI.GUI.Controls.Base.Commands;
using Mocassin.UI.GUI.Controls.ProjectWorkControl;

namespace Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.FileMenu.Commands
{
    /// <summary>
    ///     A <see cref="OpenDefaultContentWindowCommand" /> implementation to open a windowed
    ///     <see cref="ProjectWorkTabControlView" />
    /// </summary>
    public class OpenProjectWorkTabHostWindow : OpenDefaultContentWindowCommand
    {
        /// <inheritdoc />
        public OpenProjectWorkTabHostWindow(IProjectAppControl projectControl)
            : base(projectControl)
        {
        }

        /// <inheritdoc />
        protected override VvmContainer CreateVvmContainer()
        {
            var view = new ProjectWorkTabControlView();
            var viewModel = new ProjectWorkTabControlViewModel(ProjectControl);
            return new VvmContainer(view, viewModel, "Work Tab");
        }

        /// <inheritdoc />
        protected override IEnumerable<Action> GetAdditionalDisposeActions(VvmContainer vvmContainer)
        {
            yield break;
        }
    }
}