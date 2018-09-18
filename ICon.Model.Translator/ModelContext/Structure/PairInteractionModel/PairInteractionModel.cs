using ICon.Mathematics.ValueTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator.ModelContext
{
    /// <summary>
    /// Implementation of a pair interaction model that combines structure information and energy model of a single interacting atom pair
    /// </summary>
    public class PairInteractionModel : ModelComponentBase, IPairInteractionModel
    {
        /// <summary>
        /// The pair energy model that belongs to the interaction
        /// </summary>
        public IPairEnergyModel PairEnergyModel { get; set; }

        /// <summary>
        /// The environment model the interaction belongs to
        /// </summary>
        public IEnvironmentModel EnvironmentModel { get; set; }

        /// <summary>
        /// The relative fractional 3D of the interaction
        /// </summary>
        public Fractional3D RelativeVector3D { get; set; }

        /// <summary>
        /// The relative encoded 4D vector of the interaction
        /// </summary>
        public CrystalVector4D RelativeVector4D { get; set; }
    }
}
