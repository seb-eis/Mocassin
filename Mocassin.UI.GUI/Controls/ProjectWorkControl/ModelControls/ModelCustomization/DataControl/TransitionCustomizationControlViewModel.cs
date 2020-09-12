using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            UpdateSetControlViewModels();
        }

        /// <summary>
        ///     Updates the required <see cref="KineticRuleSetControlViewModel" /> collection
        /// </summary>
        private void UpdateSetControlViewModels()
        {
            KineticRuleSetCollectionViewModel.Items ??= new ObservableCollection<KineticRuleSetControlViewModel>();
            KineticRuleSetCollectionViewModel.Items.Clear();

            var interactionSets = ContentSource?.TransitionModelCustomization?.KineticTransitionParameterSets;
            if (interactionSets == null) return;
            KineticRuleSetCollectionViewModel.Items.AddRange(interactionSets.Select(ruleSetData => new KineticRuleSetControlViewModel(ruleSetData)));
        }
    }
}