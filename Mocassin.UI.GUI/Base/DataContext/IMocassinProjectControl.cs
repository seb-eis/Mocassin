using System;
using System.ComponentModel;
using Mocassin.Framework.Messaging;
using Mocassin.UI.GUI.Controls.ProjectBrowser;
using Mocassin.UI.GUI.Controls.ProjectConsole;
using Mocassin.UI.GUI.Controls.ProjectMenuBar;
using Mocassin.UI.GUI.Controls.ProjectStatusBar;
using Mocassin.UI.GUI.Controls.ProjectWorkControl;
using ProjectWorkControl;
using Mocassin.UI.Xml.ProjectLibrary;

namespace Mocassin.UI.GUI.Base.DataContext
{
    /// <summary>
    ///     General interface for access to the controls of the currently opened <see cref="MainWindow" />
    /// </summary>
    public interface IMocassinProjectControl : INotifyPropertyChanged
    {
        /// <summary>
        ///     Get an <see cref="IObservable{T}" /> notifier for changes in the <see cref="OpenProjectLibrary" />
        /// </summary>
        IObservable<IMocassinProjectLibrary> LibraryChangeNotification { get; }

        /// <summary>
        ///     Get the currently loaded work <see cref="IMocassinProjectLibrary" />
        /// </summary>
        IMocassinProjectLibrary OpenProjectLibrary { get; }

        /// <summary>
        ///     Get or set the main <see cref="PushMessageSystem" />
        /// </summary>
        IPushMessageSystem PushMessageSystem { get; set; }

        /// <summary>
        ///     Get the <see cref="ProjectMenuBarViewModel" /> that controls the primary menu bar
        /// </summary>
        ProjectMenuBarViewModel ProjectMenuBarViewModel { get; }

        /// <summary>
        ///     Get the <see cref="Mocassin.UI.GUI.Controls.ProjectStatusBar.ProjectStatusBarViewModel" /> that controls the
        ///     primary status bar
        /// </summary>
        ProjectStatusBarViewModel ProjectStatusBarViewModel { get; }

        /// <summary>
        ///     Get the <see cref="Mocassin.UI.GUI.Controls.ProjectBrowser.ProjectBrowserViewModel" /> that controls the project
        ///     data browser
        /// </summary>
        ProjectBrowserViewModel ProjectBrowserViewModel { get; }

        /// <summary>
        ///     Get the <see cref="Mocassin.UI.GUI.Controls.ProjectConsole.ProjectConsoleTabControlViewModel" /> that controls the
        ///     text console
        /// </summary>
        ProjectConsoleTabControlViewModel ProjectConsoleTabControlViewModel { get; }

        /// <summary>
        ///     Get the <see cref="ProjectWorkControl.ProjectWorkTabControlViewModel" /> that controls the work
        ///     tab selection
        /// </summary>
        ProjectWorkTabControlViewModel ProjectWorkTabControlViewModel { get; }
    }
}