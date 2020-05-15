using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Commands;
using Mocassin.UI.GUI.Controls.Tools.MslTools;

namespace Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.ToolMenu.Commands
{
    /// <summary>
    ///     The <see cref="ProjectControlCommand"/> implementation to add a new
    /// </summary>
    public class AddMslToolsTabCommand : ProjectControlCommand
    {
        /// <inheritdoc />
        public AddMslToolsTabCommand(IProjectAppControl projectControl)
            : base(projectControl)
        {
        }

        /// <inheritdoc />
        public override void Execute()
        {
            var view = new MslToolsView();
            var viewModel = new MslToolsViewModel();
            ProjectControl.ProjectWorkTabControlViewModel.TabHostViewModel.AddDynamicTab("Msl Tools", viewModel, view);
        }
    }
}