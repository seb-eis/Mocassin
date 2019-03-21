using System.Windows.Controls;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.ViewModels;
using Mocassin.UI.GUI.Base.Views;

namespace Mocassin.UI.GUI.Controls.Base.Commands
{
    /// <summary>
    ///     Base class for <see cref="ProjectControlCommand" /> that add closeable work tabs to the main work tab control
    /// </summary>
    public abstract class AddWorkTabCommand : ProjectControlCommand
    {
        /// <inheritdoc />
        protected AddWorkTabCommand(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
        }

        /// <inheritdoc />
        public override bool CanExecuteInternal()
        {
            return ProjectControl.ProjectWorkTabControlViewModel != null;
        }

        /// <inheritdoc />
        public override void Execute()
        {
            ProjectControl.ProjectWorkTabControlViewModel.TabControlViewModel
                .AddCloseableTab(GetTabName(), GetViewModel(), GetUserControl());
        }

        /// <summary>
        ///     Get the <see cref="ViewModelBase" /> that the tab should use
        /// </summary>
        /// <returns></returns>
        protected virtual ViewModelBase GetViewModel()
        {
            return new EmptyViewModel();
        }

        /// <summary>
        ///     Get the <see cref="UserControl" /> of the tab
        /// </summary>
        /// <returns></returns>
        protected virtual UserControl GetUserControl()
        {
            return new NoContentView();
        }

        /// <summary>
        ///     Get the name <see cref="string" /> of the tab
        /// </summary>
        /// <returns></returns>
        protected virtual string GetTabName()
        {
            return "New Tab";
        }
    }
}