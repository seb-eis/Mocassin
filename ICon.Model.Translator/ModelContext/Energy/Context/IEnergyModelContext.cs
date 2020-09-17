using System.Collections.Generic;
using Mocassin.Model.Energies;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <summary>
    ///     Represents an energy model context that provides all reference energy information of the project
    /// </summary>
    public interface IEnergyModelContext
    {
        /// <summary>
        ///     The list of existing pair energy models
        /// </summary>
        IList<IPairEnergyModel> PairEnergyModels { get; set; }

        /// <summary>
        ///     The list of existing group energy models
        /// </summary>
        IList<IGroupEnergyModel> GroupEnergyModels { get; set; }

        /// <summary>
        ///     The list of all possible non-zero defect energies
        /// </summary>
        IList<IDefectEnergy> DefectEnergies { get; set; }
    }
}