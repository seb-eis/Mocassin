using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.ModelCustomization.DataControl;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.ModelCustomization
{
    /// <summary>
    ///     The <see cref="ProjectGraphControlViewModel" /> for <see cref="ModelCustomizationControlView" /> that controls model
    ///     translation data customization
    /// </summary>
    public class ModelCustomizationControlViewModel : ProjectGraphControlViewModel
    {
        /// <summary>
        ///     Get the <see cref="EnergyCustomizationControlViewModel"/>
        /// </summary>
        public EnergyCustomizationControlViewModel EnergyCustomizationViewModel { get; }

        /// <summary>
        ///     Get the <see cref="TransitionCustomizationControlViewModel"/>
        /// </summary>
        public TransitionCustomizationControlViewModel TransitionCustomizationViewModel { get; }

        /// <inheritdoc />
        public ModelCustomizationControlViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            EnergyCustomizationViewModel = new EnergyCustomizationControlViewModel(projectControl);
            TransitionCustomizationViewModel = new TransitionCustomizationControlViewModel(projectControl);
        }

        /// <inheritdoc />
        public override void ChangeContentSource(MocassinProjectGraph contentSource)
        {
            ContentSource = contentSource;
            EnergyCustomizationViewModel.ChangeContentSource(null);
            TransitionCustomizationViewModel.ChangeContentSource(null);
        }
    }
}