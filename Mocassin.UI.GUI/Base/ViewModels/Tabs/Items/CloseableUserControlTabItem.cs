using System.Windows.Controls;
using System.Windows.Input;
using Mocassin.UI.Base.Commands;
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
        public ICommand CloseTabCommand { get; }

        /// <summary>
        ///     Creates new <see cref="CloseableUserControlTabItem" /> with a host <see cref="ObservableCollectionViewModel{T}" />
        /// </summary>
        /// <param name="tabName"></param>
        /// <param name="viewModelBase"></param>
        /// <param name="userControl"></param>
        /// <param name=""></param>
        /// <param name="hostCollectionViewModel"></param>
        public CloseableUserControlTabItem(string tabName, ViewModelBase viewModelBase, UserControl userControl,
            IObservableCollectionViewModel<UserControlTabItem> hostCollectionViewModel)
            : base(tabName, viewModelBase, userControl)
        {
            CloseTabCommand = MakeTabCloseCommand(hostCollectionViewModel);
        }

        /// <summary>
        ///     Creates an <see cref="ICommand"/> that removes the tab from its host <see cref="IObservableCollectionViewModel{T}"/>
        /// </summary>
        /// <param name="hostCollectionViewModel"></param>
        /// <returns></returns>
        private ICommand MakeTabCloseCommand(IObservableCollectionViewModel<UserControlTabItem> hostCollectionViewModel)
        {
            void CloseTab()
            {
                Dispose();
                hostCollectionViewModel.RemoveCollectionItem(this);
            }
            bool CanCloseTab() => hostCollectionViewModel.CollectionContains(this);
            return new RelayCommand(CloseTab, CanCloseTab);
        }
    }
}