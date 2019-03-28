using System;
using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Mocassin.Model.Structures;
using Mocassin.Model.Transitions;
using Mocassin.UI.Xml.Base;

namespace Mocassin.UI.Xml.TransitionModel
{
    /// <summary>
    ///     Serializable data object for <see cref="Mocassin.Model.Transitions.IMetropolisTransition" /> model object creation
    /// </summary>
    [XmlRoot("MetropolisTransition")]
    public class MetropolisTransitionGraph : ModelObjectGraph, IEquatable<MetropolisTransitionGraph>
    {
        /// <summary>
        ///     Get or set the abstract transition key for the transition logic
        /// </summary>
        [XmlAttribute("Abstract")]
        public string AbstractTransitionKey { get; set; }

        /// <summary>
        ///     Get or set the unit cell position key for first involved wyckoff position
        /// </summary>
        [XmlAttribute("FirstWyckoff")]
        public string FirstUnitCellPositionKey { get; set; }

        /// <summary>
        ///     Get or set the unit cell position key for second involved wyckoff position
        /// </summary>
        [XmlAttribute("SecondWyckoff")]
        public string SecondUnitCellPositionKey { get; set; }

        /// <inheritdoc />
        protected override ModelObject GetModelObjectInternal()
        {
            var obj = new MetropolisTransition
            {
                AbstractTransition = new AbstractTransition {Key = AbstractTransitionKey},
                FirstUnitCellPosition = new UnitCellPosition {Key = FirstUnitCellPositionKey},
                SecondUnitCellPosition = new UnitCellPosition {Key = SecondUnitCellPositionKey}
            };
            return obj;
        }

        /// <inheritdoc />
        public bool Equals(MetropolisTransitionGraph other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (other == null || AbstractTransitionKey != other.AbstractTransitionKey) return false;
            if (HasNullKeys() || other.HasNullKeys()) return false;
            return FirstUnitCellPositionKey == other.FirstUnitCellPositionKey
                   && SecondUnitCellPositionKey == other.SecondUnitCellPositionKey
                   || SecondUnitCellPositionKey == other.FirstUnitCellPositionKey
                   && FirstUnitCellPositionKey == other.SecondUnitCellPositionKey;
        }

        /// <summary>
        ///     Checks if one of the key <see cref="string" /> values is null
        /// </summary>
        /// <returns></returns>
        public bool HasNullKeys()
        {
            return AbstractTransitionKey == null
                   || FirstUnitCellPositionKey == null
                   || SecondUnitCellPositionKey == null;
        }
    }
}