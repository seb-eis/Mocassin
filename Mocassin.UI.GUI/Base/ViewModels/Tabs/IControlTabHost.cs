using System.Windows.Controls;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.ViewModels.Collections;

namespace Mocassin.UI.GUI.Base.ViewModels.Tabs
{
    /// <summary>
    ///     Represents a <see cref="TabControl" /> host that provides collections of <see cref="UserControl" /> instances
    /// </summary>
    public interface IControlTabHost : IObservableCollectionViewModel<ControlTabItem>, IDataObjectAcceptor
    {
        /// <summary>
        ///     Get or set the currently selected <see cref="ControlTabItem" />
        /// </summary>
        ControlTabItem SelectedTab { get; set; }

        /// <summary>
        ///     Get or set the <see cref="Dock" /> for the tab strip position
        /// </summary>
        Dock TabStripPlacement { get; set; }

        /// <summary>
        ///     Get or set a boolean flag if new tabs should be inserted at the front instead of the back
        /// </summary>
        bool IsFrontInsertMode { get; set; }

        /// <summary>
        ///     Removes the tab affiliated with the provided <see cref="ControlTabItem" />
        /// </summary>
        /// <param name="tabItem"></param>
        void RemoveAndDispose(ControlTabItem tabItem);

        /// <summary>
        ///     Changes the index of a tab. If the second index is negative it defaults to the last entry
        /// </summary>
        /// <param name="oldIndex"></param>
        /// <param name="newIndex"></param>
        void MoveTab(int oldIndex, int newIndex);

        /// <summary>
        ///     Adds a <see cref="ViewModelBase" /> and <see cref="UserControl" /> as a closable <see cref="ControlTabItem" />
        /// </summary>
        /// <param name="tabName"></param>
        /// <param name="contentViewModel"></param>
        /// <param name="content"></param>
        /// <param name="selectTab"></param>
        void AddDynamicTab(string tabName, ViewModelBase contentViewModel, Control content, bool selectTab = true);

        /// <summary>
        ///     Adds a <see cref="ViewModelBase" /> and <see cref="UserControl" /> as a non closable <see cref="ControlTabItem" />
        /// </summary>
        /// <param name="tabName"></param>
        /// <param name="contentViewModel"></param>
        /// <param name="content"></param>
        /// <param name="selectTab"></param>
        void AddStaticTab(string tabName, ViewModelBase contentViewModel, Control content, bool selectTab = true);

        /// <summary>
        ///     Adds a new <see cref="ControlTabItem" /> to the host
        /// </summary>
        /// <param name="tabItem"></param>
        /// <param name="selectTab"></param>
        void AddTab(ControlTabItem tabItem, bool selectTab = true);

        /// <summary>
        ///     Removes a <see cref="ControlTabItem" /> from the host
        /// </summary>
        /// <param name="tabItem"></param>
        /// <returns></returns>
        void RemoveTab(ControlTabItem tabItem);

        /// <summary>
        ///     Initializes any default <see cref="ControlTabItem" /> components
        /// </summary>
        void InitializeDefaultTabs();

        /// <summary>
        ///     Disposes all tab items and clears the collection
        /// </summary>
        void DisposeAndClearItems();

        /// <summary>
        ///     Sets the <see cref="SelectedTab" /> property to the item with the passed index. Negative values should default to
        ///     the last item
        /// </summary>
        void SetActiveTabByIndex(int index);
    }
}