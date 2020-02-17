using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Deployment.Application;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Mocassin.Framework.Events;
using Mocassin.Framework.Messaging;
using Mocassin.Model.DataManagement;
using Mocassin.Model.ModelProject;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.ViewModels;
using Mocassin.UI.GUI.Controls.Base.IO;
using Mocassin.UI.GUI.Controls.ProjectBrowser;
using Mocassin.UI.GUI.Controls.ProjectConsole;
using Mocassin.UI.GUI.Controls.ProjectMenuBar;
using Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.ProjectManager;
using Mocassin.UI.GUI.Controls.ProjectStatusBar;
using Mocassin.UI.GUI.Controls.ProjectWorkControl;
using Mocassin.UI.GUI.Properties;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.ProjectLibrary;

namespace Mocassin.UI.GUI
{
    /// <summary>
    ///     <see cref="ViewModelBase" /> for the main window of the Mocassin GUI
    /// </summary>
    public class MainWindowViewModel : ViewModelBase, IMocassinProjectControl
    {
        /// <summary>
        ///     The <see cref="OpenProjectLibrary" /> backing field
        /// </summary>
        private IMocassinProjectLibrary openProjectLibrary;

        /// <summary>
        ///     The <see cref="ProjectGraphs" /> backing field
        /// </summary>
        private ObservableCollection<MocassinProject> projectGraphs;

        private string windowDescription;

        /// <summary>
        ///     Get the <see cref="ReactiveEvent{TSubject}" /> for changes in the open <see cref="IMocassinProjectLibrary" />
        /// </summary>
        private ReactiveEvent<IMocassinProjectLibrary> ProjectLibraryChangedEvent { get; }

        /// <inheritdoc />
        public IPushMessageSystem PushMessageSystem { get; set; }

        /// <inheritdoc />
        public ProjectMenuBarViewModel ProjectMenuBarViewModel { get; }

        /// <inheritdoc />
        public ProjectStatusBarViewModel ProjectStatusBarViewModel { get; }

        /// <inheritdoc />
        public ProjectBrowserViewModel ProjectBrowserViewModel { get; }

        /// <inheritdoc />
        public ProjectConsoleTabControlViewModel ProjectConsoleTabControlViewModel { get; }

        /// <inheritdoc />
        public ProjectWorkTabControlViewModel ProjectWorkTabControlViewModel { get; }

        /// <inheritdoc />
        public ProjectManagerViewModel ProjectManagerViewModel { get; }

        /// <inheritdoc />
        public IEnumerable<Assembly> PluginAssemblies { get; }

        /// <inheritdoc />
        public string WindowDescription
        {
            get => windowDescription;
            set => SetProperty(ref windowDescription, value);
        }

        /// <inheritdoc />
        public IMocassinProjectLibrary OpenProjectLibrary
        {
            get => openProjectLibrary;
            private set => SetProperty(ref openProjectLibrary, value);
        }

        /// <inheritdoc />
        public IModelProject ServiceModelProject { get; }

        /// <inheritdoc />
        public ObservableCollection<MocassinProject> ProjectGraphs
        {
            get => projectGraphs;
            set => SetProperty(ref projectGraphs, value);
        }

        /// <inheritdoc />
        public IObservable<IMocassinProjectLibrary> ProjectLibraryChangeNotification => ProjectLibraryChangedEvent.AsObservable();

        /// <summary>
        ///     Creates new <see cref="MainWindowViewModel" /> for the Mocassin GUI with the provided set of plugin
        ///     <see cref="Assembly" />
        /// </summary>
        public MainWindowViewModel(IEnumerable<Assembly> pluginAssemblies)
        {
            PluginAssemblies = pluginAssemblies ?? new Assembly[0];

            EnsureResourcesDeployed(false);
            ServiceModelProject = CreateServiceModelProject();
            WindowDescription = MakeWindowDescription();

            ProjectLibraryChangedEvent = new ReactiveEvent<IMocassinProjectLibrary>();
            PushMessageSystem = new AsyncMessageSystem();
            ProjectMenuBarViewModel = new ProjectMenuBarViewModel(this);
            ProjectStatusBarViewModel = new ProjectStatusBarViewModel(this);
            ProjectBrowserViewModel = new ProjectBrowserViewModel(this);
            ProjectConsoleTabControlViewModel = new ProjectConsoleTabControlViewModel(this);
            ProjectWorkTabControlViewModel = new ProjectWorkTabControlViewModel(this);
            ProjectManagerViewModel = new ProjectManagerViewModel(this);
        }

        /// <inheritdoc />
        public void ChangeOpenProjectLibrary(IMocassinProjectLibrary projectLibrary)
        {
            StopServices();
            OpenProjectLibrary?.Dispose();
            ProjectGraphs = projectLibrary?.MocassinProjectGraphs.Local.ToObservableCollection();
            OpenProjectLibrary = projectLibrary;
            ProjectLibraryChangedEvent.OnNext(projectLibrary);
            WindowDescription = MakeWindowDescription();
            StartServices();
        }

