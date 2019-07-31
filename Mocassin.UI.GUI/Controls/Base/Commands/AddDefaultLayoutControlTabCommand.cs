using System;
using System.Windows.Controls;
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
        protected AddDefaultLayoutControlTabCommand(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
        }

        /// <summary>
        ///     Get the <see cref="ContentControl" /> for the data control <see cref="ContentPresenter" />
        /// </summary>
        /// <returns></returns>
        protected virtual ContentControl GetDataControl()
        {
            return GetNoContentControl();
        }

        /// <summary>
        ///     Get the default <see cref="ContentControl" /> for undefined <see cref="ContentPresenter" /> content
        /// </summary>
        /// <returns></returns>
        protected virtual ContentControl GetNoContentControl()
        {
            return new NoContentView();
        }

        /// <inheritdoc />
        protected override string GetTabName()
        {
            return "Control";
        }

        /// <inheritdoc />
        protected sealed override ViewModelBase GetViewModel()
        {
            var viewModel = new BasicModelContentControlViewModel(ProjectControl)
            {
                DataContentControl = GetDataControl(),
                SelectedProjectGraph = null
            };
            return viewModel;
        }

        /// <inheritdoc />
        protected sealed override UserControl GetUserControl()
        {
            return new BasicModelContentControlView();
        }
    }
}