using System.Collections.Generic;
using Mocassin.Model.Energies;
using Mocassin.Model.Particles;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <summary>
    ///     Describes a single pair energy model with energy table and reference geometry information
    /// </summary>
    public interface IPairEnergyModel : IModelComponent
    {
        /// <summary>
        ///     Boolean flag that indicates if the interaction behaves asymmetrically
        /// </summary>
        bool IsAsymmetric { get; }

        /// <summary>
        ///     The pair interaction the model is based upon
        /// </summary>
        IPairInteraction PairInteraction { get; set; }

        /// <summary>
        ///     The list of existing energy entries in the pair energy model
        /// </summary>
        IList<PairEnergyEntry> EnergyEntries { get; set; }

        /// <summary>
        ///     The pair energy table that assigns each particle index pair an energy value
        /// </summary>
        double[,] EnergyTable { get; set; }

        /// <summary>
        ///     Tries to get an energy value based upon a center particle and an interacting one
        /// </summary>
        /// <param name="centerParticle"></param>
        /// <param name="other"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool TryGetEnergy(IParticle centerParticle, IParticle other, out double value);

        /// <summary>
        ///     Get the last particle index in the pair interaction model
        /// </summary>
        int GetMaxParticleIndex();
    }
}