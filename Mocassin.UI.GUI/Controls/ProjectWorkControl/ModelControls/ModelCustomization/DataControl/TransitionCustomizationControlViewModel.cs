using System.Linq;
using Mocassin.Framework.Extensions;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.ModelCustomization.GridControl;
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
        ///     Get the <see cref="CollectionControlViewModel{T}" /> for the customizable <see cref="KineticRuleSetData" />
        ///     instances
        /// </summary>
        public CollectionControlViewModel<KineticRuleSetControlViewModel> KineticRuleSetCollectionViewModel { get; }

        /// <inheritdoc />
        public TransitionCustomizationControlViewModel(IProjectAppControl projectControl)
            : base(projectControl)
        {
            KineticRuleSetCollectionViewModel = new CollectionControlViewModel<KineticRuleSetControlViewModel>();
        }

        /// <inheritdoc />
        public override void ChangeContentSource(ProjectCustomizationTemplate contentSource)
        {
            ContentSource = contentSource;
            CreateSetControlViewModels();
        }

        /// <summary>
        ///     Creates and sets the new <see cref="KineticRuleSetControlViewModel" /> collection
        /// </summary>
        private void CreateSetControlViewModels()
        {
            var interactionSets = ContentSource?.TransitionModelCustomization?.KineticTransitionParameterSets;
            if (interactionSets == null)
            {
                KineticRuleSetCollectionViewModel.SetCollection(null);
                return;
            }

            var viewModels = interactionSets.Select(x => new KineticRuleSetControlViewModel(x)).ToList(interactionSets.Count);
            KineticRuleSetCollectionViewModel.SetCollection(viewModels);
        }
    }
}