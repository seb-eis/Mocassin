using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Base class for all simulation energy model components stored in the interop database
    /// </summary>
    public abstract class EnergyModelComponentBase : InteropEntityBase
    {
        /// <summary>
        ///     The energy model context key
        /// </summary>
        [ForeignKey(nameof(SimulationEnergyModel))]
        public int EnergyModelId { get; set; }

        /// <summary>
        ///     The energy model navigation property
        /// </summary>
        public SimulationEnergyModel SimulationEnergyModel { get; set; }
    }
}