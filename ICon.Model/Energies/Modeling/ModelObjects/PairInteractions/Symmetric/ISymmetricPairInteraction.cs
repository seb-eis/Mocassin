using System.Collections.Generic;

using ICon.Framework.Collections;
using ICon.Mathematics.ValueTypes;

using ICon.Model.Basic;
using ICon.Model.Structures;

namespace ICon.Model.Energies
{
    /// <summary>
    /// Access interface for the symmetric pair interaction that allows for manipulation of the energy dictionary
    /// </summary>
    public interface ISymmetricPairInteraction : IPairInteraction
    {
        /// <summary>
        /// Read only access to the energy dictionary
        /// </summary>
        IReadOnlyDictionary<SymParticlePair, double> EnergyDictionary { get; }
    }
}
