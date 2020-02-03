using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
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
    public class ModelCustomizationControlViewModel : ProjectGraphControlViewModel, IContentSupplier<ProjectCustomizationTemplate>
    {
        /// <summary>
        ///     Get or set the <see cref="ProjectCustomizationTemplate" /> that serves as the current content source
        /// </summary>
        private ProjectCustomizationTemplate CustomizationContentSource { get; set; }

        /// <summary>
        ///     Get the <see cref="TransitionCustomizationControlViewModel" /> that controls the transition customization
        /// </summary>
        public TransitionCustomizationControlViewModel TransitionCustomizationViewModel { get; }

        /// <summary>
        ///     Get the <see cref="PairInteractionControlViewModel" /> that controls the stable pair interaction customization
        /// </summary>
        public PairInteractionControlViewModel StablePairInteractionViewModel { get; }

        /// <summary>
        ///     Get the <see cref="PairInteractionControlViewModel" /> that controls the unstable pair interaction customization
        /// </summary>
        public PairInteractionControlViewModel UnstablePairInteractionViewModel { get; }

        /// <summary>
        ///     Get the <see cref="GroupInteractionControlViewModel" /> that controls the group interaction customization
        /// </summary>
        public GroupInteractionControlViewModel GroupInteractionViewModel { get; }

        /// <inheritdoc />
        ProjectCustomizationTemplate IContentSupplier<ProjectCustomizationTemplate>.ContentSource => CustomizationContentSource;

        /// <inheritdoc />
        public ModelCustomizationControlViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            StablePairInteractionViewModel = new PairInteractionControlViewModel(x => x?.EnergyModelCustomization?.StablePairEnergyParameterSets);
            UnstablePairInteractionViewModel = new PairInteractionControlViewModel(x => x?.EnergyModelCustomization?.UnstablePairEnergyParameterSets);
            TransitionCustomizationViewModel = new TransitionCustomizationControlViewModel(projectControl);
            GroupInteractionViewModel = new GroupInteractionControlViewModel();
        }

        /// <inheritdoc />
        public override void ChangeContentSource(MocassinProject contentSource)
        {
            ContentSource = contentSource;
            TransitionCustomizationViewModel.ChangeContentSource(null);
            StablePairInteractionViewModel.ChangeContentSource(null);
            UnstablePairInteractionViewModel.ChangeContentSource(null);
            GroupInteractionViewModel.ChangeContentSource(null);
        }

        /// <inheritdoc />
        void IContentSupplier<ProjectCustomizationTemplate>.ChangeContentSource(ProjectCustomizationTemplate contentSource)
        {
            TransitionCustomizationViewModel.ChangeContentSource(contentSource);
            StablePairInteractionViewModel.ChangeContentSource(contentSource);
            UnstablePairInteractionViewModel.ChangeContentSource(contentSource);
            GroupInteractionViewModel.ChangeContentSource(contentSource);
            CustomizationContentSource = contentSource;
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            StablePairInteractionViewModel.Dispose();
            base.Dispose();
        }
    }
}