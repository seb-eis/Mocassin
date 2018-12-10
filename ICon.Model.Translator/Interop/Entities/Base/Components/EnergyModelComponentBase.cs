using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Base class for all simulation energy model components stored in the interop database
    /// </summary>
    public class EnergyModelComponentBase : InteropEntityBase
    {
        /// <summary>
        ///     Static backing field for the state change actions of energy model components
        /// </summary>
        private static IList<StateChangeAction> _stateChangeDelegates;

        /// <inheritdoc />
        protected override IList<StateChangeAction> StateChangeActions
        {
            get => _stateChangeDelegates;
            set => _stateChangeDelegates = value;
        }

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