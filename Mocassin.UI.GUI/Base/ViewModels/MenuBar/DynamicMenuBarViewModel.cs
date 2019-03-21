using System.Collections.Generic;
using System.Windows.Controls;
using Mocassin.UI.GUI.Base.ViewModels.Collections;

namespace Mocassin.UI.GUI.Base.ViewModels.MenuBar
{
    /// <summary>
    ///     Base class for <see cref="ViewModelBase" /> implementations of observable <see cref="MenuItem" /> collections
    /// </summary>
    public class DynamicMenuBarViewModel : ObservableCollectionViewModel<UserControl>, IDynamicMenuBarViewModel
    {
        private Dock dockPanelDock = Dock.Top;

        /// <inheritdoc />
        public Dock DockPanelDock
        {
            get => dockPanelDock;
            set => SetProperty(ref dockPanelDock, value);
        }

        /// <summary>
        ///     Creates new <see cref="DynamicMenuBarViewModel" /> witch specified <see cref="Dock" /> value
        /// </summary>
        /// <param name="dockPanelDock"></param>
        public DynamicMenuBarViewModel(Dock dockPanelDock)
        {
            DockPanelDock = dockPanelDock;
        }

        /// <summary>
        ///     Initializes the default <see cref="UserControl" /> instances of the <see cref="DynamicMenuBarViewModel" />
        /// </summary>
        protected virtual void InitializeDefaultMenuItems()
        {
        }
    }
}