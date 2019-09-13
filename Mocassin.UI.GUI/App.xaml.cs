using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;

namespace Mocassin.UI.GUI
{
    /// <summary>
    ///     Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        /// <inheritdoc />
        protected override void OnStartup(StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            using (var startupWindow = new StartupWindow())
            {
                startupWindow.Show();
                var viewModel = CreateNewMainWindowViewModel();
                viewModel.OnStartup(e.Args);
                var mainWindow = CreateNewMainWindow(viewModel);
                base.OnStartup(e);
                mainWindow.Show();
            }
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
        ///     Loads all plugin <see cref="Assembly" /> instances and returns the created <see cref="List{T}" />
        /// </summary>
        /// <param name="sourceDirectory"></param>
        /// <returns></returns>
        public List<Assembly> LoadPlugins(string sourceDirectory)
        {
            return null;
        }

        /// <summary>
        ///     Action to be called if an unhandled exception event occurs in the <see cref="AppDomain" />
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            if (!args.IsTerminating) return;
            var time = $"{DateTime.Now:yyyyMMddhmmss}";
            var logPath = Environment.ExpandEnvironmentVariables($"{GUI.Properties.Resources.Folder_Userprofile_Resources}Crash.{time}.log");
            var baseMessage = GUI.Properties.Resources.Error_Uncaught_Exception_Message;
            var caption = GUI.Properties.Resources.Error_Uncaught_Exception_Caption;
            var exception = args.ExceptionObject as Exception ?? new Exception($"Non exception object thrown {args.ExceptionObject}.");
            MessageBox.Show($"{baseMessage}\n ({logPath})", caption, MessageBoxButton.OK, MessageBoxImage.Error);

            File.WriteAllText(logPath, $"{sender}\n\n{exception}");

            #if (!DEBUG)
            Process.GetCurrentProcess().Kill();
            #endif
        }
    }
}