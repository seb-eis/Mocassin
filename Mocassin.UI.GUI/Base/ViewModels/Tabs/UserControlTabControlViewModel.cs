using System.Windows.Controls;
using Mocassin.UI.GUI.Base.ViewModels.Collections;

namespace Mocassin.UI.GUI.Base.ViewModels.Tabs
{
    /// <summary>
    ///     Base <see cref="ViewModel" /> for providing sets of <see cref="System.Windows.Controls.UserControl" /> through a
    ///     <see cref="System.Windows.Controls.TabControl" />
    /// </summary>
    public class UserControlTabControlViewModel : ObservableCollectionViewModel<UserControlTabItem>, IUserControlTabControlViewModel
    {
        /// <summary>
        ///     The <see cref="SelectedTab" /> backing field
        /// </summary>
        private UserControlTabItem selectedTab;

        /// <summary>
        ///     The <see cref="HeaderFontSize" /> backing field
        /// </summary>
        private int headerFontSize = 14;

        /// <summary>
        ///     The <see cref="TabStripPlacement" /> backing field
        /// </summary>
        private Dock tabStripPlacement = Dock.Top;

        /// <inheritdoc />
        public Dock TabStripPlacement
        {
            get => tabStripPlacement;
            set => SetProperty(ref tabStripPlacement, value);
        }

        /// <inheritdoc />
        public UserControlTabItem SelectedTab
        {
            get => selectedTab;
            set => SetProperty(ref selectedTab, value);
        }

        /// <inheritdoc />
        public int HeaderFontSize
        {
            get => headerFontSize;
            set => SetProperty(ref headerFontSize, value);
        }

        /// <inheritdoc />
        public void AddCloseableTab(string tabName, ViewModel viewModel, UserControl userControl)
        {
            var tabItem = new CloseableUserControlTabItem(tabName, viewModel, userControl, this);
            AddCollectionItem(tabItem);
        }

        /// <inheritdoc />
        public void AddNonClosableTab(string tabName, ViewModel viewModel, UserControl userControl)
        {
            var tabItem = new UserControlTabItem(tabName, viewModel, userControl);
            AddCollectionItem(tabItem);
        }

        /// <inheritdoc />
        public virtual void InitializeDefaultTabs()
        {
        }

        /// <summary>
        ///     Sets the <see cref="SelectedTab" /> property to the item with the passed index. Negative values default to the last
        ///     item
        /// </summary>
        public void SetActiveTabByIndex(int index)
        {
            if (ObservableItems.Count == 0) return;
            if (index < 0)
            {
                SelectedTab = ObservableItems[ObservableItems.Count - 1];
                return;
            }

            SelectedTab = ObservableItems[index];
        }
    }
}