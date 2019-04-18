using System.Windows.Controls;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.ViewModels;
using Mocassin.UI.GUI.Controls.Base.Commands;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.ProjectBuilding;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.ProjectBuilding.DataControl;

namespace Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.ProjectMenu.Commands
{
    /// <summary>
    ///     The <see cref="AddDefaultLayoutControlTabCommand" /> implementation to add a new project deployment tab to the main tab control
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
            return new ProjectBuildingControlView {DataContext = new ProjectBuildGraphControlViewModel(ProjectControl)};
        }

        /// <inheritdoc />
        protected override string GetTabName()
        {
            return "Project Deployment";
        }
    }
}