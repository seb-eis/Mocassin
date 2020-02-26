using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Energies
{
    /// <inheritdoc cref="IUnstablePairInteraction" />
    public class UnstablePairInteraction : PairInteraction, IUnstablePairInteraction
    {
        /// <summary>
        ///     The polar pair energy dictionary that assigns each possible particle pair an energy value
        /// </summary>
        public Dictionary<ParticleInteractionPair, double> EnergyDictionary { get; set; }

        /// <inheritdoc />
        public override IPairInteraction ChiralPartner => null;


        /// <inheritdoc />
        public override string ObjectName => "Asymmetric Pair Interaction";

        /// <inheritdoc />
        public UnstablePairInteraction()
        {
        }

        /// <inheritdoc />
        public UnstablePairInteraction(PairCandidate candidate, Dictionary<ParticleInteractionPair, double> energyDictionary)
            : base(candidate)
        {
            EnergyDictionary = energyDictionary ?? throw new ArgumentNullException(nameof(energyDictionary));
        }

        /// <inheritdoc />
        public IReadOnlyDictionary<ParticleInteractionPair, double> GetEnergyDictionary()
        {
            return EnergyDictionary ?? new Dictionary<ParticleInteractionPair, double>();
        }

        /// <inheritdoc />
        public override IEnumerable<PairEnergyEntry> GetEnergyEntries()
        {
            return EnergyDictionary.Select(item => new PairEnergyEntry(item.Key, item.Value));
        }

        /// <inheritdoc />
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (!(CastIfNotDeprecated<IUnstablePairInteraction>(obj) is { } interaction))
                return null;

            base.PopulateFrom(obj);

            EnergyDictionary = new Dictionary<ParticleInteractionPair, double>(interaction.GetEnergyDictionary().Count);
            foreach (var item in interaction.GetEnergyDictionary())
                EnergyDictionary.Add(item.Key, item.Value);

            return null;
        }

        /// <inheritdoc />
        public override bool TrySetEnergyEntry(PairEnergyEntry energyEntry)
        {
            if (energyEntry == null) throw new ArgumentNullException(nameof(energyEntry));
            if (!EnergyDictionary.ContainsKey(energyEntry.ParticleInteractionPair)) return false;
            EnergyDictionary[energyEntry.ParticleInteractionPair] = energyEntry.Energy;
            return true;
        }
    }
}