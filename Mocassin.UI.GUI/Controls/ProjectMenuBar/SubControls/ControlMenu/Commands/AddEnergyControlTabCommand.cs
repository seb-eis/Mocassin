using System.Windows.Controls;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Commands;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.SubControls.EnergyModel;

namespace Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.ControlMenu.Commands
{
    /// <summary>
    ///     The <see cref="AddDefaultLayoutControlTabCommand" /> implementation to add a new energy model control tab to the
    ///     main tab control
    /// </summary>
    public class AddEnergyControlTabCommand : AddDefaultLayoutControlTabCommand
    {
        /// <inheritdoc />
        public AddEnergyControlTabCommand(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
        }

        /// <inheritdoc />
        protected override ContentControl GetDataControl()
        {
            return new EnergyModelControlView {DataContext = new EnergyModelControlViewModel(ProjectControl)};
        }

        /// <inheritdoc />
        protected override string GetTabName()
        {
            return "Energy Control";
        }
    }
}