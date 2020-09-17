using System.Collections.Generic;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <summary>
    ///     Represents a pair interaction model that combines geometry information with pair energy model information
    /// </summary>
    public interface IPairInteractionModel : IModelComponent
    {
        /// <summary>
        ///     Get the number of equivalent interaction models that exist
        /// </summary>
        int EquivalentModelCount { get; }

        /// <summary>
        ///     The pair energy model that belongs to the interaction
        /// </summary>
        IPairEnergyModel PairEnergyModel { get; set; }

        /// <summary>
        ///     The environment model the interaction is assigned to
        /// </summary>
        IEnvironmentModel EnvironmentModel { get; set; }

        /// <summary>
        ///     The list of equivalent pair interaction models
        /// </summary>
        IList<IPairInteractionModel> EquivalentModels { get; set; }

        /// <summary>
        ///     The target position information
        /// </summary>
        ITargetPositionInfo TargetPositionInfo { get; set; }
    }
}