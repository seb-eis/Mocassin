﻿using System.Windows.Controls;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.ViewModels;
using Mocassin.UI.GUI.Base.Views;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.Base.Content;

namespace Mocassin.UI.GUI.Controls.Base.Commands
{
    /// <summary>
    ///     Base class for <see cref="ProjectControlCommand" /> implementations to add default layout model control tabs
    /// </summary>
    public class AddDefaultLayoutControlTabCommand : AddWorkTabCommand
    {
        /// <inheritdoc />
        protected AddDefaultLayoutControlTabCommand(IProjectAppControl projectControl)
            : base(projectControl)
        {
        }

        /// <summary>
        ///     Get the <see cref="ContentControl" /> for the data control <see cref="ContentPresenter" />
        /// </summary>
        /// <returns></returns>
        protected virtual ContentControl GetDataControl() => GetNoContentControl();

        /// <summary>
        ///     Get the default <see cref="ContentControl" /> for undefined <see cref="ContentPresenter" /> content
        /// </summary>
        /// <returns></returns>
        protected virtual ContentControl GetNoContentControl() => new NoContentView();

        /// <inheritdoc />
        protected override string GetTabName() => "Control";

        /// <inheritdoc />
        protected sealed override ViewModelBase GetViewModel()
        {
            var viewModel = new BasicModelContentControlViewModel(ProjectControl)
            {
                DataContentControl = GetDataControl(),
                SelectedProject = ProjectControl.ProjectBrowserViewModel.GetActiveWorkProject()
            };
            return viewModel;
        }

        /// <inheritdoc />
        protected sealed override UserControl GetUserControl() => new BasicModelContentControlView();
    }
}