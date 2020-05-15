using System.Windows.Controls;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Commands;
using Mocassin.UI.GUI.Controls.Visualizer;

namespace Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.VisualMenu.Commands
{
    /// <summary>
    ///     A <see cref="AddDefaultLayoutControlTabCommand" /> for adding a new <see cref="ModelViewport3DView" /> tab that is
    ///     based on the WPF built-ins
    /// </summary>
    public class AddWpfModelViewportTabCommand : AddDefaultLayoutControlTabCommand
    {
        /// <inheritdoc />
        public AddWpfModelViewportTabCommand(IProjectAppControl projectControl)
            : base(projectControl)
        {
        }

        /// <inheritdoc />
        protected override ContentControl GetDataControl()
        {
            return new ModelViewport3DView {DataContext = new ModelViewport3DViewModel(ProjectControl)};
        }

        /// <inheritdoc />
        protected override string GetTabName()
        {
            return "3D Model Viewer [DX9]";
        }
    }
}