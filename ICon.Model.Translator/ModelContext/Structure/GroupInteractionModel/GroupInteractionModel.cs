using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator.ModelContext
{
    /// <summary>
    /// Group interaction model implemeentation that combines structural and energy model to a single group interaction
    /// </summary>
    public class GroupInteractionModel : ModelComponentBase, IGroupInteractionModel
    {
        /// <summary>
        /// The list of pair interaction models that form the geometry of the group
        /// </summary>
        public IList<IPairInteractionModel> PairInteractionModels { get; set; }

        /// <summary>
        /// The list of group interaction models that are symmetry equivalent to this one
        /// </summary>
        public IList<IGroupInteractionModel> EquivalentGroupInteractionModels { get; set; }

        /// <summary>
        /// The environment model the group interaction belongs to
        /// </summary>
        public IEnvironmentModel EnvironmentModel { get; set; }

        /// <summary>
        /// The group energy model that the interaction uses
        /// </summary>
        public IGroupEnergyModel EnergyModel { get; set; }
    }
}
