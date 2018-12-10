using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Base class for all simulation transition model components stored in the interop database
    /// </summary>
    public class TransitionModelComponentBase : InteropEntityBase
    {
        /// <summary>
        ///     Static backing field for the transition model state change actions
        /// </summary>
        private static IList<StateChangeAction> _stateChangeDelegates;

        /// <inheritdoc />
        protected override IList<StateChangeAction> StateChangeActions
        {
            get => _stateChangeDelegates;
            set => _stateChangeDelegates = value;
        }

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