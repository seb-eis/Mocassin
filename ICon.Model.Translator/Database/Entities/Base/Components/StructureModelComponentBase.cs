using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Base class for all simulation structure model components stored in the interop database
    /// </summary>
    public abstract class StructureModelComponentBase : InteropEntityBase
    {
        /// <summary>
        ///     The structure model context key
        /// </summary>
        [ForeignKey(nameof(SimulationStructureModel))]
        public int StructureModelId { get; set; }

        /// <summary>
        ///     The structure model navigation property
        /// </summary>
        public SimulationStructureModel SimulationStructureModel { get; set; }
    }
}