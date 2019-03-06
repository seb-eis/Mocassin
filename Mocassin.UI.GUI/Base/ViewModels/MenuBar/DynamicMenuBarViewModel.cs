using System.Collections.Generic;
using System.Windows.Controls;
using Mocassin.UI.GUI.Base.ViewModels.Collections;

namespace Mocassin.UI.Base.ViewModels.MenuBar
{
    /// <summary>
    ///     Base class for <see cref="ViewModel" /> implementations of observable <see cref="MenuItem" /> collections
    /// </summary>
    public abstract class DynamicMenuBarViewModel : ObservableCollectionViewModel<MenuItem>
    {
        /// <summary>
        ///     The <see cref="DockPanelDock" /> backing field
        /// </summary>
        private Dock dockPanelDock = Dock.Top;

        /// <summary>
        ///     The <see cref="FontSize" /> backing field
        /// </summary>
        private int fontSize = 14;

        /// <summary>
        ///     Get or set the <see cref="System.Windows.Controls.Dock" /> value of the menu bar <see cref="DockPanel" />
        /// </summary>
        public Dock DockPanelDock
        {
            get => dockPanelDock;
            set => SetProperty(ref dockPanelDock, value);
        }

        /// <summary>
        ///     Get or set the font size of the menu bar
        /// </summary>
        public int FontSize
        {
            get => fontSize;
            set => SetProperty(ref fontSize, value);
        }

        /// <summary>
        ///     Creates new <see cref="DynamicMenuBarViewModel" /> witch specified <see cref="Dock" /> value
        /// </summary>
        /// <param name="dockPanelDock"></param>
        protected DynamicMenuBarViewModel(Dock dockPanelDock)
        {
            DockPanelDock = dockPanelDock;
        }

        /// <summary>
        ///     Initializes the default <see cref="MenuItem" /> instances of the <see cref="DynamicMenuBarViewModel" />
        /// </summary>
        protected virtual void InitializeDefaultMenuItems()
        {
        }

        /// <summary>
        ///     Creates a new <see cref="MenuItem" /> that contains the default file sub menus
        /// </summary>
        /// <param name="subItems"></param>
        /// <returns></returns>
        protected MenuItem MakeDefaultFileMenu(IEnumerable<MenuItem> subItems)
        {
            var item = new MenuItem
            {
                Header = "_File"
            };
            item.Items.Add(new MenuItem {Header = "_Open"});
            item.Items.Add(new MenuItem {Header = "_Close"});
            item.Items.Add(new MenuItem {Header = "_Save"});
            item.Items.Add(new Separator());

            if (subItems != null)
            {
                foreach (var subItem in subItems) item.Items.Add(subItem);
            }

            item.Items.Add(new MenuItem {Header = "_Exit"});
            return item;
        }
    }
}