using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Commands;
using Mocassin.UI.GUI.Controls.Base.ViewModels;

namespace Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.ControlMenu
{
    /// <summary>
    ///     The <see cref="PrimaryControlViewModel" /> for the <see cref="ControlMenuView" /> that provides tab creation
    ///     control
    /// </summary>
    public class ControlMenuViewModel : PrimaryControlViewModel
    {
        /// <summary>
        ///     Get the <see cref="AutoProjectCommandSource" /> that supplies command instances
        /// </summary>
        public AutoProjectCommandSource CommandSource { get; }

        /// <inheritdoc />
        public ControlMenuViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            CommandSource = new AutoProjectCommandSource(projectControl);
        }
    }
}