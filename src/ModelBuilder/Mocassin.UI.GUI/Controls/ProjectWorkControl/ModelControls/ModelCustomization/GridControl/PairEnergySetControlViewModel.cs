using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Data.Customization;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.ModelCustomization.GridControl
{
    /// <summary>
    ///     The <see cref="CollectionControlViewModel{T}" /> for the<see cref="PairEnergySetControlView" />
    /// </summary>
    public sealed class PairEnergySetControlViewModel : CollectionControlViewModel<PairEnergyData>, IDisposable
    {
        /// <summary>
        ///     Get the <see cref="PairEnergySetData" /> that is the chiral partner of the controlled one
        /// </summary>
        public PairEnergySetData ChiralPairEnergySet { get; }

        /// <summary>
        ///     Get the <see cref="PairEnergySetData" /> that the view model targets
        /// </summary>
        public PairEnergySetData PairEnergySet { get; }

        /// <summary>
        ///     Get a boolean flag if the control is chiral dependent from another
        /// </summary>
        public bool IsChiralDependent => ChiralPairEnergySet != null && PairEnergySet.ModelIndex < ChiralPairEnergySet.ModelIndex;

        /// <summary>
        ///     Create new <see cref="PairEnergySetControlViewModel" />
        /// </summary>
        /// <param name="pairEnergySet"></param>
        /// <param name="parentCollection"></param>
        public PairEnergySetControlViewModel(PairEnergySetData pairEnergySet, IReadOnlyList<PairEnergySetData> parentCollection)
        {
            if (parentCollection == null) throw new ArgumentNullException(nameof(parentCollection));
            PairEnergySet = pairEnergySet ?? throw new ArgumentNullException(nameof(pairEnergySet));
            ChiralPairEnergySet = FindChiralPartner(pairEnergySet, parentCollection);
            SubscribeToEnergyChanges(ChiralPairEnergySet);
            SetCollection(pairEnergySet.PairEnergyEntries);
            SelectedItem = Items?.FirstOrDefault();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (ChiralPairEnergySet == null) return;
            foreach (var item in ChiralPairEnergySet.PairEnergyEntries) item.PropertyChanged -= RelayPartnerEnergyChange;
        }

        /// <summary>
        ///     Finds the chiral partner <see cref="PairEnergySetData" /> within the provided <see cref="IReadOnlyList{T}" />.
        ///     Returns null if it doesn't exist
        /// </summary>
        /// <param name="original"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        private static PairEnergySetData FindChiralPartner(PairEnergySetData original, IReadOnlyList<PairEnergySetData> collection)
        {
            if (original.ChiralPartnerModelIndex < 0 || original.ChiralPartnerModelIndex >= collection.Count) return null;
            return collection[original.ChiralPartnerModelIndex];
        }

        /// <summary>
        ///     Subscribes to the property change events of all <see cref="PairEnergyData" /> entries in the provided
        ///     <see cref="PairEnergySetData" />
        /// </summary>
        /// <param name="source"></param>
        private void SubscribeToEnergyChanges(PairEnergySetData source)
        {
            if (source == null) return;
            foreach (var energyEntry in ChiralPairEnergySet.PairEnergyEntries) energyEntry.PropertyChanged += RelayPartnerEnergyChange;
        }

        /// <summary>
        ///     Catches the energy change of a chiral partner <see cref="PairEnergyData" /> and copies the value to the managed
        ///     one
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void RelayPartnerEnergyChange(object sender, PropertyChangedEventArgs args)
        {
            static bool PairGraphsAreEqual(PairEnergyData first, PairEnergyData second) =>
                first.CenterParticle.Equals(second.CenterParticle) && first.PartnerParticle.Equals(second.PartnerParticle);

            if (args.PropertyName != nameof(PairEnergyData.Energy)) return;
            if (!(sender is PairEnergyData source)) return;
            var match = PairEnergySet.PairEnergyEntries.FirstOrDefault(x => PairGraphsAreEqual(x, source));
            if (match == null) throw new InvalidOperationException("Relay to chiral partner failed, no match for the source was found.");
            match.Energy = source.Energy;
        }
    }
}