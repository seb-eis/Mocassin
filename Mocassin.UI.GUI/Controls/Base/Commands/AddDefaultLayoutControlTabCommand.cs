using System;
using System.Windows.Controls;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.ViewModels;
using Mocassin.UI.GUI.Base.Views;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.SubControls.Base.Content;

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
        ///     Get the <see cref="ContentControl" /> for the visualization control <see cref="ContentPresenter" />
        /// </summary>
        /// <returns></returns>
        protected virtual ContentControl GetVisualizerControl()
        {
            return GetNoContentControl();
        }

        /// <summary>
        ///     Get the <see cref="ContentControl" /> for the info control <see cref="ContentPresenter" />
        /// </summary>
        /// <returns></returns>
        protected virtual ContentControl GetInfoControl()
        {
            return GetNoContentControl();
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
        protected sealed override GUI.Base.ViewModels.ViewModelBase GetViewModel()
        {
            var viewModel = new BasicModelContentControlViewModel(ProjectControl)
            {
                DataContentControl = GetDataControl(),
                InfoContentControl = GetInfoControl(),
                VisualizerContentControl = GetInfoControl(),
                SelectedProjectGraph = null
            };
            return viewModel;
        }

        /// <inheritdoc />
        protected sealed override UserControl GetUserControl()
        {
            return new BasicModelContentControlView();
        }

        /// <summary>
        ///     Counts the user controls of the specified type currently open in the main work tab control
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected int CountOpenUserControls(Type type)
        {
            var result = 0;
            foreach (var item in ProjectControl.ProjectWorkTabControlViewModel.TabControlViewModel.ObservableItems)
            {
                if (item.UserControl.GetType() == type) result++;
            }
            return result;
        }
    }
}