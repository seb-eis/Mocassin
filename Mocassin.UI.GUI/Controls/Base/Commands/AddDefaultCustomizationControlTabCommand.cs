using System.Windows.Controls;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.ViewModels;
using Mocassin.UI.GUI.Base.Views;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.Base.Content;

namespace Mocassin.UI.GUI.Controls.Base.Commands
{
    /// <summary>
    ///     Bas class for <see cref="AddWorkTabCommand" /> implementations that use the
    ///     <see cref="BasicCustomizationContentControlView" /> layout
    /// </summary>
    public class AddDefaultCustomizationControlTabCommand : AddWorkTabCommand
    {
        /// <inheritdoc />
        protected AddDefaultCustomizationControlTabCommand(IProjectAppControl projectControl)
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
        protected override string GetTabName() => "Parametrization Control";

        /// <inheritdoc />
        protected sealed override ViewModelBase GetViewModel()
        {
            var viewModel = new BasicCustomizationContentControlViewModel(ProjectControl)
            {
                DataContentControl = GetDataControl(),
                SelectedProject = ProjectControl.ProjectBrowserViewModel.GetActiveWorkProject(),
                SelectedCustomizationTemplate = null
            };
            return viewModel;
        }

        /// <inheritdoc />
        protected sealed override UserControl GetUserControl() => new BasicCustomizationContentControlView();
    }
}