using System.Collections.ObjectModel;
using System.Windows.Controls;
using Mocassin.UI.GUI.Base.ViewModels;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.ViewModels.Tabs;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectBrowser.SubControls.LibraryBrowser;
using Mocassin.UI.GUI.Controls.ProjectBrowser.SubControls.ReportBrowser;

namespace Mocassin.UI.GUI.Controls.ProjectBrowser
{
    /// <summary>
    ///     The <see cref="PrimaryControlViewModel" /> that controls project browser tabs
    /// </summary>
    public class ProjectBrowserViewModel : PrimaryControlViewModel, IUserControlTabControlViewModel
    {
        /// <summary>
        ///     Get the <see cref="ProjectLibraryBrowserViewModel" /> that controls model data browsing
        /// </summary>
        private ProjectLibraryBrowserViewModel ProjectLibraryBrowserViewModel { get; }

        /// <summary>
        ///     Get the <see cref="ReportBrowserViewModel" /> that controls report browsing
        /// </summary>
        private ReportBrowserViewModel ReportBrowserViewModel { get; }

        /// <summary>
        ///     Get the <see cref="UserControlTabControlViewModel" /> that controls the browser tabs
        /// </summary>
        private UserControlTabControlViewModel TabControlViewModel { get; }

        /// <inheritdoc />
        public ProjectBrowserViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            ProjectLibraryBrowserViewModel = new ProjectLibraryBrowserViewModel(projectControl);
            ReportBrowserViewModel = new ReportBrowserViewModel(projectControl);
            TabControlViewModel = new UserControlTabControlViewModel();
            InitializeDefaultTabs();
        }

        /// <inheritdoc />
        public UserControlTabItem SelectedTab
        {
            get => TabControlViewModel.SelectedTab;
            set => TabControlViewModel.SelectedTab = value;
        }

        /// <inheritdoc />
        public int HeaderFontSize
        {
            get => TabControlViewModel.HeaderFontSize;
            set => TabControlViewModel.HeaderFontSize = value;
        }

        /// <inheritdoc />
        public Dock TabStripPlacement
        {
            get => TabControlViewModel.TabStripPlacement;
            set => TabControlViewModel.TabStripPlacement = value;
        }

        /// <inheritdoc />
        public ObservableCollection<UserControlTabItem> ObservableItems => TabControlViewModel.ObservableItems;

        /// <inheritdoc />
        public void AddCloseableTab(string tabName, ViewModel viewModel, UserControl userControl)
        {
            TabControlViewModel.AddCloseableTab(tabName, viewModel, userControl);
        }

        /// <inheritdoc />
        public void AddNonClosableTab(string tabName, ViewModel viewModel, UserControl userControl)
        {
            TabControlViewModel.AddNonClosableTab(tabName, viewModel, userControl);
        }

        /// <inheritdoc />
        public void InitializeDefaultTabs()
        {
            TabControlViewModel.InitializeDefaultTabs();
            TabControlViewModel.AddNonClosableTab("Project Library", ProjectLibraryBrowserViewModel, new ProjectLibraryBrowserView());
            TabControlViewModel.AddNonClosableTab("Project Reports", ReportBrowserViewModel, new ReportBrowserView());
            TabControlViewModel.SetActiveTabByIndex(0);
        }

        /// <inheritdoc />
        public void InsertCollectionItem(int index, UserControlTabItem value)
        {
            TabControlViewModel.InsertCollectionItem(index, value);
        }

        /// <inheritdoc />
        public void AddCollectionItem(UserControlTabItem value)
        {
            TabControlViewModel.AddCollectionItem(value);
        }

        /// <inheritdoc />
        public void RemoveCollectionItem(UserControlTabItem value)
        {
            TabControlViewModel.RemoveCollectionItem(value);
        }

        /// <inheritdoc />
        public bool CollectionContains(UserControlTabItem value)
        {
            return TabControlViewModel.CollectionContains(value);
        }
    }
}