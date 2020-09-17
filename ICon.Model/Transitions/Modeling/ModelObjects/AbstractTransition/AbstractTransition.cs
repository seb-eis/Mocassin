using System.Collections.Generic;
using System.Linq;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Transitions
{
    /// <inheritdoc cref="Mocassin.Model.Transitions.IAbstractTransition" />
    public class AbstractTransition : ModelObject, IAbstractTransition
    {
        /// <summary>
        ///     List of affiliated state change group for each step
        /// </summary>
        [UseTrackedData]
        public List<IStateExchangeGroup> StateExchangeGroups { get; set; }

        /// <summary>
        ///     Connector types for each step
        /// </summary>
        public List<ConnectorType> Connectors { get; set; }

        /// <inheritdoc />
        public int StateCount => StateExchangeGroups?.Count ?? 0;

        /// <inheritdoc />
        public int ConnectorCount => Connectors?.Count ?? 0;

        /// <inheritdoc />
        public bool IsMetropolis => StateCount == 2;

        /// <inheritdoc />
        public bool IsAssociation { get; set; }

        /// <inheritdoc />
        public override string ObjectName => "Abstract Transition";

        /// <inheritdoc />
        public IEnumerable<ConnectorType> GetConnectorSequence() => (Connectors ?? new List<ConnectorType>()).AsEnumerable();

        /// <inheritdoc />
        public IEnumerable<IStateExchangeGroup> GetStateExchangeGroups() => (StateExchangeGroups ?? new List<IStateExchangeGroup>()).AsEnumerable();

        /// <inheritdoc />
        public bool Equals(IAbstractTransition other)
        {
            if (other == null)
                return false;

            if (IsAssociation != other.IsAssociation)
                return false;

            var indices = StateExchangeGroups.Select(a => a.Index).ToList();
            var otherIndices = other.GetStateExchangeGroups().Select(a => a.Index).ToList();

            if (indices.SequenceEqual(otherIndices) && Connectors.SequenceEqual(other.GetConnectorSequence()))
                return true;

            otherIndices.Reverse();
            return indices.SequenceEqual(otherIndices)
                   && Connectors.SequenceEqual(other.GetConnectorSequence().Reverse());
        }

        /// <inheritdoc />
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (!(CastIfNotDeprecated<IAbstractTransition>(obj) is { } transition)) return null;

            Name = transition.Name;
            IsAssociation = transition.IsAssociation;
            StateExchangeGroups = transition.GetStateExchangeGroups().ToList();
            Connectors = transition.GetConnectorSequence().ToList();
            return this;
        }
    }
}