using System.Collections.Generic;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Energies
{
    /// <summary>
    ///     Represents a basic environment info that limits range and defines which basic pair interactions should be ignored.
    ///     Valid for all stable environments
    /// </summary>
    public interface IStableEnvironmentInfo : IModelParameter
    {
        /// <summary>
        ///     Get the maximum interaction range for the regular position environments (In internal units)
        /// </summary>
        double MaxInteractionRange { get; }

        /// <summary>
        ///     Get an enumerable for all ignored pair codes that are generally not include during environment sampling
        /// </summary>
        /// <returns></returns>
        IEnumerable<SymmetricParticlePair> GetIgnoredPairs();
    }
}