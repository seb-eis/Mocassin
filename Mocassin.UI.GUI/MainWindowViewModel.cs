using System;
using Mocassin.Framework.Events;
using Mocassin.Framework.Messaging;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectBrowser;
using Mocassin.UI.GUI.Controls.ProjectConsole;
using Mocassin.UI.GUI.Controls.ProjectMenuBar;
using Mocassin.UI.GUI.Controls.ProjectStatusBar;
using Mocassin.UI.GUI.Controls.ProjectWorkControl;
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
        public IMocassinProjectLibrary OpenProjectLibrary
        {
            get => openProjectLibrary;
            set
            {
                SetProperty(ref openProjectLibrary, value);
                ProjectLibraryChangedEvent.OnNext(value);
            }
        }

        /// <inheritdoc />
        public IObservable<IMocassinProjectLibrary> LibraryChangeNotification => ProjectLibraryChangedEvent.AsObservable();

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
        }
    }
}