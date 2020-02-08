using System.ComponentModel.DataAnnotations.Schema;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Base class for all simulation transition model components stored in the interop database
    /// </summary>
    public abstract class TransitionModelComponentBase : InteropEntityBase
    {
        /// <summary>
        ///     The transition model context key
        /// </summary>
        [ForeignKey(nameof(SimulationTransitionModel))]
        public int TransitionModelId { get; set; }

        /// <summary>
        ///     The transition model navigation property
        /// </summary>
        public SimulationTransitionModel SimulationTransitionModel { get; set; }
    }
}