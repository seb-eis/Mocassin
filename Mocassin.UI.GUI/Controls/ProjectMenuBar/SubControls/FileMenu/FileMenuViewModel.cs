using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Commands;
using Mocassin.UI.GUI.Controls.Base.ViewModels;

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
        public LazyProjectCommandRelay CommandRelay { get; }

        /// <inheritdoc />
        public FileMenuViewModel(IProjectAppControl projectControl)
            : base(projectControl)
        {
            CommandRelay = new LazyProjectCommandRelay(projectControl);
        }
    }
}