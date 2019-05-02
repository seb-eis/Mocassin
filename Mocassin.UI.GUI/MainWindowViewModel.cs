using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
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
        ///     Get or set the <see cref="ProjectContentChangeTriggerViewModel"/> that periodically triggers the project content change
        /// </summary>
        public ProjectContentChangeTriggerViewModel ChangeTriggerViewModel { get; set; }

        /// <inheritdoc />
        public void SetOpenProjectLibrary(IMocassinProjectLibrary projectLibrary)
        {
            OpenProjectLibrary?.Dispose();
            ProjectGraphs = projectLibrary?.MocassinProjectGraphs.Local.ToObservableCollection();
            OpenProjectLibrary = projectLibrary;
            ProjectLibraryChangedEvent.OnNext(projectLibrary);
            ChangeTriggerViewModel = ChangeTriggerViewModel ?? new ProjectContentChangeTriggerViewModel(this);
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
            ServiceModelProject = CreateServiceModelProject();
        }

        /// <summary>
        ///     Creates <see cref="IModelProject"/> that supplies access to most model services
        /// </summary>
        /// <returns></returns>
        public IModelProject CreateServiceModelProject()
        {
            return ModelProject.Create(ProjectSettings.CreateDefault());
        }

        /// <inheritdoc />
        public IModelProject CreateModelProject()
        {
            var modelProject = CreateServiceModelProject();
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
                new TransitionManagerFactory(),
                new EnergyManagerFactory(),
                new SimulationManagerFactory()
            };
            return factories;
        }
    }
}