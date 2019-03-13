using System.Collections.ObjectModel;
using System.Windows.Controls;
using Mocassin.UI.GUI.Base.ViewModels;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.ViewModels.Tabs;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.SubControls.ParticleModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl
{
    /// <summary>
    ///     The <see cref="PrimaryControlViewModel" /> for the <see cref="UserControlTabControlView" /> of work tabs
    /// </summary>
    public class ProjectWorkTabControlViewModel : PrimaryControlViewModel, IUserControlTabControlViewModel
    {
        /// <summary>
        ///     Get the <see cref="UserControlTabControlViewModel" /> that controls the project control tabs
        /// </summary>
        private UserControlTabControlViewModel TabControlViewModel { get; }

        /// <inheritdoc />
        public ProjectWorkTabControlViewModel(IMocassinProjectControl mainProjectControl)
            : base(mainProjectControl)
        {
            TabControlViewModel = new UserControlTabControlViewModel();
            InitializeDefaultTabs();
        }

        /// <inheritdoc />
        public ObservableCollection<UserControlTabItem> ObservableItems => TabControlViewModel.ObservableItems;


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
            TabControlViewModel.AddNonClosableTab("Particle Control", new ParticleModelControlViewModel(MainProjectControl), new ParticleModelControlView());
        }
    }
}