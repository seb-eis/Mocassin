using System;
using System.Collections.Generic;
using ICon.Model.Basic;


namespace ICon.Model.Transitions
{
    /// <summary>
    /// Represents a property group of donor and acceptor pairs for transitions
    /// </summary>
    public interface IPropertyGroup : IModelObject, IEquatable<IPropertyGroup>
    {
        /// <summary>
        /// The number of state pairs in the group
        /// </summary>
        int StatePairCount { get; }

        /// <summary>
        /// Flag if the property group is a vacancy group
        /// </summary>
        bool VacancyGroup { get; }

        /// <summary>
        /// Get the property state pair indices affiliated with this group
        /// </summary>
        IEnumerable<IPropertyStatePair> GetPropertyStatePairs();
    }
}
