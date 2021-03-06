﻿using System.Windows.Controls;
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
        protected AddWorkTabCommand(IProjectAppControl projectControl)
            : base(projectControl)
        {
        }

        /// <inheritdoc />
        public override bool CanExecuteInternal() => ProjectControl.ProjectWorkTabControlViewModel != null;

        /// <inheritdoc />
        public override void Execute()
        {
            var (tabName, viewModel, control) = (GetTabName(), GetViewModel(), GetUserControl());
            ProjectControl.ProjectWorkTabControlViewModel.TabHostViewModel.AddDynamicTab(tabName, viewModel, control);
        }

        /// <summary>
        ///     Get the <see cref="ViewModelBase" /> that the tab should use
        /// </summary>
        /// <returns></returns>
        protected virtual ViewModelBase GetViewModel() => new EmptyViewModel();

        /// <summary>
        ///     Get the <see cref="UserControl" /> of the tab
        /// </summary>
        /// <returns></returns>
        protected virtual UserControl GetUserControl() => new NoContentView();

        /// <summary>
        ///     Get the name <see cref="string" /> of the tab
        /// </summary>
        /// <returns></returns>
        protected virtual string GetTabName() => "New Tab";
    }
}