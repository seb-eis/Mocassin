using System.Collections.Generic;

namespace Mocassin.Model.Energies
{
    /// <summary>
    ///     Represents a unstable to stable interaction between to particles with an energy permutation dictionary
    /// </summary>
    public interface IUnstablePairInteraction : IPairInteraction
    {
        /// <summary>
        ///     Read only access to the energy dictionary
        /// </summary>
        IReadOnlyDictionary<ParticleInteractionPair, double> GetEnergyDictionary();
    }
}