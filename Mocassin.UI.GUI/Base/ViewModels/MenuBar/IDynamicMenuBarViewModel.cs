using System.Windows.Controls;
using Mocassin.UI.GUI.Base.ViewModels.Collections;

namespace Mocassin.UI.GUI.Base.ViewModels.MenuBar
{
    /// <summary>
    ///     Represents a <see cref="ViewModel" /> for a <see cref="DynamicMenuBarView" />
    /// </summary>
    public interface IDynamicMenuBarViewModel : IObservableCollectionViewModel<MenuItem>
    {
        /// <summary>
        ///     Get or set the <see cref="Dock" /> of the menu bar <see cref="DockPanel" />
        /// </summary>
        Dock DockPanelDock { get; set; }

        /// <summary>
        ///     Get or set the menu bar font size
        /// </summary>
        int FontSize { get; set; }
    }
}