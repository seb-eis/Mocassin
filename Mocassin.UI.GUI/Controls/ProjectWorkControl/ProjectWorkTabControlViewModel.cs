using System.Collections.Specialized;
using System.Windows;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.ViewModels.Tabs;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.WelcomeControl;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl
{
    /// <summary>
    ///     The <see cref="PrimaryControlViewModel" /> for the <see cref="ControlTabHostView" /> of work tabs
    /// </summary>
    public class ProjectWorkTabControlViewModel : PrimaryControlViewModel
    {
        /// <summary>
        ///     Get the welcome <see cref="ControlTabItem" />
        /// </summary>
        private ControlTabItem WelcomeTabItem { get; }

        /// <summary>
        ///     Get the <see cref="ControlTabHostViewModel" /> that controls the project control tabs
        /// </summary>
        public ControlTabHostViewModel TabHostViewModel { get; }

        /// <inheritdoc />
        public ProjectWorkTabControlViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            TabHostViewModel = new ControlTabHostViewModel {IsFrontInsertMode = true};
            WelcomeTabItem = new ControlTabItem("Welcome to Mocassin", new WelcomeControlViewModel(ProjectControl), new WelcomeControlView());
            TabHostViewModel.ObservableItems.CollectionChanged += OnTabCollectionChanged;
            TabHostViewModel.AddItem(WelcomeTabItem);
            TabHostViewModel.SelectedTab = WelcomeTabItem;
            EnsureWelcomeTabIsVisible();
        }

        /// <summary>
        ///     Activates the welcome tab and makes it visible
        /// </summary>
        private void EnsureWelcomeTabIsVisible()
        {
            if (WelcomeTabItem.Visibility == Visibility.Visible) return;
            ExecuteOnAppThread(() => WelcomeTabItem.Visibility = Visibility.Visible);
        }

        /// <summary>
        ///     Deactivates the welcome tab and makes it invisible
        /// </summary>
        private void EnsureWelcomeTabIsHidden()
        {
            if (WelcomeTabItem.Visibility == Visibility.Hidden) return;
            ExecuteOnAppThread(() => WelcomeTabItem.Visibility = Visibility.Hidden);
        }

        /// <summary>
        ///     Action that is attached to the tab collection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnTabCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action != NotifyCollectionChangedAction.Add && args.Action != NotifyCollectionChangedAction.Remove) return;
            if (TabHostViewModel.ObservableItems.Count == 1) EnsureWelcomeTabIsVisible();
            if (TabHostViewModel.ObservableItems.Count == 2) EnsureWelcomeTabIsHidden();
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            TabHostViewModel.DisposeAndClearItems();
            base.Dispose();
        }
    }
}