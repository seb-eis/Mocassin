using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Base class for all simulation structure model components stored in the interop database
    /// </summary>
    public class StructureModelComponentBase : InteropEntityBase
    {
        /// <summary>
        ///     Static backing field for the structure model component state change actions
        /// </summary>
        private static IList<StateChangeAction> _stateChangeDelegates;

        /// <inheritdoc />
        protected override IList<StateChangeAction> StateChangeActions
        {
            get => _stateChangeDelegates;
            set => _stateChangeDelegates = value;
        }

        /// <summary>
        ///     The structure model context key
        /// </summary>
        [ForeignKey(nameof(StructureModel))]
        public int StructureModelId { get; set; }

        /// <summary>
        ///     The structure model navigation property
        /// </summary>
        public StructureModel StructureModel { get; set; }
    }
}