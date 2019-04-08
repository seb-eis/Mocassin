using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Customization;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.ModelCustomization.DataControl
{
    /// <summary>
    ///     The <see cref="CustomizationGraphControlViewModel" /> for <see cref="EnergyCustomizationControlView" /> that
    ///     controls manipulation of energy model values
    /// </summary>
    public class EnergyCustomizationControlViewModel : CustomizationGraphControlViewModel
    {
        /// <inheritdoc />
        public EnergyCustomizationControlViewModel(IMocassinProjectControl projectControl)
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