using System;
using System.Collections.Generic;
using System.Text;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc />
    public class EnergyModelContext : IEnergyModelContext
    {
        /// <inheritdoc />
        public IList<IPairEnergyModel> PairEnergyModels { get; set; }

        /// <inheritdoc />
        public IList<IGroupEnergyModel> GroupEnergyModels { get; set; }
    }
}
