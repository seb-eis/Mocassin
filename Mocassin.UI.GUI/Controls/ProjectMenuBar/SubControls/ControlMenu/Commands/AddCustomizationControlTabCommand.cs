using System.Windows.Controls;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Commands;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.ModelCustomization;

namespace Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.ControlMenu.Commands
{
    /// <summary>
    ///     The <see cref="AddDefaultCustomizationControlTabCommand" /> to add a default layout
    ///     <see cref="ModelCustomizationControlView" /> with affiliated view model to the work tab control
    /// </summary>
    public class AddCustomizationControlTabCommand : AddDefaultCustomizationControlTabCommand
    {
        /// <inheritdoc />
        public AddCustomizationControlTabCommand(IProjectAppControl projectControl)
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