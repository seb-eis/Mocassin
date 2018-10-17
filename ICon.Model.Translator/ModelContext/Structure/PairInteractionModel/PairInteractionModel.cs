using Mocassin.Mathematics.ValueTypes;
using System.Collections.Generic;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc cref="IPairInteractionModel"/>
    public class PairInteractionModel : ModelComponentBase, IPairInteractionModel
    {
        /// <inheritdoc />
        public int EquivalentModelCount => EquivalentModels.Count;

        /// <inheritdoc />
        public IPairEnergyModel PairEnergyModel { get; set; }

        /// <inheritdoc />
        public IEnvironmentModel EnvironmentModel { get; set; }

        /// <inheritdoc />
        public IList<IPairInteractionModel> EquivalentModels { get; set; }

        /// <inheritdoc />
        public ITargetPositionInfo TargetPositionInfo { get; set; }
    }
}
