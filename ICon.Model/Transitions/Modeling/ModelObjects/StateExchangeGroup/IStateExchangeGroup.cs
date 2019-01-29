using System;
using System.Collections.Generic;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Transitions
{
    /// <summary>
    ///     Represents a state exchange group of state exchange pairs to describe state changes in transitions
    /// </summary>
    public interface IStateExchangeGroup : IModelObject, IEquatable<IStateExchangeGroup>
    {
        /// <summary>
        ///     The number of state pairs in the group
        /// </summary>
        int StatePairCount { get; }

        /// <summary>
        ///     Flag if the property group is a vacancy group
        /// </summary>
        bool IsVacancyGroup { get; }

        /// <summary>
        /// Flag if the exchange group is only valid for unstable positions
        /// </summary>
        bool IsUnstablePositionGroup { get; }

        /// <summary>
        ///     Get the state exchange pairs forming the exchange group
        /// </summary>
        IEnumerable<IStateExchangePair> GetStateExchangePairs();
    }
}