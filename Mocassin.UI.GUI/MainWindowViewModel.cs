using System;
using Mocassin.UI.Base.ViewModel;
using Mocassin.UI.GUI.MainControls.ProjectBrowser;
using Mocassin.UI.GUI.MainControls.ProjectConsole;
using Mocassin.UI.GUI.MainControls.ProjectMenuBar;
using Mocassin.UI.GUI.MainControls.ProjectStatusBar;
using Mocassin.UI.GUI.MainControls.ProjectTabControl;

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
        ///     Get the <see cref="ProjectConsoleViewModel" /> that controls the text console
        /// </summary>
        public ProjectConsoleViewModel ProjectConsoleViewModel { get; }

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
        /// <param name="projectConsoleViewModel"></param>
        /// <param name="projectTabControlViewModel"></param>
        public MainWindowViewModel(ProjectMenuBarViewModel projectMenuBarViewModel, ProjectStatusBarViewModel projectStatusBarViewModel,
            ProjectBrowserViewModel projectBrowserViewModel, ProjectConsoleViewModel projectConsoleViewModel,
            ProjectTabControlViewModel projectTabControlViewModel)
        {
            ProjectMenuBarViewModel = projectMenuBarViewModel ?? throw new ArgumentNullException(nameof(projectMenuBarViewModel));
            ProjectStatusBarViewModel = projectStatusBarViewModel ?? throw new ArgumentNullException(nameof(projectStatusBarViewModel));
            ProjectBrowserViewModel = projectBrowserViewModel ?? throw new ArgumentNullException(nameof(projectBrowserViewModel));
            ProjectConsoleViewModel = projectConsoleViewModel ?? throw new ArgumentNullException(nameof(projectConsoleViewModel));
            ProjectTabControlViewModel = projectTabControlViewModel ?? throw new ArgumentNullException(nameof(projectTabControlViewModel));
        }
    }
}