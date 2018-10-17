using System.Collections.Generic;
using Mocassin.Model.Energies;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <summary>
    /// Builder for pair energy models that fully describe the energetic properties of pair interactions
    /// </summary>
    public interface IPairEnergyModelBuilder
    {
        /// <summary>
        /// Builds all pair energy models for the passed set of pair interactions
        /// </summary>
        /// <param name="pairInteractions"></param>
        /// <returns></returns>
        IList<IPairEnergyModel> BuildModels(IEnumerable<IPairInteraction> pairInteractions);
    }
}