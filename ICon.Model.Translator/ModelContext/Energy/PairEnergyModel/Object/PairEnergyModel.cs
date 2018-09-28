using ICon.Model.Energies;
using ICon.Model.Particles;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator.ModelContext
{
    /// <inheritdoc cref="ICon.Model.Translator.ModelContext.IPairEnergyModel"/>
    public class PairEnergyModel : ModelComponentBase, IPairEnergyModel
    {
        /// <inheritdoc />
        public bool IsAsymmetric => PairInteraction is IAsymmetricPairInteraction;

        /// <inheritdoc />
        public IPairInteraction PairInteraction { get; set; }

        /// <inheritdoc />
        public IList<PairEnergyEntry> EnergyEntries { get; set; }

        /// <inheritdoc />
        public double[,] EnergyTable { get; set; }

        /// <summary>
        /// Create new pair energy model for the provided pair interaction
        /// </summary>
        /// <param name="pairInteraction"></param>
        public PairEnergyModel(IPairInteraction pairInteraction)
        {
            PairInteraction = pairInteraction ?? throw new ArgumentNullException(nameof(pairInteraction));
        }

        /// <inheritdoc />
        public bool TryGetEnergy(IParticle centerParticle, IParticle other, out double value)
        {
            if (GetMaxParticleIndex() > centerParticle.Index || GetMaxParticleIndex() > other.Index)
            {
                value = default;
                return false;
            }
            value = EnergyTable[centerParticle.Index, other.Index];
            return true;
        }

        /// <inheritdoc />
        public int GetMaxParticleIndex()
        {
            return EnergyTable.GetUpperBound(0);
        }
    }
}
