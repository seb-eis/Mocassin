using System;
using System.Windows;
using Mocassin.UI.GUI.MainControls.ProjectBrowser;
using Mocassin.UI.GUI.MainControls.ProjectConsole;
using Mocassin.UI.GUI.MainControls.ProjectConsole.SubControls.MessageConsole;
using Mocassin.UI.GUI.MainControls.ProjectMenuBar;
using Mocassin.UI.GUI.MainControls.ProjectStatusBar;
using Mocassin.UI.GUI.MainControls.ProjectTabControl;

namespace Mocassin.UI.GUI
{
    /// <summary>
    ///     Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var projectBrowserViewModel = new ProjectBrowserViewModel();
            var projectConsoleViewModel = new ProjectConsoleViewModel();
            var projectMenuBarViewModel = new ProjectMenuBarViewModel();
            var projectTabControlViewModel = new ProjectTabControlViewModel();
            var projectStatusBarViewModel = new ProjectStatusBarViewModel();
            var mainViewModel = new MainWindowViewModel(projectMenuBarViewModel, projectStatusBarViewModel, projectBrowserViewModel,
                projectConsoleViewModel, projectTabControlViewModel);
            var mainWindow = new MainWindow
            {
                DataContext = mainViewModel
            };
        
            mainWindow.Show();
            base.OnStartup(e);
        }

        /// <summary>
        ///     Creates new <see cref="MainWindowViewModel" /> with initialized sub view models
        /// </summary>
        /// <returns></returns>
        public MainWindowViewModel CreateNewMainWindowViewModel()
        {
            var projectBrowserViewModel = new ProjectBrowserViewModel();
            var projectConsoleViewModel = new ProjectConsoleViewModel();
            var projectMenuBarViewModel = new ProjectMenuBarViewModel();
            var projectTabControlViewModel = new ProjectTabControlViewModel();
            var projectStatusBarViewModel = new ProjectStatusBarViewModel();
            var mainViewModel = new MainWindowViewModel(projectMenuBarViewModel, projectStatusBarViewModel, projectBrowserViewModel,
                projectConsoleViewModel, projectTabControlViewModel);

            return mainViewModel;
        }

        /// <summary>
        ///     Creates a new <see cref="MainWindow" /> that uses the provided <see cref="MainWindowViewModel" />
        /// </summary>
        /// <param name="mainWindowViewModel"></param>
        /// <returns></returns>
        public MainWindow CreateNewMainWindow(MainWindowViewModel mainWindowViewModel)
        {
            if (mainWindowViewModel == null) throw new ArgumentNullException(nameof(mainWindowViewModel));
            var mainWindow = new MainWindow
            {
                DataContext = mainWindowViewModel
            };
            return mainWindow;
        }
    }
}