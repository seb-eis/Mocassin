using System.Collections.ObjectModel;
using System.Windows.Controls;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.ViewModels;
using Mocassin.UI.GUI.Base.ViewModels.Tabs;
using Mocassin.UI.GUI.Controls.Base;
using Mocassin.UI.GUI.Controls.ProjectConsole.SubControls.MessageConsole;

namespace Mocassin.UI.GUI.Controls.ProjectConsole
{
    /// <summary>
    ///     TThe <see cref="ViewModel" /> for the main project console
    /// </summary>
    public class ProjectConsoleTabControlViewModel : PrimaryControlViewModel, IUserControlTabControlViewModel
    {
        /// <summary>
        ///     Get the <see cref="SubControls.MessageConsole.MessageConsoleViewModel" /> that controls the display of string
        ///     messages
        /// </summary>
        private MessageConsoleViewModel MessageConsoleViewModel { get; }

        /// <summary>
        ///     Get the <see cref="UserControlTabControlViewModel" /> that controls the tabs of the console control
        /// </summary>
        private UserControlTabControlViewModel TabControlViewModel { get; }

        /// <inheritdoc />
        public ObservableCollection<UserControlTabItem> ObservableItems => TabControlViewModel.ObservableItems;


        /// <inheritdoc />
        public ProjectConsoleTabControlViewModel(IMocassinProjectControl mainProjectControl)
            : base(mainProjectControl)
        {
            TabControlViewModel = new UserControlTabControlViewModel();
            MessageConsoleViewModel = new MessageConsoleViewModel(mainProjectControl);
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
            TabControlViewModel.AddNonClosableTab("Messages", MessageConsoleViewModel, new MessageConsoleView());
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