using System;
using System.Windows.Controls;

namespace Mocassin.UI.GUI.Base.ViewModels.Tabs
{
    /// <summary>
    ///     Class to provide <see cref="Mocassin.UI.Base.ViewModels.ViewModel" /> of
    ///     <see cref="System.Windows.Controls.UserControl" /> to <see cref="TabControl" /> systems
    /// </summary>
    public class UserControlTabItem
    {
        /// <summary>
        ///     Get the <see cref="Mocassin.UI.Base.ViewModels.ViewModel" /> of the tab
        /// </summary>
        public ViewModel ViewModel { get; }

        /// <summary>
        ///     Get the <see cref="System.Windows.Controls.UserControl" /> of the tab
        /// </summary>
        public UserControl UserControl { get; }

        /// <summary>
        ///     Get the name for the tab
        /// </summary>
        public string TabName { get; }

        /// <summary>
        ///     Creates a new <see cref="UserControlTabItem" /> from tab name, view model and user control
        /// </summary>
        /// <param name="tabName"></param>
        /// <param name="viewModel"></param>
        /// <param name="userControl"></param>
        public UserControlTabItem(string tabName, ViewModel viewModel, UserControl userControl)
        {
            TabName = tabName ?? throw new ArgumentNullException(nameof(tabName));
            ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            UserControl = userControl ?? throw new ArgumentNullException(nameof(userControl));
            UserControl.DataContext = ViewModel;
        }
    }
}