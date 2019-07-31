using System.Windows.Controls;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Commands;
using Mocassin.UI.GUI.Controls.Visualizer;

namespace Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.VisualMenu.Commands
{
    /// <summary>
    ///     A <see cref="AddDefaultLayoutControlTabCommand" /> for adding a new <see cref="ModelViewport3DView" />
    /// </summary>
    public class AddModelViewport3DTabCommand : AddDefaultLayoutControlTabCommand
    {
        /// <inheritdoc />
        public AddModelViewport3DTabCommand(IMocassinProjectControl projectControl)
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
            return "3D Model Viewport";
        }
    }
}