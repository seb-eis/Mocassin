using ICon.Model.Energies;
using ICon.Model.Particles;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator.ModelContext
{
    /// <summary>
    /// PAir energy model implementation that provides the model data context for a single pair interaction
    /// </summary>
    public class PairEnergyModel : ModelComponentBase, IPairEnergyModel
    {
        /// <summary>
        /// Boolean flag that indictes if the interaction behaves asymetrically
        /// </summary>
        public bool IsAsymmetric => PairInteraction is IAsymmetricPairInteraction;

        /// <summary>
        /// The pair interaction the model is based upon
        /// </summary>
        public IPairInteraction PairInteraction { get; set; }

        /// <summary>
        /// The list of existing energy entries in the pair energy model
        /// </summary>
        public IList<PairEnergyEntry> EnergyEntries { get; set; }

        /// <summary>
        /// The pair energy table that assignes each particle index pair an energy value
        /// </summary>
        public double[,] EnergyTable { get; set; }

        /// <summary>
        /// Create new pair energy model for the provided pair interaction
        /// </summary>
        /// <param name="pairInteraction"></param>
        public PairEnergyModel(IPairInteraction pairInteraction)
        {
            PairInteraction = pairInteraction ?? throw new ArgumentNullException(nameof(pairInteraction));
        }

        /// <summary>
        /// Tries to get an energy value based upon center and interacting particle. Handles asymmetric/asymmetic discrimination
        /// </summary>
        /// <param name="centerParticle"></param>
        /// <param name="other"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get the last particle index in the pair interaction model
        /// </summary>
        public int GetMaxParticleIndex()
        {
            return EnergyTable.GetUpperBound(0);
        }
    }
}
