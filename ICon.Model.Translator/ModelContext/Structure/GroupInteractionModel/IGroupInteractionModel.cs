using ICon.Mathematics.ValueTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator.ModelContext
{
    /// <summary>
    /// Represents a group interaction model that combines group energy model with geometric information
    /// </summary>
    public interface IGroupInteractionModel : IModelComponent
    {
        /// <summary>
        /// The list of pair interaction models that form the geometry of the group
        /// </summary>
        IList<IPairInteractionModel> PairInteractionModels { get; set; }

        /// <summary>
        /// The list of group interaction models that are symmetry equivalent to this one
        /// </summary>
        IList<IGroupInteractionModel> EquivalentGroupInteractionModels { get; set; }

        /// <summary>
        /// The environment model the group interaction belongs to
        /// </summary>
        IEnvironmentModel EnvironmentModel { get; set; }

        /// <summary>
        /// The group energy model that the interaction uses
        /// </summary>
        IGroupEnergyModel EnergyModel { get; set; }
    }
}
