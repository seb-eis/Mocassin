using System.Collections.Generic;
using System.Linq;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Transitions
{
    /// <inheritdoc cref="Mocassin.Model.Transitions.IStateExchangeGroup" />
    public class StateExchangeGroup : ModelObject, IStateExchangeGroup
    {
        /// <summary>
        ///     The state exchange pairs affiliated with this state exchange group group
        /// </summary>
        [UseTrackedData]
        public List<IStateExchangePair> StateExchangePairs { get; set; }

        /// <inheritdoc />
        public bool IsVacancyGroup => StateExchangePairs?.Any(x => x.IsVacancyPair) ?? false;

        /// <inheritdoc />
        public bool IsUnstablePositionGroup => StateExchangePairs?.Any(x => x.IsUnstablePositionPair) ?? false;

        /// <inheritdoc />
        public int StatePairCount => StateExchangePairs.Count;

        /// <inheritdoc />
        public override string ObjectName => "State Exchange Group";

        /// <inheritdoc />
        public IEnumerable<IStateExchangePair> GetStateExchangePairs() => (StateExchangePairs ?? new List<IStateExchangePair>()).AsEnumerable();

        /// <inheritdoc />
        public bool Equals(IStateExchangeGroup other)
        {
            if (other == null)
                return false;

            var firstContainsSecond = other.GetStateExchangePairs()
                                           .Select(a => a.Index)
                                           .All(index => StateExchangePairs.Select(a => a.Index).Contains(index));

            var secondContainsFirst = StateExchangePairs
                                      .Select(a => a.Index)
                                      .All(index => other.GetStateExchangePairs().Select(a => a.Index).Contains(index));

            return firstContainsSecond && secondContainsFirst;
        }

        /// <inheritdoc />
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (!(CastIfNotDeprecated<IStateExchangeGroup>(obj) is { } group))
                return null;

            Index = group.Index;
            StateExchangePairs = group.GetStateExchangePairs().ToList();
            return this;
        }
    }
}