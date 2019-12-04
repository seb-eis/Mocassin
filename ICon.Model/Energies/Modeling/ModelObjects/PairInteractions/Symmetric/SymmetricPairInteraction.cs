using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Energies
{
    /// <inheritdoc cref="ISymmetricPairInteraction" />
    [DataContract]
    public class SymmetricPairInteraction : PairInteraction, ISymmetricPairInteraction
    {
        private SymmetricPairInteraction ChiralPartnerInternal { get; set; }

        /// <summary>
        ///     The symmetric pair energy dictionary that assigns each possible particle pair an energy value
        /// </summary>
        [DataMember]
        public Dictionary<SymmetricParticlePair, double> EnergyDictionary { get; set; }

        /// <inheritdoc />
        [DataMember]
        public override IPairInteraction ChiralPartner => ChiralPartnerInternal;

        /// <inheritdoc />
        public SymmetricPairInteraction()
        {
        }

        /// <inheritdoc />
        public SymmetricPairInteraction(in PairCandidate candidate, Dictionary<SymmetricParticlePair, double> energyDictionary)
            : base(candidate)
        {
            EnergyDictionary = energyDictionary ?? throw new ArgumentNullException(nameof(energyDictionary));
        }

        /// <inheritdoc />
        public IReadOnlyDictionary<SymmetricParticlePair, double> GetEnergyDictionary()
        {
            return EnergyDictionary ?? new Dictionary<SymmetricParticlePair, double>();
        }

		/// <inheritdoc />
		public override string ObjectName => "Symmetric Pair Interaction";

		/// <inheritdoc />
		public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (!(CastIfNotDeprecated<ISymmetricPairInteraction>(obj) is ISymmetricPairInteraction interaction))
                return null;

            base.PopulateFrom(obj);

            EnergyDictionary = new Dictionary<SymmetricParticlePair, double>(interaction.GetEnergyDictionary().Count);
            foreach (var item in interaction.GetEnergyDictionary()) 
                EnergyDictionary.Add(item.Key, item.Value);

            return null;
        }

        /// <inheritdoc />
        public override bool TrySetEnergyEntry(in PairEnergyEntry energyEntry)
        {
            if (!(energyEntry.ParticlePair is SymmetricParticlePair particlePair))
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

        /// <summary>
        ///     Sets the chiral partner <see cref="SymmetricPairInteraction"/>
        /// </summary>
        /// <param name="partner"></param>
        public void SetChiralPartner(SymmetricPairInteraction partner)
        {
            if (partner == null) throw new ArgumentNullException(nameof(partner));
            if (ReferenceEquals(ChiralPartnerInternal, partner)) return;
            ChiralPartnerInternal = partner;
            if (!ReferenceEquals(ChiralPartner, partner.ChiralPartner)) partner.SetChiralPartner(this);
        }
    }
}