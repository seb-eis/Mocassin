using Mocassin.UI.Base.ViewModel;
using Mocassin.UI.GUI.Base.ViewModels.Collections;

namespace Mocassin.UI.GUI.Base.ViewModels.Tabs
{
    /// <summary>
    ///     <see cref="ViewModel" /> for providing sets of <see cref="System.Windows.Controls.UserControl" /> through a
    ///     <see cref="System.Windows.Controls.TabControl" />
    /// </summary>
    public class UserControlTabControlViewModel : ObservableCollectionViewModel<UserControlTabItem>
    {
        /// <summary>
        ///     The <see cref="SelectedTab" /> backing field
        /// </summary>
        private UserControlTabItem selectedTab;

        /// <summary>
        ///     Get or set the currently selected tab
        /// </summary>
        public UserControlTabItem SelectedTab
        {
            get => selectedTab;
            set => SetProperty(ref selectedTab, value);
        }
    }
}