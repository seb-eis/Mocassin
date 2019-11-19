using System.Windows.Controls;
using Mocassin.UI.GUI.Base.ViewModels.Collections;

namespace Mocassin.UI.GUI.Base.ViewModels.Tabs
{
    /// <summary>
    ///     Base <see cref="ViewModelBase" /> for providing sets of <see cref="System.Windows.Controls.UserControl" /> through a
    ///     <see cref="System.Windows.Controls.TabControl" />
    /// </summary>
    public class UserControlTabControlViewModel : ObservableCollectionViewModel<UserControlTabItem>, IUserControlTabControlViewModel
    {
        private UserControlTabItem selectedTab;
        private int headerFontSize = 14;
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
        public void AddCloseableTab(string tabName, ViewModelBase viewModelBase, UserControl userControl)
        {
            var tabItem = new CloseableUserControlTabItem(tabName, viewModelBase, userControl, this);
            AddCollectionItem(tabItem);
        }

        /// <inheritdoc />
        public void AddNonClosableTab(string tabName, ViewModelBase viewModelBase, UserControl userControl)
        {
            var tabItem = new UserControlTabItem(tabName, viewModelBase, userControl);
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