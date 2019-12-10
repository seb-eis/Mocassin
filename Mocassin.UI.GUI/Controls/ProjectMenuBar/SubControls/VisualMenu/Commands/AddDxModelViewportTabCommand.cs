using System.Windows.Controls;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Commands;
using Mocassin.UI.GUI.Controls.DxVisualizer.ModelViewer;

namespace Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.VisualMenu.Commands
{
    /// <summary>
    ///     A <see cref="AddDefaultLayoutControlTabCommand" /> for adding a new <see cref="DxModelVisualizationView" /> tab
    ///     that is based on SharpDX
    /// </summary>
    public class AddDxModelViewportTabCommand : AddDefaultLayoutControlTabCommand
    {
        /// <inheritdoc />
        public AddDxModelViewportTabCommand(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
        }

        /// <inheritdoc />
        protected override ContentControl GetDataControl()
        {
            return new DxModelVisualizationView {DataContext = new DxModelVisualizationViewModel(ProjectControl)};
        }

        /// <inheritdoc />
        protected override string GetTabName()
        {
            return "3D Model Viewer [DX10+]";
        }
    }
}