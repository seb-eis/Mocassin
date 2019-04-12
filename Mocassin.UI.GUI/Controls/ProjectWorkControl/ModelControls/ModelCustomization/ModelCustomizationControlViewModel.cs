using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.ModelCustomization.DataControl;
using Mocassin.UI.Xml.Customization;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.ModelCustomization
{
    /// <summary>
    ///     The <see cref="ProjectGraphControlViewModel" /> for <see cref="ModelCustomizationControlView" /> that controls
    ///     model
    ///     translation data customization
    /// </summary>
    public class ModelCustomizationControlViewModel : ProjectGraphControlViewModel
    {
        /// <summary>
        ///     Get the <see cref="TransitionCustomizationControlViewModel" />
        /// </summary>
        public TransitionCustomizationControlViewModel TransitionCustomizationViewModel { get; }

        /// <summary>
        ///     Get the <see cref="PairInteractionControlViewModel"/> that controls the stable pair interactions
        /// </summary>
        public PairInteractionControlViewModel StablePairInteractionViewModel { get; }

        /// <summary>
        ///     Get the <see cref="PairInteractionControlViewModel"/> that controls the unstable pair interactions
        /// </summary>
        public PairInteractionControlViewModel UnstablePairInteractionViewModel { get; }

        /// <inheritdoc />
        public ModelCustomizationControlViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            StablePairInteractionViewModel = new PairInteractionControlViewModel(x => x?.EnergyModelCustomization?.StablePairEnergyParameterSets);
            UnstablePairInteractionViewModel = new PairInteractionControlViewModel(x => x?.EnergyModelCustomization?.UnstablePairEnergyParameterSets);
            TransitionCustomizationViewModel = new TransitionCustomizationControlViewModel(projectControl);
        }

        /// <inheritdoc />
        public override void ChangeContentSource(MocassinProjectGraph contentSource)
        {
            ContentSource = contentSource;
            TransitionCustomizationViewModel.ChangeContentSource(null);
            StablePairInteractionViewModel.ChangeContentSource(null);
            UnstablePairInteractionViewModel.ChangeContentSource(null);

        }

        /// <inheritdoc />
        public override void ChangeContentSource(object contentSource)
        {
            if (contentSource is ProjectCustomizationGraph customizationGraph)
            {
                TransitionCustomizationViewModel.ChangeContentSource(customizationGraph);
                StablePairInteractionViewModel.ChangeContentSource(customizationGraph);
                UnstablePairInteractionViewModel.ChangeContentSource(customizationGraph);
                return;
            }

            base.ChangeContentSource(contentSource);
        }
    }
}