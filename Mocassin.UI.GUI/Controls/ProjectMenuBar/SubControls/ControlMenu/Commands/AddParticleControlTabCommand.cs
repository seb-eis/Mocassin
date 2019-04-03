using System.Windows.Controls;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Commands;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.ParticleModel;

namespace Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.ControlMenu.Commands
{
    /// <summary>
    ///     <see cref="AddDefaultLayoutControlTabCommand" /> to add a new particle control to the main user control tab sstem
    /// </summary>
    public class AddParticleControlTabCommand : AddDefaultLayoutControlTabCommand
    {
        /// <inheritdoc />
        public AddParticleControlTabCommand(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
        }

        /// <inheritdoc />
        protected override ContentControl GetDataControl()
        {
            return new ParticleModelControlView {DataContext = new ParticleModelControlViewModel(ProjectControl)};
        }

        /// <inheritdoc />
        protected override string GetTabName()
        {
            return "Particle Control";
        }
    }
}