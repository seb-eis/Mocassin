using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Commands;
using Mocassin.UI.GUI.Controls.Base.ViewModels;

namespace Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.ToolMenu
{
    /// <summary>
    ///     The <see cref="PrimaryControlViewModel" /> for <see cref="ProjectToolMenuView" /> that supplies project related
    ///     tool
    /// </summary>
    public class ProjectToolMenuViewModel : PrimaryControlViewModel
    {
        /// <summary>
        ///     Get the <see cref="AutoProjectCommandSource" /> that supplies command instances
        /// </summary>
        public AutoProjectCommandSource CommandSource { get; }

        /// <inheritdoc />
        public ProjectToolMenuViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            CommandSource = new AutoProjectCommandSource(ProjectControl);
        }
    }
}