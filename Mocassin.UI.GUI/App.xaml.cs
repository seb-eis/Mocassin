using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Documents;
using Mocassin.UI.GUI.Base.Loading;
using Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.ControlMenu;

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
            var pluginAssemblies = LoadPlugins(null);
            return new MainWindowViewModel(pluginAssemblies);
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

        /// <summary>
        ///     Loads all plugin <see cref="Assembly"/> instances and returns the created <see cref="List{T}"/>
        /// </summary>
        /// <param name="sourceDirectory"></param>
        /// <returns></returns>
        public List<Assembly> LoadPlugins(string sourceDirectory)
        {
            return null;
        }
    }
}