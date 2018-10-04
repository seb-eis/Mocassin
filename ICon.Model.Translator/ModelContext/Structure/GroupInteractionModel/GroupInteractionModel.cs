using System;
using System.Collections.Generic;
using System.Text;
using ICon.Model.Energies;

namespace ICon.Model.Translator.ModelContext
{
    /// <inheritdoc cref="ICon.Model.Translator.ModelContext.IGroupInteractionModel"/>
    public class GroupInteractionModel : ModelComponentBase, IGroupInteractionModel
    {
        /// <inheritdoc />
        public int EquivalentModelCount => EquivalentModels.Count;

        /// <inheritdoc />
        public IList<IPairInteractionModel> PairInteractionModels { get; set; }

        /// <inheritdoc />
        public IList<IGroupInteractionModel> EquivalentModels { get; set; }

        /// <inheritdoc />
        public IEnvironmentModel EnvironmentModel { get; set; }

        /// <inheritdoc />
        public IGroupEnergyModel EnergyModel { get; set; }

        /// <inheritdoc />
        public IPositionGroupInfo PositionGroupInfo { get; set; }

        /// <inheritdoc />
        public int[] PairIndexCoding { get; set; }
    }
}
