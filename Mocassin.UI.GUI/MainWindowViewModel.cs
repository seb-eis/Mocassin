using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;
using Mocassin.Framework.Events;
using Mocassin.Framework.Messaging;
using Mocassin.Model.DataManagement;
using Mocassin.Model.ModelProject;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectBrowser;
using Mocassin.UI.GUI.Controls.ProjectConsole;
using Mocassin.UI.GUI.Controls.ProjectMenuBar;
using Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.ProjectManager;
using Mocassin.UI.GUI.Controls.ProjectStatusBar;
using Mocassin.UI.GUI.Controls.ProjectWorkControl;
using Mocassin.UI.GUI.Logic.Updating;
using Mocassin.UI.GUI.Properties;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.ProjectLibrary;
using Application = System.Windows.Application;

namespace Mocassin.UI.GUI
{
    /// <summary>
    ///     <see cref="ViewModelBase" /> for the main window of the Mocassin GUI
    /// </summary>
    public class MainWindowViewModel : ViewModelBase, IMocassinProjectControl
    {
        private string windowDescription;

        /// <summary>
        ///     Get the <see cref="ReactiveEvent{TSubject}" /> for changes in the open <see cref="IMocassinProjectLibrary" />
        /// </summary>
        private ReactiveEvent<IMocassinProjectLibrary> ProjectLibraryChangedEvent { get; }

        /// <summary>
        ///     The <see cref="OpenProjectLibrary" /> backing field
        /// </summary>
        private IMocassinProjectLibrary openProjectLibrary;

        /// <summary>
        ///     The <see cref="ProjectGraphs" /> backing field
        /// </summary>
        private ObservableCollection<MocassinProjectGraph> projectGraphs;

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

        /// <summary>
        ///     Get or set the window description <see cref="string"/>
        /// </summary>
        public string WindowDescription
        {
            get => windowDescription;
            set => SetProperty(ref windowDescription, value);
        }

        /// <summary>
        ///     Get or set the <see cref="ProjectContentChangeTriggerViewModel"/> that periodically triggers the project content change
        /// </summary>
        public ProjectContentChangeTriggerViewModel ChangeTriggerViewModel { get; set; }

        /// <inheritdoc />
        public void ChangeOpenProjectLibrary(IMocassinProjectLibrary projectLibrary)
        {
            OpenProjectLibrary?.Dispose();
            ProjectGraphs = projectLibrary?.MocassinProjectGraphs.Local.ToObservableCollection();
            OpenProjectLibrary = projectLibrary;
            ProjectLibraryChangedEvent.OnNext(projectLibrary);
            ChangeTriggerViewModel = ChangeTriggerViewModel ?? new ProjectContentChangeTriggerViewModel(this);
            WindowDescription = MakeWindowDescription();
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
        public ObservableCollection<MocassinProjectGraph> ProjectGraphs
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
            ProjectLibraryChangedEvent = new ReactiveEvent<IMocassinProjectLibrary>();
            PushMessageSystem = new AsyncMessageSystem();
            ProjectMenuBarViewModel = new ProjectMenuBarViewModel(this);
            ProjectStatusBarViewModel = new ProjectStatusBarViewModel(this);
            ProjectBrowserViewModel = new ProjectBrowserViewModel(this);
            ProjectConsoleTabControlViewModel = new ProjectConsoleTabControlViewModel(this);
            ProjectWorkTabControlViewModel = new ProjectWorkTabControlViewModel(this);
            ProjectManagerViewModel = new ProjectManagerViewModel(this);

            EnsureResourcesCreated(false);
            ServiceModelProject = CreateServiceModelProject();
            WindowDescription = MakeWindowDescription();
        }

        /// <summary>
        ///     Creates <see cref="IModelProject"/> that supplies access to most model services with the custom project config.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Will return a default config project if the</remarks>
        public IModelProject CreateServiceModelProject()
        {
            return ModelProject.Create(LoadProjectSettings());
        }

        /// <inheritdoc />
        public IModelProject CreateModelProject()
        {
            var modelProject = ModelProject.Create(ServiceModelProject.Settings);
            foreach (var factory in GetModelManagerFactories()) modelProject.CreateAndRegister(factory);
            return modelProject;
        }

        /// <inheritdoc />
        public void DisposeServices()
        {
            ChangeTriggerViewModel?.Dispose();
        }

        /// <summary>
        ///     Get the <see cref="IEnumerable{T}"/> of known <see cref="IModelManagerFactory"/> that can be used to register services to <see cref="IModelProject"/> interface
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IModelManagerFactory> GetModelManagerFactories()
        {
            var factories = new List<IModelManagerFactory>
            {
                new ParticleManagerFactory(),
                new StructureManagerFactory(),
                new LatticeManagerFactory(),
                new TransitionManagerFactory(),
                new EnergyManagerFactory(),
                new SimulationManagerFactory()
            };
            return factories;
        }

        /// <summary>
        ///     Builds the window description string <see cref="string"/>
        /// </summary>
        /// <returns></returns>
        public string MakeWindowDescription()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            return $"[{version}]{(OpenProjectLibrary?.SourceName == null ? "" : $" - {OpenProjectLibrary.SourceName}")}";
        }

        /// <summary>
        ///     Loads the <see cref="ProjectSettings"/> from the file lookup location defined in App.xaml
        /// </summary>
        /// <returns></returns>
        public ProjectSettings LoadProjectSettings()
        {
            try
            {
                var filePath = GetFullResourceFilePath(Resources.Filename_Project_Default_Configuration);
                return ProjectSettings.Deserialize(filePath, PluginAssemblies);
            }
            catch (Exception)
            {
                MessageBox.Show("Your settings file is corrupt, restoring defaults.",
                    "Error - Settings",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                EnsureProjectConfigCreated(true);
                return ProjectSettings.CreateDefault();
            }
        }

        /// <summary>
        ///     Ensures that all application resources that can be modified are deployed into their target location. An optional overwrite flag can enforce recreation
        /// </summary>
        /// <param name="isOverwrite"></param>
        public void EnsureResourcesCreated(bool isOverwrite)
        {
            var dir = Directory.CreateDirectory(Environment.ExpandEnvironmentVariables(Resources.Folder_Userprofile_Resources));

            EnsureProjectConfigCreated(isOverwrite);
            EnsureSymmetryDatabaseCreated(isOverwrite);
        }

        /// <summary>
        ///     Ensures that the default symmetry database is deployed to the config directory with optional enforced overwrite
        /// </summary>
        /// <param name="isOverwrite"></param>
        public void EnsureSymmetryDatabaseCreated(bool isOverwrite)
        {
            var dbPath = GetFullResourceFilePath(Resources.Filename_Symmetry_Default_Database);
            if (!File.Exists(dbPath) || isOverwrite) File.WriteAllBytes(dbPath, Resources.Symmetry_Database_Default);
        }

        /// <summary>
        ///     Ensures that the default project settings config is deployed to the config directory with optional enforced overwrite
        /// </summary>
        /// <param name="isOverwrite"></param>
        public void EnsureProjectConfigCreated(bool isOverwrite)
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
    }
}