using System;
using System.Windows.Controls;
using System.Windows.Input;
using Mocassin.UI.Base.Commands;

namespace Mocassin.UI.GUI.Base.ViewModels.Tabs
{
    /// <summary>
    ///     Extends the <see cref="UserControlTabItem" /> to provide a close command that removes it from its host view model
    /// </summary>
    public class CloseableUserControlTabItem : UserControlTabItem
    {
        /// <summary>
        ///     Get the hosting <see cref="IUserControlTabHost" />
        /// </summary>
        private IUserControlTabHost TabHost { get; }

        /// <summary>
        ///     Get a command to remove the tab fom its host <see cref="IUserControlTabHost" />
        /// </summary>
        public ICommand CloseTabCommand { get; }

        /// <summary>
        ///     Creates new <see cref="CloseableUserControlTabItem" /> that belongs to the provided
        ///     <see cref="IUserControlTabHost" />
        /// </summary>
        /// <param name="tabName"></param>
        /// <param name="viewModelBase"></param>
        /// <param name="userControl"></param>
        /// <param name=""></param>
        /// <param name="tabHost"></param>
        public CloseableUserControlTabItem(string tabName, ViewModelBase viewModelBase, UserControl userControl, IUserControlTabHost tabHost)
            : base(tabName, viewModelBase, userControl)
        {
            TabHost = tabHost ?? throw new ArgumentNullException(nameof(tabHost));
            CloseTabCommand = MakeTabCloseCommand();
        }

        /// <summary>
        ///     Creates an <see cref="ICommand" /> that removes the tab from its host <see cref="IUserControlTabHost" />
        /// </summary>
        /// <returns></returns>
        private ICommand MakeTabCloseCommand()
        {
            void CloseTab()
            {
                TabHost.RemoveAndDispose(this);
            }

            bool CanCloseTab()
            {
                return TabHost.CollectionContains(this);
            }

            return new RelayCommand(CloseTab, CanCloseTab);
        }
    }
}