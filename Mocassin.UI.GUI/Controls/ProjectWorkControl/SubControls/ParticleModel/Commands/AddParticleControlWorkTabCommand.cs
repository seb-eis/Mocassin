using System.Windows.Controls;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.ViewModels.Tabs;
using Mocassin.UI.GUI.Controls.Base.Attributes;
using Mocassin.UI.GUI.Controls.Base.Commands;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.SubControls.Base.Content;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.SubControls.ParticleModel.Commands
{
    /// <summary>
    ///     <see cref="ProjectControlCommand"/> to add new particle control to the main <see cref="ProjectWorkTabControlViewModel"/>
    /// </summary>
    [ControlMenuCommand("New Particle Control")]
    public class AddParticleControlWorkTabCommand : AddBasicModelControlTabCommand
    {
        /// <inheritdoc />
        public AddParticleControlWorkTabCommand(IMocassinProjectControl projectControl)
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