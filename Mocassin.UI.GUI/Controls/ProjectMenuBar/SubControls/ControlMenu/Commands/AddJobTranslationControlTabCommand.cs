using System.Windows.Controls;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Commands;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.Base.Content;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.JobTranslation;

namespace Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.ControlMenu.Commands
{
    /// <summary>
    ///     The <see cref="AddDefaultJobTranslationControlTabCommand" /> to add a default layout
    ///     <see cref="BasicJobTranslationContentControlView" /> with affiliated view model to the work tab control
    /// </summary>
    public class AddJobTranslationControlTabCommand : AddDefaultJobTranslationControlTabCommand
    {
        /// <inheritdoc />
        public AddJobTranslationControlTabCommand(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
        }

        /// <inheritdoc />
        protected override ContentControl GetDataControl()
        {
            return new JobTranslationControlView {DataContext = new JobTranslationControlViewModel(ProjectControl)};
        }
    }
}