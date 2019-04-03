﻿using System.Collections.ObjectModel;
using System.Windows.Controls;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.ViewModels.MenuBar;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.ControlMenu;
using Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.FileMenu;
using Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.ProjectMenu;

namespace Mocassin.UI.GUI.Controls.ProjectMenuBar
{
    /// <summary>
    ///     The <see cref="PrimaryControlViewModel" /> for the <see cref="ProjectMenuBarView" /> that supplies project controls
    /// </summary>
    public class ProjectMenuBarViewModel : PrimaryControlViewModel
    {
        /// <summary>
        ///     The <see cref="SubControls.FileMenu.FileMenuViewModel"/> that controls basic file operations
        /// </summary>
        public FileMenuViewModel FileMenuViewModel { get; }

        /// <summary>
        ///     The <see cref="SubControls.ControlMenu.ControlMenuViewModel"/> that controls basic control options
        /// </summary>
        public ControlMenuViewModel ControlMenuViewModel { get; }

        /// <summary>
        ///     Get the <see cref="IDynamicMenuBarViewModel" /> that controls the additional menu items
        /// </summary>
        public IDynamicMenuBarViewModel MenuBarViewModel { get; } 

        /// <inheritdoc />
        public ProjectMenuBarViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            MenuBarViewModel = new DynamicMenuBarViewModel(Dock.Top);
            FileMenuViewModel = new FileMenuViewModel(projectControl);
            ControlMenuViewModel = new ControlMenuViewModel(projectControl);
            MenuBarViewModel.AddCollectionItem(new ProjectMenuView {DataContext = new ProjectMenuViewModel(projectControl)});
        }
    }
}