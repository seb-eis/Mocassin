using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator.ModelContext
{
    /// <summary>
    /// Represents an energy model context that provides all refernce energy informations of the project 
    /// </summary>
    public interface IEnergyModelContext
    {
        /// <summary>
        /// The list of existing pair energy models
        /// </summary>
        IList<IPairEnergyModel> PairEnergyModels { get; set; }

        /// <summary>
        /// The list of existing group energy models
        /// </summary>
        IList<IGroupEnergyModel> GroupEnergyModels { get; set; }
    }
}
