using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Commands;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.FileMenu;

namespace Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.HelpMenu
{
    /// <summary>
    ///     The <see cref="PrimaryControlViewModel" /> for the <see cref="HelpMenuView" /> that supplies help and update
    ///     commands
    /// </summary>
    public class HelpMenuViewModel : PrimaryControlViewModel
    {
        /// <summary>
        ///     Get the <see cref="ProjectControlCommandDictionary" /> of the <see cref="FileMenuViewModel" />
        /// </summary>
        public AutoProjectCommandSource CommandSource { get; }

        /// <inheritdoc />
        public HelpMenuViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            CommandSource = new AutoProjectCommandSource(projectControl);
        }
    }
}