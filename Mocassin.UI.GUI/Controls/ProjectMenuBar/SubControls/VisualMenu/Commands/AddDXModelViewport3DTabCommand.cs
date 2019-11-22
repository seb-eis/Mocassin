using System.Windows.Controls;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Commands;
using Mocassin.UI.GUI.Controls.VisualizerDX;

namespace Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.VisualMenu.Commands
{
    /// <summary>
    ///     The <see cref="AddDefaultLayoutControlTabCommand"/> that adds a new <see cref="DX3DModelDataView"/> tab to the work tab control
    /// </summary>
    public class AddDXModelViewport3DTabCommand : AddDefaultLayoutControlTabCommand
    {
        /// <inheritdoc />
        public AddDXModelViewport3DTabCommand(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
        }

        /// <inheritdoc />
        protected override ContentControl GetDataControl()
        {
            return new DX3DModelDataView {DataContext = new DX3DModelDataViewModel(ProjectControl)};
        }

        /// <inheritdoc />
        protected override string GetTabName()
        {
            return "3D Model Viewer [DX10+]";
        }
    }
}