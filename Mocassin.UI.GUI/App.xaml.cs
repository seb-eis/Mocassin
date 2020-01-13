using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using Mocassin.UI.GUI.Properties;

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
            using var startupWindow = new StartupWindow();
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            startupWindow.Show();
            var viewModel = CreateNewMainWindowViewModel();
            viewModel.OnStartup(e.Args);
            var mainWindow = CreateNewMainWindow(viewModel);
            base.OnStartup(e);
            mainWindow.Show();
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
                DataContext = mainWindowViewModel, 
                Resources = {["App.Default.FontSize"] = Settings.Default.Default_FontSize}
            };
            return mainWindow;
        }

        /// <summary>
        ///     Loads all plugin <see cref="Assembly" /> instances and returns the created <see cref="List{T}" />
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="searchOption"></param>
        /// <returns></returns>
        public List<Assembly> LoadPlugins(string directoryPath, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            if (directoryPath == null || !Directory.Exists(directoryPath)) return null;
            var fileNames = Directory.GetFiles(directoryPath, "*.dll", searchOption);
            var result = new List<Assembly>(fileNames.Length);
            try
            {
                result.AddRange(fileNames.Select(Assembly.LoadFile));
                return result;
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error on plugin loading:\n{e}", "Loading - Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        /// <summary>
        ///     Action to be called if an unhandled exception event occurs in the <see cref="AppDomain" />
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            DebugRethrow(sender, args);
            LogUnhandledExceptionAndKillIfTerminating(sender, args);
        }

        /// <summary>
        ///     Logs the occurence of an uncaught <see cref="UnhandledExceptionEventArgs" /> and terminates the process if the
        ///     event is terminating
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void LogUnhandledExceptionAndKillIfTerminating(object sender, UnhandledExceptionEventArgs args)
        {
            var time = $"{DateTime.Now:yyyyMMddhmmss}";
            var baseMessage = GUI.Properties.Resources.Error_Uncaught_Exception_Message;
            var caption = GUI.Properties.Resources.Error_Uncaught_Exception_Caption;

            var logPath = Environment.ExpandEnvironmentVariables($"{GUI.Properties.Resources.Folder_Userprofile_Resources}Crash.{time}.log");
            var exception = args.ExceptionObject as Exception ?? new Exception($"Non exception object thrown {args.ExceptionObject}.");
            MessageBox.Show($"{baseMessage}\n ({logPath})", caption, MessageBoxButton.OK, MessageBoxImage.Error);

            File.WriteAllText(logPath, $"{sender}\n\n{exception}");
            if (args.IsTerminating) Process.GetCurrentProcess().Kill();
        }

        /// <summary>
        ///     Conditional method that rethrows an uncaught exception if the DEBUG flag is set
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        [Conditional("DEBUG")]
        private static void DebugRethrow(object sender, UnhandledExceptionEventArgs args)
        {
            throw (Exception) args.ExceptionObject;
        }
    }
}