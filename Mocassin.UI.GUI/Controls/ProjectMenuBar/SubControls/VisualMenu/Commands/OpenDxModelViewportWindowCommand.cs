using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.Objects;
using Mocassin.UI.GUI.Controls.Base.Commands;
using Mocassin.UI.GUI.Controls.DxVisualizer.ModelViewer;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.Base.Content;
using System;
using System.Collections.Generic;

namespace Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.VisualMenu.Commands
{
    /// <summary>
    ///     A <see cref="OpenDefaultContentWindowCommand" /> to open the <see cref="DxModelSceneView" /> in a new window
    /// </summary>
    public class OpenDxModelViewportWindowCommand : OpenDefaultContentWindowCommand
    {
        /// <inheritdoc />
        public OpenDxModelViewportWindowCommand(IProjectAppControl projectControl)
            : base(projectControl)
        {
        }

        /// <inheritdoc />
        protected override VvmContainer CreateVvmContainer()
        {
            var control = new BasicModelContentControlView();
            var viewModel = new BasicModelContentControlViewModel(ProjectControl)
            {
                DataContentControl = new DxModelSceneView {DataContext = new DxModelSceneViewModel(ProjectControl)},
                SelectedProject = ProjectControl.ProjectBrowserViewModel.GetWorkProject()
            };
            return new VvmContainer(control, viewModel, "Dx Visualizer");
        }

        /// <inheritdoc />
        protected override IEnumerable<Action> GetAdditionalDisposeActions(VvmContainer vvmContainer)
        {
            yield break;
        }
    }
}