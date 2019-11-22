using System;
using System.Windows;
using System.Windows.Controls;

namespace Mocassin.UI.GUI.Base.ViewModels.Tabs
{
    /// <summary>
    ///     Class to provide <see cref="Mocassin.UI.Base.ViewModels.ViewModel" /> of
    ///     <see cref="System.Windows.Controls.UserControl" /> to <see cref="TabControl" /> systems
    /// </summary>
    public class UserControlTabItem : ViewModelBase, IDisposable
    {
        private Visibility visibility = Visibility.Visible;

        /// <summary>
        ///     Get the <see cref="Mocassin.UI.Base.ViewModels.ViewModel" /> of the tab
        /// </summary>
        public ViewModelBase ControlViewModel { get; }

        /// <summary>
        ///     Get the <see cref="System.Windows.Controls.UserControl" /> of the tab
        /// </summary>
        public UserControl ContentControl { get; }

        /// <summary>
        ///     Get the name for the tab
        /// </summary>
        public string TabName { get; }

        /// <summary>
        ///     Get or set the <see cref="System.Windows.Visibility"/> of the tab
        /// </summary>
        public Visibility Visibility
        {
            get => visibility;
            set => SetProperty(ref visibility, value);
        }

        /// <summary>
        ///     Creates a new <see cref="UserControlTabItem" /> from tab name, view model and user control
        /// </summary>
        /// <param name="tabName"></param>
        /// <param name="viewModelBase"></param>
        /// <param name="userControl"></param>
        public UserControlTabItem(string tabName, ViewModelBase viewModelBase, UserControl userControl)
        {
            TabName = tabName ?? throw new ArgumentNullException(nameof(tabName));
            ControlViewModel = viewModelBase ?? throw new ArgumentNullException(nameof(viewModelBase));
            ContentControl = userControl ?? throw new ArgumentNullException(nameof(userControl));
            ContentControl.DataContext = ControlViewModel;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            (ControlViewModel as IDisposable)?.Dispose();
            (ContentControl as IDisposable)?.Dispose();
        }
    }
}