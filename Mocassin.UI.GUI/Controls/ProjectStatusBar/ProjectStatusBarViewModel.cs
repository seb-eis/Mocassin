using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.ViewModels;

namespace Mocassin.UI.GUI.Controls.ProjectStatusBar
{
    /// <summary>
    ///     The <see cref="PrimaryControlViewModel" /> for the <see cref="ProjectStatusBarView" />
    /// </summary>
    public class ProjectStatusBarViewModel : PrimaryControlViewModel
    {
        /// <inheritdoc />
        public ProjectStatusBarViewModel(IProjectAppControl projectControl)
            : base(projectControl)
        {
        }
    }
}