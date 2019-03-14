using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Mocassin.Framework.Events;
using Mocassin.Framework.Messaging;
using Mocassin.Framework.SQLiteCore;
using Mocassin.Framework.Xml;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.ViewModels;
using Mocassin.UI.GUI.Base.ViewModels.JsonBrowser;
using Mocassin.UI.GUI.Controls.ProjectBrowser;
using Mocassin.UI.GUI.Controls.ProjectConsole;
using Mocassin.UI.GUI.Controls.ProjectMenuBar;
using Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.ProjectManager;
using Mocassin.UI.GUI.Controls.ProjectStatusBar;
using Mocassin.UI.GUI.Controls.ProjectWorkControl;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.Model;
using Mocassin.UI.Xml.ProjectLibrary;

namespace Mocassin.UI.GUI
{
    /// <summary>
    ///     <see cref="ViewModel" /> for the main window of the Mocassin GUI
    /// </summary>
    public class MainWindowViewModel : ViewModel, IMocassinProjectControl
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
        ///     The <see cref="ProjectGraphs"/> backing field
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
        public void SetOpenProjectLibrary(IMocassinProjectLibrary projectLibrary)
        {
            OpenProjectLibrary?.Dispose();
            ProjectGraphs = projectLibrary?.MocassinProjectGraphs.Local.ToObservableCollection();
            OpenProjectLibrary = projectLibrary;
            ProjectLibraryChangedEvent.OnNext(projectLibrary);
        }

        /// <inheritdoc />
        public IMocassinProjectLibrary OpenProjectLibrary
        {
            get => openProjectLibrary;
            private set => SetProperty(ref openProjectLibrary, value);
        }

        /// <inheritdoc />
        public ObservableCollection<MocassinProjectGraph> ProjectGraphs
        {
            get => projectGraphs;
            set => SetProperty(ref projectGraphs, value);
        }

        /// <inheritdoc />
        public IObservable<IMocassinProjectLibrary> ProjectLibraryChangeNotification => ProjectLibraryChangedEvent.AsObservable();

        /// <summary>
        ///     Creates new <see cref="MainWindowViewModel" /> for the Mocassin GUI
        /// </summary>
        public MainWindowViewModel()
        {
            ProjectLibraryChangedEvent = new ReactiveEvent<IMocassinProjectLibrary>();
            PushMessageSystem = new AsyncMessageSystem();
            ProjectMenuBarViewModel = new ProjectMenuBarViewModel(this);
            ProjectStatusBarViewModel = new ProjectStatusBarViewModel(this);
            ProjectBrowserViewModel = new ProjectBrowserViewModel(this);
            ProjectConsoleTabControlViewModel = new ProjectConsoleTabControlViewModel(this);
            ProjectWorkTabControlViewModel = new ProjectWorkTabControlViewModel(this);
            ProjectManagerViewModel = new ProjectManagerViewModel(this);
        }
    }
}