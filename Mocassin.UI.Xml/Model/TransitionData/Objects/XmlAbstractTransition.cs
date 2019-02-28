using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Mocassin.Model.Transitions;
using Mocassin.UI.Xml.Base;

namespace Mocassin.UI.Xml.TransitionModel
{
    /// <summary>
    ///     Serializable data object for <see cref="Mocassin.Model.Transitions.IAbstractTransition" /> model object creation
    /// </summary>
    [XmlRoot("AbstractTransition")]
    public class XmlAbstractTransition : XmlModelObject
    {
        /// <summary>
        ///     Get the regex for the connector string
        /// </summary>
        private static readonly Regex _connectorRegex = new Regex($"{ConnectorType.Static.ToString()}|{ConnectorType.Dynamic.ToString()}");

        /// <summary>
        ///     Get or set a name for the abstract transition
        /// </summary>
        [XmlAttribute("Name")]
        public string Name { get; set; }

        /// <summary>
        ///     Get or set the association/dissociation flag that enables this behavior
        /// </summary>
        [XmlAttribute("IsAssociation")]
        public bool IsAssociation { get; set; }

        /// <summary>
        ///     Get or set the list of state exchange groups
        /// </summary>
        [XmlArray("StateChangeOptions")]
        [XmlArrayItem("StateChangeGroup")]
        public List<XmlStateExchangeGroup> StateExchangeGroups { get; set; }

        /// <summary>
        ///     Get or set the string that describes the connector sequence
        /// </summary>
        [XmlAttribute("Connection")]
        public string ConnectorString { get; set; }

        /// <inheritdoc />
        protected override ModelObject GetPreparedModelObject()
        {
            var obj = new AbstractTransition
            {
                Name = Name,
                IsAssociation = IsAssociation,
                StateExchangeGroups = StateExchangeGroups.Select(x => x.GetInputObject()).Cast<IStateExchangeGroup>().ToList(),
                Connectors = GetConnectorSequence().ToList()
            };
            return obj;
        }

        /// <summary>
        ///     Translates the currently set connector string into a connector sequence
        /// </summary>
        /// <returns></returns>
        private IEnumerable<ConnectorType> GetConnectorSequence()
        {
            var matches = _connectorRegex.Matches(ConnectorString);
            foreach (Match match in matches)
            {
                if (Enum.TryParse(match.Value, out ConnectorType connector))
                {
                    yield return connector;
                }
            }
        }
    }
}