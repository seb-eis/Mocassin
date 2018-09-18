using System;
using System.Collections.Generic;
using System.Text;
using ICon.Model.ProjectServices;

namespace ICon.Model.Translator.ModelContext
{
    /// <summary>
    /// BAsic implementation of the energy model context that provides a fully extended projection of all energy data of the model
    /// </summary>
    public class EnergyModelContext : IEnergyModelContext
    {
        /// <summary>
        /// The extended list of all pair energy models described by the reference project data
        /// </summary>
        public IList<IPairEnergyModel> PairEnergyModels { get; set; }

        /// <summary>
        /// The extended list of group energy models described by the reference project data
        /// </summary>
        public IList<IGroupEnergyModel> GroupEnergyModels { get; set; }
    }
}
