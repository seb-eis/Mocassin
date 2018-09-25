using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator.ModelContext
{
    /// <inheritdoc cref="ICon.Model.Translator.ModelContext.IGroupInteractionModel"/>
    public class GroupInteractionModel : ModelComponentBase, IGroupInteractionModel
    {
        /// <inheritdoc />
        public IList<IPairInteractionModel> PairInteractionModels { get; set; }

        /// <inheritdoc />
        public IList<IGroupInteractionModel> EquivalentGroupInteractionModels { get; set; }

        /// <inheritdoc />
        public IEnvironmentModel EnvironmentModel { get; set; }

        /// <inheritdoc />
        public IGroupEnergyModel EnergyModel { get; set; }
    }
}
