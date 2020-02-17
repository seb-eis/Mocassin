using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Energies
{
    /// <inheritdoc cref="IAsymmetricPairInteraction" />
    public class AsymmetricPairInteraction : PairInteraction, IAsymmetricPairInteraction
    {
        /// <summary>
        ///     The polar pair energy dictionary that assigns each possible particle pair an energy value
        /// </summary>
        public Dictionary<AsymmetricParticlePair, double> EnergyDictionary { get; set; }

        /// <inheritdoc />
        public override IPairInteraction ChiralPartner => null;


        /// <inheritdoc />
        public override string ObjectName => "Asymmetric Pair Interaction";

        /// <inheritdoc />
        public AsymmetricPairInteraction()
        {
        }

        /// <inheritdoc />
        public AsymmetricPairInteraction(in PairCandidate candidate, Dictionary<AsymmetricParticlePair, double> energyDictionary)
            : base(candidate)
        {
            EnergyDictionary = energyDictionary ?? throw new ArgumentNullException(nameof(energyDictionary));
        }

        /// <inheritdoc />
        public IReadOnlyDictionary<AsymmetricParticlePair, double> GetEnergyDictionary()
        {
            return EnergyDictionary ?? new Dictionary<AsymmetricParticlePair, double>();
        }

        /// <inheritdoc />
        public override IEnumerable<PairEnergyEntry> GetEnergyEntries()
        {
            return EnergyDictionary.Select(item => new PairEnergyEntry(item.Key, item.Value));
        }

        /// <inheritdoc />
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (!(CastIfNotDeprecated<IAsymmetricPairInteraction>(obj) is { } interaction))
                return null;

            base.PopulateFrom(obj);

            EnergyDictionary = new Dictionary<AsymmetricParticlePair, double>(interaction.GetEnergyDictionary().Count);
            foreach (var item in interaction.GetEnergyDictionary())
                EnergyDictionary.Add(item.Key, item.Value);

            return null;
        }

        /// <inheritdoc />
        public override bool TrySetEnergyEntry(in PairEnergyEntry energyEntry)
        {
            if (!(energyEntry.ParticlePair is AsymmetricParticlePair particlePair))
                return false;

            if (!EnergyDictionary.ContainsKey(particlePair))
                return false;

            EnergyDictionary[particlePair] = energyEntry.Energy;
            return true;
        }
    }
}