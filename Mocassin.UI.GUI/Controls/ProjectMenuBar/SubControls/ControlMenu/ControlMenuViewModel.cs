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
        ///     Get the <see cref="LazyProjectCommandRelay" /> that supplies command instances
        /// </summary>
        public LazyProjectCommandRelay CommandRelay { get; }

        /// <inheritdoc />
        public ControlMenuViewModel(IProjectAppControl projectControl)
            : base(projectControl)
        {
            CommandRelay = new LazyProjectCommandRelay(projectControl);
        }
    }
}