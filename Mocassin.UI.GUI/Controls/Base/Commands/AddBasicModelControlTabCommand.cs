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
    public class AddBasicModelControlTabCommand : AddWorkTabCommand
    {
        /// <inheritdoc />
        protected AddBasicModelControlTabCommand(IMocassinProjectControl projectControl)
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
        protected sealed override ViewModel GetViewModel()
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
        protected  sealed override UserControl GetUserControl()
        {
            return new BasicModelContentControlView();
        }
    }
}