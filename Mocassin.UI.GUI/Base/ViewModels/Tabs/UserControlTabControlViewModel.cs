using System;
using System.Windows.Controls;
using Mocassin.UI.GUI.Base.ViewModels.Collections;

namespace Mocassin.UI.GUI.Base.ViewModels.Tabs
{
    /// <summary>
    ///     Base <see cref="ViewModelBase" /> for providing sets of <see cref="System.Windows.Controls.UserControl" /> through a
    ///     <see cref="System.Windows.Controls.TabControl" />
    /// </summary>
    public class UserControlTabControlViewModel : ObservableCollectionViewModel<UserControlTabItem>, IUserControlTabHost
    {
        private UserControlTabItem selectedTab;
        private int headerFontSize = 14;
        private Dock tabStripPlacement = Dock.Top;
        private bool isFrontInsertMode;

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
        public bool IsFrontInsertMode
        {
            get => isFrontInsertMode;
            set => SetProperty(ref isFrontInsertMode, value);
        }

        /// <inheritdoc />
        public void RemoveAndDispose(UserControlTabItem tabItem)
        {
            var index = ObservableItems.IndexOf(tabItem);
            if (index < 0) throw new InvalidOperationException("Tab does not belong to this host.");
            ExecuteOnAppThread(() =>
            {
                if (ReferenceEquals(tabItem, SelectedTab)) SetActiveTabByIndex(index == 0 ? 1 : index - 1);
                ObservableItems.Remove(tabItem);
                tabItem.Dispose();
            });
        }

        /// <inheritdoc />
        public void MoveTab(int oldIndex, int newIndex)
        {
            MoveItem(oldIndex, newIndex);
        }

        /// <inheritdoc />
        public void AddCloseableTab(string tabName, ViewModelBase viewModelBase, UserControl userControl, bool selectTab = true)
        {
            var tabItem = new CloseableUserControlTabItem(tabName, viewModelBase, userControl, this);
            if (IsFrontInsertMode) 
                InsertItem(0, tabItem);
            else
                AddItem(tabItem);
            if (!selectTab) return;
            SelectedTab = tabItem;
        }

        /// <inheritdoc />
        public void AddNonClosableTab(string tabName, ViewModelBase viewModelBase, UserControl userControl, bool selectTab = true)
        {
            var tabItem = new UserControlTabItem(tabName, viewModelBase, userControl);
            if (IsFrontInsertMode) 
                InsertItem(0, tabItem);
            else
                AddItem(tabItem);
            if (!selectTab) return;
            SelectedTab = tabItem;
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
            if (ObservableItems.Count == 0)
            {
                SelectedTab = null;
                return;
            }
            if (index < 0)
            {
                SelectedTab = ObservableItems[ObservableItems.Count - 1];
                return;
            }
            if (index == ObservableItems.Count)
            {
                SelectedTab = ObservableItems[index-1];
                return;
            }

            SelectedTab = ObservableItems[index];
        }
    }
}