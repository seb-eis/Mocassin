using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Mocassin.Model.Transitions;
using Mocassin.UI.Xml.BaseData;

namespace Mocassin.UI.Xml.TransitionData
{
    /// <summary>
    ///     Serializable data object for <see cref="Mocassin.Model.Transitions.IStateExchangeGroup" /> model object creation
    /// </summary>
    [XmlRoot("StateChangeGroup")]
    public class XmlStateExchangeGroup : XmlModelObject
    {
        /// <summary>
        ///     Get or set the list of state exchange pairs contained in the exchange group
        /// </summary>
        [XmlElement("StateChange")]
        public List<XmlStateExchangePair> StateExchangePairs { get; set; }

        /// <inheritdoc />
        protected override ModelObject GetPreparedModelObject()
        {
            var obj = new StateExchangeGroup
            {
                StateExchangePairs = StateExchangePairs.Select(x => x.GetInputObject()).Cast<IStateExchangePair>().ToList()
            };
            return obj;
        }
    }
}