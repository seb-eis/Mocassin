using System.Collections.ObjectModel;
using System.Windows.Controls;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.ViewModels.MenuBar;
using Mocassin.UI.GUI.Controls.Base;

namespace Mocassin.UI.GUI.Controls.ProjectMenuBar
{
    /// <summary>
    ///     The <see cref="PrimaryControlViewModel" /> for the <see cref="DynamicMenuBarView" /> of the main menu
    /// </summary>
    public class ProjectMenuBarViewModel : PrimaryControlViewModel, IDynamicMenuBarViewModel
    {
        /// <summary>
        ///     Get the <see cref="DynamicMenuBarViewModel" /> that controls the menu items
        /// </summary>
        private DynamicMenuBarViewModel MenuBarViewModel { get; }

        /// <inheritdoc />
        public Dock DockPanelDock
        {
            get => MenuBarViewModel.DockPanelDock;
            set => MenuBarViewModel.DockPanelDock = value;
        }

        /// <inheritdoc />
        public int FontSize
        {
            get => MenuBarViewModel.FontSize;
            set => MenuBarViewModel.FontSize = value;
        }

        /// <inheritdoc />
        public ObservableCollection<MenuItem> ObservableItems => MenuBarViewModel.ObservableItems;


        /// <inheritdoc />
        public ProjectMenuBarViewModel(IMocassinProjectControl mainProjectControl) : base(mainProjectControl)
        {
            MenuBarViewModel = new DynamicMenuBarViewModel(Dock.Top);
        }

        /// <inheritdoc />
        public void InsertCollectionItem(int index, MenuItem value)
        {
            MenuBarViewModel.InsertCollectionItem(index, value);
        }

        /// <inheritdoc />
        public void AddCollectionItem(MenuItem value)
        {
            MenuBarViewModel.AddCollectionItem(value);
        }

        /// <inheritdoc />
        public void RemoveCollectionItem(MenuItem value)
        {
            MenuBarViewModel.RemoveCollectionItem(value);
        }

        /// <inheritdoc />
        public bool CollectionContains(MenuItem value)
        {
            return MenuBarViewModel.CollectionContains(value);
        }
    }
}