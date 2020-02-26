using System.Collections.Generic;

namespace Mocassin.Model.Energies
{
    /// <summary>
    ///     Represents a stable to stable interaction between to particles with an energy permutation dictionary
    /// </summary>
    public interface IStablePairInteraction : IPairInteraction
    {
        /// <summary>
        ///     Read only access to the energy dictionary
        /// </summary>
        IReadOnlyDictionary<ParticleInteractionPair, double> GetEnergyDictionary();
    }
}