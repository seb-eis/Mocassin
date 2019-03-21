using System.Windows.Controls;
using Mocassin.UI.GUI.Base.ViewModels.Collections;

namespace Mocassin.UI.GUI.Base.ViewModels.Tabs
{
    /// <summary>
    ///     Represents a <see cref="TabControl" /> model that provides collections of <see cref="UserControl" /> instances
    /// </summary>
    public interface IUserControlTabControlViewModel : IObservableCollectionViewModel<UserControlTabItem>
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
        ///     Adds a <see cref="ViewModelBase" /> and <see cref="UserControl" /> as a closable <see cref="UserControlTabItem" />
        /// </summary>
        /// <param name="tabName"></param>
        /// <param name="viewModelBase"></param>
        /// <param name="userControl"></param>
        void AddCloseableTab(string tabName, ViewModelBase viewModelBase, UserControl userControl);

        /// <summary>
        ///     Adds a <see cref="ViewModelBase" /> and <see cref="UserControl" /> as a non closable <see cref="UserControlTabItem" />
        /// </summary>
        /// <param name="tabName"></param>
        /// <param name="viewModelBase"></param>
        /// <param name="userControl"></param>
        void AddNonClosableTab(string tabName, ViewModelBase viewModelBase, UserControl userControl);

        /// <summary>
        ///     Initializes any default <see cref="UserControlTabItem" /> components
        /// </summary>
        void InitializeDefaultTabs();
    }
}