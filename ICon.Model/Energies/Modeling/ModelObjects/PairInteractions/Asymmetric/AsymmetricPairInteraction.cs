using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Energies
{
    /// <inheritdoc cref="IAsymmetricPairInteraction" />
    [DataContract]
    public class AsymmetricPairInteraction : PairInteraction, IAsymmetricPairInteraction
    {
        /// <summary>
        ///     The polar pair energy dictionary that assigns each possible particle pair an energy value
        /// </summary>
        [DataMember]
        public Dictionary<AsymmetricParticlePair, double> EnergyDictionary { get; set; }

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
		public override string ObjectName => "Asymmetric Pair Interaction";

		/// <inheritdoc />
		public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (!(CastIfNotDeprecated<IAsymmetricPairInteraction>(obj) is IAsymmetricPairInteraction interaction))
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

        /// <inheritdoc />
        public override IEnumerable<PairEnergyEntry> GetEnergyEntries()
        {
            foreach (var item in EnergyDictionary) 
                yield return new PairEnergyEntry(item.Key, item.Value);
        }
    }
}