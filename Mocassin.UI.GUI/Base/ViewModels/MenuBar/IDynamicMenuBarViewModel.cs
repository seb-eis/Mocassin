using System.Windows.Controls;
using Mocassin.UI.GUI.Base.ViewModels.Collections;

namespace Mocassin.UI.GUI.Base.ViewModels.MenuBar
{
    /// <summary>
    ///     Represents a <see cref="ViewModelBase" /> for a <see cref="DynamicMenuBarView" />
    /// </summary>
    public interface IDynamicMenuBarViewModel : IObservableCollectionViewModel<UserControl>
    {
        /// <summary>
        ///     Get or set the <see cref="Dock" /> of the menu bar <see cref="DockPanel" />
        /// </summary>
        Dock DockPanelDock { get; set; }
    }
}