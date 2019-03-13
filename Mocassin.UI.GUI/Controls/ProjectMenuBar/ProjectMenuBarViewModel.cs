using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using Mocassin.UI.Base.Commands;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.ViewModels.MenuBar;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectMenuBar.Commands;
using Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.ProjectManager;
using Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.ProjectManager.Commands;

namespace Mocassin.UI.GUI.Controls.ProjectMenuBar
{
    /// <summary>
    ///     The <see cref="PrimaryControlViewModel" /> for the <see cref="DynamicMenuBarView" /> of the main menu
    /// </summary>
    public class ProjectMenuBarViewModel : PrimaryControlViewModel, IDynamicMenuBarViewModel
    {
        /// <summary>
        ///     Get the <see cref="DynamicMenuBarViewModel" /> that controls the additional menu items
        /// </summary>
        private DynamicMenuBarViewModel MenuBarViewModel { get; }

        /// <inheritdoc />
        public Dock DockPanelDock
        {
            get => MenuBarViewModel.DockPanelDock;
            set => MenuBarViewModel.DockPanelDock = value;
        }

        /// <inheritdoc />
        public int FontSize
        {
            get => MenuBarViewModel.FontSize;
            set => MenuBarViewModel.FontSize = value;
        }

        /// <inheritdoc />
        public ObservableCollection<MenuItem> ObservableItems => MenuBarViewModel.ObservableItems;

        /// <summary>
        ///     Get a <see cref="ICommand"/> to start the project loading dialog
        /// </summary>
        public ICommand ShowProjectLoadingDialogCommand { get; }

        /// <summary>
        ///     Get a <see cref="ICommand"/> to star the project creation dialog
        /// </summary>
        public ICommand ShowProjectCreationDialogCommand { get; }

        /// <summary>
        ///     Get a <see cref="ICommand"/> to safely exit the program with save changes check and cancel option
        /// </summary>
        public ICommand SaveExitProgramCommand { get; }

        /// <summary>
        ///     Get a <see cref="ICommand"/> to safely close the current project with save changes check and cancel option
        /// </summary>
        public ICommand SaveCloseProjectLibraryCommand { get; }

        /// <inheritdoc />
        public ProjectMenuBarViewModel(IMocassinProjectControl mainProjectControl)
            : base(mainProjectControl)
        {
            MenuBarViewModel = new DynamicMenuBarViewModel(Dock.Top);
            ShowProjectLoadingDialogCommand = new ShowProjectLoadingDialogCommand(mainProjectControl);
            ShowProjectCreationDialogCommand = new ShowProjectCreationDialogCommand(mainProjectControl);
            SaveExitProgramCommand = new SaveExitProgramCommand(mainProjectControl);
            SaveCloseProjectLibraryCommand = MakeCloseCommandRelay(mainProjectControl);
        }

        /// <inheritdoc />
        public void InsertCollectionItem(int index, MenuItem value)
        {
            MenuBarViewModel.InsertCollectionItem(index, value);
        }

        /// <inheritdoc />
        public void AddCollectionItem(MenuItem value)
        {
            MenuBarViewModel.AddCollectionItem(value);
        }

        /// <inheritdoc />
        public void RemoveCollectionItem(MenuItem value)
        {
            MenuBarViewModel.RemoveCollectionItem(value);
        }

        /// <inheritdoc />
        public bool CollectionContains(MenuItem value)
        {
            return MenuBarViewModel.CollectionContains(value);
        }

        /// <summary>
        ///     Creates a <see cref="RelayCommand"/> for closing the current project
        /// </summary>
        /// <param name="projectControl"></param>
        /// <returns></returns>
        private RelayCommand MakeCloseCommandRelay(IMocassinProjectControl projectControl)
        {
            if (projectControl == null) throw new ArgumentNullException(nameof(projectControl));
            return new RelayCommand(() => projectControl.ProjectManagerViewModel.CloseActiveProjectLibrary());
        }
    }
}