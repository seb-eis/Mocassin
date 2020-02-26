using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Energies
{
    /// <inheritdoc cref="IStablePairInteraction" />
    public class StablePairInteraction : PairInteraction, IStablePairInteraction
    {
        private StablePairInteraction ChiralPartnerInternal { get; set; }

        /// <summary>
        ///     The symmetric pair energy dictionary that assigns each possible particle pair an energy value
        /// </summary>
        public Dictionary<ParticleInteractionPair, double> EnergyDictionary { get; set; }

        /// <inheritdoc />
        public override IPairInteraction ChiralPartner => ChiralPartnerInternal;

        /// <inheritdoc />
        public override string ObjectName => "Symmetric Pair Interaction";

        /// <inheritdoc />
        public StablePairInteraction()
        {
        }

        /// <inheritdoc />
        public StablePairInteraction(in PairCandidate candidate, Dictionary<ParticleInteractionPair, double> energyDictionary)
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
            if (!(CastIfNotDeprecated<IStablePairInteraction>(obj) is { } interaction))
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

        /// <summary>
        ///     Sets the chiral partner <see cref="StablePairInteraction" />
        /// </summary>
        /// <param name="partner"></param>
        public void SetChiralPartner(StablePairInteraction partner)
        {
            if (partner == null) throw new ArgumentNullException(nameof(partner));
            if (ReferenceEquals(ChiralPartnerInternal, partner)) return;
            ChiralPartnerInternal = partner;
            if (!ReferenceEquals(ChiralPartner, partner.ChiralPartner)) partner.SetChiralPartner(this);
        }
    }
}