using System.Collections.Generic;
using ICon.Model.Basic;

namespace ICon.Model.Particles
{
    /// <summary>
    ///     Represents a particle cache port that provides extended model data for the particles
    /// </summary>
    public interface IParticleCachePort : IModelCachePort
    {
        /// <summary>
        ///     Get an enumerable sequence of all possible pair codes that can exist in the system
        /// </summary>
        /// <returns></returns>
        IEnumerable<PairCode> GetPossiblePairCodes();
    }
}