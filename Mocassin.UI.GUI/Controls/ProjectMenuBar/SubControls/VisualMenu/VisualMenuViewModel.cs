using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Commands;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.FileMenu;

namespace Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.VisualMenu
{
    /// <summary>
    ///     The <see cref="PrimaryControlViewModel" /> for the <see cref="VisualMenuView" /> that supplies project
    ///     commands for visualization
    /// </summary>
    public class VisualMenuViewModel : PrimaryControlViewModel
    {
        /// <summary>
        ///     Get the <see cref="ProjectControlCommandDictionary" /> of the <see cref="FileMenuViewModel" />
        /// </summary>
        public AutoProjectCommandSource CommandSource { get; }

        /// <inheritdoc />
        public VisualMenuViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            CommandSource = new AutoProjectCommandSource(projectControl);
        }
    }
}