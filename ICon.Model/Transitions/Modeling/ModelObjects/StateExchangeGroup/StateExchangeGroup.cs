using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Transitions
{
    /// <inheritdoc cref="Mocassin.Model.Transitions.IStateExchangeGroup"/>
    [DataContract]
    public class StateExchangeGroup : ModelObject, IStateExchangeGroup
    {
        /// <summary>
        ///     The state exchange pairs affiliated with this state exchange group group
        /// </summary>
        [DataMember]
        [IndexResolved]
        public List<IStateExchangePair> StateExchangePairs { get; set; }

        /// <inheritdoc />
        [DataMember]
        public bool IsVacancyGroup { get; set; }

        /// <inheritdoc />
        [IgnoreDataMember]
        public int StatePairCount => StateExchangePairs.Count;

        /// <inheritdoc />
        public IEnumerable<IStateExchangePair> GetStateExchangePairs()
        {
            return (StateExchangePairs ?? new List<IStateExchangePair>()).AsEnumerable();
        }

        /// <inheritdoc />
        public override string GetObjectName()
        {
            return "Property Group";
        }

        /// <inheritdoc />
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (!(CastIfNotDeprecated<IStateExchangeGroup>(obj) is IStateExchangeGroup group))
                return null;

            Index = group.Index;
            IsVacancyGroup = group.IsVacancyGroup;
            StateExchangePairs = group.GetStateExchangePairs().ToList();
            return this;

        }

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

            return firstContainsSecond || secondContainsFirst;
        }
    }
}