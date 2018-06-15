using System.Collections.Generic;

using ICon.Framework.Collections;
using ICon.Mathematics.ValueTypes;

using ICon.Model.Basic;
using ICon.Model.Structures;

namespace ICon.Model.Energies
{
    /// <summary>
    /// Represents an asymmetric pair interaction (directional) that carries an energy dictionary for its permutations
    /// </summary>
    public interface IAsymmetricPairInteraction : IPairInteraction
    {
        /// <summary>
        /// Read only access to the energy dictionary
        /// </summary>
        IReadOnlyDictionary<AsymParticlePair, double> EnergyDictionary { get; }
    }
}
