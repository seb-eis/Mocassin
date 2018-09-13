using ICon.Mathematics.ValueTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator.ModelContext
{
    /// <summary>
    /// Describes a pair interaction model that combines geometry information with pair energy model information
    /// </summary>
    public interface IPairInteractionModel : IModelComponent
    {
        /// <summary>
        /// The pair energy model that belongs to the interaction
        /// </summary>
        IPairEnergyModel PairEnergyModel { get; set; }

        /// <summary>
        /// The environment model the interaction belongs to
        /// </summary>
        IEnvironmentModel EnvironmentModel { get; set; }

        /// <summary>
        /// The relative fractional 3D of the interaction
        /// </summary>
        Fractional3D RelativeVector3D { get; set; }

        /// <summary>
        /// The relative encoded 4D vector of the interaction
        /// </summary>
        CrystalVector4D RelativeVector4D { get; set; }
    }
}
