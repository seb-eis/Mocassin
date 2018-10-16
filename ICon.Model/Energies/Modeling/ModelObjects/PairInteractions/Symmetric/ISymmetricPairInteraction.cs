using System.Collections.Generic;

namespace ICon.Model.Energies
{
    /// <summary>
    ///     Access interface for the symmetric pair interaction that allows for manipulation of the energy dictionary
    /// </summary>
    public interface ISymmetricPairInteraction : IPairInteraction
    {
        /// <summary>
        ///     Read only access to the energy dictionary
        /// </summary>
        IReadOnlyDictionary<SymmetricParticlePair, double> GetEnergyDictionary();
    }
}