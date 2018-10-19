using System.Collections.Generic;

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