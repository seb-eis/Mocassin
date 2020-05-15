using System.Windows.Controls;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.ViewModels.MenuBar;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.ControlMenu;
using Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.FileMenu;
using Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.HelpMenu;
using Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.ToolMenu;
using Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.VisualMenu;

namespace Mocassin.UI.GUI.Controls.ProjectMenuBar
{
    /// <summary>
    ///     The <see cref="PrimaryControlViewModel" /> for the <see cref="ProjectMenuBarView" /> that supplies project controls
    /// </summary>
    public class ProjectMenuBarViewModel : PrimaryControlViewModel
    {
        /// <summary>
        ///     The <see cref="SubControls.FileMenu.FileMenuViewModel" /> that controls basic file operations
        /// </summary>
        public FileMenuViewModel FileMenuViewModel { get; }

        /// <summary>
        ///     The <see cref="SubControls.ControlMenu.ControlMenuViewModel" /> that controls basic control options
        /// </summary>
        public ControlMenuViewModel ControlMenuViewModel { get; }

        /// <summary>
        ///     The <see cref="SubControls.VisualMenu.VisualMenuViewModel" /> that controls basic visual options
        /// </summary>
        public VisualMenuViewModel VisualMenuViewModel { get; }

        /// <summary>
        ///     The <see cref="SubControls.HelpMenu.HelpMenuViewModel" /> that controls basic help options
        /// </summary>
        public HelpMenuViewModel HelpMenuViewModel { get; }

        /// <summary>
        ///     Get the <see cref="IDynamicMenuBarViewModel" /> that controls the additional menu items
        /// </summary>
        public IDynamicMenuBarViewModel MenuBarViewModel { get; }

        /// <inheritdoc />
        public ProjectMenuBarViewModel(IProjectAppControl projectControl)
            : base(projectControl)
        {
            MenuBarViewModel = new DynamicMenuBarViewModel(Dock.Top);
            FileMenuViewModel = new FileMenuViewModel(projectControl);
            ControlMenuViewModel = new ControlMenuViewModel(projectControl);
            VisualMenuViewModel = new VisualMenuViewModel(projectControl);
            HelpMenuViewModel = new HelpMenuViewModel(projectControl);
            MenuBarViewModel.AddItem(new ProjectToolMenuView {DataContext = new ProjectToolMenuViewModel(projectControl)});
        }
    }
}