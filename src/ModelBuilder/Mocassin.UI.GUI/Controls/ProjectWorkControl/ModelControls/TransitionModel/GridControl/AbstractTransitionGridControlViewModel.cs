using System.Collections.Generic;
using Mocassin.UI.GUI.Base.ViewModels.Collections;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Data.Main;
using Mocassin.UI.Data.TransitionModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.TransitionModel.GridControl
{
    /// <summary>
    ///     A <see cref="CollectionControlViewModel{T}" /> implementation for <see cref="AbstractTransitionData" />
    /// </summary>
    public class AbstractTransitionGridControlViewModel : CollectionControlViewModel<AbstractTransitionData>,
        IContentSupplier<MocassinProject>
    {
        /// <inheritdoc />
        public MocassinProject ContentSource { get; protected set; }

        /// <summary>
        ///     Get the <see cref="ObservableCollectionViewModel{T}" /> of selectable <see cref="TransitionMechanismViewModel" />
        ///     instances
        /// </summary>
        public ObservableCollectionViewModel<TransitionMechanismViewModel> SelectableTransitionMechanisms { get; }

        /// <inheritdoc />
        public AbstractTransitionGridControlViewModel()
        {
            SelectableTransitionMechanisms = new ObservableCollectionViewModel<TransitionMechanismViewModel>();
            SelectableTransitionMechanisms.AddItems(GetSupportedMechanismViewModels());
        }

        /// <inheritdoc />
        public void ChangeContentSource(MocassinProject contentSource)
        {
            ContentSource = contentSource;
            SetCollection(ContentSource?.ProjectModelData?.TransitionModelData?.AbstractTransitions);
        }

        /// <summary>
        ///     Get a <see cref="IEnumerable{T}" /> of all supported <see cref="TransitionMechanismViewModel" /> instances
        /// </summary>
        /// <returns></returns>
        private IEnumerable<TransitionMechanismViewModel> GetSupportedMechanismViewModels()
        {
            yield return new TransitionMechanismViewModel
            {
                Name = "2-Site Metropolis",
                ConnectionPattern = "Dynamic",
                Description = "The default two position MMC exchange.",
                PositionChainDescription = "[Stable][Stable]"
            };
            yield return new TransitionMechanismViewModel
            {
                Name = "3-Site Migration",
                ConnectionPattern = "Dynamic Dynamic",
                Description = "The default three site migration for KMC that can model vacancy migration, polaron hopping or interstitial movement.",
                PositionChainDescription = "[Stable][Unstable][Stable]"
            };
            yield return new TransitionMechanismViewModel
            {
                Name = "5-Site Interstitialcy-Like",
                ConnectionPattern = "Dynamic Dynamic Dynamic Dynamic",
                Description = "An intersticialcy-like pushing of one migrating species by another where three stable sites are involved.",
                PositionChainDescription = "[Stable][Unstable][Stable][Unstable][Stable]"
            };
            yield return new TransitionMechanismViewModel
            {
                Name = "7-Site Interstitialcy-Like",
                ConnectionPattern = "Dynamic Dynamic Dynamic Dynamic Dynamic Dynamic",
                Description = "An intersticialcy-like pushing of two migrating species by another where four stable sites are involved.",
                PositionChainDescription = "[Stable][Unstable][Stable][Unstable][Stable][Unstable][Stable]"
            };
            yield return new TransitionMechanismViewModel
            {
                Name = "5-Site 2-Species-Vehicle",
                ConnectionPattern = "Dynamic Static Static Dynamic",
                Description =
                    "A vehicle of two 3-Position migration events where the transition state is described/modeled by a single shared transition site.",
                PositionChainDescription = "[Stable][Stable][Unstable][Stable][Stable]",
                HasAssociationFlagSupport = true
            };
            yield return new TransitionMechanismViewModel
            {
                Name = "6-Site 2-Species-Vehicle",
                ConnectionPattern = "Dynamic Dynamic Static Dynamic Dynamic",
                Description = "A vehicle of two 3-Position migration events where the transition site of each event is considered/modeled explicitly.",
                PositionChainDescription = "[Stable][Unstable][Stable][Stable][Unstable][Stable]",
                HasAssociationFlagSupport = true
            };
        }
    }
}