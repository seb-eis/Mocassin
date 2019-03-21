using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Commands;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.FileMenu.Commands;

namespace Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.FileMenu
{
    /// <summary>
    ///     The <see cref="PrimaryControlViewModel" /> for the <see cref="FileMenuView" /> that supplies basic project
    ///     operations
    /// </summary>
    public class FileMenuViewModel : PrimaryControlViewModel
    {
        /// <summary>
        ///     Get the <see cref="ProjectControlCommandDictionary" /> of the <see cref="FileMenuViewModel" />
        /// </summary>
        public AutoProjectCommandSource CommandSource { get; }

        /// <inheritdoc />
        public FileMenuViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            CommandSource = new AutoProjectCommandSource(projectControl);
        }
    }
}