using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

using ICon.Model.Basic;

namespace ICon.Model.Transitions
{
    /// <summary>
    /// Abstract base description for a transitions that specififes involved position occupations and
    /// </summary>
    [DataContract(Name ="AbstractTransition")]
    public class AbstractTransition : ModelObject, IAbstractTransition
    {
        /// <summary>
        /// The name of the abstract transition
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// List of affiliated state change group for each step
        /// </summary>
        [DataMember]
        [LinkableByIndex]
        public List<IStateExchangeGroup> StateExchangeGroups { get; set; }

        /// <summary>
        /// Connector types for each step
        /// </summary>
        [DataMember]
        public List<ConnectorType> Connectors { get; set; }

        /// <summary>
        /// The number of sttaes of the transition
        /// </summary>
        [IgnoreDataMember]
        public int StateCount => (StateExchangeGroups != null) ? StateExchangeGroups.Count : 0;

        /// <summary>
        /// The number of connectors of the transition
        /// </summary>
        [IgnoreDataMember]
        public int ConnectorCount => (Connectors != null) ? Connectors.Count : 0;

        /// <summary>
        /// Checks if the transition abstract is metropolis only (State count of two has to be metropolis)
        /// </summary>
        [IgnoreDataMember]
        public bool IsMetropolis => StateCount == 2;

        /// <summary>
        /// Get the connector types
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ConnectorType> GetConnectorSequence()
        {
            return (Connectors ?? new List<ConnectorType>()).AsEnumerable();
        }

        /// <summary>
        /// Get the state exchange groups of the transition
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IStateExchangeGroup> GetStateExchangeGroups()
        {
            return (StateExchangeGroups ?? new List<IStateExchangeGroup>()).AsEnumerable();
        }

        /// <summary>
        /// Get the type name of the model object
        /// </summary>
        /// <returns></returns>
        public override string GetModelObjectName()
        {
            return "'Abstract Transition'";
        }

        /// <summary>
        /// Tries to create new abstract transition object from model object interface (Retruns null if wrong type or deprecated)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (CastWithDepricatedCheck<IAbstractTransition>(obj) is var transition)
            {
                Name = transition.Name;
                StateExchangeGroups = transition.GetStateExchangeGroups().ToList();
                Connectors = transition.GetConnectorSequence().ToList();
                return this;
            }
            return null;
        }

        /// <summary>
        /// Compares abstract transition with other abstract transition in terms of property groups and connector steps (Also compares inverted case)
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IAbstractTransition other)
        {
            var indices = StateExchangeGroups.Select(a => a.Index);
            var otherIndices = other.GetStateExchangeGroups().Select(a => a.Index);

            if (indices.SequenceEqual(otherIndices) && Connectors.SequenceEqual(other.GetConnectorSequence()))
            {
                return true;
            }
            if (indices.SequenceEqual(otherIndices.Reverse()) && Connectors.SequenceEqual(other.GetConnectorSequence().Reverse()))
            {
                return true;
            }
            return false;
        }
    }
}
