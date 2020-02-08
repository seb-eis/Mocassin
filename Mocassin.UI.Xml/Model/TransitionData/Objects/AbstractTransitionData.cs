using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    [XmlRoot]
    public sealed class AbstractTransitionData : ModelDataObject
    {
        /// <summary>
        ///     Get the regex for the connector string
        /// </summary>
        public static readonly Regex ConnectorRegex = new Regex($"{ConnectorType.Static.ToString()}|{ConnectorType.Dynamic.ToString()}");

        private string connectorString = ConnectorType.Dynamic.ToString();

        private bool isAssociation;

        private ObservableCollection<ModelObjectReference<StateExchangeGroup>> stateExchangeGroups =
            new ObservableCollection<ModelObjectReference<StateExchangeGroup>>();

        /// <summary>
        ///     Get or set the association/dissociation flag that enables this behavior
        /// </summary>
        [XmlAttribute]
        public bool IsAssociation
        {
            get => isAssociation;
            set => SetProperty(ref isAssociation, value);
        }

        /// <summary>
        ///     Get or set the list of state exchange groups
        /// </summary>
        [XmlArray]
        public ObservableCollection<ModelObjectReference<StateExchangeGroup>> StateExchangeGroups
        {
            get => stateExchangeGroups;
            set => SetProperty(ref stateExchangeGroups, value);
        }

        /// <summary>
        ///     Get or set the string that describes the connector sequence
        /// </summary>
        [XmlAttribute]
        public string ConnectorString
        {
            get => connectorString;
            set => SetProperty(ref connectorString, value);
        }

        /// <inheritdoc />
        protected override ModelObject GetModelObjectInternal()
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
            var matches = ConnectorRegex.Matches(ConnectorString);
            foreach (Match match in matches)
            {
                if (Enum.TryParse(match.Value, out ConnectorType connector))
                    yield return connector;
            }
        }
    }
}