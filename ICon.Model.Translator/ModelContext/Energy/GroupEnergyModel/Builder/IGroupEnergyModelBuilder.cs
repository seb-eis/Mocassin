using System.Collections.Generic;
using ICon.Model.Energies;

namespace ICon.Model.Translator.ModelContext
{
    /// <summary>
    /// Builder for group energy models that fully describe the energetic properties of a simulation position group
    /// </summary>
    public interface IGroupEnergyModelBuilder
    {
        /// <summary>
        /// Builds all group energy models from te passed list of group interactions
        /// </summary>
        /// <param name="groupInteractions"></param>
        /// <returns></returns>
        IList<IGroupEnergyModel> BuildModels(IList<IGroupInteraction> groupInteractions);
    }
}