        /// <inheritdoc />
        public IModelProject CreateModelProject()
        {
            return ModelProjectFactory.Create(ServiceModelProject.Settings);
        }

        /// <inheritdoc />
        public void DisposeServices()
        {
        }

        /// <inheritdoc />
        public void StopServices()
        {
        }

        /// <inheritdoc />
        public void StartServices()
        {
        }

        /// <summary>
        ///     Creates <see cref="IModelProject" /> that supplies access to most model services with the custom project config.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Will return a default config project without any attached model managers</remarks>
        public IModelProject CreateServiceModelProject()
        {
            return ModelProject.Create(LoadProjectSettings());
        }

        /// <summary>
        ///     Builds the window description string <see cref="string" />
        /// </summary>
        /// <returns></returns>
        public string MakeWindowDescription()
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                var currentVersion = ApplicationDeployment.CurrentDeployment.CurrentVersion;
                return $"[{currentVersion}]{(OpenProjectLibrary?.SourceName == null ? "" : $" - {OpenProjectLibrary.SourceName}")}";
            }

            var version = Assembly.GetExecutingAssembly().GetName().Version;
            return $"[{version}, Non-Deploy]{(OpenProjectLibrary?.SourceName == null ? "" : $" - {OpenProjectLibrary.SourceName}")}";
        }

        /// <summary>
        ///     Loads the <see cref="ProjectSettings" /> from the file lookup location defined in App.xaml
        /// </summary>
        /// <returns></returns>
        public ProjectSettings LoadProjectSettings()
        {
            try
            {
                var filePath = GetFullResourceFilePath(Resources.Filename_Project_Default_Configuration);
                return ProjectSettings.Deserialize(filePath, PluginAssemblies);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                MessageBox.Show("Project settings are corrupt, defaults will be restored.", "Error - Settings", MessageBoxButtons.OK, MessageBoxIcon.Error);

                EnsureProjectConfigDeployed(true);
                return ProjectSettings.CreateDefault();
            }
        }

        /// <summary>
        ///     Ensures that all application resources that can be modified are deployed into their target location. An optional
        ///     overwrite flag can enforce recreation
        /// </summary>
        /// <param name="isOverwrite"></param>
        public void EnsureResourcesDeployed(bool isOverwrite)
        {
            Directory.CreateDirectory(Environment.ExpandEnvironmentVariables(Resources.Folder_Userprofile_Resources));
            EnsureProjectConfigDeployed(isOverwrite);
            EnsureSymmetryDatabaseDeployed(isOverwrite);
        }

        /// <summary>
        ///     Ensures that the default symmetry database is deployed to the config directory with optional enforced overwrite
        /// </summary>
        /// <param name="isOverwrite"></param>
        public void EnsureSymmetryDatabaseDeployed(bool isOverwrite)
        {
            var dbPath = GetFullResourceFilePath(Resources.Filename_Symmetry_Default_Database);
            if (!File.Exists(dbPath) || isOverwrite) File.WriteAllBytes(dbPath, Resources.Symmetry_Database_Default);
        }

        /// <summary>
        ///     Ensures that the default project settings config is deployed to the config directory with optional enforced
        ///     overwrite
        /// </summary>
        /// <param name="isOverwrite"></param>
        public void EnsureProjectConfigDeployed(bool isOverwrite)
        {
            var dbPath = GetFullResourceFilePath(Resources.Filename_Symmetry_Default_Database);
            var configPath = GetFullResourceFilePath(Resources.Filename_Project_Default_Configuration);
            var projectSettings = ProjectSettings.CreateDefault();
            projectSettings.SymmetrySettings.SpaceGroupDbPath = dbPath;
            var settingsXml = projectSettings.Serialize(PluginAssemblies);

            if (!File.Exists(configPath) || isOverwrite) File.WriteAllText(configPath, settingsXml);
        }

        /// <summary>
        ///     Get the full resource file path string for the passed file name
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string GetFullResourceFilePath(string fileName)
        {
            return Environment.ExpandEnvironmentVariables(Resources.Folder_Userprofile_Resources + fileName);
        }

        /// <summary>
        ///     Handles the app startup program arguments, if the they are empty the system tries to get the data from the
        ///     <see cref="AppDomain" /> activation arguments
        /// </summary>
        /// <param name="args"></param>
        public void OnStartup(string[] args)
        {
            var fileSelectionSource = UserFileSelectionSource.CreateForProjectFiles(false);
            if (args.Length == 0 && ApplicationDeployment.IsNetworkDeployed)
            {
                var activationData = AppDomain.CurrentDomain.SetupInformation.ActivationArguments?.ActivationData;
                if (activationData != null) args = activationData;
            }

            foreach (var item in args)
            {
                if (!fileSelectionSource.SupportedFileTypes.Any(x => $".{x.Extension}".Equals(Path.GetExtension(item)))) continue;
                ProjectManagerViewModel.LoadActiveProjectLibrary(item);
                return;
            }
        }
    }
}