using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Customization;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.ModelCustomization.GridControl
{
    /// <summary>
    ///     The <see cref="CollectionControlViewModel{T}" /> for the<see cref="PairEnergySetControlView" />
    /// </summary>
    public sealed class PairEnergySetControlViewModel : CollectionControlViewModel<PairEnergyGraph>, IDisposable
    {
        /// <summary>
        ///     Get the <see cref="PairEnergySetGraph" /> that is the chiral partner of the controlled one
        /// </summary>
        public PairEnergySetGraph ChiralPairEnergySet { get; }

        /// <summary>
        ///     Get the <see cref="PairEnergySetGraph" /> that the view model targets
        /// </summary>
        public PairEnergySetGraph PairEnergySet { get; }

        /// <summary>
        ///     Get a boolean flag if the control is chiral dependent from another
        /// </summary>
        public bool IsChiralDependent => ChiralPairEnergySet != null && PairEnergySet.PairInteractionIndex < ChiralPairEnergySet.PairInteractionIndex;

        /// <summary>
        ///     Create new <see cref="PairEnergySetControlViewModel" />
        /// </summary>
        /// <param name="pairEnergySet"></param>
        /// <param name="parentCollection"></param>
        public PairEnergySetControlViewModel(PairEnergySetGraph pairEnergySet, IReadOnlyList<PairEnergySetGraph> parentCollection)
        {
            if (parentCollection == null) throw new ArgumentNullException(nameof(parentCollection));
            PairEnergySet = pairEnergySet ?? throw new ArgumentNullException(nameof(pairEnergySet));
            ChiralPairEnergySet = FindChiralPartner(pairEnergySet, parentCollection);
            SubscribeToEnergyChanges(ChiralPairEnergySet);
            SetCollection(pairEnergySet.PairEnergyEntries);
            SelectedItem = Items?.FirstOrDefault();
        }

        /// <summary>
        ///     Finds the chiral partner <see cref="PairEnergySetGraph" /> within the provided <see cref="IReadOnlyList{T}" />.
        ///     Returns null if it doesn't exist
        /// </summary>
        /// <param name="original"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        private static PairEnergySetGraph FindChiralPartner(PairEnergySetGraph original, IReadOnlyList<PairEnergySetGraph> collection)
        {
            if (original.ChiralInteractionIndex < 0 || original.ChiralInteractionIndex >= collection.Count) return null;
            return collection[original.ChiralInteractionIndex];
        }

        /// <summary>
        ///     Subscribes to the property change events of all <see cref="PairEnergyGraph" /> entries in the provided
        ///     <see cref="PairEnergySetGraph" />
        /// </summary>
        /// <param name="source"></param>
        private void SubscribeToEnergyChanges(PairEnergySetGraph source)
        {
            if (source == null) return;
            foreach (var energyEntry in ChiralPairEnergySet.PairEnergyEntries) energyEntry.PropertyChanged += RelayPartnerEnergyChange;
        }

        /// <summary>
        ///     Catches the energy change of a chiral partner <see cref="PairEnergyGraph" /> and copies the value to the managed
        ///     one
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void RelayPartnerEnergyChange(object sender, PropertyChangedEventArgs args)
        {
            static bool PairGraphsAreEqual(PairEnergyGraph first, PairEnergyGraph second)
            {
                return first.CenterParticle.Equals(second.CenterParticle) && first.PartnerParticle.Equals(second.PartnerParticle);
            }

            if (args.PropertyName != nameof(PairEnergyGraph.Energy)) return;
            if (!(sender is PairEnergyGraph source)) return;
            var match = PairEnergySet.PairEnergyEntries.FirstOrDefault(x => PairGraphsAreEqual(x, source));
            if (match == null) throw new InvalidOperationException("Relay to chiral partner failed, no match for the source was found.");
            match.Energy = source.Energy;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (ChiralPairEnergySet == null) return;
            foreach (var item in ChiralPairEnergySet.PairEnergyEntries) item.PropertyChanged -= RelayPartnerEnergyChange;
        }
    }
}