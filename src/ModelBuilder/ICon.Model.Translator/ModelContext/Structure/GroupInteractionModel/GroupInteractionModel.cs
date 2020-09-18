using System.Collections.Generic;
using Mocassin.Model.Energies;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc cref="IGroupInteractionModel" />
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
        public IGroupEnergyModel GroupEnergyModel { get; set; }

        /// <inheritdoc />
        public IPositionGroupInfo PositionGroupInfo { get; set; }

        /// <inheritdoc />
        public int[] PairIndexCoding { get; set; }
    }
}