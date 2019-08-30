using System.Collections.Generic;
using Mocassin.Model.Energies;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc />
    public class EnergyModelContext : IEnergyModelContext
    {
        /// <inheritdoc />
        public IList<IPairEnergyModel> PairEnergyModels { get; set; }

        /// <inheritdoc />
        public IList<IGroupEnergyModel> GroupEnergyModels { get; set; }

        /// <inheritdoc />
        public IList<IDefectEnergy> DefectEnergies { get; set; }
    }
}