using System.Windows.Controls;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Commands;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.ModelCustomization;

namespace Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.ControlMenu.Commands
{
    /// <summary>
    ///     The <see cref="AddDefaultCustomizationControlTabCommand" /> to add a
    /// </summary>
    public class AddCustomizationControlTabCommand : AddDefaultCustomizationControlTabCommand
    {
        /// <inheritdoc />
        public AddCustomizationControlTabCommand(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
        }

        /// <inheritdoc />
        protected override ContentControl GetDataControl()
        {
            return new ModelCustomizationControlView {DataContext = new ModelCustomizationControlViewModel(ProjectControl)};
        }
    }
}