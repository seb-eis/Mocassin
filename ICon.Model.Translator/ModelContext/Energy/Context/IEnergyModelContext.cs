using System.Collections.Generic;

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
    }
}