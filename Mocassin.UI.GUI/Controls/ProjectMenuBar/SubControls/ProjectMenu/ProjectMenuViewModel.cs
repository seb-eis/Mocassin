using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Commands;
using Mocassin.UI.GUI.Controls.Base.ViewModels;

namespace Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.ProjectMenu
{
    /// <summary>
    ///The <see cref="PrimaryControlViewModel"/> for <see cref="ProjectMenuView"/> that supplies project related commands
    /// </summary>
    public class ProjectMenuViewModel : PrimaryControlViewModel
    {
        /// <summary>
        ///     Get the <see cref="AutoProjectCommandSource" /> that supplies command instances
        /// </summary>
        public AutoProjectCommandSource CommandSource { get; }

        /// <inheritdoc />
        public ProjectMenuViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            CommandSource = new AutoProjectCommandSource(ProjectControl);
        }
    }
}