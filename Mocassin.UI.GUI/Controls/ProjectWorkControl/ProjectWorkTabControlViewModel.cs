using System.Collections.Specialized;
using System.Linq;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.ViewModels.Tabs;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.WelcomeControl;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl
{
    /// <summary>
    ///     The <see cref="PrimaryControlViewModel" /> for the <see cref="UserControlTabControlView" /> of work tabs
    /// </summary>
    public class ProjectWorkTabControlViewModel : PrimaryControlViewModel
    {
        /// <summary>
        ///     Get the welcome <see cref="UserControlTabItem"/>
        /// </summary>
        private UserControlTabItem WelcomeTabItem { get; }

        /// <summary>
        ///     Get the <see cref="UserControlTabControlViewModel" /> that controls the project control tabs
        /// </summary>
        public UserControlTabControlViewModel TabControlViewModel { get; }

        /// <inheritdoc />
        public ProjectWorkTabControlViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            TabControlViewModel = new UserControlTabControlViewModel();
            WelcomeTabItem = new UserControlTabItem("Welcome to Mocassin", new WelcomeControlViewModel(ProjectControl), new WelcomeControlView());
            ActivateWelcomeTab();
            TabControlViewModel.ObservableItems.CollectionChanged += OnTabCollectionChanged;
        }

        /// <summary>
        ///     Adds the welcome tab to the <see cref="UserControlTabControlViewModel"/> of the work tab system if its not currently there
        /// </summary>
        private void ActivateWelcomeTab()
        {
            if (TabControlViewModel.ObservableItems.Contains(WelcomeTabItem)) return;
            TabControlViewModel.AddCollectionItem(WelcomeTabItem);
            TabControlViewModel.SetActiveTabByIndex(-1);
        }

        /// <summary>
        ///     Removes the welcome tab from the <see cref="UserControlTabControlViewModel"/> of the work tab system
        /// </summary>
        private void DeactivateWelcomeTab()
        {
            TabControlViewModel.ObservableItems.Remove(WelcomeTabItem);
        }

        /// <summary>
        ///     Action that is attached to the tab collection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnTabCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (TabControlViewModel.ObservableItems.Count == 0) SendToDispatcher(ActivateWelcomeTab);
            if (TabControlViewModel.ObservableItems.Count > 1) SendToDispatcher(DeactivateWelcomeTab);
        }
    }
}