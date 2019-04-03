using System.Windows.Controls;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Commands;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.TransitionModel;

namespace Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.ControlMenu.Commands
{
    /// <summary>
    ///     The <see cref="AddDefaultLayoutControlTabCommand" /> to add a default <see cref="TransitionModelControlView" /> to
    ///     the work tab system
    /// </summary>
    public class AddTransitionControlTabCommand : AddDefaultLayoutControlTabCommand
    {
        /// <inheritdoc />
        public AddTransitionControlTabCommand(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
        }

        /// <inheritdoc />
        protected override ContentControl GetDataControl()
        {
            return new TransitionModelControlView {DataContext = new TransitionModelControlViewModel(ProjectControl)};
        }

        /// <inheritdoc />
        protected override string GetTabName()
        {
            return "Transition Control";
        }
    }
}