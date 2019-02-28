using System;
using System.Windows.Controls;
using Mocassin.UI.Base.ViewModel;

namespace Mocassin.UI.GUI.Base.ViewModels.Tabs
{
    /// <summary>
    ///     Adapter class to wrap <see cref="UI.Base.ViewModel.ViewModel" /> o
    /// </summary>
    public class UserControlTabItem
    {
        /// <summary>
        ///     Get the name of the tab
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Get the <see cref="UI.Base.ViewModel.ViewModel" /> of the tab
        /// </summary>
        public ViewModel ViewModel { get; }

        /// <summary>
        ///     Get the <see cref="System.Windows.Controls.UserControl" /> of the tab
        /// </summary>
        public UserControl UserControl { get; }

        public Type ViewModelType { get; }

        public Type UserControlType { get; }

        /// <summary>
        ///     Creates a new <see cref="UserControlTabItem" /> from tab name and view model
        /// </summary>
        /// <param name="name"></param>
        /// <param name="viewModel"></param>
        /// <param name="userControl"></param>
        public UserControlTabItem(string name, ViewModel viewModel, UserControl userControl)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            UserControl = userControl ?? throw new ArgumentNullException(nameof(userControl));
            ViewModelType = viewModel.GetType();
            UserControlType = userControl.GetType();
        }
    }
}