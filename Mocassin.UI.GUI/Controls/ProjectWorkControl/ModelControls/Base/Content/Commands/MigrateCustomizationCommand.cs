using System.Threading.Tasks;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Commands;
using Mocassin.UI.Xml.Customization;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.Base.Content.Commands
{
    /// <summary>
    ///     A <see cref="AsyncProjectControlCommand"/> implementation that enables partial recycling of deprecated <see cref="ProjectCustomizationGraph"/> instances
    /// </summary>
    public class MigrateCustomizationCommand : AsyncProjectControlCommand<ProjectCustomizationGraph>
    {
        /// <inheritdoc />
        public MigrateCustomizationCommand(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
        }

        /// <inheritdoc />
        public override Task ExecuteAsync(ProjectCustomizationGraph parameter)
        {
            throw new System.NotImplementedException();
        }
    }
}