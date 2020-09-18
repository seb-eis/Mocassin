using System.Windows.Controls;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Commands;
using Mocassin.UI.GUI.Controls.DxVisualizer.ModelViewer;

namespace Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.VisualMenu.Commands
{
    /// <summary>
    ///     A <see cref="AddDefaultLayoutControlTabCommand" /> for adding a new <see cref="DxModelSceneView" /> tab
    ///     that is based on SharpDX
    /// </summary>
    public class AddDxModelViewportTabCommand : AddDefaultLayoutControlTabCommand
    {
        /// <inheritdoc />
        public AddDxModelViewportTabCommand(IProjectAppControl projectControl)
            : base(projectControl)
        {
        }

        /// <inheritdoc />
        protected override ContentControl GetDataControl() => new DxModelSceneView {DataContext = new DxModelSceneViewModel(ProjectControl)};

        /// <inheritdoc />
        protected override string GetTabName() => "3D Model Viewer [DX10+]";
    }
}