using System;
using System.Windows;
using Mocassin.UI.GUI.Controls.ProjectBrowser;
using Mocassin.UI.GUI.Controls.ProjectBrowser.SubControls.LibraryBrowser;
using Mocassin.UI.GUI.Controls.ProjectBrowser.SubControls.ReportBrowser;
using Mocassin.UI.GUI.Controls.ProjectConsole;
using Mocassin.UI.GUI.Controls.ProjectConsole.SubControls.MessageConsole;
using Mocassin.UI.GUI.Controls.ProjectMenuBar;
using Mocassin.UI.GUI.Controls.ProjectStatusBar;
using ProjectWorkControl;
using ProjectWorkControl.SubControls.XmlControl;

namespace Mocassin.UI.GUI
{
    /// <summary>
    ///     Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var viewModel = CreateNewMainWindowViewModel();
            var mainWindow = CreateNewMainWindow(viewModel);
        
            mainWindow.Show();
            base.OnStartup(e);
        }

        /// <summary>
        ///     Creates new <see cref="MainWindowViewModel" /> with initialized sub view models
        /// </summary>
        /// <returns></returns>
        public MainWindowViewModel CreateNewMainWindowViewModel()
        {
            return new MainWindowViewModel();
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