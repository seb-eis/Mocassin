using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Energies
{
    /// <inheritdoc cref="ISymmetricPairInteraction" />
    public class SymmetricPairInteraction : PairInteraction, ISymmetricPairInteraction
    {
        private SymmetricPairInteraction ChiralPartnerInternal { get; set; }

        /// <summary>
        ///     The symmetric pair energy dictionary that assigns each possible particle pair an energy value
        /// </summary>
        public Dictionary<SymmetricParticlePair, double> EnergyDictionary { get; set; }

        /// <inheritdoc />
        public override IPairInteraction ChiralPartner => ChiralPartnerInternal;

        /// <inheritdoc />
        public override string ObjectName => "Symmetric Pair Interaction";

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
        public override IEnumerable<PairEnergyEntry> GetEnergyEntries()
        {
            return EnergyDictionary.Select(item => new PairEnergyEntry(item.Key, item.Value));
        }

        /// <inheritdoc />
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (!(CastIfNotDeprecated<ISymmetricPairInteraction>(obj) is { } interaction))
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

        /// <summary>
        ///     Sets the chiral partner <see cref="SymmetricPairInteraction" />
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