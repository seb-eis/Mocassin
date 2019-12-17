using System.Windows.Controls;
using Mocassin.UI.GUI.Base.ViewModels.Collections;

namespace Mocassin.UI.GUI.Base.ViewModels.Tabs
{
    /// <summary>
    ///     Represents a <see cref="TabControl" /> host that provides collections of <see cref="UserControl" /> instances
    /// </summary>
    public interface IUserControlTabHost : IObservableCollectionViewModel<UserControlTabItem>
    {
        /// <summary>
        ///     Get or set the currently selected <see cref="UserControlTabItem" />
        /// </summary>
        UserControlTabItem SelectedTab { get; set; }

        /// <summary>
        ///     Get or set the header font size
        /// </summary>
        int HeaderFontSize { get; set; }

        /// <summary>
        ///     Get or set the <see cref="Dock"/> for the tab strip position
        /// </summary>
        Dock TabStripPlacement { get; set; }

        /// <summary>
        ///     Removes the tab affiliated with the provided <see cref="UserControlTabItem"/>
        /// </summary>
        /// <param name="tabItem"></param>
        void RemoveAndDispose(UserControlTabItem tabItem);

        /// <summary>
        ///     Get or set a boolean flag if new tabs should be inserted at the front instead of the back
        /// </summary>
        bool IsFrontInsertMode { get; set; }

        /// <summary>
        ///     Changes the index of a tab. If the second index is negative it defaults to the last entry
        /// </summary>
        /// <param name="oldIndex"></param>
        /// <param name="newIndex"></param>
        void MoveTab(int oldIndex, int newIndex);

        /// <summary>
        ///     Adds a <see cref="ViewModelBase" /> and <see cref="UserControl" /> as a closable <see cref="UserControlTabItem" />
        /// </summary>
        /// <param name="tabName"></param>
        /// <param name="viewModelBase"></param>
        /// <param name="userControl"></param>
        /// <param name="selectTab"></param>
        void AddCloseableTab(string tabName, ViewModelBase viewModelBase, UserControl userControl, bool selectTab = true);

        /// <summary>
        ///     Adds a <see cref="ViewModelBase" /> and <see cref="UserControl" /> as a non closable <see cref="UserControlTabItem" />
        /// </summary>
        /// <param name="tabName"></param>
        /// <param name="viewModelBase"></param>
        /// <param name="userControl"></param>
        /// <param name="selectTab"></param>
        void AddNonClosableTab(string tabName, ViewModelBase viewModelBase, UserControl userControl, bool selectTab = true);

        /// <summary>
        ///     Initializes any default <see cref="UserControlTabItem" /> components
        /// </summary>
        void InitializeDefaultTabs();

        /// <summary>
        ///     Disposes all tab items and clears the collection
        /// </summary>
        void DisposeAndClearItems();

        /// <summary>
        ///     Sets the <see cref="SelectedTab" /> property to the item with the passed index. Negative values should default to the last item
        /// </summary>
        void SetActiveTabByIndex(int index);
    }
}