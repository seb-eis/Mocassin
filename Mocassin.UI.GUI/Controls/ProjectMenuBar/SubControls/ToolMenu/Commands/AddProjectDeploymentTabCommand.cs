using System.Windows.Controls;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Commands;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.ProjectBuilding;

namespace Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.ToolMenu.Commands
{
    /// <summary>
    ///     The <see cref="AddDefaultLayoutControlTabCommand" /> implementation to add a new project deployment tab to the main
    ///     tab control
    /// </summary>
    public class AddProjectDeploymentTabCommand : AddDefaultLayoutControlTabCommand
    {
        /// <inheritdoc />
        public AddProjectDeploymentTabCommand(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
        }

        /// <inheritdoc />
        protected override ContentControl GetDataControl()
        {
            return new ProjectBuildingControlView {DataContext = new ProjectBuildingControlViewModel(ProjectControl)};
        }

        /// <inheritdoc />
        protected override string GetTabName()
        {
            return "Simulation Builder";
        }
    }
}