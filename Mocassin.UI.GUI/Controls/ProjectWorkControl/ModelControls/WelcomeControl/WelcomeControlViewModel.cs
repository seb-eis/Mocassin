using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.ViewModels;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.WelcomeControl
{
    /// <summary>
    ///     The <see cref="PrimaryControlViewModel"/> for the <see cref="WelcomeControlView"/> that supplies first step information
    /// </summary>
    public class WelcomeControlViewModel : PrimaryControlViewModel
    {
        /// <inheritdoc />
        public WelcomeControlViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
        }
    }
}