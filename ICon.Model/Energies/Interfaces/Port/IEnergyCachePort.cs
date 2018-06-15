using System.Collections.Generic;
using ICon.Mathematics.Constraints;
using ICon.Model.Basic;

namespace ICon.Model.Energies
{
    /// <summary>
    /// Represents an access port for on-demand extended energy data that is automatically cached
    /// </summary>
    public interface IEnergyCachePort : IModelCachePort
    {
        /// <summary>
        /// Get a pair interaction finder that can be used to search the currently linked structure system for symmetric and asymmetric interactions
        /// </summary>
        /// <returns></returns>
        IPairInteractionFinder GetPairInteractionFinder();
    }
}
