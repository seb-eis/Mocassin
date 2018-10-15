using ICon.Mathematics.ValueTypes;
using System;
using System.Collections.Generic;
using System.Text;
using ICon.Model.Energies;

namespace ICon.Model.Translator.ModelContext
{
    /// <summary>
    /// Represents a group interaction model that combines group energy model with geometric information
    /// </summary>
    public interface IGroupInteractionModel : IModelComponent
    {
        /// <summary>
        /// Get the number of equivalent group interactions
        /// </summary>
        int EquivalentModelCount { get; }

        /// <summary>
        /// The list of pair interaction models that form the geometry of the group
        /// </summary>
        IList<IPairInteractionModel> PairInteractionModels { get; set; }

        /// <summary>
        /// The list of group interaction models that are symmetry equivalent to this one
        /// </summary>
        IList<IGroupInteractionModel> EquivalentModels { get; set; }

        /// <summary>
        /// The environment model the group interaction belongs to
        /// </summary>
        IEnvironmentModel EnvironmentModel { get; set; }

        /// <summary>
        /// The group energy model that the interaction uses
        /// </summary>
        IGroupEnergyModel GroupEnergyModel { get; set; }

        /// <summary>
        /// The position group info that carries extended symmetry info for the group interaction
        /// </summary>
        IPositionGroupInfo PositionGroupInfo { get; set; }

        /// <summary>
        /// The pair index coding array for the simulation. Describes the cluster through pair interaction ids
        /// </summary>
        /// <remarks> Length has to be 8, value of -1 marks end of entries </remarks>
        int[] PairIndexCoding { get; set; }
    }
}
