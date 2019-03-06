using System;
using Mocassin.UI.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectBrowser;
using Mocassin.UI.GUI.Controls.ProjectConsole;
using Mocassin.UI.GUI.Controls.ProjectMenuBar;
using Mocassin.UI.GUI.Controls.ProjectStatusBar;
using Mocassin.UI.GUI.Controls.ProjectTabControl;

namespace Mocassin.UI.GUI
{
    /// <summary>
    ///     <see cref="ViewModel" /> for the main window of the Mocassin GUI
    /// </summary>
    public class MainWindowViewModel : ViewModel
    {
        /// <summary>
        ///     Get the <see cref="ProjectMenuBarViewModel" /> that controls the primary menu bar
        /// </summary>
        public ProjectMenuBarViewModel ProjectMenuBarViewModel { get; }

        /// <summary>
        ///     Get the <see cref="ProjectStatusBarViewModel" /> that controls the primary status bar
        /// </summary>
        public ProjectStatusBarViewModel ProjectStatusBarViewModel { get; }

        /// <summary>
        ///     Get the <see cref="ProjectBrowserViewModel" /> that controls the project data browser
        /// </summary>
        public ProjectBrowserViewModel ProjectBrowserViewModel { get; }

        /// <summary>
        ///     Get the <see cref="ProjectConsoleTabControlViewModel" /> that controls the text console
        /// </summary>
        public ProjectConsoleTabControlViewModel ProjectConsoleTabControlViewModel { get; }

        /// <summary>
        ///     Get the <see cref="ProjectTabControlViewModel" /> that controls the work tab selection
        /// </summary>
        public ProjectTabControlViewModel ProjectTabControlViewModel { get; }

        /// <summary>
        ///     Creates new <see cref="MainWindowViewModel" /> for the Mocassin GUI
        /// </summary>
        /// <param name="projectMenuBarViewModel"></param>
        /// <param name="projectStatusBarViewModel"></param>
        /// <param name="projectBrowserViewModel"></param>
        /// <param name="projectConsoleTabControlViewModel"></param>
        /// <param name="projectTabControlViewModel"></param>
        public MainWindowViewModel(ProjectMenuBarViewModel projectMenuBarViewModel, ProjectStatusBarViewModel projectStatusBarViewModel,
            ProjectBrowserViewModel projectBrowserViewModel, ProjectConsoleTabControlViewModel projectConsoleTabControlViewModel,
            ProjectTabControlViewModel projectTabControlViewModel)
        {
            ProjectMenuBarViewModel = projectMenuBarViewModel ?? throw new ArgumentNullException(nameof(projectMenuBarViewModel));
            ProjectStatusBarViewModel = projectStatusBarViewModel ?? throw new ArgumentNullException(nameof(projectStatusBarViewModel));
            ProjectBrowserViewModel = projectBrowserViewModel ?? throw new ArgumentNullException(nameof(projectBrowserViewModel));
            ProjectConsoleTabControlViewModel = projectConsoleTabControlViewModel ?? throw new ArgumentNullException(nameof(projectConsoleTabControlViewModel));
            ProjectTabControlViewModel = projectTabControlViewModel ?? throw new ArgumentNullException(nameof(projectTabControlViewModel));
        }
    }
}