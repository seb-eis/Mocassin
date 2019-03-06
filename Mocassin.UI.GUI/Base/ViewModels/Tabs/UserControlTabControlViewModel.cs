using System.Windows.Controls;
using Mocassin.UI.Base.ViewModels;
using Mocassin.UI.GUI.Base.ViewModels.Collections;

namespace Mocassin.UI.GUI.Base.ViewModels.Tabs
{
    /// <summary>
    ///     Base <see cref="ViewModel" /> for providing sets of <see cref="System.Windows.Controls.UserControl" /> through a
    ///     <see cref="System.Windows.Controls.TabControl" />
    /// </summary>
    public abstract class UserControlTabControlViewModel : ObservableCollectionViewModel<UserControlTabItem>
    {
        /// <summary>
        ///     The <see cref="SelectedTab" /> backing field
        /// </summary>
        private UserControlTabItem selectedTab;

        /// <summary>
        ///     The <see cref="HeaderFontSize" /> backing field
        /// </summary>
        private int headerFontSize = 14;

        /// <summary>
        ///     Get or set the currently selected tab
        /// </summary>
        public UserControlTabItem SelectedTab
        {
            get => selectedTab;
            set => SetProperty(ref selectedTab, value);
        }

        /// <summary>
        /// </summary>
        public int HeaderFontSize
        {
            get => headerFontSize;
            set => SetProperty(ref headerFontSize, value);
        }

        /// <summary>
        ///     Adds a <see cref="ViewModel" /> and <see cref="UserControl" /> as a <see cref="CloseableUserControlTabItem" /> with
        ///     the given tab name
        /// </summary>
        /// <param name="tabName"></param>
        /// <param name="viewModel"></param>
        /// <param name="userControl"></param>
        public void AddCloseableTab(string tabName, ViewModel viewModel, UserControl userControl)
        {
            var tabItem = new CloseableUserControlTabItem(tabName, viewModel, userControl, this);
            AddCollectionItem(tabItem);
        }

        /// <summary>
        ///     Adds a <see cref="ViewModel" /> and <see cref="UserControl" /> as a <see cref="UserControlTabItem" /> with the
        ///     given tab name
        /// </summary>
        /// <param name="tabName"></param>
        /// <param name="viewModel"></param>
        /// <param name="userControl"></param>
        public void AddNonClosableTab(string tabName, ViewModel viewModel, UserControl userControl)
        {
            var tabItem = new UserControlTabItem(tabName, viewModel, userControl);
            AddCollectionItem(tabItem);
        }

        /// <summary>
        ///     Initializes the default <see cref="UserControlTabItem" /> instances of the control
        /// </summary>
        protected virtual void InitializeDefaultTabs()
        {
        }
    }
}