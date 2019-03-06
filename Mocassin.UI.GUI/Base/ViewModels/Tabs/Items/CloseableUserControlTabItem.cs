using System.Windows.Controls;
using System.Windows.Input;
using Mocassin.UI.Base.Commands;
using Mocassin.UI.Base.ViewModels;
using Mocassin.UI.GUI.Base.ViewModels.Collections;

namespace Mocassin.UI.GUI.Base.ViewModels.Tabs
{
    /// <summary>
    /// </summary>
    public class CloseableUserControlTabItem : UserControlTabItem
    {
        /// <summary>
        ///     Get a command to remove the tab fom its host <see cref="ObservableCollectionViewModel{T}"/>
        /// </summary>
        public ParameterlessCommand CloseTabCommand { get; }

        /// <summary>
        ///     Creates new <see cref="CloseableUserControlTabItem" /> with a host <see cref="ObservableCollectionViewModel{T}" />
        /// </summary>
        /// <param name="tabName"></param>
        /// <param name="viewModel"></param>
        /// <param name="userControl"></param>
        /// <param name=""></param>
        /// <param name="hostCollectionViewModel"></param>
        public CloseableUserControlTabItem(string tabName, ViewModel viewModel, UserControl userControl,
            ICollectionViewModel<UserControlTabItem> hostCollectionViewModel)
            : base(tabName, viewModel, userControl)
        {
            CloseTabCommand = MakeTabCloseCommand(hostCollectionViewModel);
        }

        private RelayCommand MakeTabCloseCommand(ICollectionViewModel<UserControlTabItem> hostCollectionViewModel)
        {
            void CloseTab() => hostCollectionViewModel.RemoveCollectionItem(this);
            bool CanCloseTab() => hostCollectionViewModel.CollectionContains(this);
            return new RelayCommand(CloseTab, CanCloseTab);
        }
    }
}