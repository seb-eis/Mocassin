using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using Mocassin.Framework.Messaging;
using Mocassin.Model.ModelProject;
using Mocassin.UI.GUI.Controls.ProjectBrowser;
using Mocassin.UI.GUI.Controls.ProjectConsole;
using Mocassin.UI.GUI.Controls.ProjectMenuBar;
using Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.ProjectManager;
using Mocassin.UI.GUI.Controls.ProjectStatusBar;
using Mocassin.UI.GUI.Controls.ProjectWorkControl;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.ProjectLibrary;

namespace Mocassin.UI.GUI.Base.DataContext
{
    /// <summary>
    ///     General interface for access to the controls of the currently opened <see cref="MainWindow" />
    /// </summary>
    public interface IMocassinProjectControl : INotifyPropertyChanged
    {
        /// <summary>
        ///     Get the window description <see cref="string" />
        /// </summary>
        string WindowDescription { get; }

        /// <summary>
        ///     Get an <see cref="IObservable{T}" /> that informs about replacement of the open
        ///     <see cref="IMocassinProjectLibrary" />
        /// </summary>
        IObservable<IMocassinProjectLibrary> ProjectLibraryChangeNotification { get; }

        /// <summary>
        ///     Get the currently loaded work <see cref="IMocassinProjectLibrary" />
        /// </summary>
        IMocassinProjectLibrary OpenProjectLibrary { get; }

        /// <summary>
        ///     Get a <see cref="IModelProject" /> that does not support input but provides the internal services
        /// </summary>
        IModelProject ServiceModelProject { get; }

        /// <summary>
        ///     Get an <see cref="ObservableCollection{T}" /> of <see cref="MocassinProject" /> that stays in sync with the
        ///     loaded <see cref="IMocassinProjectLibrary" />
        /// </summary>
        ObservableCollection<MocassinProject> ProjectGraphs { get; }

        /// <summary>
        ///     Get or set the main <see cref="PushMessageSystem" />
        /// </summary>
        IPushMessageSystem PushMessageSystem { get; }

        /// <summary>
        ///     Get the <see cref="Mocassin.UI.GUI.Controls.ProjectMenuBar.ProjectMenuBarViewModel" /> that controls the primary
        ///     menu bar
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
        ///     Get the <see cref="Mocassin.UI.GUI.Controls.ProjectWorkControl.ProjectWorkTabControlViewModel" /> that controls the
        ///     work tab selection
        /// </summary>
        ProjectWorkTabControlViewModel ProjectWorkTabControlViewModel { get; }

        /// <summary>
        ///     Get the <see cref="Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.ProjectManager.ProjectManagerViewModel" />
        ///     that controls project file operations
        /// </summary>
        ProjectManagerViewModel ProjectManagerViewModel { get; }

        /// <summary>
        ///     Get a <see cref="IList{T}" /> of loaded plugin <see cref="Assembly" /> instances
        /// </summary>
        /// <returns></returns>
        IEnumerable<Assembly> PluginAssemblies { get; }

        /// <summary>
        ///     Sets a new <see cref="IMocassinProjectLibrary" /> as the open library
        /// </summary>
        /// <param name="projectLibrary"></param>
        void ChangeOpenProjectLibrary(IMocassinProjectLibrary projectLibrary);

        /// <summary>
        ///     Creates a new <see cref="IModelProject" /> interface with all internally defined settings and capabilities
        /// </summary>
        /// <returns></returns>
        IModelProject CreateModelProject();

        /// <summary>
        ///     Stops and disposes all services of the project control to prepare for application shutdown
        /// </summary>
        void DisposeServices();

        /// <summary>
        ///     Stops all running project services
        /// </summary>
        void StopServices();

        /// <summary>
        ///     Starts all project services that are not already running
        /// </summary>
        void StartServices();

        /// <summary>
        ///     Executes the provided action on the application UI thread
        /// </summary>
        void ExecuteOnAppThread(Action action);
    }
}