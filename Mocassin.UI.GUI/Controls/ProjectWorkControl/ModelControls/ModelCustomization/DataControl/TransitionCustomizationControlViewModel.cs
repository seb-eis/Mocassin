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
        /// <summary>
        ///     Get the <see cref="CollectionControlViewModel{T}" /> for the customizable <see cref="KineticRuleSetGraph" />
        ///     instances
        /// </summary>
        public CollectionControlViewModel<KineticRuleSetGraph> KineticRuleSetCollectionViewModel { get; }

        /// <inheritdoc />
        public TransitionCustomizationControlViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            KineticRuleSetCollectionViewModel = new CollectionControlViewModel<KineticRuleSetGraph>();
        }

        /// <inheritdoc />
        public override void ChangeContentSource(ProjectCustomizationGraph contentSource)
        {
            ContentSource = contentSource;
            KineticRuleSetCollectionViewModel.SetCollection(contentSource?.TransitionModelCustomization?.KineticTransitionParameterSets);
        }
    }
}