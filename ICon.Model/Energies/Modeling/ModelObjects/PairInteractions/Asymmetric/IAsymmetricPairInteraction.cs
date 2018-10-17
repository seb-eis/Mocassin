using System.Collections.Generic;

namespace Mocassin.Model.Energies
{
    /// <summary>
    ///     Represents an asymmetric pair interaction (directional) that carries an energy dictionary for its permutations
    /// </summary>
    public interface IAsymmetricPairInteraction : IPairInteraction
    {
        /// <summary>
        ///     Read only access to the energy dictionary
        /// </summary>
        IReadOnlyDictionary<AsymmetricParticlePair, double> GetEnergyDictionary();
    }
}