using System.Windows.Controls;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Commands;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.SimulationModel;

namespace Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.ControlMenu.Commands
{
    /// <summary>
    ///     The <see cref="AddDefaultLayoutControlTabCommand" /> implementation to add a new simulation control panel to the
    ///     main tab control
    /// </summary>
    public class AddSimulationControlTabCommand : AddDefaultLayoutControlTabCommand
    {
        /// <inheritdoc />
        public AddSimulationControlTabCommand(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
        }

        /// <inheritdoc />
        protected override ContentControl GetDataControl()
        {
            return new SimulationModelControlView {DataContext = new SimulationModelControlViewModel(ProjectControl)};
        }

        /// <inheritdoc />
        protected override string GetTabName()
        {
            return "Simulation Control";
        }
    }
}