using System;
using System.Windows.Controls;
using Mocassin.Framework.Extensions;
using Mocassin.UI.GUI.Base.ViewModels.Collections;

namespace Mocassin.UI.GUI.Base.ViewModels.Tabs
{
    /// <summary>
    ///     Base <see cref="ViewModelBase" /> for providing sets of <see cref="System.Windows.Controls.UserControl" /> through a
    ///     <see cref="System.Windows.Controls.TabControl" />
    /// </summary>
    public class ControlTabHostViewModel : ObservableCollectionViewModel<ControlTabItem>, IControlTabHost
    {
        private ControlTabItem selectedTab;
        private Dock tabStripPlacement = Dock.Top;
        private bool isFrontInsertMode;

        /// <inheritdoc />
        public Dock TabStripPlacement
        {
            get => tabStripPlacement;
            set => SetProperty(ref tabStripPlacement, value);
        }

        /// <inheritdoc />
        public ControlTabItem SelectedTab
        {
            get => selectedTab;
            set => SetProperty(ref selectedTab, value);
        }

        /// <inheritdoc />
        public bool IsFrontInsertMode
        {
            get => isFrontInsertMode;
            set => SetProperty(ref isFrontInsertMode, value);
        }

        /// <inheritdoc />
        public void RemoveAndDispose(ControlTabItem tabItem)
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
        public void AddDynamicTab(string tabName, ViewModelBase contentViewModel, Control content, bool selectTab = true)
        {
            AddTab(new DynamicControlTabItem(tabName, contentViewModel, content, this));
        }

        /// <inheritdoc />
        public void AddStaticTab(string tabName, ViewModelBase contentViewModel, Control content, bool selectTab = true)
        {
            AddTab(new ControlTabItem(tabName, contentViewModel, content));
        }

        /// <inheritdoc />
        public void AddTab(ControlTabItem tabItem, bool selectTab = true)
        {
            if (Contains(tabItem))
            {
                if (selectTab) SelectedTab = tabItem;
                return;
            }

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

        /// <inheritdoc />
        public void DisposeAndClearItems()
        {
            var items = ObservableItems.ToList(ObservableItems.Count);
            ExecuteOnAppThread(() =>
            {
                SelectedTab = null;
                Clear();
                foreach (var item in items) item.Dispose();
            });
            items.Clear();
        }

        /// <inheritdoc />
        public void SetActiveTabByIndex(int index)
        {
            ExecuteOnAppThread(() => SetActiveTabByIndexInternal(index));
        }

        /// <summary>
        ///     Internal implementation of <see cref="SetActiveTabByIndex"/>
        /// </summary>
        /// <param name="index"></param>
        private void SetActiveTabByIndexInternal(int index)
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