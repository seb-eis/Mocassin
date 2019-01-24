using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Mocassin.Model.Structures;
using Mocassin.Model.Transitions;
using Mocassin.UI.Xml.BaseData;

namespace Mocassin.UI.Xml.TransitionData
{
    /// <summary>
    ///     Serializable data object for <see cref="Mocassin.Model.Transitions.IMetropolisTransition" /> model object creation
    /// </summary>
    [XmlRoot("MetropolisTransition")]
    public class XmlMetropolisTransition : XmlModelObject
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
        protected override ModelObject GetPreparedModelObject()
        {
            var obj = new MetropolisTransition
            {
                AbstractTransition = new AbstractTransition {Key = AbstractTransitionKey},
                FirstUnitCellPosition = new UnitCellPosition {Key = FirstUnitCellPositionKey},
                SecondUnitCellPosition = new UnitCellPosition {Key = SecondUnitCellPositionKey}
            };
            return obj;
        }
    }
}