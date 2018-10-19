using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Transitions
{
    /// <inheritdoc cref="Mocassin.Model.Transitions.IAbstractTransition"/>
    [DataContract(Name = "AbstractTransition")]
    public class AbstractTransition : ModelObject, IAbstractTransition
    {
        /// <inheritdoc />
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        ///     List of affiliated state change group for each step
        /// </summary>
        [DataMember]
        [IndexResolved]
        public List<IStateExchangeGroup> StateExchangeGroups { get; set; }

        /// <summary>
        ///     Connector types for each step
        /// </summary>
        [DataMember]
        public List<ConnectorType> Connectors { get; set; }

        /// <inheritdoc />
        [IgnoreDataMember]
        public int StateCount => StateExchangeGroups?.Count ?? 0;

        /// <inheritdoc />
        [IgnoreDataMember]
        public int ConnectorCount => Connectors?.Count ?? 0;

        /// <inheritdoc />
        [IgnoreDataMember]
        public bool IsMetropolis => StateCount == 2;

        /// <inheritdoc />
        public IEnumerable<ConnectorType> GetConnectorSequence()
        {
            return (Connectors ?? new List<ConnectorType>()).AsEnumerable();
        }

        /// <inheritdoc />
        public IEnumerable<IStateExchangeGroup> GetStateExchangeGroups()
        {
            return (StateExchangeGroups ?? new List<IStateExchangeGroup>()).AsEnumerable();
        }

        /// <inheritdoc />
        public override string GetObjectName()
        {
            return "'Abstract Transition'";
        }

        /// <inheritdoc />
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (!(CastIfNotDeprecated<IAbstractTransition>(obj) is IAbstractTransition transition)) 
                return null;

            Name = transition.Name;
            StateExchangeGroups = transition.GetStateExchangeGroups().ToList();
            Connectors = transition.GetConnectorSequence().ToList();
            return this;

        }

        /// <inheritdoc />
        public bool Equals(IAbstractTransition other)
        {
            if (other == null) 
                return false;

            var indices = StateExchangeGroups.Select(a => a.Index).ToList();
            var otherIndices = other.GetStateExchangeGroups().Select(a => a.Index).ToList();

            if (indices.SequenceEqual(otherIndices) && Connectors.SequenceEqual(other.GetConnectorSequence()))
                return true;

            otherIndices.Reverse();
            return indices.SequenceEqual(otherIndices)
                   && Connectors.SequenceEqual(other.GetConnectorSequence().Reverse());
        }
    }
}