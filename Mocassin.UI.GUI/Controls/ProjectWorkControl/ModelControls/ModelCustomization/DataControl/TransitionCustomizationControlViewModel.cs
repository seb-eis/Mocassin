using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Customization;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.ModelCustomization.DataControl
{
    /// <summary>
    ///     The <see cref="CustomizationGraphControlViewModel" /> for <see cref="TransitionCustomizationControlView" /> that
    ///     controls transition model values
    /// </summary>
    public class TransitionCustomizationControlViewModel : CustomizationGraphControlViewModel
    {
        /// <inheritdoc />
        public TransitionCustomizationControlViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
        }

        /// <inheritdoc />
        public override void ChangeContentSource(ProjectCustomizationGraph contentSource)
        {
            ContentSource = contentSource;
        }
    }
}